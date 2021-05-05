using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceLoaderMedpomData;

namespace MedpomService
{
    public interface IPacketCreator
    {
        void StartAway(FilePacket fp);
    }

    public class PacketCreator : IPacketCreator
    {
        private IRepository mybd { get; }
        private ISchemaCheck SchemaCheck { get; }
        private TASKManager<CloserPackParam> listTH { get; } = new TASKManager<CloserPackParam>();


       
        private ILogger Logger;
        public PacketCreator(IRepository mybd, ISchemaCheck SchemaCheck, ILogger Logger)
        {
            this.mybd = mybd;
            this.SchemaCheck = SchemaCheck;
            this.Logger = Logger;
        }


        public void StartAway(FilePacket fp)
        {
            var param = new CloserPackParam { pack = fp };
            var cts = new CancellationTokenSource();
            var task = new Task(()=>
            {
                CloserPack(param, cts.Token);
            });
            listTH.AddTask(task, cts, param);
            task.Start();
        }

      
        private class CloserPackParam
        {
            public FilePacket pack { get; set; }
        }

        private void Delay(int MS)
        {
            var task = Task.Delay(MS);
            task.Wait();
        }
        /// <summary>
        /// Поток на закрытие схемы пакета
        /// </summary>
        /// <param name="_pack">Массив [2]. 1 - пакет(FilePacket) 2 - ожидать ли файлы(bool)</param>
        private void CloserPack(CloserPackParam param, CancellationToken cancel)
        {
            var pack = param.pack;
            try
            {
                try
                {
                    pack.CaptionMO = mybd.GetNameLPU(pack.CodeMO);
                }
                catch (Exception ex)
                {
                    pack.CaptionMO = ex.Message;
                }

                var sec = 0;
                while (sec < AppConfig.Property.TimePacketOpen && pack.StopTime != true)
                {
                    sec++;
                    pack.Comment = $"Обработка пакета: Ожидание файлов {AppConfig.Property.TimePacketOpen - sec} сек";
                    pack.CommentSite = $"Формирование пакета {AppConfig.Property.TimePacketOpen - sec} сек";
                    Delay(1000);
                }

                pack.Status = StatusFilePack.Close;

                pack.Comment = "Обработка пакета: Перемещение файлов";
                pack.CommentSite = "Перенос файлов";
                //Перемещение файлов в рабочий каталог
                var catalog = GetCatalogPath(pack);
                var catalogSIGN = Path.Combine(catalog, "SIGN");

                if (Directory.Exists(catalog))
                {
                    var t = true;
                    while (t)
                    {
                        try
                        {
                            Directory.Delete(catalog, true);
                            t = false;
                        }
                        catch (Exception ex)
                        {
                            pack.Comment = $"Каталог {Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO)} занят.({ex.Message})";
                            pack.CommentSite = "Рабочий каталог занят... Ожидание освобождения";
                            Delay(5000);
                        }
                    }
                }

                Directory.CreateDirectory(catalog);

                foreach (var item in pack.Files)
                {
                    pack.Comment = $"Обработка пакета: Ожидание доступности файла {item.FileName} (Файл занят другим процессом)";
                    if (!FilesHelper.CheckFileAv(item.FilePach, 3))
                    {
                        item.Comment = "Файл не доступен";
                        item.Process = StepsProcess.FlkErr;
                        continue;
                    }

                    while (item.IsArchive == false && pack.IST == IST.MAIL)
                    {
                        pack.Comment = $"Обработка пакета: Ожидание доступности файла {item.FileName} (Файл переносится в архив)";
                    }

                    pack.Comment = $"Обработка пакета: Перенос файла {item.FileName}";
                    if (!FilesHelper.MoveFile(item, catalog, 3))
                    {
                        item.Process = StepsProcess.FlkErr;
                        continue;
                    }

                    item.FilePach = Path.Combine(catalog, item.FileName);
                    item.FileLog = new LogFile(Path.Combine(Path.GetDirectoryName(item.FilePach), Path.GetFileNameWithoutExtension(item.FilePach)) + ".log");

                    //Перенос Л файла
                    if (item.filel != null)
                    {
                        pack.Comment = $"Обработка пакета: Ожидание доступности файла {item.filel.FileName} (Файл занят другим процессом)";
                        if (!FilesHelper.CheckFileAv(item.filel.FilePach, 3))
                        {
                            item.filel.Comment = "Файл не доступен";
                            item.filel.Process = StepsProcess.FlkErr;
                            continue;
                        }

                        pack.Comment = $"Обработка пакета: Перенос файла {item.filel.FileName}";
                        if (!FilesHelper.MoveFile(item.filel, catalog, 3))
                        {
                            item.filel.Process = StepsProcess.FlkErr;
                            continue;
                        }

                        item.filel.FilePach = Path.Combine(catalog, item.filel.FileName);
                        item.filel.FileLog = new LogFile(Path.Combine(Path.GetDirectoryName(item.filel.FilePach), Path.GetFileNameWithoutExtension(item.filel.FilePach)) + ".log");
                    }

                    try
                    {
                        CreateSIGN(item, catalogSIGN);
                        if (item.filel != null)
                            CreateSIGN(item.filel, catalogSIGN);
                    }
                    catch (Exception ex1)
                    {
                        Logger.AddLog($"Ошибка сохранения подписи: {ex1.Message} для {item.FileName}", LogType.Error);
                    }
                }
                pack.CloserLogFiles();
                //Проверка схемы
                SchemaCheck.StartCheck(pack);
            }
            catch (Exception ex)
            {
                pack.CommentSite = "Что то пошло не так...";
                pack.Comment = ex.Message;
                Logger.AddLog($"Ошибка при проверке схемы или закрытии пакета: {ex.Message}", LogType.Error);
                pack.Comment = $"Ошибка при проверке схемы или закрытии пакета: {ex.Message}";
            }
            finally
            {
                if(Task.CurrentId.HasValue)
                    listTH.RemoveTask(Task.CurrentId.Value);
            }
        }

        public void CreateSIGN(FileItemBase item, string catalogSIGN)
        {
            if (!string.IsNullOrEmpty(item.SIGN_BUH))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN, $"{Path.GetFileNameWithoutExtension(item.FilePach)}.BUH.SIG")))
                {
                    steam.Write(item.SIGN_BUH);
                    steam.Close();
                }
            }

            if (!string.IsNullOrEmpty(item.SIGN_ISP))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN, $"{Path.GetFileNameWithoutExtension(item.FilePach)}.ISP.SIG")))
                {
                    steam.Write(item.SIGN_ISP);
                    steam.Close();
                }
            }

            if (!string.IsNullOrEmpty(item.SIGN_DIR))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN, $"{Path.GetFileNameWithoutExtension(item.FilePach)}.DIR.SIG")))
                {
                    steam.Write(item.SIGN_DIR);
                    steam.Close();
                }
            }
        }
        string GetCatalogPath(FilePacket pack)
        {
            return Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO);
        }
    }
}

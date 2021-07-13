using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MedpomService;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;

namespace MedpomService
{
  
    public interface ISchemaCheck
    {
        void StartCheck(FilePacket fp);
        void StartReCheck(FilePacket fp);
        void Break(FilePacket fp);
        void SetSchemaCollection(SchemaCollection sc);
        void SaveSchemaCollection(string path);
        void LoadSchemaCollection(string path);
        SchemaCollection GetSchemaCollection();

    }

    public class SchemaCheck : ISchemaCheck
    {
        private IRepository mybd { get; set; }
        private IMessageMO messageMo { get; set; }
        private IExcelProtokol excelProtokol { get; set; }
        SchemaCollection SC { get; set; } = new SchemaCollection();
       
        string SvodFileNameXLS = "FileStat.xlsx";
        private ILogger Logger;

        public SchemaCheck(IRepository mybd, IMessageMO messageMo, IExcelProtokol excelProtokol, ILogger Logger)
        {
            this.mybd = mybd;
            this.messageMo = messageMo;
            this.excelProtokol = excelProtokol;
            this.Logger = Logger;
        }
    
        private TASKManager<CheckPackParam> taskList = new TASKManager<CheckPackParam>();

        public void StartCheck(FilePacket fp)
        {
           var param = new CheckPackParam {isResetLog = false, pack = fp};
           var cts = new CancellationTokenSource();
           var task = new Task(() => CheckPack(param, cts.Token));
           taskList.AddTask(task, cts, param);
           task.Start();
        }

        public void StartReCheck(FilePacket fp)
        {
            ClearCatalog(fp);
            var param = new CheckPackParam { isResetLog = true, pack = fp };
            var cts = new CancellationTokenSource();
            var task = new Task(() => CheckPack(param, cts.Token));
            taskList.AddTask(task, cts, param);
            task.Start();
        }

        private void ClearCatalog(FilePacket fp)
        {
            try
            {
                fp.Comment = "Очистка каталога перед проверкой";
                if (!string.IsNullOrEmpty(fp.PATH_STAT))
                {
                    if (File.Exists(fp.PATH_STAT))
                    {
                        File.Delete(fp.PATH_STAT);
                    }

                    fp.PATH_STAT = "";
                }

                if (!string.IsNullOrEmpty(fp.PATH_ZIP))
                {
                    if (File.Exists(fp.PATH_ZIP))
                    {
                        File.Delete(fp.PATH_ZIP);
                    }

                    fp.PATH_STAT = "";
                }

                var fileFLK = Directory.GetFiles(GetCatalogPath(fp), "*FLK.xml");
                foreach (var str in fileFLK)
                {
                    if (File.Exists(str))
                    {
                        File.Delete(str);
                    }
                }

                foreach (var f in fp.Files)
                {
                    f.Comment = "";
                    if (f.filel == null) continue;
                    if (f.Process != StepsProcess.NotInvite)
                    {
                        f.Process = StepsProcess.Invite;
                        f.filel.Process = StepsProcess.Invite;
                    }

                    f.filel.Comment = "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в ClearCatalog: {ex.Message}", ex);
            }
        }

        public void  Break(FilePacket fp)
        {
            var item = taskList.Get().FirstOrDefault(x => x.Param.pack == fp);
            item?.CTS?.Cancel();
        }

        public void SetSchemaCollection(SchemaCollection sc)
        {
            SC = sc;
        }

        public void SaveSchemaCollection(string path)
        {
            SC.SaveToFile(path);
        }

        public void LoadSchemaCollection(string path)
        {
            if (File.Exists(path))
            {
                if (!SC.LoadFromFile(path))
                {
                    Logger.AddLog($"Не удалось прочитать {path}", LogType.Error);
                }
                else
                {
                    Logger.AddLog($"Файл XML-схем загружен :{path}", LogType.Information);
                }
            }
            else
            {
                Logger.AddLog($"Файл {path} не найден. Создан новый экземпляр", LogType.Error);
            }
        }

        public SchemaCollection GetSchemaCollection()
        {
            return SC;
        }


        class CheckPackParam
        {
            public FilePacket pack { get; set; }
            public bool isResetLog { get; set; }
        }


     

        private void CheckPack(CheckPackParam param, CancellationToken cancel)
        {
            var pack = param.pack;
            var isResetLog = param.isResetLog;
            try
            {
                if (isResetLog)
                    pack.ResetLogFiles();
                else
                    pack.OpenLogFiles();
                //проверка на файл L
                FindFileL(pack);
                //Проверка на схему
                pack.Comment = "Обработка пакета: Проверка схемы";
                pack.CommentSite = "Проверка схемы документов";
                //Проверка на файл  H

                if (pack.Files.All(x => x.Type != FileType.H))
                    pack.WARNNING = "Отсутствует файл H";



                foreach (var item in pack.Files)
                {
                    cancel.ThrowIfCancellationRequested();
                    if (item.Process == StepsProcess.NotInvite || item.Process == StepsProcess.FlkErr) continue;
                    var vers_file = ReadVersion(item);
                    var vers_file_l = ReadVersion(item.filel);
                    var dt_file = ReadDateFile(item);

                    item.Version = VersionMP.NONE;
                    item.filel.Version = VersionMP.NONE;


                    var FileXSD = FindXSD(SC, vers_file, dt_file, item);
                    var LFileXSD = FindXSD(SC, vers_file_l, dt_file, item.filel);


                    pack.Comment = $"Обработка пакета: Проверка схемы файла {item.FileName}";
                    CheckXSD(item, FileXSD);
                    pack.Comment = $"Обработка пакета: Проверка схемы файла {item.filel.FileName}";
                    CheckXSD(item.filel, LFileXSD);
                    var IsMainErr = item.Process == StepsProcess.ErrorXMLxsd;
                    var IsLErr = item.filel.Process == StepsProcess.ErrorXMLxsd;

                    if (IsMainErr)
                    {
                        item.filel.Process = StepsProcess.ErrorXMLxsd;
                        item.filel.CommentAndLog = $"Ошибка: Файл владелец({item.FileName}) содержит ошибки в дальнейшей обработке отказано";
                    }
                    if (IsLErr)
                    {
                        item.Process = StepsProcess.ErrorXMLxsd;
                        item.CommentAndLog = $"Ошибка: Файл персональных данных({item.filel.FileName}) содержит ошибки в дальнейшей обработке отказано";
                    }
                }

                cancel.ThrowIfCancellationRequested();
                //Проверка на уникальность кода внутри пакета
                try
                {
                    Check_code_FilePack(pack);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка проверки уникальности кода: {ex.Message}", ex);
                }

                //Проверка уникальности файла
                cancel.ThrowIfCancellationRequested();
                foreach (var fi in pack.Files)
                {
                    try
                    {
                        if (fi.Process != StepsProcess.XMLxsd) continue;
                        pack.Comment = "Обработка пакета: Проверка уникальности файла";
                        if (!CheckNameFile(pack.CodeMO, fi, true, mybd))
                        {
                            fi.Process = StepsProcess.FlkErr;
                            fi.FileLog.WriteLn("Ошибка проверки имен файла. В приеме файла отказано полностью!!!");
                            fi.filel?.FileLog.WriteLn("Ошибка проверки имен файла. В приеме файла отказано полностью!!!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка при проверке уникальности файла: {ex.Message}", ex);
                    }
                }

                pack.Status = StatusFilePack.XMLSchemaOK;
                pack.Comment = "Обработка пакета: Ожидание ФЛК";
                pack.CommentSite = "Ожидание проверки";
                if (pack.Files.Count(x => x.Process == StepsProcess.XMLxsd && x.filel.Process == StepsProcess.XMLxsd) == 0)
                {
                    //Закрываем все и фурмируем ошибки
                    pack.Status = StatusFilePack.FLKOK;
                    pack.CloserLogFiles();
                    //Формируем сводный файл
                    excelProtokol.CreateExcelSvod(pack, Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO, SvodFileNameXLS), null, null, null);
                    messageMo.CreateErrorMessage(pack);
                    pack.Comment = "Обработка пакета: Завершено";
                    pack.CommentSite = "Завершено";
                }
            }
            catch (OperationCanceledException)
            {
                pack.Status = StatusFilePack.FLKERR;
                pack.CommentSite = "Отмена обработки пользователем";
                pack.Comment = "Ошибка проверки схемы: Отмена обработки пользователем";
            }
            catch (Exception ex)
            {
                pack.Status = StatusFilePack.FLKERR;
                pack.CommentSite = "Что то пошло не так...";
                pack.Comment = $"Ошибка проверки схемы: {ex.Message}";
            }
            finally
            {
                pack.CloserLogFiles();
                if(Task.CurrentId.HasValue)
                    taskList.RemoveTask(Task.CurrentId.Value);
            }
        }

        private string ReadVersion(FileItemBase item)
        {
            try
            {
                return SchemaChecking.GetELEMENT(item.FilePach, "VERSION");
            }
            catch (Exception ex)
            {
                item.Process = StepsProcess.ErrorXMLxsd;
                item.FileLog.WriteLn($"Ошибка: Не удалось прочитать версию док-та:{ex.Message}");
                item.Comment = $"Ошибка: Не удалось прочитать версию док-та:{ex.Message}";
            }
            return null;
        }

        private DateTime ReadDateFile(FileItem item)
        {
            try
            {
                var el = SchemaChecking.GetELEMENTs(item.FilePach, "YEAR", "MONTH");
                return new DateTime(Convert.ToInt32(el["YEAR"]), Convert.ToInt32(el["MONTH"]), 1);
            }
            catch (Exception ex)
            {
                item.Process = StepsProcess.ErrorXMLxsd;
                item.FileLog.WriteLn($"Ошибка: Не удалось прочитать отчетный период док-та:{ex.Message}");
                item.Comment = $"Ошибка: Не удалось прочитать отчетный период док-та:{ex.Message}";
            }
            return DateTime.Now;
        }

        private bool CheckXSD(FileItemBase item, string pathXSD)
        {
            if (!string.IsNullOrEmpty(pathXSD))
            {
                var scchek = new SchemaChecking();
                if (scchek.CheckSchema(item, pathXSD))
                {
                    item.Process = StepsProcess.XMLxsd;
                    return true;
                }
                item.Process = StepsProcess.ErrorXMLxsd;
                item.CommentAndLog = "Ошибка: Файл не соответствует схеме";
                return false;
            }
            return false;
        }

        private string FindXSD(SchemaCollection sc, string version, DateTime dt, FileItemBase item)
        {
            if (!item.Type.HasValue)
            {
                item.Process = StepsProcess.ErrorXMLxsd;
                item.CommentAndLog = $"Не указан тип файла";
                return null;
            }
              
            var sc_file = sc.FindSchema(version, dt, item.Type.Value);
            if (sc_file.Result)
            {
                item.Version = sc_file.Vers;
                return sc_file.Value.Value;
            }

            item.Process = StepsProcess.ErrorXMLxsd;
            item.CommentAndLog = $"Недопустимая версия документа: {sc_file.Exception}";
            return null;

        }
        private void FindFileL(FilePacket pack)
        {

            for (var i = 0; i < pack.Files.Count; i++)
            {
                var fi = pack.Files[i];
                if (fi.filel != null) continue;
                var findfile = fi.FileName;
                switch (fi.Type)
                {
                    case FileType.DD:
                    case FileType.DF:
                    case FileType.DO:
                    case FileType.DP:
                    case FileType.DR:
                    case FileType.DS:
                    case FileType.DU:
                    case FileType.DV:
                    case FileType.DA:
                    case FileType.DB:
                    case FileType.H:
                        findfile = $"L{findfile.Remove(0, 1)}";
                        break;
                    case FileType.T:
                    case FileType.C:
                        findfile = $"L{findfile}";
                        break;
                    default:
                        continue;
                }

                var x = pack.Files.FindIndex(F => F.FileName == findfile);
                if (x != -1)
                {
                    fi.Process = StepsProcess.Invite;
                    fi.FileLog.WriteLn("Контроль: Файл персональных данных присутствует");
                    pack.Files[x].Process = StepsProcess.Invite;
                    var h = new FileL
                    {
                        Process = StepsProcess.Invite,
                        FileLog = pack.Files[x].FileLog,
                        FileName = pack.Files[x].FileName,
                        FilePach = pack.Files[x].FilePach,
                        DateCreate = pack.Files[x].DateCreate,
                        Type = pack.Files[x].Type,
                        Comment = pack.Files[x].Comment
                    };
                    fi.filel = h;
                    fi.filel.FileLog.WriteLn($"Контроль: Файл владелец присутствует ({fi.FileName})");
                    pack.Files.Remove(pack.Files[x]);
                    if (x < i) i--;
                }
                else
                {
                    fi.FileLog.WriteLn("Ошибка: Файл персональных данных отсутствует");
                    fi.Comment = ("Ошибка: Файл персональных данных отсутствует");
                }
            }


            foreach (var F in pack.Files)
            {
                if (F.Process == StepsProcess.NotInvite)
                    switch (F.Type)
                    {
                        case FileType.LD:
                        case FileType.LF:
                        case FileType.LO:
                        case FileType.LP:
                        case FileType.LR:
                        case FileType.LS:
                        case FileType.LU:
                        case FileType.LV:
                        case FileType.LH:
                        case FileType.LT:
                        case FileType.LC:
                        case FileType.LA:
                        case FileType.LB:
                            F.FileLog.WriteLn("Ошибка: Файл владелец данных отсутствует");
                            F.Comment = ("Ошибка: Файл владелец данных отсутствует");
                            break;
                        default: continue;
                    }

            }
        }


        private void Check_code_FilePack(FilePacket fp)
        {
            var ListDouble = new Dictionary<string, List<FileItem>>();
            foreach (var item in fp.Files)
            {
                if (item.Process != StepsProcess.XMLxsd)
                    continue;
                var CODE = SchemaChecking.GetELEMENT(item.FilePach, "CODE");

                if (ListDouble.ContainsKey(CODE))
                {
                    ListDouble[CODE].Add(item);
                }
                else
                {
                    ListDouble.Add(CODE, new List<FileItem>());
                    ListDouble[CODE].Add(item);
                }
            }

            foreach (var val in ListDouble)
            {
                if (val.Value.Count <= 1) continue;
                foreach (var fi in val.Value)
                {
                    fi.Comment = "Файл имеет не уникальный CODE";
                    fi.Process = StepsProcess.FlkErr;
                    fi.FileLog.Append();
                    fi.FileLog.WriteLn($"Файл имеет не уникальный CODE = {val.Key}");
                    fi.FileLog.WriteLn($"Файлы с повтором: {string.Join(",", val.Value.Where(x => x != fi).Select(x => x.FileName).ToArray())}");
                    fi.FileLog.Close();
                }
            }
        }

        private bool CheckNameFile(string MO,FileItem FILE, bool checkL, IRepository bd)
        {
            try
            {
                var FILE_L = FILE.filel;
                if (string.IsNullOrEmpty(FILE_L.FileName))
                {
                    FILE_L.CommentAndLog = "Файл не имеет FILENAME";
                    return false;
                }
                if (string.IsNullOrEmpty(FILE.FileName))
                {
                    FILE.CommentAndLog = "Файл не имеет FILENAME";
                    return false;
                }

                if (checkL)
                {
                    var el_l = SchemaChecking.GetELEMENTs(FILE_L.FilePach, "FILENAME", "FILENAME1");
                    var FileName = el_l["FILENAME"];
                    var FileName1 = el_l["FILENAME1"];


                    if (FileName != Path.GetFileNameWithoutExtension(FILE_L.FileName))
                    {
                        FILE_L.CommentAndLog = $"Файл {FILE_L.FileName} имеет не корректный FILENAME = {FileName}";
                        return false;
                    }
                    if (FileName1 != Path.GetFileNameWithoutExtension(FILE.FileName))
                    {
                        FILE_L.CommentAndLog = $"Файл {FILE.FileName} имеет не корректный FILENAME1 = {FileName1}";
                        return false;
                    }
                }
                var el = SchemaChecking.GetELEMENTs(FILE.FilePach, "FILENAME", "CODE", "CODE_MO", "YEAR");
                var FILENAME = el["FILENAME"];
                var CODE = el["CODE"];
                var CODE_MO = el["CODE_MO"];
                var YEAR = el["YEAR"];

                if (FILENAME != Path.GetFileNameWithoutExtension(FILE.FileName))
                {
                    FILE.CommentAndLog = $"Файл {FILE.FileName} имеет не корректный FILENAME = {FILENAME}";
                    return false;
                }

                var ZGLV = bd.GetZGLV_BYFileName(FILENAME);
                if (ZGLV.Count != 0)
                {
                    FILE.CommentAndLog = $"Файл {FILE.FileName} FILENAME = {FILENAME}, который присутствует в предыдущих периодах{Environment.NewLine}{string.Join(Environment.NewLine, ZGLV.Select(zglv => $"Файл {zglv.FILENAME} от {zglv.DSCHET:dd-MM-yyyy}"))}";
                    return false;
                }

                ZGLV = bd.GetZGLV_BYCODE_CODE_MO(Convert.ToInt32(CODE), CODE_MO, Convert.ToInt32(YEAR));
                if (ZGLV.Count != 0)
                {
                    FILE.CommentAndLog = $"Файл имеет код счета который присутствует в предыдущих периодах code = {CODE}{Environment.NewLine}{string.Join(Environment.NewLine, ZGLV.Select(zglv => $"Файл {zglv.FILENAME} code = {zglv.CODE} от {zglv.DSCHET:dd-MM-yyyy}"))}";
                    return false;
                }

                if (MO != CODE_MO)
                {
                    FILE.CommentAndLog ="Код МО указанный в наименовании файла не совпадает с тэгом CODE_MO";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка при проверке на соответствие имен файлов для {FILE.FileName}: {ex.Message} {ex.StackTrace}", LogType.Error);
                return false;
            }
        }

        string GetCatalogPath(FilePacket pack)
        {
            return Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO);
        }
    }
}

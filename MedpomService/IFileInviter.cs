using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ServiceLoaderMedpomData;

namespace MedpomService
{

    public interface IFileInviter
    {
        /// <summary>
        /// Запустить поток приема архива
        /// </summary>
        void StartArchiveInviter();
        /// <summary>
        /// Остановить поток приема архивов
        /// </summary>
        void StopArchiveInviter();
        /// <summary>
        /// Запущен ли поток приема архивов
        /// </summary>
        /// <returns></returns>
        bool isArchiveInviter();
        /// <summary>
        /// Запустить поток приема файлов
        /// </summary>
        void StartFileInviter();
        /// <summary>
        /// Остановить поток приема файлов
        /// </summary>
        void StopFileInviter();
        /// <summary>
        /// Запущен ли поток приема файлов
        /// </summary>
        /// <returns></returns>
        bool isFileInviter();
        /// <summary>
        /// Активировать\деактивировать авто поиск файлов в каталоге
        /// </summary>
        /// <param name="value"></param>
        /// <param name="path"></param>
        void ActivateFileAuto(bool value, string path);
        bool isActivateFileAuto();
        void ToArchive(FilePacket fp);

        void AddFile(string[] Files);
    }
    public class FileInviter : IFileInviter
    {
        private IMessageMO messageMo;
        private readonly IPacketQuery FM;
        private string ErrorPath => Path.Combine(AppConfig.Property.ErrorDir, DateTime.Now.ToString("yyyy_MM_dd"));
        FileSystemWatcher fsw;
        private ILogger Logger;

        
        public FileInviter(IMessageMO messageMo, IPacketQuery FM, ILogger Logger)
        {
            fsw = new FileSystemWatcher() { EnableRaisingEvents = false, Filter = "*.*", IncludeSubdirectories = false, NotifyFilter = NotifyFilters.FileName };
            fsw.Created += fileSystemWatcher_Created;
            this.messageMo = messageMo;
            this.FM = FM;
            this.Logger = Logger;
        }
        private string GetErrorName(MatchParseFileName FP)
        {
            return FP.Ni != null ? $"{FP.Ni} ошибки МП от {DateTime.Now:dd_MM_yyyy HH_mm}" : $"UNKNOWN ошибки МП от {DateTime.Now:dd_MM_yyyy HH_mm}";
        }
        class FileListItem
        {
            public string path { get; set; } = "";
            public bool InArchive { get; set; }
            public DateTime DateIN { get; set; } = DateTime.Now;
        }
        /// <summary>
        ///  Список поступивших файлов
        /// </summary>
        private List<FileListItem> FileList { get; set; } = new List<FileListItem>();
        /// <summary>
        /// Файлы из архива
        /// </summary>
        private List<string> FileFromArchive { get; set; } = new List<string>();
        /// <summary>
        /// Список поступивших архивов
        /// </summary>
        private List<FileListItem> ArchiveFileList { get; set; } = new List<FileListItem>();
        /// <summary>
        /// обработка файла архива ZIP
        /// </summary>
        private void ArchiveInviter()
        {
            CTSThArchive = new CancellationTokenSource();
            var Name = "";
            while (!CTSThArchive.IsCancellationRequested)
            {
                //Даем зазор в 0,5 секунды, бывает что файл 0 байт и открывается в процессе копирования. Вроде доступен с ПО, а потом бах и занят...
                var ArchiveFileListFiller = ArchiveFileList.Where(x => (DateTime.Now - x.DateIN).Milliseconds > 500).ToList();
                if (ArchiveFileListFiller.Count == 0)
                {
                    Thread.Sleep(1500);
                    continue;
                }

                var item = ArchiveFileListFiller[0];
                var FullPath = item.path;
                Name = Path.GetFileName(FullPath);
                ArchiveFileList.Remove(item);
                try
                {
                    PrichinAv? pr;
                    var NOT_FOUND = 0;
                    while (!FilesHelper.CheckFileAv(FullPath, out pr))
                    {

                        if (!pr.HasValue) continue;
                        if (pr.Value != PrichinAv.NOT_FOUND) continue;
                        Logger.AddLog($"Не удалось найти файл {FullPath}", LogType.Error);
                        NOT_FOUND++;
                        if (NOT_FOUND == 3)
                        {
                            throw new Exception($"Не удалось найти файл {FullPath}");
                        }
                        Thread.Sleep(1000);
                    }

                    var fp = ParseFileName.Parse(Name);
                    //Если название  не читаться
                    if (!fp.Success)
                    {
                        var CODE_MO = "UNKNOWN";
                        //Если организация не определена
                        if (fp.Ni == null)
                        {
                            //Ищем в архиве организацию
                            foreach (var entry in FilesHelper.GetFileNameInArchive(FullPath))
                            {
                                var fpentry = ParseFileName.Parse(entry);
                                if (fpentry.Ni == null) continue;
                                CODE_MO = fpentry.Ni;
                                break;
                            }
                        }
                        else
                        {
                            CODE_MO = fp.Ni;
                        }

                        Logger.AddLog($"Наименование файла {Name} не соответствует формату. Файл не принят в обработку!", LogType.Warning);
                        messageMo.CreateMessage($"Наименование файла {Name} не соответствует формату. Файл не принят в обработку!{Environment.NewLine}{fp.ErrText}", Path.Combine(AppConfig.Property.ErrorMessageFile, $"{GetErrorName(fp)}.zip"), FullPath);
                        FilesHelper.MoveFileTo(FullPath, Path.Combine(ErrorPath, CODE_MO, Name));
                    }
                    else
                    {
                        if (fp.Np == "75" && fp.Pp.Value == Penum.T || fp.Np == "75003" && fp.Pp.Value == Penum.S)
                        {
                            //Извлекаем файлы из архива
                            var tmpdir = Directory.CreateDirectory($"{AppConfig.Property.ProcessDir}\\tmp");
                            var t = FilesHelper.FilesExtract(FullPath, tmpdir.FullName);
                            if (t.Result)
                            {
                                var mas = tmpdir.GetFiles();
                                foreach (var fi in mas)
                                {
                                    var name = fi.Name;
                                    while (File.Exists($"{AppConfig.Property.IncomingDir}\\{name}"))
                                    {
                                        name = $"{Path.GetFileNameWithoutExtension(name)}1{Path.GetExtension(name)}";
                                    }
                                    if (name != fi.Name)
                                        Logger.AddLog($"Произошла замена имени файла из {Name} для {fi.Name} на {name}", LogType.Warning);
                                    FileFromArchive.Add(name.ToUpper());
                                    File.Move(fi.FullName, $"{AppConfig.Property.IncomingDir}\\{name}");

                                    if (!AppConfig.Property.AUTO)
                                    {
                                        FileList.Add(new FileListItem { path = $"{AppConfig.Property.IncomingDir}\\{name}", InArchive = true });
                                    }
                                }
                                Directory.Delete(tmpdir.FullName, true);
                                FilesHelper.MoveFileTo(FullPath, Path.Combine(AppConfig.Property.InputDir, "Archive", DateTime.Now.ToString("yyyy_MM_dd"), "MAIL", fp.Ni, Name));
                            }
                            else
                            {
                                messageMo.CreateMessage($"Файл {Name} не читается!!!", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(fp) + ".zip"), FullPath);
                                Logger.AddLog($"Ошибка распаковки файла {Name}: {t.Exception}", LogType.Warning);
                                FilesHelper.MoveFileTo(FullPath, Path.Combine(ErrorPath, fp.Ni, Name));
                            }
                        }
                        else
                        {
                            messageMo.CreateMessage($"В файле {Name} не верно указана организация-получатель", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(fp) + ".zip"), FullPath);
                            Logger.AddLog($"В файле {Name} не верно указана организация-получатель", LogType.Warning);
                            FilesHelper.MoveFileTo(FullPath, Path.Combine(ErrorPath, fp.Ni, Name));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.AddLog($"Ошибка в потоке приема архивов({Name}): {ex.Message}", LogType.Error);
                }
            }
        }

        /// <summary>
        /// Обработка файла XML
        /// </summary>
        private void FilesInviter()
        {
            CTSThFiles = new CancellationTokenSource();
            while (!CTSThFiles.IsCancellationRequested)
            {
                if (FileList.Count == 0)
                {
                    Thread.Sleep(500);
                    continue;
                }
                var Fitem = FileList[0];
                var name = Path.GetFileName(Fitem.path);
                var FullPath = Fitem.path;
                FileList.RemoveAt(0);

                PrichinAv? pr;
                var flagcontinue = false;
                while (!FilesHelper.CheckFileAv(FullPath, out pr))
                {
                    if (pr.HasValue)
                        if (pr.Value == PrichinAv.NOT_FOUND)
                        {
                            Logger.AddLog($"Не удалось найти файл {FullPath}", LogType.Error);
                            flagcontinue = true;
                            break;
                        }
                }
                if (flagcontinue)
                    continue;
                var FP = ParseFileName.Parse(name);
                if (!FP.IsNull)
                {
                    if ((FP.Np == "75" && FP.Pp.Value == Penum.T) || (FP.Np == "75003" && FP.Pp.Value == Penum.S))
                    {
                        var item = new FileItem
                        {
                            FileName = name.ToUpper(),
                            FilePach = FullPath,
                            FileLog = null,
                            Comment = "",
                            Type = FP.FILE_TYPE.ToFileType(),
                            Process = StepsProcess.NotInvite,
                            DateCreate = DateTime.Now,
                            IsArchive = Fitem.InArchive
                        };

                        FM.AddItem(item, FP.Ni);
                        try
                        {
                            if (!item.IsArchive)
                            {
                                FilesHelper.CopyFileTo(FullPath, Path.Combine(AppConfig.Property.InputDir, "Archive", DateTime.Now.ToString("yyyy_MM_dd"), "MAIL", FP.Ni, name));
                                item.IsArchive = true;
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.AddLog($"Ошибка при копировании в архив{FullPath}: {ex.Message}",LogType.Error);
                        }
                    }
                    else
                    {
                        Logger.AddLog($"В файле {name} не верно указана организация-получатель", LogType.Warning);
                        messageMo.CreateMessage($"В файле {name} не верно указана организация-получатель", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(FP) + ".zip"), FullPath);
                        FilesHelper.MoveFileTo(FullPath, Path.Combine(ErrorPath, FP.Ni, name));
                    }
                }
                else
                {
                    try
                    {
                        var CODE_MO = "UNKNOWN";
                        if (FP.Ni != null)
                        {
                            CODE_MO = FP.Ni;
                        }
                        Logger.AddLog($"Имя файла {name} не корректно. Файл не принят в обработку!", LogType.Warning);
                        messageMo.CreateMessage($"Имя файла {name} не корректно. Файл не принят в обработку!{Environment.NewLine}{FP.ErrText}", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(FP) + ".ZIP"), FullPath);
                        FilesHelper.MoveFileTo(FullPath, Path.Combine(ErrorPath, CODE_MO, name));
                    }
                    catch (Exception ex)
                    {
                        Logger.AddLog($"Ошибка при переносе {FullPath}: {ex.Message}", LogType.Error);
                    }
                }
            }

        }

        public async void ToArchive(FilePacket fp)
        {
            await Task.Run(() =>
            {
                //Файлы в архив приема
                foreach (var fi in fp.Files)
                {
                    fp.CommentSite = "Перенос файлов в архив";
                    fp.Comment = "Перенос файлов в архив приема";
                    var name = Path.GetFileName(fi.FilePach);
                    var ID = "NOT_ID";
                    if (fp.ID.HasValue)
                        ID = fp.ID.Value.ToString();
                    var DIR = Path.Combine(AppConfig.Property.InputDir, "Archive", DateTime.Now.ToString("yyyy_MM_dd"), "SITE", fp.CodeMO, ID);
                    FilesHelper.CopyFileTo(fi.FilePach, Path.Combine(DIR, name));
                    name = Path.GetFileName(fi.filel.FilePach);
                    FilesHelper.CopyFileTo(fi.filel.FilePach, Path.Combine(DIR, name));
                }
            });
        }

        public void AddFile(string[] Files)
        {
            foreach (var file in Files)
            {
                TransferFile(file);
            }
        }


        private CancellationTokenSource CTSThArchive;
        private Thread ThArchive;


        public void StartArchiveInviter()
        {
            if(isArchiveInviter())
                throw new Exception("Поток приема архивов уже запущен");
            ThArchive = new Thread(ArchiveInviter) {IsBackground = true};
            ThArchive.Start();
        }

        public void StopArchiveInviter()
        {
            CTSThArchive?.Cancel();
            ArchiveFileList.Clear();
        }

        public bool isArchiveInviter()
        {
            return ThArchive != null && ThArchive.IsAlive;
        }

        private CancellationTokenSource CTSThFiles;
        private Thread ThFiles;
        public void StartFileInviter()
        {
            if (isFileInviter())
                throw new Exception("Поток приема файлов уже запущен");
            ThFiles = new Thread(FilesInviter) { IsBackground = true };
            ThFiles.Start();
        }

        public void StopFileInviter()
        {
            CTSThFiles?.Cancel();
            FileList.Clear();
            FileFromArchive.Clear();
        }

        public bool isFileInviter()
        {
            return ThFiles != null && ThFiles.IsAlive;
        }

        public void ActivateFileAuto(bool value, string path)
        {
            fsw.Path = path;
            fsw.EnableRaisingEvents = value;
        }

        public bool isActivateFileAuto()
        {
            return fsw.EnableRaisingEvents;
        }

        private void fileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            TransferFile(e.FullPath);
        }

        private void TransferFile(string path)
        {
            path = path.ToUpper();
            var file_name = Path.GetFileName(path)?.ToUpper();
            var file_ext = Path.GetExtension(file_name);
            switch (file_ext)
            {
                case ".XML":
                    var fi = new FileListItem {path = path, InArchive = false};
                    if (FileFromArchive.Contains(path.ToUpper()))
                    {
                        fi.InArchive = true;
                        FileFromArchive.Remove(path.ToUpper());
                    }
                    FileList.Add(fi);
                    break;
                case ".ZIP":
                    ArchiveFileList.Add(new FileListItem { path = path });
                    break;
                default:
                    Logger.AddLog($"Файл {file_name} не подлежит обработке(расширение не поддерживается)", LogType.Error);
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceLoaderMedpomData;
using Ionic.Zip;

namespace MedpomService
{
    public partial class WcfInterface
    {
        public List<FilePacket> GetFileManagerList()
        {
            return PacketQuery?.Get();
        }

        public void ClearFileManagerList()
        {
            try
            {
                //Удаление связанных файлов
                foreach (var pack in PacketQuery.Get())
                {
                    if (Directory.Exists(Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO)))
                        Directory.Delete(Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO), true);
                }
                PacketQuery.Clear();
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка при очистке пакетов: {ex.Message}", LogType.Error);
            }
        }


        public BoolResult StopProcess()
        {
            AddLog("Попытка приостановки обработки", LogType.Information);
            var br = new BoolResult();
            try
            {
                AppConfig.Property.FILE_ON = false;
                AppConfig.Save();
                processReestr.StopBDInvite();
                FileInviter.StopFileInviter();
                FileInviter.StopArchiveInviter();
                FileInviter.ActivateFileAuto(false,AppConfig.Property.IncomingDir);
            }
            catch (Exception ex)
            {
                br.Result = false;
                br.Exception = ex.Message;
                return br;
            }
            br.Result = true;
            Logger.AddLog("Обработка остановлена", LogType.Information);
            return br;
        }
        public BoolResult StartProcess(bool MainPriem, bool Auto, DateTime dt)
        {
            AppConfig.Property.FILE_ON = true;
            AppConfig.Property.MainTypePriem = MainPriem;
            AppConfig.Property.OtchetDate = dt;
            AppConfig.Property.AUTO = Auto;
            AppConfig.Save();
            AddLog("Попытка запуска обработки", LogType.Information);
            var br = new BoolResult();
            if (CheckedDir())
            {
                try
                {
                   
                    processReestr.StartBDInvite();
                    FileInviter.StartArchiveInviter();
                    FileInviter.StartFileInviter();
                    FileInviter.ActivateFileAuto(AppConfig.Property.AUTO, AppConfig.Property.IncomingDir);
                }
                catch (Exception ex)
                {
                    AddLog(ex.Message, LogType.Error);
                    br.Exception = ex.Message;
                    br.Result = false;
                    return br;
                }
            }
            else
            {
                br.Exception = "Ошибка при проверке директории. См. лог службы";
                br.Result = false;
                AddLog("Ошибка при проверке директории", LogType.Error);
                return br;
            }
            br.Result = true;
            AddLog("Обработка запущена", LogType.Information);
            return br;
        }

        /// <summary>
        /// Проверка на существование директорий
        /// </summary>
        /// <returns></returns>
        private bool CheckedDir()
        {
            try
            {
                var IncomingDir = CheckDir(AppConfig.Property.IncomingDir);
                var ErrorMessageFile = CheckDir(AppConfig.Property.ErrorMessageFile);
                var ErrorDir = CheckDir(AppConfig.Property.ErrorDir);
                var InputDir = CheckDir(AppConfig.Property.InputDir);
                var ProcessDir = CheckDir(AppConfig.Property.ProcessDir);
                return IncomingDir && ErrorMessageFile && ErrorDir && InputDir && ProcessDir;
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка при проверке директорий: {ex.Message}", LogType.Error);
                return false;
            }
        }

        private bool CheckDir(string dir)
        {
            AddLog($"Проверка {dir}", LogType.Information);
            if (!Directory.Exists(AppConfig.Property.IncomingDir))
            {
                try
                {
                    Directory.CreateDirectory(AppConfig.Property.IncomingDir);
                   
                }
                catch (Exception ex)
                {
                    AddLog(ex.Message, LogType.Error);
                    return false;
                }
            }
            return true;
        }

        public StatusPriem GetStatusInvite()
        {
            return new StatusPriem(
                TypePriem: AppConfig.Property.MainTypePriem, 
                OtchetDate:AppConfig.Property.OtchetDate, 
                AutoPriem: AppConfig.Property.AUTO,
                ActiveAutoPriem: FileInviter.isActivateFileAuto(),
                THArchiveInviter: FileInviter.isArchiveInviter(),
                FilesInviterStatus: FileInviter.isFileInviter(),
                FLKInviterStatus: processReestr.IsBDInvite());
        }

        public void SetAutoPriem(bool Auto)
        {
            AppConfig.Property.AUTO = Auto;
            AppConfig.Save();
            FileInviter.ActivateFileAuto(AppConfig.Property.AUTO, AppConfig.Property.IncomingDir);
        }


        public EntriesMy[] GetEventLogEntry(int count)
        {
            var rez = new List<EntriesMy>();
            try
            {
                if (EventLog.Exists("MedpomServiceLog"))
                {
                    var EventLog1 = new EventLog { Source = "MedpomServiceLog" };

                    for (var i = 0; i < count; i++)
                    {
                        if (i > EventLog1.Entries.Count - 1)
                            continue;
                        var entry = EventLog1.Entries[EventLog1.Entries.Count - 1 - i];
                        var item = new EntriesMy { Message = entry.Message, TimeGenerated = entry.TimeGenerated };
                        switch (entry.EntryType)
                        {
                            case EventLogEntryType.Error: item.Type = TypeEntries.error; break;
                            case EventLogEntryType.Warning: item.Type = TypeEntries.warning; break;
                            default:
                                item.Type = TypeEntries.message; break;
                        }

                        rez.Add(item);
                    }
                }

                return rez.ToArray();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public void ClearEventLogEntry()
        {
            Logger.Clear();
        }

        public bool SetPriority(Guid guid, int priority)
        {
            try
            {
            
                var item = PacketQuery.FindPack(guid);
                if (item != null)
                {
                    item.Priory = priority;
                }
                return true;
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка присвоения приоритета для guid: {guid} => {ex.Message}", LogType.Error);
                return false;
            }
        }

        public bool DelPack(Guid guid)
        {
            try
            {
                var item = PacketQuery.FindPack(guid);
                if (item != null)
                {
                    PacketQuery.DeletePack(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка удаления для guid: {guid.ToString()} => {ex.Message}", LogType.Error);
                return false;
            }
        }


        private ProgressClass progress = new ProgressClass();
        public void SaveProcessArch()
        {
            if (progress != null)
            {
                
                if (progress.Active)
                {
                    throw new FaultException("Операция уже выполняется!!!");
                }
            }
            Task.Run(() => { SaveProgressFolder(); });
        }

        private void SaveProgressFolder()
        {
            try
            {
                progress.Active = true;
                var pathDir = Path.Combine(AppConfig.Property.InputDir, DateTime.Now.Year.ToString(), DateTime.Now.ToString("MMMMMMMMMMMMM"));
                if (!Directory.Exists(pathDir))
                    Directory.CreateDirectory(pathDir);
                var num = 1;
                var path = Path.Combine(pathDir, $"PROCESS{num}.zip");
                while (File.Exists(path))
                {
                    num++;
                    path = Path.Combine(pathDir, $"PROCESS{num}.zip");
                }
                using (var zf = new ZipFile(path, Encoding.GetEncoding("cp866")))
                {
                    zf.AddDirectory(AppConfig.Property.ProcessDir, "PROCESS");
                    zf.SaveProgress += zf_SaveProgress;
                    zf.Save();
                }
                progress.TXT = "Завершено";
            }
            catch (Exception ex)
            {
                progress.TXT = $"Ошибка при архивировании PROCESS: {ex.Message}";
                AddLog($"Ошибка при архивировании PROCESS: {ex.Message}", LogType.Error);
            }
            finally
            {
                progress.Max = 0;
                progress.Value = 0;
                progress.Active = false;
            }
        }
        public ProgressClass GetProgressClassProcessArch()
        {
            return progress;
        }

        void zf_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
                if (e.CurrentEntry != null)
                {
                    progress.Value = e.EntriesSaved;
                    progress.Max = e.EntriesTotal;
                    progress.TXT = e.CurrentEntry.FileName;
                }
            if (e.EventType == ZipProgressEventType.Error_Saving)
            {
                if (e.CurrentEntry != null)
                {
                    progress.TXT = $"Ошибка: {e.CurrentEntry.FileName} {e.CurrentEntry.Comment}";
                }
            }
        }

        public void AddListFile(List<string> List)
        {
            if (!FileInviter.isActivateFileAuto() && FileInviter.isFileInviter())
            {
                FileInviter.AddFile(List.ToArray());
            }
            else
            {
                throw new FaultException("Не возможно добавить файлы в список т.к. не соблюдены условия работы службы: Активный прием в ручном режиме");
            }
        }

    

        public void BreakProcessPac(Guid guid)
        {
            try
            {
                var item = PacketQuery.FindPack(guid);
                if (item != null)
                {
                    Task.Run(() =>
                    {
                        processReestr.Break(item);
                        SchemaCheck.Break(item);
                    });
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
   

        public void RepeatClosePac(Guid[] guid)
        {
            try
            {
                var packs = guid.Select(x=>PacketQuery.FindPack(x)).Where(x=>x!=null).ToList();
                if (packs.Any(pack => pack.Status != StatusFilePack.FLKOK && pack.Status != StatusFilePack.FLKERR))
                {
                    throw new FaultException("Повтор возможен только при статусах: FLKOK,FLKERR");
                }
                Task.Run(() =>
                {
                    foreach (var pack in packs)
                    {
                        pack.Status = StatusFilePack.Close;
                        SchemaCheck.StartReCheck(pack);
                    }
                });
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public delegate void eventRegisterNewFileManager(USER us, bool register);
        public event eventRegisterNewFileManager RaiseRegisterNewFileManager;
        public void RegisterNewFileManager()
        {
            var us = GETUSER;
            if (us != null)
            {
                RaiseRegisterNewFileManager?.Invoke(us, true);
            }
        }
     
        public void UnRegisterNewFileManager()
        {
            var us = GETUSER;
            if (us != null)
            {
                RaiseRegisterNewFileManager?.Invoke(us, false);
            }
        }

    }
}

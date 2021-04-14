using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using System.Threading.Tasks;
using ServiceLoaderMedpomData.EntityMP_V31;
using ExcelManager;
using Ionic.Zip;
using ThreadState = System.Threading.ThreadState;

//Общая концепция
//Мониторим файлы в каталоге после прихода файла посылаем ешл на обработку. На каждый файл свой поток. Работа с файлами проходит в виде пакетов.
//Когда файл прошел схему ставим ему процесс(см. классы) схемаОК. И его подхватит поток переноса в БД. Загружаем все файлы пакета(т.е ЛПУ) в БД. Делаем флк пишем результат

namespace MedpomService
{
    public partial class MedpomService : ServiceBase
    {
        FilesManager FM; //Пакеты
        SchemaColection SC;//Схемы
        //  JournalReception jor;//Журнал приема
        WcfInterface wi;//ВКФ сервис. Взаимодействие с сервером 
        List<ItemThreadList> listTH;//Список потоков которые активны. Нужно удалять поток при поступлении файлов от больницы, в то время как работает еще предыдущий
        private List<FileListItem> ArciveFileList;//Список поступивших архивов
        class FileListItem
        {
            public string path { get; set; } = "";
            public bool InArchive { get; set; }
            public  DateTime DateIN { get; set; } = DateTime.Now;
        }
        private List<FileListItem> FileList;//Список поступивших файлов
        private List<string> FileFromArchive = new List<string>(); //фавйлы из архива
        string PathEXE ;
       
        class ItemThreadList
        {
            public Thread th;
            public FilePacket pac;

        }

        public static string SysLog = "";
        public static string SysPass = "";

        public MedpomService()
        {
            InitializeComponent();
        }

        private string localDir => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

      
        private  CancellationTokenSource cancelSendNewFileManagerTh = new CancellationTokenSource();
        protected override void OnStart(string[] args)
        {
            try
            {
                try
                {
                    var curr_dir = Path.Combine(localDir, "SYS_PWD.txt");
                    if (File.Exists(curr_dir))
                    {
                        var file = File.OpenText(curr_dir);
                        SysLog = file.ReadLine();
                        SysPass = file.ReadLine();
                        file.Close();
                    }
                }
                catch (Exception ex)
                {
                    WcfInterface.AddLog("Ошибка файла SYS_PWD.txt:" + ex.Message, EventLogEntryType.Error);
                }
                try
                {
                    PathEXE = Process.GetCurrentProcess().MainModule?.FileName;
                }
                catch (Exception ex)
                {
                    WcfInterface.AddLog("Ошибка при получении директории запуска " + ex.Message, EventLogEntryType.Error);
                }
                WcfInterface.AddLog("Старт службы " + ServiceName, EventLogEntryType.Information);
                listTH = new List<ItemThreadList>();
                FM = new FilesManager();
                
                try
                {
                    if (File.Exists(Path.Combine(localDir, "FM.dat")))
                    {
                        FM.LoadToFile(Path.Combine(localDir, "FM.dat"));
                    }
                }
                catch (Exception ex)
                {
                    WcfInterface.AddLog("Ошибка при загрузке списка пакетов: " + ex.Message, EventLogEntryType.Error);
                }

                FM.CollectionChanged += FM_CollectionChanged;
                foreach (var FM_item in FM.Get())
                {
                    FM_item.PropertyChanged += SendNewFileManager_PropertyChanged;
                    FM_item.changeSiteStatus += onPackChanged;
                    foreach (var file in FM_item.Files)
                    {
                        file.PropertyChanged += SendNewFileManager_PropertyChanged;
                        if(file.filel!=null)
                            file.filel.PropertyChanged += SendNewFileManager_PropertyChanged;
                    }
                }



                SC = new SchemaColection();
                ArciveFileList = new List<FileListItem>();
                FileList = new List<FileListItem>();

                WcfInterface.AddLog($"Чтение конфигурации XML-схем:{localDir}\\schemaset.dat", EventLogEntryType.Information);
                if (File.Exists($"{localDir}\\schemaset.dat"))
                {
                    if (!SC.LoadFromFile($"{localDir}\\schemaset.dat"))
                    {
                        WcfInterface.AddLog($"Не удалось прочитать {localDir}\\schemaset.dat", EventLogEntryType.Error);
                    }
                    else
                    {
                        WcfInterface.AddLog($"Файл XML-схем загружен :{localDir}\\schemaset.dat", EventLogEntryType.Information);
                    }
                }
                else
                {
                    WcfInterface.AddLog($"Файл {localDir}\\schemaset.dat не найден. Создан новый экземпляр", EventLogEntryType.Error);
                }
                //////////////////////////////////////////////////////

                WcfInterface.AddLog("Запуск сервера WCF", EventLogEntryType.Information);

                if (StartServer())
                    WcfInterface.AddLog("WCF сервер запущен", EventLogEntryType.Information);
                else
                    Stop();
                wi.PathEXE = PathEXE;

                if (AppConfig.Property.FILE_ON)
                    wi.StartProccess(AppConfig.Property.MainTypePriem, AppConfig.Property.AUTO, AppConfig.Property.OtchetDate);

                var th = new Thread(SendNewFileManagerThread);
                th.Start();
            }
            catch(Exception ex)
            {
                WcfInterface.AddLog($"Ошибка запуска службы: {ex.Message}", EventLogEntryType.Error);
                Stop();
            }
        }
        protected override void OnStop()
        {
            try
            {
                FM.SaveToFile(Path.Combine(localDir, "FM.dat"));
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Ошибка при сохранении списка пакетов: {ex.Message}", EventLogEntryType.Error);
            }


            WcfInterface.AddLog("Остановка WCF сервера", EventLogEntryType.Information);
            cancelSendNewFileManagerTh.Cancel();
            WcfConection.Abort();
            WcfInterface.AddLog("Служба остановлена", EventLogEntryType.Information);
        }

        private void FM_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            AddSendNewFileManager();
        }

        private void SendNewFileManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddSendNewFileManager();
        }
      
        private void onPackChanged(FilePacket fp)
        {
            SendNewPackState(fp.codeMOstr);
        }
        public static ServiceHost WcfConection { set; get; }
        private bool StartServer()
        {
            try
            {// @"net.tcp://localhost:12344/TFOMSMEDPOM.svc"
                const string uri = @"net.tcp://localhost:12344/TFOMSMEDPOM.svc"; // Адрес, который будет прослушивать сервер
        

                var netTcpBinding = new NetTcpBinding(SecurityMode.None)
                {
                    ReaderQuotas =
                    {
                        MaxArrayLength = int.MaxValue,
                        MaxBytesPerRead = int.MaxValue,
                        MaxStringContentLength = int.MaxValue
                    },
                    MaxBufferPoolSize = long.MaxValue,
                    MaxReceivedMessageSize = int.MaxValue,
                    MaxBufferSize = int.MaxValue,
                    OpenTimeout = new TimeSpan(24, 0, 0),
                    ReceiveTimeout = new TimeSpan(24, 0, 0),
                    SendTimeout = new TimeSpan(24, 0, 0)
                };


                wi = new WcfInterface();
                WcfConection = new ServiceHost(wi, new Uri(uri));

                // WcfConection = new ServiceHost(wi, new Uri(uri)); // Запускаем прослушивание

                wi.SetFileManager(FM);
                //  wi.SetJornal(jor);
                wi.SetThreadBDinvite(backgroundWorkerBD_DoWork);
                wi.SetWatcher(FilesInviter, fileSystemWatcher1, ArchiveInviter);

                wi.SetSchemaColection(SC);

                wi.SetCheckList(new ChekingList());
                wi.saveListTransfer += wi_saveListTransfer;
                wi.repeatClosePacDel += RepeatCloserPack;
                wi.breackProcess += Wi_breackProcess;
                wi.siteFilePack += Wi_siteFilePack;
                wi.RaiseRegisterNewFileManager += Wi_RaiseRegisterNewFileManager; ;
                wi.RaiseUnRegisterNewFileManager += Wi_RaiseUnRegisterNewFileManager; ;
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LIST_TRANSFER.XML")))
                {
                    wi.LoadTransferList(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LIST_TRANSFER.XML"));
                }


                var myEndpointAdd =new EndpointAddress(new Uri(uri), EndpointIdentity.CreateDnsIdentity("MSERVICE"));

                var ep = WcfConection.AddServiceEndpoint(typeof(IWcfInterface), netTcpBinding, "");

                ep.Address = myEndpointAdd;


                WcfConection.OpenTimeout = new TimeSpan(24, 0, 0);
                WcfConection.CloseTimeout = new TimeSpan(24, 0, 0);
           

                netTcpBinding.Security.Mode = SecurityMode.Message;
                netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;




                WcfConection.Credentials.UserNameAuthentication.UserNamePasswordValidationMode =System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
                WcfConection.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator =new MyCustomUserNameValidator();
                WcfConection.Credentials.ClientCertificate.Authentication.CertificateValidationMode =System.ServiceModel.Security.X509CertificateValidationMode.PeerOrChainTrust;
                WcfConection.Credentials.WindowsAuthentication.AllowAnonymousLogons = true;
                WcfConection.Credentials.WindowsAuthentication.IncludeWindowsGroups = false;

            

                WcfConection.Credentials.ServiceCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My,
                    X509FindType.FindBySubjectName, "MSERVICE");

                //МЕТАДАННЫЕ
               /*
                var smb = WcfConection.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();
                

                smb.HttpGetEnabled = true;
                smb.HttpGetUrl = new Uri(@"HTTP://localhost:12343/TFOMSMEDPOM.svc");
                smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                smb.ExternalMetadataLocation = new Uri(@"HTTP://localhost:12343/TFOMSMEDPOM.svc");
                WcfConection.Description.Behaviors.Add(smb);
                WcfConection.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexTcpBinding(), "mex");
                */

                WcfConection.Authorization.ServiceAuthorizationManager = new MyServiceAuthorizationManager();
                var list1 = new List<System.IdentityModel.Policy.IAuthorizationPolicy> {new AuthorizationPolicy()};
                var list =
                    new System.Collections.ObjectModel.ReadOnlyCollection<
                        System.IdentityModel.Policy.IAuthorizationPolicy>(list1);
                WcfConection.Authorization.ExternalAuthorizationPolicies = list;
                WcfConection.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;



                WcfConection.Open();
            
                wi.addFileFunct += wi_addFileFunct;
                wi.SchemaCollectionOnChanged += Wi_SchemaCollectionOnChanged;

                // WcfInterface.AddLog("Включение имперсонализации для WCF", EventLogEntryType.Information);
                //  wi.SetImpers();
                return true;
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Ошибка при запуске WCF: " + ex.Message, EventLogEntryType.Error);
                return false;
            }
        }

    
        private void Wi_breackProcess(int index)
        {
            var pac = FM[index];
            pac.Status =StatusFilePack.FLKERR;
            pac.Comment = "Прерывание...";
            Task.Factory.StartNew(() => {
                BreakPack(pac);
                pac.Status = StatusFilePack.FLKERR;
                pac.Comment = "Прервано";
            });
        }
        private void Wi_SchemaCollectionOnChanged(SchemaColection sc)
        {
            SC = sc;
        }

        private void Wi_siteFilePack(FilePacket fp)
        {
            AddPacket(fp);
        }

        void wi_saveListTransfer()
        {
            try
            {
                wi.SaveTransferList(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LIST_TRANSFER.XML"));
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Ошибка сохранения LIST_TRANSFER.XML: {ex.Message}", EventLogEntryType.Error);
            }
        }

        void wi_addFileFunct(string File)
        {
            fileSystemWatcher1_Created(null, new FileSystemEventArgs(WatcherChangeTypes.Created, Path.GetDirectoryName(File), Path.GetFileName(File)));
        }
      
        void SaveFilesParam()
        {
            var dir = Path.GetDirectoryName(PathEXE);
            try
            {
                FM.SaveToFile(Path.Combine(dir, "FM.dat"));
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Ошибка при сохранении списка пакетов: " + ex.Message, EventLogEntryType.Error);
            }   
        }
        string GetCatalogPath(FilePacket pack)
        {
            return Path.Combine(AppConfig.Property.ProcessDir, pack.codeMOstr);
        }
        /// <summary>
        /// Поток на закрытие схемы пакета
        /// </summary>
        /// <param name="_pack">Массив [2]. 1 - пакет(FilePacket) 2 - ожидать ли файлы(bool)</param>
        private void CloserPack(object _pack)
        {
            var pack = (FilePacket)((object[])_pack)[0];
            var TimeWait = (bool)((object[])_pack)[1];
            try
            {
                try
                {
                    var name = MYBDOracleNEW.GetNameLPU(pack.codeMOstr,new OracleConnection(AppConfig.Property.ConnectionString));
                    if (name != "") pack.CaptionMO = name;
                }
                catch (Exception ex)
                {
                    pack.CaptionMO = ex.Message;
                }

                var sec = 0;
                if (TimeWait)
                {
                    while (sec < AppConfig.Property.TimePacketOpen && pack.StopTime != true)
                    {
                        sec++;
                        pack.Comment =$"Обработка пакета: Ожидание файлов {AppConfig.Property.TimePacketOpen - sec} сек";
                        pack.CommentSite = $"Формирование пакета {AppConfig.Property.TimePacketOpen - sec} сек";
                        Thread.Sleep(1000);
                    }
                }

                pack.Status = StatusFilePack.Close;
                pack.Comment = "Обработка пакета: Перемещение файлов";
                pack.CommentSite = "Перенос файлов";

                if (TimeWait)
                {
                    //Перемещение файлов в рабочий каталог
                    var catalog = GetCatalogPath(pack);
                    var catalogSIGN = Path.Combine(catalog, "SIGN");
                    try
                    {
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
                                    pack.Comment =$"Каталог {Path.Combine(AppConfig.Property.ProcessDir, pack.codeMOstr)} занят.({ex.Message})";
                                    pack.CommentSite = "Рабочий каталог занят... Ожидание освобождения";
                                    Thread.Sleep(5000);
                                }
                            }
                        }

                        Directory.CreateDirectory(catalog);

                        foreach (var item in pack.Files)
                        {
                            pack.Comment =$"Обработка пакета: Ожидание доступности файла {item.FileName} (Файл занят другим процессом)";
                            if (!CheckFileAv(item.FilePach, 3))
                            {
                                item.Comment = "Файл не доступен";
                                item.Process = StepsProcess.FlkErr;
                                continue;
                            }

                            while (item.IsArchive == false && pack.IST == IST.MAIL)
                            {
                                pack.Comment =$"Обработка пакета: Ожидание доступности файла {item.FileName} (Файл переносится в архив)";
                            }

                            pack.Comment = $"Обработка пакета: Перенос файла {item.FileName}";

                            if (!MoveFile(item, catalog, 3))
                            {
                                item.Process = StepsProcess.FlkErr;
                                continue;
                            }

                            item.FilePach = Path.Combine(catalog, item.FileName);
                            item.FileLog = new LogFile(Path.Combine(Path.GetDirectoryName(item.FilePach),Path.GetFileNameWithoutExtension(item.FilePach)) + ".log");

                            //Перенос Л файла
                            if (item.filel != null)
                            {
                                pack.Comment =
                                    $"Обработка пакета: Ожидание доступности файла {item.filel.FileName} (Файл занят другим процессом)";
                                if (!CheckFileAv(item.filel.FilePach, 3))
                                {
                                    item.filel.Comment = "Файл не доступен";
                                    item.filel.Process = StepsProcess.FlkErr;
                                    continue;
                                }

                                pack.Comment = $"Обработка пакета: Перенос файла {item.filel.FileName}";
                                if (!MoveFile(item.filel, catalog, 3))
                                {
                                    item.filel.Process = StepsProcess.FlkErr;
                                    continue;
                                }

                                item.filel.FilePach = Path.Combine(catalog, item.filel.FileName);
                                item.filel.FileLog = new LogFile(Path.Combine(Path.GetDirectoryName(item.filel.FilePach),Path.GetFileNameWithoutExtension(item.filel.FilePach)) + ".log");
                            }

                            try
                            {
                                CreateSIGN(item, catalogSIGN);
                                if (item.filel != null)
                                    CreateSIGN(item.filel, catalogSIGN);
                            }
                            catch (Exception ex1)
                            {
                                WcfInterface.AddLog($"Ошибка сохранения подписи: {ex1.Message} для {item.FileName}",EventLogEntryType.Error);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        pack.CommentSite = "Что то пошло не так...";
                        pack.Comment = ex.Message;
                        pack.CloserLogFiles();
                        var THitem = listTH.Find(itemThreadList => itemThreadList.pac.CodeMO == pack.CodeMO);
                        if (THitem != null)
                            listTH.Remove(THitem);
                        return;
                    }
                }


                //проверка на файл L
                if (!TimeWait)
                {
                    pack.ResetLogFiles();
                }
                else
                {
                    pack.OpenLogFiles();
                }

                FindFileL(pack);
                //Проверка на схему
                pack.Comment = "Обработка пакета: Проверка схемы";
                pack.CommentSite = "Проверка схемы документов";
                //Проверка на файл  H

                if (pack.Files.All(x => x.Type != FileType.H))
                {
                    pack.WARNNING = "Отсутствует файл H";
                }

                var countNoError = 0;
                foreach (var item in pack.Files)
                {
                    if (item.Process == StepsProcess.NotInvite || item.Process == StepsProcess.FlkErr) continue;
                    var vers_file = "";
                    var dt_file = DateTime.Now;
                    try
                    {
                        vers_file = SchemaChecking.GetCode_fromXML(item.FilePach, "VERSION");
                        var year = SchemaChecking.GetCode_fromXML(item.FilePach, "YEAR");
                        var month = SchemaChecking.GetCode_fromXML(item.FilePach, "MONTH");
                        dt_file = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
                    }
                    catch (Exception ex)
                    {
                        item.Process = StepsProcess.ErrorXMLxsd;
                        item.FileLog.WriteLn($"Ошибка: Не удалось прочитать версию док-та:{ex.Message}");
                        item.Comment = $"Ошибка: Не удалось прочитать версию док-та:{ex.Message}";
                    }

                    var vers_file_l = "";

                    try
                    {
                        vers_file_l = SchemaChecking.GetCode_fromXML(item.filel.FilePach, "VERSION");

                    }
                    catch (Exception ex)
                    {
                        item.filel.Process = StepsProcess.ErrorXMLxsd;
                        item.filel.FileLog.WriteLn($"Ошибка: Не удалось определить версию док-та:{ex.Message}");
                        item.filel.Comment = $"Ошибка: Не удалось определить версию док-та:{ex.Message}";
                    }

                    item.Version = VersionMP.NONE;
                    item.filel.Version = VersionMP.NONE;
                    var sc_file = SC.FindSchema(vers_file, dt_file, item.Type.Value);
                    var sc_filel = SC.FindSchema(vers_file_l, dt_file, item.filel.Type.Value);


                    if (sc_file.Result)
                    {
                        item.Version = sc_file.Vers;
                    }
                    else
                    {
                        item.Process = StepsProcess.ErrorXMLxsd;
                        item.CommentAndLog = $"Недопустимая версия документа: {sc_file.Exception}";
                    }

                    if (sc_filel.Result)
                    {
                        item.filel.Version = sc_filel.Vers;
                    }
                    else
                    {
                        item.filel.Process = StepsProcess.ErrorXMLxsd;
                        item.filel.CommentAndLog = $"Недопустимая версия документа: {sc_filel.Exception}";
                    }

                    var scheck = new SchemaChecking();
                    if (item.Version != VersionMP.NONE)
                    {
                        pack.Comment = $"Обработка пакета: Проверка схемы файла {item.FileName}";
                        if (scheck.CheckSchema(item, sc_file.Value.Value))
                        {
                            item.Process = StepsProcess.XMLxsd;
                            countNoError++;
                        }
                        else
                        {
                            item.Process = StepsProcess.ErrorXMLxsd;
                            item.CommentAndLog = "Ошибка: Файл не соответствует схеме";
                        }
                    }

                    if (item.filel.Version != VersionMP.NONE)
                    {
                        pack.Comment = $"Обработка пакета: Проверка схемы файла {item.filel.FileName}";
                        if (scheck.CheckSchema(item.filel, sc_filel.Value.Value))
                        {
                            if (item.Process == StepsProcess.ErrorXMLxsd)
                            {
                                item.filel.Process = StepsProcess.ErrorXMLxsd;
                                item.filel.CommentAndLog =$"Ошибка: Файл владелец({item.FileName}) содержит ошибки в дальнейшей обработке отказано";
                            }

                            item.filel.Process = StepsProcess.XMLxsd;
                        }
                        else
                        {
                            item.filel.Process = StepsProcess.ErrorXMLxsd;
                            item.filel.CommentAndLog = "Ошибка: Файл не соответствует схеме";
                            if (item.Process == StepsProcess.XMLxsd)
                            {
                                countNoError--;
                                item.Process = StepsProcess.ErrorXMLxsd;
                                item.CommentAndLog =$"Ошибка: Файл персональных данных({item.filel.FileName}) содержит ошибки в дальнейшей обработке отказано";
                            }
                        }
                    }

                    if (item.Version == VersionMP.NONE || item.filel.Version == VersionMP.NONE)
                    {
                        item.Process = StepsProcess.ErrorXMLxsd;
                        item.filel.Process = StepsProcess.ErrorXMLxsd;
                        if (item.filel.Version == VersionMP.NONE)
                        {
                            item.CommentAndLog = "Недопустимая версия документа  файла перс данных!";
                            item.Process = StepsProcess.ErrorXMLxsd;
                        }
                    }
                }

                //Проверка на уникальность кода внутри пакета
                try
                {
                    Check_code_FilePack(pack);
                    GC.Collect();
                }
                catch (Exception ex)
                {
                    WcfInterface.AddLog($"Ошибка проверки уникальности кода: {ex.Message}", EventLogEntryType.Error);
                    pack.CommentSite = "Что то пошло не так...";
                }

                //Проверка уникальности файла
                using (var mybd = CreateMyBD())
                {
                    foreach (var fi in pack.Files)
                    {
                        try
                        {
                            if (fi.Process != StepsProcess.XMLxsd) continue;
                            pack.Comment = "Обработка пакета: Проверка уникальности файла";
                            if (!CheckNameFile(fi, true, mybd))
                            {
                                fi.Process = StepsProcess.FlkErr;
                                fi.FileLog.WriteLn("Ошибка проверки имен файла. В приеме файла отказано полностью!!!");
                                fi.filel?.FileLog.WriteLn("Ошибка проверки имен файла. В приеме файла отказано полностью!!!");
                            }
                        }
                        catch (ThreadAbortException ex)
                        {
                            pack.CloserLogFiles();
                            CreateErrorMessage(pack);
                            pack.Status = StatusFilePack.FLKERR;
                            pack.CommentSite = "Что то пошло не так...";
                            pack.Comment = "Прерывание потока выполнения";
                            WcfInterface.AddLog($"Прерывание потока выполнения {pack.codeMOstr} ({ex.Source}:{ex.Message})", EventLogEntryType.Error);
                            return;
                        }
                        catch (Exception ex)
                        {
                            pack.CloserLogFiles();
                            pack.CommentSite = "Что то пошло не так...";
                            WcfInterface.AddLog($"Ошибка при проверки уникальности файла{fi.FileName}: {ex.Message}", EventLogEntryType.Error);
                        }
                    }
             
                }
                GC.Collect();
                //Закрываем логи
                pack.CloserLogFiles();
                pack.Status = StatusFilePack.XMLSchemaOK;
                pack.Comment = "Обработка пакета: Ожидание ФЛК";
                pack.CommentSite = "Ожидание проверки";
                if (countNoError == 0)
                {
                    //Закрываем все и фурмируем ошибки
                    pack.Status = StatusFilePack.FLKOK;
                    pack.CloserLogFiles();
                    //Формируем сводный файл
                    CreateExcelSvod2(pack, Path.Combine(AppConfig.Property.ProcessDir, pack.codeMOstr, SvodFileNameXLS), null, null,null);
                    CreateErrorMessage(pack);
                    pack.Comment = "Обработка пакета: Завершено";
                    pack.CommentSite = "Завершено";
                    //Сохраняем файлы(журнала и работы) на всякий случай.
                    // SaveFilesParam();
                }

                var THitem1 = listTH.Find(itemThreadList => itemThreadList.pac.CodeMO == pack.CodeMO);
             
                if (THitem1 != null)
                    listTH.Remove(THitem1);
            }
            catch (Exception Ex)
            {
                pack.CloserLogFiles();
                pack.CommentSite = "Что то пошло не так...";
                WcfInterface.AddLog($"Ошибка при проверке схемы или закрытии пакета: {Ex.Message}", EventLogEntryType.Error);
                pack.Comment = $"Ошибка при проверке схемы или закрытии пакета: {Ex.Message}";
                var THitem1 = listTH.Find(itemThreadList => itemThreadList.pac.CodeMO == pack.CodeMO);
                if (THitem1 != null)
                    listTH.Remove(THitem1);

            }

        }

        private bool CheckFileAv(string path,int NOT_FOUND_COUNT = 1)
        {
            var NOT_FOUND = 0;
            SchemaChecking.PrichinAv? pr;
            while (!SchemaChecking.CheckFileAv(path, out pr))
            {
                if (pr == SchemaChecking.PrichinAv.NOT_FOUND)
                {
                    WcfInterface.AddLog($"Не удалось найти файл {path}", EventLogEntryType.Error);
                }
                NOT_FOUND++;
                if (NOT_FOUND >= NOT_FOUND_COUNT)
                {
                    return false;
                }
                Thread.Sleep(1500);
            }
            return true;

        }
        private bool MoveFile(FileItemBase item, string catalog, int NOT_FOUND_COUNT = 1)
        {
            var NOT_FOUND = 0;
            while (true)
            {
                try
                {
                    File.SetAttributes(item.FilePach, FileAttributes.Normal);
                    File.Move(item.FilePach, Path.Combine(catalog, item.FileName));
                    break;
                }
                catch (Exception ex)
                {
                    NOT_FOUND++;
                    item.Comment = $"Обработка пакета: Перенос файла {item.FileName}: {ex.GetType()} {ex.FullError()}";
                    try
                    {
                        if (!Directory.Exists(catalog))
                            Directory.CreateDirectory(catalog);
                    }
                    catch (Exception ex1)
                    {
                        WcfInterface.AddLog($"Ошибка создания директории: {ex1.Message} для {item.FileName}", EventLogEntryType.Error);
                    }
                  
                    if (NOT_FOUND == NOT_FOUND_COUNT)
                    {
                        WcfInterface.AddLog($"Ошибка переноса файла {item.FileName}({item.FilePach}): {ex.GetType()} {ex.FullError()}", EventLogEntryType.Error);
                        item.Comment = $"Обработка пакета: Перенос файла {item.FileName}: {ex.GetType()} {ex.FullError()}";
                        return false;
                    }
                    Thread.Sleep(5000);
                }
            }
            return true;
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
                    case FileType.H:
                        findfile = "L" + findfile.Remove(0, 1);
                        break;
                    case FileType.T:
                    case FileType.C:
                        findfile = "L" + findfile;
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
                            F.FileLog.WriteLn("Ошибка: Файл владелец данных отсутствует");
                            F.Comment = ("Ошибка: Файл владелец данных отсутствует");
                            break;
                        default: continue;
                    }

            }
        }
        public void CreateSIGN(FileItemBase item,string catalogSIGN)
        {
            if (!string.IsNullOrEmpty(item.SIGN_BUH))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN,$"{Path.GetFileNameWithoutExtension(item.FilePach)}.BUH.SIG")))
                {
                    steam.Write(item.SIGN_BUH);
                    steam.Close();
                }
            }

            if (!string.IsNullOrEmpty(item.SIGN_ISP))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN,$"{Path.GetFileNameWithoutExtension(item.FilePach)}.ISP.SIG")))
                {
                    steam.Write(item.SIGN_ISP);
                    steam.Close();
                }
            }

            if (!string.IsNullOrEmpty(item.SIGN_DIR))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN,$"{Path.GetFileNameWithoutExtension(item.FilePach)}.DIR.SIG")))
                {
                    steam.Write(item.SIGN_DIR);
                    steam.Close();
                }
            }
        }
        public void RepeatCloserPack(int[] index)
        {
           var Th = new Thread(RepeatCloserPackThread) {IsBackground = true};
           Th.Start(index);
        }

        void RepeatCloserPackThread(object par)
        {
            try
            {
                var index = (int[])par;
                foreach (var ind in index)
                {
                    var th = new Thread(CloserPack) { IsBackground = true };
                    var itth = new ItemThreadList { pac = FM[ind], th = th };
                    itth.pac.Comment = "Очистка каталога перед проверкой";

                    if (!string.IsNullOrEmpty(itth.pac.PATH_STAT))
                    {
                        if (File.Exists(itth.pac.PATH_STAT))
                        {
                            File.Delete(itth.pac.PATH_STAT);
                        }
                        itth.pac.PATH_STAT = "";
                    }
                    if (!string.IsNullOrEmpty(itth.pac.PATH_ZIP))
                    {
                        if (File.Exists(itth.pac.PATH_ZIP))
                        {
                            File.Delete(itth.pac.PATH_ZIP);
                        }
                        itth.pac.PATH_STAT = "";
                    }
                    var fileFLK = Directory.GetFiles(GetCatalogPath(itth.pac), "*FLK.xml");
                    foreach (var str in fileFLK)
                    {
                        if (File.Exists(str))
                        {
                            File.Delete(str);
                        }
                    }
                    foreach (var f in itth.pac.Files)
                    {
                        f.Comment = "";
                        if (f.filel != null)
                        {
                            if (f.Process != StepsProcess.NotInvite)
                            {
                                f.Process = StepsProcess.Invite;
                                f.filel.Process = StepsProcess.Invite;
                            }
                            f.filel.Comment = "";
                        }
                    }
                    listTH.Add(itth);
                    var obj = new object[] { itth.pac, false };
                    th.Start(obj);
                }
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Ошибка в RepeatCloserPackThread: {ex.Message}", EventLogEntryType.Error);
            }
        }



        //Загрузка в БД от МО
        FilePacket DBinvitePac;

        MYBDOracleNEW CreateMyBD()
        {
            return new MYBDOracleNEW(
                                 AppConfig.Property.ConnectionString,
                                 new TableInfo { TableName = AppConfig.Property.xml_h_zglv, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_schet, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sank, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sank_code_exp, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },

                            

                                 new TableInfo { TableName = AppConfig.Property.xml_h_pacient, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_zap, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_usl, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sluch, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },

                                      new TableInfo { TableName = AppConfig.Property.xml_h_ds2, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_ds3, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_crit, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },

                                 new TableInfo { TableName = AppConfig.Property.xml_h_z_sluch, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_kslp, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_l_zglv, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_l_pers, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.XML_H_DS2_N, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.XML_H_NAZR, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_b_diag, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_b_prot, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_napr, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 
                                 new TableInfo { TableName = AppConfig.Property.xml_h_onk_usl, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_lek_pr, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_date_inj, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_cons, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 
                                 new TableInfo { TableName = AppConfig.Property.xml_errors, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 AppConfig.Property.OtchetDate);


        }

        private MYBDOracleNEW currmyBD;


        private void CheckCancel(CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested) throw new CancelException();
        }
        private void backgroundWorkerBD_DoWork()
        {
            try
            {
                wi.BDinviteCancellationTokenSource = new CancellationTokenSource();
                var cancel = wi.BDinviteCancellationTokenSource.Token;
                while (!cancel.IsCancellationRequested && AppConfig.Property.FILE_ON)
                {
                    var i = FM.GetIndexHighPriority();
                    if (i != -1)
                    {
                        var currentpack = FM[i];
                        try
                        {
                            if (currentpack.Status == StatusFilePack.XMLSchemaOK)
                            {
                                DBinvitePac = currentpack;
                                currentpack.OpenLogFiles();
                                currentpack.Comment = "Обработка пакета: Перенос данных";
                                currentpack.CommentSite = "Загрузка данных";
                                using (var mybd = CreateMyBD())
                                {
                                    currmyBD = mybd;
                                    try
                                    {
                                        CheckCancel(cancel);
                                        mybd.TruncALL();
                                    }
                                    catch (CancelException)
                                    {
                                        throw;
                                    }
                                    catch (Exception ex)
                                    {
                                        currentpack.Status = StatusFilePack.FLKERR;
                                        currentpack.CommentSite = "Что то пошло не так...";
                                        WcfInterface.AddLog($"Ошибка при очистки базы перед загрузкой для {currentpack.codeMOstr}: {ex.Message}",EventLogEntryType.Error);
                                        continue;
                                    }

                                    //Проверка на сбой(т.е. во время переноса что то случилось. И теперь считает что они в БД
                                    foreach (var fi in currentpack.Files)
                                    {
                                        if (fi.Process == StepsProcess.FlkOk)
                                        {
                                            fi.Process = StepsProcess.XMLxsd;
                                        }
                                    }

                                    //Загрузка данных в БД
                                
                                    foreach (var fi in currentpack.Files)
                                    {
                                        try
                                        {
                                            CheckCancel(cancel);
                                            if (fi.Process != StepsProcess.XMLxsd) continue;
                                            currentpack.Comment =$"Обработка пакета: Перенос данных в БД ({fi.FileName})";
                                            fi.Comment = "Перенос файла";
                                            var rez1 = ToBaseFileFULL(fi, mybd);
                                            if (rez1.Result)
                                            {
                                                fi.FullProcess = StepsProcess.FlkOk;
                                                fi.Comment = "Перенос завершен";
                                                
                                            }
                                            else
                                            {
                                                fi.FullProcess = StepsProcess.FlkErr;
                                                fi.Comment = "Ошибка переноса";
                                                fi.FileLog.WriteLn(rez1.Exception);
                                            }

                                            GC.Collect();
                                        }
                                        catch (CancelException)
                                        {
                                            throw;
                                        }
                                        catch (Exception ex)
                                        {
                                            WcfInterface.AddLog($"Ошибка при переносе {fi.FileName}: {ex.Message}",EventLogEntryType.Error);
                                        }
                                    }

                                    Task clearTemp100Task = null;
                                    var ProcessList = currentpack.Files.Where(x =>x.Process == StepsProcess.FlkOk && x.filel.Process == StepsProcess.FlkOk).ToList();
                                    CheckCancel(cancel);
                                    //ОЧИСТКА БАЗЫ ПЕРЕНОСА
                                    if (AppConfig.Property.TransferBD)
                                    {
                                        try
                                        {
                                            clearTemp100Task = mybd.DeleteTemp100TASK(currentpack.codeMOstr);

                                        }
                                        catch (Exception ex)
                                        {
                                            WcfInterface.AddLog($"Ошибка при очистки базы переноса для {currentpack.codeMOstr}: {ex.Message}",EventLogEntryType.Error);
                                            throw;
                                        }
                                    }

                                    currentpack.CommentSite = "Проверка";
                                    currentpack.Comment = "Обработка пакета: ФЛК и сбор ошибок";

                                    //Проверяем флк и пишем ЛОГ  


                                    if (ProcessList.Count != 0)
                                    {
                                        foreach (var fi in ProcessList)
                                        {
                                            fi.WriteLnFull(":Начало ФЛК:");
                                        }

                                        CheckCancel(cancel);
                                        CheckFLK_ALL(mybd, currentpack,cancel);
                                        foreach (var fi in ProcessList)
                                        {
                                                fi.WriteLnFull(":Конец ФЛК:");
                                                fi.WriteLnFull(":ФАЙЛ ПРИНЯТ:");
                                        }
                                    }

                                    currentpack.CommentSite = "Формирование ошибок";
                                    currentpack.Comment = "Обработка пакета: Формирование EXCEL";
                                    //Формируем Excel файл
                                    try
                                    {
                                        if (ProcessList.Count != 0)
                                        {
                                            var tbl = mybd.GetErrorView();
                                            currentpack.PATH_STAT = Path.Combine(AppConfig.Property.ProcessDir,currentpack.codeMOstr, SvodFileNameXLS);
                                            ExportExcel2(tbl, currentpack.PATH_STAT);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        WcfInterface.AddLog("Ошибка при формировании EXCEL файла:" + ex.Message,EventLogEntryType.Error);
                                    }

                                    currentpack.Comment = "Обработка пакета: Формирование Свода";

                                    //Формируем сводный файл

                                    CreateExcelSvod2(currentpack,Path.Combine(AppConfig.Property.ProcessDir, currentpack.codeMOstr,SvodFileNameXLS),mybd.SVOD_FILE_TEMP99(), mybd.STAT_VIDMP_TEMP99(), mybd.STAT_FULL_TEMP99());
                                    currentpack.CommentSite = "Удаление предыдущей выгрузки";
                                    currentpack.Comment =$"Обработка пакета: Очистка базы {AppConfig.Property.xml_h_zglv_transfer} от {currentpack.codeMOstr}";
                                    try
                                    {
                                        clearTemp100Task?.Wait(cancel);
                                    }
                                    catch (AggregateException ex)
                                    {
                                        currentpack.CommentSite = "Что то пошло не так...";
                                        foreach (var e in ex.InnerExceptions)
                                        {
                                            currentpack.Comment =$"При очистке базы {AppConfig.Property.xml_h_zglv_transfer} от {currentpack.codeMOstr}: {e.Message}";
                                            WcfInterface.AddLog($"При очистке базы {AppConfig.Property.xml_h_zglv_transfer} от {currentpack.codeMOstr}: {e.Message}",EventLogEntryType.Error);
                                        }
                                        throw  new Exception("Ошибка при очистке TEMP100");
                                    }

                                    currentpack.CommentSite = "Сохранение данных";
                                    //Перенос в месячную БД
                                    if (AppConfig.Property.TransferBD)
                                    {
                                        CheckCancel(cancel);
                                        try
                                        {
                                            foreach (var fi in currentpack.Files)
                                            {
                                                if (fi.Process == StepsProcess.FlkOk && fi.Process == StepsProcess.FlkOk)
                                                {
                                                    fi.WriteLnFull("Начало переноса");
                                                }
                                            }

                                            //L_ZGLV
                                            currentpack.Comment = "Обработка пакета: Перенос L_ZGLV";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_l_zglv,AppConfig.Property.schemaOracle,AppConfig.Property.xml_l_zglv_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //L_PERS
                                            currentpack.Comment = "Обработка пакета: Перенос L_PERS";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_l_pers,AppConfig.Property.schemaOracle,AppConfig.Property.xml_l_pers_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //ZGLV
                                            currentpack.Comment = "Обработка пакета: Перенос ZGLV";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_zglv,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_zglv_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //SCHET
                                            currentpack.Comment = "Обработка пакета: Перенос SCHET";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_schet,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_schet_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //ZAP
                                            currentpack.Comment = "Обработка пакета: Перенос ZAP";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_zap,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_zap_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //PAC
                                            currentpack.Comment = "Обработка пакета: Перенос PACIENT";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_pacient,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_pacient_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //Z_SLUCH
                                            currentpack.Comment = "Обработка пакета: Перенос Z_SLUCH";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_z_sluch,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_z_sluch_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //SLUCH
                                            currentpack.Comment = "Обработка пакета: Перенос SLUCH";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_sluch,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_sluch_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //DS2_N
                                            currentpack.Comment = "Обработка пакета: Перенос DS2_N";
                                            TransResult(mybd.TransferTable(AppConfig.Property.XML_H_DS2_N,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_ds2_n_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //NAZR
                                            currentpack.Comment = "Обработка пакета: Перенос NAZR";
                                            TransResult(mybd.TransferTable(AppConfig.Property.XML_H_NAZR,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_nazr_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //KSLP
                                            currentpack.Comment = "Обработка пакета: Перенос KOEF";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_kslp,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_kslp_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //USL
                                            currentpack.Comment = "Обработка пакета: Перенос USL";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_usl,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_usl_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //SANK
                                            currentpack.Comment = "Обработка пакета: Перенос SANK";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_sank,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_sank_smo_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //B_DIAG
                                            currentpack.Comment = "Обработка пакета: Перенос B_DIAG";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_b_diag,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_b_diag_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //B_PROT
                                            currentpack.Comment = "Обработка пакета: Перенос B_PROT";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_b_prot,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_b_prot_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //NAPR
                                            currentpack.Comment = "Обработка пакета: Перенос NAPR";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_napr,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_napr_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //ONK_USL
                                            currentpack.Comment = "Обработка пакета: Перенос ONK_USL";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_onk_usl,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_onk_usl_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //LEK_PR
                                            currentpack.Comment = "Обработка пакета: Перенос LEK_PR";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_lek_pr,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_lek_pr_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //LEK_PR_DATE_INJ
                                            currentpack.Comment = "Обработка пакета: Перенос LEK_PR_DATE_INJ";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_date_inj,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_date_inj_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //CONS
                                            currentpack.Comment = "Обработка пакета: Перенос CONS";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_cons,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_cons_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //ds2
                                            currentpack.Comment = "Обработка пакета: Перенос DS2";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_ds2,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_ds2_transfer,AppConfig.Property.schemaOracle_transfer));
                                            //ds3
                                            currentpack.Comment = "Обработка пакета: Перенос DS3";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_ds3,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_ds3_transfer,AppConfig.Property.schemaOracle_transfer));

                                            //CRIT
                                            currentpack.Comment = "Обработка пакета: Перенос CRIT";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_crit,AppConfig.Property.schemaOracle,AppConfig.Property.xml_h_crit_transfer,AppConfig.Property.schemaOracle_transfer));

                                            foreach (var fi in ProcessList)
                                            {
                                                    fi.WriteLnFull("Перенос завершен");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            foreach (var fi in ProcessList)
                                            {
                                                fi.WriteLnFull("Ошибка переноса");
                                            }

                                            currentpack.CommentSite = "Что то пошло не так...";
                                            currentpack.Comment = $"Ошибка при переносе в месячную БД: {ex.Message}";
                                            WcfInterface.AddLog($"Ошибка при переносе в месячную БД для {currentpack.codeMOstr}: {ex.Message}",EventLogEntryType.Error);
                                        }

                                    }
                                    else
                                    {
                                        foreach (var fi in ProcessList)
                                        {
                                            fi.WriteLnFull("Перенос не активен");
                                        }
                                    }


                                    GC.Collect();
                                    //Закрываем все и фурмируем ошибки
                                    currentpack.CloserLogFiles();
                                    CreateErrorMessage(currentpack);
                                    currentpack.Status = StatusFilePack.FLKOK;
                                    currentpack.Comment = "Обработка пакета: Завершено";
                                    currentpack.CommentSite = "Завершено";
                                    //Сохраняем файлы(журнала и работы) на всякий случай.
                                    SaveFilesParam();
                                    DBinvitePac = null;
                                    currmyBD = null;
                                }
                            }
                        }
                        catch (CancelException)
                        {
                            currentpack.Status = StatusFilePack.FLKERR;
                            currentpack.CloserLogFiles();
                            CreateErrorMessage(currentpack);
                            currentpack.CommentSite = "Обработка прервана пользователем";
                            currentpack.Comment = "Обработка прервана пользователем";
                            WcfInterface.AddLog($"Прерывание потока выполнения {currentpack.codeMOstr}",EventLogEntryType.Error);
                            DBinvitePac = null;

                            return;
                        }
                        catch (Exception ex)
                        {
                            currentpack.Status = StatusFilePack.FLKERR;
                            currentpack.CommentSite = "Что то пошло не так...";
                            currentpack.Comment = "Ошибка при переносе в БД";
                            WcfInterface.AddLog($"Ошибка при переносе в БД {currentpack.codeMOstr} ({ex.Source}:{ex.Message})",EventLogEntryType.Error);
                            currentpack.CloserLogFiles();
                        }
                    }

                    Thread.Sleep(1000);
                    DBinvitePac = null;
                }
            }
            catch (Exception ex)
            {
                DBinvitePac = null;
                WcfInterface.AddLog($"Ошибка в потоке ФЛК ({ex.Source}:{ex.Message})",EventLogEntryType.Error);
            }
        }


        private void TransResult(MYBDOracleNEW.TransferTableRESULT res)
        {
            if (res == null) return;
            if(res.Colums.Count!=0)
                WcfInterface.AddLog($"Данные таблицы {res.Table} сохраняются не в полном объеме. Потери в {string.Join(",", res.Colums)}", EventLogEntryType.Error);
        }

        /// <summary>
        /// Экспорт в Excel вьюжки Error
        /// </summary>
        /// <param name="TBL">Таблица </param>
        /// <param name="path">Путь выходного файла</param>

        private void ExportExcel2(DataTable TBL, string path)
        {
            ExcelOpenXML efm = null;
            try
            {
                if (File.Exists(path))
                {
                    efm = new ExcelOpenXML();
                    efm.OpenFile(path, 0);
                    if (efm.CurrentSheetName != "Ошибки ФЛК")
                    {
                        efm.AddSheet("Ошибки ФЛК");
                    }
                }
                else
                {
                    efm = new ExcelOpenXML(path, "Ошибки ФЛК");
                }

                var styleCellDefault = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center , wordwrap = true }, new BorderOpenXML(), new FillOpenXML());               
                var styleCellYellow = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, wordwrap = true}, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.Yellow  });
                var styleCellRed = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, wordwrap = true }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var styleCellDefaultDate = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Format = Convert.ToUInt32(DefaultNumFormat.F14) }, new BorderOpenXML(), new FillOpenXML());


               
         
                var styleColumn = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Bold = true }, new BorderOpenXML(), new FillOpenXML());

                var r = efm.GetRow(1);
                r.Height = 48;
                efm.PrintCell(r, 1, "Внутренний номер ТФОМС", styleColumn);
                efm.PrintCell(r, 2, "Код МО", styleColumn);
                efm.PrintCell(r, 3, "Фамилия пациента/представителя", styleColumn);
                efm.PrintCell(r, 4, "Имя пациента/представителя", styleColumn);
                efm.PrintCell(r, 5, "Отчество пациента/представителя", styleColumn);
                efm.PrintCell(r, 6, "ДР пациента/представителя", styleColumn);
                efm.PrintCell(r, 7, "Код пациента", styleColumn);
                efm.PrintCell(r, 8, "Тип полиса", styleColumn);
                efm.PrintCell(r, 9, "Серия полиса", styleColumn);
                efm.PrintCell(r, 10, "Номер полиса", styleColumn);
                efm.PrintCell(r, 11, "Подразделение МО", styleColumn);
                efm.PrintCell(r, 12, "№ истории болезни/ амбулаторной карты", styleColumn);
                efm.PrintCell(r, 13, "Условие оказания МП", styleColumn);
                efm.PrintCell(r, 14, "Код врача, закрывшего случай", styleColumn);
                efm.PrintCell(r, 15, "Дата начала", styleColumn);
                efm.PrintCell(r, 16, "Дата окончания", styleColumn);
                efm.PrintCell(r, 17, "Ошибка", styleColumn);

                efm.SetColumnWidth(1, 11);
                efm.SetColumnWidth(2, 18);
                efm.SetColumnWidth(3, 18);
                efm.SetColumnWidth(4, 18);
                efm.SetColumnWidth(5, 18);
                efm.SetColumnWidth(6, 18);
                efm.SetColumnWidth(7, 17);
                efm.SetColumnWidth(8, 9);
                efm.SetColumnWidth(9, 9);
                efm.SetColumnWidth(10, 19);
                efm.SetColumnWidth(11, 16);
                efm.SetColumnWidth(12, 17);
                efm.SetColumnWidth(13, 9);
                efm.SetColumnWidth(14, 18);
                efm.SetColumnWidth(15, 12);
                efm.SetColumnWidth(16, 12);
                efm.SetColumnWidth(17, 64);


          
                uint RowIndexEx = 2;
             
                if (TBL.Rows.Count == 0)
                {
                    r = efm.GetRow(RowIndexEx,true);
                    efm.PrintCell(r, 1, "Ошибки отсутствуют", styleCellDefault);
                    efm.Save();
                    return;
                }
                var ERR_LIST = new Dictionary<string, List<ErrorProtocolXML>>();

                for (var i = 0; i < TBL.Rows.Count; i++)
                {
                    #region Формирование файлов FLK
                    var fi = TBL.Rows[i]["FILENAME"].ToString();
                    var err = new ErrorProtocolXML
                    {
                        BAS_EL = TBL.Rows[i]["BAS_EL"].ToString(),
                        IDCASE = TBL.Rows[i]["IDCASE"].ToString(),
                        ID_SERV = TBL.Rows[i]["ID_SERV"].ToString(),
                        SL_ID = TBL.Rows[i]["SL_ID"].ToString(),
                        N_ZAP = TBL.Rows[i]["N_ZAP"].ToString(),
                        OSHIB = Convert.ToInt32(TBL.Rows[i]["OSHIB"]),
                        Comment = TBL.Rows[i]["ERR"].ToString()
                    };
                    if (!ERR_LIST.ContainsKey(fi))
                    {
                        ERR_LIST.Add(fi, new List<ErrorProtocolXML>());
                    }
                    ERR_LIST[fi].Add(err);
                    #endregion
                    var row = ExportExcelRow.Get(TBL.Rows[i]);
                    r = efm.GetRow(RowIndexEx, true);

                    uint tERR;

                    switch (row.ERR_TYPE)
                    {
                        case "1": tERR = styleCellYellow; break;
                        case "2": tERR = styleCellRed; break;
                        default:
                            tERR = styleCellDefault;
                            break;
                    }

                    efm.PrintCell(r, 1, row.SLUCH_ID, styleCellDefault);
                    efm.PrintCell(r, 2, row.CODE_MO, styleCellDefault);                 
                    efm.PrintCell(r, 3, row.FAM, styleCellDefault);
                    efm.PrintCell(r, 4, row.IM, styleCellDefault);
                    efm.PrintCell(r, 5, row.OT, styleCellDefault);
                    efm.PrintCell(r, 6, row.DR, styleCellDefault);
                    efm.PrintCell(r, 7, row.ID_PAC, styleCellDefault);
                    efm.PrintCell(r, 8, row.VPOLIS, styleCellDefault);
                    efm.PrintCell(r, 9, row.SPOLIS, styleCellDefault);
                    efm.PrintCell(r, 10, row.NPOLIS, styleCellDefault);
                    efm.PrintCell(r, 11, row.LPU_1, styleCellDefault);
                    efm.PrintCell(r, 12, row.NHISTORY, styleCellDefault);
                    efm.PrintCell(r, 13, row.USL_OK, styleCellDefault);
                    efm.PrintCell(r, 14, row.IDDOKT, styleCellDefault);
                    efm.PrintCell(r, 15, row.DATE_1, styleCellDefaultDate);
                    efm.PrintCell(r, 16, row.DATE_2, styleCellDefaultDate);
                    efm.PrintCell(r, 17, row.ERR, tERR);

                    RowIndexEx++;
                }
                efm.Save();
                efm = null;
                GC.Collect();
                foreach (var T in ERR_LIST)
                {
                    var pathToXML = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(T.Key) + "FLK.xml");
                    SchemaChecking.XMLfileFLK(pathToXML, T.Key, T.Value);
                }

            }
            catch (Exception ex)
            {
                efm?.Dispose();
                GC.Collect();
                WcfInterface.AddLog("Ошибка при формировании Excel(ФЛК) файла(" + path + "): " + ex.Message, EventLogEntryType.Error);
            }
        }
        class ExportExcelRow
        {
            public static ExportExcelRow Get(DataRow row)
            {
                try
                {
                    var item = new ExportExcelRow();
                    if (row["SLUCH_ID"] != DBNull.Value)
                        item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                    if (row["CODE_MO"] != DBNull.Value)
                        item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                    if (row["FAM"] != DBNull.Value)
                        item.FAM = Convert.ToString(row["FAM"]);
                    if (row["IM"] != DBNull.Value)
                        item.IM = Convert.ToString(row["IM"]);
                    if (row["OT"] != DBNull.Value)
                        item.OT = Convert.ToString(row["OT"]);
                    if (row["DR"] != DBNull.Value)
                        item.DR = Convert.ToString(row["DR"]);
                    if (row["ID_PAC"] != DBNull.Value)
                        item.ID_PAC = Convert.ToString(row["ID_PAC"]);
                    if (row["VPOLIS"] != DBNull.Value)
                        item.VPOLIS = Convert.ToString(row["VPOLIS"]);
                    if (row["SPOLIS"] != DBNull.Value)
                        item.SPOLIS = Convert.ToString(row["SPOLIS"]);
                    if (row["NPOLIS"] != DBNull.Value)
                        item.NPOLIS = Convert.ToString(row["NPOLIS"]);
                    if (row["NHISTORY"] != DBNull.Value)
                        item.NHISTORY = Convert.ToString(row["NHISTORY"]);
                    if (row["USL_OK"] != DBNull.Value)
                        item.USL_OK = Convert.ToString(row["USL_OK"]);
                    if (row["LPU_1"] != DBNull.Value)
                        item.LPU_1 = Convert.ToString(row["LPU_1"]);
                    if (row["IDDOKT"] != DBNull.Value)
                        item.IDDOKT = Convert.ToString(row["IDDOKT"]);
                    if (row["DATE_1"] != DBNull.Value)
                        item.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                    if (row["DATE_2"] != DBNull.Value)
                        item.DATE_2 = Convert.ToDateTime(row["DATE_2"]);
                    if (row["ERR"] != DBNull.Value)
                        item.ERR = Convert.ToString(row["ERR"]);
                    if (row["ERR_TYPE"] != DBNull.Value)
                        item.ERR_TYPE = Convert.ToString(row["ERR_TYPE"]);

                    return item;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при получение ExportExcelRow: {ex.Message}", ex);
                }
            }

            public decimal SLUCH_ID { get; set; } = 0;
            public string CODE_MO { get; set; }
            public string FAM { get; set; } = "";
            public string IM { get; set; } = "";
            public string OT { get; set; } = "";
            public string DR { get; set; }
            public string ID_PAC { get; set; } = "";
            public string VPOLIS { get; set; } = "";
            public string SPOLIS { get; set; } = "";
            public string NPOLIS { get; set; } = "";
            public string NHISTORY { get; set; } = "";
            public string USL_OK { get; set; } = "";
            public string LPU_1 { get; set; } = "";
            public string IDDOKT { get; set; } = "";
            public DateTime? DATE_1 { get; set; }
            public DateTime? DATE_2 { get; set; }
            public string ERR { get; set; } = "";
            public string ERR_TYPE { get; set; } = "0";

        }
        class  ItemSvod
        {
            public int? NUM { get; set; }
            public  FileItemBase File { get; set; }

            public string FileName => File==null? "" : File.FileName;

            public string Result
            {
                get
                {
                    switch (File.Process)
                    {
                        case StepsProcess.NotInvite:
                           return "Файл не принят! (Файл не поступил на обработку) подробности в " + File.FileLog?.FileName;
                        case StepsProcess.ErrorXMLxsd:
                            return  "Файл не принят! (Ошибка при проверке схемы документа) подробности в " + File.FileLog?.FileName;
                        case StepsProcess.FlkOk:
                            return $"Файл принят подробности в {File.FileLog?.FileName}";
                        case StepsProcess.Invite:
                            return
                                $"Файл не принят! Файл поступил в обработку (ФЛК не выполнялось) подробности в {File.FileLog?.FileName}";
                        case StepsProcess.XMLxsd:
                            return
                                $"Файл не принят! (Проверка на схему выполнена, ФЛК не выполнялось) подробности в {File.FileLog?.FileName}";
                        case StepsProcess.FlkErr:
                            return
                                $"Файл не принят! (Ошибка при выполнении ФЛК) подробности в {File.FileLog?.FileName}";
                        default:
                            return $"Не определенность подробности в {File.FileLog?.FileName}";
                    }
                }
            }

            public bool IsError => File.Process != StepsProcess.FlkOk;

            public decimal? SUM { get; set; }
            public decimal? SUM_MEK { get; set; }
            public int? CSLUCH { get; set; }
            public int? CUSL { get; set; }
            public DateTime DATE { get; set; } = DateTime.Now;
            public string REL => File.FileLog?.FileName;
            public string Comment { get; set; }
        }

        string SvodFileNameXLS = "FileStat.xlsx";
        private void CreateExcelSvod2(FilePacket pack, string path, DataTable SVOD, DataTable STAT_VID_MP,DataTable STAT_FULL)
        {
            ExcelOpenXML efm = null;
            try
            {
                var SvodList = new List<ItemSvod>();
                pack.PATH_STAT = path;

                for (var i = 0; i < pack.Files.Count; i++)
                {
                    var item = new ItemSvod {File = pack.Files[i], NUM = i+1};
                    SvodList.Add(item);
                    var index = FindRowSVODTBL(SVOD, item.FileName);
                    if (index != -1)
                    {
                        item.Comment = SVOD.Rows[index]["COM"].ToString();
                        item.SUM = Convert.ToDecimal(SVOD.Rows[index]["SUM"]);
                        item.SUM_MEK = Convert.ToDecimal(SVOD.Rows[index]["SUM_MEK"]);
                        item.CSLUCH = Convert.ToInt32(SVOD.Rows[index]["CSLUCH"]);
                        item.CUSL = Convert.ToInt32(SVOD.Rows[index]["CUSL"]);
                    }
                    else
                    {
                        item.SUM = 0;
                        item.SUM_MEK = 0;
                        item.CSLUCH = 0;
                        item.CUSL = 0;
                    }

                    if (pack.Files[i].filel != null)
                    {
                        var itemL = new ItemSvod {File = pack.Files[i].filel};
                        SvodList.Add(itemL);
                    }
                }

                if (File.Exists(path))
                {
                    efm = new ExcelOpenXML();
                    efm.OpenFile(path, 0);
                    if (efm.CurrentSheetName != "Свод")
                    {
                        efm.InsertSheet("Свод");
                    }
                }
                else
                {
                    efm = new ExcelOpenXML(path, "Свод");
                }
                var styleHeader = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Bold = true }, new BorderOpenXML(), new FillOpenXML());

                var r = efm.GetRow(1);

                efm.PrintCell(r, 1, "№", styleHeader);
                efm.PrintCell(r, 2, "Имя файла", styleHeader);
                efm.PrintCell(r, 3, "Результат приема", styleHeader);
                efm.PrintCell(r, 4, "Сумма реестра", styleHeader);
                efm.PrintCell(r, 5, "Сумма снятия(предварительно)", styleHeader);
                efm.PrintCell(r, 6, "Кол-во случаев", styleHeader);
                efm.PrintCell(r, 7, "Кол-во услуг", styleHeader);
                efm.PrintCell(r, 8, "Дата приёма", styleHeader);
                efm.PrintCell(r, 9, "Дополнительно", styleHeader);

                efm.SetColumnWidth(1, 4);
               
                var Style = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML());
                var StyleWRAP = efm.CreateType(new FontOpenXML { wordwrap = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML());
                var StyleBold = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left , Bold = true}, new BorderOpenXML(), new FillOpenXML());
                var StyleRed = efm.CreateType(new FontOpenXML {   HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var StyleRedWRAP = efm.CreateType(new FontOpenXML { wordwrap = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });


                var StyleResTrue = efm.CreateType(new FontOpenXML { color = System.Windows.Media.Colors.Blue, Underline = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.YellowGreen });
                var StyleResFalse = efm.CreateType(new FontOpenXML { color = System.Windows.Media.Colors.Blue, Underline = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
               
                var Style1 = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML());
                var Style1Red = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var Style1Bold = efm.CreateType(new FontOpenXML { Bold = true, HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML());

                var Style1ResFalse = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var Style2 = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F3) }, new BorderOpenXML(), new FillOpenXML());
                var Style2Red = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F3) }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var Style2Bold = efm.CreateType(new FontOpenXML { Bold = true, HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F3) }, new BorderOpenXML(), new FillOpenXML());

                var stWARNNING = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true }, null, new FillOpenXML { color = System.Windows.Media.Colors.Yellow });
                var stWARNNINGBorder = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.Yellow });

                uint indexRow = 2;
                foreach (var item in SvodList)
                {
                    r = efm.GetRow(indexRow);
                    efm.PrintCell(r, 1, item.NUM, Style);
                    efm.PrintCell(r, 2, item.FileName, Style);
                    efm.PrintCell(r, 3, item.Result, item.IsError? StyleResFalse : StyleResTrue);
                    if(!string.IsNullOrEmpty(item.REL))
                        efm.PrintHyperlink(r, 3, item.REL);


                    if (item.SUM.HasValue && item.CSLUCH.HasValue  && item.CUSL.HasValue )
                    {
                        efm.PrintCell(r, 4, item.SUM, Style1);
                        efm.PrintCell(r, 5, item.SUM_MEK, item.SUM_MEK == 0? Style1 : Style1ResFalse);
                        efm.PrintCell(r, 6, item.CSLUCH, Style2);
                        efm.PrintCell(r, 7, item.CUSL, Style2);
                    }
                    else
                    {
                        efm.PrintCell(r, 4, "", Style1);
                        efm.PrintCell(r, 5, "", Style1);
                        efm.PrintCell(r, 6, "", Style1);
                        efm.PrintCell(r, 7, "", Style1);
                    }
                    efm.PrintCell(r, 8, item.DATE.ToShortDateString() + " " + item.DATE.ToShortTimeString(), Style);
                    efm.PrintCell(r, 9, item.Comment,string.IsNullOrEmpty(item.Comment)? Style : stWARNNINGBorder);
                    indexRow++;
                }

                indexRow++;
                //efm.AutoSizeColumn(0, 8);
                var t = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true }, null, new FillOpenXML());
                r = efm.GetRow(indexRow);
                efm.PrintCell(r, 1,AppConfig.Property.MainTypePriem? $"Тип приёма реестров: Основной" : "Тип приёма реестров: Предварительный", t);
                indexRow++;
                r = efm.GetRow(indexRow);
                efm.PrintCell(r, 1, $"Отчетный период: {AppConfig.Property.OtchetDate:MMMMMMMMMM yyyy} года.", t);
                indexRow++;
                r = efm.GetRow(indexRow);
                efm.PrintCell(r, 1, $"Ф.И.О. принимающего лица: {AppConfig.Property.ISP_NAME}", t);
                if(pack.WARNNING!="")
                {
                    indexRow++;
                    r = efm.GetRow(indexRow);
                    efm.PrintCell(r, 1, $"Предупреждение: {pack.WARNNING}", stWARNNING);
                    string str = null;
                    efm.PrintCell(r, 2, str, stWARNNING);
                    efm.PrintCell(r, 3, str, stWARNNING);
                }
                efm.AutoSizeColumns(2, 9);

                if (STAT_VID_MP != null)
                {
                    if (STAT_VID_MP.Rows.Count != 0)
                    {
                        efm.AddSheet("Статистика");
                        indexRow = 1;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "Всего предъявлено:", t);
                        indexRow++;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "№", styleHeader);
                        efm.PrintCell(r, 2, "Вид медицинской помощи", styleHeader);
                        efm.PrintCell(r, 3, "Кол-во законченных случаев", styleHeader);
                        efm.PrintCell(r, 4, "Кол-во случаев", styleHeader);
                        efm.PrintCell(r, 5, "Сумма", styleHeader);
                        efm.SetColumnWidth(1, 9);
                        efm.SetColumnWidth(2, 91);
                        efm.SetColumnWidth(3, 16);
                        efm.SetColumnWidth(4, 16);
                        efm.SetColumnWidth(5, 16);

                        indexRow++;
                        double C_ZS_S = 0;
                        double C_SL_S = 0;
                        double SUMV_S = 0;

                        double C_ZS = 0;
                        double C_SL = 0;
                        double SUMV = 0;

                        foreach (DataRow row in STAT_VID_MP.Rows)
                        {
                            r = efm.GetRow(indexRow);
                             C_ZS = Convert.ToDouble(row["C_ZS"]);
                             C_SL = Convert.ToDouble(row["C_SL"]);
                             SUMV = Convert.ToDouble(row["SUMV"]);
                             C_ZS_S+= C_ZS;
                             C_SL_S+= C_SL;
                             SUMV_S+=  SUMV;
                            if (row["PS"]==DBNull.Value)
                            {
                                efm.PrintCell(r, 1, row["PS"].ToString(), StyleRed);
                                efm.PrintCell(r, 2, row["NAME"].ToString(), StyleRedWRAP);
                                efm.PrintCell(r, 3, C_ZS, Style2Red);
                                efm.PrintCell(r, 4, C_SL, Style2Red);
                                efm.PrintCell(r, 5, SUMV, Style1Red);
                            }
                            else
                            {
                                efm.PrintCell(r, 1, row["PS"].ToString(), Style);
                                efm.PrintCell(r, 2, row["NAME"].ToString(), StyleWRAP);
                                efm.PrintCell(r, 3, C_ZS, Style2);
                                efm.PrintCell(r, 4, C_SL, Style2);
                                efm.PrintCell(r, 5, SUMV, Style1);
                            }
                            indexRow++;
                        }
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 2, "Итого", StyleBold);
                        efm.PrintCell(r, 3, Math.Round(C_ZS_S,0), Style2Bold);
                        efm.PrintCell(r, 4, Math.Round(C_SL_S, 0), Style2Bold);
                        efm.PrintCell(r, 5, Math.Round(SUMV_S, 2), Style1Bold);
                      
                    }
                }


                if (STAT_FULL != null)
                {
                    if (STAT_FULL.Rows.Count != 0)
                    {
                        efm.AddSheet("Статистика(Расширенная)");
                        indexRow = 1;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "Всего предъявлено:", t);
                        indexRow++;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "Условие оказания", styleHeader);
                        efm.PrintCell(r, 2, "КСГ/Услуга/Метод ВМП", styleHeader);
                        efm.PrintCell(r, 3, "Наименование", styleHeader);
                        efm.PrintCell(r, 4, "Кол-во случаев", styleHeader);
                        efm.PrintCell(r, 5, "Кол-во услуг/ует", styleHeader);
                        efm.PrintCell(r, 6, "Сумма", styleHeader);
                        efm.SetColumnWidth(1, 21);
                        efm.SetColumnWidth(2, 14);
                        efm.SetColumnWidth(3, 85);
                        efm.SetColumnWidth(4, 16);
                        efm.SetColumnWidth(5, 16);
                        efm.SetColumnWidth(6, 16);
                        indexRow++;
                        double SL;
                        double SUM_SL = 0;
                        double SUMV;
                        double SUM_SUMV = 0;
                        double KOL_USL = 0;
                        double SUM_KOL_USL = 0;
                        foreach (DataRow row in STAT_FULL.Rows)
                        {
                            r = efm.GetRow(indexRow);
                            SL = Convert.ToDouble(row["SL"]);
                            SUMV = Convert.ToDouble(row["SUMV"]);
                            KOL_USL = Convert.ToDouble(row["KOL_USL"]);
                            SUM_SL += SL;
                            SUM_SUMV += SUMV;
                            SUM_KOL_USL += KOL_USL;
                            efm.PrintCell(r, 1, row["USL_OK"].ToString(), Style);
                            efm.PrintCell(r, 2, row["N_KSG"].ToString(), Style);
                            efm.PrintCell(r, 3, row["KSG"].ToString(), StyleWRAP);
                            efm.PrintCell(r, 4, SL, Style2);
                            efm.PrintCell(r, 5, KOL_USL, Style1);
                            efm.PrintCell(r, 6, SUMV, Style1);
                            indexRow++;
                        }

                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 3, "Итого", StyleBold);
                        efm.PrintCell(r, 4, Math.Round(SUM_SL, 0), Style2Bold);
                        efm.PrintCell(r, 5, Math.Round(SUM_KOL_USL, 0), Style2Bold);
                        efm.PrintCell(r, 6, Math.Round(SUM_SUMV, 2), Style1Bold);
                    }
                }

                efm.Save();
               
                efm = null;
                STAT_VID_MP = null;
                GC.Collect();

            }
            catch(Exception ex)
            {
                efm?.Dispose();
                WcfInterface.AddLog($"Ошибка при формировании Excel(СВОД) файла({path}): {ex.Message}", EventLogEntryType.Error);
            }
        }
    
        /// <summary>
        /// Выполнение ФЛК для Пакета
        /// </summary>
        /// <param name="bd">Класс MYBD</param>
        /// <param name="pack">Пакет</param>
        private void CheckFLK_ALL(MYBDOracleNEW bd, FilePacket pack,CancellationToken cancel)
        {
            var cList = new List<TableName> { TableName.ZGLV, TableName.SCHET, TableName.ZAP, TableName.PACIENT, TableName.SLUCH, TableName.USL,  TableName.L_ZGLV ,TableName.L_PERS,};
            foreach (var tn in cList)
            {
                CheckCancel(cancel);
                try
                {
                    CheckFLK(bd, pack, tn,cancel);
                }
                catch (Exception ex)
                {
                    pack.WriteAllLog($"Не удалось провести ФЛК {tn}:" + ex.Message);
                }
            }
        }

        private void CheckFLK(MYBDOracleNEW bd, FilePacket pack, TableName TN, CancellationToken cancel)
        {
            bd.Checking(TN, wi.CheckList, cancel, ref pack._Comment);
            //Вывод кританувших процедур
            var listEROR = wi.CheckList[TN].FindAll(val => val.Excist == StateExistProcedure.NotExcist && val.STATE);
            foreach (var or in listEROR)
            {
                WcfInterface.AddLog($"Ошибка при выполнении {or.NAME_PROC}({or.Comment}) для {pack.codeMOstr}", EventLogEntryType.Error);
            }
        }


        /// <summary>
        /// Записать текст message в лог файл с именем Fname в пакете pack
        /// </summary>
        /// <param name="pack">Пакет</param>
        /// <param name="Fname">Имя файла</param>
        /// <param name="message">Текст</param>
        private void ToPacketLog(FilePacket pack, string Fname, string message)
        {
            Fname = Path.GetFileNameWithoutExtension(Fname) + ".XML";
            Fname = Fname.ToUpper();
            var file = pack.Files.Find(fi => fi.FileName == Fname);
            if (file != null)
            {
                file.FileLog.WriteLn(message);
            }
            else
            {
                file = pack.Files.Find(delegate(FileItem fi)
                    {
                        if (fi.filel == null) return false;
                        return (fi.filel.FileName == Fname);
                    }
                );

                if (file != null)
                {
                    if (file.filel != null)
                        file.filel.FileLog.WriteLn(message);
                    else
                        WcfInterface.AddLog(
                            "Ошибка вывода лога после ФЛК для файла. Файл " + Fname + " не найден. МО - " +
                            pack.codeMOstr, EventLogEntryType.Error);
                }
                else
                {
                    WcfInterface.AddLog(
                        "Ошибка вывода лога после ФЛК для файла. Файл " + Fname + " не найден. МО - " + pack.codeMOstr,
                        EventLogEntryType.Error);
                }
            }

        }

        private BoolResult ToBaseFileFULL(FileItem fi, MYBDOracleNEW mybd)
        {
            try
            {
                var zl = ZL_LIST.GetZL_LIST(fi.Version, fi.FilePach);

                zl.SCHET.YEAR_BASE = zl.SCHET.YEAR;
                zl.SCHET.MONTH_BASE = zl.SCHET.MONTH;
                zl.SCHET.YEAR = AppConfig.Property.OtchetDate.Year;
                zl.SCHET.MONTH = AppConfig.Property.OtchetDate.Month;


                mybd.BeginTransaction();
                mybd.InsertFile(zl, PERS_LIST.LoadFromFile(fi.filel.FilePach));
                mybd.Commit();
                return new BoolResult {Result = true};
            }
            catch (Exception ex)
            {
                mybd.Rollback();
                return new BoolResult {Result = false, Exception = "Ошибка при переносе в БД:" + ex.Message};
            }
        }

     
        /// <summary>
        /// Проверка на соответвие имён в файле и имени файла
        /// </summary>
        /// <param name="_fi"></param>
        /// <param name="checkL>проверять ли файл L</param> 
        /// <returns></returns>
        private bool CheckNameFile(FileItem _fi,bool checkL, MYBDOracleNEW bd)
        {
            try
            {
                if (checkL)
                {
                    var fi = _fi.filel;


                    var FileName = SchemaChecking.GetCode_fromXML(fi.FilePach, "FILENAME");
                    var FileName1 = SchemaChecking.GetCode_fromXML(fi.FilePach, "FILENAME1");
                

                    if (FileName.ToUpper() != Path.GetFileNameWithoutExtension(fi.FileName).ToUpper())
                    {
                        fi.FileLog.WriteLn($"Файл {fi.FileName} имеет не корректный FILENAME = {FileName.ToUpper()}");
                        fi.Comment = $"Файл {fi.FileName} имеет не корректный FILENAME = {FileName.ToUpper()}";
                      
                        return false;
                    }
                    if (FileName1.ToUpper() != Path.GetFileNameWithoutExtension(_fi.FileName).ToUpper())
                    {
                        fi.FileLog.WriteLn($"Файл {fi.FileName} имеет не корректный FILENAME1 = {FileName1.ToUpper()}");
                        fi.Comment = $"Файл {fi.FileName} имеет не корректный FILENAME1 = {FileName1.ToUpper()}";                        
                        return false;
                    }
                }
               
                var FILENAME = SchemaChecking.GetCode_fromXML(_fi.FilePach, "FILENAME");
             
                if (FILENAME.ToUpper() != Path.GetFileNameWithoutExtension(_fi.FileName).ToUpper())
                {
                    _fi.FileLog.WriteLn($"Файл {_fi.FileName} имеет не корректный FILENAME = {FILENAME.ToUpper()}");
                    _fi.Comment = $"Файл {_fi.FileName} имеет не корректный FILENAME = {FILENAME.ToUpper()}";
                   // dat.Dispose();
                    return false;
                }

                var tblnames = bd.GetZGLV_BYFileName(FILENAME);
                if (tblnames.Rows.Count != 0)
                {
                    _fi.FileLog.WriteLn($"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, который присутствует в предыдущих периодах");
                    foreach (DataRow row in tblnames.Rows)
                    {
                        _fi.FileLog.WriteLn($"Файл {row["FileName"]} от {Convert.ToDateTime(row["DSCHET"]).ToShortDateString()}");
                    }
                    _fi.Comment =$"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, который присутствует в предыдущих периодах";
                    return false;
                }


                var CODE = SchemaChecking.GetCode_fromXML(_fi.FilePach, "CODE");
                var CODE_MO = SchemaChecking.GetCode_fromXML(_fi.FilePach, "CODE_MO");
                var YEAR = SchemaChecking.GetCode_fromXML(_fi.FilePach, "YEAR");

                var tbl_schet = bd.GetSCHET_BYCODE_CODE_MO(Convert.ToInt32(CODE), CODE_MO, Convert.ToInt32(YEAR));
                if (tbl_schet.Rows.Count != 0)
                {
                    _fi.FileLog.WriteLn($"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, имеет код счета который присутствует в предыдущих периодах code = {CODE}");
                    foreach (DataRow row in tbl_schet.Rows)
                    {
                        _fi.FileLog.WriteLn($"Файл {row["FileName"]} code = {row["code"]} от {Convert.ToDateTime(row["DSCHET"]).ToShortDateString()}");
                    }
                    _fi.Comment =$"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, имеет код счета который присутствует в предыдущих периодах code = {CODE}";
                    
                    return false;
                }

                var parse = ParseFileName.Parse(FILENAME);
                if (parse.Ni != CODE_MO)
                {
                    _fi.FileLog.WriteLn($"Код МО указанный в наименовании файла не совпадает с тэгом CODE_MO");
                    _fi.Comment = $"Код МО указанный в наименовании файла не совпадает с тэгом CODE_MO";
                    return false;
                }
             
                return true;
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Ошибка при проверке на соответствие имен файлов для " + _fi.FileName + ": " + ex.Message, EventLogEntryType.Error);
                return false;
            }
            

        }

        /// <summary>
        /// Создание сообщений для МО. Сбор .log и *FLK.XML и *.XLS;
        /// </summary>
        /// <param name="pack">Пакет для обработки</param>
        private void CreateErrorMessage(FilePacket pack)
        {             
            // формируем файл для страховых
            var zipF = new ZipFile(Encoding.GetEncoding("cp866"));
            zipF.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
            var dir = new DirectoryInfo(Path.Combine(AppConfig.Property.ProcessDir , pack.codeMOstr));
            foreach (var fi in dir.EnumerateFiles("*.log"))
                zipF.AddFile(fi.FullName, "");
            foreach (var fi in dir.EnumerateFiles("*FLK.XML"))
                zipF.AddFile(fi.FullName, "");
            foreach (var fi in dir.EnumerateFiles("*.XLS"))
                zipF.AddFile(fi.FullName, "");

            if (AppConfig.Property.AddDIRInERROR != "")
                if (Directory.Exists(AppConfig.Property.AddDIRInERROR))
                    zipF.AddDirectory( AppConfig.Property.AddDIRInERROR, Path.GetFileNameWithoutExtension( AppConfig.Property.AddDIRInERROR));

            var ZIP_NAME = GetErrorName(pack) + ".zip";
            pack.Comment = "Обработка пакета: Формирование ошибок";

           
            zipF.Save(Path.Combine(AppConfig.Property.ProcessDir, pack.codeMOstr, ZIP_NAME));
            zipF.Dispose();
            pack.PATH_ZIP = Path.Combine(AppConfig.Property.ProcessDir, pack.codeMOstr, ZIP_NAME);         
            FilesManager.CopyFileTo(pack.PATH_ZIP, Path.Combine(ErrorArchivePath, Path.GetFileName(pack.PATH_ZIP)));
            
            if (pack.IST == IST.MAIL)
            {
                var FilePath = Path.Combine(AppConfig.Property.ErrorMessageFile, ZIP_NAME);
                var checkfile = FilePath;
                var x = 1;
                while (File.Exists(checkfile))
                {
                    checkfile = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + "(" + x.ToString() + ")" + Path.GetExtension(FilePath));
                    x++;
                }
                FilePath = checkfile;

                FilesManager.CopyFileTo(pack.PATH_ZIP, FilePath);
            }


          
            pack.Comment = "Обработка пакета: Формирование ошибок закончено";
        }
        private string ErrorArchivePath => Path.Combine(AppConfig.Property.ErrorDir, "ARCHIVE", DateTime.Now.ToString("yyyy_MM_dd"));

        private string ErrorPath => Path.Combine(AppConfig.Property.ErrorDir, DateTime.Now.ToString("yyyy_MM_dd"));




        #region Механизм уведомления изменения данных

        private void AddSendNewFileManager()
        {
            HasFileManagerNewData = true;
        }
        private void Wi_RaiseUnRegisterNewFileManager(USER us)
        {
            Monitor.Enter(ListCallBack);
            try
            {
                var item = ListCallBack.FirstOrDefault(x => x.US == us);
                if (item == null) return;
                if (ListCallBack.Contains(item))
                {
                    ListCallBack.Remove(item);
                }
            }
            finally
            {
                Monitor.Exit(ListCallBack);
            }
        }
        private void Wi_RaiseRegisterNewFileManager(USER us)
        {
            Monitor.Enter(ListCallBack);
            try
            {
                if (!ListCallBack.Contains(new USER_FMT(us)))
                {
                    ListCallBack.Add(new USER_FMT(us));
                }
            }
            finally
            {
                Monitor.Exit(ListCallBack);
            }
        }
        private class USER_FMT
        {
            public USER_FMT(USER us)
            {
                US = us;
            }
            public USER US { get; }
            public int CountFail { get; set; }
        }
        private List<USER_FMT> ListCallBack { get; } = new List<USER_FMT>();
        private bool HasFileManagerNewData;
        void SendNewFileManagerThread()
        {
            while (!cancelSendNewFileManagerTh.IsCancellationRequested)
            {
                if (HasFileManagerNewData)
                {
                    HasFileManagerNewData = false;
                    try
                    {
                        Monitor.Enter(ListCallBack);
                        foreach (var lcb in ListCallBack)
                        {
                            var us = lcb.US;
                            try
                            {
                                if (!us.IsOpen)
                                {
                                    lcb.CountFail++;
                                    continue;
                                }
                                var t = us.Context.GetCallbackChannel<IWcfInterfaceCallback>();
                                t.NewFileManager();
                                lcb.CountFail = 0;
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                        }

                        foreach (var lcb in ListCallBack.Where(x => x.CountFail > 10).ToList())
                        {
                            ListCallBack.Remove(lcb);
                        }
                    }
                    finally
                    {
                        Monitor.Exit(ListCallBack);
                    }

                }
                Thread.Sleep(2000);
            }
        }
        #endregion

        /// <summary>
        /// Отправка PUSH на сайт
        /// </summary>
        /// <param name="CODE_MO"></param>
        private void SendNewPackState(string CODE_MO)
        {
            ///Признак сайта бы к акаунту
            foreach(var us in BankAcc.GetEnumerable())
            {
                try
                {
                    if (us == null) continue;
                    if (!us.IsOpen) continue;
                    var t = us.Context.GetCallbackChannel<IWcfInterfaceCallback>();
                    t.NewPackState(CODE_MO);
                }
                catch (Exception)
                {
                    // ignored
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
                var CODE = SchemaChecking.GetCode_fromXML(item.FilePach, "CODE");
               
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
                    fi.FileLog.WriteLn("Файл имеет не уникальный CODE = " + val.Key);
                    fi.FileLog.WriteLn("Файлы с повтором: " + string.Join(",", val.Value.Where(x => x != fi).Select(x => x.FileName).ToArray()));
                    fi.FileLog.Close();
                }
            }
        }
        //Событие создание файлов
        private void fileSystemWatcher1_Created(object sender, FileSystemEventArgs e)
        {
            if (Path.GetExtension(e.Name)?.ToUpper() == ".XML")
            {
                var arc = false;
                if (FileFromArchive.Contains(e.Name.ToUpper()))
                {
                    arc = true;
                    FileFromArchive.Remove(e.Name.ToUpper());
                }
                FileList.Add(new FileListItem { path = e.FullPath, InArchive = arc });
                return;
            }
            if (Path.GetExtension(e.Name)?.ToUpper() == ".ZIP")
            {
                ArciveFileList.Add(new FileListItem { path = e.FullPath });
                return;
            }
            WcfInterface.AddLog("Файл " + e.Name + " не подлежит обработке(расширение не поддерживается)", EventLogEntryType.Error);

        }
        object Flag = new object(); //Флаг для блокировки потоков для Monitor
        /// <summary>
        /// Создать архив ZIP. Содержащий файл Message с текстом и файлы
        /// </summary>
        /// <param name="Message">Сообщение в файле</param>
        /// <param name="FilePath">Путь к архиву</param>
        /// <param name="Attachment">Список файлов</param>

        private void CreateMessage(string Message, string FilePath, params string[] Attachment)
        {
            Monitor.Enter(Flag);
            try
            {
                var buf = Encoding.UTF8.GetBytes(Message);
                var zfile = new ZipFile(Encoding.GetEncoding("cp866"));
                zfile.AddFiles(Attachment, "");
                zfile.AddEntry("message.txt", buf);
                var checkfile = FilePath;
                var x = 1;
                while (File.Exists(checkfile))
                {
                    checkfile = Path.Combine(Path.GetDirectoryName(FilePath),$"{Path.GetFileNameWithoutExtension(FilePath)}({x}){Path.GetExtension(FilePath)}");
                    x++;
                }
                FilePath = checkfile;

                var pathArchive = Path.Combine(ErrorArchivePath, Path.GetFileName(FilePath));

                if (!Directory.Exists(Path.GetDirectoryName(pathArchive)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(pathArchive));
                }
                zfile.Save(pathArchive);
                zfile.Save(FilePath);
                zfile.Dispose();
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Ошибка при создания сообщения ошибки({Message}){ex.Message}", EventLogEntryType.Error);
            }
            finally
            {
                Monitor.Exit(Flag);
            }
        }
        /// <summary>
        /// обработка файла архива ZIP
        /// </summary>
        private void ArchiveInviter()
        {
            var Name = "";
            while (AppConfig.Property.FILE_ON || ArciveFileList.Count != 0)
            {
                //Даем зазор в 0,5 секунды, бывает что файл 0 байт и открывается в процессе копирования. Вроде доступен с ПО, а потом бах и занят...
                var ArchiveFileListFiller = ArciveFileList.Where(x => (DateTime.Now - x.DateIN).Milliseconds > 500).ToList();
                if (ArchiveFileListFiller.Count == 0)
                {
                    Thread.Sleep(1500);
                    continue;
                }

                var item = ArchiveFileListFiller[0];
                var FullPath = item.path;
                Name = Path.GetFileName(FullPath);
                ArciveFileList.Remove(item);
                try
                {
                    SchemaChecking.PrichinAv? pr;
                    var NOT_FOUND = 0;
                    while (!SchemaChecking.CheckFileAv(FullPath, out pr))
                    {
                      
                        if (!pr.HasValue) continue;
                        if (pr.Value != SchemaChecking.PrichinAv.NOT_FOUND) continue;
                        WcfInterface.AddLog($"Не удалось найти файл {FullPath}", EventLogEntryType.Error);
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
                            foreach (var entry in FilesManager.GetFileNameInArchvie(FullPath))
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
                       
                        WcfInterface.AddLog($"Наименование файла {Name} не соответствует формату. Файл не принят в обработку!", EventLogEntryType.Warning);
                        CreateMessage($"Наименование файла {Name} не соответствует формату. Файл не принят в обработку!{Environment.NewLine}{fp.ErrText}", Path.Combine(AppConfig.Property.ErrorMessageFile, $"{GetErrorName(fp)}.zip"), FullPath);
                       
                        FilesManager.MoveFileTo(FullPath, Path.Combine(ErrorPath, CODE_MO, Name));
                    }
                    else
                    {
                        if (fp.Np == "75" && fp.Pp.Value == Penum.T || fp.Np == "75003" && fp.Pp.Value == Penum.S)
                        {
                            //Извлекаем файлы из архива
                            var tmpdir = Directory.CreateDirectory(AppConfig.Property.ProcessDir + "\\" + "tmp");
                            var t = FilesManager.FilesExtract(FullPath, tmpdir.FullName);
                            if (t.Result)
                            {
                                var mas = tmpdir.GetFiles();
                                foreach (var fi in mas)
                                {
                                    var name = fi.Name;
                                    while (File.Exists(AppConfig.Property.IncomingDir + "\\" + name))
                                    {
                                        name = Path.GetFileNameWithoutExtension(name) + "1" + Path.GetExtension(name);
                                    }
                                    if (name != fi.Name)
                                        WcfInterface.AddLog($"Произошла замена имени файла из {Name} для {fi.Name} на {name}", EventLogEntryType.Warning);
                                    FileFromArchive.Add(name.ToUpper());
                                    File.Move(fi.FullName, $"{AppConfig.Property.IncomingDir}\\{name}");

                                    if (!AppConfig.Property.AUTO)
                                    {
                                        FileList.Add(new FileListItem { path =$"{AppConfig.Property.IncomingDir}\\{name}", InArchive = true });
                                    }
                                }
                                Directory.Delete(tmpdir.FullName, true);
                                FilesManager.MoveFileTo(FullPath, Path.Combine(AppConfig.Property.InputDir, "Archive", DateTime.Now.ToString("yyyy_MM_dd"), "MAIL", fp.Ni, Name));
                            }
                            else
                            {
                                CreateMessage($"Файл {Name} не читается!!!", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(fp) + ".zip"), FullPath);
                                WcfInterface.AddLog($"Ошибка распаковки файла {Name}: {t.Exception}", EventLogEntryType.Warning);
                                FilesManager.MoveFileTo(FullPath, Path.Combine(ErrorPath, fp.Ni, Name));
                            }
                        }
                        else
                        {
                            CreateMessage($"В файле {Name} не верно указана организация-получатель", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(fp) + ".zip"), FullPath);
                            WcfInterface.AddLog($"В файле {Name} не верно указана организация-получатель", EventLogEntryType.Warning);
                            FilesManager.MoveFileTo(FullPath, Path.Combine(ErrorPath, fp.Ni, Name));
                        }
                    }
                }
                catch (Exception ex)
                {
                    WcfInterface.AddLog($"Ошибка в потоке приема архивов({Name}): {ex.Message}", EventLogEntryType.Error);
                }
            }
        }


  
        /// <summary>
        /// Обработка файла XML
        /// </summary>
        private void FilesInviter()
        {
            while (AppConfig.Property.FILE_ON || FileList.Count != 0)
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

                SchemaChecking.PrichinAv? pr;
                var flagcontinue = false;
                while (!SchemaChecking.CheckFileAv(FullPath, out pr))
                {
                    if (pr.HasValue)
                        if (pr.Value == SchemaChecking.PrichinAv.NOT_FOUND)
                        {
                            WcfInterface.AddLog($"Не удалось найти файл {FullPath}", EventLogEntryType.Error);
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

                        var codeMO = new byte[FP.Ni.Length];
                        for (var i = 0; i < FP.Ni.Length; i++)
                        {
                            codeMO[i] = Convert.ToByte(FP.Ni.Substring(i, 1));
                        }

                        FilePacket currentPack;
                        var indexpacket = FM.FindIndexPacket(codeMO);
                        if (indexpacket == -1)
                        {
                            currentPack = new FilePacket()
                            {
                                CodeMO = codeMO,
                                CaptionMO = "[наименование]",
                                Date = item.DateCreate,
                                Status = StatusFilePack.Open,
                                Files = new List<FileItem>()
                            };
                            currentPack.changeSiteStatus += onPackChanged;
                            currentPack.PropertyChanged += SendNewFileManager_PropertyChanged;
                            FM.Add(currentPack);
                            var th = new Thread(CloserPack) {IsBackground = true};
                            var itth = new ItemThreadList {pac = currentPack, th = th};
                            listTH.Add(itth);
                            var obj = new object[] { currentPack, true };
                            th.Start(obj); 
                        }
                        else
                        {
                            currentPack = FM[indexpacket];
                        }


                        if (currentPack.Status == StatusFilePack.Open)
                        {
                            try
                            {
                                currentPack.Files.Add(item);
                                currentPack.OnPropertyChanged("Files");
                                item.PropertyChanged += SendNewFileManager_PropertyChanged;
                                if (!item.IsArchive)
                                { 
                                    FilesManager.CopyFileTo(FullPath, Path.Combine(AppConfig.Property.InputDir, "Archive", DateTime.Now.ToString("yyyy_MM_dd"), "MAIL", FP.Ni, name));
                                    item.IsArchive = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                WcfInterface.AddLog($"Ошибка при копировании в архив{FullPath}: {ex.Message}", EventLogEntryType.Error);
                            }
                        }
                        else
                        {
                            DeletePack(currentPack);
                            FileList.Insert(0, Fitem);
                        }
                    }
                    else
                    {
                        WcfInterface.AddLog($"В файле {name} не верно указана организация-получатель", EventLogEntryType.Warning);
                        CreateMessage($"В файле {name} не верно указана организация-получатель", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(FP) + ".zip"), FullPath);
                        FilesManager.MoveFileTo(FullPath, Path.Combine(ErrorPath, FP.Ni, name));
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
                        WcfInterface.AddLog($"Имя файла {name} не корректно. Файл не принят в обработку!", EventLogEntryType.Warning);
                        CreateMessage($"Имя файла {name} не корректно. Файл не принят в обработку!{Environment.NewLine}{FP.ErrText}", Path.Combine(AppConfig.Property.ErrorMessageFile, GetErrorName(FP) + ".ZIP"), FullPath);
                        FilesManager.MoveFileTo(FullPath, Path.Combine(ErrorPath, CODE_MO, name));
                    }
                    catch (Exception ex)
                    {
                        WcfInterface.AddLog($"Ошибка при переносе {FullPath}: {ex.Message}", EventLogEntryType.Error);
                    }
                }
            }

        }
        private string GetErrorName(FilePacket FP)
        {
            return $"{FP.codeMOstr} результат приема МП от {DateTime.Now:dd_MM_yyyy HH_mm}";
        }
        private string GetErrorName(MatchParseFileName FP)
        {
            return FP.Ni!=null ? $"{FP.Ni} ошибки МП от {DateTime.Now:dd_MM_yyyy HH_mm}" : $"UNKNOWN ошибки МП от {DateTime.Now:dd_MM_yyyy HH_mm}";
        }

        /// <summary>
        /// Добавления пакета с сайта
        /// </summary>
        /// <param name="newPack"></param>
        private void AddPacket(FilePacket newPack)
        {
            var indexpacket = FM.FindIndexPacket(newPack.CodeMO);
            if (indexpacket != -1)
            {
                DeletePack(FM[indexpacket]);
            }

            newPack.changeSiteStatus += onPackChanged;
            newPack.PropertyChanged += SendNewFileManager_PropertyChanged;
            foreach (var item in newPack.Files)
            {
                item.PropertyChanged += SendNewFileManager_PropertyChanged;
                if(item.filel!=null)
                    item.filel.PropertyChanged += SendNewFileManager_PropertyChanged;
            }
            FM.Add(newPack);
            var fail = false;
            try
            {
                //Файлы в архив приема
                foreach (var fi in newPack.Files)
                {
                    newPack.CommentSite = "Перенос файлов";
                    newPack.Comment = "Перенос файлов в архив приема";
                    var name = Path.GetFileName(fi.FilePach);
                    var ID = "NOT_ID";
                    if (newPack.ID.HasValue)
                        ID = newPack.ID.Value.ToString();
                    var DIR = Path.Combine(AppConfig.Property.InputDir, "Archive", DateTime.Now.ToString("yyyy_MM_dd"),
                        "SITE", newPack.codeMOstr, ID);
                    FilesManager.CopyFileTo(fi.FilePach, Path.Combine(DIR, name));
                    name = Path.GetFileName(fi.filel.FilePach);
                    FilesManager.CopyFileTo(fi.filel.FilePach, Path.Combine(DIR, name));
                }
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Ошибка при переносе файлов в архив с сайта:{ex.Message}",
                    EventLogEntryType.Error);
                fail = true;
            }

            if (!fail)
            {
                var th = new Thread(CloserPack) {IsBackground = true};
                var itth = new ItemThreadList {pac = newPack, th = th};
                listTH.Add(itth);
                var obj = new object[] {newPack, true};
                th.Start(obj);
            }
            else
            {
                newPack.CommentSite = "Что то пошло не так...";
                newPack.Status = StatusFilePack.FLKERR;
            }
        }




        void BreakDBInvite(string CODE_MO)
        {
            if (DBinvitePac == null) return;
            if (DBinvitePac.codeMOstr != CODE_MO) return;
            WcfInterface.AddLog("Прерывание потока переноса в БД для " + CODE_MO, EventLogEntryType.Error);
            wi.BDinviteCancellationTokenSource.Cancel();
            currmyBD?.Dispose();
            WcfInterface.AddLog("Ожидание прерывания потока переноса в БД для " + CODE_MO, EventLogEntryType.Error);
            while (wi.BDinvite.ThreadState == ThreadState.Running)
            {
                Thread.Sleep(1000);
            }
            currmyBD = null;
            DBinvitePac = null;
            wi.BDinvite = new Thread(backgroundWorkerBD_DoWork) {IsBackground = true};
            wi.SetThreadBDinvite(backgroundWorkerBD_DoWork);
            wi.BDinvite.Start();
            WcfInterface.AddLog("Прерывание успешно", EventLogEntryType.Error);
        }


        void DeleteTHSchema(string CODE_MO)
        {
            var THitem = listTH.Find(itemThreadList => itemThreadList.pac.codeMOstr == CODE_MO);
            if (THitem != null)
            {
                WcfInterface.AddLog("Прерывание потока выполнения проверки схемы для " + CODE_MO, EventLogEntryType.Error);
                THitem.th.Abort();
                WcfInterface.AddLog("Ожидание прерывание потока выполнения проверки схемы для " + CODE_MO, EventLogEntryType.Error);
                while (THitem.th.ThreadState == System.Threading.ThreadState.Running)
                {
                }
                WcfInterface.AddLog("Прерывание успешно", EventLogEntryType.Error);
            }
        }


        void DeletePack(FilePacket currentPack)
        {
            DeleteTHSchema(currentPack.codeMOstr);
            BreakDBInvite(currentPack.codeMOstr);
            currentPack.CloserLogFiles();
            FM.Remove(currentPack);
        }


        void BreakPack(FilePacket currentPack)
        {
            DeleteTHSchema(currentPack.codeMOstr);
            BreakDBInvite(currentPack.codeMOstr);
            currentPack.CloserLogFiles();
        }


        private int FindRowSVODTBL(DataTable tbl, string FileName)
        {
            var i = 0;
            if (tbl == null)
                return -1;
            if (!tbl.Columns.Contains("FILENAME"))
                return -1;
            foreach (DataRow row in tbl.Rows)
            {
                if (Path.GetFileNameWithoutExtension(row["FILENAME"].ToString().ToUpper()) == Path.GetFileNameWithoutExtension(FileName.ToUpper()))
                    return i;
                i++;
            }
            return -1;
        }
     }
}

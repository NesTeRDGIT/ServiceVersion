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
using ExcelManager;
using Ionic.Zip;
using ServiceLoaderMedpomData.EntityMP_V31;
using ThreadState = System.Threading.ThreadState;

//Общая концепция
//Мониторим файлы в каталоге после прихода файла посылаем ешл на обработку. На каждый файл свой поток. Работа с файлами проходит в виде пакетов.
//Когда файл прошел схему ставим ему процесс(см. классы) схемаОК. И его подхватит поток переноса в БД. Загружаем все файлы пакета(т.е ЛПУ) в БД. Делаем флк пишем результат

namespace MedpomService
{
    public partial class MedpomService : ServiceBase
    {
       WcfInterface wi;//ВКФ сервис. Взаимодействие с сервером 
        
        public static string SysLog = "";
        public static string SysPass = "";

        public MedpomService()
        {
            InitializeComponent();
            _MyOracleProvider = new MyOracleProvider(Logger);
        }


        private const string pathToFM = "FM.dat";

        private IPacketQuery PacketQuery;
        private IMessageMO MessageMO;
        private IRepository mybd;
        private IExcelProtokol ExcelProtokol;
        private ISchemaCheck SchemaCheck;
        private IPacketCreator PacketCreator;
        private IProcessReestr ProcessReestr;
        private IFileInviter FileInviter;
        private ILogger Logger = new LoggerEventLog("MedpomServiceLog");
        private MyOracleProvider _MyOracleProvider;

       // private ChekingList chList;

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
                    Logger.AddLog("Ошибка файла SYS_PWD.txt:" + ex.Message, LogType.Error);
                }
                Logger.AddLog("Старт службы " + ServiceName, LogType.Information);

                MessageMO = new MessageMO(Logger);
                mybd = CreateMyBD();
                ExcelProtokol = new ExcelProtokol(Logger);
                SchemaCheck = new SchemaCheck(mybd, MessageMO, ExcelProtokol, Logger);

                PacketCreator = new PacketCreator(mybd, SchemaCheck, Logger);
                PacketQuery = new PacketQuery(PacketCreator,  SchemaCheck);
                ProcessReestr = new ProcessReestr(PacketQuery, ExcelProtokol, MessageMO, Logger);
                PacketQuery.SetProcessReestr(ProcessReestr);
                FileInviter = new FileInviter(MessageMO, PacketQuery, Logger);

                try
                {
                    PacketQuery.LoadFromFile(Path.Combine(localDir, pathToFM));
                    PacketQuery.PropertyChanged += SendNewFileManager_PropertyChanged;
                    PacketQuery.changeSiteStatus += onPackChanged;
                }
                catch (Exception ex)
                {
                    Logger.AddLog("Ошибка при загрузке списка пакетов: " + ex.Message, LogType.Error);
                }

                Logger.AddLog($"Чтение конфигурации XML-схем:{localDir}\\schemaset.dat", LogType.Information);
                SchemaCheck.LoadSchemaCollection($"{localDir}\\schemaset.dat");



                //////////////////////////////////////////////////////

                Logger.AddLog("Запуск сервера WCF", LogType.Information);

                if (StartServer())
                    Logger.AddLog("WCF сервер запущен", LogType.Information);
                else
                    Stop();
            
                if (AppConfig.Property.FILE_ON)
                    wi.StartProcess(AppConfig.Property.MainTypePriem, AppConfig.Property.AUTO, AppConfig.Property.OtchetDate);

                var th = new Thread(SendNewFileManagerThread);
                th.Start();
            }
            catch(Exception ex)
            {
                Logger.AddLog($"Ошибка запуска службы: {ex.Message}", LogType.Error);
                Stop();
            }
        }
        protected override void OnStop()
        {
            try
            {
                PacketQuery.SaveToFile(Path.Combine(localDir, "FM.dat"));
            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка при сохранении списка пакетов: {ex.Message}", LogType.Error);
            }
            Logger.AddLog("Остановка WCF сервера", LogType.Information);
            cancelSendNewFileManagerTh.Cancel();
            WcfConection.Abort();
            Logger.AddLog("Служба остановлена", LogType.Information);
        }

     

        private void SendNewFileManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            AddSendNewFileManager();
        }
      
        private void onPackChanged(FilePacket fp)
        {
            SendNewPackState(fp.CodeMO);
        }
        public static ServiceHost WcfConection { set; get; }
        private bool StartServer()
        {
            try
            {
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

                wi = new WcfInterface(ProcessReestr, SchemaCheck, FileInviter, PacketQuery, Logger);
                WcfConection = new ServiceHost(wi, new Uri(uri));
                wi.RaiseRegisterNewFileManager += Wi_RaiseRegisterNewFileManager;
                var myEndpointAdd =new EndpointAddress(new Uri(uri), EndpointIdentity.CreateDnsIdentity("MSERVICE"));
                var ep = WcfConection.AddServiceEndpoint(typeof(IWcfInterface), netTcpBinding, "");
                ep.Address = myEndpointAdd;

                WcfConection.OpenTimeout = new TimeSpan(24, 0, 0);
                WcfConection.CloseTimeout = new TimeSpan(24, 0, 0);

                netTcpBinding.Security.Mode = SecurityMode.Message;
                netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;




                WcfConection.Credentials.UserNameAuthentication.UserNamePasswordValidationMode =System.ServiceModel.Security.UserNamePasswordValidationMode.Custom;
                WcfConection.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator =new MyCustomUserNameValidator(_MyOracleProvider);
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
                // WcfInterface.AddLog("Включение имперсонализации для WCF", EventLogEntryType.Information);
                //  wi.SetImpers();
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка при запуске WCF: {ex.Message}", LogType.Error);
                return false;
            }
        }

    

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

        #region Механизм уведомления изменения данных

        private void AddSendNewFileManager()
        {
            HasFileManagerNewData = true;
        }
      
        private void Wi_RaiseRegisterNewFileManager(USER us, bool register)
        {
            Monitor.Enter(ListCallBack);
            try
            {
                if (register)
                {
                    if (!ListCallBack.Contains(new USER_FMT(us)))
                    {
                        ListCallBack.Add(new USER_FMT(us));
                    }
                }
                else
                {
                    var item = ListCallBack.FirstOrDefault(x => x.US == us);
                    if (item == null) return;
                    if (ListCallBack.Contains(item))
                    {
                        ListCallBack.Remove(item);
                    }
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
            //Признак сайта бы к акаунту
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
   

     }
}

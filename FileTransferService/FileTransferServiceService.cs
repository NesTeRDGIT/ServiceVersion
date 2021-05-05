using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using FileTransferContract;
using FileTransferContract.Class;

namespace FileTransferService
{
    public partial class FileTransferServiceService : ServiceBase
    {
        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;

        ILogger logger = new LoggerEventLog("FileTransferService");
        private IWCFContract wi;
        private IRuleManager ruleManager;
        private IRepository repository = new FileRepository(Path.Combine(LocalFolder, "Setting.xml"));
        public FileTransferServiceService()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            try
            {
                logger.Add($"Старт службы {ServiceName}", false);
                ruleManager = new RuleManager(repository, logger);
                if (StartServer())
                    logger.Add("WCF сервер запущен", false);
                else
                    Stop();
            }
            catch (Exception ex)
            {
                logger.Add($"Ошибка запуска службы: {ex.Message}", true);
                Stop();
            }
        }
        public static ServiceHost WcfConection { set; get; }
        private bool StartServer()
        {
            try
            {
                //Адрес, который будет прослушивать сервер
                const string uri = @"net.tcp://localhost:12344/FileTransferService.svc"; 
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

                wi = new WCF(logger, ruleManager);
                WcfConection = new ServiceHost(wi, new Uri(uri));
 
                var myEndpointAdd = new EndpointAddress(new Uri(uri));
                var ep = WcfConection.AddServiceEndpoint(typeof(IWCFContract), netTcpBinding, "");
                ep.Address = myEndpointAdd;

                WcfConection.OpenTimeout = new TimeSpan(24, 0, 0);
                WcfConection.CloseTimeout = new TimeSpan(24, 0, 0);
                netTcpBinding.Security.Mode = SecurityMode.None;


                WcfConection.Open();
                return true;
            }
            catch (Exception ex)
            {
                logger.Add($"Ошибка при запуске WCF: {ex.Message}", true);
                return false;
            }
        }

        protected override void OnStop()
        {

            ruleManager.Dispose();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public static IWcfInterface wcf { set; get; }
        public static IWcfInterfaceCallback callback;
        public static List<string> SecureCard { get; set; } = new List<string>();
        public LoginFormVM VM { get; set; }
        private  void OnConnect(IWcfInterface arg1, MyServiceCallback arg2, List<string> arg3)
        {
            wcf = arg1;
            callback = arg2;
            SecureCard = arg3;
            DialogResult = true;
            arg2.OnNewFileManager += Callback_OnNewFileManager;
        }
        private void Callback_OnNewFileManager()
        {
            try
            {
                foreach (var win in GetWindow())
                {
                    win.UpdateList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private List<FilesManagerView> GetWindow()
        {
            var result = new List<FilesManagerView>();
            Dispatcher.Invoke(()=>
            {
                foreach (var win in Application.Current.Windows)
                {
                    if (win is FilesManagerView item)
                    {
                        result.Add(item);
                    }
                }
            });
            return result;
        }

        public LoginForm()
        {
            SecureCard = new List<string>();
            wcf = null;
            VM = new LoginFormVM(Dispatcher.CurrentDispatcher, OnConnect)
            {
                Password = ProtectStr.UnprotectString(Properties.Settings.Default.PASSWORD), 
                Login = ProtectStr.UnprotectString(Properties.Settings.Default.USER_NAME), 
                HOST = Properties.Settings.Default.IP_CONNECT, SaveLogAndPass = Properties.Settings.Default.SAVE_LOG_AND_PASS
            };
            InitializeComponent();
             PasswordBox.Password = VM.Password;
        }

        public string DIALOG_MESSAGE { get; set; } = null;
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            if (!string.IsNullOrEmpty(DIALOG_MESSAGE))
            {
                MessageBox.Show(DIALOG_MESSAGE);
            }
        }
        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            VM.Password = PasswordBox.Password;
        }
    }


    public class LoginFormVM : INotifyPropertyChanged
    {
        private IWcfInterface wcf { set; get; }
        private MyServiceCallback callback { set; get; }
        private Dispatcher dispatcher { set; get; }

        private Action<IWcfInterface, MyServiceCallback, List<string>> onConnect;

        public LoginFormVM(Dispatcher dispatcher, Action<IWcfInterface, MyServiceCallback, List<string>> onConnect)
        {
            this.dispatcher = dispatcher;
            this.onConnect = onConnect;
        }

        private string _HOST { get; set; }
        public string HOST
        {
            get => _HOST;
            set
            {
                _HOST = value;
                RaisePropertyChanged();
            }
        }

        private string _Login { get; set; }
        public string Login
        {
            get => _Login;
            set
            {
                _Login = value;
                RaisePropertyChanged();
            }
        }
      
        private string _Password { get; set; }
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                RaisePropertyChanged();
            }
        }

      

        private bool _SaveLogAndPass { get; set; }
        public bool SaveLogAndPass
        {
            get => _SaveLogAndPass;
            set
            {
                _SaveLogAndPass = value;
                RaisePropertyChanged();
            }
        }

        private string _Title { get; set; }
        public string Title
        {
            get => _Title;
            set
            {
                _Title = value;
                dispatcher.Invoke(() =>{ RaisePropertyChanged();});
            }
        }


        private bool _Connecting { get; set; }
        public bool Connecting
        {
            get => _Connecting;
            set
            {
                _Connecting = value;
                dispatcher.Invoke(() => { RaisePropertyChanged(); });
            }
        }
        public ICommand ConnectCommand=> new Command(obj=>
        {
            try
            {
                Connecting = true;
                Title = "Сохранение параметров";
                if (SaveLogAndPass)
                {
                    Properties.Settings.Default.PASSWORD = ProtectStr.ProtectString(Password);
                    Properties.Settings.Default.USER_NAME = ProtectStr.ProtectString(Login);
                    Properties.Settings.Default.IP_CONNECT = HOST;
                }

                Properties.Settings.Default.SAVE_LOG_AND_PASS = SaveLogAndPass;
                Properties.Settings.Default.Save();
                Task.Run(Connect);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.FullMessage());
            }
        });

        public void Connect()
        {
            try
            {
                Title = "Создание канала";
                CreateChannel();
                Title = "Запрос прав";
                var SecureCard = wcf.Connect();
                dispatcher.Invoke(() => { onConnect.Invoke(wcf, callback, SecureCard); });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.FullMessage());
            }
            finally
            {
                Connecting = false;
            }
        }

        private void CreateChannel()
        {
            var addr = $@"net.tcp://{HOST}:12344/TFOMSMEDPOM.svc"; // Адрес сервиса
            var tcpUri = new Uri(addr);
            var address = new EndpointAddress(tcpUri, EndpointIdentity.CreateDnsIdentity("MSERVICE"));
            var netTcpBinding = new NetTcpBinding(SecurityMode.None)
            {
                ReaderQuotas = {MaxArrayLength = int.MaxValue, MaxBytesPerRead = int.MaxValue, MaxStringContentLength = int.MaxValue},
                MaxBufferPoolSize = 105000000,
                MaxReceivedMessageSize = 105000000,
                SendTimeout = new TimeSpan(24, 0, 0),
                ReceiveTimeout = new TimeSpan(24, 0, 0),
                Security = {Mode = SecurityMode.TransportWithMessageCredential,Message ={ ClientCredentialType = MessageCredentialType.UserName}, Transport = {ClientCredentialType = TcpClientCredentialType.None}}
            };

            callback = new MyServiceCallback();
            var instanceContext = new InstanceContext(callback);
            var factory = new DuplexChannelFactory<IWcfInterface>(instanceContext, netTcpBinding, address);

            factory.Credentials.UserName.UserName = Login;
            factory.Credentials.UserName.Password = Password;
            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            wcf = factory.CreateChannel(); // Создаём само подключение         
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

}

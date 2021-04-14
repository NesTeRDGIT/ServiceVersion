using System;
using System.Collections.Generic;
using System.Linq;
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
using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT;
using ServiceLoaderMedpomData;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для LoginForm.xaml
    /// </summary>
    public partial class LoginForm : Window
    {
        public static IWcfInterface wcf { set; get; }
        public MyServiceCallback callback;
        public string DIALOG_MESSAGE = null;

        public LoginForm()
        {
            InitializeComponent();
            passwordBoxPass.Password = ProtectStr.UnprotectString(Properties.Settings.Default.PASSWORD);
            textBoxUserName.Text = ProtectStr.UnprotectString(Properties.Settings.Default.USER_NAME);
            textBoxHOST.Text = Properties.Settings.Default.IP_CONNECT;
        }

        void StartAnimateButton1()
        {
            var s = (DoubleAnimation)this.FindResource("DA");
            var rgb = (RadialGradientBrush)button1.Background;
            s.RepeatBehavior = RepeatBehavior.Forever;
            rgb.GradientStops[0].BeginAnimation(GradientStop.OffsetProperty, s);
        }
        void StopAnimateButton1()
        {
            var s = (DoubleAnimation)this.FindResource("DA");

            var rgb = (RadialGradientBrush)button1.Background;
            s.RepeatBehavior = new RepeatBehavior(0);

            rgb.GradientStops[0].BeginAnimation(GradientStop.OffsetProperty, s);
            //rgb.GradientStops[0].Offset = Convert.ToDouble( 0.0);

        }

        private bool ChekData()
        {
            try
            {
                var ca = ((ColorAnimation)(this.FindResource("CA")));
                var result = true;
                if (passwordBoxPass.Password.Trim() == "")
                {
                    result = false;
                    passwordBoxPass.BorderBrush = new SolidColorBrush();
                    passwordBoxPass.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);
                }

                if (textBoxUserName.Text.Trim() == "")
                {
                    result = false;
                    textBoxUserName.BorderBrush = new SolidColorBrush();
                    textBoxUserName.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);

                }

                if (textBoxHOST.Text.Trim() == "")
                {
                    result = false;
                    textBoxHOST.BorderBrush = new SolidColorBrush();
                    textBoxHOST.BorderBrush.BeginAnimation(SolidColorBrush.ColorProperty, ca);

                }

                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ChekData())
                {

                    if (!button1.IsHitTestVisible) return;
                    button1.IsHitTestVisible = false;
                    button1.Focusable = false;
                    StartAnimateButton1();
                    var th = new Thread(ThreadConnect) { IsBackground = true };
                    th.Start();
                }

            }
            catch (Exception ex)
            {
                button1.IsHitTestVisible = true;
                button1.Focusable = true;
                ;
                MessageBox.Show(ex.Message);
                StopAnimateButton1();
            }
        }

        public static List<string> SecureCard = new List<string>();
     

        private string Log { get; set; }

        private string PASS { get; set; }

        public void ThreadConnect()
        {
            try
            {

                Dispatcher?.Invoke(new Action(() =>
                {
                    if (checkBox1.IsChecked == true)
                    {
                        Properties.Settings.Default.PASSWORD = ProtectStr.ProtectString(passwordBoxPass.Password);
                        Properties.Settings.Default.USER_NAME = ProtectStr.ProtectString(textBoxUserName.Text);
                    }
                    Log = textBoxUserName.Text;
                    PASS = passwordBoxPass.Password;
                    Properties.Settings.Default.IP_CONNECT = textBoxHOST.Text;
                }));

                Connect();
                Dispatcher?.BeginInvoke(new Action(() => { this.Title = "Подключение: Запрос прав"; }));
                SecureCard =  wcf.Connect();
                Dispatcher?.BeginInvoke(new Action(() => { this.Title = "Подключение: Сохранение параметров"; }));
                Properties.Settings.Default.Save();
                Dispatcher?.Invoke(new Action(() =>
                {
                    this.Title = "Подключение";
                    StopAnimateButton1();
                    this.DialogResult = true;
                }));
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.FullMessage());
                Dispatcher?.Invoke(() =>
                {
                    button1.IsHitTestVisible = true;
                    button1.Focusable = true;
                    StopAnimateButton1();
                });
            }
        }

        private void Connect()
        {
            var addr = @"net.tcp://" + Properties.Settings.Default.IP_CONNECT + ":12344/TFOMSMEDPOM.svc"; // Адрес сервиса
            var tcpUri = new Uri(addr);
            var address = new EndpointAddress(tcpUri, EndpointIdentity.CreateDnsIdentity("MSERVICE"));

            var t = address.Identity;


            //  BasicHttpBinding basicHttpBinding = new BasicHttpBinding(BasicHttpSecurityMode.None); //HTTP!
            var netTcpBinding = new NetTcpBinding(SecurityMode.None);


            // Ниже строки для того, чтоб пролазили таблицы развером побольше
            netTcpBinding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            netTcpBinding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            netTcpBinding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            netTcpBinding.MaxBufferPoolSize = 105000000;
            netTcpBinding.MaxReceivedMessageSize = 105000000;
            netTcpBinding.SendTimeout = new TimeSpan(24, 0, 0);
            netTcpBinding.ReceiveTimeout = new TimeSpan(24, 0, 0);

            netTcpBinding.Security.Mode = SecurityMode.Message;
            netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.None;





            callback = new MyServiceCallback();
            callback.OnNewFileManager += Callback_OnNewFileManager;
            var instanceContext = new InstanceContext(callback);

            var factory = new DuplexChannelFactory<IWcfInterface>(instanceContext, netTcpBinding, address);

            factory.Credentials.UserName.UserName = Log;
            factory.Credentials.UserName.Password = PASS;

            factory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.None;
            //factory.Credentials.ClientCertificate.SetCertificate(StoreLocation.CurrentUser, StoreName.My, X509FindType.FindBySubjectName, "MSERVICE");
            wcf = factory.CreateChannel(); // Создаём само подключение         
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (!string.IsNullOrEmpty(DIALOG_MESSAGE))
            {
                MessageBox.Show(DIALOG_MESSAGE);
            }

        }
        private void Callback_OnNewFileManager()
        {
            Dispatcher.Invoke(() =>
            {
                try
                {
                    foreach (var win in Application.Current.Windows)
                    {
                        (win as FilesManagerView)?.UpdateList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            });
        }

        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SAVE_LOG_AND_PASS = checkBox1.IsChecked.Value;
        }

        private void textBoxHOST_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                button1_Click(null, null);
            }
        }

    }

}

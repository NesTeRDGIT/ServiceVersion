using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ServiceLoaderMedpomData;
using System.ServiceModel;
namespace ClientServise
{
    public partial class LoginForm : Form
    {


        public IWcfInterface MyWcfConnection;//{ set; get; }
        public MyServiceCallback callback;
        public string DIALOG_MESSAGE = null;
        public LoginForm()
        {
            InitializeComponent();
            SecureCard = new List<string>();
            passwordBoxPass.Text = Properties.Settings.Default.PASSWORD;
            textBoxUserName.Text = Properties.Settings.Default.USER_NAME;
            textBoxHOST.Text = Properties.Settings.Default.IP_CONNECT;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChekData())
                {
                    if (!button1.Enabled) return;
                    button1.Enabled = false;              
                    var th = new Thread(ThreadConnect);
                    th.Start();
                }
            }
            catch (Exception ex)
            {
                button1.Enabled = false;
                MessageBox.Show(ex.Message);
            }
        }

        private bool ChekData()
        {
            try
            {
                var result = passwordBoxPass.Text.Trim() != "";
                if (textBoxUserName.Text.Trim() == "")
                {
                    result = false;
                }
                if (textBoxHOST.Text.Trim() == "")
                {
                    result = false;
                }
                return result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }


        public static List<string> SecureCard;
        public static int ID = -1;
        string Log = "";
        string PASS = "";
        public void ThreadConnect()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    if (checkBox1.Checked)
                    {
                        Properties.Settings.Default.PASSWORD = passwordBoxPass.Text;
                        Properties.Settings.Default.USER_NAME = textBoxUserName.Text;
                    }
                    Log = textBoxUserName.Text;
                    PASS = passwordBoxPass.Text;
                    Properties.Settings.Default.IP_CONNECT = textBoxHOST.Text;
                    Properties.Settings.Default.SAVE_LOG_AND_PASS = checkBox1.Checked;
                }));

                Connect();
                SecureCard = MyWcfConnection.Connect();
                //ID = MyWcfConnection.GetMyID();
                Properties.Settings.Default.Save();
                this.Invoke(new Action(() =>
                {                   
                    this.DialogResult =  System.Windows.Forms.DialogResult.OK;
                }));
            }

            catch (Exception ex)
            {
                var errr = "";
                var ex1 = ex;
                errr = ex.Message;
                while (ex1.InnerException != null)
                {
                    ex1 = ex1.InnerException;
                    errr += Environment.NewLine + ex1.Source + ": " + ex1.Message + ";";

                }
                MessageBox.Show($@"{ex1.Message}{Environment.NewLine}Полный текст ошибки: {errr}");


                this.Invoke(new Action(() =>
                {
                    button1.Enabled = true;
                }));
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
            this.MyWcfConnection = factory.CreateChannel(); // Создаём само подключение         
        }

        private void Callback_OnNewFileManager()
        {
            try
            {
                foreach (Form openForm in Application.OpenForms)
                {
                    (openForm as FilesManagerView)?.UpdateList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            checkBox1.Checked = Properties.Settings.Default.SAVE_LOG_AND_PASS;
        }

        private void textBoxHOST_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                button1_Click(null, null);
            }
        }
        
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IdentityModel.Selectors;
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Security;
using System.Security.AccessControl;
using System.ServiceModel;
using System.Windows.Forms;
//using Ionic.Zip;
using ServiceLoaderMedpomData;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;
using Oracle.ManagedDataAccess.Client;
using DocumentFormat.OpenXml.Validation;
using DocumentFormat.OpenXml.Packaging;

namespace ClientServise
{
    public partial class MainForm : Form
    {
        public static IWcfInterface MyWcfConnection;
        public static MyServiceCallback callback { set; get; }
         bool closed_onSTART = false;
        public MainForm()
        {
           
            var f = new LoginForm();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {

                MyWcfConnection = f.MyWcfConnection;
                callback = f.callback;
               
                var form = new Launcher.Launcher();
                form.ShowDialog();
             
                if (form.RESTART)
                {
                    closed_onSTART = true;
                }
                ((ICommunicationObject)wcf).Faulted += LoginForm_Faulted;
                ((ICommunicationObject)wcf).Closed += LoginForm_Faulted;

            }
            else
            {
                closed_onSTART = false;
            }

            InitializeComponent();

            if (!closed_onSTART)
            {
                SetControlForm(LoginForm.SecureCard);
                RefreshStatusOperation();
            
            }
        }

        public IWcfInterface wcf => MyWcfConnection;


        void LoginForm_Faulted(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
               {
                   var f = new LoginForm {DIALOG_MESSAGE = "Связь с сервером потеряна!"};

                   if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                   {
                       MyWcfConnection = f.MyWcfConnection;
                       callback = f.callback;                       
                       var form = new Launcher.Launcher();
                       form.ShowDialog();
                       if (form.RESTART)
                       {
                           user_closed = true;
                           this.Close();
                           return;
                       }
                       ((ICommunicationObject)wcf).Faulted += LoginForm_Faulted;
                       ((ICommunicationObject)wcf).Closed += LoginForm_Faulted;
                   }

                   SetControlForm(LoginForm.SecureCard);
                   RefreshStatusOperation();
                   
                  
               }));


        }

    
        public static bool PLAN_DISCONNECT = false;

        bool activivty = false;
        private void RefreshStatusOperation()
        {
            try
            {
                if (button9.Enabled)
                {

                    activivty = false;
                    if (MyWcfConnection.FilesInviterStatus())
                    {
                        listView1.Items[0].ForeColor = Color.LawnGreen;
                        listView1.Items[0].SubItems[1].Text = @"Активно";
                        activivty = true;
                    }
                    else
                    {
                        listView1.Items[0].ForeColor = Color.Red;
                        listView1.Items[0].SubItems[1].Text = @"Не активно";

                    }
                    if (MyWcfConnection.ArchiveInviterStatus())
                    {
                        listView1.Items[1].ForeColor = Color.LawnGreen;
                        listView1.Items[1].SubItems[1].Text = @"Активно";
                        activivty = true;
                    }
                    else
                    {
                        listView1.Items[1].ForeColor = Color.Red;
                        listView1.Items[1].SubItems[1].Text = @"Не активно";
                    }
                    if (MyWcfConnection.FLKInviterStatus())
                    {
                        listView1.Items[2].ForeColor = Color.LawnGreen;
                        listView1.Items[2].SubItems[1].Text = @"Активно";
                        activivty = true;
                    }
                    else
                    {
                        listView1.Items[2].ForeColor = Color.Red;
                        listView1.Items[2].SubItems[1].Text = @"Не активно";
                    }

                    if (MyWcfConnection.GetAutoFileAdd())
                    {
                        listView1.Items[3].ForeColor = Color.LawnGreen;
                        listView1.Items[3].SubItems[1].Text = @"Активно";
                        activivty = true;
                        button8.Enabled = false;
                    }
                    else
                    {
                        listView1.Items[3].ForeColor = Color.Red;
                        listView1.Items[3].SubItems[1].Text = @"Не активно";
                        button8.Enabled = true;
                    }


                    if (!activivty)
                    {
                        button5.Enabled = true;
                        button5.Text = @"Параметры";


                        button5.Enabled = LoginForm.SecureCard.Contains("GetSettingsFolder") && LoginForm.SecureCard.Contains("GetSettingConnect") &&
                            LoginForm.SecureCard.Contains("GetSchemaColection") &&
                            LoginForm.SecureCard.Contains("GetSchemaColection") && LoginForm.SecureCard.Contains("GetSettingTransfer") &&
                            LoginForm.SecureCard.Contains("GetListTransfer") && LoginForm.SecureCard.Contains("GetListTransfer") &&
                            LoginForm.SecureCard.Contains("GetUserPriv") && LoginForm.SecureCard.Contains("GetChekingList") && LoginForm.SecureCard.Contains("GetCheckClearProc");
                        radioButtonMainPriem.Enabled = radioButtonPredvPriem.Enabled = dateTimePicker1.Enabled = button7.Enabled = true;

                        button8.Enabled = false;
                        button6.Enabled = false;
                    }
                    else
                    {
                        button5.Enabled = false;
                        button5.Text = @"Параметры
(Для изменения остановите мониторинг)";

                         radioButtonMainPriem.Enabled =  radioButtonPredvPriem.Enabled = dateTimePicker1.Enabled = button7.Enabled = false;
                        button6.Enabled = true;
                    }


                    if (MyWcfConnection.GetTypePriem())
                    {
                        radioButtonMainPriem.Checked = true;
                    }
                    else
                    {
                        radioButtonPredvPriem.Checked = true;
                    }

                    if (MyWcfConnection.GetAutoPriem())
                    {
                        radioButtonAuto.Checked = true;
                    }
                    else
                    {
                        radioButtonHAND.Checked = true;
                    }

                    dateTimePicker1.Value = MyWcfConnection.GetOtchetDate();
                }
                if (button3.Enabled)
                {
                    if (thERR == null)
                    {
                        thERR = new Thread(checkErr) {IsBackground = true};
                        thERR.Start();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        Thread thERR = null;
        private void button1_Click(object sender, EventArgs e)
        {
            var form = new FilesManagerView(!activivty, LoginForm.SecureCard);
            form.Owner = this;
            form.Show();
        }
        Thread PingThread;

        private void Form1_Load(object sender, EventArgs e)
        {
            if (closed_onSTART)
            {
                user_closed = true;
                this.Close();
                return;
            }
            if (Properties.Settings.Default.ShowUpdate)
            {
           //     Launcher.UpdateInfo win = new Launcher.UpdateInfo();
             //   win.ShowDialog();
            }

            PingThread = new Thread(Pinging) {IsBackground = true};
            PingThread.Start();

        }

        void Pinging()
        {
            while (true)
            {
                try
                {
                    if (wcf != null)
                    {
                        if (((ICommunicationObject)wcf).State == CommunicationState.Opened)
                        {
                            wcf.Ping();
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($@"Не удалось подтвердить связь с сервером: {ex.Message}");
                    }));
                }
                Thread.Sleep(1800000);
                //Thread.Sleep(30000);
            }
        }

        void checkErr()
        {
            while (true)
            {
                try
                {
                    if (wcf != null)
                    {
                        if (((ICommunicationObject)wcf).State == CommunicationState.Opened)
                        {
                            this.Invoke(new Action(() =>
                            {
                                button3_Click(button3, new EventArgs()); 
                            }));
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($@"Ошибка в потоке проверки ошибок: {ex.Message}");
                    }));
                }
                Thread.Sleep(600000);
                //Thread.Sleep(30000);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                listViewLog.Items.Clear();
                var Entries = MyWcfConnection.GetEventLogEntry(Convert.ToInt32(numericUpDown1.Value));
                if (Entries == null) return;
         
                foreach (var ent in Entries)
                {
                    var lvi = new ListViewItem("[" + ent.TimeGenerated.ToShortDateString() + " " + ent.TimeGenerated.ToShortTimeString() + "]:" + ent.Message);
                    lvi.ToolTipText = lvi.Text;
                    switch (ent.Type)
                    {
                        case TypeEntries.error:
                            lvi.ImageIndex = 3;
                            break;
                        case TypeEntries.message:
                            lvi.ImageIndex = 2;
                            break;
                        case TypeEntries.warning:
                            lvi.ImageIndex = 4;
                            break;
                    }
                    lvi.ToolTipText = lvi.Text;
                    listViewLog.Items.Add(lvi);
             
                }
                Entries = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        

        private void button4_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new SettingForm {Owner = this};
                form.ShowDialog();
                form.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                var br = MyWcfConnection.StopProccess();
                if (!br.Result)
                {
                    MessageBox.Show(br.Exception);
                    return;
                }

                RefreshStatusOperation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                var br = MyWcfConnection.StartProccess(radioButtonMainPriem.Checked, radioButtonAuto.Checked,
                    dateTimePicker1.Value);
                if (!br.Result)
                {
                    MessageBox.Show(br.Exception);
                    return;
                }

                RefreshStatusOperation();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            RefreshStatusOperation();
        }
        
        void SetControlForm(List<string> card)
        {
        
            button9.Enabled = groupBoxStatus.Enabled = groupBoxMonitor.Enabled = card.Contains("FilesInviterStatus") && card.Contains("ArchiveInviterStatus") && card.Contains("FLKInviterStatus") &&
                card.Contains("GetTypePriem") && card.Contains("GetOtchetDate");


            button7.Enabled = card.Contains("StartProccess");
            button6.Enabled = card.Contains("StopProccess");
            button3.Enabled = card.Contains("GetEventLogEntry");
            button1.Enabled = card.Contains("GetFileManagerList");
            мониторингРеестровToolStripMenuItem.Enabled = card.Contains("GetNotReestr") || card.Contains("GetSumReestr") || card.Contains("GetSumReestrDetail");
            своднаяToolStripMenuItem.Enabled = card.Contains("GetSVOD_SMO_TEMP100") || card.Contains("GetSVOD_SMO_TEMP1")
                ||
                card.Contains("GetSVOD_DISP_TEMP100") || card.Contains("GetSVOD_DISP_TEMP1")
                ||
                card.Contains("GetSVOD_VMP_TEMP100") || card.Contains("GetSVOD_VMP_TEMP1")
                ||
                card.Contains("GetSVOD_SMP_TEMP100") || card.Contains("GetSVOD_SMP_TEMP1");


            управлениеПользователямиToolStripMenuItem.Enabled = card.Contains("Roles_GetUsers_Roles") && card.Contains("Roles_GetRoles") && card.Contains("Roles_GetUsers");
            управлениеРолямиToolStripMenuItem.Enabled = card.Contains("Roles_GetRoles") && card.Contains("Roles_GetRolesClaims") && card.Contains("Roles_GetMethod");


            управлениеМетодамиToolStripMenuItem.Enabled = card.Contains("Roles_GetMethod");

            завершениеПриемаToolStripMenuItem.Enabled = card.Contains("GetListTransfer") && card.Contains("RunProcListTransfer");


            завершениеПриемаToolStripMenuItem.Enabled = (card.Contains("GetListTransfer") && card.Contains("RunProcListTransfer")) ||
                (
                card.Contains("GetID_SPOSOB") && card.Contains("GetVIDMP") &&
                                card.Contains("GetMUR_FIN") && card.Contains("GetMUR_FIN_SMP") &&
                                card.Contains("Getf003") && card.Contains("Getf002") &&
                                card.Contains("GetV_XML_H_FAKTURA")
                );
        }

        private void редакторXMLСхемToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ClientService.SchemaEditor.XMLschemaEditor();
            form.ShowDialog();
        }


        private void мониторингРеестровToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyWcfConnection != null)
            {
                var form = new MonitorReestr();
                form.Owner = this;
                form.Show();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                notifyIcon1.Visible = true;
            }
        }
        bool user_closed = false;
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!user_closed)
            {
                this.WindowState = FormWindowState.Minimized;
                e.Cancel = true;
            }

        }

        private void закрытьПрограммуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            user_closed = true;
            this.Close();
        }

        private void своднаяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyWcfConnection != null)
            {
                var From = new SvodSMO {Owner = this};
                From.Show();
            }

        }


        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            user_closed = true;
            this.Close();
        }


        

        private void сравнитьФайлыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new CompareFileXML();
            f.ShowDialog();
        }

      

        private void управлениеРолямиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new Roles.RoleEdit(MyWcfConnection) {Owner = this};
            f.ShowDialog();
        }

        private void управлениеМетодамиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new Roles.MethodEdit(MyWcfConnection) {Owner = this};
            f.ShowDialog();
        }



        private void редакторXMLСхемToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var form = new ClientService.SchemaEditor.XMLschemaEditor();
            form.ShowDialog();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            var f = new FolderDialog(MyWcfConnection.GetSettingsFolder().IncomingDir, false, true);
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MyWcfConnection.AddListFile(f.FileNames);
            }

        }

        private void завершениеПриемаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var pr = new PrepareReestr();
            pr.ShowDialog();
        }

        private void управлениеПользователямиToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            var f = new Roles.USER_EDIT {Owner = this};
            f.ShowDialog();
         
        }

        private void отключитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ((ICommunicationObject) wcf)?.Abort();
        }

        private void подключитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var f = new LoginForm();
            if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MyWcfConnection = f.MyWcfConnection;
                callback = f.callback;

                var form = new Launcher.Launcher();
                form.ShowDialog();
                if (form.RESTART)
                {
                    user_closed = true;
                    this.Close();
                    return;
                }
                ((ICommunicationObject)wcf).Faulted += LoginForm_Faulted;
                ((ICommunicationObject)wcf).Closed += LoginForm_Faulted;
            }

            SetControlForm(LoginForm.SecureCard);
            RefreshStatusOperation();
        }

        private void приемСанкцииОтСМОNEWToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var si = new SANK_INVITER2();
            si.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                var str = (radioButtonAuto.Checked ? "включить" : "отключить");
                if (MessageBox.Show($@"Вы уверены что хотите {str} автоматический прием?", "",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    wcf.SetAutoPriem(radioButtonAuto.Checked);
                    RefreshStatusOperation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void тЕСТToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var t = new SANK_SMO.TEST();
            t.Show();
        }
        OpenFileDialog ofd = new OpenFileDialog();
        
        private void button10_Click(object sender, EventArgs e)
        {
            Parce();
        }


        private void Parce()
        {
            var str = "LS750T75_20031";
            var t =  ParseFileName.Parse(str);
        }
    }
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public class MyServiceCallback : IWcfInterfaceCallback
    {
        public void NewNotifi()
        {
           
        }

        public void NewPackState(string CODE_MO)
        {
            
        }
        public delegate void newFileManager();
        public event newFileManager OnNewFileManager = null;

        public void NewFileManager()
        {
            OnNewFileManager?.Invoke();
        }

        public void PING()
        {
            
        }
    }
}

using ServiceLoaderMedpomData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientServiceWPF.MEK_RESULT;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public IWcfInterface wcf => LoginForm.wcf;
        public static MyServiceCallback callback { set; get; }
        private CollectionViewSource CollectionViewSourceLOG;
        private CollectionViewSource CollectionViewSourceStatusOP;
        
        private bool closed_onSTART;

        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {
            var f = new LoginForm();
            if (f.ShowDialog() == true)
            {
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

            CollectionViewSourceLOG = this.FindResource("CollectionViewSourceLOG") as CollectionViewSource;
            CollectionViewSourceStatusOP = this.FindResource("CollectionViewSourceStatusOP") as CollectionViewSource;
            CreateNotifiIcon();
        }

        private void CreateNotifiIcon()
        {
            ni.Text = @"Управление службой MedpomService";
            ni.Visible = false;
            ni.Icon = Properties.Resources.doctor;

            ni.ContextMenu = new System.Windows.Forms.ContextMenu(new[]
            {
                new System.Windows.Forms.MenuItem("Развернуть", delegate
                {
                    Show();
                    ni.Visible = false;
                    WindowState = WindowState.Normal;
                }) {DefaultItem = true},
                new System.Windows.Forms.MenuItem("-"),
                new System.Windows.Forms.MenuItem("Закрыть", delegate { CloseAPP(); })
            });
            ni.DoubleClick +=
                delegate
                {
                    Show();
                    ni.Visible = false;
                    WindowState = WindowState.Normal;
                };

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new Setting(Active);
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

   


        public List<EntriesMy> Entries { get; set; } = new List<EntriesMy>();
        private void buttonRefreshLog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Entries.Clear();
                Entries.AddRange(wcf.GetEventLogEntry(Convert.ToInt32(textBoxCountLog.Text)));
                CollectionViewSourceLOG.View.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

  

        private bool user_closed;
        private void MenuItemConnect_Click(object sender, RoutedEventArgs e)
        {
            var f = new LoginForm();
            if (f.ShowDialog() == true)
            {
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

        void LoginForm_Faulted(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                var f = new LoginForm { DIALOG_MESSAGE = "Связь с сервером потеряна!" };

                if (f.ShowDialog() == true)
                {
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
            });
        }

        void SetControlForm(List<string> card)
        {

            buttonRefreshStatus.IsEnabled = GroupBoxStatus.IsEnabled = GroupBoxMonitorWork.IsEnabled = card.Contains("FilesInviterStatus") && card.Contains("ArchiveInviterStatus") && card.Contains("FLKInviterStatus") && card.Contains("GetTypePriem") && card.Contains("GetOtchetDate");
            ButtonStartProcess.IsEnabled = card.Contains("StartProccess");
            ButtonStopProcess.IsEnabled = card.Contains("StopProccess");
            buttonRefreshLog.IsEnabled = card.Contains("GetEventLogEntry");
            ButtonWork.IsEnabled = card.Contains("GetFileManagerList");
            MenuItemMonitor.IsEnabled = card.Contains("GetNotReestr") || card.Contains("GetSumReestr") || card.Contains("GetSumReestrDetail");
            MenuItemEditUser.IsEnabled = card.Contains("Roles_GetUsers_Roles") && card.Contains("Roles_GetRoles") && card.Contains("Roles_GetUsers");
            
        }

       

  

        public List<StatusOper> ListOP { get; set; } = new List<StatusOper>();
        private Thread thERR;
        private bool Active;
        private void RefreshStatusOperation()
        {
            try
            {
                var StatusOperFileInvite = new StatusOper {NameOP = "Прием файлов"};
                var StatusOperArcInvite = new StatusOper {NameOP = "Прием архивов"};
                var StatusOperFLKInvite = new StatusOper {NameOP = "Обработка ФЛК"};
                var StatusOperAutoInvite = new StatusOper {NameOP = "Захват файлов"};
                ListOP.Clear();
                ListOP.Add(StatusOperFileInvite);
                ListOP.Add(StatusOperArcInvite);
                ListOP.Add(StatusOperFLKInvite);
                ListOP.Add(StatusOperAutoInvite);

                if (buttonRefreshStatus.IsEnabled)
                {
                    StatusOperFileInvite.Status = wcf.FilesInviterStatus();
                    StatusOperArcInvite.Status = wcf.ArchiveInviterStatus();
                    StatusOperFLKInvite.Status = wcf.FLKInviterStatus();
                    StatusOperAutoInvite.Status = wcf.GetAutoFileAdd();
                    buttonFileAdd.IsEnabled = !StatusOperAutoInvite.Status;

                    if (ListOP.Count(x => x.Status) == 0)
                    {
                        RadioButtonMainTypePriem.IsEnabled = RadioButtonPREDTypePriem.IsEnabled = DatePickerPERIOD.IsEnabled = ButtonStartProcess.IsEnabled = true;
                        buttonFileAdd.IsEnabled = false;
                        ButtonStopProcess.IsEnabled = false;
                        Active = false;
                    }
                    else
                    {
                        RadioButtonMainTypePriem.IsEnabled = RadioButtonPREDTypePriem.IsEnabled = DatePickerPERIOD.IsEnabled = ButtonStartProcess.IsEnabled = false;
                        ButtonStopProcess.IsEnabled = true;
                        Active = true;
                    }


                    if (wcf.GetTypePriem())
                    {
                        RadioButtonMainTypePriem.IsChecked = true;
                    }
                    else
                    {
                        RadioButtonPREDTypePriem.IsChecked = true;
                    }

                    if (wcf.GetAutoPriem())
                    {
                        RadioButtonFileAuto.IsChecked = true;
                    }
                    else
                    {
                        RadioButtonFileHand.IsChecked = true;
                    }

                    DatePickerPERIOD.SelectedDate = wcf.GetOtchetDate();
                }

                if (buttonRefreshLog.IsEnabled)
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
            finally
            {
                CollectionViewSourceStatusOP?.View.Refresh();
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
                            this.Dispatcher.Invoke(() =>
                            {
                                buttonRefreshLog_Click(buttonRefreshLog, new RoutedEventArgs());
                            });

                        }
                    }
                }
                catch (Exception ex)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        System.Windows.Forms.MessageBox.Show($@"Ошибка в потоке проверки ошибок: {ex.Message}");
                    });
                }
                Thread.Sleep(600000);
            }
        }

        private void MenuItemActMEK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new ACT_MEK();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemDisconnect_Click(object sender, RoutedEventArgs e)
        {
            ((ICommunicationObject)wcf)?.Abort();
        }

        private void CloseAPP(bool Shutdown = true)
        {
            user_closed = true;
            ni.Visible = false;
            ni.Dispose();
            if(Shutdown)
                Application.Current.Shutdown();
        }

        private void MenuItemCloseApp_Click(object sender, RoutedEventArgs e)
        {
            CloseAPP();
        }

        private void this_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!user_closed)
            {
                this.WindowState = WindowState.Minimized;
                e.Cancel = true;
            }
            else
            {
                CloseAPP(false);
            }
        }

        private void this_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                ni.Visible = true;
            }
        }

        private void buttonRefreshStatus_Click(object sender, RoutedEventArgs e)
        {
            RefreshStatusOperation();
        }

        private void ButtonStartProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(!DatePickerPERIOD.SelectedDate.HasValue)
                    throw new Exception("Не указан период");
                var br = wcf.StartProccess(RadioButtonMainTypePriem.IsChecked==true, RadioButtonFileAuto.IsChecked == true, DatePickerPERIOD.SelectedDate.Value);
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

        private void ButtonStopProcess_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var br = wcf.StopProccess();
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

        private void buttonFileChangeMode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var str = (RadioButtonFileAuto.IsChecked==true ? "включить" : "отключить");
                if (MessageBox.Show($@"Вы уверены что хотите {str} автоматический прием?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    wcf.SetAutoPriem(RadioButtonFileAuto.IsChecked == true);
                    RefreshStatusOperation();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonFileAdd_Click(object sender, RoutedEventArgs e)
        {
            var f = new RemoteFolderDialog(wcf.GetSettingsFolder().IncomingDir, false, true);
            if (f.ShowDialog() == true)
            {
                wcf.AddListFile(f.FileNames);
            }
        }

        private void ButtonWork_Click(object sender, RoutedEventArgs e)
        {
            var form = new FilesManagerView(Active);
            form.Owner = this;
            form.Show();
        }

        private void MenuItemSMOInvite_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
               var win = new SANK_INVITER.SANK_INVITER();
               win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemMonitor_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
               var win = new MonitorReestr();
               win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemEditUser_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new USER_EDIT.USER_ROLE();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemXSDCreator_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new SchemaEditor.XMLshema();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemPrintXLSX_OnClick(object sender, RoutedEventArgs e)
        {
             PRINT_FILE_XLSX win = new PRINT_FILE_XLSX();
             win.Show();
        }

        private void MenuExportFileMEK_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new ExportFile();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuVolumeControl_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new VOLUM_CONTROL();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

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


    public class StatusOper
    {
        public string NameOP { get; set; }
        public bool Status { get; set; }

        public string StatusText => Status ? "Активно" : "Не активно";
    }


    public static partial class Ext
    {
        public static bool ContainsAND(this List<string> val, params string[] par)
        {
            return par.All(val.Contains);
        }
     
    }
}

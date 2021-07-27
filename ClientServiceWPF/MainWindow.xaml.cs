using ServiceLoaderMedpomData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
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
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using ClientServiceWPF.MEK_RESULT.FileCreator;
using ClientServiceWPF.ORDERS.DISP;
using ClientServiceWPF.ORDERS.FSB;
using ClientServiceWPF.ORDERS.ORD104;
using ClientServiceWPF.ORDERS.ORD15;
using ClientServiceWPF.ORDERS.ORD23;
using ClientServiceWPF.ORDERS.ORD260;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowVM VM { get; set; } = new MainWindowVM(Dispatcher.CurrentDispatcher);
        public IWcfInterface wcf => LoginForm.wcf;

        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {
            ConnectWCF();
            InitializeComponent();
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

        private void ConnectWCF(string DIALOG_MESSAGE = null)
        {
            var f = new LoginForm();
            if (wcf != null)
            {
                var t = wcf;
                LoginForm.wcf = null;
                ((ICommunicationObject) t)?.Abort();
                return;
            }

            if (!string.IsNullOrEmpty(DIALOG_MESSAGE))
                f.DIALOG_MESSAGE = DIALOG_MESSAGE;
            if (f.ShowDialog() == true)
            {
                var form = new Launcher.Launcher();
                form.ShowDialog();
                if (form.RESTART)
                {
                    CloseAPP();
                    return;
                }

                if (wcf is ICommunicationObject co)
                {
                    co.Faulted += LoginForm_Faulted;
                    co.Closed += LoginForm_Faulted;
                }
              
            }
            VM.SetWCF(wcf).SetCard(LoginForm.SecureCard);

        }
        private void CloseAPP(bool Shutdown = true)
        {
            user_closed = true;
            ni.Visible = false;
            ni.Dispose();
            VM?.Dispose();
            if (Shutdown)
                Application.Current.Shutdown();
        }

        private bool user_closed;
        private void MenuItemConnect_Click(object sender, RoutedEventArgs e)
        {
            ConnectWCF();
        }

        void LoginForm_Faulted(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                ConnectWCF("Связь с сервером потеряна!");
            });
        }

     

        private void MenuItemDisconnect_Click(object sender, RoutedEventArgs e)
        {
            ((ICommunicationObject)wcf)?.Abort();
            LoginForm.wcf = null;
            VM.SetWCF(wcf).SetCard(new List<string>());
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

       
    }


    public class MainWindowVM : INotifyPropertyChanged, IDisposable
    {
        private Dispatcher dispatcher;
        public MainWindowVM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            ListOP.AddRange(StatusOperFileInvite, StatusOperArcInvite, StatusOperFLKInvite, StatusOperAutoInvite);
        }
        #region Right
        private IWcfInterface wcf { get; set; }
        public MainWindowVM SetWCF(IWcfInterface wcf)
        {
            this.wcf = wcf;
            if (wcf != null)
                StartStatusRefreshTask();
            return this;
        }
        private List<string> _card = new List<string>();
        public List<string> card
        {
            get => _card;
            set
            {
                _card = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasConnect));
            }
        }
        public MainWindowVM SetCard(List<string> card)
        {
            this.wcf = wcf;
            this.card = card;
            return this;
        }
    
        public bool HasConnect => card.Count != 0;

        #endregion
        #region Logs
        public ObservableCollection<EntriesMy> Entries { get; set; } = new ObservableCollection<EntriesMy>();
        private int _CountLog = 50;
        public int CountLog
        {
            get => _CountLog;
            set
            {
                _CountLog = value;
                RaisePropertyChanged();
            }
        }
        public ICommand GetLogCommand => new Command(obj =>
        {
            try
            {
                Entries.Clear();
                Entries.AddRange(wcf.GetEventLogEntry(CountLog));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, obj => card.Contains(nameof(IWcfInterface.GetEventLogEntry)));
        #endregion
        #region StatusOperation
        private StatusPriem _Status;
        public StatusPriem Status
        {
            get => _Status;
            set
            {
                _Status = value;
                StatusOperFileInvite.Status = _Status.FilesInviterStatus;
                StatusOperArcInvite.Status = _Status.THArchiveInviter;
                StatusOperFLKInvite.Status = _Status.FLKInviterStatus;
                StatusOperAutoInvite.Status = _Status.ActiveAutoPriem;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Active));
            }
        }

        public bool Active => StatusOperFileInvite.Status || StatusOperArcInvite.Status || StatusOperFLKInvite.Status || StatusOperAutoInvite.Status;
        public StatusOper StatusOperFileInvite { get; set; }= new StatusOper { NameOP = "Прием файлов" };
        public StatusOper StatusOperArcInvite { get; set; } = new StatusOper { NameOP = "Прием архивов" };
        public StatusOper StatusOperFLKInvite { get; set; } = new StatusOper { NameOP = "Обработка ФЛК" };
        public StatusOper StatusOperAutoInvite { get; set; } = new StatusOper { NameOP = "Захват файлов" };
        public ObservableCollection<StatusOper> ListOP { get; set; } = new ObservableCollection<StatusOper>();
        public ICommand RefreshStatusOperationCommand => new Command(obj =>
        {
            try
            {
                Status = wcf.GetStatusInvite();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => card.Contains(nameof(IWcfInterface.GetStatusInvite)));
        public ICommand ChangeFileInviteTypeCommand => new Command(o =>
        {
            try
            {
                if (MessageBox.Show($@"Вы уверены что хотите {(Status.AutoPriem ? "включить" : "отключить")} автоматический прием?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    wcf.SetAutoPriem(Status.AutoPriem);
                    RefreshStatusOperationCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => card.Contains(nameof(IWcfInterface.SetAutoPriem)));
        public ICommand AddFileCommand => new Command(o =>
        {
            try
            {
                var f = new RemoteFolderDialog(wcf.GetSettingsFolder().IncomingDir, false, true);
                if (f.ShowDialog() == true)
                {
                    wcf.AddListFile(f.FileNames);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => card.ContainsAND(nameof(IWcfInterface.StartProcess), nameof(IWcfInterface.AddListFile)));
        public ICommand StartProcessCommand => new Command(o =>
        {
            try
            {
                var br = wcf.StartProcess(Status.TypePriem, Status.AutoPriem, Status.OtchetDate);
                if (!br.Result)
                {
                    MessageBox.Show(br.Exception);
                    return;
                }
                RefreshStatusOperationCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => card.Contains(nameof(IWcfInterface.StartProcess)));
        public ICommand StopProcessCommand => new Command(o =>
        {
            try
            {
                var br = wcf.StopProcess();
                if (!br.Result)
                {
                    MessageBox.Show(br.Exception);
                    return;
                }
                RefreshStatusOperationCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => card.Contains(nameof(IWcfInterface.StopProcess)));
        #endregion
        #region StatusRefreshTask

        private void StartStatusRefreshTask()
        {
            StopStatusRefreshTask();
            TaskCheckErrCTS = new CancellationTokenSource();
            TaskCheckErr = Task.Run(() =>
            {
                StatusRefreshTask(TaskCheckErrCTS.Token);
            });
        }

        private void StopStatusRefreshTask()
        {
            TaskCheckErrCTS?.Cancel();
        }
        private Task TaskCheckErr;
        private CancellationTokenSource TaskCheckErrCTS;
        void StatusRefreshTask(CancellationToken cancel)
        {
            while (!cancel.IsCancellationRequested)
            {
                try
                {
                    if (wcf != null)
                    {
                        if (((ICommunicationObject) wcf).State == CommunicationState.Opened)
                        {
                            dispatcher?.Invoke(() =>
                            {
                                RefreshStatusOperationCommand.Execute(null);
                                GetLogCommand.Execute(null);
                            });
                        }
                    }

                    var del = Task.Delay(600000, cancel);
                    del.Wait(cancel);
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception ex)
                {
                    dispatcher?.Invoke(() =>
                    {
                        System.Windows.Forms.MessageBox.Show($@"Ошибка в потоке проверки ошибок: {ex.Message}");
                    });
                }
            }
        }


        #endregion
        #region Navigate
        public ICommand SettingNavigateCommand => new Command(o =>
        {
            try
            {
                var win = new Setting(Active || wcf==null);
                win.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand FilesManagerViewNavigateCommand => new Command(o =>
        {
            try
            {
                var form = new FilesManagerView(Active);
                form.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        },  o => HasConnect);

        public ICommand MonitorReestrNavigateCommand => new Command(o =>
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
        }, o => HasConnect);


        public ICommand SANK_INVITERNavigateCommand => new Command(o =>
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
        });

        public ICommand ACT_MEKNavigateCommand => new Command(o =>
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
        });

        public ICommand ExportFileNavigateCommand => new Command(o =>
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
        });
        public ICommand VOLUM_CONTROLNavigateCommand => new Command(o =>
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
        });

        public ICommand XMLshemaNavigateCommand => new Command(o =>
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
        });

        public ICommand PRINT_FILE_XLSXNavigateCommand => new Command(o =>
        {
            try
            {
                var win = new PRINT_FILE_XLSX();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand ORD15NavigateCommand => new Command(o =>
        {
            try
            {
                var win = new ORD15();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ORD104NavigateCommand => new Command(o =>
        {
            try
            {
                var win = new ORD104();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ORD260NavigateCommand => new Command(o =>
        {
            try
            {
                var win = new ORD260();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand FSBNavigateCommand => new Command(o =>
        {
            try
            {
                var win = new FSB();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand DISPNavigateCommand => new Command(o =>
        {
            try
            {
                var win = new DISPWin();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ORD23NavigateCommand => new Command(o =>
        {
            try
            {
                var win = new ORD23();
                win.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand USER_EDITNavigateCommand => new Command(o =>
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
        }, o => HasConnect);

        
        #endregion
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
        public void Dispose()
        {
            StopStatusRefreshTask();
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

        public event newFileManager OnNewFileManager;

        public void NewFileManager()
        {
            OnNewFileManager?.Invoke();
        }

        public void PING()
        {

        }
    }
    public class StatusOper:INotifyPropertyChanged
    {
        public string NameOP { get; set; }
        private bool _Status;

        public bool Status
        {
            get => _Status;
            set
            {
                _Status = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(StatusText));
            }
        }

        public string StatusText => Status ? "Активно" : "Не активно";
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public static partial class Ext
    {
        public static bool ContainsAND(this List<string> val, params string[] par)
        {
            return par.All(val.Contains);
        }
        public static bool ContainsOR(this List<string> val, params string[] par)
        {
            return par.Any(val.Contains);
        }


        public static void AddRange<T>(this ObservableCollection<T> source, params T[] values)
        {
            foreach (var item in values)
            {
                source.Add(item);
            }
        }

        public static void AddRange<T>(this ObservableCollection<T> source,IEnumerable<T> values)
        {
            foreach (var item in values)
            {
                source.Add(item);
            }
        }



    }


 
}

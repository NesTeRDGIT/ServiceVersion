using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using FileTransfer.Annotations;
using FileTransfer.Class;
using Path = System.IO.Path;

namespace FileTransfer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        public MainWindowVM VM { get; set; }= new MainWindowVM(new FileRepository(Path.Combine(LocalFolder, "Setting.xml")));
        public MainWindow()
        {
            InitializeComponent();
            VM.LoadCommand.Execute(null);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public List<TransferVM> ListViewSelected=> listView.SelectedItems.Cast<TransferVM>().ToList();

        private void ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           OnPropertyChanged(nameof(ListViewSelected));
        }
    }

    public class LogItem
    {
        public string Text { get; set; }
        public bool IsError { get; set; }
    }

    public class MainWindowVM : INotifyPropertyChanged
    {
        private IRepository repository;
        public ObservableCollection<TransferVM> TransferList { get; set; } = new ObservableCollection<TransferVM>();
        public ObservableCollection<LogItem> LogList { get; set; } = new ObservableCollection<LogItem>();
        private Logger log;
        public MainWindowVM(IRepository repository)
        {
            this.repository = repository;
            log = new Logger();
            log.EventAdd += Log_EventAdd;
        }

        private void Log_EventAdd(string text, bool isError)
        {
            LogList.Add(new LogItem() {Text = text, IsError =  isError});
        }

        public ICommand LoadCommand=>new Command(o =>
        {
            try
            {
                updateTransfer(repository.Load());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
       
        public ICommand RefreshStatusCommand => new Command(o =>
        {
            try
            {
                foreach (var item in TransferList)
                {
                    item.RefreshStatus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand SaveCommand => new Command(o =>
        {
            try
            {
                repository.Save(TransferList.Select(x => x.TransferRule));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand NewTransferVMCommand => new Command(o =>
        {
            try
            {
                var win = new Setting(new TransferRule());
                if (win.ShowDialog() == true)
                {
                    var rule = win.VM.Rule;
                    var item = new TransferVM(rule, log);
                    TransferList.Add(item);
                    SaveCommand.Execute(null);
                    if (rule.onStart)
                        item.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand EditTransferVMCommand => new Command(o =>
        {
            try
            {
                var items = (IEnumerable<TransferVM>) o;
                var item = items.FirstOrDefault();
                if (item != null)
                {
                    var win = new Setting(item.TransferRule);
                    if (win.ShowDialog() == true)
                    {
                        item.ReplaceTransferRule(win.VM.Rule);
                        SaveCommand.Execute(null);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand StartTransferVMCommand => new Command(o =>
        {
            try
            {
                var items = (IEnumerable<TransferVM>)o;
                foreach (var item in items)
                {
                    item.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand StopTransferVMCommand => new Command(o =>
        {
            try
            {
                var items = (IEnumerable<TransferVM>)o;
                foreach (var item in items)
                {
                    item.Stop();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand RemoveTransferVMCommand => new Command(o =>
        {
            try
            {
                var items = (IEnumerable<TransferVM>)o;
                foreach (var item in items)
                {
                    item.Stop();
                    SaveCommand.Execute(null);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });


        void updateTransfer(IEnumerable<TransferRule> rules)
        {
            foreach (var item in TransferList)
            {
                item.Stop();
            }
            TransferList.Clear();
            foreach (var item in rules)
            {
                TransferList.Add(new TransferVM(item, log));
            }
        }

      
     
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    #region  Logger
    public class Logger : ILogger
    {

        public delegate void AddDelegate(string text, bool isError);

        public event AddDelegate EventAdd;
        public Logger()
        {
           
        }
        public void Add(string text, bool isError)
        {
            EventAdd?.Invoke(text, isError);
        }
    }

    #endregion


    public class TransferVM : INotifyPropertyChanged
    {
        public bool isActive => task?.Status  == TaskStatus.Running || task?.Status == TaskStatus.WaitingForActivation || task?.Status == TaskStatus.WaitingForChildrenToComplete;
        private TransferRule _TransferRule;
        public TransferRule TransferRule
        {
            get { return _TransferRule;}
            private set
            {
                _TransferRule = value;
                OnPropertyChanged();
            } }

        private ILogger logger;
        public TransferVM(TransferRule tr, ILogger logger)
        {
            TransferRule = tr;
            this.logger = logger;
        }


        public void ReplaceTransferRule(TransferRule tr)
        {
            this.Stop();
            TransferRule = tr;
        }



        CancellationTokenSource cts;
        private Task task;
        public void Start()
        {
            logger?.Add($"Запуск задачи переноса из {TransferRule.PathSource} в {TransferRule.PathDestination} от имени пользователя {TransferRule.User.FULL_USER}", false);
            cts = new CancellationTokenSource();
            task = Task.Run(async () =>
            {
                try
                {
                    while (!cts.IsCancellationRequested)
                    {
                        FileMover.MoveFiles(TransferRule, logger);
                        await Task.Delay(TransferRule.TimeOut * 1000);
                    }
                    logger?.Add($"Остановка задачи переноса из {TransferRule.PathSource} в {TransferRule.PathDestination} от имени пользователя {TransferRule.User.FULL_USER}", false);
                }
                catch (Exception e)
                {
                    logger?.Add($"Ошибка: {e.Message}{Environment.NewLine}{e.StackTrace}", true);
                }
                finally
                {
                    RefreshStatus();
                }

            }, cts.Token);
            RefreshStatus();
        }

        public void Stop()
        {
            cts?.Cancel();
            RefreshStatus();
        }

        public void RefreshStatus()
        {
            OnPropertyChanged(nameof(isActive));
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


    
}

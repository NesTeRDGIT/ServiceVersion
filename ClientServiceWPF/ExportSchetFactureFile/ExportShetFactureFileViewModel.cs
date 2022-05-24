using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using ServiceLoaderMedpomData.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;
using LogType = ClientServiceWPF.Class.LogType;
using System.Collections.ObjectModel;

namespace ClientServiceWPF.ExportSchetFactureFile
{
    internal class ExportShetFactureFileViewModel : INotifyPropertyChanged
    {
        private readonly Dispatcher dispatcher;

        public ExportShetFactureFileViewModel()
        {
            CurrentDate = DateTime.Now;
        }

        public ExportShetFactureFileViewModel(Dispatcher dispatcher)
        {
            CurrentDate = DateTime.Now;
            this.dispatcher = dispatcher;
        }

        private CancellationTokenSource cts;
        private FolderBrowserDialog fbd = new FolderBrowserDialog();
        private string connStr = AppConfig.Property.ConnectionString;
        public ObservableCollection<LogItem> Logs { get; set; } = new ObservableCollection<LogItem>();
        public ProgressItem Progress1 { get; } = new ProgressItem();
        public ProgressItem Progress2 { get; } = new ProgressItem();

        private DateTime _currentDate;
        public DateTime CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsOperationRun;
        public bool IsOperationRun
        {
            get => _IsOperationRun;
            set
            {
                _IsOperationRun = value;
                RaisePropertyChanged();
                //CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand ExportSchetFactureFileComand => new Command(async o =>
        {
            try
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    IsOperationRun = true;
                    //Logs.Clear();
                    Progress1.IsIndeterminate = true;
                    cts = new CancellationTokenSource();
                    //var files = await GetFileAsync(fbd.SelectedPath, PARAM.DATE_B, PARAM.DATE_E, cts.Token);

                    if (MessageBox.Show(@"Завершено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        // ShowSelectedInExplorer.FilesOrFolders(files);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.FullMessage());
            }
            finally
            {
                Progress1.Clear("");
                Progress2.Clear("");
                IsOperationRun = false;
            }
        }, o => !IsOperationRun);

        public ICommand BreakCommand => new Command(o =>
        {
            try
            {
                cts?.Cancel();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }, o => IsOperationRun);

        private void AddLogs(LogType type, params string[] Message)
        {
            dispatcher.Invoke(() =>
            {
                foreach (var mes in Message)
                {
                    Logs.Add(new LogItem(type, mes));
                }
            }
            );
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

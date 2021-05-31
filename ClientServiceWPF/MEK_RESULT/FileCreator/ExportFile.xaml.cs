using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ClientServiceWPF.SANK_INVITER;
using ExcelManager;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using ServiceLoaderMedpomData.EntityMP_V31;
using LogType = ClientServiceWPF.Class.LogType;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Threading.Timer;

namespace ClientServiceWPF.MEK_RESULT.FileCreator
{
    /// <summary>
    /// Логика взаимодействия для ExportFile.xaml
    /// </summary>
    public partial class ExportFile : Window
    {
        public ExportFileVM VM { get; set; }
     
        public ExportFile()
        {
            var exportFileRepository = new ExportFileRepository(AppConfig.Property.ConnectionString);
            var fileCreator = new FileCreator(exportFileRepository);
            VM = new ExportFileVM(fileCreator, exportFileRepository, Dispatcher.CurrentDispatcher);
            InitializeComponent();
        }

    
        private void ExportFile_OnLoaded(object sender, RoutedEventArgs e)
        {
            VM.PARAM.PERIOD = DateTime.Now.AddMonths(-1);
            VM.PARAM.IsSMO = true;
            VM.PARAM.CountTask = 3;
            VM.PARAM.IsTEMP1 = true;
        }
    }

    public class V_EXPORT_H_ZGLVRowVM : INotifyPropertyChanged
    {
        public V_EXPORT_H_ZGLVRow Item { get; }

        public V_EXPORT_H_ZGLVRowVM(V_EXPORT_H_ZGLVRow Item)
        {
            this.Item = Item;
            Logs.CollectionChanged += Logs_CollectionChanged;
        }

        private void Logs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsTypeLog));
        }

        private bool _InWork;

        public bool InWork
        {
            get => _InWork;
            set
            {
                _InWork = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsSelect = true;

        public bool IsSelect
        {
            get => _IsSelect;
            set
            {
                _IsSelect = value;
                RaisePropertyChanged();
            }
        }

        private decimal? _SUM_XLS;

        public decimal? SUM_XLS
        {
            get => _SUM_XLS;
            set
            {
                _SUM_XLS = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<LogItem> Logs { get; } = new ObservableCollection<LogItem>();

        public void AddLogs(LogType t, params string[] m)
        {
            Logs.AddRange(m.Select(x => new LogItem(t, x)).ToArray());
        }

        private decimal? _SUM { get; set; }

        public decimal? SUM
        {
            get => _SUM;
            set
            {
                _SUM = value;
                RaisePropertyChanged();
            }
        }

        public LogType? IsTypeLog
        {
            get
            {
                if (Logs.Count(x => x.Type == LogType.Error) != 0)
                    return LogType.Error;
                if (Logs.Count(x => x.Type == LogType.Warning) != 0)
                    return LogType.Warning;
                if (Logs.Count(x => x.Type == LogType.Info) != 0)
                    return LogType.Info;
                return null;
            }
        }

        private bool _Finish;

        public bool Finish
        {
            get => _Finish;
            set
            {
                _Finish = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<string> PathArc { get; } = new ObservableCollection<string>();

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }


   

  

    public class ExportFileVM : INotifyPropertyChanged
    {
        private IFileCreator fileCreator;
        private Dispatcher dispatcher;
        private IExportFileRepository exportFileRepository;

        public ExportFileVM(IFileCreator fileCreator, IExportFileRepository exportFileRepository, Dispatcher dispatcher)
        {
            this.fileCreator = fileCreator;
            this.exportFileRepository = exportFileRepository;
            this.dispatcher = dispatcher;
        }
        public ObservableCollection<V_EXPORT_H_ZGLVRowVM> ZGLV_LIST { get; } = new ObservableCollection<V_EXPORT_H_ZGLVRowVM>();

        public ExportFilePARAM PARAM { get; } = new ExportFilePARAM();
        public ProgressItem progress1 { get; } = new ProgressItem();
        public ProgressItem progress2 { get; } = new ProgressItem();
        public ProgressItem progress3 { get; } = new ProgressItem();
        private bool _isProcessing;
        public bool isProcessing
        {
            get=>_isProcessing;
            set
            {
                _isProcessing = value;
                RaisePropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        } 

        public ICommand RefreshListCommand => new Command(async o =>
        {
            try
            {
                isProcessing = true;
                progress1.IsIndeterminate = true;
                var items = await Task.Run(() => exportFileRepository.V_EXPORT_H_ZGLV(PARAM.IsTEMP1, PARAM.MONTH, PARAM.YEAR));
                ZGLV_LIST.Clear();
                ZGLV_LIST.AddRange(items.Select(x => new V_EXPORT_H_ZGLVRowVM(x)).ToArray());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                progress1.IsIndeterminate = false;
                isProcessing = false;
            }
        }, o => !isProcessing);

        public ICommand SelectAllListCommand => new Command(o =>
        {
            try
            {
                if (ZGLV_LIST.Count == 0) return;
                var max = ZGLV_LIST.Max(x => x.IsSelect);
                foreach (var item in ZGLV_LIST)
                {
                    item.IsSelect = !max;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        private FolderBrowserDialog fbd { get; } = new FolderBrowserDialog();

        private CancellationTokenSource cts { get; set; }
        public ICommand BreakCommand => new Command( o =>
        {
            try
            {
                cts?.Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => isProcessing);


        public ICommand SaveCommand => new Command(async o =>
        {
            try
            {
                isProcessing = true;
                foreach (var item in ZGLV_LIST)
                {
                    item.Logs.Clear();
                    item.Finish = false;
                    item.InWork = false;
                    item.SUM = null;
                    item.SUM_XLS = null;
                    item.PathArc.Clear();
                }

                var items = ZGLV_LIST.Where(x => x.IsSelect).ToList();
                if (PARAM.CountTask <= 0 || PARAM.CountTask > 10)
                    throw new Exception("Количество потоков должно быть от 1 до 10");
                if (items.Count == 0)
                    throw new Exception("Не выбрано не одного файла");

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    List<F002> smoList = null;
                    if (PARAM.IsSMO)
                    {
                        progress1.Text = "Запрос справочник F002";
                        smoList = await Task.Run(() => exportFileRepository.GetF002());
                    }

                    cts = new CancellationTokenSource();
                    var tasks = new List<Task> {Task.Run(() => { GetFilesXML(items, fbd.SelectedPath, PARAM.IsTEMP1, PARAM.IsSMO, smoList, cts.Token); })};

                    if (PARAM.IsSMO)
                    {
                        tasks.Add(Task.Run(() => { GetFilesXLS(items,  fbd.SelectedPath, smoList, PARAM.DATE_1_XLS, PARAM.DATE_2_XLS,  cts.Token); }));
                    }
                    await Task.WhenAll(tasks);




                    var Err = CheckResult(items, PARAM.IsSMO);
                    if (CustomMessageBox.Show($"Завершено. Показать файлы?{(Err.Count != 0 ? $"{Environment.NewLine}{string.Join(Environment.NewLine, Err.Select(x => x))}" : "")}", "") == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(fbd.SelectedPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                progress1.SetValues(1,0,""); 
                progress2.SetValues(1, 0, "");
                progress3.SetValues(1, 0, "");
                isProcessing = false;
            }
        }, o => !isProcessing);

        private List<string> CheckResult(List<V_EXPORT_H_ZGLVRowVM> items,bool IsSMO)
        {
            var Err = new List<string>();
            var err_count = items.Count(x => (x.IsTypeLog ?? LogType.Info) != LogType.Info);
            if (err_count != 0)
                Err.Add($@"Выгрузка содержит {err_count} ошибочных файлов");

            if (IsSMO)
            {
                var RESULT = items.GroupBy(x => x.Item.CODE_MO);
                foreach (var res in RESULT)
                {
                    var sumXLS = Math.Round(res.Sum(x => x.SUM_XLS ?? 0), 2);
                    var sumXML = Math.Round(res.Sum(x => x.SUM ?? 0), 2);
                    if (sumXML != sumXLS)
                    {
                        Err.Add($@"Ошибка контроля сумм XML и XLS для МО = {res.Key} сумма XML ={sumXML} сумма XLS = {sumXLS}");
                    }
                }
            }
            return Err;
        }

        private void GetFilesXML(List<V_EXPORT_H_ZGLVRowVM> Items, string Folder, bool IsTemp1, bool isSMO, List<F002> smoList, CancellationToken cancel)
        {
            var tm = new TaskManager(PARAM.CountTask);
            var index = 0;
            var count = Items.Count;
            dispatcher.Invoke(() => { progress1.SetValues(count,index, ""); });
            foreach (var fi in Items)
            {
                try
                {
                    fi.SUM = 0;
                    var tmitem = tm.WaitFreeItem(cancel);

                    cancel.ThrowIfCancellationRequested();
                    tmitem.TSK = CreateFileTask(fi, Folder, IsTemp1, smoList);
                    tmitem.Free = false;
                    tmitem.TSK.Start();
                    dispatcher.Invoke(() => { progress1.SetTextValue(index, $"Выгрузка {index} из {count}"); });
                }
                catch (Exception ex)
                {
                    dispatcher.Invoke(() => { fi.AddLogs(LogType.Error, $"Ошибка {ex.Source}: {ex.FullError()}"); });
                }

                index++;
            }

            cancel.ThrowIfCancellationRequested();
            dispatcher.Invoke(() =>
            {
                progress1.Text = "Ожидание завершения потоков";
                progress1.IsIndeterminate = true;
            });
            tm.WaitIsSTOP(cancel);

            dispatcher.Invoke(() => 
            {

                progress1.IsIndeterminate = false;
                progress1.Text = "";
            });

            if (!isSMO)
            {
                var GLIST = Items.Where(x => x.PathArc.Count != 0).GroupBy(x => new {x.Item.YEAR, x.Item.MONTH, x.Item.CODE_MO});
                var countGR = GLIST.Count();
                var i = 1;
                dispatcher.Invoke(() => { progress1.SetValues(countGR, 0, "Сбор файлов в архив"); });
                foreach (var gr in GLIST)
                {
                    cancel.ThrowIfCancellationRequested();
                    var name_arc = $"Результаты МЭК {gr.Key.CODE_MO} за {gr.Key.MONTH:D2}.{gr.Key.YEAR}.ZIP";
                    var NAME_ARC = Path.Combine(Folder, name_arc);

                    dispatcher.Invoke(() => { progress1.SetTextValue(i, $"Сбор файлов в архив: {name_arc}"); });

                    using (var archive = ZipFile.Open(NAME_ARC, ZipArchiveMode.Create))
                    {
                        foreach (var item in gr)
                        {
                            foreach (var file in item.PathArc)
                            {
                                dispatcher.Invoke(() => { progress2.Text = $"Добавление {file}"; });
                                archive.CreateEntryFromFile(file, Path.GetFileName(file));
                                File.Delete(file);
                            }
                        }
                    }
                    i++;
                }
                dispatcher.Invoke(() => { progress2.Text = ""; });
            }
        }

        private Task CreateFileTask(V_EXPORT_H_ZGLVRowVM item, string Folder, bool IsTemp1, List<F002> smoList)
        {
            return new Task(() =>
            {
                try
                {
                    dispatcher.Invoke(() => { item.InWork = true; });
                    var Result = new List<FileCreatorResult>();
                    var pr = new Progress<ProgressFileCreator>(o =>
                    {
                        dispatcher.Invoke(() =>
                        {
                            foreach (var mes in o.Message)
                            {
                                item.Logs.Add(new LogItem(o.Type, mes));
                            }
                        });
                    });

                    if (smoList == null)
                        Result.Add(fileCreator.GetFileXML(item.Item, Folder, IsTemp1, null, pr));
                    else
                    {
                        Result.AddRange(smoList.Select(smo => fileCreator.GetFileXML(item.Item, Folder, IsTemp1, smo.SMOCOD, pr)));
                    }

                    dispatcher.Invoke(() =>
                    {
                        var validResult = Result.Where(x => x.Result).ToList();
                        item.SUM = validResult.Sum(x => x.SUM);
                        item.PathArc.AddRange(validResult.Select(x => x.PathARC).ToList());
                    });
                }
                catch (Exception ex)
                {
                    dispatcher.Invoke(() => { item.AddLogs(LogType.Error, $"Ошибка при выгрузке данных: {ex.Message}"); });
                }
                finally
                {
                    dispatcher.Invoke(() =>
                    {
                        item.InWork = false;
                        item.Finish = true;
                    });
                }
            });
        }


        void GetFilesXLS(List<V_EXPORT_H_ZGLVRowVM> Items,  string Folder, List<F002> smoList, DateTime D_START_XLS, DateTime D_END_XLS, CancellationToken cancel)
        {
            var timer = new ActionTimer<string>(o =>
            {
                dispatcher.Invoke(() => { progress3.SetTextValue(0, o); });
            }, 500);
            try
            {
                var list = Items.GroupBy(x => x.Item.CODE_MO).OrderBy(x => x.Key).ToList();
                //Прогресс создания файла
                var pr = new Progress<string>(o => timer.RaiseAction(o));
                dispatcher.Invoke(() => { progress2.SetValues(list.Count, 0, ""); });

                dispatcher.Invoke(() => { progress2.SetValues(list.Count, 0, ""); });
                var index = 1;
                foreach (var pack in list)
                {
                    cancel.ThrowIfCancellationRequested();
                    dispatcher.Invoke(() => { progress2.SetTextValue(index, $"Выгрузка XLS для {pack.Key}"); });
                    index++;
                    pack.First().SUM_XLS = fileCreator.GetFileXLS(pack.ToList().Select(x => x.Item).ToList(), smoList, Folder, D_START_XLS, D_END_XLS, pr);
                }

                dispatcher.Invoke(() => { progress2.SetTextValue(0, ""); });
            }
            catch (Exception ex)
            {
                dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally
            {
                timer.Dispose();
            }
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

    public class ExportFilePARAM : INotifyPropertyChanged
    {
        private DateTime _PERIOD;
        public DateTime PERIOD
        {
            get => _PERIOD;
            set
            {
                _PERIOD = value;
                DATE_1_XLS = new DateTime(value.Year, value.Month, 1);
                DATE_2_XLS = new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month));
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(YEAR));
                RaisePropertyChanged(nameof(MONTH));
            } 
        }
        public int YEAR => _PERIOD.Year;
        public int MONTH => _PERIOD.Month;
        private int _CountTask;
        public int CountTask
        {
            get => _CountTask;
            set
            {
                _CountTask = value;
                RaisePropertyChanged();
            }
        }
        private DateTime _DATE_1_XLS;
        public DateTime DATE_1_XLS
        {
            get => _DATE_1_XLS;
            set
            {
                _DATE_1_XLS = value;
                RaisePropertyChanged();
            }
        }
        private DateTime _DATE_2_XLS;
        public DateTime DATE_2_XLS
        {
            get => _DATE_2_XLS;
            set
            {
                _DATE_2_XLS = value;
                RaisePropertyChanged();
            }
        }
        private bool _IsTEMP1;
        public bool IsTEMP1
        {
            get => _IsTEMP1;
            set
            {
                _IsTEMP1 = value;
                RaisePropertyChanged();
            }
        }
        private bool _IsSMO;
        public bool IsSMO
        {
            get => _IsSMO;
            set
            {
                _IsSMO = value;
                RaisePropertyChanged();
            }
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


    /// <summary>
    /// Класс который будет выполнять действие не чаще чем раз в MS
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ActionTimer<T>:IDisposable
    {
        private T LastValue { get; set; }
        private Timer timer;
        private Action<T> action;
        public ActionTimer(Action<T> action, int MS)
        {
            this.action = action;
            timer = new Timer(state => InvokeAction(), null, 0, MS);
        }

        private void InvokeAction()
        {
            var  item = GetLastValue();
            if (item != null)
            {
                action.Invoke(item);
            }
        }

        private readonly object lockobject = new object();
        private T GetLastValue()
        {
            lock (lockobject)
            {
                var item = LastValue;
                LastValue = default;
                return item;
            }
        }

        public void RaiseAction(T value)
        {
            lock (lockobject)
            {
                LastValue = value;
            }
        }

        public void Dispose()
        {
            timer?.Dispose();
            InvokeAction();
        }
    }
   

}

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
            VM = new ExportFileVM(fileCreator, exportFileRepository, new FileCombiner(), Dispatcher.CurrentDispatcher);
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

        public List<FileCreatorResult> Results { get; } = new List<FileCreatorResult>();


        public void Clear()
        {
            Logs.Clear();
            Finish = false;
            InWork = false;
            SUM = null;
            SUM_XLS = null;
            PathArc.Clear();
            Results.Clear();
        }

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
        private IFileCombiner fileCombiner;
        private IFileCreator fileCreator;
        private Dispatcher dispatcher;
        private IExportFileRepository exportFileRepository;

        public ExportFileVM(IFileCreator fileCreator, IExportFileRepository exportFileRepository, IFileCombiner fileCombiner, Dispatcher dispatcher)
        {
            this.fileCreator = fileCreator;
            this.exportFileRepository = exportFileRepository;
            this.dispatcher = dispatcher;
            this.fileCombiner = fileCombiner;
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
                var items = new List<V_EXPORT_H_ZGLVRow>();
                items.AddRange(await Task.Run(() => exportFileRepository.V_EXPORT_H_ZGLV(PARAM.Source, PARAM.TypeFileCreate, PARAM.PeriodParam, PARAM.SluchParam)));
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
                    item.Clear();
                }

                var items = ZGLV_LIST.Where(x => x.IsSelect).ToList();
                if (PARAM.CountTask <= 0 || PARAM.CountTask > 10)
                    throw new Exception("Количество потоков должно быть от 1 до 10");
                if (items.Count == 0)
                    throw new Exception("Не выбрано не одного файла");

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    List<F002> smoList = null;
                    if (PARAM.TypeFileCreate.In(TypeFileCreate.SMO, TypeFileCreate.FFOMSDx))
                    {
                        progress1.Text = "Запрос справочника F002";
                        smoList = await exportFileRepository.GetF002Async();
                    }

                    cts = new CancellationTokenSource();
                    var tasks = new List<Task> { CreateFilesAsync(items, fbd.SelectedPath, PARAM.Source, PARAM.TypeFileCreate, smoList, cts.Token) };
                    if (PARAM.TypeFileCreate== TypeFileCreate.SMO)
                    {
                        tasks.Add(GetFilesXLSAsync(items,  fbd.SelectedPath, smoList, PARAM.DATE_1_XLS, PARAM.DATE_2_XLS,  cts.Token));
                    }
                    await Task.WhenAll(tasks);

                    var err = CheckResult(items, PARAM.IsSMO);
                    if (CustomMessageBox.Show($"Завершено. Показать файлы?{(err.Count != 0 ? $"{Environment.NewLine}{string.Join(Environment.NewLine, err.Select(x => x))}" : "")}", "") == MessageBoxResult.Yes)
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
            var err = new List<string>();
            var errCount = items.Count(x => (x.IsTypeLog ?? LogType.Info) != LogType.Info);
            if (errCount != 0)
                err.Add($@"Выгрузка содержит {errCount} ошибочных файлов");

            if (IsSMO)
            {
                var result = items.GroupBy(x => x.Item.CODE_MO);
                foreach (var res in result)
                {
                    var sumXls = Math.Round(res.Sum(x => x.SUM_XLS ?? 0), 2);
                    var sumXml = Math.Round(res.Sum(x => x.SUM ?? 0), 2);
                    if (sumXml != sumXls)
                    {
                        err.Add($@"Ошибка контроля сумм XML и XLS для МО = {res.Key} сумма XML ={sumXml} сумма XLS = {sumXls}");
                    }
                }
            }
            return err;
        }


        private Task CreateFilesAsync(List<V_EXPORT_H_ZGLVRowVM> Items, string Folder, DBSource source, TypeFileCreate typeFileCreate, List<F002> smoList, CancellationToken cancel)
        {
            return Task.Run(() => CreateFiles(Items, Folder, source, typeFileCreate, smoList, cancel));
        }
        private void  CreateFiles(List<V_EXPORT_H_ZGLVRowVM> Items, string Folder, DBSource source, TypeFileCreate typeFileCreate, List<F002> smoList, CancellationToken cancel)
        {
            var dicOrder = new Dictionary<string, int>();
            var index = 0;
            var count = Items.Count;
            dispatcher.Invoke(() => { progress1.SetValues(count,0, ""); });
            var parallelManager = new ParallelManager<V_EXPORT_H_ZGLVRowVM>(Items, PARAM.CountTask, item =>
            {
                try
                {
                    var key = $"{item.Item.CODE_MO}{item.Item.FILENAME.Substring(0, 2)}";
                    if (!dicOrder.ContainsKey(key))
                        dicOrder.Add(key, 0);
                    dicOrder[key]++;
                    item.SUM = 0;
                    cancel.ThrowIfCancellationRequested();
                    dispatcher.Invoke(() => { progress1.SetTextValue(index, $"Выгрузка {index} из {count}"); });
                    dispatcher.Invoke(() => { item.InWork = true; });
                    fileCreator.CreateFile(item, Folder, source, typeFileCreate, PARAM.SluchParam, smoList, PARAM.OrderInMonth, dicOrder[key]);
                }
                catch (Exception ex)
                {
                    dispatcher.Invoke(() => { item.AddLogs(LogType.Error, $"Ошибка {ex.Source}: {ex.FullError()}"); });
                }
                finally
                {
                    dispatcher.Invoke(() =>
                    {
                        item.InWork = false;
                        item.Finish = true;
                    });
                }
                index++;
            });

            while (!parallelManager.IsCompleted)
            {
                cancel.ThrowIfCancellationRequested();
            }
            dispatcher.Invoke(() =>
            {
                progress1.Clear();
            });

            if (typeFileCreate.In(TypeFileCreate.MO, TypeFileCreate.MEK_P_P_MO))
            {
                fileCombiner.CreateMOMail(Items, Folder, typeFileCreate, cancel, new Progress<ProgressItemDouble>(o =>
                {
                    dispatcher.Invoke(() =>
                    {
                        progress1.CopyFrom(o.progress1);
                        progress2.CopyFrom(o.progress2);
                    });
                }));
            }

            if (typeFileCreate.In(TypeFileCreate.SMO, TypeFileCreate.MEK_P_P_SMO))
            {
                fileCombiner.CreateSMOMail(Items, Folder, typeFileCreate, cancel, new Progress<ProgressItemDouble>(o =>
                {
                    dispatcher.Invoke(() =>
                    {
                        progress1.CopyFrom(o.progress1);
                        progress2.CopyFrom(o.progress2);
                    });
                }));
            }
            if (typeFileCreate == TypeFileCreate.SLUCH && PARAM.SluchParam.OneFile)
            {
                fileCombiner.CreateSolidFile(Items, Folder,PARAM.NewFileName);
            }
        }
        Task GetFilesXLSAsync(List<V_EXPORT_H_ZGLVRowVM> Items, string Folder, List<F002> smoList, DateTime D_START_XLS, DateTime D_END_XLS, CancellationToken cancel)
        {
            return Task.Run(() => GetFilesXLS(Items, Folder, smoList, D_START_XLS, D_END_XLS, cancel, new Progress<ProgressItemDouble>(mes =>
            {
                progress2.CopyFrom(mes.progress1);
                progress3.CopyFrom( mes.progress2);
            })));
        }

        void GetFilesXLS(List<V_EXPORT_H_ZGLVRowVM> Items,  string Folder, List<F002> smoList, DateTime D_START_XLS, DateTime D_END_XLS, CancellationToken cancel, IProgress<ProgressItemDouble> progress)
        {
            try
            {
                var pItem = new ProgressItemDouble();
                var list = Items.GroupBy(x => x.Item.CODE_MO).OrderBy(x => x.Key).ToList();
                //Прогресс создания файла
                pItem.progress1.SetValues(list.Count, 0, "");
                pItem.progress2.SetValues(list.Count, 0, "");
                progress?.Report(pItem);
                var index = 1;
                foreach (var pack in list)
                {
                    cancel.ThrowIfCancellationRequested();
                    pItem.progress1.SetTextValue(index, $"Выгрузка XLS для {pack.Key}");
                    progress?.Report(pItem);
                    index++;
                    pack.First().SUM_XLS = fileCreator.GetFileXLS(pack.ToList().Select(x => x.Item).ToList(), smoList, Folder, D_START_XLS, D_END_XLS, new Progress<string>(val =>
                    {
                        pItem.progress2.Text = val;
                        progress?.Report(pItem);
                    }));
                }

                pItem.progress1.Clear();
                pItem.progress2.Clear();
                progress?.Report(pItem);
            }
            catch (Exception ex)
            {
                dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
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
        private bool _IsTEMP100;
        public bool IsTEMP100
        {
            get => _IsTEMP100;
            set
            {
                _IsTEMP100 = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsMainBD;
        public bool IsMainBD
        {
            get => _IsMainBD;
            set
            {
                _IsMainBD = value;
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

        private bool _IsMO;
        public bool IsMO
        {
            get => _IsMO;
            set
            {
                _IsMO = value;
                RaisePropertyChanged();
            }
        }

        private bool _IsFFOMSDx;
        public bool IsFFOMSDx
        {
            get => _IsFFOMSDx;
            set
            {
                _IsFFOMSDx = value;
                RaisePropertyChanged();
            }
        }
        private bool _IsMEK_P_P_SMO;
        public bool IsMEK_P_P_SMO
        {
            get => _IsMEK_P_P_SMO;
            set
            {
                _IsMEK_P_P_SMO = value;
                RaisePropertyChanged();
                if (_IsMEK_P_P_SMO)
                {
                    IsMainBD = true;
                }
            }
        }

        private bool _IsMEK_P_P_MO;
        public bool IsMEK_P_P_MO
        {
            get => _IsMEK_P_P_MO;
            set
            {
                _IsMEK_P_P_MO = value;
                RaisePropertyChanged();
                if (_IsMEK_P_P_MO)
                {
                    IsMainBD = true;
                }
            }
        }

        private string _SLUCH_Z_ID;
        public string SLUCH_Z_ID
        {
            get => _SLUCH_Z_ID;
            set
            {
                _SLUCH_Z_ID = value;
                RaisePropertyChanged();
            }
        }
        private bool _SLUCH_Z_ID_IsSMO;
        public bool SLUCH_Z_ID_IsSMO
        {
            get => _SLUCH_Z_ID_IsSMO;
            set
            {
                _SLUCH_Z_ID_IsSMO = value;
                RaisePropertyChanged();
            }
        }
        private bool _OneFile;
        public bool OneFile
        {
            get => _OneFile;
            set
            {
                _OneFile = value;
                RaisePropertyChanged();
            }
        }
        private string _NewFileName;
        public string NewFileName
        {
            get => _NewFileName;
            set
            {
                _NewFileName = value;
                RaisePropertyChanged();
            }
        }

        public SLUCH_PARAM SluchParam
        {
            get
            {
                return new SLUCH_PARAM()
                {
                    SLUCH_Z_ID = SLUCH_Z_ID?.Length >0 ? SLUCH_Z_ID.Split(',').Select(x=>Convert.ToInt64(x)).ToArray(): new long[0],
                    isSMO = SLUCH_Z_ID_IsSMO,
                    NewFileName = NewFileName,
                    OneFile = OneFile
                };

            }
        }
        


        public DBSource Source
        {
            get
            {
                if (IsTEMP1)
                    return DBSource.TEMP1;
                if (IsTEMP100)
                    return DBSource.TEMP100;
                if (IsMainBD)
                    return DBSource.MAIN;
                throw new Exception("Не удалось определить источник данных");
            }
        }
        public PERIOD_PARAM PeriodParam => new PERIOD_PARAM(){Month = MONTH, Year = YEAR};

        public TypeFileCreate TypeFileCreate
        {
            get
            {
                if (!string.IsNullOrEmpty(SLUCH_Z_ID)) return TypeFileCreate.SLUCH;
                if (IsSMO) return TypeFileCreate.SMO;
                if (IsMO) return TypeFileCreate.MO;
                if (IsFFOMSDx) return TypeFileCreate.FFOMSDx;
                if (IsMEK_P_P_SMO) return TypeFileCreate.MEK_P_P_SMO;
                if (IsMEK_P_P_MO) return TypeFileCreate.MEK_P_P_MO;
                throw new Exception("Не удалось определить тип выгрузки");
            }
        }
        private int _OrderInMonth;
        public int OrderInMonth
        {
            get => _OrderInMonth;
            set
            {
                _OrderInMonth = value;
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
   
   
}

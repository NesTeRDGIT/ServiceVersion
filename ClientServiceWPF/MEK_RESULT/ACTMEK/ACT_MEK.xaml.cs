using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ServiceLoaderMedpomData.Annotations;
using MessageBox = System.Windows.MessageBox;

namespace ClientServiceWPF.MEK_RESULT.ACTMEK
{
    /// <summary>
    /// Логика взаимодействия для ACT_MEK.xaml
    /// </summary>
    public partial class ACT_MEK : Window
    {
        public ACT_MEKVM VM { get; set; }
    
        public ACT_MEK()
        {
            VM = new ACT_MEKVM(Dispatcher.CurrentDispatcher, new MEKRepository(AppConfig.Property.ConnectionString), SaveParam);
            InitializeComponent();
        
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            VM.PERIOD = DateTime.Now.AddMonths(-1);
            LoadParam();
        }

         void SaveParam()
        {
            Properties.Settings.Default.ACT_MEK_ISP_DOLG = VM.ISP.DOLG;
            Properties.Settings.Default.ACT_MEK_RUK_DOLG = VM.RUK.DOLG;
            Properties.Settings.Default.ACT_MEK_ISP_FIO = VM.ISP.FIO;
            Properties.Settings.Default.ACT_MEK_RUK_FIO = VM.RUK.FIO;
            Properties.Settings.Default.Save();
        }

        void LoadParam()
        {
            VM.ISP.DOLG = Properties.Settings.Default.ACT_MEK_ISP_DOLG;
            VM.RUK.DOLG = Properties.Settings.Default.ACT_MEK_RUK_DOLG;
            VM.ISP.FIO = Properties.Settings.Default.ACT_MEK_ISP_FIO;
            VM.RUK.FIO = Properties.Settings.Default.ACT_MEK_RUK_FIO;
        }
    }


    public class ACT_MEKVM : INotifyPropertyChanged
    {
        
        private Action SaveParam;
        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        private string ACT_MEK_TEMPLATE { get; set; }=  System.IO.Path.Combine(LocalFolder, "TEMPLATE", "TEMPLATE_ACT_MEK.xlsx");
        private Dispatcher dispatcher;
        private IMEKRepository repository;
        private PODPISANT _ISP = new PODPISANT();
        public PODPISANT ISP
        {
            get => _ISP;
            set
            {
                _ISP = value;
                RaisePropertyChanged();
            }
        }
        private PODPISANT _RUK = new PODPISANT();
        public PODPISANT RUK
        {
            get => _RUK;
            set
            {
                _RUK = value;
                RaisePropertyChanged();
            }
        }

        public ACT_MEKVM(Dispatcher dispatcher, IMEKRepository repository, Action SaveParam)
        {
            this.dispatcher = dispatcher;
            this.repository = repository;
            this.SaveParam = SaveParam;
        }

        #region Progress
        private bool _IsOperationRun;
        public bool IsOperationRun
        {
            get => _IsOperationRun;
            set
            {
                _IsOperationRun = value;
                dispatcher.Invoke(() => RaisePropertyChanged());

            }
        }
        private ProgressItem _Progress1 { get; set; } = new ProgressItem();
        public ProgressItem Progress1
        {
            get => _Progress1;
            set
            {
                _Progress1 = value;
                dispatcher.Invoke(() => RaisePropertyChanged());
            }
        }

        private ProgressItem _Progress2 { get; set; } = new ProgressItem();
        public ProgressItem Progress2
        {
            get => _Progress2;
            set
            {
                _Progress2 = value;
                dispatcher.Invoke(() => RaisePropertyChanged());
            }
        }

        private ProgressItem _Progress3 { get; set; } = new ProgressItem();
        public ProgressItem Progress3
        {
            get => _Progress3;
            set
            {
                _Progress3 = value;
                dispatcher.Invoke(() => RaisePropertyChanged());
            }
        }

        #endregion
        #region MO_LIST
        public ObservableCollection<MO_ITEM> MO_LIST { get; set; } = new ObservableCollection<MO_ITEM>();
        private DateTime _PERIOD;
        public DateTime PERIOD
        {
            get => _PERIOD;
            set
            {
                _PERIOD = value;
                RaisePropertyChanged();
            }
        }
        public ICommand RefreshMO_LISTCommand => new Command(o =>
        {
            try
            {
                Task.Run(()=>
                {
                    try
                    {
                        IsOperationRun = true;
                        dispatcher.Invoke(() => { MO_LIST.Clear(); });
                        var items = repository.GetMO_ITEM(PERIOD.Year, PERIOD.Month);
                        dispatcher.Invoke(() =>
                        {
                            MO_LIST.AddRange(items);
                        });

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        IsOperationRun = false;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => !IsOperationRun);
        public ICommand CheckAllMO_LISTCommand => new Command(o =>
        {
            try
            {
                if (MO_LIST.Count != 0)
                {
                    var max = MO_LIST.Max(x => x.IsSelect);
                    foreach (var moItem in MO_LIST)
                    {
                        moItem.IsSelect = !max;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => !IsOperationRun);
        #endregion MO_LIST

        #region CreateFiles
        private bool _IsMEK = true;
        public bool IsMEK
        {
            get => _IsMEK;
            set
            {
                _IsMEK = value;
                RaisePropertyChanged();
            }
        }
        private bool _isDopMEK = true;
        public bool isDopMEK
        {
            get => _isDopMEK;
            set
            {
                _isDopMEK = value;
                RaisePropertyChanged();
            }
        }
        private FolderBrowserDialog fbd = new FolderBrowserDialog();

        private  CancellationTokenSource cts { get; set; }
        public ICommand SaveCommand => new Command(async  o =>
        {
            try
            {
                if (IsMEK || isDopMEK)
                {
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        IsOperationRun = true;
                        cts = new CancellationTokenSource();
                        var tasks = new List<Task>();
                        SaveParam();
                        if (IsMEK)
                        {
                            tasks.Add(Task.Run(() => { ExportFileMEK(fbd.SelectedPath, cts.Token); }));
                        }
                        if (isDopMEK)
                        {
                            tasks.Add(Task.Run(() => { ExportDop(fbd.SelectedPath, cts.Token); }));
                        }

                        await Task.WhenAll(tasks);
                        if (MessageBox.Show("Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            ShowSelectedInExplorer.FileOrFolder(fbd.SelectedPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                IsOperationRun = false;
            }
        }, o => !IsOperationRun);
        public ICommand BreakCommand => new Command(o =>
        {
            try
            {
                cts?.Cancel();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => IsOperationRun);

      
        private void ExportFileMEK(string folder, CancellationToken cancel)
        {
            try
            {
                var path = Path.Combine(folder, "Акты МЭК");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                var pathSVOD = System.IO.Path.Combine(path, "Своды");
                if (!Directory.Exists(pathSVOD))
                    Directory.CreateDirectory(pathSVOD);
                var selectItem = MO_LIST.Where(x=>x.IsSelect).ToList();
                var i = 1;
                var svod = new Dictionary<string, SVOD_MEK>(); ;
                dispatcher.Invoke(() => { Progress1.IsIndeterminate = true; Progress1.Text = "Запрос суммы МП по выданным направлениям по всем МО"; });
                var NAPR_FROM_MO = repository.FindNAPR_FROM_MO(selectItem.First().YEAR, selectItem.First().MONTH);
                dispatcher.Invoke(() => { Progress1.IsIndeterminate = false; Progress1.Text = ""; Progress1.Maximum = selectItem.Count; Progress1.Value = 0; });
                foreach (var item in selectItem)
                {
                    cancel.ThrowIfCancellationRequested();
                    dispatcher.Invoke(() =>
                    {
                        Progress1.Value = i;
                        Progress1.Text = $"Выгрузка акта для: {item.NAME_MOK}";
                    });
                    var pathFile = CorrectFileName($"Заключение МЭК за {new DateTime(item.YEAR, item.MONTH, 1):yyyy_MM} для {item.CODE_MO}_{item.SMO} №{item.N_ACT} от {item.D_ACT:dd.MM.yyyy} .xlsx");
                    CreateActMEK(item, System.IO.Path.Combine(path, pathFile), svod, NAPR_FROM_MO);
                    i++;
                }
                foreach (var sv in svod)
                {
                    dispatcher.Invoke(() => { Progress1.Text = $"Выгрузка сводного акта для {sv.Key}"; });
                    CreateActSVOD(sv.Value, System.IO.Path.Combine(pathSVOD, $"Заключение МЭК СВОД для {sv.Key}.xlsx"));
                    CreateActSVOD_SHORT(sv.Value, System.IO.Path.Combine(pathSVOD, $"Заключение МЭК СВОД_КРАТКИЙ для {sv.Key}.xlsx"));
                }
            }
            catch (Exception ex)
            {
                dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally
            {
                dispatcher.Invoke(() =>
                {
                    Progress1.Value = Progress2.Value = 0;
                    Progress1.Text = Progress2.Text = "";
                });
            }
        }
        private string CorrectFileName(string path)
        {
            var invalidChar = System.IO.Path.GetInvalidFileNameChars();
            return invalidChar.Aggregate(path, (current, c) => current.Replace(c, '$'));
        }
        private void CreateActMEK(MO_ITEM item, string ACT_MEK_PATH, Dictionary<string, SVOD_MEK> SVOD_MEK, Dictionary<MO_SMO, decimal> FindNAPR_FROM_MO)
        {
            dispatcher.Invoke(() =>
            {
                Progress2.Maximum = 4;
                Progress2.Value = 1;
                Progress2.Text = $"Запрос данных фондодержания";
            });
            var FOND_INFO = repository.FindFOND_INFO(item);
            dispatcher.Invoke(() =>
            {
                Progress2.Value = 2;
                Progress2.Text = "Запрос выполненных объёмов";
            });
            var VOLUME = repository.FindVOLUME(item);
            dispatcher.Invoke(() =>
            {
                Progress2.Value = 3;
                Progress2.Text = "Запрос дефектов";
            });
            //Дефекты
            var DEFECT = repository.FindDEFECT(item);
            dispatcher.Invoke(() =>
            {
                Progress2.Value = 4;
                Progress2.Text = "Создание файла";
            });

            var fnfm = FindNAPR_FROM_MO.Keys.FirstOrDefault(x => x.MO == item.CODE_MO && x.SMO == item.SMO);
            var NAPR_FROM_MO = fnfm != null ? FindNAPR_FROM_MO[fnfm] : 0;

            var creator = new ActMEKCreator(ACT_MEK_TEMPLATE, Progress2, dispatcher);

            var param_mek = creator.ConvertVOLUMEToMEK_PARAM(VOLUME, NAPR_FROM_MO);
            creator.CreateActMEK(item, ACT_MEK_PATH, FOND_INFO, DEFECT, param_mek, ISP, RUK);
            if (!SVOD_MEK.ContainsKey(item.SMO))
                SVOD_MEK.Add(item.SMO, new SVOD_MEK());
            SVOD_MEK[item.SMO].Add(new MO_INFO(item, FOND_INFO, param_mek, DEFECT));
        }
        private void CreateActSVOD(SVOD_MEK svod, string ACT_MEK_PATH)
        {
            dispatcher.Invoke(() => { Progress2.Text = "Формирования СВОДА"; });
            var FOND_INFO = svod.FOND;
            var par = svod.PARAM;
            //Дефекты
            var DEFECT = svod.DEFECT;
            var item = svod.MO_ITEM;
            var creator = new ActMEKCreator(ACT_MEK_TEMPLATE, Progress2, dispatcher);
            creator.CreateActMEK(item, ACT_MEK_PATH, FOND_INFO, DEFECT, par, ISP, RUK);
        }
        private void CreateActSVOD_SHORT(SVOD_MEK svod, string ACT_MEK_PATH)
        {
            dispatcher.Invoke(() => { Progress2.Text = "Формирования СВОДА"; });
            var FOND_INFO = svod.FOND;
            var par = svod.PARAM;
            //Дефекты
            var DEFECT = svod.DEFECT;
            var item = svod.MO_ITEM;
            var creator = new ActMEKCreator(ACT_MEK_TEMPLATE, Progress2, dispatcher);
            creator.CreateActMEK_SHORT(item, ACT_MEK_PATH, FOND_INFO, DEFECT, par, ISP, RUK);
        }

        private void ExportDop(string folder, CancellationToken cancel)
        {
            try
            {
                var path = System.IO.Path.Combine(folder, "Дополнения к актам");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                dispatcher.Invoke(() =>
                {
                    Progress3.Text = "Запрос данных о пересечениях";
                    Progress3.IsIndeterminate = true;
                });
                var dic = repository.GetV_Сrossing(PERIOD.Year, PERIOD.Month);
                dispatcher.Invoke(() => { Progress3.IsIndeterminate = false; Progress3.Text = ""; Progress3.Maximum = dic.Keys.Count; });
                var i = 1;
                foreach (var item in dic)
                {
                    cancel.ThrowIfCancellationRequested();
                    dispatcher.Invoke(() =>
                    {
                        Progress3.Value = i;
                        Progress3.Text = $"Выгрузка дополнения к актам для: {item.Key.CODE_MO} за {item.Key.YEAR}_{item.Key.MONTH:00}";
                    });
                    var pathFile = CorrectFileName($"Дополнение к актам МЭК за  {item.Key.YEAR}_{item.Key.MONTH:00} для {item.Key.CODE_MO}.xlsx");
                    var creator = new ActMEKCreator(ACT_MEK_TEMPLATE, Progress2, dispatcher);
                    creator.CreateFileСrossing(System.IO.Path.Combine(path, pathFile), item.Value);
                    i++;
                }
            }
            catch (Exception ex)
            {
                dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally
            {
                dispatcher.Invoke(() =>
                {
                    Progress3.Value = 0;
                    Progress3.Text = "";
                    Progress3.IsIndeterminate = false;
                });
            }
        }

        #endregion






        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }




    public class PODPISANT : INotifyPropertyChanged
    {
        private string _DOLG;

        public string DOLG
        {
            get => _DOLG;
            set
            {
                _DOLG = value;
                RaisePropertyChanged();
            }
        }
        private string _FIO;
        public string FIO
        {
            get => _FIO;
            set
            {
                _FIO = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }




    public static partial class Ext
    {
        public static bool In(this string val, params string[] par)
        {
            return par.Contains(val);
        }
        public static string FullMessage(this Exception ex)
        {
            if (ex == null) return "";
            var add = ex.InnerException.FullMessage();
            return ex.Message + (string.IsNullOrEmpty(add) ? "" : $";{add}");
        }
    }
}

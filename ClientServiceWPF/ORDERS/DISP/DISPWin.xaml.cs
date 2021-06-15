using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT.FileCreator;
using ClientServiceWPF.ORDERS.ORD104;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using LogType = ClientServiceWPF.Class.LogType;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;


namespace ClientServiceWPF.ORDERS.DISP
{
    /// <summary>
    /// Логика взаимодействия для DISP.xaml
    /// </summary>
    public partial class DISPWin : Window
    {
        public DISPWinVM VM { get; set; } = new DISPWinVM(Dispatcher.CurrentDispatcher);
        public DISPWin()
        {
          
            VM.PARAM.NN = 1;
            VM.PARAM.DATE = DateTime.Now;
            
            InitializeComponent();
        }
    }

    public class DISPWinVM : INotifyPropertyChanged
    {
        private string PATH_XSD = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PR_XSD", "DISP.xsd");
        private Dispatcher dispatcher;

        public DISPWinVM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public ORD15PARAM PARAM { get; } = new ORD15PARAM();

        public ObservableCollection<LogItem> Logs { get; set; } = new ObservableCollection<LogItem>();

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

        public ProgressItem Progress1 { get; } = new ProgressItem();

        public ProgressItem Progress2 { get; } = new ProgressItem();
        private bool _IsOperationRun;
        public bool IsOperationRun
        {
            get => _IsOperationRun;
            set
            {
                _IsOperationRun = value;
                RaisePropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private CancellationTokenSource cts;
        private FolderBrowserDialog fbd = new FolderBrowserDialog() ;
        public ICommand SaveFileCommand => new Command(async o =>
        {
            try
            {
               
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    IsOperationRun = true;
                    Logs.Clear();
                    Progress1.IsIndeterminate = true;
                    cts = new CancellationTokenSource();
                    var files = await GetFileAsync(fbd.SelectedPath, PARAM.DATE, PARAM.FILENAME, PARAM.NN, cts.Token);

                    if (MessageBox.Show(@"Завершено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FilesOrFolders(files);
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
        },o=>!IsOperationRun);

        public ICommand BreakCommand => new Command( o =>
        {
            try
            {
               cts?.Cancel();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        },o=>IsOperationRun);


        Task<List<string>> GetFileAsync(string Folder, DateTime DateFile,string FILENAME, int MIN_NN, CancellationToken cancel)
        {
            return Task.Run(async () =>
            {
                var result = new List<string>();
                var STAT_LIST = new List<STAT>();
                using (var connSQL = new SqlConnection( //"Data Source=ndv-1;Initial Catalog=Marina;Integrated Security=True"
                                                         "Data Source=mssqldb.tfoms.local;Initial Catalog=Marina;Integrated Security=True"
                    ))
                {
                    connSQL.Open();
                    using (var sqlCommand = new SqlCommand("select * from marina.dbo._pers_20210605", connSQL) {CommandType = CommandType.Text})
                    {
                        dispatcher.Invoke(() => { Progress1.Text = "Запрос данных"; });
                        var reader = await sqlCommand.ExecuteReaderAsync(cancel);
                        var COUNT = 0;
                        var persList = new List<PERS>();

                        while (true)
                        {
                            cancel.ThrowIfCancellationRequested();
                            var isREAD = await reader.ReadAsync(cancel);

                            if (isREAD)
                            {
                                COUNT++;
                                persList.Add(PERS.Get(reader));
                            }

                            if (COUNT == 200000 || !isREAD)
                            {
                                var filename = $"{FILENAME}{MIN_NN}.XML";
                              
                                dispatcher.Invoke(() => { Progress1.Text = $"Создание модели файла {filename}"; });
                                var filepath = Path.Combine(Folder, filename);
                                var filepathSTAT = Path.Combine(Folder, $"{filename}.txt");
                                var file = CreateFile(persList, filename, DateFile, cancel);
                                var STAT = new STAT();
                                foreach (var item in file.ZAP)
                                {
                                    STAT.N_ZAP++;
                                    STAT.isC_COVID += item.COV19 != null ? 1 : 0;
                                    STAT.IsZ_SL += item.Z_SL != null ? 1 : 0;
                                    STAT.IsDISPN_C += item.DISPN.Count != 0 ? 1 : 0;
                                    STAT.DISPN_C_ALL += item.DISPN.Count;
                                    STAT.IsSL_C += item.SL.Count != 0 ? 1 : 0;
                                    STAT.SL_C_ALL += item.SL.Count ;
                                    STAT.IsDISP_C += item.DISP != null ? 1 : 0;
                                    STAT.IsDISP21_C += item.DISP21 != null ? 1 : 0;
                                }
                                STAT_LIST.Add(STAT);
                                dispatcher.Invoke(() => { Progress1.Text = $"Сохранение файла {filename}"; });
                                SaveFile(file, filepath);
                                dispatcher.Invoke(() => { Progress1.Text = $"Проверка схемы файла {filename}"; });
                                var sc = new SchemaChecking();
                                var resXSD = sc.CheckXML(filepath, PATH_XSD);
                                if (resXSD.Count != 0)
                                {
                                    AddLogs(LogType.Info,$"Ошибки схемы файла: {filepath}");
                                    AddLogs(LogType.Error, resXSD.Select(x=>x.MessageOUT).ToArray());
                                }
                                STAT.SaveFile(filepathSTAT);
                                result.Add(filepath);
                                COUNT = 0;
                                persList.Clear();
                                MIN_NN++;
                            }

                            if (!isREAD)
                                break;
                        }

                        var SUM_STAT = new STAT();
                        SUM_STAT = STAT_LIST.Aggregate(SUM_STAT, (current, item) => current + item);
                        SUM_STAT.SaveFile(Path.Combine(Folder, $"{FILENAME}.txt"));

                    }
                    connSQL.Close();
                }
                return result;
            }, cancel);
        }


        private class STAT
        {
            /// <summary>
            /// Кол-во записей
            /// </summary>
            public int N_ZAP { get; set; }
            /// <summary>
            /// Кол-во записей c covid19
            /// </summary>
            public int isC_COVID { get; set; }
            /// <summary>
            /// Кол-во записей cо сведениями о последнем обращении за медпомощью
            /// </summary>
            public int IsZ_SL { get; set; }
            /// <summary>
            /// Кол-во записей cо сведениями о нахождении на диспансерном наблюдением
            /// </summary>
            public int IsDISPN_C { get; set; }
            /// <summary>
            /// Кол-во сведений о нахождении на диспансерном наблюдением
            /// </summary>
            public int DISPN_C_ALL { get; set; }
            /// <summary>
            /// Кол-во записей cо сведениями о случаях оказания медицинской помощи по отдельным нозологиям
            /// </summary>
            public int IsSL_C { get; set; }
            /// <summary>
            /// Кол-во сведений о случаях оказания медицинской помощи по отдельным нозологиям
            /// </summary>
            public int SL_C_ALL { get; set; }
            /// <summary>
            /// Кол-во записей c данными по последней диспансеризации за 2019-2020 год
            /// </summary>
            public int IsDISP_C { get; set; }
            /// <summary>
            /// Кол-во записей c данными по последней диспансеризации за 2021 год
            /// </summary>
            public int IsDISP21_C { get; set; }


            public static  STAT operator +(STAT a,STAT b)
            {
                return new STAT()
                {
                    N_ZAP = a.N_ZAP + b.N_ZAP,
                    isC_COVID = a.isC_COVID + b.isC_COVID,
                    IsZ_SL = a.IsZ_SL + b.IsZ_SL,
                    IsDISPN_C = a.IsDISPN_C + b.IsDISPN_C,
                    DISPN_C_ALL = a.DISPN_C_ALL + b.DISPN_C_ALL,
                    IsSL_C = a.IsSL_C + b.IsSL_C,
                    SL_C_ALL = a.SL_C_ALL + b.SL_C_ALL,
                    IsDISP_C = a.IsDISP_C + b.IsDISP_C,
                    IsDISP21_C = a.IsDISP21_C + b.IsDISP21_C
                };
            }

            public void SaveFile(string path)
            {
                File.WriteAllLines(path, new List<string>
                {
                    $"Кол-во записей: {N_ZAP:N0}",
                    $"Кол-во записей c covid19: {isC_COVID:N0}",
                    $"Кол-во записей cо сведениями о последнем обращении за медпомощью: {IsZ_SL:N0}",
                    $"Кол-во записей cо сведениями о нахождении на диспансерном наблюдением: {IsDISPN_C:N0}",
                    $"Кол-во сведений о нахождении на диспансерном наблюдением: {DISPN_C_ALL:N0}",
                    $"Кол-во записей cо сведениями о случаях оказания медицинской помощи по отдельным нозологиям: {IsSL_C:N0}",
                    $"Кол-во сведений о случаях оказания медицинской помощи по отдельным нозологиям: {SL_C_ALL:N0}",
                    $"Кол-во записей c данными по последней диспансеризации за 2019-2020 год: {IsDISP_C:N0}",
                    $"Кол-во записей c данными по последней диспансеризации за 2021 год: {IsDISP21_C:N0}"
                });
            }
        }
        
        

        private  ZL_LIST CreateFile(List<PERS> persList,string FileName, DateTime DT,CancellationToken cancel)
        {
            var file = new ZL_LIST {ZGLV = {FILENAME = FileName, DATA = DT}};
            var count = 1;
            var ENP = persList.Select(x => x.ENP).ToList().Distinct().ToList();
            var dic_data = new Dictionary<string, SVOD_DATA>();
            var part =  ENP.PartitionList(800);
            dispatcher.Invoke(() => { Progress2.SetValues(part.Count, 0, "Сбор данных"); });
            var c = 0;
            var tm = new TaskManager(10);

           
            foreach (var enps in part)
            {
                tm.ThrowIfException();
                c++;
                dispatcher.Invoke(() => { Progress2.Value = c; });
                var fi = tm.WaitFreeItem(cancel);
                
                lock (enps)
                {
                    foreach (var enp in enps)
                    {
                        dic_data.Add(enp, new SVOD_DATA());
                    }
                }
                fi.Free = false;
                fi.TSK = getSmDataAsync(enps, dic_data);
            }
            dispatcher.Invoke(() => { Progress2.Text = "Ожидание завершения"; });
            tm.WaitIsSTOP(cancel);
            tm.ThrowIfException();
            dispatcher.Invoke(() => { Progress2.Text = "Создание файла"; });
            foreach (var p in persList)
            {
                var z = new ZAP {N_ZAP = count, PERS = p};
                file.ZAP.Add(z);
                var dic = dic_data[p.ENP];
                z.COV19 = dic.Cov19.Where(x => x.USL_OK == 1).OrderByDescending(x => x.DSCDATE).FirstOrDefault() ?? dic.Cov19.OrderByDescending(x => x.DSCDATE).FirstOrDefault();
                z.Z_SL = dic.Z_sl.OrderByDescending(x => x.DATE_Z_1).FirstOrDefault();
                z.DISPN = dic.DN.Where(x => x.Is2021).ToList();
                z.SL = dic.DN.Where(x => x.IsSL).Select(x=> new SL{DS = x.DS, DSG = x.DSG}).ToList();
                z.DISP = dic.Disp.Where(x => x.DATEDISP.Year == 2020 || x.DATEDISP.Year == 2019).OrderByDescending(x=>x.DATEDISP).FirstOrDefault();
                z.DISP21 = dic.Disp.Where(x => x.DATEDISP.Year == 2021).OrderByDescending(x=>x.DATEDISP).FirstOrDefault();
                count++;
            }
            dispatcher.Invoke(() => { Progress2.Text = "Завершено"; });
            return file;

        }

        private Task getSmDataAsync(List<string> enps, Dictionary<string, SVOD_DATA> dic_data)
        {
            return Task.Run(async () =>
            {

                var covidTask = GetCovAsync(enps);
                var disTask = GetDispAsync(enps);
                var z_slTask = GetZ_SLAsync(enps);
                var dnTask = GetDNAsync(enps);

                var covid = await covidTask;
                var disp = await disTask;
                var z_sl = await z_slTask;
                var dn = await dnTask;
                lock (dic_data)
                {
                    foreach (var item in covid)
                    {
                        dic_data[item.ENP_REG].Cov19.Add(item);
                    }

                    foreach (var item in disp)
                    {
                        dic_data[item.ENP_REG].Disp.Add(item);
                    }

                    foreach (var item in z_sl)
                    {
                        dic_data[item.ENP_REG].Z_sl.Add(item);
                    }

                    foreach (var item in dn)
                    {
                        dic_data[item.ENP_REG].DN.Add(item);
                    }
                }
            }
        );
    }
        class  SVOD_DATA
        {
            public List<COV19> Cov19 { get; set; } = new List<COV19>();
            public List<DISP> Disp { get; set; } = new List<DISP>();
            public List<Z_SL> Z_sl { get; set; } = new List<Z_SL>();
            public List<DISPN> DN { get; set; } = new List<DISPN>();

           
        }


        private Task<List<COV19>> GetCovAsync(List<string> ENP)
        {
            return Task.Run(() =>
            {
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from DISP_COVID where ENP_REG in ({string.Join(",", ENP.Select(x => $"'{x}'"))})", conn))
                    {
                        var tbl = new DataTable();
                        oda.Fill(tbl);
                        return tbl.Select().Select(COV19.Get).ToList();
                    }
                }
            });
        }

        private Task<List<DISP>> GetDispAsync(List<string> ENP)
        {
            return Task.Run(() =>
            {
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from DISP_DISP where ENP_REG in ({string.Join(",", ENP.Select(x => $"'{x}'"))})", conn))
                    {
                        var tbl = new DataTable();
                        oda.Fill(tbl);
                        return tbl.Select().Select(DISP.Get).ToList();
                    }
                }
            });
        }

        private Task<List<Z_SL>> GetZ_SLAsync(List<string> ENP)
        {
            return Task.Run(() =>
            {
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from DISP_Z_SL where ENP_REG in ({string.Join(",", ENP.Select(x => $"'{x}'"))})", conn))
                    {
                        var tbl = new DataTable();
                        oda.Fill(tbl);
                        return tbl.Select().Select(Z_SL.Get).ToList();
                    }
                }
            });
        }


        private Task<List<DISPN>> GetDNAsync(List<string> ENP)
        {
            return Task.Run(() =>
            {
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from DISP_DN where ENP_REG in ({string.Join(",", ENP.Select(x => $"'{x}'"))})", conn))
                    {
                        var tbl = new DataTable();
                        oda.Fill(tbl);
                        return tbl.Select().Select(DISPN.Get).ToList();
                    }
                }
            });
        }

        private void SaveFile(ZL_LIST file, string filepath)
        {
            using (var st = File.Create(filepath))
            {
                file.WriteXml(st);
            }
        }

       
        private string GetXML(ZL_LIST file)
        {
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(ms))
                {

                    return sr.ReadToEnd();
                }
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
    public class ORD15PARAM : INotifyPropertyChanged
    {
        private DateTime _DATE;
        public DateTime DATE
        {
            get => _DATE;
            set
            {
                _DATE = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FILENAME));
                RaisePropertyChanged(nameof(FILENAME_VIEW));
            }
        }
        private int _NN;
        public int NN
        {
            get => _NN;
            set
            {
                _NN = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FILENAME_VIEW));
            }
        }
      
        public string FILENAME_VIEW => $"{FILENAME}{{{NN}}}";
        public string FILENAME => $"DISP75{DATE:yyMMdd}";

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public static partial class Ext
    {
        public static List<List<string>> PartitionList(this List<string> List, int countList)
        {
            var resuList = new List<List<string>>();
            var rez_sub = new List<string>();
            int count = 0;
            var last = List.LastOrDefault();
            foreach (var item in List)
            {
                rez_sub.Add(item);
                count++;
                if (count == countList || item == last)
                {
                    count = 0;
                    resuList.Add(rez_sub);
                    rez_sub = new List<string>();
                }
            }
            return resuList;
        }
    }
}

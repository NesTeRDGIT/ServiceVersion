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

using System.Windows.Threading;
using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using ClientServiceWPF.MEK_RESULT.FileCreator;

using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;

using LogType = ClientServiceWPF.Class.LogType;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;


namespace ClientServiceWPF.ORDERS.SchetPalat
{
    /// <summary>
    /// Логика взаимодействия для PalatWin.xaml
    /// </summary>
    public partial class PalatWin : Window
    {
        public PalatWinVM VM { get; set; } = new PalatWinVM(Dispatcher.CurrentDispatcher);
    
        public PalatWin()
        {
            VM.PARAM.DATE_B = new DateTime(2018, 1, 1);
            VM.PARAM.DATE_E = new DateTime(2021, 5, 31);
            InitializeComponent();
        }
    }

    public class PalatWinVM : INotifyPropertyChanged
    {
        private Dispatcher dispatcher;

        public PalatWinVM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public PalatPARAM PARAM { get; } = new PalatPARAM();

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
        private FolderBrowserDialog fbd = new FolderBrowserDialog();
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
                    var files = await GetFileAsync(fbd.SelectedPath, PARAM.DATE_B, PARAM.DATE_E, cts.Token);

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



        class ID_PACGenerator
        {
            private Dictionary<string, string> dictPacient = new Dictionary<string, string>();
            private int NUM { get; set; } = 1;
            public void Clear()
            {
                dictPacient.Clear();
                NUM = 1;
            }

            public string GetID_PAC(string value)
            {
                if (!dictPacient.ContainsKey(value))
                {
                    dictPacient.Add(value, $"{NUM:75_00000000}");
                    NUM++;
                }
                return dictPacient[value];
            }
        }

        private ID_PACGenerator IdPacGenerator = new ID_PACGenerator();


        private void ModernFile(List<ZAP> zaps)
        {
            var dic = zaps.ToDictionary(zap => zap.SLUCH_ID);
            var part = zaps.Select(x => x.SLUCH_ID).Distinct().ToList().Partition(800);
            foreach (var p in part)
            {
                var ds = GetDS2_DS3(p);
                foreach (DataRow row in ds.Rows)
                {
                    switch (row["TYPE"].ToString())
                    {
                        case "DS2": dic[Convert.ToInt64(row["SLUCH_ID"])].DS2_LIST.Add(row["DS"].ToString()); break;
                        case "DS3": dic[Convert.ToInt64(row["SLUCH_ID"])].DS3_LIST.Add(row["DS"].ToString()); break;
                    }
                }
            }

            foreach (var zap in zaps)
            {
                zap.ID_PAC = IdPacGenerator.GetID_PAC(zap.ENP_REG);
            }
        }


        private void Save(List<ZAP> zaps, string path)
        {
            var data = new OutputData() { ZAP = zaps };
            using (var st = File.Create(path))
            {
                data.WriteXml(st);
            }
            var data_map = zaps.Select(x=>new MapZap(){ENP = x.ENP_REG, ID_PAC = x.ID_PAC, SLUCH_ID = x.SLUCH_ID}).ToList();
            using (var st = File.Create($"{path}.xmlmap"))
            {
                data_map.WriteXml(st);
            }

        }


        string connStr = AppConfig.Property.ConnectionString;

        private DataTable GetDS2_DS3(List<long> sluch_id)
        {
            using (var conn = new OracleConnection(connStr))
            {
                using (var oda = new OracleDataAdapter($"select * from V_SCHET_PALAT_DS where SLUCH_ID in ({string.Join(",", sluch_id)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
        }


        Task<List<string>> GetFileAsync(string Folder, DateTime DATE_B, DateTime DATE_E, CancellationToken cancel)
        {
            IdPacGenerator.Clear();
            return Task.Run(async () =>
            {
                var result = new List<string>();
                using (var conn = new OracleConnection(connStr))
                {
                    using (var cmd = new OracleCommand($"select * from V_SCHET_PALAT where DATE_2 between '{DATE_B:dd.MM.yyyy}' and '{DATE_E:dd.MM.yyyy}' order by DATE_2", conn))
                    {
                        dispatcher.Invoke(() => {  Progress1.Text = "Запрос данных"; Progress1.IsIndeterminate = true; });
                        await conn.OpenAsync(cancel);
                        var reader = await cmd.ExecuteReaderAsync(cancel);
                        DateTime? LastPeriod = null;
                        var num_file = 0; 
                        List<ZAP> ListValue = new List<ZAP>();
                        long CountAll = 0;
                        while (true)
                        {
                            var isREAD = await reader.ReadAsync(cancel);
                            CountAll++;
                           
                            if (isREAD)
                            {
                                var z = ZAP.Get(reader);
                                if (!LastPeriod.HasValue)
                                {
                                    LastPeriod = z.DATE_2;
                                }
                                dispatcher.Invoke(() => { Progress1.Text = $"Обработка {CountAll} строки {LastPeriod:yyyy-MM}"; });
                                var isChangePeriod = LastPeriod.Value.Year != z.DATE_2.Year || LastPeriod.Value.Month != z.DATE_2.Month;
                                if (isChangePeriod)
                                {
                                    num_file++;
                                    var path = Path.Combine(Folder, $"ShP_T75_F{LastPeriod:yy}{LastPeriod:MM}{num_file:00}.xml");
                                    dispatcher.Invoke(() => { Progress2.Text = $"Модернизация файла {path}"; });
                                    ModernFile(ListValue);
                                    dispatcher.Invoke(() => { Progress2.Text = $"Сохранение файла {path}"; });
                                    Save(ListValue, path);
                                    result.Add(path);
                                    LastPeriod = z.DATE_2;
                                    num_file = 0;
                                    ListValue.Clear();
                                    dispatcher.Invoke(() => { Progress2.Text = $""; });
                                }
                                ListValue.Add(z);
                                if (ListValue.Count == 100000)
                                {
                                    num_file++;
                                    var path = Path.Combine(Folder, $"ShP_T75_F{LastPeriod:yy}{LastPeriod:MM}{num_file:00}.xml");
                                    dispatcher.Invoke(() => { Progress2.Text = $"Модернизация файла {path}"; });
                                    ModernFile(ListValue);
                                    dispatcher.Invoke(() => { Progress2.Text = $"Сохранение файла {path}"; });
                                    Save(ListValue, path);
                                    result.Add(path);
                                    ListValue.Clear();
                                    dispatcher.Invoke(() => { Progress2.Text = $""; });
                                }
                            }
                            else
                            {
                                if (ListValue.Count != 0)
                                {
                                    num_file++;
                                    var path = Path.Combine(Folder, $"ShP_T75_F{LastPeriod:yy}{LastPeriod:MM}{num_file:00}.xml");
                                    dispatcher.Invoke(() => { Progress2.Text = $"Модернизация файла {path}"; });
                                    ModernFile(ListValue);
                                    dispatcher.Invoke(() => { Progress2.Text = $"Сохранение файла {path}"; });
                                    Save(ListValue, path);
                                    result.Add(path);
                                    ListValue.Clear();
                                    dispatcher.Invoke(() => { Progress2.Text = $""; });
                                }
                                break;
                            }
                        }
                        return result;
                    }
                    
                }
               
            }, cancel);

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
    public class PalatPARAM : INotifyPropertyChanged
    {
        private DateTime _DATE_B;
        public DateTime DATE_B
        {
            get => _DATE_B;
            set
            {
                _DATE_B = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _DATE_E;
        public DateTime DATE_E
        {
            get => _DATE_E;
            set
            {
                _DATE_E = value;
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

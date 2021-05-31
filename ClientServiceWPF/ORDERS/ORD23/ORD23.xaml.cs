using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using LogType = ClientServiceWPF.Class.LogType;
using Path = System.IO.Path;

namespace ClientServiceWPF.ORDERS.ORD23
{
    /// <summary>
    /// Логика взаимодействия для ORD23.xaml
    /// </summary>
    public partial class ORD23 : Window
    {
        public ORD23VM VM { get; set; } = new ORD23VM(Dispatcher.CurrentDispatcher);
        public ORD23()
        {
            VM.PARAM.PERIOD = DateTime.Now.AddMonths(-1);
            VM.PARAM.DATE = DateTime.Now;
            InitializeComponent();
        }
    }


    public class ORD23VM : INotifyPropertyChanged
    {
        private string PATH_XSD = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PR_XSD", "PR23.xsd");
        private Dispatcher dispatcher;

        public ORD23VM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public ORD23PARAM PARAM { get; } = new ORD23PARAM();

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

        private void AddLogAndProgress(LogType type,  string Message)
        {
            dispatcher.Invoke(() =>
                {
                    Logs.Add(new LogItem(type, Message));
                    Progress1.Text = Message;
                }
            );
        }

        public ProgressItem Progress1 { get; } = new ProgressItem();

        private SaveFileDialog sfd = new SaveFileDialog() { Filter = "*.xml|*.xml" };
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


        public ICommand SaveFileCommand => new Command(async o =>
        {
            try
            {
                sfd.FileName = PARAM.FILENAME;
                if (sfd.ShowDialog() == true)
                {
                    IsOperationRun = true;
                    Logs.Clear();
                  
                    await Task.Run(() => { GetFileTRK(sfd.FileName, PARAM.FILENAME, PARAM.DATE, PARAM.ISP ? PARAM.ISP_NAME : null, PARAM.YEAR, PARAM.MONTH, PARAM.NN); });
                    if (MessageBox.Show(@"Завершено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(sfd.FileName);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Progress1.Clear("");
                IsOperationRun = false;
            }
        });

        void GetFileTRK(string path, string FILENAME, DateTime DateFile, string ISPName, int YEAR, int MONTH, int NN)
        {
            try
            {
                dispatcher.Invoke(() => { Progress1.IsIndeterminate = true; });
                AddLogAndProgress(LogType.Info, "Запрос справочника КСЛП");

                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {


                    var KSLP_KOEF = new DataTable();
                    var oda = new OracleDataAdapter(
                        @"select distinct to_char(t.lpu) lpu,to_char(t.lpu_1) lpu_1,to_char(t.usl_ok) usl_ok,t.tip_koef,t.koef, to_date('01.'||lpad(t.month_beg,2,0)||'.'||t.year_beg) date_b,case when t.year_end is null or  t.month_end is null then sysdate else  to_date('27.'||lpad(t.month_end,2,0)||'.'||t.year_end) end date_e from nsi.MEDCARE_KOEF t",
                        conn);
                    oda.Fill(KSLP_KOEF);
                    AddLogAndProgress(LogType.Info, "Создание ZGLV");
                    var xml = new ISP_OB {ZGLV = {VERSION = "2.0", DATA = DateFile, FILENAME = FILENAME}};
                    if (!string.IsNullOrEmpty(ISPName))
                        xml.ZGLV.FIRSTNAME = ISPName;
                    AddLogs(LogType.Info, "Создание SVD");
                    xml.SVD.CODE = YEAR * 10000 + NN;
                    if (MONTH == 12)
                    {
                        xml.SVD.YEAR = YEAR + 1;
                        xml.SVD.MONTH = 1;
                    }
                    else
                    {
                        xml.SVD.YEAR = YEAR;
                        xml.SVD.MONTH = MONTH + 1;
                    }

                    AddLogAndProgress(LogType.Info, "Запрос случаев");

                    var tbl = new DataTable();
                    oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_SLUCH t where t.year = {YEAR} and t.month = {MONTH}", conn);
                    oda.Fill(tbl);
                    AddLogAndProgress(LogType.Info, "Запрос КСЛП случаев");
                    var tlbkslp = new DataTable();
                    foreach (var sl in GetIDFromDataTable(tbl.Select("ISMTR = 0"), "SLUCH_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_KSLP t where ISMTR = 0 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(tlbkslp);
                    }

                    foreach (var sl in GetIDFromDataTable(tbl.Select("ISMTR = 1"), "SLUCH_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_KSLP t where ISMTR = 1 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(tlbkslp);
                    }

                    var ds3tbl = new DataTable();

                    AddLogAndProgress(LogType.Info, "Запрос DS2-DS3 случаев");
                    var ds2tbl = new DataTable();
                    var crittbl = new DataTable();
                    foreach (var sl in GetIDFromDataTable(tbl.Select("ISMTR = 0"), "SLUCH_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_DS2 t where ISMTR = 0 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(ds2tbl);
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_DS3 t where ISMTR = 0 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(ds3tbl);
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_CRIT t where ISMTR = 0 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(crittbl);
                    }

                    foreach (var sl in GetIDFromDataTable(tbl.Select("ISMTR = 1"), "SLUCH_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_DS2 t where ISMTR = 1 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(ds2tbl);
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_DS3 t where ISMTR = 1 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(ds3tbl);
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_CRIT t where ISMTR = 1 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(crittbl);
                    }

                    AddLogAndProgress(LogType.Info, "Запрос услуг");
                    var usltbl = new DataTable();
                    foreach (var sl in GetIDFromDataTable(tbl.Select("ISMTR = 0"), "SLUCH_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_USL t where ISMTR = 0 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(usltbl);
                    }

                    foreach (var sl in GetIDFromDataTable(tbl.Select("ISMTR = 1"), "SLUCH_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from V_EXPORT_TKR_USL t where ISMTR = 1 and t.sluch_id in ({string.Join(",", sl)})", conn);
                        oda.Fill(usltbl);
                    }

                    dispatcher.Invoke(() =>
                    {
                        Progress1.IsIndeterminate = false; 
                        Progress1.SetValues(tbl.Rows.Count, 0, $"Обработка данных: 0 из {tbl.Rows.Count}");
                    });
                    var j = 0;
                    foreach (DataRow row in tbl.Rows)
                    {
                        try
                        {
                            var z = new ZAP {N_ZAP = j + 1};
                            xml.PODR.Add(z);
                            z.PACIENT = PACIENT.Get(row);
                            z.SLUCH = SLUCH.Get(row, ds2tbl.Select($"SLUCH_ID = {row["SLUCH_ID"]}"), ds3tbl.Select($"SLUCH_ID = {row["SLUCH_ID"]}"), tlbkslp.Select($"SLUCH_ID = {row["SLUCH_ID"]}"),
                                usltbl.Select($"SLUCH_ID = {row["SLUCH_ID"]}"), crittbl.Select($"SLUCH_ID = {row["SLUCH_ID"]}", "ORD"));
                            //пересчет КСЛП
                            z.SLUCH.SL_K = z.SLUCH.SL_KOEF.Count == 0 ? 0 : 1;
                            if (z.SLUCH.SL_KOEF.Count != 0)
                            {
                                z.SLUCH.IT_SL = z.SLUCH.SL_KOEF.Sum(x => x.Z_SL - 1) + 1;
                            }

                            j++;
                            dispatcher.Invoke(() => { Progress1.SetTextValue(j, $"Обработка данных: {j} из {tbl.Rows.Count}"); });
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"Для случая {row["SLUCH_ID"]}: {ex.Message}", ex);
                        }
                    }

                    dispatcher.Invoke(() => { Progress1.IsIndeterminate = true; });
                    AddLogAndProgress(LogType.Info, "Сохранение XML");
                    using (var st = File.Create(path))
                    {
                        xml.WriteXml(st);
                        st.Close();
                    }

                    AddLogAndProgress(LogType.Info, "Проверка схемы");
                    var sc = new ServiceLoaderMedpomData.SchemaChecking();
                    var err = sc.CheckXML(path, PATH_XSD);
                    AddLogs(LogType.Error, err.Select(x => x.MessageOUT).ToArray());
                    AddLogAndProgress(LogType.Info, "Завершено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<int[]> GetIDFromDataTable(DataTable tbl, string column_name)
        {
            return GetIDFromDataTable(tbl.Select(), column_name);

        }
        private List<int[]> GetIDFromDataTable(IEnumerable<DataRow> rows, string column_name)
        {
            const int countList = 500;
            var rez = new List<int[]>();
            var rez_sub = new List<int>();
            var count = 0;
            var list = rows.Select(row => row[column_name]).Select(Convert.ToInt32).Distinct();

            foreach (var val in list)
            {
                rez_sub.Add(val);
                count++;
                if (count == countList)
                {
                    count = 0;
                    rez.Add(rez_sub.ToArray());
                    rez_sub = new List<int>();
                }
            }

            if (count != 0)
            {
                rez.Add(rez_sub.ToArray());
            }

            return rez;

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
    public class ORD23PARAM : INotifyPropertyChanged
    {
        private DateTime _PERIOD;
        public DateTime PERIOD
        {
            get => _PERIOD;
            set
            {
                _PERIOD = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(YEAR));
                RaisePropertyChanged(nameof(MONTH));
                RaisePropertyChanged(nameof(FILENAME));
            }
        }
        public int YEAR => _PERIOD.Year;
        public int MONTH => _PERIOD.Month;
        private DateTime _DATE;
        public DateTime DATE
        {
            get => _DATE;
            set
            {
                _DATE = value;
                RaisePropertyChanged();
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
                RaisePropertyChanged(nameof(FILENAME));
            }
        }
        private bool _ISP;
        public bool ISP
        {
            get => _ISP;
            set
            {
                _ISP = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FILENAME));
            }
        }
        private string _ISP_NAME;
        public string ISP_NAME
        {
            get => _ISP_NAME;
            set
            {
                _ISP_NAME = value;
                RaisePropertyChanged();
            }
        }
        public string FILENAME => $"{(ISP ? "TKRS" : "TKR")}75{YEAR.ToString().Substring(2)}{NN:D4}";

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

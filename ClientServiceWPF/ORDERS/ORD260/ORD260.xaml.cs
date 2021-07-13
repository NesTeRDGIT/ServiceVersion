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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ClientServiceWPF.ORDERS.ORD104;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData.Annotations;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace ClientServiceWPF.ORDERS.ORD260
{
    /// <summary>
    /// Логика взаимодействия для ORD260.xaml
    /// </summary>
    public partial class ORD260 : Window
    {
        public ORD260VM VM { get; set; } = new ORD260VM(Dispatcher.CurrentDispatcher);
        public ORD260()
        {
            VM.PARAM.PERIOD = DateTime.Now.AddMonths(-1);
            VM.PARAM.DATE = DateTime.Now;
            VM.PARAM.С_FILE = true;
            InitializeComponent();
        }
    }


    public class ORD260VM : INotifyPropertyChanged
    {
        private string PATH_XSD_T = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PR_XSD", "TPR260.xsd");
        private string PATH_XSD_C = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PR_XSD", "CPR260.xsd");
        private string PATH_XSD_L = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PR_XSD", "LPR260.xsd");
        private Dispatcher dispatcher;

        public ORD260VM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public ORD260PARAM PARAM { get; } = new ORD260PARAM();

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

        private FolderBrowserDialog fbd = new FolderBrowserDialog();
        public ICommand SaveFileCommand => new Command(async o =>
        {
            try
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    IsOperationRun = true;
                    Logs.Clear();
                    await Task.Run(() => { GetFile(fbd.SelectedPath, PARAM.FILENAME, PARAM.YEAR, PARAM.MONTH, PARAM.С_FILE);});

                    if (MessageBox.Show(@"Завершено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(fbd.SelectedPath);
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

        void GetFile(string folder, string FILENAME, int YEAR, int MONTH, bool C_FILE)
        {
            try
            {
                var FilenameH = FILENAME;
                var FilenameL = $"L{FILENAME}";
                dispatcher.Invoke(() => { Progress1.IsIndeterminate = true; });
                AddLogs(LogType.Info, "Запрос счетов");
                var SCHETtbl = new DataTable();
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from PR260_SCHET  t where year = {YEAR} and month = {MONTH} and filename like '{(C_FILE ? "C" : "T")}%' order by IsMTR", conn))
                    {
                        oda.Fill(SCHETtbl);
                    }
                }
                var file = new ZL_LIST();
                var fileL = new PERS_LIST();
                AddLogs(LogType.Info, "Запрос данных");
                var i = 0;
                var dictPERS = new Dictionary<decimal, PERS>();
                dispatcher.Invoke(() =>
                {
                    Progress1.IsIndeterminate = false;
                    Progress1.Maximum = SCHETtbl.Rows.Count;
                });
                foreach (DataRow row in SCHETtbl.Rows)
                {
                    i++;
                    dispatcher.Invoke(() => { Progress1.SetTextValue( i, "Запрос данных"); });

                    SCHET_AND_PERS val;
                    if (Convert.ToInt32(row["IsMTR"]) == 0)
                    {
                        val = GetSchet(row, "75001");
                        val.sc.PLAT = "75001";
                        if (val.sc.ZAP.Count != 0)
                        {
                            file.SCHET.Add(val.sc);
                            AddDict(dictPERS, val.pers);
                        }
                        val = GetSchet(row, "75003");
                        val.sc.PLAT = "75003";
                        if (val.sc.ZAP.Count != 0)
                        {
                            file.SCHET.Add(val.sc);
                            AddDict(dictPERS, val.pers);
                        }
                    }
                    else
                    {
                        val = GetSchet(row, "75");
                        val.sc.PLAT = null;
                        if (val.sc.ZAP.Count != 0)
                        {
                            file.SCHET.Add(val.sc);
                            AddDict(dictPERS, val.pers);
                        }
                    }
                }
                dispatcher.Invoke(() =>
                {
                    Progress1.Text =  "Формирование файлов";
                    Progress1.IsIndeterminate = true;
                });

                fileL.PERS.AddRange(dictPERS.Select(x => x.Value));
                AddLogs(LogType.Info, "Пересчет");
                file.ZGLV.VERSION = fileL.ZGLV.VERSION = "3.1";
                file.ZGLV.DATA = fileL.ZGLV.DATA = DateTime.Now;
                file.ZGLV.FILENAME = fileL.ZGLV.FILENAME1 = FilenameH;
                fileL.ZGLV.FILENAME = FilenameL;
                file.ZGLV.SD_Z = file.SCHET.Sum(x => x.ZAP.Count);
                i = 0;
                foreach (var t in file.SCHET)
                {
                    i++;
                    t.CODE = i;
                }

                var DIC = new Dictionary<string, PERS>();
                foreach (var t in fileL.PERS)
                {
                    if (DIC.ContainsKey(t.ID_PAC))
                    {
                        AddLogs(LogType.Error, $"Файл перс данных содержит дубляж PERS_ID = {t.ID_PAC}");
                    }
                    else
                        DIC.Add(t.ID_PAC, t);
                }

                foreach (var p in file.SCHET.SelectMany(x => x.ZAP).Select(x => x.PACIENT))
                {
                    if (!DIC.ContainsKey(p.ID_PAC))
                    {
                        AddLogs(LogType.Error, $"Файл перс данных не содержит PERS_ID = {p.ID_PAC} не найден в файле перс данных");
                    }
                }

                var pathfile = Path.Combine(folder, $"{FilenameH}.xml");
                AddLogs(LogType.Info, $"Сохранение файла {pathfile}");
                using (var st = File.Create(pathfile))
                {
                    file.WriteXml(st);
                    st.Close();
                }

                var pathfileL = Path.Combine(folder, $"{FilenameL}.xml");
                AddLogs(LogType.Info, $"Сохранение файла {pathfileL}");

                using (var st = File.Create(pathfileL))
                {
                    fileL.WriteXml(st);
                    st.Close();
                }
                AddLogs(LogType.Info, "Проверка схемы");
                var sc = new ServiceLoaderMedpomData.SchemaChecking();
                var err = sc.CheckXML(pathfile, C_FILE? PATH_XSD_C: PATH_XSD_T);
                AddLogs(LogType.Error, err.Select(x => x.MessageOUT).ToArray());
                err = sc.CheckXML(pathfileL, PATH_XSD_L);
                AddLogs(LogType.Error, err.Select(x => x.MessageOUT).ToArray());
                AddLogs(LogType.Info, "Завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        SCHET_AND_PERS GetSchet(DataRow schetRow, string SMO)
        {
            var isMTR = SMO == "75" ? 1 : 0;
            try
            {
                using (var connInner = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    var ZAP = new DataTable();
                    var schet_id = Convert.ToInt32(schetRow["SCHET_ID"]);
                    var oda = new OracleDataAdapter($"select * from PR260_ZAP  t where schet_id = {schet_id} {(isMTR == 0 ? $"and SMO = '{SMO}'" : "")} and isMTR = {isMTR}", connInner);
                    oda.Fill(ZAP);

                    var SLUCH = new DataTable();
                    var SANK = new DataTable();
                    foreach (var sl in GetIDFromDataTable(ZAP, "SLUCH_Z_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from PR260_SLUCH  t where  isMTR = {isMTR} and SLUCH_Z_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(SLUCH);
                        //SANK
                        oda = new OracleDataAdapter($"select * from PR260_SANK  t where isMTR = {isMTR} and SLUCH_Z_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(SANK);
                    }

                    var SL_KOEF = new DataTable();
                    var USL = new DataTable();
                    var ONK_USLtbl = new DataTable();
                    var LEK_PR = new DataTable();
                    var NAPR = new DataTable();
                    var B_PROT = new DataTable();
                    var H_CONS = new DataTable();
                    var DS2 = new DataTable();
                    var DS3 = new DataTable();
                    var CRIT = new DataTable();
                    var B_DIAG = new DataTable();

                    foreach (var sl in GetIDFromDataTable(SLUCH, "SLUCH_ID"))
                    {
                        //KOEF
                        oda = new OracleDataAdapter($"select * from PR260_SL_KOEF  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(SL_KOEF);
                        //USL
                        oda = new OracleDataAdapter($"select * from PR260_USL  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(USL);
                        //ONK_USL_PR
                        oda = new OracleDataAdapter($"select * from PR260_ONK_USL  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(ONK_USLtbl);
                        //LEK_PR
                        oda = new OracleDataAdapter($"select * from PR260_LEK_PR  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(LEK_PR);
                        //LEK_PR
                        oda = new OracleDataAdapter($"select * from PR260_NAPR  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(NAPR);
                        //B_PROT
                        oda = new OracleDataAdapter($"select * from PR260_B_PROT  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(B_PROT);
                        //H_CONS
                        oda = new OracleDataAdapter($"select * from PR260_CONS  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(H_CONS);
                        //B_DIAG
                        oda = new OracleDataAdapter($"select * from PR260_B_DIAG  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(B_DIAG);
                        //DS2
                        oda = new OracleDataAdapter($"select * from PR260_DS2  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(DS2);
                        //DS3
                        oda = new OracleDataAdapter($"select * from PR260_DS3  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(DS3);
                        //CRIT
                        oda = new OracleDataAdapter($"select * from PR260_CRIT  t where isMTR = {isMTR} and SLUCH_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(CRIT);
                    }
                    var PERS = new DataTable();
                    foreach (var sl in GetIDFromDataTable(ZAP, "PERS_ID"))
                    {
                        oda = new OracleDataAdapter($"select * from PR260_PERS  t where isMTR = {isMTR} and PERS_ID in ({string.Join(",", sl)})", connInner);
                        oda.Fill(PERS);
                    }

                    var rez = new SCHET_AND_PERS
                    {
                        sc = CreateFile(schetRow, ZAP, SLUCH, USL, SANK, SL_KOEF, NAPR, B_PROT, B_DIAG, H_CONS, ONK_USLtbl, LEK_PR, DS2, DS3, CRIT),
                        pers = CreateFileL(PERS)
                    };
                    return rez;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в GetSchet: {ex.Message}", ex);
            }
        }

        private void AddDict(Dictionary<decimal, PERS> dictPERS, List<PERS> pers)
        {
            foreach (var p in pers.Where(p => !dictPERS.ContainsKey(p.PERS_ID.Value)))
            {
                dictPERS.Add(p.PERS_ID.Value, p);
            }
        }

        private List<PERS> CreateFileL(DataTable PERStbl)
        {
            return (from DataRow row in PERStbl.Rows select PERS.Get(row)).ToList();
        }

        SCHET CreateFile(DataRow SCHETrow, DataTable ZAPtbl, DataTable SLUCHtbl, DataTable USLtbl, DataTable SANKtbl,
            DataTable KOEFtbl, DataTable NAPRtbl, DataTable B_PROTtbl, DataTable B_DIAGtbl, DataTable H_CONStbl, DataTable ONK_USLtbl, DataTable LEK_PRtbl,
            DataTable DS2, DataTable DS3, DataTable CRIT)
        {
            var step = 0;
            try
            {
                decimal SUMP = 0;
                decimal SANK_IT = 0;
                decimal SUMV = 0;
                decimal SD_Z = 0;

                var sc = SCHET.Get(SCHETrow);
                step = 1;
                foreach (DataRow row_z in ZAPtbl.Rows)
                {
                    var z = ZAP.Get(row_z);
                    z.PACIENT = PACIENT.Get(row_z);
                    z.Z_SL = Z_SL.Get(row_z);
                    SD_Z++;
                    SUMP += z.Z_SL.SUMP ?? 0;
                    SANK_IT += z.Z_SL.SANK_IT ?? 0;
                    SUMV += z.Z_SL.SUMV;

                    sc.ZAP.Add(z);
                    step = 2;
                    if (SANKtbl != null)
                        foreach (var san_row in SANKtbl.Select($"SLUCH_Z_ID = {z.Z_SL.SLUCH_Z_ID}"))
                        {
                            z.Z_SL.SANK.Add(SANK.Get(san_row));
                        }

                    foreach (var sl_row in SLUCHtbl.Select($"SLUCH_Z_ID = {z.Z_SL.SLUCH_Z_ID}"))
                    {
                        var sl = SL.Get(sl_row,
                            DS2.Select($"SLUCH_ID = {sl_row["SLUCH_ID"]}"),
                            DS3.Select($"SLUCH_ID = {sl_row["SLUCH_ID"]}"),
                            CRIT.Select($"SLUCH_ID = {sl_row["SLUCH_ID"]}", "ORD"));
                        z.Z_SL.SL.Add(sl);
                        step = 3;
                        foreach (var usl_row in USLtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            var us = USL.Get(usl_row);
                            sl.USL.Add(us);
                        }

                        step = 4;
                        foreach (var onk_usl_row in ONK_USLtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            var o_us = ONK_USL.Get(onk_usl_row);
                            sl.ONK_SL.ONK_USL.Add(o_us);
                            foreach (var lek_pr_row in LEK_PRtbl.Select($"ONK_USL_ID = {o_us.ONK_USL_ID}"))
                            {
                                var lek = LEK_PR.Get(lek_pr_row);
                                o_us.LEK_PR.Add(lek);

                            }
                        }

                        step = 5;
                        foreach (var napr_row in NAPRtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            sl.NAPR.Add(NAPR.Get(napr_row));
                        }

                        step = 6;
                        foreach (var cons_row in H_CONStbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            sl.CONS.Add(CONS.Get(cons_row));
                        }

                        step = 7;

                        step = 8;

                        step = 9;
                        if (KOEFtbl != null)
                            foreach (var koef_row in KOEFtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.KSG_KPG.SL_KOEF.Add(SL_KOEF.Get(koef_row));
                            }

                        step = 10;
                        if (B_PROTtbl != null)
                            foreach (var prot_row in B_PROTtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.ONK_SL.B_PROT.Add(B_PROT.Get(prot_row));
                            }

                        step = 11;
                        if (B_DIAGtbl != null)
                            foreach (var diag_row in B_DIAGtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.ONK_SL.B_DIAG.Add(B_DIAG.Get(diag_row));
                            }

                        step = 12;
                    }
                }

                sc.SUMMAP = SUMP;
                sc.SANK_MEK = SANK_IT;
                sc.SUMMAV = SUMV;

                return sc;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при формировании класса H:" + ex.Message + "step " + step);
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

    public class ORD260PARAM : INotifyPropertyChanged
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




        private bool _С_FILE;

        public bool С_FILE
        {
            get => _С_FILE;
            set
            {
                _С_FILE = value;
                RaisePropertyChanged();
            }
        }

        public string FILENAME => $"{(С_FILE ? "CT" : "TT")}75_{YEAR.ToString().Substring(2)}{MONTH:D2}{NN}";


        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
    public class SCHET_AND_PERS
    {
        public SCHET sc { get; set; }
        public List<PERS> pers { get; set; }
    }
    public static class Ext
    {
        public static bool In(this int value, params int[] values)
        {
            return values.Contains(value);
        }
    }
}

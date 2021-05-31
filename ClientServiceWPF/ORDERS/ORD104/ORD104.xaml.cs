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
using ClientServiceWPF.ORDERS.ORD15;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using LogType = ClientServiceWPF.Class.LogType;


namespace ClientServiceWPF.ORDERS.ORD104
{
    /// <summary>
    /// Логика взаимодействия для ORD104.xaml
    /// </summary>
    public partial class ORD104 : Window
    {
        public ORD104VM VM { get; set; } = new ORD104VM(Dispatcher.CurrentDispatcher);
        public ORD104()
        {
            VM.PARAM.PERIOD = DateTime.Now.AddMonths(-1);
            VM.PARAM.DATE = DateTime.Now;
            InitializeComponent();
        }
    }


    public class ORD104VM : INotifyPropertyChanged
    {
        private Dispatcher dispatcher;

        public ORD104VM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public ORD104PARAM PARAM { get; } = new ORD104PARAM();

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
                    Progress1.IsIndeterminate = true;
                    Task task;
                    task = PARAM.MR_FILE ? new Task(()=> { GetFileMR(sfd.FileName, PARAM.FILENAME, PARAM.DATE, PARAM.ISP ? PARAM.ISP_NAME : null, PARAM.YEAR, PARAM.MONTH); }) : new Task(() => { GetFileER(sfd.FileName, PARAM.FILENAME, PARAM.DATE, PARAM.ISP ? PARAM.ISP_NAME : null, PARAM.YEAR, PARAM.MONTH); });
                    task.Start();

                    await task;

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
                Progress1.IsIndeterminate = false;
                IsOperationRun = false;
            }
        });
        void GetFileMR(string path, string FILENAME, DateTime DateFile, string ISPName, int Year, int Month)
        {
            try
            {
                AddLogs(LogType.Info, "Создание ZGLV");
                var MR_OB = new MR_OB {ZGLV = {DATA = DateFile, FILENAME = FILENAME, VERSION = "3.0"}, SVD = {CODE = Year * 1000 + Month, YEAR = Year, MONTH = Month}};
                if (!string.IsNullOrEmpty(ISPName))
                    MR_OB.ZGLV.FIRSTNAME = ISPName;

                AddLogs(LogType.Info, "Запрос случаев");
                var tbl = new DataTable();
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from V_EXPORT_104_PR t where t.year = {Year} and t.month = {Month}", conn))
                    {
                        oda.Fill(tbl);
                    }
                }
                var i = 1;
                AddLogs(LogType.Info, "Формирование случаев");
                foreach (DataRow row in tbl.Rows)
                {
                    var z = new ZAP { N_ZAP = i };
                    if (row["DR"] != DBNull.Value)
                    {
                        z.PACIENT.DR = Convert.ToDateTime(row["DR"]);
                    }
                    z.PACIENT.VZRS = Convert.ToDecimal(row["VZRS"]);
                    if (row["AP_TYPE"] != DBNull.Value)
                    {
                        z.SLUCH.AP_TYPE = row["AP_TYPE"].ToString();
                    }
                    if (row["FOR_POM"] != DBNull.Value)
                    {
                        z.SLUCH.FOR_POM = Convert.ToDecimal(row["FOR_POM"]);
                    }
                    z.SLUCH.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                    z.SLUCH.DS1 = Convert.ToString(row["DS1"]);
                    z.SLUCH.RSLT = Convert.ToDecimal(row["RSLT"]);
                    z.SLUCH.USL_OK = row["USL_OK"].ToString();
                    z.SLUCH.isLETAL = Convert.ToBoolean(row["isLETAL"]);
                    z.SLUCH.OT_NAIM = Convert.ToDecimal(row["OT_NAIM"]);
                    i++;
                    MR_OB.PODR.Add(z);
                }
                AddLogs(LogType.Info, "Формирование свода");
                var SVOD = MR_OB.PODR.GroupBy(x => new { x.SLUCH.AP_TYPE, x.SLUCH.FOR_POM, x.SLUCH.USL_OK }, x => x);

                var N_SV = 1;
                foreach (var t in SVOD)
                {
                    var it = new IT_SV();
                    MR_OB.OB_SV.Add(it);
                    it.AP_TYPE = t.Key.AP_TYPE;
                    it.FOR_POM = t.Key.FOR_POM;
                    it.USL_OK = Convert.ToDecimal(t.Key.USL_OK);
                    it.N_SV = N_SV;
                    N_SV++;

                    var C_0 = t.Where(x => x.PACIENT.VZRS < 18).ToList();
                    var C_18 = t.Where(x => x.PACIENT.VZRS >= 18 && x.PACIENT.VZRS < 60).ToList();
                    var C_60 = t.Where(x => x.PACIENT.VZRS >= 60).ToList();

                    var NOZ = new[] { 0, 1, 2, 3, 4, 5, 6, 7 };


                    foreach (var NOZitem in NOZ)
                    {
                        if (C_0.Count != 0 && NOZitem.In(0, 4, 6, 7))
                        {
                            var v_it = new VZS_IT
                            {
                                VZST = 0,
                                OT_NAIM = NOZitem,
                                ZBL_IT = C_0.Count(x => (x.SLUCH.OT_NAIM == NOZitem || NOZitem == 0) && !x.SLUCH.isLETAL),
                                SMR_IT = C_0.Count(x => (x.SLUCH.OT_NAIM == NOZitem || NOZitem == 0) && x.SLUCH.isLETAL)
                            };
                            it.VZS_IT.Add(v_it);
                        }

                        if (C_18.Count != 0 && NOZitem.In(0, 1, 2, 3, 4, 5))
                        {
                            var v_it = new VZS_IT
                            {
                                VZST = 1,
                                OT_NAIM = NOZitem,
                                ZBL_IT = C_18.Count(x => (x.SLUCH.OT_NAIM == NOZitem || NOZitem == 0) && !x.SLUCH.isLETAL),
                                SMR_IT = C_18.Count(x => (x.SLUCH.OT_NAIM == NOZitem || NOZitem == 0) && x.SLUCH.isLETAL)
                            };
                            it.VZS_IT.Add(v_it);
                        }

                        if (C_60.Count != 0 && NOZitem.In(0, 1, 2, 3, 4, 5))
                        {
                            var v_it = new VZS_IT
                            {
                                VZST = 2,
                                OT_NAIM = NOZitem,
                                ZBL_IT = C_60.Count(x => (x.SLUCH.OT_NAIM == NOZitem || NOZitem == 0) && !x.SLUCH.isLETAL),
                                SMR_IT = C_60.Count(x => (x.SLUCH.OT_NAIM == NOZitem || NOZitem == 0) && x.SLUCH.isLETAL)
                            };
                            it.VZS_IT.Add(v_it);
                        }
                    }
                    it.VZS_IT = it.VZS_IT.OrderBy(x => x.VZST).ThenBy(x => x.OT_NAIM).ToList();
                }
                AddLogs(LogType.Info, "Сохранение файла");
                using (Stream st = File.Create(path))
                {
                    MR_OB.WriteXml(st);
                    st.Close();
                }
                AddLogs(LogType.Info, "Завершено");
            }
            catch (Exception ex)
            {
                AddLogs(LogType.Error, ex.FullMessage());
                MessageBox.Show(ex.Message);
            }
        }


        void GetFileER(string path, string FILENAME, DateTime DateFile, string ISPName, int Year, int Month)
        {
            try
            {
               AddLogs(LogType.Info, "Создание ZGLV");
               var MR_OB = new ER_OB {ZGLV = {DATA = DateFile, FILENAME = FILENAME, VERSION = "3.0" }, SVD = {CODE = Year * 1000 + Month, YEAR = Year, MONTH = Month}};
                if (!string.IsNullOrEmpty(ISPName))
                    MR_OB.ZGLV.FIRSTNAME = ISPName;
                AddLogs(LogType.Info, "Запрос случаев");
                var tbl_SL = new DataTable();
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    using (var oda = new OracleDataAdapter($"select * from V_EXPORT_104_PR_EKMP_SL t where t.year = {Year} and t.month = {Month}", conn))
                    {
                        oda.Fill(tbl_SL);
                    }
                }
                var i = 1;
                AddLogs(LogType.Info, "Формирование случаев");
                foreach (DataRow row in tbl_SL.Rows)
                {
                    var z = new ZAP { N_ZAP = i };
                    if (row["DR"] != DBNull.Value)
                    {
                        z.PACIENT.DR = Convert.ToDateTime(row["DR"]);
                    }
                    z.PACIENT.VZRS = Convert.ToDecimal(row["VZRS"]);
                    if (row["AP_TYPE"] != DBNull.Value)
                    {
                        z.SLUCH.AP_TYPE = row["AP_TYPE"].ToString();
                    }
                    if (row["FOR_POM"] != DBNull.Value)
                    {
                        z.SLUCH.FOR_POM = Convert.ToDecimal(row["FOR_POM"]);
                    }
                    z.SLUCH.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                    z.SLUCH.DS1 = Convert.ToString(row["DS1"]);
                    z.SLUCH.RSLT = Convert.ToDecimal(row["RSLT"]);

                    z.SLUCH.SLUCH_ID = Convert.ToInt32(row["SLUCH_ID"]);
                    z.SLUCH.SLUCH_Z_ID = Convert.ToInt32(row["SLUCH_Z_ID"]);

                    z.NO_EKMP = null;
                    z.EKMP = new EKMP { TYPE = Convert.ToInt32(row["TYPE"]) };
                    z.EKMP.setPROBLEM(row["PROBLEM"].ToString());
                    i++;
                    MR_OB.PODR.Add(z);
                }
                AddLogs(LogType.Info, "Сохранение файла");
                using (Stream st = File.Create(path))
                {
                    MR_OB.WriteXml(st);
                    st.Close();
                }
                AddLogs(LogType.Info, "Завершено");
            }
            catch (Exception ex)
            {
                AddLogs(LogType.Error, ex.FullMessage());
                MessageBox.Show(ex.Message);
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

    public class ORD104PARAM : INotifyPropertyChanged
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

        private bool _MR_FILE;

        public bool MR_FILE
        {
            get => _MR_FILE;
            set
            {
                _MR_FILE = value;
                RaisePropertyChanged();
            }
        }

        public string FILENAME => $"{(MR_FILE ? "MR" : "ER")}{(ISP ? "S" : "")}75{YEAR.ToString().Substring(2)}{NN:D4}";

 

    #region INotifyPropertyChanged
public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public static class Ext
    {
        public static bool In(this int value, params int[] values)
        {
            return values.Contains(value);
        }
    }
}

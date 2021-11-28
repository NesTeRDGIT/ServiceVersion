using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData.Annotations;
using DataTable = System.Data.DataTable;
using Path = System.IO.Path;
using Window = System.Windows.Window;

namespace ClientServiceWPF.MEK_RESULT.VOLUM_CONTROL
{
    /// <summary>
    /// Логика взаимодействия для VOLUM_CONTROL.xaml
    /// </summary>
    public partial class VOLUM_CONTROL : Window,INotifyPropertyChanged
    {
        public ControlProcedureVM ControlProcedureVM { get; } = new ControlProcedureVM();
        public ResultControl ResultControlVM { get; }

        public VOLUM_CONTROL()
        {
            ResultControlVM  = new ResultControl(new RepositoryVolumeControl(), new VCExcelManager(), SaveParam);
            InitializeComponent();

            CVSLIMITs = (CollectionViewSource)FindResource("CVSLIMITs");
            CVSLIMMO = (CollectionViewSource)FindResource("CVSLIMMO");
            CVSLIMSMO = (CollectionViewSource)FindResource("CVSLIMSMO");
            CVSLIMRUB = (CollectionViewSource)FindResource("CVSLIMRUB");
            DefaultValue();
        }


        public LIMIT_DATA LIM_DATA { get; set; } = new LIMIT_DATA();
     
        public class LIMIT_DATA
        {
            public List<LIMITRow> LIST { get; set; } = new List<LIMITRow>();
            public List<MO_SPRRow> MO_SPR { get; set; } = new List<MO_SPRRow>();
            public List<SMO_SPRRow> SMO_SPR { get; set; } = new List<SMO_SPRRow>();
            public List<RUBRIC_SPRRow> RUBRIC_SPR { get; set; } = new List<RUBRIC_SPRRow>();
            public void CreateSPR()
            {

                var DIC_MO = new Dictionary<string, string>();
                foreach (var row in LIST)
                {
                    if (!DIC_MO.ContainsKey(row.CODE_MO))
                        DIC_MO.Add(row.CODE_MO, row.NAM_MOK);
                }
                var DIC_SMO = new Dictionary<string, string>();
                foreach (var row in LIST)
                {
                    if (!DIC_SMO.ContainsKey(row.SMO))
                        DIC_SMO.Add(row.SMO, row.NAM_SMOK);
                }
                var DIC_RUB = new Dictionary<string, string>();
                foreach (var row in LIST)
                {
                    if (!DIC_RUB.ContainsKey(row.VOLUM_RUBRIC_ID))
                        DIC_RUB.Add(row.VOLUM_RUBRIC_ID, row.NAME_RUB);
                }
                MO_SPR.Clear();
                MO_SPR.Insert(0, new MO_SPRRow());
                MO_SPR.AddRange(DIC_MO.Select(x => new MO_SPRRow { CODE_MO = x.Key, NAM_OK = x.Value }).OrderBy(x => x.CODE_MO));

                SMO_SPR.Clear();
                SMO_SPR.Insert(0, new SMO_SPRRow());
                SMO_SPR.AddRange(DIC_SMO.Select(x => new SMO_SPRRow { SMOCOD = x.Key, NAM_SMOK = x.Value }).OrderBy(x => x.SMOCOD));

                RUBRIC_SPR.Clear();
                RUBRIC_SPR.Insert(0, new RUBRIC_SPRRow());
                RUBRIC_SPR.AddRange(DIC_RUB.Select(x => new RUBRIC_SPRRow { VOLUM_RUBRIC_ID = x.Key, NAME = x.Value }).OrderBy(x => x.VOLUM_RUBRIC_ID));
            }
        }
      






      
        private CollectionViewSource CVSLIMITs;
        private CollectionViewSource CVSLIMMO;
        private CollectionViewSource CVSLIMSMO;
        private CollectionViewSource CVSLIMRUB;





          

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

   

        public void DefaultValue()
        {
            ResultControlVM.Param.IsTemp1 = true;
            ResultControlVM.Param.Period = DateTime.Now.AddMonths(-1);
            ResultControlVM.Param.DOLG = Properties.Settings.Default.PRIL5_DOLG;
            ResultControlVM.Param.FIO = Properties.Settings.Default.PRIL5_FIO;
        }

        void SaveParam()
        {
            Properties.Settings.Default.PRIL5_DOLG = ResultControlVM.Param.DOLG;
            Properties.Settings.Default.PRIL5_FIO = ResultControlVM.Param.FIO;
            Properties.Settings.Default.Save();
        }

        class LimitsParam
        {
            public int YEAR { get; set; }
            public int MONTH { get; set; }
        }
        private void buttonRefreshLimit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DatePickerLimit.SelectedDate.HasValue)
                {
                    var par = new LimitsParam {YEAR = DatePickerLimit.SelectedDate.Value.Year, MONTH = DatePickerLimit.SelectedDate.Value.Month};
                    var th = new Thread(GetLimits) {IsBackground = true};
                    th.Start(par);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetLimits(object obj)
        {
            try
            {
                Dispatcher.Invoke(() => { ProgressBarLimitRefresh.IsIndeterminate = true; });
                var param = (LimitsParam) obj;
                var oda = new OracleDataAdapter($"select * from table(volum_control.GET_LIMITS({param.YEAR},{param.MONTH}))", AppConfig.Property.ConnectionString);
                var tbl = new DataTable();
                oda.Fill(tbl);
                LIM_DATA.LIST.Clear();
                LIM_DATA.LIST.AddRange(LIMITRow.Get(tbl.Select()));
                LIM_DATA.CreateSPR();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    ProgressBarLimitRefresh.IsIndeterminate = false;
                    CVSLIMITs.View.Refresh();
                    CVSLIMMO.View.Refresh();
                    CVSLIMSMO.View.Refresh();
                    CVSLIMRUB.View.Refresh();
                });
            }
          
        }

        private void buttonLIM_FILTER_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fp = ReadFilterParamLim();
                var th = new Thread(FiltringListLim) { IsBackground = true };
                th.Start(fp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public class FilterParamLim
        {
            public string CODE_MO { get; set; }
            public string SMO { get; set; }
            public string RUB { get; set; }
            public bool? IsErr { get; set; }
        }

        private FilterParamLim ReadFilterParamLim()
        {
            var fp = new FilterParamLim {CODE_MO = (comboBoxLIM_MO.SelectedItem as MO_SPRRow)?.CODE_MO, SMO = (comboBoxLIM_SMO.SelectedItem as SMO_SPRRow)?.SMOCOD, RUB = (comboBoxLIM_RUB.SelectedItem as RUBRIC_SPRRow)?.VOLUM_RUBRIC_ID, IsErr = checkBoxLIM_ERR.IsChecked};
            return fp;
        }

        private void FiltringListLim(object obj)
        {
            var fp = (FilterParamLim)obj;
            Dispatcher.Invoke(() =>
            {
                ProgressBarLimits.IsIndeterminate = true;
                LabelLimits.Content = "Фильтрация...";
            });
            foreach (var item in LIM_DATA.LIST)
            {
                item.IsShow = true;
                if (fp.IsErr.HasValue)
                {
                    item.IsShow &= fp.IsErr.Value == item.IsErr;
                }
                if (!string.IsNullOrEmpty(fp.CODE_MO))
                {
                    item.IsShow &= fp.CODE_MO == item.CODE_MO;
                }
                if (!string.IsNullOrEmpty(fp.SMO))
                {
                    item.IsShow &= fp.SMO == item.SMO;
                }
                if (!string.IsNullOrEmpty(fp.RUB))
                {
                    item.IsShow &= fp.RUB == item.VOLUM_RUBRIC_ID;
                }
            }

            Dispatcher.Invoke(() =>
            {
                ProgressBarLimits.IsIndeterminate = false;
                LabelLimits.Content = "";
                CVSLIMITs.View.Refresh();
            });
        }


        private void CVSLIMITs_OnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = ((LIMITRow)e.Item).IsShow;
        }

        public decimal? SUM_SELECT_LIM
        {
            get
            {
                decimal result = 0;
                foreach (var cell in DataGridLimit.SelectedCells)
                {
                    var type = cell.Item.GetType().GetProperty(cell.Column.SortMemberPath);
                    if (type != null)
                    {
                        var obj = type.GetValue(cell.Item);
                        if (type.PropertyType == typeof(int))
                            result += (int)obj;
                        if (type.PropertyType == typeof(decimal))
                            result += (decimal)obj;
                    }
                }
                return result;
            }
        }

   

        private void DataGridLimit_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            RaisePropertyChanged("SUM_SELECT_LIM");
        }

        private void buttonRefreshVOLUM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var th = new Thread(UpdateVolumeParam) {IsBackground = true};
                th.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public class VOLUME_PARAM
        {
            public int? YEAR { get; set; }
            public int? MONTH { get; set; }
            public DateTime? DT => YEAR.HasValue && MONTH.HasValue ? new DateTime(YEAR.Value, MONTH.Value, 1) : (DateTime?)null;
            public int? CountSANK { get; set; }
            public bool? IsVOLUME => CountSANK.HasValue? CountSANK != 0 : (bool?)null;
            public bool? IsSyncMainBD { get; set; }
            public DateTime? DT_ACT { get; set; }
        }

       public VOLUME_PARAM VP { get; set; }= new VOLUME_PARAM();

        void UpdateVolumeParam()
        {
            try
            {
                this.Dispatcher.Invoke(() => { ProgressBarVOLUMRefresh.IsIndeterminate = true; });

                using (var CONN = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    this.Dispatcher.Invoke(() => { LabelProgressBarVOLUMRefresh.Content = "Запрос отчетного периода"; });
                    CONN.Open();
                    using (var cmd = new OracleCommand("select VOLUM_CONTROL.SchetDT() from dual", CONN))
                    {
                        var t = Convert.ToDateTime(cmd.ExecuteScalar());
                        VP.YEAR = t.Year;
                        VP.MONTH = t.Month;
                    }
                    this.Dispatcher.Invoke(() => { LabelProgressBarVOLUMRefresh.Content = "Запрос наличия санкций"; });
                    using (var cmd = new OracleCommand("select VOLUM_CONTROL.CountSANK() from dual", CONN))
                    {
                        VP.CountSANK = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    this.Dispatcher.Invoke(() => { LabelProgressBarVOLUMRefresh.Content = "Запрос синхронизации БД"; });
                    using (var cmd = new OracleCommand("select volum_control.IsSyncMainBD from dual", CONN))
                    {
                        VP.IsSyncMainBD = Convert.ToBoolean(cmd.ExecuteScalar());
                    }
                    this.Dispatcher.Invoke(() => { LabelProgressBarVOLUMRefresh.Content = "Запрос даты акта МЭК"; });
                    using (var cmd = new OracleCommand("select VOLUM_CONTROL.ACT_DT() from dual", CONN))
                    {
                        var obj = cmd.ExecuteScalar();
                        if (obj != null && obj != DBNull.Value)
                        {
                            VP.DT_ACT = Convert.ToDateTime(cmd.ExecuteScalar());
                        }
                       
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                this.Dispatcher.Invoke(() =>
                {
                    ProgressBarVOLUMRefresh.IsIndeterminate = false;
                    LabelProgressBarVOLUMRefresh.Content = "";
                    RaisePropertyChanged("VP");
                });
            }
        }

        private Thread thVolumeCheck;

        private void ButtonVolumeCheck_Click(object sender, RoutedEventArgs e)
        {
            if (VP.IsVOLUME.HasValue)
            {
                if (thVolumeCheck?.IsAlive==true)
                {
                    MessageBox.Show("Контроль уже выполняется. Дождитесь завершения процесса");
                    return;
                }
                if (VP.IsVOLUME == true)
                {
                    if(MessageBox.Show("Контроль уже проведен. Вы уверены что хотите перепровести контроль?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;
                }

                thVolumeCheck = new Thread(VolumeCheck) {IsBackground = true};
                thVolumeCheck.Start();
            }
        }


        void VolumeCheck()
        {
            try
            {
                this.Dispatcher.Invoke(() => { ProgressBarVOLUMRefresh.IsIndeterminate = true; });
                using (var CONN = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    this.Dispatcher.Invoke(() => { LabelProgressBarVOLUMRefresh.Content = "Проведение контроля объемов"; });
                    CONN.Open();
                    using (var cmd = new OracleCommand("begin VOLUM_CONTROL.VolumeCheck(); end;", CONN))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                this.Dispatcher.Invoke(() =>
                {
                    ProgressBarVOLUMRefresh.IsIndeterminate = false;
                    LabelProgressBarVOLUMRefresh.Content = "";
                    buttonRefreshVOLUM_Click(buttonRefreshVOLUM, new RoutedEventArgs());
                });
            }
        }

        private Thread thSyncMainBD;
        private void ButtonSyncMainBD_Click(object sender, RoutedEventArgs e)
        {
            if (VP.IsSyncMainBD.HasValue)
            {
                if (thSyncMainBD?.IsAlive == true)
                {
                    MessageBox.Show("Контроль уже выполняется. Дождитесь завершения процесса");
                    return;
                }
                if (VP.IsSyncMainBD == true)
                {
                    if (MessageBox.Show("БД уже синхронизирована. Вы уверены что хотите провести синхронизацию повторно?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;
                }

                thSyncMainBD = new Thread(SyncMainBD) { IsBackground = true };
                thSyncMainBD.Start();
            }
        }

        void SyncMainBD()
        {
            try
            {
                this.Dispatcher.Invoke(() => { ProgressBarVOLUMRefresh.IsIndeterminate = true; });
                using (var CONN = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    this.Dispatcher.Invoke(() => { LabelProgressBarVOLUMRefresh.Content = "Синхронизация БД"; });
                    CONN.Open();
                    using (var cmd = new OracleCommand("begin VOLUM_CONTROL.SyncMainBD(); end;", CONN))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                this.Dispatcher.Invoke(() =>
                {
                    ProgressBarVOLUMRefresh.IsIndeterminate = false;
                    LabelProgressBarVOLUMRefresh.Content = "";
                    buttonRefreshVOLUM_Click(buttonRefreshVOLUM, new RoutedEventArgs());
                });
            }
        }

        private Thread ThDATE_ACT;
        private void ButtonSetActDate_Click(object sender, RoutedEventArgs e)
        {
            if (DatePickerDT_ACT.SelectedDate.HasValue)
            {
                if (ThDATE_ACT?.IsAlive == true)
                {
                    MessageBox.Show("Процесс уже выполняется. Дождитесь завершения процесса");
                    return;
                }
                ThDATE_ACT = new Thread(SetActDT) { IsBackground = true };
                ThDATE_ACT.Start(DatePickerDT_ACT.SelectedDate.Value);
            }
            
        }

        void SetActDT(object obj)
        {
            try
            {
                var dt = (DateTime) obj;
                this.Dispatcher.Invoke(() => { ProgressBarVOLUMRefresh.IsIndeterminate = true; });
                using (var CONN = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    this.Dispatcher.Invoke(() => { LabelProgressBarVOLUMRefresh.Content = "Установка даты акта"; });
                    CONN.Open();
                    using (var cmd = new OracleCommand($"begin VOLUM_CONTROL.SetActDT('{dt:dd.MM.yyyy}'); end;", CONN))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                this.Dispatcher.Invoke(() =>
                {
                    ProgressBarVOLUMRefresh.IsIndeterminate = false;
                    LabelProgressBarVOLUMRefresh.Content = "";
                    buttonRefreshVOLUM_Click(buttonRefreshVOLUM, new RoutedEventArgs());
                });
            }
        }

      
    }
    public class VOLUME_CONTROLRow
    {
        public static List<VOLUME_CONTROLRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static VOLUME_CONTROLRow Get(DataRow row)
        {
            try
            {
                var item = new VOLUME_CONTROLRow();
                item.MEK_SUM = Convert.ToBoolean(row["MEK_SUM"]);
                item.SUM_ITOG = Convert.ToDecimal(row["SUM_ITOG"]);
                item.MEK_KOL = Convert.ToBoolean(row["MEK_KOL"]);
                item.KOL_ITOG = Convert.ToDecimal(row["KOL_ITOG"]);
                item.FOND = Convert.ToDecimal(row["FOND"]);
                item.MUR = Convert.ToDecimal(row["MUR"]);
                item.FOND_SCOR = Convert.ToDecimal(row["FOND_SCOR"]);
                item.KOL_LIMIT = Convert.ToDecimal(row["KOL_LIMIT"]);
                item.SUM_LIMIT = Convert.ToDecimal(row["SUM_LIMIT"]);
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.NAM_MOK = Convert.ToString(row["NAM_MOK"]);
                item.SMO = Convert.ToString(row["SMO"]);
                item.NAM_SMOK = Convert.ToString(row["NAM_SMOK"]);
                item.SLUCH_Z_ID = Convert.ToInt32(row["SLUCH_Z_ID"]);
                item.RUBRIC_ID = Convert.ToString(row["RUBRIC_ID"]);
                item.NAME_RUB = Convert.ToString(row["NAME_RUB"]);
                item.RUBRIC_ID_NEW = Convert.ToString(row["RUBRIC_ID_NEW"]);
                item.NAME_RUB_NEW = Convert.ToString(row["NAME_RUB_NEW"]);
                item.KOL = Convert.ToDecimal(row["KOL"]);
                item.SUM = Convert.ToDecimal(row["SUM"]);
                item.DATE_Z_1 = Convert.ToDateTime(row["DATE_Z_1"]);
                item.DATE_Z_2 = Convert.ToDateTime(row["DATE_Z_2"]);
                item.IDCASE = Convert.ToString(row["IDCASE"]);
                return item;
            }
            catch (Exception e)
            {
                
                throw new Exception($"Ошибка получения VOLUME_CONTROL: {e.Message}");
            }
        }


        public bool IsSHOW { get; set; } = true;
        public bool MEK_SUM { get; set; }
        public decimal SUM_ITOG { get; set; }
        public bool MEK_KOL { get; set; }
        public decimal KOL_ITOG { get; set; }
        public decimal FOND { get; set; }
        public decimal MUR { get; set; }
        public decimal FOND_SCOR { get; set; }
        public decimal KOL_LIMIT { get; set; }
        public decimal SUM_LIMIT { get; set; }
        public string CODE_MO { get; set; }
        public string NAM_MOK { get; set; }

        public string SMO { get; set; }
        public string NAM_SMOK { get; set; }
        public int SLUCH_Z_ID { get; set; }
        public  string RUBRIC_ID { get; set; }
        public string NAME_RUB { get; set; }
        public string RUBRIC_ID_NEW { get; set; }
        public string NAME_RUB_NEW { get; set; }
        public decimal KOL { get; set; }
        public decimal SUM { get; set; }
        public  DateTime DATE_Z_1 { get; set; }
        public DateTime DATE_Z_2 { get; set; }
        public  string IDCASE { get; set; }


    }
    public class MO_SPRRow
    {
        public static List<MO_SPRRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static MO_SPRRow Get(DataRow row)
        {
            try
            {
                var item = new MO_SPRRow();
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.NAM_OK = Convert.ToString(row["NAM_OK"]);
                return item;
            }
            catch (Exception e)
            {

                throw new Exception($"Ошибка получения MO_SPR: {e.Message}");
            }
        }

        public string CODE_MO { get; set; }
        public string NAM_OK { get; set; }

        public string FULL_NAME => $"{CODE_MO} {NAM_OK}";

    }
    public class SMO_SPRRow
    {
        public static List<SMO_SPRRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static SMO_SPRRow Get(DataRow row)
        {
            try
            {
                var item = new SMO_SPRRow();
                item.SMOCOD = Convert.ToString(row["SMOCOD"]);
                item.NAM_SMOK = Convert.ToString(row["NAM_SMOK"]);
                return item;
            }
            catch (Exception e)
            {

                throw new Exception($"Ошибка получения SMO_SPR: {e.Message}");
            }
        }

        public string SMOCOD { get; set; }
        public string NAM_SMOK { get; set; }
        public string FULL_NAME => $"{SMOCOD} {NAM_SMOK}";
    }
    public class RUBRIC_SPRRow
    {
        public static List<RUBRIC_SPRRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static RUBRIC_SPRRow Get(DataRow row)
        {
            try
            {
                var item = new RUBRIC_SPRRow();
                item.VOLUM_RUBRIC_ID = Convert.ToString(row["VOLUM_RUBRIC_ID"]);
                item.NAME = Convert.ToString(row["NAME"]);
                return item;
            }
            catch (Exception e)
            {

                throw new Exception($"Ошибка получения SMO_SPR: {e.Message}");
            }
        }
        public string VOLUM_RUBRIC_ID { get; set; }
        public string NAME { get; set; }

        public string FULL_NAME => $"{VOLUM_RUBRIC_ID} {NAME}";
    }
    public class LIMITRow
    {
        public static List<LIMITRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }

        public static LIMITRow Get(DataRow row)
        {
            try
            {
                var item = new LIMITRow();
                item.YEAR = Convert.ToInt32(row["YEAR"]);
                item.MONTH = Convert.ToInt32(row["MONTH"]);
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.NAM_MOK = Convert.ToString(row["NAM_MOK"]);
                item.SMO = Convert.ToString(row["SMO"]);
                item.NAM_SMOK = Convert.ToString(row["NAM_SMOK"]);
                item.VOLUM_RUBRIC_ID = Convert.ToString(row["VOLUM_RUBRIC_ID"]);
                item.NAME_RUB = Convert.ToString(row["NAME_RUB"]);
                item.KOL_ISP = Convert.ToDecimal(row["KOL_ISP"]);
                item.SUM_ISP = Convert.ToDecimal(row["SUM_ISP"]);
                item.KOL_M = Convert.ToDecimal(row["KOL_M"]);
                item.SUM_M = Convert.ToDecimal(row["SUM_M"]);
                item.KOL_Q1 = Convert.ToDecimal(row["KOL_Q1"]);
                item.SUM_Q1 = Convert.ToDecimal(row["SUM_Q1"]);
                item.KOL_Q2 = Convert.ToDecimal(row["KOL_Q2"]);
                item.SUM_Q2 = Convert.ToDecimal(row["SUM_Q2"]);
                item.KOL_Q3 = Convert.ToDecimal(row["KOL_Q3"]);
                item.SUM_Q3 = Convert.ToDecimal(row["SUM_Q3"]);
                item.KOL_Q4 = Convert.ToDecimal(row["KOL_Q4"]);
                item.SUM_Q4 = Convert.ToDecimal(row["SUM_Q4"]);
                item.KOL_Y = Convert.ToDecimal(row["KOL_Y"]);
                item.SUM_Y = Convert.ToDecimal(row["SUM_Y"]);
                item.IsISP = Convert.ToBoolean(row["IsISP"]);
                item.IsYEAR = Convert.ToBoolean(row["IsYEAR"]);
                item.IsQ = Convert.ToBoolean(row["IsQ"]);
                item.IsLast = Convert.ToBoolean(row["IsLast"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения LIMITRow: {e.Message}");
            }
        }

        public bool IsShow { get; set; } = true;
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string CODE_MO { get; set; }
        public string NAM_MOK { get; set; }
        public string SMO { get; set; }
        public string NAM_SMOK { get; set; }
        public string VOLUM_RUBRIC_ID { get; set; }
        public string NAME_RUB { get; set; }

        public decimal KOL_ISP { get; set; }
        public decimal SUM_ISP { get; set; }

        public decimal KOL_M { get; set; }
        public decimal SUM_M { get; set; }

        public decimal KOL_Q1 { get; set; }
        public decimal SUM_Q1 { get; set; }

        public decimal KOL_Q2 { get; set; }
        public decimal SUM_Q2 { get; set; }

        public decimal KOL_Q3 { get; set; }
        public decimal SUM_Q3 { get; set; }

        public decimal KOL_Q4 { get; set; }
        public decimal SUM_Q4 { get; set; }

        public decimal KOL_Y { get; set; }
        public decimal SUM_Y { get; set; }

        public bool IsYEAR { get; set; }
        public bool IsQ { get; set; }
        public bool IsISP { get; set; }

        public bool IsLast { get; set; }


        public string VID_STR => IsYEAR ? "Годовой" : "Квартальный";

        public string ErrComment
        {
            get
            {
                var Err = new List<string>();
                if (NotVID)
                    Err.Add("2 вида контроля(годовой и квартальный)");
                if (IsLess0)
                    Err.Add("Числа меньше нуля");
                if (IsErrKV_KOL_Y)
                    Err.Add("Сумма квартальных лимитов(кол-во) не равна годовой сумме");
                if (IsErrKV_SUM_Y)
                    Err.Add("Сумма квартальных лимитов(сумма) не равна годовой сумме");
                if (IsKOL_Merr)
                    Err.Add("Месячный лимит(кол-во) превышает квартальный или годовой");
                if (IsSUM_Merr)
                    Err.Add("Месячный лимит(сумма) превышает квартальный лимит или годовой");
                if (IsISP_KOLerr)
                    Err.Add("Исполнение(кол-во) превышает квартальный лимит или годовой");
                if (IsISP_SUMerr)
                    Err.Add("Исполнение(сумма) превышает квартальный лимит или годовой");
                if (IsISP_LIM_SUMerr)
                    Err.Add("Лимит + исполнено (Сумма) не равно квартальному лимиту или годовому");
                if (IsISP_LIM_KOLerr)
                    Err.Add("Лимит + исполнено (Кол-во) не равно квартальному лимиту или годовому");

                return string.Join(Environment.NewLine, Err);
            }
        }


       
        public bool IsErr => IsErrKV_KOL_Y || IsErrKV_SUM_Y || IsKOL_Merr || IsSUM_Merr || IsLess0 || NotVID || IsISP_KOLerr || IsISP_SUMerr || IsISP_LIM_SUMerr || IsISP_LIM_KOLerr;

        /// <summary>
        /// 2 вида контроля(годовой и квартальный)
        /// </summary>
        public bool NotVID => IsYEAR == IsQ;

        /// <summary>
        /// Числа меньше нуля
        /// </summary>
        public bool IsLess0 => KOL_ISP < 0 || SUM_ISP < 0 || KOL_M < 0 || SUM_M < 0 || KOL_Q1 < 0 || SUM_Q1 < 0 || KOL_Q2 < 0 || SUM_Q2 < 0 || KOL_Q3 < 0 || SUM_Q3 < 0 || KOL_Q4 < 0 || SUM_Q4 < 0 || KOL_Y < 0 || SUM_Y < 0;

        /// <summary>
        /// Сумма квартальных лимитов(кол-во) не равна годовой сумме
        /// </summary>
        public bool IsErrKV_KOL_Y => Math.Round(KOL_Q1 + KOL_Q2 + KOL_Q3 + KOL_Q4, 2) != KOL_Y;

        /// <summary>
        /// Сумма квартальных лимитов(сумма) не равна годовой сумме
        /// </summary>
        public bool IsErrKV_SUM_Y => Math.Round(SUM_Q1 + SUM_Q2 + SUM_Q3 + SUM_Q4, 2) != SUM_Y;

        /// <summary>
        /// Месячный лимит(кол-во) превышает квартальный или годовой
        /// </summary>
        public bool IsKOL_Merr
        {
            get
            {
                if (IsQ && KOL_M > GetKV_KOL(MONTH))
                {
                    return true;
                }

                if (IsYEAR && KOL_M > KOL_Y && !IsISP) return true;
                return false;
            }
        }

        /// <summary>
        /// Месячный лимит(сумма) превышает квартальный лимит или годовой
        /// </summary>
        public bool IsSUM_Merr
        {
            get
            {
                if (IsQ && SUM_M > GetKV_SUM(MONTH))
                {
                    return true;
                }

                if (IsYEAR && SUM_M > SUM_Y && !IsISP) return true;
                return false;
            }
        }

        /// <summary>
        /// Исполнение(кол-во) превышает квартальный лимит или годовой
        /// </summary>
        public bool IsISP_KOLerr
        {
            get
            {
                if (IsQ && KOL_ISP > GetKV_KOL(MONTH))
                {
                    return true;
                }

                if (IsYEAR && KOL_ISP > KOL_Y) return true;
                return false;
            }
        }

        /// <summary>
        /// Исполнение(сумма) превышает квартальный лимит или годовой
        /// </summary>
        public bool IsISP_SUMerr
        {
            get
            {
                if (IsQ && SUM_ISP > GetKV_SUM(MONTH))
                {
                    return true;
                }

                if (IsYEAR && SUM_ISP > SUM_Y) return true;
                return false;
            }
        }

        /// <summary>
        /// Лимит + исполнено (сумма) не равно квартальному лимиту или годовому
        /// </summary>
        /// <returns></returns>
        public bool IsISP_LIM_SUMerr
        {
            get
            {
                if (IsLast)
                {
                    if (IsQ && Math.Round((GetKV_SUM(MONTH) - SUM_ISP) / Delimetr, 2, MidpointRounding.AwayFromZero) != SUM_M)
                    {
                        return true;
                    }

                    if (IsYEAR && decimal.Round(SUM_Y - SUM_ISP, 2, MidpointRounding.AwayFromZero) != SUM_M) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Лимит + исполнено (Кол-во) не равно квартальному лимиту или годовому
        /// </summary>
        /// <returns></returns>
        public bool IsISP_LIM_KOLerr
        {
            get
            {
                if(IsLast)
                {
                    if (IsQ && Math.Round((GetKV_KOL(MONTH) - KOL_ISP) / Delimetr, 0, MidpointRounding.AwayFromZero) != KOL_M)
                    {
                        return true;
                    }

                    if (IsYEAR && Math.Round(KOL_Y - KOL_ISP, 0, MidpointRounding.AwayFromZero) != KOL_M) return true;
                }
                return false;
            }
        }

        private decimal GetKV_KOL(int Month)
        {
            switch (Month)
            {
                case 1:
                case 2:
                case 3: return KOL_Q1;

                case 4:
                case 5:
                case 6: return KOL_Q1+KOL_Q2;

                case 7:
                case 8:
                case 9: return KOL_Q1 + KOL_Q2+KOL_Q3;

                case 10:
                case 11:
                case 12: return KOL_Q1 + KOL_Q2 + KOL_Q3+KOL_Q4;
                default: return 0;
            }

        }

        private decimal GetKV_SUM(int Month)
        {
            switch (Month)
            {
                case 1:
                case 2:
                case 3: return SUM_Q1;

                case 4:
                case 5:
                case 6: return SUM_Q1+SUM_Q2;

                case 7:
                case 8:
                case 9: return SUM_Q1 + SUM_Q2+SUM_Q3;

                case 10:
                case 11:
                case 12: return SUM_Q1 + SUM_Q2 + SUM_Q3 + SUM_Q4;
                default: return 0;
            }

        }

        private decimal Delimetr
        {
            get
            {
                switch (MONTH)
                {
                    case 1:
                    case 4:
                    case 7:
                    case 10:
                        return 3;
                    case 2:
                    case 5:
                    case 8:
                    case 11:
                        return 2;
                    case 3:
                    case 6:
                    case 9:
                    case 12:
                        return 1;
                    default: return 0;
                }
            }
        }
    }
    public class LIMIT_RESULTRow
    {
        public static List<LIMIT_RESULTRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static LIMIT_RESULTRow Get(DataRow row)
        {
            try
            {
                var item = new LIMIT_RESULTRow();
                item.YEAR = Convert.ToInt32(row[nameof(YEAR)]);
                item.MONTH = Convert.ToInt32(row[nameof(MONTH)]);
                item.CODE_MO = Convert.ToString(row[nameof(CODE_MO)]);
                item.NAM_MOK = Convert.ToString(row[nameof(NAM_MOK)]);
                item.SMO = Convert.ToString(row[nameof(SMO)]);
                item.NAM_SMOK = Convert.ToString(row[nameof(NAM_SMOK)]);
                item.RUBRIC = Convert.ToString(row[nameof(RUBRIC)]);
                item.RUBRIC_NAME = Convert.ToString(row[nameof(RUBRIC_NAME)]);
                item.KOL = Convert.ToDecimal(row[nameof(KOL)]);
                item.SUM = Convert.ToDecimal(row[nameof(SUM)]);


                item.K_MEK_NOT_V = Convert.ToDecimal(row[nameof(K_MEK_NOT_V)]);
                item.S_MEK_NOT_V = Convert.ToDecimal(row[nameof(S_MEK_NOT_V)]);
                item.K_P_NOT_V = Convert.ToDecimal(row[nameof(K_P_NOT_V)]);
                item.S_P_NOT_V = Convert.ToDecimal(row[nameof(S_P_NOT_V)]);

                item.FOND = Convert.ToDecimal(row[nameof(FOND)]);
                item.MUR = Convert.ToDecimal(row[nameof(MUR)]);
                item.FAP = Convert.ToDecimal(row[nameof(FAP)]);
                item.KOL_LIMIT = Convert.ToDecimal(row[nameof(KOL_LIMIT)]);
                item.SUM_LIMIT = Convert.ToDecimal(row[nameof(SUM_LIMIT)]);
                item.K_MEK_VK = Convert.ToDecimal(row[nameof(K_MEK_VK)]);
                item.S_MEK_VK = Convert.ToDecimal(row[nameof(S_MEK_VK)]);
                item.K_MEK_VK_RUB = Convert.ToDecimal(row[nameof(K_MEK_VK_RUB)]);
                item.S_MEK_VK_RUB = Convert.ToDecimal(row[nameof(S_MEK_VK_RUB)]);
                item.K_MEK_VS = Convert.ToDecimal(row[nameof(K_MEK_VS)]);
                item.S_MEK_VS = Convert.ToDecimal(row[nameof(S_MEK_VS)]);
                item.K_MEK_VS_RUB = Convert.ToDecimal(row[nameof(K_MEK_VS_RUB)]);
                item.S_MEK_VS_RUB = Convert.ToDecimal(row[nameof(S_MEK_VS_RUB)]);
                item.KOL_P = Convert.ToDecimal(row[nameof(KOL_P)]);
                item.SUM_P = Convert.ToDecimal(row[nameof(SUM_P)]);
                item.SUM_MEK_P = Convert.ToDecimal(row[nameof(SUM_MEK_P)]);
                item.KOL_MEK_P = Convert.ToDecimal(row[nameof(KOL_MEK_P)]);
                item.MUR_RETURN = Convert.ToDecimal(row[nameof(MUR_RETURN)]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения LIMIT_RESULTRow: {e.Message}");
            }
        }
        public bool IsSHOW { get; set; } = true;
        /// <summary>
        /// Год
        /// </summary>
        public int YEAR { get; set; }
        /// <summary>
        /// Месяц
        /// </summary>
        public int MONTH { get; set; }
        /// <summary>
        /// Код МО
        /// </summary>
        public string CODE_MO { get; set; }
        /// <summary>
        /// Наименование МО
        /// </summary>
        public string NAM_MOK { get; set; }
        /// <summary>
        /// СМО
        /// </summary>
        public string SMO { get; set; }
        /// <summary>
        /// Наименование СМО
        /// </summary>
        public string NAM_SMOK { get; set; }
        /// <summary>
        /// Рубрика
        /// </summary>
        public string RUBRIC { get; set; }
        /// <summary>
        /// Наименование рубрики
        /// </summary>
        public string RUBRIC_NAME { get; set; }
        /// <summary>
        /// Кол-во
        /// </summary>
        public decimal KOL { get; set; }
        /// <summary>
        /// Сумма
        /// </summary>
        public decimal SUM { get; set; }
        /// <summary>
        /// Кол-во снятых на МЭК без учета контроля объемов
        /// </summary>
        public decimal K_MEK_NOT_V { get; set; }
        /// <summary>
        /// Сумма снятых на МЭК без учета контроля объемов
        /// </summary>
        public decimal S_MEK_NOT_V { get; set; }
        /// <summary>
        /// Кол-во принятых к оплате без учета контроля объемов
        /// </summary>
        public decimal K_P_NOT_V { get; set; }
        /// <summary>
        /// Сумма принятых к оплате без учета контроля объемов
        /// </summary>
        public decimal S_P_NOT_V { get; set; }
        /// <summary>
        /// Фондодержание
        /// </summary>
        public decimal FOND { get; set; }
        /// <summary>
        /// Муры
        /// </summary>
        public decimal MUR { get; set; }
        /// <summary>
        /// Фондодержание фап
        /// </summary>
        public decimal FAP { get; set; }
        /// <summary>
        /// Количественный лимит
        /// </summary>
        public decimal KOL_LIMIT { get; set; }
        /// <summary>
        /// Стоимостной лимит
        /// </summary>
        public decimal SUM_LIMIT { get; set; }
        /// <summary>
        /// Количество снятых по превышению объемов(количественные показатели)
        /// </summary>
        public decimal K_MEK_VK { get; set; }
        /// <summary>
        /// Сумма снятых по превышению объемов(количественные показатели)
        /// </summary>
        public decimal S_MEK_VK { get; set; }
        /// <summary>
        /// Количество снятых по превышению объемов(количественные показатели)(в рамках рубрики)
        /// </summary>
        public decimal K_MEK_VK_RUB { get; set; }
        /// <summary>
        /// Сумма снятых по превышению объемов(количественные показатели)(в рамках рубрики)
        /// </summary>
        public decimal S_MEK_VK_RUB { get; set; }
        /// <summary>
        /// Количество снятых по превышению объемов(стоимостные показатели)
        /// </summary>
        public decimal K_MEK_VS { get; set; }
        /// <summary>
        /// Сумма снятых по превышению объемов(стоимостные показатели)
        /// </summary>
        public decimal S_MEK_VS { get; set; }
        /// <summary>
        /// Количество снятых по превышению объемов(стоимостные показатели)(в рамках рубрики)
        /// </summary>
        public decimal K_MEK_VS_RUB { get; set; }
        /// <summary>
        /// Сумма снятых по превышению объемов(стоимостные показатели)(в рамках рубрики)
        /// </summary>
        public decimal S_MEK_VS_RUB { get; set; }
        /// <summary>
        /// Принято кол-во
        /// </summary>
        public  decimal KOL_P { get; set; }
        /// <summary>
        ///  Принято Сумма
        /// </summary>
        public decimal SUM_P { get; set; }
        /// <summary>
        /// МЭК прошлого периода сумма
        /// </summary>
        public decimal SUM_MEK_P { get; set; }
        /// <summary>
        ///  МЭК прошлого периода сумма количество
        /// </summary>
        public decimal KOL_MEK_P { get; set; }
        /// <summary>
        ///  Возврат муров
        /// </summary>
        public decimal MUR_RETURN { get; set; }

        /// <summary>
        /// Сумма всего
        /// </summary>
        public decimal SUM_ALL => SUM + FOND + FAP;
        /// <summary>
        /// Сумма принято всего
        /// </summary>
        public decimal SUM_P_ALL => SUM_P + FOND + FAP-MUR+ MUR_RETURN;
        /// <summary>
        /// Признак формирование акта МЭК
        /// </summary>
        public bool IsACT_MEK { get; set; }
        public bool IsMEK_KOL => K_MEK_VK != 0 || S_MEK_VK!=0;
        public bool IsMEK_SUM => K_MEK_VS != 0 || S_MEK_VS != 0;

        public decimal ProcSUM_P => SUM_ALL == 0 ? 0 : Math.Round(SUM_P_ALL / SUM_ALL * 100,2);
        public decimal ProcKOL_P => KOL == 0 ? 0 : Math.Round(KOL_P / KOL * 100, 2);
    }




    public class ControlProcedureVM:INotifyPropertyChanged
    {
        public ProgressItem Progress1 { get; } = new ProgressItem();
        public ObservableCollection<VOLUME_CONTROLRow> LIST { get; set; } = new ObservableCollection<VOLUME_CONTROLRow>();
        public ObservableCollection<MO_SPRRow> MO_SPR { get; set; } = new ObservableCollection<MO_SPRRow>();
        public ObservableCollection<SMO_SPRRow> SMO_SPR { get; set; } = new ObservableCollection<SMO_SPRRow>();
        public ObservableCollection<RUBRIC_SPRRow> RUBRIC_SPR { get; set; } = new ObservableCollection<RUBRIC_SPRRow>();
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


        private void Clear()
        {
            LIST.Clear();
            MO_SPR.Clear();
            SMO_SPR.Clear();
            RUBRIC_SPR.Clear();
        }
        public ICommand RefreshCommand => new Command(async obj =>
        {
            try
            {
                IsOperationRun = true;
                Clear();
                Progress1.IsIndeterminate = true;
                Progress1.Text = "Запрос данных";
                var list = await Task.Run(GetVolumViewData);
                SetLIST(list);
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
           
        }, o=>!IsOperationRun);
        private void SetLIST(List<VOLUME_CONTROLRow> List)
        {
            LIST.AddRange(List);
            var DIC_MO = new Dictionary<string, string>();
            var DIC_SMO = new Dictionary<string, string>();
            var DIC_RUB = new Dictionary<string, string>();
            foreach (var row in LIST)
            {
                if (!DIC_MO.ContainsKey(row.CODE_MO))
                    DIC_MO.Add(row.CODE_MO, row.NAM_MOK);

                if (!DIC_SMO.ContainsKey(row.SMO))
                    DIC_SMO.Add(row.SMO, row.NAM_SMOK);

                if (!DIC_RUB.ContainsKey(row.RUBRIC_ID))
                    DIC_RUB.Add(row.RUBRIC_ID, row.NAME_RUB);

            }
            MO_SPR.Clear();
            MO_SPR.Insert(0, new MO_SPRRow());
            MO_SPR.AddRange(DIC_MO.Select(x => new MO_SPRRow { CODE_MO = x.Key, NAM_OK = x.Value }).OrderBy(x => x.CODE_MO));

            SMO_SPR.Clear();
            SMO_SPR.Insert(0, new SMO_SPRRow());
            SMO_SPR.AddRange(DIC_SMO.Select(x => new SMO_SPRRow { SMOCOD = x.Key, NAM_SMOK = x.Value }).OrderBy(x => x.SMOCOD));

            RUBRIC_SPR.Clear();
            RUBRIC_SPR.Insert(0, new RUBRIC_SPRRow());
            RUBRIC_SPR.AddRange(DIC_RUB.Select(x => new RUBRIC_SPRRow { VOLUM_RUBRIC_ID = x.Key, NAME = x.Value }).OrderBy(x => x.VOLUM_RUBRIC_ID));

            RaisePropertyChanged(nameof(LIST_FILTER));
        }

        public ICommand FilterCommand => new Command(async obj =>
        {
            try
            {
                IsOperationRun = true;
                Progress1.IsIndeterminate = true;
                Progress1.Text = "Фильтрация...";
    
                await Task.Run(FilteringList);
                RaisePropertyChanged(nameof(LIST_FILTER));
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

        }, o => !IsOperationRun);

        private void FilteringList()
        {
            foreach (var item in LIST)
            {
                item.IsSHOW = true;
                if (Filter.IsMEK_KOL.HasValue)
                {
                    item.IsSHOW &= Filter.IsMEK_KOL.Value == item.MEK_KOL;
                }
                if (Filter.IsMEK_SUM.HasValue)
                {
                    item.IsSHOW &= Filter.IsMEK_SUM.Value == item.MEK_SUM;
                }
                if (!string.IsNullOrEmpty(Filter.CODE_MO))
                {
                    item.IsSHOW &= Filter.CODE_MO == item.CODE_MO;
                }
                if (!string.IsNullOrEmpty(Filter.SMO))
                {
                    item.IsSHOW &= Filter.SMO == item.SMO;
                }
                if (!string.IsNullOrEmpty(Filter.RUB))
                {
                    item.IsSHOW &= Filter.RUB == item.RUBRIC_ID;
                }
            }
        }


        public bool OnFilterTriggered(object item)
        {
            if (item is VOLUME_CONTROLRow vol)
            {
                return vol.IsSHOW;
            }
            return true;
        }
        List<VOLUME_CONTROLRow> GetVolumViewData()
        {

            using (var oda = new OracleDataAdapter("select * from table(volum_control.GET_VOLUME)", AppConfig.Property.ConnectionString))
            {
                var tbl = new DataTable();
                oda.Fill(tbl);
                return VOLUME_CONTROLRow.Get(tbl.Select());
            }
        }

        public IEnumerable<VOLUME_CONTROLRow> LIST_FILTER
        {
            get
            {
                foreach (var item in LIST)
                {
                    if(item.IsSHOW)
                        yield return item;
                }
            }
        }



        public FilterParam Filter { get; } = new FilterParam();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }



    public class FilterParam : INotifyPropertyChanged
    {
        private string _CODE_MO;
        public string CODE_MO
        {
            get => _CODE_MO;
            set
            {
                _CODE_MO = value;
                RaisePropertyChanged();
            }
        }
        private string _SMO;
        public string SMO
        {
            get => _SMO;
            set
            {
                _SMO = value;
                RaisePropertyChanged();
            }
        }
        private string _RUB;
        public string RUB
        {
            get => _RUB;
            set
            {
                _RUB = value;
                RaisePropertyChanged();
            }
        }
        private bool? _IsMEK_SUM;
        public bool? IsMEK_SUM
        {
            get => _IsMEK_SUM;
            set
            {
                _IsMEK_SUM = value;
                RaisePropertyChanged();
            }
        }
        private bool? _IsMEK_KOL;
        public bool? IsMEK_KOL
        {
            get => _IsMEK_KOL;
            set
            {
                _IsMEK_KOL = value;
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


    public class ResultControl : INotifyPropertyChanged
    {

        
        private IRepositoryVolumeControl repository;
        private IVCExcelManager VcExcelManager;
        private Action SaveParam;
        public ResultControl(IRepositoryVolumeControl repository, IVCExcelManager VcExcelManager, Action SaveParam)
        {
            this.repository = repository;
            this.VcExcelManager = VcExcelManager;
            this.SaveParam = SaveParam;

        }

        public Param Param { get; } = new Param();
        public ProgressItem Progress1 { get; } = new ProgressItem();
        public ObservableCollection<LIMIT_RESULTRow> LIST { get; set; } = new ObservableCollection<LIMIT_RESULTRow>();
        public ObservableCollection<MO_SPRRow> MO_SPR { get; set; } = new ObservableCollection<MO_SPRRow>();
        public ObservableCollection<SMO_SPRRow> SMO_SPR { get; set; } = new ObservableCollection<SMO_SPRRow>();
        public ObservableCollection<RUBRIC_SPRRow> RUBRIC_SPR { get; set; } = new ObservableCollection<RUBRIC_SPRRow>();
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


        private void Clear()
        {
            LIST.Clear();
            MO_SPR.Clear();
            SMO_SPR.Clear();
            RUBRIC_SPR.Clear();
        }
        public ICommand RefreshCommand => new Command(async obj =>
        {
            try
            {
                IsOperationRun = true;
                Clear();
                Progress1.IsIndeterminate = true;
                Progress1.Text = "Запрос данных";
                var list = Param.IsTemp1? await repository.GET_VRAsync() : await repository.GET_VRAsync(Param.Period.Year, Param.Period.Month);
                SetLIST(list);
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

        }, o => !IsOperationRun);

        private void SetLIST(List<LIMIT_RESULTRow> List)
        {
            LIST.AddRange(List);
            var DIC_MO = new Dictionary<string, string>();
            var DIC_SMO = new Dictionary<string, string>();
            var DIC_RUB = new Dictionary<string, string>();
            foreach (var row in LIST)
            {
                if (!DIC_MO.ContainsKey(row.CODE_MO))
                    DIC_MO.Add(row.CODE_MO, row.NAM_MOK);

                if (!DIC_SMO.ContainsKey(row.SMO))
                    DIC_SMO.Add(row.SMO, row.NAM_SMOK);

                if (!DIC_RUB.ContainsKey(row.RUBRIC))
                    DIC_RUB.Add(row.RUBRIC, row.RUBRIC_NAME);

            }
            MO_SPR.Clear();
            MO_SPR.Insert(0, new MO_SPRRow());
            MO_SPR.AddRange(DIC_MO.Select(x => new MO_SPRRow { CODE_MO = x.Key, NAM_OK = x.Value }).OrderBy(x => x.CODE_MO));

            SMO_SPR.Clear();
            SMO_SPR.Insert(0, new SMO_SPRRow());
            SMO_SPR.AddRange(DIC_SMO.Select(x => new SMO_SPRRow { SMOCOD = x.Key, NAM_SMOK = x.Value }).OrderBy(x => x.SMOCOD));

            RUBRIC_SPR.Clear();
            RUBRIC_SPR.Insert(0, new RUBRIC_SPRRow());
            RUBRIC_SPR.AddRange(DIC_RUB.Select(x => new RUBRIC_SPRRow { VOLUM_RUBRIC_ID = x.Key, NAME = x.Value }).OrderBy(x => x.VOLUM_RUBRIC_ID));

            RaisePropertyChanged(nameof(LIST_FILTER));
        }

        public ICommand FilterCommand => new Command(async obj =>
        {
            try
            {
                IsOperationRun = true;
                Progress1.IsIndeterminate = true;
                Progress1.Text = "Фильтрация...";
                await Task.Run(FilteringList);
                RaisePropertyChanged(nameof(LIST_FILTER));
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

        }, o => !IsOperationRun);

        private SaveFileDialog sfd = new SaveFileDialog() { Filter = "Файлы Excel(*.xlsx)|*.xlsx" };
        public ICommand SavePril5Command => new Command(async obj =>
        {
            try
            {
                sfd.FileName = "Приложение 5";
                if (sfd.ShowDialog() == true)
                {
                    var SMO_LIST = LIST.Select(x => x.SMO).Distinct();
                    List<string> Files = new List<string>();
                    foreach (var SMO in SMO_LIST)
                    {
                        var items = LIST.Where(x => x.SMO == SMO).ToList();
                        if (items.Count != 0)
                        {
                            var year = items[0].YEAR;
                            var month = items[0].MONTH;

                            var path = Path.Combine(Path.GetDirectoryName(sfd.FileName), $"{Path.GetFileNameWithoutExtension(sfd.FileName)} для {SMO} за {year}_{month:D2}{Path.GetExtension(sfd.FileName)}");
                            Files.Add(path);

                           VcExcelManager.VR_TO_PRIL5(path,Param.FIO,Param.DOLG, LIST.Where(x => x.SMO == SMO).ToList());
                        }
                    }
                    if (MessageBox.Show("Показать файлы?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FilesOrFolders(Files);
                    }
                    SaveParam?.Invoke();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => !IsOperationRun);
        public ICommand SaveVRCommand => new Command(async obj =>
        {
            try
            {
                if (LIST.Count != 0)
                {
                    var f = LIST.First();
                    sfd.FileName = $"Отчет по оплате за {new DateTime(f.YEAR, f.MONTH, 1):yyyy_MM} от {DateTime.Now:dd.MM.yyyy}";
                    if (sfd.ShowDialog() == true)
                    {
                        VcExcelManager.VR_TO_XLS(sfd.FileName, LIST);
                        if (MessageBox.Show("Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
                            ShowSelectedInExplorer.FileOrFolder(sfd.FileName);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }, o => !IsOperationRun);


            

        private void FilteringList()
        {
            foreach (var item in LIST)
            {
                item.IsSHOW = true;
                if (Filter.IsMEK_KOL.HasValue)
                {
                    item.IsSHOW &= Filter.IsMEK_KOL.Value == item.IsMEK_KOL;
                }
                if (Filter.IsMEK_SUM.HasValue)
                {
                    item.IsSHOW &= Filter.IsMEK_SUM.Value == item.IsMEK_SUM;
                }
                if (!string.IsNullOrEmpty(Filter.CODE_MO))
                {
                    item.IsSHOW &= Filter.CODE_MO == item.CODE_MO;
                }
                if (!string.IsNullOrEmpty(Filter.SMO))
                {
                    item.IsSHOW &= Filter.SMO == item.SMO;
                }
                if (!string.IsNullOrEmpty(Filter.RUB))
                {
                    item.IsSHOW &= Filter.RUB == item.RUBRIC;
                }
            }
        }


       
      

        public IEnumerable<LIMIT_RESULTRow> LIST_FILTER
        {
            get
            {
                foreach (var item in LIST)
                {
                    if (item.IsSHOW)
                        yield return item;
                }
            }
        }



        public FilterParam Filter { get; } = new FilterParam();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class Param : INotifyPropertyChanged
    {
        private bool _IsTemp1;
        public bool IsTemp1
        {
            get => _IsTemp1;
            set
            {
                _IsTemp1 = value;
                RaisePropertyChanged();
            }
        }

        private DateTime _Period;
        public DateTime Period
        {
            get => _Period;
            set
            {
                _Period = value;
                RaisePropertyChanged();
            }
        }
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


}

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
    public partial class VOLUM_CONTROL : Window, INotifyPropertyChanged
    {
        public ControlProcedureVM ControlProcedureVM { get; }
        public ResultControlVM ResultControlVM { get; }

        public LimitViewVM LimitViewVM { get; }

        public VOLUM_CONTROL()
        {
            var repo = new RepositoryVolumeControl(AppConfig.Property.ConnectionString);
            ResultControlVM = new ResultControlVM(repo, new VCExcelManager(), SaveParam);
            LimitViewVM = new LimitViewVM(repo);
            ControlProcedureVM = new ControlProcedureVM(repo);
            LimitViewVM = new LimitViewVM(repo);

            InitializeComponent();
            DefaultValue();
        }





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













        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }



        public void DefaultValue()
        {
            ResultControlVM.Param.IsTemp1 = true;
            ResultControlVM.Param.Period = LimitViewVM.Param.Period = DateTime.Now.AddMonths(-1);
            ResultControlVM.Param.DOLG = Properties.Settings.Default.PRIL5_DOLG;
            ResultControlVM.Param.FIO = Properties.Settings.Default.PRIL5_FIO;
        }

        void SaveParam()
        {
            Properties.Settings.Default.PRIL5_DOLG = ResultControlVM.Param.DOLG;
            Properties.Settings.Default.PRIL5_FIO = ResultControlVM.Param.FIO;
            Properties.Settings.Default.Save();
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
            RaisePropertyChanged(nameof(SUM_SELECT_LIM));
        }

        private void buttonRefreshVOLUM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var th = new Thread(UpdateVolumeParam) { IsBackground = true };
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
            public bool? IsVOLUME => CountSANK.HasValue ? CountSANK != 0 : (bool?)null;
            public bool? IsSyncMainBD { get; set; }
            public DateTime? DT_ACT { get; set; }
        }

        public VOLUME_PARAM VP { get; set; } = new VOLUME_PARAM();

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
                if (thVolumeCheck?.IsAlive == true)
                {
                    MessageBox.Show("Контроль уже выполняется. Дождитесь завершения процесса");
                    return;
                }

                if (VP.IsVOLUME == true)
                {
                    if (MessageBox.Show("Контроль уже проведен. Вы уверены что хотите перепровести контроль?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;
                }

                thVolumeCheck = new Thread(VolumeCheck) { IsBackground = true };
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
                var dt = (DateTime)obj;
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



    public class MO_SPRRow
    {
        public string CODE_MO { get; set; }
        public string NAM_OK { get; set; }

        public string FULL_NAME => $"{CODE_MO} {NAM_OK}";

    }

    public class SMO_SPRRow
    {
        public string SMOCOD { get; set; }
        public string NAM_SMOK { get; set; }
        public string FULL_NAME => $"{SMOCOD} {NAM_SMOK}";
    }

    public class RUBRIC_SPRRow
    {
        public string VOLUM_RUBRIC_ID { get; set; }
        public string NAME { get; set; }

        public string FULL_NAME => $"{VOLUM_RUBRIC_ID} {NAME}";
    }

}

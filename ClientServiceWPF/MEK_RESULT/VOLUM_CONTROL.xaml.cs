using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClientServiceWPF.Class;
using ExcelManager;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using Path = System.IO.Path;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для VOLUM_CONTROL.xaml
    /// </summary>
    public partial class VOLUM_CONTROL : Window,INotifyPropertyChanged
    {
        public VOLUME_DATA VOL_DATA { get; set; } = new VOLUME_DATA();
        public class VOLUME_DATA
        {
            public List<VOLUME_CONTROLRow> LIST { get; set; } = new List<VOLUME_CONTROLRow>();
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
            }
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
        public VOL_RESULT_DATA VR_DATA { get; set; } = new VOL_RESULT_DATA();
        public class VOL_RESULT_DATA
        {
            public List<LIMIT_RESULTRow> LIST { get; set; } = new List<LIMIT_RESULTRow>();
            public List<MO_SPRRow> MO_SPR { get; set; } = new List<MO_SPRRow>();
            public List<SMO_SPRRow> SMO_SPR { get; set; } = new List<SMO_SPRRow>();
            public List<RUBRIC_SPRRow> RUBRIC_SPR { get; set; } = new List<RUBRIC_SPRRow>();

            public void CreateSPR()
            {
             
                var DIC_MO = new Dictionary<string,string>();
                foreach (var row in LIST)
                {
                    if(!DIC_MO.ContainsKey(row.CODE_MO))
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
                    if (!DIC_RUB.ContainsKey(row.RUBRIC))
                        DIC_RUB.Add(row.RUBRIC, row.RUBRIC_NAME);
                }
                MO_SPR.Clear();
                MO_SPR.Insert(0, new MO_SPRRow());
                MO_SPR.AddRange(DIC_MO.Select(x=>new MO_SPRRow { CODE_MO =  x.Key, NAM_OK = x.Value}).OrderBy(x=>x.CODE_MO));

                SMO_SPR.Clear();
                SMO_SPR.Insert(0, new SMO_SPRRow());
                SMO_SPR.AddRange(DIC_SMO.Select(x => new SMO_SPRRow { SMOCOD = x.Key, NAM_SMOK = x.Value }).OrderBy(x => x.SMOCOD));

                RUBRIC_SPR.Clear();
                RUBRIC_SPR.Insert(0, new RUBRIC_SPRRow());
                RUBRIC_SPR.AddRange(DIC_RUB.Select(x => new RUBRIC_SPRRow { VOLUM_RUBRIC_ID = x.Key, NAME = x.Value }).OrderBy(x => x.VOLUM_RUBRIC_ID));
            }
        }






        private CollectionViewSource CVSVolumView;
        private CollectionViewSource CVSVolumMO;
        private CollectionViewSource CVSVolumSMO;
        private CollectionViewSource CVSVolumRUB;

        private CollectionViewSource CVSLIMITs;
        private CollectionViewSource CVSLIMMO;
        private CollectionViewSource CVSLIMSMO;
        private CollectionViewSource CVSLIMRUB;


        private CollectionViewSource CVSVolumResult;
        private CollectionViewSource CVSVolumResultMO;
        private CollectionViewSource CVSVolumResultSMO;
        private CollectionViewSource CVSVolumResultRUB;



          

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        public VOLUM_CONTROL()
        {
            InitializeComponent();
            CVSVolumView = (CollectionViewSource) FindResource("CVSVolumView");
            CVSVolumMO = (CollectionViewSource) FindResource("CVSVolumMO");
            CVSVolumSMO = (CollectionViewSource) FindResource("CVSVolumSMO");
            CVSVolumRUB = (CollectionViewSource) FindResource("CVSVolumRUB");

            CVSLIMITs = (CollectionViewSource) FindResource("CVSLIMITs");
            CVSLIMMO = (CollectionViewSource) FindResource("CVSLIMMO");
            CVSLIMSMO = (CollectionViewSource) FindResource("CVSLIMSMO");
            CVSLIMRUB = (CollectionViewSource) FindResource("CVSLIMRUB");


            CVSVolumResult = (CollectionViewSource) FindResource("CVSVolumResult");
            CVSVolumResultMO = (CollectionViewSource) FindResource("CVSVolumResultMO");
            CVSVolumResultSMO = (CollectionViewSource) FindResource("CVSVolumResultSMO");
            CVSVolumResultRUB = (CollectionViewSource) FindResource("CVSVolumResultRUB");
        }

        private void buttonGetVolumGetData_Click(object sender, RoutedEventArgs e)
        {
            var th = new Thread(GetVolumViewData) {IsBackground = true};
            th.Start();
        }

        void GetVolumViewData()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    buttonGetVolumGetData.IsEnabled = false;
                    ProgressBarVolumView.IsIndeterminate = true;
                    LabelProgress.Content = "Запрос данных";
                });
                var oda = new OracleDataAdapter("select * from table(volum_control.GET_VOLUME)", AppConfig.Property.ConnectionString);
                var tbl = new DataTable();
                oda.Fill(tbl);
                VOL_DATA.LIST.Clear();
                VOL_DATA.LIST.AddRange(VOLUME_CONTROLRow.Get(tbl.Select()));
                VOL_DATA.CreateSPR();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    buttonGetVolumGetData.IsEnabled = true;
                    ProgressBarVolumView.IsIndeterminate = false;
                    LabelProgress.Content = "";
                    CVSVolumView.View.Refresh();
                    CVSVolumMO.View.Refresh();
                    CVSVolumSMO.View.Refresh();
                    CVSVolumRUB.View.Refresh();
                });

            }
           

        }

        public class FilterParam
        {
            public string CODE_MO { get; set; }
            public string SMO { get; set; }
            public string RUB { get; set; }
            public bool? IsMEK_SUM { get; set; }
            public bool? IsMEK_KOL { get; set; }
        }

        private FilterParam ReadFilterParam()
        {
            var fp = new FilterParam();
            fp.CODE_MO = (comboBoxVOL_MO.SelectedItem as MO_SPRRow)?.CODE_MO;
            fp.SMO = (comboBoxVOL_SMO.SelectedItem as SMO_SPRRow)?.SMOCOD;
            fp.RUB = (comboBoxVOL_RUB.SelectedItem as RUBRIC_SPRRow)?.VOLUM_RUBRIC_ID;
            fp.IsMEK_KOL = CheckBoxMEK_KOL.IsChecked;
            fp.IsMEK_SUM = CheckBoxMEK_SUM.IsChecked;
            return fp;
        }

        private void buttonFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fp = ReadFilterParam();
                var th = new Thread(FiltringList) {IsBackground = true};
                th.Start(fp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void FiltringList(object obj)
        {
            var fp = (FilterParam) obj;
            Dispatcher.Invoke(() =>
            {
                ProgressBarVolumView.IsIndeterminate = true;
                LabelProgress.Content = "Фильтрация...";
            });
            foreach (var item in VOL_DATA.LIST)
            {
                item.IsSHOW = true;
                if (fp.IsMEK_KOL.HasValue)
                {
                    item.IsSHOW &= fp.IsMEK_KOL.Value == item.MEK_KOL;
                }
                if (fp.IsMEK_SUM.HasValue)
                {
                    item.IsSHOW &= fp.IsMEK_SUM.Value == item.MEK_KOL;
                }
                if (!string.IsNullOrEmpty(fp.CODE_MO))
                {
                    item.IsSHOW &= fp.CODE_MO == item.CODE_MO;
                }
                if (!string.IsNullOrEmpty(fp.SMO))
                {
                    item.IsSHOW &= fp.SMO == item.SMO;
                }
                if (!string.IsNullOrEmpty(fp.RUB))
                {
                    item.IsSHOW &= fp.RUB == item.RUBRIC_ID;
                }
            }

            Dispatcher.Invoke(() =>
            {
                ProgressBarVolumView.IsIndeterminate = false;
                LabelProgress.Content = "";
                CVSVolumView.View.Refresh();
            });
        }

        private void CVSVolumView_OnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = ((VOLUME_CONTROLRow) e.Item).IsSHOW;
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

        private void ButtonVOL_RESGetData_OnClick(object sender, RoutedEventArgs e)
        {
            var th = new Thread(GetVR_DATA) { IsBackground = true };
            th.Start();
        }

        void GetVR_DATA()
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    buttonVOL_RESGetData.IsEnabled = false;
                    ProgressBarVOL_RES.IsIndeterminate = true;
                    LabelProgressVOL_RES.Content = "Запрос данных";
                });
                var oda = new OracleDataAdapter("select * from table(volum_control.GET_VR)", AppConfig.Property.ConnectionString);
                var tbl = new DataTable();
                oda.Fill(tbl);
                VR_DATA.LIST.Clear();
                VR_DATA.LIST.AddRange(LIMIT_RESULTRow.Get(tbl.Select()));
                var DicMO = new HashSet<string>(VR_DATA.LIST.Where(x=>x.KOL!=0 || x.SUM!=0).Select(x=>x.CODE_MO+x.SMO).Distinct());
                foreach (var item in VR_DATA.LIST)
                {
                    item.IsACT_MEK = DicMO.Contains(item.CODE_MO + item.SMO);
                }
                VR_DATA.CreateSPR();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Dispatcher.Invoke(() =>
                {
                    buttonVOL_RESGetData.IsEnabled = true;
                    ProgressBarVOL_RES.IsIndeterminate = false;
                    LabelProgressVOL_RES.Content = "";
                    CVSVolumResult.View.Refresh();
                    CVSVolumResultMO.View.Refresh();
                    CVSVolumResultSMO.View.Refresh();
                    CVSVolumResultRUB.View.Refresh();
                });
            }
        }

        private void ButtonFilterVOL_RES_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var fp = ReadFilterVRParam();
                var th = new Thread(FiltringVRList) { IsBackground = true };
                th.Start(fp);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public class FilterVRParam
        {
            public string CODE_MO { get; set; }
            public string SMO { get; set; }
            public string RUB { get; set; }
            public bool? IsMEK_SUM { get; set; }
            public bool? IsMEK_KOL { get; set; }
        }

        private FilterVRParam ReadFilterVRParam()
        {
            var fp = new FilterVRParam();
            fp.CODE_MO = (comboBoxVOL_RES_MO.SelectedItem as MO_SPRRow)?.CODE_MO;
            fp.SMO = (comboBoxVOL_RES_SMO.SelectedItem as SMO_SPRRow)?.SMOCOD;
            fp.RUB = (comboBoxVOL_RES_RUB.SelectedItem as RUBRIC_SPRRow)?.VOLUM_RUBRIC_ID;
            fp.IsMEK_KOL = CheckBoxVOL_RES_KOL.IsChecked;
            fp.IsMEK_SUM = CheckBoxVOL_RES_SUM.IsChecked;
            return fp;
        }

        private void FiltringVRList(object obj)
        {
            var fp = (FilterVRParam)obj;
            Dispatcher.Invoke(() =>
            {
                ProgressBarVOL_RES.IsIndeterminate = true;
                LabelProgressVOL_RES.Content = "Фильтрация...";
            });
            foreach (var item in VR_DATA.LIST)
            {
                item.IsShow = true;

                if (fp.IsMEK_KOL.HasValue)
                {
                    item.IsShow &= fp.IsMEK_KOL.Value == item.IsMEK_KOL;
                }
                if (fp.IsMEK_SUM.HasValue)
                {
                    item.IsShow &= fp.IsMEK_SUM.Value == item.IsMEK_SUM;
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
                    item.IsShow &= fp.RUB == item.RUBRIC;
                }
            }

            Dispatcher.Invoke(() =>
            {
                ProgressBarVOL_RES.IsIndeterminate = false;
                LabelProgressVOL_RES.Content = "";
                CVSVolumResult.View.Refresh();
            });
        }

        private void CVSVolumResult_OnFilter(object sender, FilterEventArgs e)
        {
            e.Accepted = ((LIMIT_RESULTRow)e.Item).IsShow;
        }

        private SaveFileDialog sfd = new SaveFileDialog() {Filter = "Файлы Excel(*.xlsx)|*.xlsx"};
        private void ButtonXLSVOL_RES_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VR_DATA.LIST.Count != 0)
                {
                    var f = VR_DATA.LIST.First();
                    sfd.FileName = $"Отчет по оплате за {new DateTime(f.YEAR, f.MONTH, 1):yyyy_MM} от {DateTime.Now:dd.MM.yyyy}";
                    if (sfd.ShowDialog() == true)
                    {

                        VR_TO_XLS(sfd.FileName);
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
        }


        class PRIL5
        {
            public string CODE_MO { get; set; }
            public string NAME_MOK { get; set; }
            public List<PRIL5_Row> Rows { get; set; } = new List<PRIL5_Row>();
        }

        class  PRIL5_Row
        {
            public int CODE { get; set; }
            public string NAME { get; set; }
            public decimal SUMV { get; set; }
            public decimal SUM_MEK { get; set; }
            public decimal SUM_VOLUM { get; set; }
            public decimal SUM_MUR { get; set; }
            public decimal SUM_P { get; set; }

        }

    
        private PRIL5_Row GetPril5NameRow(string CODE)
        {
            switch (CODE)
            {
                case "1.1":
                case "1.4": return new PRIL5_Row {CODE = 14, NAME = "Стационар без онкологии"};
                case "1.2": return new PRIL5_Row {CODE = 15, NAME = "Стационар онкология" };
                case "1.3":return new PRIL5_Row  {CODE = 17, NAME = "ВМП" };
                case "2.1": return new PRIL5_Row {CODE = 18, NAME = "Дневной стационар без онкологии" };
                case "2.2": return new PRIL5_Row {CODE = 19, NAME = "Дневной стационар онкология" };
                case "2.3": return new PRIL5_Row {CODE = 20, NAME = "Дневной стационар ЭКО" };
                case "3.1.1":
                case "3.1.2":
                case "3.4.1":
                case "3.4.2":
                case "3.5.1":
                case "3.5.2":
                case "3.6.1":
                case "3.6.2":
                case "3.7.1":
                case "3.7.2":
                    return new PRIL5_Row { CODE = 2, NAME = "Амбулаторная МП" };
                case "3.1.3":
                case "3.1.4":
                    return new PRIL5_Row { CODE = 6, NAME = "ФП/ФАП" };
                case "3.2.1":
                case "3.2.2":
                    return new PRIL5_Row { CODE = 13, NAME = "Неотложная МП" };
                case "3.3.1":
                    return new PRIL5_Row { CODE = 7, NAME = "КТ" };
                case "3.3.2":
                    return new PRIL5_Row { CODE = 8, NAME = "МРТ" };
                case "3.3.3":
                    return new PRIL5_Row { CODE = 9, NAME = "УЗИ сердечно-сосуд.системы" };
                case "3.3.4":
                    return new PRIL5_Row { CODE = 10, NAME = "Эндоскопические диагн. исследования" };
                case "3.3.5":
                    return new PRIL5_Row { CODE = 11, NAME = "Патологоанатомические исследования" };
                case "3.3.6":
                    return new PRIL5_Row { CODE = 12, NAME = "Молекулярно - диагн.исследования" };
                case "3.3.7":
                case "3.3.8":
                    return new PRIL5_Row { CODE = 5, NAME = "Определение РНК коронавирусов" };
                case "3.5.3":
                    return new PRIL5_Row { CODE = 3, NAME = "Диспансеризация 2 этап" };
                case "4":
                case "4.3":
                    return new PRIL5_Row { CODE = 1, NAME = "Скорая МП" };
                case "5.1":
                    return new PRIL5_Row { CODE = 16, NAME = "Стационар диализ" };
                case "5.2":
                    return new PRIL5_Row { CODE = 4, NAME = "Услуги диализа" };
                case "-":
                    return new PRIL5_Row { CODE = 21, NAME = "Прочее" };
            }
            throw new Exception($"Не найдена строка в приложении 5 для: {CODE}");
        }


        Dictionary<string, PRIL5> ConvertToPril5(IEnumerable<LIMIT_RESULTRow> list)
        {
            var pril5 = new Dictionary<string, PRIL5>();
            foreach (var item in list)
            {
                if(!pril5.ContainsKey(item.CODE_MO))
                    pril5.Add(item.CODE_MO, new PRIL5() {CODE_MO = item.CODE_MO, NAME_MOK =  item.NAM_MOK});
                var pr = pril5[item.CODE_MO];
                var st = GetPril5NameRow(item.RUBRIC);
                var pr_row = pr.Rows.FirstOrDefault(x => x.CODE == st.CODE);
                if (pr_row == null)
                {
                    pr.Rows.Add(st);
                    pr_row = st;
                }

                pr_row.SUMV += item.SUM_ALL;
                pr_row.SUM_MEK += item.S_MEK_NOT_V;
                pr_row.SUM_VOLUM += item.S_MEK_VS+item.S_MEK_VK;
                pr_row.SUM_MUR += item.MUR;
                pr_row.SUM_P += item.SUM_P_ALL;
            }
            return pril5;
        }

        private void VR_TO_PRIL5(string Path, IEnumerable<LIMIT_RESULTRow> list)
        {
             
            using (var efm = new ExcelOpenXML(Path, "Приложение 5"))
            {
                uint ColI = 6;uint RowI = 1;
                efm.SetColumnWidth("A", 8.43);
                efm.SetColumnWidth("B", 4.71);
                efm.SetColumnWidth("C", 25);
                efm.SetColumnWidth("D", 20.14);
                efm.SetColumnWidth("E", 33.29);
                efm.SetColumnWidth("F", 13.71);
                efm.SetColumnWidth("G", 10.43);
                efm.SetColumnWidth("H", 10.57);
                efm.SetColumnWidth("I", 10.57);
                efm.SetColumnWidth("J", 15.29);
                

                var stringPRIL5Style = efm.CreateType(new FontOpenXML() { Bold = false, fontname = "Calibri", size = 11,HorizontalAlignment = HorizontalAlignmentV.Right, VerticalAlignment = VerticalAlignmentV.Center, wordwrap = true }, new BorderOpenXML(), null);
                var mrow = efm.GetRow(RowI);
                mrow.Height = 66;
                efm.PrintCell(mrow, ColI, "Приложение № 5 к Положению о порядке оплаты медицинской помощи в системе ОМС Забайкальского края", stringPRIL5Style); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI+4));
                RowI++;
                ColI = 2;
                var HeadStyle = efm.CreateType(new FontOpenXML() { Bold = true, fontname = "Calibri", size = 10, HorizontalAlignment = HorizontalAlignmentV.Center, VerticalAlignment = VerticalAlignmentV.Center, wordwrap = true }, new BorderOpenXML(), null);
                mrow = efm.GetRow(RowI);
                mrow.Height = 38.25;
                efm.PrintCell(mrow, ColI, "№", HeadStyle);ColI++;
                efm.PrintCell(mrow, ColI, "Наименование МО", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Код", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "вид мед.помощи", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Представлено по реестру", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК без превышения объемов", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК превышение объемов", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Снято по МУР", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Принято реестров", HeadStyle); ColI++;

                RowI++;
                ColI = 2;
                mrow = efm.GetRow(RowI);

                efm.PrintCell(mrow, ColI, "1", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "2", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "3", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "4", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "5", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "6", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "7", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "8", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "9", HeadStyle); ColI++;




              
               
                var TextStyle = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, fontname = "Calibri", size = 11 }, new BorderOpenXML(), null);
                var TextStyleBOLD = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true, fontname = "Calibri", size = 11 }, new BorderOpenXML(), null);


                var NumberStyle = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Right, Format = (uint)DefaultNumFormat.F4 , fontname = "Calibri", size = 11}, new BorderOpenXML(), null);
                var NumberStyleBold = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Right, Format = (uint)DefaultNumFormat.F4 , fontname = "Calibri", size = 11 , Bold = true}, new BorderOpenXML(), null);


                var PRIL5 = ConvertToPril5(list);
                int i = 0;
               
                foreach (var dic_item in PRIL5)
                {
                    RowI++;
                    var pr5 = dic_item.Value;
                    i++;
                    ColI = 2;
                    mrow = efm.GetRow(RowI);
                    efm.PrintCell(mrow, ColI, i.ToString(), TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.NAME_MOK, TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.CODE_MO, TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, "Итого", TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.Rows.Sum(x=>x.SUMV), NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.Rows.Sum(x => x.SUM_MEK), NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.Rows.Sum(x => x.SUM_VOLUM), NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.Rows.Sum(x => x.SUM_MUR), NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.Rows.Sum(x => x.SUM_P), NumberStyleBold); ColI++;


                    foreach (var row in pr5.Rows.OrderBy(x=>x.CODE))
                    {
                        RowI++;
                        ColI = 2;
                        mrow = efm.GetRow(RowI);
                        efm.PrintCell(mrow, ColI, "", TextStyleBOLD); ColI++;
                        efm.PrintCell(mrow, ColI, "", TextStyleBOLD); ColI++;
                        efm.PrintCell(mrow, ColI, "", TextStyleBOLD); ColI++;
                        efm.PrintCell(mrow, ColI, row.NAME, TextStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUMV, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_MEK, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_VOLUM, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_MUR, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_P, NumberStyle); ColI++;
                    }
                }
                efm.Save();
            }
        }


        private void VR_TO_XLS(string Path)
        {
            using (var efm = new ExcelOpenXML(Path, "Лист1"))
            {
                uint ColI = 1;
                uint RowI = 1;
                var mrow = efm.GetRow(RowI);
                mrow.Height = 50;
                var mrow2 = efm.GetRow(RowI+1);
                var HeadStyle = efm.CreateType(new FontOpenXML() {Bold = true, HorizontalAlignment = HorizontalAlignmentV.Center, VerticalAlignment = VerticalAlignmentV.Center,wordwrap = true}, new BorderOpenXML(), null);
                var TextStyle = efm.CreateType(new FontOpenXML() {  HorizontalAlignment = HorizontalAlignmentV.Left}, new BorderOpenXML(), null);
                var NumberStyle = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Right, Format = (uint)DefaultNumFormat.F4}, new BorderOpenXML(), null);

                efm.PrintCell(mrow, ColI, "СМО", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI,RowI+1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Код МО", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Наименование МО", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Рубрика", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Наименование", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Предъявлено", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI , ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle);ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Предъявлено(всего)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК без учета контроля объемов", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Принято без учета контроля объемов", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Фондодержание", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "МУР", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "ФАП", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Месячный лимит", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(количественные показатели)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(количественные показатели)(в рамках рубрики)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(стоимостные показатели)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(стоимостные показатели)(в рамках рубрики)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;

                efm.PrintCell(mrow, ColI, "Принято к оплате(реестры)", HeadStyle);
                efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle);
                efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 3)); ColI++;
                efm.PrintCell(mrow2, ColI, "%", HeadStyle); ColI++;
                efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow2, ColI, "%", HeadStyle);ColI++;

                efm.PrintCell(mrow, ColI, "Принято к оплате(всего)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Наличие акта МЭК", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                RowI +=2;
                mrow = efm.GetRow(RowI);
                for (uint i = 1; i < ColI; i++)
                {
                    efm.PrintCell(mrow,i, i.ToString(), HeadStyle);
                    var width = 20;
                    switch (i)
                    {
                        case 1:
                        case 2:
                            width = 8;
                            break;
                        case 3:
                            width = 37;
                            break;
                        case 4:
                            width = 9;
                            break;
                        case 5:
                            width = 62;
                            break;
                        default:
                            width = 14;
                            break;
                    }

                    efm.SetColumnWidth(i, width);
                }
                RowI++;
                foreach (var row in VR_DATA.LIST)
                {
                    mrow = efm.GetRow(RowI);
                    RowI++;
                    ColI = 1;
                    efm.PrintCell(mrow, ColI, row.SMO, TextStyle);ColI++;
                    efm.PrintCell(mrow, ColI, row.CODE_MO, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.NAM_MOK, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.RUBRIC, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.RUBRIC_NAME, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.KOL, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_ALL, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_P_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_P_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.FOND, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.MUR, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.FAP, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.KOL_LIMIT, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_LIMIT, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VK, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VK, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VK_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VK_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VS, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VS, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VS_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VS_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.KOL_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.ProcKOL_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.ProcSUM_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_P_ALL, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.IsACT_MEK?"Да" : "Нет", TextStyle); ColI++;
                }
                efm.Save();
            }
         

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

        private void buttonXLSPRIL5_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sfd.FileName = "Приложение 5";
                if (sfd.ShowDialog() == true)
                {
                    var SMO_LIST = VR_DATA.LIST.Select(x => x.SMO).Distinct();
                    List<string> Files = new List<string>();
                    foreach (var SMO in SMO_LIST)
                    {
                        var path = Path.Combine(Path.GetDirectoryName(sfd.FileName), $"{Path.GetFileNameWithoutExtension(sfd.FileName)} для {SMO}{Path.GetExtension(sfd.FileName)}");
                        Files.Add(path);
                        VR_TO_PRIL5(path, VR_DATA.LIST.Where(x=>x.SMO == SMO));
                    }

                    if (MessageBox.Show("Показать файлы?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FilesOrFolders(Files);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                case 6: return KOL_Q2;

                case 7:
                case 8:
                case 9: return KOL_Q3;

                case 10:
                case 11:
                case 12: return KOL_Q4;
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
                case 6: return SUM_Q2;

                case 7:
                case 8:
                case 9: return SUM_Q3;

                case 10:
                case 11:
                case 12: return SUM_Q4;
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
                item.YEAR = Convert.ToInt32(row["YEAR"]);
                item.MONTH = Convert.ToInt32(row["MONTH"]);
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.NAM_MOK = Convert.ToString(row["NAM_MOK"]);
                item.SMO = Convert.ToString(row["SMO"]);
                item.NAM_SMOK = Convert.ToString(row["NAM_SMOK"]);
                item.RUBRIC = Convert.ToString(row["RUBRIC"]);
                item.RUBRIC_NAME = Convert.ToString(row["RUBRIC_NAME"]);
                item.KOL = Convert.ToDecimal(row["KOL"]);
                item.SUM = Convert.ToDecimal(row["SUM"]);


                item.K_MEK_NOT_V = Convert.ToDecimal(row["K_MEK_NOT_V"]);
                item.S_MEK_NOT_V = Convert.ToDecimal(row["S_MEK_NOT_V"]);
                item.K_P_NOT_V = Convert.ToDecimal(row["K_P_NOT_V"]);
                item.S_P_NOT_V = Convert.ToDecimal(row["S_P_NOT_V"]);

                item.FOND = Convert.ToDecimal(row["FOND"]);
                item.MUR = Convert.ToDecimal(row["MUR"]);
                item.FAP = Convert.ToDecimal(row["FAP"]);
                item.KOL_LIMIT = Convert.ToDecimal(row["KOL_LIMIT"]);
                item.SUM_LIMIT = Convert.ToDecimal(row["SUM_LIMIT"]);
                item.K_MEK_VK = Convert.ToDecimal(row["K_MEK_VK"]);
                item.S_MEK_VK = Convert.ToDecimal(row["S_MEK_VK"]);
                item.K_MEK_VK_RUB = Convert.ToDecimal(row["K_MEK_VK_RUB"]);
                item.S_MEK_VK_RUB = Convert.ToDecimal(row["S_MEK_VK_RUB"]);
                item.K_MEK_VS = Convert.ToDecimal(row["K_MEK_VS"]);
                item.S_MEK_VS = Convert.ToDecimal(row["S_MEK_VS"]);
                item.K_MEK_VS_RUB = Convert.ToDecimal(row["K_MEK_VS_RUB"]);
                item.S_MEK_VS_RUB = Convert.ToDecimal(row["S_MEK_VS_RUB"]);
                item.KOL_P = Convert.ToDecimal(row["KOL_P"]);
                item.SUM_P = Convert.ToDecimal(row["SUM_P"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения LIMIT_RESULTRow: {e.Message}");
            }
        }
        public bool IsShow { get; set; } = true;
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
        /// Сумма всего
        /// </summary>
        public decimal SUM_ALL => SUM + FOND + FAP;
        /// <summary>
        /// Сумма принято всего
        /// </summary>
        public decimal SUM_P_ALL => SUM_P + FOND + FAP-MUR;
        /// <summary>
        /// Признак формирование акта МЭК
        /// </summary>
        public bool IsACT_MEK { get; set; }
   

        public bool IsMEK_KOL => K_MEK_VK != 0 || S_MEK_VK!=0;
        public bool IsMEK_SUM => K_MEK_VS != 0 || S_MEK_VS != 0;

        public decimal ProcSUM_P => SUM_ALL == 0 ? 0 : Math.Round(SUM_P_ALL / SUM_ALL * 100,2);
        public decimal ProcKOL_P => KOL == 0 ? 0 : Math.Round(KOL_P / KOL * 100, 2);
    }


  

}

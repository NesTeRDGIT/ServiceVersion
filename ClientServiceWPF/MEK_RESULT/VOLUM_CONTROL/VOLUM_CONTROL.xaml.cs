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
        public DataControlVM DataControlVM { get; }

        public VolumControlVM VolumControlVM { get; }

        public VOLUM_CONTROL()
        {
            var repo = new RepositoryVolumeControl(AppConfig.Property.ConnectionString);
            ResultControlVM = new ResultControlVM(repo, new VCExcelManager(), SaveParam);
            LimitViewVM = new LimitViewVM(repo);
            ControlProcedureVM = new ControlProcedureVM(repo);
            LimitViewVM = new LimitViewVM(repo);
            DataControlVM = new DataControlVM(new DataControlRepository(AppConfig.Property.ConnectionString));
            VolumControlVM = new VolumControlVM(repo);
            InitializeComponent();
            DefaultValue();
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


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
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

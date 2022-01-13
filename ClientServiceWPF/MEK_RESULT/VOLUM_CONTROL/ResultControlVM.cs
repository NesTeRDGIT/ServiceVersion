using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ClientServiceWPF.Class;
using Microsoft.Win32;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF.MEK_RESULT.VOLUM_CONTROL
{
    public class ResultControlVM : INotifyPropertyChanged
    {
        private IRepositoryVolumeControl repository;
        private IVCExcelManager VcExcelManager;
        private Action SaveParam;

        public ResultControlVM(IRepositoryVolumeControl repository, IVCExcelManager VcExcelManager, Action SaveParam)
        {
            this.repository = repository;
            this.VcExcelManager = VcExcelManager;
            this.SaveParam = SaveParam;
            Filter.PropertyChanged += Filter_PropertyChanged;
        }

        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FilterCommand.Execute(null);
        }

        public ResultControlParam Param { get; } = new ResultControlParam();
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
                var list = Param.IsTemp1 ? await repository.GET_VRAsync() : await repository.GET_VRAsync(Param.Period.Year, Param.Period.Month);
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
            Filter.Clear();
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

        public ICommand SavePril5Command => new Command(obj =>
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

                            VcExcelManager.VR_TO_PRIL5(path, Param.FIO, Param.DOLG, LIST.Where(x => x.SMO == SMO).ToList());
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

        public ICommand SaveVRCommand => new Command(obj =>
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

    public class ResultControlParam : INotifyPropertyChanged
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


        public void Clear()
        {
            CODE_MO = RUB = SMO = "";
            IsMEK_KOL = IsMEK_SUM = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

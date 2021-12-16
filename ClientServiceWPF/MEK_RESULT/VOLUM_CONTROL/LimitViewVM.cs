using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ClientServiceWPF.Class;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF.MEK_RESULT.VOLUM_CONTROL
{
    public class LimitViewVM : INotifyPropertyChanged
    {
        private IRepositoryVolumeControl repository { get; }
        public LimitViewVM(IRepositoryVolumeControl repository)
        {
            this.repository = repository;
            this.Filter.PropertyChanged += FilterOnPropertyChanged;
        }



        public FilterParamLimitView Filter { get; } = new FilterParamLimitView();

        public ProgressItem Progress1 { get; } = new ProgressItem();
        public ParamLimitView Param { get; } = new ParamLimitView();
        public ObservableCollection<LIMITRow> LIST { get; set; } = new ObservableCollection<LIMITRow>();
        public ObservableCollection<MO_SPRRow> MO_SPR { get; set; } = new ObservableCollection<MO_SPRRow>();
        public ObservableCollection<SMO_SPRRow> SMO_SPR { get; set; } = new ObservableCollection<SMO_SPRRow>();
        public ObservableCollection<RUBRIC_SPRRow> RUBRIC_SPR { get; set; } = new ObservableCollection<RUBRIC_SPRRow>();

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
                Progress1.IsOperationRun = true;
                Clear();
                Progress1.IsIndeterminate = true;
                Progress1.Text = "Запрос данных";
                var list = await repository.GET_LIMITSAsync(Param.Period.Year, Param.Period.Month);
                SetLIST(list);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Progress1.Clear();
            }

        }, o => !Progress1.IsOperationRun);
        private void SetLIST(List<LIMITRow> List)
        {
            LIST.Clear();
            LIST.AddRange(List);
            Filter.Clear();
            var DIC_MO = new Dictionary<string, string>();
            var DIC_SMO = new Dictionary<string, string>();
            var DIC_RUB = new Dictionary<string, string>();
            foreach (var row in LIST)
            {
                if (!DIC_MO.ContainsKey(row.CODE_MO))
                    DIC_MO.Add(row.CODE_MO, row.NAM_MOK);

                if (!DIC_SMO.ContainsKey(row.SMO))
                    DIC_SMO.Add(row.SMO, row.NAM_SMOK);

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


            RaisePropertyChanged(nameof(LIST_FILTER));
        }


        private void FilterOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FilterCommand.Execute(null);
        }

        private ICommand FilterCommand => new Command(async obj =>
        {
            try
            {
                Progress1.IsOperationRun = true;
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
                Progress1.Clear();
            }

        }, o => !Progress1.IsOperationRun);

        private void FilteringList()
        {
            foreach (var item in LIST)
            {
                item.IsShow = true;
                if (Filter.IsErr.HasValue)
                {
                    item.IsShow &= Filter.IsErr.Value == item.IsErr;
                }
                if (!string.IsNullOrEmpty(Filter.CODE_MO))
                {
                    item.IsShow &= Filter.CODE_MO == item.CODE_MO;
                }
                if (!string.IsNullOrEmpty(Filter.SMO))
                {
                    item.IsShow &= Filter.SMO == item.SMO;
                }
                if (!string.IsNullOrEmpty(Filter.RUB))
                {
                    item.IsShow &= Filter.RUB == item.VOLUM_RUBRIC_ID;
                }
            }
        }




        public IEnumerable<LIMITRow> LIST_FILTER
        {
            get
            {
                foreach (var item in LIST)
                {
                    if (item.IsShow)
                        yield return item;
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
    public class ParamLimitView : INotifyPropertyChanged
    {

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FilterParamLimitView : INotifyPropertyChanged
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
        private bool? _IsErr;
        public bool? IsErr
        {
            get => _IsErr;
            set
            {
                _IsErr = value;
                RaisePropertyChanged();
            }
        }


        public void Clear()
        {
            this.CODE_MO = "";
            this.SMO = "";
            this.RUB = "";
            this.IsErr = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}

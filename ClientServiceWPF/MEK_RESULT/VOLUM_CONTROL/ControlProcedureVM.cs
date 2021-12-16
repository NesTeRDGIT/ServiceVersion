using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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

    public class ControlProcedureVM : INotifyPropertyChanged
    {
        private IRepositoryVolumeControl repository { get; }

        public ControlProcedureVM(IRepositoryVolumeControl repository)
        {
            this.repository = repository;
        }
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
                var list = await repository.GET_VOLUMEAsync();
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

    

        public IEnumerable<VOLUME_CONTROLRow> LIST_FILTER
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


}

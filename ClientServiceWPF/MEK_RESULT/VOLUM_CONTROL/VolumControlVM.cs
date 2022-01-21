using System;
using System.Collections.Generic;
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
    
    public class VolumControlVM : INotifyPropertyChanged
    {
      
        public VOLUME_PARAM VP { get; set; } = new VOLUME_PARAM();
        private IRepositoryVolumeControl repository { get; }

        public VolumControlVM(IRepositoryVolumeControl repository)
        {
            this.repository = repository;
        }

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

        public ProgressItem ProgressMain { get; } = new ProgressItem();
    
        public ICommand RefreshStatusCommand => new Command(async obj =>
        {
            try
            {
                ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                ProgressMain.Text = "Запрос отчетного периода";
                VP.Period = await repository.GetSchetDTAsync();
                if(VP.Period.HasValue)
                {
                    ProgressMain.Text = "Запрос данных о лимитах";
                    var limitStatus = await repository.GetLimitStatusAsync(VP.Period.Value.Year, VP.Period.Value.Month);
                    VP.HasLimit = limitStatus.HasLimit;
                    VP.IsBlockLimit = limitStatus.IsBLOCK;

                    ProgressMain.Text = "Запрос данных о МЭК прошлых периодов";
                    var mekppStatus = await repository.GetStatusMekPPAsync(VP.Period.Value.Year, VP.Period.Value.Month);
                    VP.HasMekPP = mekppStatus.HasMekPP;
                    VP.HasMekDefault = mekppStatus.HasMekDefault;
                    VP.ActMekPP = mekppStatus.ActMekPP;

                }
                ProgressMain.Text = "Запрос наличия санкций";
                VP.CountSANK = await repository.GetCountSANKAsync();
                ProgressMain.Text = "Запрос синхронизации БД";
                VP.IsSyncMainBD = await repository.GetIsSyncMainBDAsync(new Progress<string>(mes =>
                {
                    ProgressMain.Text = $"Запрос синхронизации БД: {mes}";
                }));
                ProgressMain.Text = "Запрос даты акта МЭК";
                VP.CurrActDt = await repository.GetActDtAsync();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
            }
        }, o=> !ProgressMain.IsOperationRun);



        public ICommand RaiseMekPPCommand => new Command(async obj =>
        {
            try
            {
                if(VP.Period.HasValue)
                {
                    ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                    ProgressMain.Text = "Проведение МЭК прошлых периодов";
                    await repository.RaiseMekPPAsync(VP.Period.Value.Year, VP.Period.Value.Month, new Progress<string>(mes =>
                    {
                        ProgressMain.Text = $"Проведение МЭК прошлых периодов: {mes}";
                    }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
                RefreshStatusCommand.Execute(null);
            }

        }, o => !ProgressMain.IsOperationRun && VP.Period.HasValue);


        public ICommand ActMekPPCommand => new Command(async obj =>
        {
            try
            {
                if (VP.Period.HasValue)
                {
                    ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                    ProgressMain.Text = "Актирование МЭК прошлых периодов";
                    await repository.CreateActMekPPAsync(VP.Period.Value.Year, VP.Period.Value.Month, new Progress<string>(mes =>
                    {
                        ProgressMain.Text = $"Актирование МЭК прошлых периодов: {mes}";
                    }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
                RefreshStatusCommand.Execute(null);
            }

        }, o => !ProgressMain.IsOperationRun && VP.Period.HasValue);


        public ICommand CancelMEK_PPCommand => new Command(async obj =>
        {
            try
            {
                if (VP.Period.HasValue)
                {
                    if(MessageBox.Show("Вы уверены, что хотите отменить МЭК прошлых периодов? Текущие результаты будет не возможно востановить","", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                        ProgressMain.Text = "Отмена МЭК прошлых периодов";
                        await repository.CancelActMekPPAsync(VP.Period.Value.Year, VP.Period.Value.Month, new Progress<string>(mes =>
                        {
                            ProgressMain.Text = $"Отмена МЭК прошлых периодов: {mes}";
                        }));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
                RefreshStatusCommand.Execute(null);
            }

        }, o => !ProgressMain.IsOperationRun && VP.Period.HasValue);


        public ICommand CalcLimitCommand => new Command(async obj =>
        {
            try
            {
                if (VP.Period.HasValue)
                {
                    ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                    ProgressMain.Text = "Расчет лимитов";
                    await repository.CalcLimitAsync(VP.Period.Value.Year, VP.Period.Value.Month,new Progress<string>(mes =>
                    {
                        ProgressMain.Text = $"Расчет лимитов: {mes}";
                    }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
                RefreshStatusCommand.Execute(null);
            }

        }, o => !ProgressMain.IsOperationRun);


        public ICommand ControlVolumeCommand => new Command(async obj =>
        {
            try
            {
                if (VP.IsVOLUME == true)
                {
                    if (MessageBox.Show("Контроль уже проведен. Вы уверены что хотите перепровести контроль?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                        return;
                }
                ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                ProgressMain.Text = "Проведение контроля объемов";
                await repository.VolumeCheckAsync(new Progress<string>(mes =>
                {
                    ProgressMain.Text = $"Проведение контроля объемов: {mes}";
                }));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
                RefreshStatusCommand.Execute(null);
            }

        }, o => !ProgressMain.IsOperationRun);


        public ICommand SincBDCommand => new Command(async obj =>
        {
            try
            {               
                ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                ProgressMain.Text = "Синхронизация БД";
                await repository.SyncBDAsync(new Progress<string>(mes =>
                {
                    ProgressMain.Text = $"Синхронизация БД: {mes}";
                }));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
                RefreshStatusCommand.Execute(null);
            }

        }, o => !ProgressMain.IsOperationRun);


        public ICommand SetActDtCommand => new Command(async obj =>
        {
            try
            {
                if (VP.NewActDt.HasValue)
                {
                    ProgressMain.IsIndeterminate = ProgressMain.IsOperationRun = true;
                    ProgressMain.Text = "Установки даты акты МЭК";
                    await repository.SetActDtAsync(VP.NewActDt.Value);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressMain.Clear();
                RefreshStatusCommand.Execute(null);
            }

        }, o => !ProgressMain.IsOperationRun);



        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }

    public class VOLUME_PARAM : INotifyPropertyChanged
    {
        private DateTime? _period;
        public DateTime? Period
        {
            get => _period;
            set {
                _period = value;
                RaisePropertyChanged();
            }
        }
        private int? _countSANK { get; set; }
        public int? CountSANK
        {
            get => _countSANK;
            set
            {
                _countSANK = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsVOLUME));
            }
        }
        public bool? IsVOLUME => CountSANK.HasValue ? CountSANK != 0 : (bool?)null;
        private bool? _IsSyncMainBD { get; set; }
        public bool? IsSyncMainBD
        {
            get => _IsSyncMainBD;
            set
            {
                _IsSyncMainBD = value;
                RaisePropertyChanged();
            }
        }
        private DateTime? _CurrActDt { get; set; }
        public DateTime? CurrActDt
        {
            get => _CurrActDt;
            set
            {
                _CurrActDt = value;
                RaisePropertyChanged();
            }
        }
        private DateTime? _NewActDt { get; set; }
        public DateTime? NewActDt
        {
            get => _NewActDt;
            set
            {
                _NewActDt = value;
                RaisePropertyChanged();
            }
        }

        private bool? _HasLimit { get; set; }
        public bool? HasLimit
        {
            get => _HasLimit;
            set
            {
                _HasLimit = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LimitStatus));
            }
        }

        private bool? _IsBlockLimit { get; set; }
        public bool? IsBlockLimit
        {
            get => _IsBlockLimit;
            set
            {
                _IsBlockLimit = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LimitStatus));
            }
        }

        public string LimitStatus => (!HasLimit.HasValue ? "Нет данных" : HasLimit.Value ? "Лимиты присутствуют" : "Лимиты отсутствуют") + (!IsBlockLimit.HasValue ? "" : IsBlockLimit.Value ? "(Блокированы)" : "(Доступны для пересчета)");

        private bool? _HasMekPP;
        /// <summary>
        /// Наличие МЭК прошлого периода в БД
        /// </summary>
        public bool? HasMekPP
        {
            get => _HasMekPP;
            set
            {
                _HasMekPP = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MekPPStatus));
            }
        }
        private bool? _HasMekDefault;
        /// <summary>
        /// Наличия аката МЭК прошлого периода по умолчанию
        /// </summary>
        public bool? HasMekDefault
        {
            get => _HasMekDefault;
            set
            {
                _HasMekDefault = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MekPPStatus));
            }
        }
        private bool? _ActMekPP;
        /// <summary>
        /// Актирован ли мэк прошлого периода,т.е. завершен
        /// </summary>
        public bool? ActMekPP
        {
            get => _ActMekPP;
            set
            {
                _ActMekPP = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MekPPStatus));
            }
        }
      
        public string MekPPStatus
        {
            get
            {
                if (!HasMekPP.HasValue || !HasMekDefault.HasValue || !ActMekPP.HasValue)
                    return "Нет данных";
              
                if(ActMekPP.Value)
                    return "МЭК прошлого периода завершен";
                else
                {
                    if(HasMekPP.Value)
                    {
                        if (HasMekDefault.Value)
                        {
                            return "МЭК прошлого периода ожидает актирования";
                        }
                        else
                        {
                            return "МЭК прошлого периода присутствует, но отказы по умолчанию отсутствуют";
                        }
                    }
                    else
                    {
                        return "МЭК прошлого периода отсутствует";
                    }
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

}

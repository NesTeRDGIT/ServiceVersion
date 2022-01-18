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

    public class DataControlVM : INotifyPropertyChanged
    {
        private IDataControlRepository repository { get; }

        public DataControlVM(IDataControlRepository repository)
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

        public ProgressItem ProgressTemp100Clear { get; } = new ProgressItem();
        private string _Temp100Status;
        public string Temp100Status
        {
            get => _Temp100Status;
            set
            {
                _Temp100Status = value;
                RaisePropertyChanged();
            }
        }

        public ICommand GetTemp100StatusCommand => new Command(async obj =>
        {
            try
            {
                var result = await repository.GetPeriodTemp100Async();
                Temp100Status = string.Join(",", result.Select(x => x.ToString("MM_yyyy")));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        });

        public ICommand ClearTemp100Command => new Command(async obj =>
        {
            try
            {
                if(MessageBox.Show("Вы уверены, что хотете очистить таблицу текущего приема?","",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                {
                    ProgressTemp100Clear.IsIndeterminate = true;
                    ProgressTemp100Clear.IsOperationRun = true;
                    await repository.ClearTemp100Async(new Progress<string>(s =>
                    {
                        if(ProgressTemp100Clear.IsIndeterminate)
                        {
                            ProgressTemp100Clear.Text = s;
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressTemp100Clear.Clear();
            }

        }, o => !ProgressTemp100Clear.IsOperationRun);



        public ProgressItem ProgressTemp1Clear { get; } = new ProgressItem();
        private string _Temp1Status;
        public string Temp1Status
        {
            get => _Temp1Status;
            set
            {
                _Temp1Status = value;
                RaisePropertyChanged();
            }
        }
        public ICommand GetTemp1StatusCommand => new Command(async obj =>
        {
            try
            {
                var result = await repository.GetPeriodTemp1Async();
                Temp1Status = string.Join(",", result.Select(x => x.ToString("MM_yyyy")));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        });

        public ICommand ClearTemp1Command => new Command(async obj =>
        {
            try
            {
                if (MessageBox.Show("Вы уверены, что хотете очистить таблицу последнего приема?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ProgressTemp1Clear.IsIndeterminate = true;
                    ProgressTemp1Clear.IsOperationRun = true;
                    await repository.ClearTemp1Async(new Progress<string>(s =>
                    {
                        if (ProgressTemp1Clear.IsIndeterminate)
                        {
                            ProgressTemp1Clear.Text = s;
                        }
                    }));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressTemp1Clear.Clear();
            }

        }, o => !ProgressTemp1Clear.IsOperationRun);

        public ProgressItem ProgressTransfer { get; } = new ProgressItem();

        public ICommand TransferTemp100Command => new Command(async obj =>
        {
            try
            {
                if (MessageBox.Show("Вы уверены, что хотете перенести таблицу текущего приема?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ProgressTransfer.IsIndeterminate = true;
                    ProgressTransfer.IsOperationRun = true;
                    await repository.TransferTemp100Async(new Progress<string>(s =>
                    {
                        if (ProgressTransfer.IsIndeterminate)
                        {
                            ProgressTransfer.Text = s;
                        }
                    }));
                    GetTemp1StatusCommand.Execute(null);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                ProgressTransfer.Clear();
            }

        }, o => !ProgressTransfer.IsOperationRun);


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

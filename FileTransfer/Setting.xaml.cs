using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using System.Windows.Shapes;
using FileTransfer.Annotations;
using FileTransfer.Class;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

using System.DirectoryServices.AccountManagement;
using System.Security.Policy;

namespace FileTransfer
{
    /// <summary>
    /// Логика взаимодействия для Setting.xaml
    /// </summary>
    public partial class Setting : Window, INotifyPropertyChanged
    {
        public SettingVM VM { get; set; } = new SettingVM();
        public Setting(TransferRule rule)
        {
            InitializeComponent();
            VM.SetRule(rule);

        }


        public List<string> listBoxRegularSelectedItems => listBoxRegular.SelectedItems.Cast<string>().ToList();



        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private void listBoxRegular_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(listBoxRegularSelectedItems));
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }

    public class SettingVM:INotifyPropertyChanged
    {
        public TransferRule Rule => new TransferRule() {PathDestination = PathDestination, PathSource = PathSource, Rule = RegularList.ToList(), TimeOut = TimeOut, User = new UserWin() {Domain = Domain, UserName = UserName, Password = Password}, onStart = onStart};
        private string _PathSource;
        public string PathSource
        {
            get { return _PathSource; }
            set
            {
                _PathSource = value;
                OnPropertyChanged();
            }
        }

        private string _PathDestination;
        public string PathDestination
        {
            get { return _PathDestination; }
            set
            {
                _PathDestination = value;
                OnPropertyChanged();
            }
        }

        private string _UserName;
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged();
            }
        }

        private string _Password;
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged();
            }
        }

        private string _Domain;
        public string Domain
        {
            get { return _Domain; }
            set
            {
                _Domain = value;
                OnPropertyChanged();
            }
        }

        private int _TimeOut;
        public int TimeOut
        {
            get { return _TimeOut; }
            set
            {
                _TimeOut = value;
                OnPropertyChanged();
            }
        }

        private bool _onStart;
        public bool onStart
        {
            get { return _onStart; }
            set
            {
                _onStart = value;
                OnPropertyChanged();
            }
        }


        public void SetRule(TransferRule rule)
        {
            PathSource = rule.PathSource;
            PathDestination = rule.PathDestination;
            UserName = rule.User.UserName;
            Password = rule.User.Password;
            Domain = rule.User.Domain;
            TimeOut = rule.TimeOut;
            onStart = rule.onStart;
            RegularList.Clear();
            foreach (var r in rule.Rule)
            {
                RegularList.Add(r);
            }
        }
        public ObservableCollection<string> RegularList { get; set; } = new ObservableCollection<string>();

        FolderBrowserDialog fbd = new FolderBrowserDialog();
        public ICommand BrowserPathSourceCommand => new Command(o =>
        {
            try
            {
                fbd.SelectedPath = PathSource;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    PathSource = fbd.SelectedPath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public ICommand BrowserPathDestinationCommand => new Command(o =>
        {
            try
            {
                fbd.SelectedPath = PathDestination;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    PathDestination = fbd.SelectedPath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public bool _IsCheckedUser { get; set; }
        public bool IsCheckedUser
        {
            get { return _IsCheckedUser; }
            set
            {
                _IsCheckedUser = value;
                OnPropertyChanged();
            }
        }
        public   ICommand  CheckUserCommand => new Command(async o =>
        {
            try
            {
                IsCheckedUser = true;
                await Task.Run(() =>
                {
                    var ct = ContextType.Machine;
                    if (!string.IsNullOrEmpty(Domain))
                        ct = ContextType.Domain;
                   
                    using (var pc = new PrincipalContext(ct, Domain))
                    {

                        var isValid = pc.ValidateCredentials(UserName, Password);
                        MessageBox.Show($"Проверка пользователя: {(isValid ? "Успешно" : "Не удалось найти пользователя")}");
                    }
                });
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                IsCheckedUser = false;
            }
        });
        public ICommand AddRegularCommand => new Command(o =>
        {
            try
            {
                var win = new RegularEdit();
                if (win.ShowDialog() == true && !string.IsNullOrEmpty(win.VM.Regular))
                {
                    RegularList.Add(win.VM.Regular);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand RemoveRegularCommand => new Command(o =>
        {
            try
            {
                var items = (IEnumerable<string>) o;
                foreach (var item in items)
                {
                    RegularList.Remove(item);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand EditRegularCommand => new Command(o =>
        {
            try
            {
                var items = (IEnumerable<string>)o;
                var item = items.FirstOrDefault();
                if (!string.IsNullOrEmpty(item))
                {
                    var win = new RegularEdit(item);
                    if (win.ShowDialog() == true && !string.IsNullOrEmpty(win.VM.Regular))
                    {
                        var ind = RegularList.IndexOf(item);
                        if (ind >= 0)
                        {
                            RegularList.RemoveAt(0);
                            RegularList.Insert(ind, win.VM.Regular);
                        }
                    }
                }
               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });


        

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}

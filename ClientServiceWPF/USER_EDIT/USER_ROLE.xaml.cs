using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using ServiceLoaderMedpomData;

namespace ClientServiceWPF.USER_EDIT
{
    /// <summary>
    /// Логика взаимодействия для USER_ROLE.xaml
    /// </summary>
    public partial class USER_ROLE : Window
    {

        IWcfInterface wcf => LoginForm.wcf;

        public USER_ROLE()
        {
            InitializeComponent();
        }

        List<ServiceLoaderMedpomData.ROLES> _Roles = new List<ServiceLoaderMedpomData.ROLES>();
        public List<ServiceLoaderMedpomData.ROLES> Roles
        {
            get { return _Roles; }
            set { _Roles = value; OnPropertyChanged("Roles"); }
        }
        List<ServiceLoaderMedpomData.USERS> _Users = new List<ServiceLoaderMedpomData.USERS>();
        public List<ServiceLoaderMedpomData.USERS> Users
        {
            get { return _Users; }
            set { _Users = value; OnPropertyChanged("Users"); }
        }
      

        System.Windows.Data.CollectionViewSource USERSViewSource;
        System.Windows.Data.CollectionViewSource ROLESViewSource;
        System.Windows.Data.CollectionViewSource USERS_ROLESViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ROLESViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("ROLESViewSource")));
                USERSViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("USERSViewSource")));
                USERS_ROLESViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("USERS_ROLESViewSource")));
              



                Roles.AddRange(wcf.Roles_GetRoles());
                Users.AddRange(wcf.Roles_GetUsers());
                USERSViewSource.View.Refresh();
                USERSViewSource.View.MoveCurrentToFirst();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var u = new NewUser();
                if (u.ShowDialog() == true)
                {
                    u.US.ID = -1;
                    Users.Add(u.US);
                    USERSViewSource.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var us = USERSViewSource.View.CurrentItem as USERS;
            if (us != null)
            {
                var u = new NewUser(us);
                if (u.ShowDialog() == true)
                {
                    u.US.CopyTo(us);
                    us.Modyfi = true;
                    USERSViewSource.View.Refresh();
                }
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var us = USERSViewSource.View.CurrentItem as USERS;
            if (us != null)
            {
                if (us.ID == -1)
                    Users.Remove(us);
                USERSViewSource.View.Refresh();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rol = new ROLES();
                rol.ShowDialog();
                Roles.Clear();
                Roles.AddRange( wcf.Roles_GetRoles());
                ROLESViewSource.View.Refresh();
                USERSViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var insert = Users.Where(x => x.ID == -1).ToList();
                    if (insert.Count != 0)
                        wcf.Roles_EditUsers(TypeEdit.New, insert);

                    var update = Users.Where(x => x.Modyfi && x.ID != -1).ToList();
                    if (update.Count != 0)
                        wcf.Roles_EditUsers(TypeEdit.Update, update);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cLIENT_ROLESDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var us = USERSViewSource.View.CurrentItem as USERS;
            var rol = ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES;
            if (us != null && rol != null)
            {
                if (!us.ROLES.Contains(rol.ID))
                {
                    us.ROLES.Add(rol.ID);
                    us.Modyfi = true;
                }
                USERS_ROLESViewSource.View.Refresh();
                ROLESViewSource.View.Refresh();
            }
        }


        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var us = USERSViewSource.View.CurrentItem as USERS;
            var rol = USERS_ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES;

            if (us != null && rol != null)
            {
                us.ROLES.Remove(rol.ID);
                us.Modyfi = true;
                USERS_ROLESViewSource.View.Refresh();
                ROLESViewSource.View.Refresh();
            }
        }

        private void CollectionViewSourceRoles_Filter(object sender, FilterEventArgs e)
        {
            var us = USERSViewSource.View.CurrentItem as USERS;
            var rol = e.Item as ServiceLoaderMedpomData.ROLES;
            if (us != null && rol != null)
            {
                e.Accepted = !us.ROLES.Contains(rol.ID);
            }
            else
            {
                e.Accepted = false;
            }
        }

        private void cLIENT_USERSDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            USERS_ROLESViewSource.View.Refresh();
            ROLESViewSource.View.Refresh();
        }
        
        private void CollectionViewSourceUSERS_ROLES_Filter(object sender, FilterEventArgs e)
        {
            var us = USERSViewSource.View.CurrentItem as USERS;
            var rol = e.Item as ServiceLoaderMedpomData.ROLES;
            if (us != null && rol != null)
            {
                e.Accepted = us.ROLES.Contains(rol.ID);
            }
            else
            {
                e.Accepted = false;
            }
        }
        
      

        public event PropertyChangedEventHandler PropertyChanged;


        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

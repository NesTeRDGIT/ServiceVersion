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
    /// Логика взаимодействия для ROLES.xaml
    /// </summary>
    public partial class ROLES : Window,INotifyPropertyChanged
    {
        IWcfInterface wcf => LoginForm.wcf;

        public ROLES()
        {
            InitializeComponent();
        }


        public List<ServiceLoaderMedpomData.ROLES> _EXIST_ROLE = new List<ServiceLoaderMedpomData.ROLES>();
        public List<ServiceLoaderMedpomData.ROLES> EXIST_ROLE
        {
            get { return _EXIST_ROLE; }
            set { _EXIST_ROLE = value; OnPropertyChanged("EXIST_ROLE"); }
        }
        List<ServiceLoaderMedpomData.ROLES> DELETE_ROLE = new List<ServiceLoaderMedpomData.ROLES>();
        List<METHOD> _EXIST_METHOD { get; set; } = new List<METHOD>();
        public List<ServiceLoaderMedpomData.METHOD> EXIST_METHOD
        {
            get { return _EXIST_METHOD; }
            set { _EXIST_METHOD = value; OnPropertyChanged("EXIST_METHOD"); }
        }

        CollectionViewSource ROLESViewSource;
        CollectionViewSource ROLES_METHODViewSource;
        CollectionViewSource METHODViewSource;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ROLESViewSource = ((CollectionViewSource)(this.FindResource("ROLESViewSource")));
                ROLES_METHODViewSource = ((CollectionViewSource)(this.FindResource("ROLES_METHODViewSource")));
                METHODViewSource = ((CollectionViewSource)(this.FindResource("METHODViewSource")));

                EXIST_METHOD = wcf.Roles_GetMethod();
                EXIST_ROLE = wcf.Roles_GetRoles();

                DELETE_ROLE = new List<ServiceLoaderMedpomData.ROLES>();
                ROLESViewSource.View.MoveCurrentToFirst();
                ROLES_METHODViewSource.View.Refresh();
                METHODViewSource.View.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var rol = new NewRoles();
            if (rol.ShowDialog() == true)
            {
                EXIST_ROLE.Add(rol._role);
                ROLESViewSource.View.Refresh();
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            var t = (ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES);
            if (t != null)
            {
                var u = new NewRoles(t);
                if (u.ShowDialog() == true)
                {
                    t.ROLE_NAME = u._role.ROLE_NAME;
                    t.ROLE_COMMENT = u._role.ROLE_COMMENT;
                    t.IsModify = true;
                    ROLESViewSource.View.Refresh();
                }
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            var selected = cLIENT_ROLESDataGrid.SelectedItems.Cast<ServiceLoaderMedpomData.ROLES>().ToList();
            foreach (var r in selected)
            {
                if (r.ID != -1)
                    DELETE_ROLE.Add(r);
                EXIST_ROLE.Remove(r);
                ROLESViewSource.View.Refresh();
            }

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var insert = EXIST_ROLE.Where(x => x.ID == -1).ToList();
                    if (insert.Count != 0)
                        wcf.Roles_EditRoles(TypeEdit.New, insert);
                    if (DELETE_ROLE.Count != 0)
                        wcf.Roles_EditRoles(TypeEdit.Delete, DELETE_ROLE);
                    var update = EXIST_ROLE.Where(x => x.IsModify && x.ID != -1).ToList();
                    if (update.Count != 0)
                        wcf.Roles_EditRoles(TypeEdit.Update, update);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void eXIST_METHODDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var roles = (ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES);
            var method = (METHODViewSource.View.CurrentItem as METHOD);

            if (roles != null && method != null)
            {
                roles.AddMethod(method.ID);
            }
            ROLES_METHODViewSource.View.Refresh();
            METHODViewSource.View.Refresh();
        }

        private void CollectionViewSourceROLES_METHOD_Filter(object sender, FilterEventArgs e)
        {
            var roles = ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES;
            var met = e.Item as ServiceLoaderMedpomData.METHOD;
            if (roles != null && met != null)
                e.Accepted = roles.METHOD.Contains(met.ID);
            else e.Accepted = false;
        }

        private void CollectionViewSourceMETHOD_Filter(object sender, FilterEventArgs e)
        {
            var met = e.Item as ServiceLoaderMedpomData.METHOD;
            if (METHOD_NOT_ROLE)
            {
                e.Accepted = !EXIST_ROLE.Any(x => x.METHOD.Contains(met.ID));
            }
            else
            {
                var roles = ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES;

                if (roles != null && met != null)
                    e.Accepted = !roles.METHOD.Contains(met.ID);
                else e.Accepted = false;
            }

        }

        private void cLIENT_ROLESDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ROLES_METHODViewSource.View.Refresh();
            METHODViewSource.View.Refresh();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            var roles = (ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES);
            var method = (ROLES_METHODViewSource.View.CurrentItem as METHOD);

            if (roles != null && method != null)
            {
                roles.RemoveMethod(method.ID);
            }
            ROLES_METHODViewSource.View.Refresh();
            METHODViewSource.View.Refresh();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var win = new Method();
                if (win.ShowDialog() == true)
                {
                    EXIST_METHOD = wcf.Roles_GetMethod();
                    METHODViewSource.View.Refresh();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

   

    

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            var roles = (ROLESViewSource.View.CurrentItem as ServiceLoaderMedpomData.ROLES);
            foreach (METHOD method in eXIST_METHODDataGrid.SelectedItems)
            {
                if (roles != null && method != null)
                {
                    roles.AddMethod(method.ID);
                }
            }

            ROLES_METHODViewSource.View.Refresh();
            METHODViewSource.View.Refresh();
        }

        bool METHOD_NOT_ROLE = false;
        private void checkBox1_Click(object sender, RoutedEventArgs e)
        {
            METHOD_NOT_ROLE = checkBox1.IsChecked.Value;
            METHODViewSource.View.Refresh();
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;


        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

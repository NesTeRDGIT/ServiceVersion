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
    /// Логика взаимодействия для Method.xaml
    /// </summary>
    public partial class Method : Window, INotifyPropertyChanged
    {
          
        public Method()
        {
            InitializeComponent();
            eXIST_METHODViewSource = (CollectionViewSource)FindResource("eXIST_METHODViewSource");
        }

        IWcfInterface wcf => LoginForm.wcf;
        private List<METHOD> _EXIST_METHOD = new List<METHOD>();

        public List<METHOD> EXIST_METHOD
        {
            get { return _EXIST_METHOD; }
            set
            {
                _EXIST_METHOD = value;
                OnPropertyChanged("EXIST_METHOD");
            }
        }

        private CollectionViewSource eXIST_METHODViewSource;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                EXIST_METHOD = wcf.Roles_GetMethod_NEW();
                DELETE = new List<METHOD>();
                var Met = ReflectClass.MethodReflectInfo<IWcfInterface>();
                foreach (var row in EXIST_METHOD)
                {
                    try
                    {
                        Met.First(item => item.Name == row.NAME);
                        row.CHECKED = true;
                    }
                    catch (InvalidOperationException)
                    {
                        row.CHECKED = false;
                    }
                }
                eXIST_METHODViewSource.View.Refresh();
                ResetMethod();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Обновляем лист методами из IWCFServerRegistrationRequests которых нет в EXIST_METHOD
        /// </summary>
        void ResetMethod()
        {
            var Met = ReflectClass.MethodReflectInfo<IWcfInterface>();
            listBoxmetod.Items.Clear();
            foreach (var mi in Met)
            {
                if (EXIST_METHOD.All(x => x.NAME != mi.Name))
                    listBoxmetod.Items.Add(mi.Name);
            }
            eXIST_METHODViewSource.View.Refresh();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (string str in listBoxmetod.SelectedItems)
                {

                    var t = new METHOD { ID = -1, NAME = str, COMENT = textBoxComm.Text.ToUpper().Trim() };
                    EXIST_METHOD.Add(t);
                }
                ResetMethod();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var insert = EXIST_METHOD.Where(x => x.ID == -1).ToList();
                    if (insert.Count != 0)
                        wcf.Roles_EditMethod_NEW(TypeEdit.New, insert);
                    if (DELETE.Count != 0)
                        wcf.Roles_EditMethod_NEW(TypeEdit.Delete, DELETE);
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<METHOD> DELETE = new List<METHOD>();
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (eXIST_METHODDataGrid.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Удаление методов приведет к лишений прав на этот метод у всех ролей. Вы уверены что хотите удалить " + eXIST_METHODDataGrid.SelectedItems.Count.ToString() + " методов?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    //Создаем список удаляемых
                    var list = eXIST_METHODDataGrid.SelectedItems.Cast<METHOD>().ToList();
                    //Помечаем на удаление
                    foreach (var row in list)
                    {
                        if (row.ID != -1)
                            DELETE.Add(row);
                        EXIST_METHOD.Remove(row);
                    }
                }
                ResetMethod();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

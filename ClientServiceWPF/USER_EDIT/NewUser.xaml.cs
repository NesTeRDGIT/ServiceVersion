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
    /// Логика взаимодействия для NewUser.xaml
    /// </summary>
    public partial class NewUser : Window, INotifyPropertyChanged
    {
        public NewUser()
        {
            InitializeComponent();
            OnPropertyChanged("US");

        }
        USERS _US = new USERS();
        public USERS US
        {
            get { return _US; }
            set { _US = value; OnPropertyChanged("US"); }
        }
        public NewUser(USERS user)
        {
            InitializeComponent();
            user.CopyTo(US);
            OnPropertyChanged("US");
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("US");
        }

      
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

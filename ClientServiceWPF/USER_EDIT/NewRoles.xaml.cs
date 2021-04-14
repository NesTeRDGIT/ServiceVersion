using System;
using System.Collections.Generic;
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

namespace ClientServiceWPF.USER_EDIT
{
    /// <summary>
    /// Логика взаимодействия для NewRoles.xaml
    /// </summary>
    public partial class NewRoles : Window
    {
        public NewRoles()
        {
            _role = new ServiceLoaderMedpomData.ROLES() { ID = -1 };
            InitializeComponent();

        }

        public ServiceLoaderMedpomData.ROLES _role;
        public NewRoles(ServiceLoaderMedpomData.ROLES role)
        {
            _role = role;
            InitializeComponent();
            textBox1.Text = role.ROLE_NAME;
            textBox2.Text = role.ROLE_COMMENT;

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            _role.ROLE_NAME = textBox1.Text;
            _role.ROLE_COMMENT = textBox2.Text;
            DialogResult = true;
            Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

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

namespace ClientServiceWPF.MEK_RESULT
{
    /// <summary>
    /// Логика взаимодействия для CustomMessageBox.xaml
    /// </summary>
    public partial class CustomMessageBox : Window
    {
        public CustomMessageBox(string Message, string Header)
        {
            InitializeComponent();
            this.Title = Header;
            textBoxText.Text = Message;
        }

        public MessageBoxResult Result;

        public  static MessageBoxResult Show(string Message, string Header)
        {
            var win = new CustomMessageBox(Message, Header);
            if (win.ShowDialog() == true)
            {
                return win.Result;
            }
            return MessageBoxResult.None;
        }

        private void buttonYes_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.Yes;
            DialogResult = true;
        }

        private void buttonNo_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageBoxResult.No;
            DialogResult = true;
        }
    }
}

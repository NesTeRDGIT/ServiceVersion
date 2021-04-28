using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = System.Windows.MessageBox;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для NewSchemaItem.xaml
    /// </summary>
    public partial class NewSchemaItem : Window
    {
        private bool isLocalFind;
        public NewSchemaItem(bool _isLocalFind)
        {
            isLocalFind = _isLocalFind;
            InitializeComponent();
        }

        private void button32_Click(object sender, RoutedEventArgs e)
        {
            if (isLocalFind)
            {
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    textBoxPathXSD.Text = ofd.FileName;
                }
            }
            else
            {
                var fd = new RemoteFolderDialog("", false);
                if (fd.ShowDialog() == true)
                {
                    textBoxPathXSD.Text = fd.FileNames[0];
                }
            }
        }
        public string PATH => textBoxPathXSD.Text;
        public DateTime DATE_B => DatePickerD_BEG.SelectedDate.Value.Date;
        public DateTime? DATE_E => DatePickerD_END.SelectedDate?.Date;
        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!DatePickerD_BEG.SelectedDate.HasValue)
                    throw new Exception("Не указана дата начала действия схемы");
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }
    }
}

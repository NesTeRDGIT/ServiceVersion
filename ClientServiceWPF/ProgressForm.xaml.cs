using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using MessageBox = System.Windows.Forms.MessageBox;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для ProgressForm.xaml
    /// </summary>
    public partial class ProgressForm : Window
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        IWcfInterface wcf => LoginForm.wcf;
        public void ProgressArch()
        {
            try
            {
                var t = new ProgressClass() { Active = true };
                while (t.Active)
                {
                    t = wcf.GetProgressClassProcessArch();
                    if (t != null)
                    {
                        ProgressBar.Dispatcher.Invoke(() =>
                        {
                            ProgressBar.Maximum = t.Max;
                            ProgressBar.Value = t.Value;
                            TextBlock.Text = t.Active ? $"Активно: {t.TXT}" : $"Не активно: {t.TXT}";
                        });
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ProgressForm_OnLoaded(object sender, RoutedEventArgs e)
        {

            var th = new Thread(ProgressArch) {IsBackground = true};
            th.Start();
        }
    }
}

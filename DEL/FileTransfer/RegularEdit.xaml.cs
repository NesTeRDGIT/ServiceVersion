using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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

namespace FileTransfer
{
    /// <summary>
    /// Логика взаимодействия для RegularEdit.xaml
    /// </summary>
    public partial class RegularEdit : Window
    {
        public RegularEditVM VM { get; set; } = new RegularEditVM();
        public RegularEdit(string regular="")
        {
            InitializeComponent();
            VM.Regular = regular;
        }

        
        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }

    public class RegularEditVM : INotifyPropertyChanged
    {
        private string _Regular;
        public string Regular
        {
            get { return _Regular; }
            set
            {
                _Regular = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ExampleResult));
            }
        }
        private string _Example;
        public string Example
        {
            get { return _Example; }
            set
            {
                _Example = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ExampleResult));
            }
        }
        public string ExampleResult
        {
            get
            {
                try
                {
                    var r = new Regex(Regular);
                    var res = r.IsMatch(Example);
                    return res ? "Выражение корректно" : "Выражение не корректно";
                }
                catch (Exception ex)
                {
                    return $"Исключение: {ex.Message}";
                }
            }
           
        }



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

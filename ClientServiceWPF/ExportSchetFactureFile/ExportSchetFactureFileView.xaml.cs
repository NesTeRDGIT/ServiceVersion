using System.Windows;
using System.Windows.Threading;

namespace ClientServiceWPF.ExportSchetFactureFile
{
    /// <summary>
    /// Логика взаимодействия для ExportSchetFactureFileView.xaml
    /// </summary>
    public partial class ExportSchetFactureFileView : Window
    {
        private ExportShetFactureFileViewModel VM { get; set;  } 
        public ExportSchetFactureFileView()
        {
            InitializeComponent();
            
            VM = new ExportShetFactureFileViewModel(dispatcher: Dispatcher.CurrentDispatcher);
            DataContext = VM;
        }
    }
}

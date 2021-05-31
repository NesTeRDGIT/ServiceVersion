using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using DocumentFormat.OpenXml.Bibliography;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using ServiceLoaderMedpomData.EntityMP_V2;
using Application = System.Windows.Application;
using LogType = ClientServiceWPF.Class.LogType;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace ClientServiceWPF.ORDERS.ORD15
{
    /// <summary>
    /// Логика взаимодействия для ORD15.xaml
    /// </summary>
    public partial class ORD15 : Window
    {
        public  ORD15VM VM { get; set; } = new ORD15VM(Dispatcher.CurrentDispatcher);
        public ORD15()
        {
            VM.PARAM.PERIOD = DateTime.Now.AddMonths(-1);
            VM.PARAM.DATE = DateTime.Now;
            
            InitializeComponent();
            
        }
    }


    public class ORD15VM : INotifyPropertyChanged
    {
        private string PATH_XSD = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"PR_XSD", "PR15.xsd");
        private Dispatcher dispatcher;

        public ORD15VM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public ORD15PARAM PARAM { get; } = new ORD15PARAM();

        public ObservableCollection<LogItem> Logs { get; set; } = new ObservableCollection<LogItem>();

        private void AddLogs(LogType type, params string[] Message)
        {
            dispatcher.Invoke(() =>
                {
                    foreach (var mes in Message)
                    {
                        Logs.Add(new LogItem(type, mes));
                    }
                }
            );
        }

        public ProgressItem Progress1 { get; } = new ProgressItem();

        private SaveFileDialog sfd = new SaveFileDialog() {Filter = "*.xml|*.xml"};
        private bool _IsOperationRun;
        public bool IsOperationRun
        {
            get => _IsOperationRun;
            set
            {
                _IsOperationRun = value;
                RaisePropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }


        public ICommand SaveFileCommand => new Command(async o =>
        {
            try
            {
                sfd.FileName = PARAM.FILENAME;
                if (sfd.ShowDialog() == true)
                {
                    IsOperationRun = true;
                    Logs.Clear();
                    Progress1.IsIndeterminate = true;
                    await Task.Run(() => { GetFileO(sfd.FileName, PARAM.FILENAME, PARAM.DATE, PARAM.ISP ? PARAM.ISP_NAME : null, PARAM.YEAR, PARAM.MONTH); });

                    if (MessageBox.Show(@"Завершено. Показать файл?", "", MessageBoxButton.YesNo) ==  MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(sfd.FileName);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                Progress1.Clear("");
                IsOperationRun = false;
            }
        });

        void GetFileO(string path, string FILENAME, DateTime DateFile, string ISPName, int Year, int Month)
        {
            AddLogs(LogType.Info, "Создание ZGLV");
            var UTV_OB = new ISP_OB {ZGLV = {DATA = DateFile, FILENAME = FILENAME, VERSION = "1.0"}, SVD = {CODE = Year * 1000 + Month, YEAR = Year, MONTH = Month}};

            if (!string.IsNullOrEmpty(ISPName))
                UTV_OB.ZGLV.FIRSTNAME = ISPName;

            AddLogs(LogType.Info, "Запрос случаев");
            var tbl = new DataTable();
            using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from PR15_ZAP t where t.year = {Year} and t.month = {Month}", conn))
                {
                    oda.Fill(tbl);
                }
            }

            var i = 1;
            AddLogs(LogType.Info, "Формирование случаев");
            foreach (DataRow row in tbl.Rows)
            {
                var z = new ZAP();
                UTV_OB.ZAP.Add(z);
                z.N_ZAP = i;
                z.PACIENT = PACIENT.Get(row);
                z.SLUCH = SLUCH.Get(row);
                i++;
            }

            AddLogs(LogType.Info, "Формирование свода");
            UTV_OB.GetOBSV();
            UTV_OB.SVD.OBLM = UTV_OB.ZAP.Count != 0 ? 1 : 0;
            AddLogs(LogType.Info, "Сохранение файла");
            using (Stream st = File.Create(path))
            {
                UTV_OB.WriteXml(st);
                st.Close();
            }

            AddLogs(LogType.Info, "Проверка схемы");

            var sc = new SchemaChecking();
            var err = sc.CheckXML(path, PATH_XSD);
            AddLogs(LogType.Error, err.Select(x => x.MessageOUT).ToArray());
            AddLogs(LogType.Info, "Завершено");
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    public class ORD15PARAM : INotifyPropertyChanged
    {
        private DateTime _PERIOD;
        public DateTime PERIOD
        {
            get => _PERIOD;
            set
            {
                _PERIOD = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(YEAR));
                RaisePropertyChanged(nameof(MONTH));
                RaisePropertyChanged(nameof(FILENAME));
            }
        }
        public int YEAR => _PERIOD.Year;
        public int MONTH => _PERIOD.Month;
        private DateTime _DATE;
        public DateTime DATE
        {
            get => _DATE;
            set
            {
                _DATE = value;
                RaisePropertyChanged();
            }
        }
        private int _NN;
        public int NN
        {
            get => _NN;
            set
            {
                _NN = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FILENAME));
            }
        }
        private bool _ISP;
        public bool ISP
        {
            get => _ISP;
            set
            {
                _ISP = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(FILENAME));
            }
        }
        private string _ISP_NAME;
        public string ISP_NAME
        {
            get => _ISP_NAME;
            set
            {
                _ISP_NAME = value;
                RaisePropertyChanged();
            }
        }
        public string FILENAME =>  $"{(ISP?"OS": "OR")}75{YEAR.ToString().Substring(2)}{NN:D4}" ;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

}

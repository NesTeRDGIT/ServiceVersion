using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using ClientServiceWPF.Class;
using ClientServiceWPF.ORDERS.ORD104;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData.Annotations;
using static System.String;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace ClientServiceWPF.ORDERS.FSB
{
    /// <summary>
    /// Логика взаимодействия для FSB.xaml
    /// </summary>
    public partial class FSB : Window
    {
        public FSBVM VM { get; set; } = new FSBVM(Dispatcher.CurrentDispatcher);
        public FSB()
        {
            VM.PARAM.PERIOD_FROM = new DateTime(DateTime.Now.Year, 1, 1);
            VM.PARAM.PERIOD_TO = DateTime.Now.AddMonths(-1);
            InitializeComponent();
        }
    }

    public class FSBVM : INotifyPropertyChanged
    {
        private Dispatcher dispatcher;
        public ORD104VM VM { get; set; } = new ORD104VM(Dispatcher.CurrentDispatcher);
        public FSBVM(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }
        public FSBPARAM PARAM { get; } = new FSBPARAM();

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
        private void AddLogAndProgress(LogType type, string Message)
        {
            dispatcher.Invoke(() =>
                {
                    Logs.Add(new LogItem(type, Message));
                    Progress1.Text = Message;
                }
            );
        }

        public ProgressItem Progress1 { get; } = new ProgressItem();

      
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

        private FolderBrowserDialog fbd = new FolderBrowserDialog();
        public ICommand SaveFileCommand => new Command(async o =>
        {
            try
            {
            
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    Progress1.IsIndeterminate = true;
                    IsOperationRun = true;
                    Logs.Clear();
                    await Task.Run(() =>
                    {
                        GetFiles(fbd.SelectedPath, PARAM.YEAR_FROM, PARAM.MONTH_FROM, PARAM.YEAR_TO, PARAM.MONTH_TO);
                    });

                    if (MessageBox.Show(@"Завершено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(fbd.SelectedPath);
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

        void GetFiles(string folder, int YEAR_FROM, int MONTH_FROM, int YEAR_TO, int MONTH_TO)
        {
            var date_s = new DateTime(YEAR_FROM, MONTH_FROM, 1);
            var date_e = new DateTime(YEAR_TO, MONTH_TO, 1);
            var List = new SVODList();
            while (date_s <= date_e)
            {
                try
                {
                    var pl = new PERS_LIST {ZGLV = {DATA = DateTime.Now, MONTH = date_s.Month, YEAR = date_s.Year}};
                    var FILENAME = $"LMP_{pl.ZGLV.YEAR}_{pl.ZGLV.MONTH:00}.xml";
                    AddLogAndProgress(LogType.Info, $"Запрос файла за {date_s:MMMMMMMMMMMM yyyy}");
                    var tbl = new DataTable();
                    using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                    {
                        using (var oda = new OracleDataAdapter($"select * from v_FSB_VIEW where year = {date_s.Year} and month = {date_s.Month}", conn))
                        {
                            oda.Fill(tbl);
                        }
                    }

                    AddLogAndProgress(LogType.Info, $"Создание файла за {date_s:MMMMMMMMMMMM yyyy}");
                    var count = 0;
                    foreach (DataRow row in tbl.Rows)
                    {
                        pl.PERS.Add(PERS.Get(row));
                        count++;
                    }

                    AddLogAndProgress(LogType.Info, $"Сохранение файла за {date_s:MMMMMMMMMMMM yyyy}");
                    using (var st = File.Create(Path.Combine(folder, FILENAME)))
                    {
                        pl.WriteXml(st);
                        st.Close();
                    }

                    var sv = new SVOD {FileName = FILENAME, COUNT = count};
                    List.SVODs.Add(sv);
                    AddLogAndProgress(LogType.Info, $"Просчет SHA1 файла за {date_s:MMMMMMMMMMMM yyyy}");
                    using (var stream = File.OpenRead(Path.Combine(folder, FILENAME)))
                    {
                        var sha = new SHA1Managed();
                        var hash = sha.ComputeHash(stream);
                        sv.Size = stream.Length;
                        sv.SHA1 = BitConverter.ToString(hash).Replace("-", Empty);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                date_s = date_s.AddMonths(1);
            }

            try
            {
                List.WriteXml(Path.Combine(folder, "SVOD.xml"));
                AddLogAndProgress(LogType.Info, "Завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

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

    public class FSBPARAM : INotifyPropertyChanged
    {
        private DateTime _PERIOD_FROM;

        public DateTime PERIOD_FROM
        {
            get => _PERIOD_FROM;
            set
            {
                _PERIOD_FROM = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(YEAR_FROM));
                RaisePropertyChanged(nameof(MONTH_FROM));
            }
        }

        public int YEAR_FROM => PERIOD_FROM.Year;
        public int MONTH_FROM => PERIOD_FROM.Month;
        private DateTime _PERIOD_TO;

        public DateTime PERIOD_TO
        {
            get => _PERIOD_TO;
            set
            {
                _PERIOD_TO = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(YEAR_TO));
                RaisePropertyChanged(nameof(MONTH_TO));
            }
        }

        public int YEAR_TO => PERIOD_TO.Year;
        public int MONTH_TO => PERIOD_TO.Month;




        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


    public class SVOD
    {
        public string FileName { get; set; }
        public int COUNT { get; set; }
        public long Size { get; set; }
        public string SHA1 { get; set; }
    }

    public class SVODList
    {
        public List<SVOD> SVODs = new List<SVOD>();

        public void WriteXml(string path)
        {
            var ser = new XmlSerializer(typeof(List<SVOD>));
            var set = new XmlWriterSettings {Encoding = Encoding.GetEncoding("Windows-1251"), Indent = true};
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var xml = XmlWriter.Create(path, set))
            {
                ser.Serialize(xml, SVODs, ns);
                xml.Close();
            }
        }
    }
}

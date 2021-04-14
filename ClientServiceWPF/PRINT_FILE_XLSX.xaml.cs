using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClientServiceWPF.Class;
using ExcelManager;
using Worksheet = Microsoft.Office.Interop.Excel.Worksheet;
using Application = Microsoft.Office.Interop.Excel.Application;
using Clipboard = System.Windows.Clipboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using Path = System.IO.Path;
using PrintDialog = System.Windows.Controls.PrintDialog;
using Window = System.Windows.Window;
using Workbook = Microsoft.Office.Interop.Excel.Workbook;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для PRINT_FILE_XLSX.xaml
    /// </summary>
    public partial class PRINT_FILE_XLSX : Window, INotifyPropertyChanged
    {

        public List<FilePrintXLS> FilePrintXLS { get; set; } = new List<FilePrintXLS>();
        private CollectionViewSource CVSFiles;
        public PRINT_FILE_XLSX()
        {
            InitializeComponent();
            //PrintXLS(@"C:\Users\ndv\Desktop\1111111\Новая папка (37)\Акт МЭК 1 для 750001.xlsx");
            CVSFiles = (CollectionViewSource) FindResource("CVSFiles");
        }

        public int CountPageAll => FilePrintXLS.Sum(x=>x.CountPage);

        private OpenFileDialog ofd = new OpenFileDialog() {Filter = "Файлы Excel(*.xlsx)|*.xlsx", Multiselect = true};

        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }

        private void ButtonAddFiles_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ofd.ShowDialog() == true)
                {
                    foreach (var x in ofd.FileNames)
                    {
                        try
                        {
                            FilePrintXLS.Add(new FilePrintXLS() { FilePath = x, IsPRINT = false, CountPage = 0 });
                          
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Для файла {x}: {ex.Message}");
                        }
                    }
                    CVSFiles.View.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void GetPage()
        {
            Microsoft.Office.Interop.Excel.Application wApplication = null;
           
            try
            {
                wApplication = new Application();
                Dispatcher.Invoke(() => {
                    ProgressBar.Maximum = FilePrintXLS.Count;
                    LabelProgress.Content = "Просчет кол-ва страниц";
                });
                int i = 0;
                foreach (var x in FilePrintXLS)
                {
                    try
                    {
                        x.CountPage = GetPageCount(wApplication, x.FilePath);
                        Dispatcher.Invoke(() => {ProgressBar.Value = i;});
                        i++;
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() => { MessageBox.Show($"Для файла {x}: {ex.Message}"); });
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally
            {
                Dispatcher.Invoke(() => {
                    CVSFiles.View.Refresh();
                    RaisePropertyChanged("CountPageAll");
                    ProgressBar.Value = 0;
                    LabelProgress.Content = "";
                });
                if (wApplication != null)
                {
                    wApplication.Quit();
                    Marshal.FinalReleaseComObject(wApplication);
                }
            }
        }

        public static int GetPageCount(Microsoft.Office.Interop.Excel.Application App, string path)
        {
            try
            {
                var workbooks = App.Workbooks;
                Microsoft.Office.Interop.Excel.Workbook book = null;
                book = workbooks.Open(path, 1, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "", false, false, 0, true, false, Microsoft.Office.Interop.Excel.XlCorruptLoad.xlNormalLoad);
                var CountAll = 0;
                foreach (Microsoft.Office.Interop.Excel.Worksheet sheet in book.Sheets)
                {
                     CountAll+= sheet.HPageBreaks.Count + 1;
                }
                book.Close(SaveChanges: false);
                workbooks.Close();
                return CountAll;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при считывании кол-ва страниц", ex);
            }

        }


        public void Block(bool val)
        {
            ButtonAddFiles.IsEnabled = val;
            ButtonPageCount.IsEnabled = val;
            MenuItemDelete.IsEnabled = val;
            MenuItemClear.IsEnabled = val;
        }
        private List<FilePrintXLS> Selected => ListView.SelectedItems.Cast<FilePrintXLS>().ToList();
        private void MenuItemDelete_OnClick(object sender, RoutedEventArgs e)
        {
            var sel = Selected;
            if (sel.Count != 0)
            {
                if (MessageBox.Show($"Вы уверены что хотите удалить {sel.Count} элементов?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    foreach (var item in sel)
                    {
                        FilePrintXLS.Remove(item);
                    }
                    CVSFiles.View.Refresh();
                }
            }
        }

        private void MenuItemClear_OnClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($"Вы уверены что хотите очистить список?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                FilePrintXLS.Clear();
                CVSFiles.View.Refresh();
            }
        }

        private bool _IsPrinting;

        public bool IsPrinting
        {
            get { return _IsPrinting; }
            set
            {
                _IsPrinting = value;
                RaisePropertyChanged("IsPrinting");
            }
        }

        private void ButtonPrint_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsPrinting)
            {
                if (MessageBox.Show($"Вы уверены что хотите прервать печать?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ct.Cancel();
                }
            }
            else
            {
                if (MessageBox.Show($"Вы уверены что хотите отправит файлы на печать?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    FilePrintXLS.ForEach(x => x.IsPRINT = false);
                    var TH = new Thread(PrintXLS) { IsBackground = true };
                    TH.SetApartmentState(ApartmentState.STA);
                    ct = new CancellationTokenSource();
                    TH.Start();
                }
            }
        }

        private CancellationTokenSource ct = new CancellationTokenSource();
        void PrintXLS()
        {
            Application wApplication = null;
            Workbook wb = null;
            try
            {
                
                Dispatcher.Invoke(() =>
                {
                    IsPrinting = true;
                    ProgressBar.Maximum = FilePrintXLS.Count;
                    ProgressBar.Value = 0;
                    Block(false);
                });

                if (FilePrintXLS.Count == 0) return;

                var print_name = "";
                int? CopyCount = 1;
                PrintQueue pq = null;
                Dispatcher.Invoke(() =>
                {
                    var pd = new PrintDialog();
                    
                    if (pd.ShowDialog() == true)
                    {
                        print_name = pd.PrintQueue.Name;
                        CopyCount = pd.PrintTicket.CopyCount;
                        pq = pd.PrintQueue;
                    }
                 
                });

                if (!string.IsNullOrEmpty(print_name))
                {
                  
                    Dispatcher.Invoke(() => { LabelProgress.Content = "Открытие приложения Excel"; });
                    wApplication = new Application();
                    //SetDuplex(print_name);
                  
                     var i = 0;
                    foreach (var file in FilePrintXLS)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            throw  new Exception("Прервано!!!");
                        }
                     
                        wApplication.Workbooks.Open(file.FilePath, null, true);
                        wb = wApplication.Workbooks[1];
                        for (var j = 0; j < (CopyCount??1); j++)
                        {
                            for (var wi = 1; wi<=wb.Worksheets.Count;wi++)
                            {
                                try
                                {
                                    Worksheet work = wb.Worksheets[wi];
                                    Dispatcher.Invoke(() => { LabelProgress.Content = $"Печать {Path.GetFileName(file.FilePath)}{((CopyCount ?? 1) != 1 ? $": копия {j + 1}" : "")}"; });
                                    work.PrintOut(Type.Missing, Type.Missing, Type.Missing, Type.Missing, print_name, Type.Missing, Type.Missing, Type.Missing);
                                    WaitPrinting(pq);
                                }
                                catch (Exception ex)
                                {
                                    if (MessageBox.Show($"Ошибка при печати:{ex.Message}{Environment.NewLine} Повторить?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                    {
                                        wi--;
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        wb.Close(false, Type.Missing, Type.Missing);
                        Marshal.FinalReleaseComObject(wb);
                        i++;
                        Dispatcher.Invoke(() => { file.IsPRINT = true; ProgressBar.Value = i; });
                    }
                }
            }
            catch (Exception e)
            {
                Dispatcher.Invoke(() => { MessageBox.Show(e.Message); });
            }
            finally
            {
                if (wApplication != null)
                {
                    wApplication.Quit();
                    Marshal.FinalReleaseComObject(wApplication);
                }
                Dispatcher.Invoke(() =>
                {
                    IsPrinting = false;
                    ProgressBar.Maximum = FilePrintXLS.Count;
                    ProgressBar.Value = 0;
                    LabelProgress.Content = "";
                    Block(true);
                });
            }
        }

        private void SetDuplex(string print)
        {
            var ds = new DuplexSettings();
            short status = 0;
            string errorMessage = string.Empty;
            status = ds.GetPrinterDuplex(print, out errorMessage);
            if (status == 0)
            {
               throw new Exception("Данный принтер не поддерживает печать с 2х сторон");
            }
            status = 2; //set duplex flag to 2
            var printerSettings = new System.Drawing.Printing.PrinterSettings();
            printerSettings.Duplex = Duplex.Horizontal;

           
            ds.SetPrinterDuplex(print, status, out errorMessage);
        }

        private void WaitPrinting(PrintQueue pq)
        {
            Dispatcher.Invoke(() => { LabelProgress2.Content = "Ожидание доступности печати"; });
            while (true)
            {
                var NumberOfJobs = 0;
                this.Dispatcher.Invoke(() =>
                {
                    pq.Refresh();
                    NumberOfJobs = pq.NumberOfJobs;
                });
                if (NumberOfJobs == 0)
                {
                    Dispatcher.Invoke(() => { LabelProgress2.Content = ""; });
                    break;
                }
                Thread.Sleep(100);
            }
        }

        private void ListView_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.C)
            {
                var sel = Selected;
                Clipboard.Clear();
                Clipboard.SetText(string.Join(Environment.NewLine, sel.Select(x=>$"{x.FileName} {x.FilePath}")));
            }
        }

        private void ButtonPageCount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var th = new Thread(GetPage) {IsBackground = true};
                th.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PRINT_FILE_XLSX_OnClosing(object sender, CancelEventArgs e)
        {
            if (IsPrinting)
            {
                e.Cancel = true;
                MessageBox.Show("Идет печать, закрытие окна не возможно");
            }
        }
    }


    public class FilePrintXLS: INotifyPropertyChanged
    {
        public  string FilePath { get; set; }
        public string FileName => Path.GetFileName(FilePath);
        private bool _IsPRINT;

        public bool IsPRINT
        {
            get { return _IsPRINT; }
            set
            {
                _IsPRINT = value;
                RaisePropertyChanged("IsPRINT");
            }
        }

        public int CountPage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
    }
}

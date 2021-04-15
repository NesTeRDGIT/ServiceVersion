using System;
using System.Collections.Generic;
using System.Data;
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
using ClientServiceWPF.Class;
using ExcelManager;
using Microsoft.Win32;
using ServiceLoaderMedpomData;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для MonitorReestr.xaml
    /// </summary>
    public partial class MonitorReestr : Window
    {
        private IWcfInterface wcf => LoginForm.wcf;
        List<string> Card => LoginForm.SecureCard;

        public List<V_NOT_REESTR_MEDSERV_row> ListNotReestr { get; set; } = new List<V_NOT_REESTR_MEDSERV_row>();
        private CollectionViewSource CVSNotReestr;

        public MonitorReestr()
        {
            InitializeComponent();
            SetControlForm(Card);
            CVSNotReestr = (CollectionViewSource) FindResource("CVSNotReestr");
        }

        void SetControlForm(List<string> card)
        {
            TabItemNotReestr.IsEnabled = card.Contains("GetNotReestr");
        }

        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            var th = new Thread(GetNotReestr) {IsBackground = true};
            th.Start();
        }

        private void GetNotReestr()
        {
            try
            {
                ListNotReestr.Clear();
                ProgressBarTool.Dispatcher.Invoke(() =>
                {
                    TextBlockTool.Text = @"Запрос не подавших...";
                    ProgressBarTool.Value = 0;
                    ProgressBarTool.Maximum = 1;
                    buttonUpdate.IsEnabled = false;
                });
                ListNotReestr.AddRange(V_NOT_REESTR_MEDSERV_row.Get(wcf.GetNotReestr().Select()));
                ProgressBarTool.Dispatcher.Invoke(() => { ProgressBarTool.Value = 1; });
            }
            catch (Exception ex)
            {
                ProgressBarTool.Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally
            {
                ProgressBarTool.Dispatcher.Invoke(() =>
                {
                    ProgressBarTool.Value = 0;
                    buttonUpdate.IsEnabled = true;
                    TextBlockTool.Text = @"Завершено";
                    CVSNotReestr.View.Refresh();
                });
            }
        }

        private SaveFileDialog sfd = new SaveFileDialog() {Filter = "Файлы Excel(*.xlsx)|*.xlsx"};

        private void buttonToExcel_Click(object sender, RoutedEventArgs e)
        {
            sfd.FileName = "Не подавшие";
            if (sfd.ShowDialog() == true)
            {
                NotReestrXLS(sfd.FileName);
            }
        }

        private void NotReestrXLS(string path)
        {
            try
            {
                var efm = new ExcelOpenXML(path, "Не подавшие");

                var style1 = efm.CreateType(new FontOpenXML() {HorizontalAlignment = HorizontalAlignmentV.Left}, new BorderOpenXML(), null);
                var style2 = efm.CreateType(new FontOpenXML() {HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Bold = true}, new BorderOpenXML(), null);
                var style3 = efm.CreateType(new FontOpenXML() {HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Bold = true}, null, null);


                uint indexRow = 1;
                var MRow = efm.GetRow(indexRow);

                efm.PrintCell(MRow, 1, "МО не подавшие реестры", style3);
                efm.AddMergedRegion(new CellRangeAddress(indexRow, 1, indexRow, 2));
                efm.SetRowHeigth(indexRow,35);

                indexRow++;

                MRow = efm.GetRow(indexRow);
                efm.PrintCell(MRow, 1, "Код МО", style2);
                efm.PrintCell(MRow, 2, "Наименование МО", style2);
                indexRow++;
                foreach (var row in ListNotReestr)
                {
                    MRow = efm.GetRow(indexRow);
                    efm.PrintCell(MRow, 1, row.CODE_MO, style1);
                    efm.PrintCell(MRow, 2, row.NAME, style1);
                    indexRow++;
                }


                efm.AutoSizeColumns(1, 2);
                efm.Save();
                if (MessageBox.Show($@"Выполнено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    ShowSelectedInExplorer.FileOrFolder(path);
                }
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
        }

    }

    public class V_NOT_REESTR_MEDSERV_row
    {
        public static List<V_NOT_REESTR_MEDSERV_row> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static V_NOT_REESTR_MEDSERV_row Get(DataRow row)
        {
            try
            {
                return  new V_NOT_REESTR_MEDSERV_row
                {
                    CODE_MO = row["Код МО"].ToString(),
                    NAME = row["Наименование"].ToString()
                };
            }
            catch (Exception ex)
            {
               throw new Exception($"Ошибка получения V_NOT_REESTR_MEDSERV_row: {ex.Message}");
            }
        }
        public string CODE_MO { get; set; }
        public string NAME { get; set; }

    }
}

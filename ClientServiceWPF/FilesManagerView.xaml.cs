using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using System.Windows.Shapes;
using ClientServiceWPF.Class;
using ExcelManager;
using ServiceLoaderMedpomData;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для FilesManagerView.xaml
    /// </summary>
    public partial class FilesManagerView : Window
    {
        IWcfInterface wcf => LoginForm.wcf;


        public BindingList<FilePacket> List { get; set; } = new BindingList<FilePacket>();

        private CollectionViewSource CVSFiles;

        public FilesManagerView(bool Activ)
        {
            InitializeComponent();
            labelUPDATE.Visibility = Visibility.Hidden;
            CVSFiles = (CollectionViewSource) this.FindResource("CVSFiles");
            SetControlForm(LoginForm.SecureCard, Activ);
        }
        void SetControlForm(List<string> card, bool activ)
        {
            buttonPriory.IsEnabled = card.Contains(nameof(IWcfInterface.SetPriority));
            buttonClear.IsEnabled = card.Contains(nameof(IWcfInterface.ClearFileManagerList)) && !activ;
            buttonDeletePack.IsEnabled = card.Contains(nameof(IWcfInterface.DelPack));
            buttonSaveToArc.IsEnabled = card.Contains(nameof(IWcfInterface.SaveProcessArch));
            MenuItemRepeat.IsEnabled = card.Contains(nameof(IWcfInterface.RepeatClosePac));
            MenuItemBreakTimeout.IsEnabled = card.Contains(nameof(IWcfInterface.StopTimeAway));
        }



        private void buttonFindCODE_MO_Click(object sender, RoutedEventArgs e)
        {
            var CODE_MO = textBoxFindCODE_MO.Text.ToUpper();
            foreach (var fp in List)
            {
                if (fp.CodeMO == CODE_MO)
                {
                    SetCurrent(fp);
                    return;
                }
            }
        }

        private void TextBoxFindCODE_MO_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonFindCODE_MO_Click(buttonFindCODE_MO, new RoutedEventArgs());
            }
        }


   



        private FindPredicate<FilePacket, string> FindPRED { get; set; } = new FindPredicate<FilePacket, string>();



        private void buttonFindMO_NAME_Click(object sender, RoutedEventArgs e)
        {
            var name = textBoxFindMO_NAME.Text.ToUpper();
            if (FindPRED.FindValue != name)
            {
                FindPRED.Clear();
                FindPRED.FindValue = name;
                FindPRED.AddVariant(List.Where(x => x.CaptionMO.ToUpper().IndexOf(name, StringComparison.Ordinal) != -1));
            }

            var FindValue = FindPRED.Next();
            if (FindValue != null)
            {
                SetCurrent(FindValue);
            }
            else
            {
                FindPRED.Clear();
                MessageBox.Show("Поиск достиг конца списка");
            }

        }

        public void SetCurrent(FilePacket item)
        {
            dataGrid.SelectedCells.Clear();
            CVSFiles.View.MoveCurrentTo(item);
            dataGrid.ScrollIntoView(item);
        }

        private bool CancelUpdateList;

        public void UpdateList()
        {
            if (!CancelUpdateList)
            {
                var th = new Thread(UpdateListThread) {IsBackground = true};
                th.Start();
            }
        }

        private readonly object ProgressThread  = new object();
        private void UpdateListThread()
        {
            lock (ProgressThread)
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            labelUPDATE.Content = @"Запрос данных...";
                            labelUPDATE.Visibility = Visibility.Visible;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });

                    var listtmp = wcf.GetFileManagerList();
                    //Thread.Sleep(30000);
                    this.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            labelUPDATE.Content = @"Обновление списка...";
                            RefreshDate(List, listtmp);
                            labelUPDATE.Visibility = Visibility.Hidden;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    });

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }



        private void RefreshDate(BindingList<FilePacket> mainlist, List<FilePacket> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (i < mainlist.Count)
                    mainlist[i].CopyFrom(list[i]);
                else
                    mainlist.Add(list[i]);
            }

            for (var i = list.Count; i < mainlist.Count; i++)
            {
                mainlist.RemoveAt(i);
            }

            if (list.Count == 0)
                mainlist.Clear();
        }


        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private bool isClosed = false;
        private Thread CloseConnectTh;
        private void FilesManagerView_OnClosing(object sender, CancelEventArgs e)
        {
            try
            {
                if (!isClosed)
                {
                    e.Cancel = true;
                    if (CloseConnectTh==null || CloseConnectTh.IsAlive == false)
                    {
                        CloseConnectTh = new Thread(CloseConnect) { IsBackground = true };
                        CloseConnectTh.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CloseConnect()
        {
            try
            {
                CancelUpdateList = true;
                lock (ProgressThread)
                {
                    wcf.UnRegisterNewFileManager();
                }
                isClosed = true;
                Dispatcher.Invoke(() => { this.Close(); });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        private void TextBoxFindMO_NAME_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && TextBoxFindMO_NAMEKeyPress)
            {
                buttonFindMO_NAME_Click(buttonFindMO_NAME, new RoutedEventArgs());
            }

            TextBoxFindMO_NAMEKeyPress = false;
        }

        private bool TextBoxFindMO_NAMEKeyPress = false;
        private void TextBoxFindMO_NAME_OnKeyDown(object sender, KeyEventArgs e)
        {
            TextBoxFindMO_NAMEKeyPress = true;
        }

        private void FilesManagerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdateList();
            wcf.RegisterNewFileManager();
        }
        private SaveFileDialog sdf = new SaveFileDialog { Filter = @"Файлы Excel(*.xlsx)|*.xlsx" };
        private void buttonXLS_Click(object sender, RoutedEventArgs e)
        {
            sdf.FileName = $"Прием реестров от {DateTime.Now:dd.MM.yyyy HH mm}";
            if (sdf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    PrintExcel(List, sdf.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        void PrintExcel(BindingList<FilePacket> mainlist, string path)
        {
            try
            {
                var sort_list = mainlist.OrderBy(FilePacket => FilePacket.CodeMO);

                using (var xls = new ExcelOpenXML(path, "Прием реестров"))
                {
                    var StyleTop = xls.CreateType(new FontOpenXML { Bold = true, wordwrap = true, HorizontalAlignment = HorizontalAlignmentV.Center }, new BorderOpenXML(), null);
                    var MRow = xls.GetRow(1);
                    xls.PrintCell(MRow, 1, "№", StyleTop);
                    xls.PrintCell(MRow, 2, "Код МО", StyleTop);
                    xls.PrintCell(MRow, 3, "Наименование МО", StyleTop);
                    xls.PrintCell(MRow, 4, "Состав файлов", StyleTop);
                    xls.PrintCell(MRow, 5, "Файлы", StyleTop);
                    xls.PrintCell(MRow, 6, "Результат", StyleTop);
                    xls.PrintCell(MRow, 7, "Комментарий", StyleTop);
                    xls.PrintCell(MRow, 8, "Статус", StyleTop);
                    uint Rowindex = 2;
                    var StyleValue = xls.CreateType(new FontOpenXML { Bold = false, wordwrap = true, HorizontalAlignment = HorizontalAlignmentV.Center }, new BorderOpenXML(), null);
                    var StyleErr = xls.CreateType(new FontOpenXML { Bold = false, wordwrap = true, HorizontalAlignment = HorizontalAlignmentV.Center }, new BorderOpenXML(), new FillOpenXML { color = Colors.Red });
                    var StyleLeft = xls.CreateType(new FontOpenXML { Bold = false, wordwrap = false, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), null);
                    var StyleCenter = xls.CreateType(new FontOpenXML { Bold = false, wordwrap = false, HorizontalAlignment = HorizontalAlignmentV.Center }, new BorderOpenXML(), null);
                    var index = 1;
                    xls.SetColumnWidth(7, 55);
                    foreach (var pack in sort_list)
                    {
                        var countDD = 0;
                        var countDF = 0;
                        var countDO = 0;
                        var countDP = 0;
                        var countDR = 0;
                        var countDS = 0;
                        var countDU = 0;
                        var countDV = 0;
                        var countH = 0;
                        var countT = 0;
                        var countC = 0;
                        var CurrRow = Rowindex;
                        foreach (var fi in pack.Files)
                        {
                            MRow = xls.GetRow(Rowindex);

                            switch (fi.Type)
                            {
                                case FileType.DD:
                                    countDD++;
                                    break;
                                case FileType.DF:
                                    countDF++;
                                    break;
                                case FileType.DO:
                                    countDO++;
                                    break;
                                case FileType.DP:
                                    countDP++;
                                    break;
                                case FileType.DR:
                                    countDR++;
                                    break;
                                case FileType.DS:
                                    countDS++;
                                    break;
                                case FileType.DU:
                                    countDU++;
                                    break;
                                case FileType.DV:
                                    countDV++;
                                    break;
                                case FileType.H:
                                    countH++;
                                    break;
                                case FileType.T:
                                    countT++;
                                    break;
                                case FileType.C:
                                    countC++;
                                    break;
                            }



                            var Style = StyleValue;
                            if (fi.Process == StepsProcess.ErrorXMLxsd || fi.Process == StepsProcess.FlkErr || fi.Process == StepsProcess.Invite || fi.Process == StepsProcess.NotInvite)
                                Style = StyleErr;
                            xls.PrintCell(MRow, 1, index.ToString(), StyleCenter);
                            xls.PrintCell(MRow, 2, pack.CodeMO, StyleCenter);
                            xls.PrintCell(MRow, 3, pack.CaptionMO, StyleLeft);
                            xls.PrintCell(MRow, 4, "", StyleValue);
                            xls.PrintCell(MRow, 5, fi.FileName, Style);
                            xls.PrintCell(MRow, 6, fi.Process.ToString(), Style);
                            xls.PrintCell(MRow, 7, FilesItemStatus(fi.Process), Style);
                            xls.PrintCell(MRow, 8, fi.Comment, Style);
                            Rowindex++;
                        }
                        var tmp = "";
                        if (countH != 0)
                            tmp += "H(" + countH + ");";
                        if (countDD != 0)
                            tmp += "DD(" + countDD + ");";
                        if (countDF != 0)
                            tmp += "DF(" + countDF + ");";
                        if (countDO != 0)
                            tmp += "DO(" + countDO + ");";
                        if (countDP != 0)
                            tmp += "DP(" + countDP + ");";
                        if (countDR != 0)
                            tmp += "DR(" + countDR + ");";
                        if (countDS != 0)
                            tmp += "DS(" + countDS + ");";
                        if (countDU != 0)
                            tmp += "DU(" + countDU + ");";
                        if (countDV != 0)
                            tmp += "DV(" + countDV + ");";
                        if (countT != 0)
                            tmp += "T(" + countT + ");";
                        if (countC != 0)
                            tmp += "C(" + countC + ");";


                        xls.PrintCell(CurrRow, 4, tmp, countH != 0 ? StyleValue : StyleErr);
                        index++;
                    }
                    xls.AutoSizeColumns(1, 6);
                    xls.SetColumnWidth(7, 55);
                    xls.SetColumnWidth(8, 55);


                    xls.PrintCell(Rowindex, 2, "", StyleValue);
                    xls.PrintCell(Rowindex, 3, "", StyleValue);
                    xls.PrintCell(Rowindex, 1, $"Всего: {mainlist.Count} пакетов", StyleValue);
                    xls.AddMergedRegion(new CellRangeAddress(Rowindex, 1, Rowindex, 3));

                    xls.Save();
                    if (MessageBox.Show($@"Выполнено. Показать файл?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(path);
                    }
                }
                    
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.FullError());
            }
        }

        string FilesItemStatus(StepsProcess value)
        {
            switch (value)
            {
                case StepsProcess.NotInvite: return "Файл не принят. Файл не корректен";
                case StepsProcess.Invite: return "Файл не принят. Файл корректен";
                case StepsProcess.ErrorXMLxsd: return "Файл не принят. Ошибка схемы документа";
                case StepsProcess.XMLxsd: return "Файл принят. Схема документа пройдена";
                case StepsProcess.FlkErr: return "Файл принят. Файл содержит ошибки ФЛК";
                case StepsProcess.FlkOk: return "Файл принят.";
                default: return "Неизвестно";
            }

        }

        private List<FilePacket> SelectedFilePacket => dataGrid.SelectedCells.Select(x =>(FilePacket) x.Item).Distinct().ToList(); 

        private void buttonPriory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = SelectedFilePacket;
                if (selected.Count != 0)
                {
                    foreach (var item in selected)
                    {
                        wcf.SetPriority(item.guid, Convert.ToInt32(textBoxPriory.Text));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }


        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show($@"Очистка пакетов приведет к отмене текущих операций вы уверены?{Environment.NewLine}Это также приведет к удалению всех связанных файлов", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                wcf.ClearFileManagerList();
            }
        }

        private void buttonDeletePack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = SelectedFilePacket;
                if (selected.Count != 0)
                {
                    if (MessageBox.Show($@"Вы уверены что хотите удалить пакет?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        wcf.DelPack(selected.First().guid);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonSaveToArc_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wcf.SaveProcessArch();

                var form = new ProgressForm();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemRepeat_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = SelectedFilePacket;
                if (selected.Count != 0)
                {
                    if (MessageBox.Show(selected.Count != 1? $"Вы уверены что хотите повторить проверку {selected.Count} пакетов?" : "Вы уверены что хотите повторить проверку пакета?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var index = selected.Select(x => x.guid).ToArray();
                        wcf.RepeatClosePac(index);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemBreakTimeout_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = SelectedFilePacket;
                if (selected.Count != 0)
                {
                    if (MessageBox.Show(selected.Count != 1 ? $"Вы уверены что хотите закончить ожидание {selected.Count} пакетов?" : "Вы уверены что хотите закончить ожидание пакета?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        foreach (var t in selected)
                        {
                            wcf.StopTimeAway(List.IndexOf(t));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemBreakProcess_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = SelectedFilePacket;
                if (selected.Count != 0)
                {
                    if (MessageBox.Show(selected.Count != 1 ? $"Вы уверены что хотите прервать обработку {selected.Count} пакетов?" : "Вы уверены что хотите прервать обработку пакета?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        foreach (var t in selected)
                        {
                            wcf.BreakProcessPac(t.guid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemView_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = SelectedFilePacket;
                if (selected.Count != 0)
                {
                    var form = new ShowFileItem(selected[0]);
                    form.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

         
        }

        private void EventSetter_OnHandler(object sender, MouseButtonEventArgs e)
        {
            MenuItemView_OnClick(MenuItemView, new RoutedEventArgs());
        }
    }
}

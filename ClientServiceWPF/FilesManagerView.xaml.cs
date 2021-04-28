using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ExcelManager;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using DataGrid = System.Windows.Controls.DataGrid;
using MessageBox = System.Windows.MessageBox;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для FilesManagerView.xaml
    /// </summary>
    public partial class FilesManagerView : Window, INotifyPropertyChanged
    {
        public  FilesManagerViewVM VM { get; set; } = new FilesManagerViewVM(LoginForm.wcf,  new ExcelFilePacket());
        public FilesManagerView(bool isActive)
        {
            InitializeComponent();
            VM.SetRight(LoginForm.SecureCard, isActive);
        }
        /// <summary>
        /// Действие при уведомлении сервера об обновлении
        /// </summary>
        public void UpdateList()
        {
            VM.UpdateListCommand.Execute(null);
        }
       
        private void FilesManagerView_OnClosing(object sender, CancelEventArgs e)
        {
           VM.OnClose();
        }
     
        private void FilesManagerView_OnLoaded(object sender, RoutedEventArgs e)
        {
            VM.OnLoad();
        }


        public ICommand FocusElementCommand { get; set; }= new Command(o => { (o as UIElement)?.Focus(); });
      

        public List<FilePacket> SelectedFilePacket => dataGrid.SelectedCells.Select(x =>(FilePacket) x.Item).Distinct().ToList(); 
        private void DataGridRow_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           VM.ShowDetailCommand.Execute(SelectedFilePacket);
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dg = sender as DataGrid;
            if (dg?.SelectedItem != null)
            {
                dg.ScrollIntoView(dg.SelectedItem);
            }
        }

        
        private void DataGrid_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedFilePacket));
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


    public class FilesManagerViewVM:INotifyPropertyChanged
    {
        private IExcelFilePacket xlsFilePacket;

        Dispatcher dispatcher;
        private IWcfInterface wcf;

        private FilePacket _SelectItem;
        public FilePacket SelectItem
        {
            get { return _SelectItem; }
            set { _SelectItem = value; OnPropertyChanged(); }
        }


        private string _StatusOperation;
        public string StatusOperation
        {
            get { return _StatusOperation; }
            set { _StatusOperation = value; OnPropertyChanged(); }
        }

        public ObservableCollection<FilePacket> List { get; set; } = new ObservableCollection<FilePacket>();
        private List<string> _WcfRight = new List<string>();
        private List<string> WcfRight
        {
            get { return _WcfRight;}
            set { _WcfRight = value; OnPropertyChanged(); }
        }
        private bool _isActive;
        private bool isActive
        {
            get { return _isActive; }
            set { _isActive = value; OnPropertyChanged(); }
        }
        public FilesManagerViewVM(IWcfInterface wcf, IExcelFilePacket xlsFilePacket)
        {
            dispatcher = Dispatcher.CurrentDispatcher;
            this.wcf = wcf;
            this.xlsFilePacket = xlsFilePacket;
        
        }
        public void SetRight(List<string> WcfRight, bool isActive)
        {
            this.WcfRight = WcfRight;
            this.isActive = isActive;
        }

        public void OnLoad()
        {
            wcf.RegisterNewFileManager();
            UpdateList();
        }


        public void OnClose()
        {
            try
            {
                Task.Run(() =>
                {
                    CancelUpdateList = true;
                    lock (ProgressThread)
                    {
                        wcf.UnRegisterNewFileManager();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        public ICommand UpdateListCommand=> new Command(o => { UpdateList(); });



        private bool CancelUpdateList;

        public void UpdateList()
        {
            if (!CancelUpdateList)
            {
                Task.Run(() => { UpdateListThread(); });
            }
        }
        private readonly object ProgressThread = new object();
        private void UpdateListThread()
        {
            lock (ProgressThread)
            {
                try
                {
                    dispatcher.Invoke(() =>{StatusOperation = @"Запрос данных...";});
                    var listtmp = wcf.GetFileManagerList();
                    dispatcher.Invoke(() =>
                    {
                        StatusOperation = @"Обновление списка...";
                        RefreshDate(listtmp);
                      
                        StatusOperation = "";
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void RefreshDate(List<FilePacket> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (i < List.Count)
                    List[i].CopyFrom(list[i]);
                else
                    List.Add(list[i]);
            }

            for (var i = list.Count; i < List.Count; i++)
            {
                List.RemoveAt(i);
            }

            if (list.Count == 0)
                List.Clear();
        }


        public ICommand FindCodeMOCommand => new Command(o =>
        {
            var CODE_MO = o.ToString().ToUpper();
            foreach (var fp in List)
            {
                if (fp.CodeMO == CODE_MO)
                {
                    SelectItem = fp;
                }
            }
        });
        private FindPredicate<FilePacket, string> FindPRED { get; set; } = new FindPredicate<FilePacket, string>();
        public ICommand FindNAM_MOKCommand => new Command(o =>
        {
            var name = o.ToString().ToUpper();
            if (FindPRED.FindValue != name)
            {
                FindPRED.Clear();
                FindPRED.FindValue = name;
                FindPRED.AddVariant(List.Where(x => x.CaptionMO.ToUpper().IndexOf(name, StringComparison.Ordinal) != -1));
            }

            var FindValue = FindPRED.Next();
            if (FindValue != null)
            {
                SelectItem = FindValue;
            }
            else
            {
                FindPRED.Clear();
                MessageBox.Show("Поиск достиг конца списка");
            }

        });

       

        public ICommand SetPriorCommand => new Command(o =>
        {
            try
            {
                var objects = (object[])o;
                var prior = (string)objects[0];
                var items = (List<FilePacket>)objects[1];
                if (items.Count != 0)
                {
                    foreach (var item in items)
                    {
                        wcf.SetPriority(item.guid, Convert.ToInt32(prior));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }, o => WcfRight.Contains(nameof(IWcfInterface.SetPriority)));
        public ICommand ToArchiveCommand => new Command(o =>
        {
            try
            {
                wcf.SaveProcessArch();
                var form = new ProgressForm();
                form.ShowDialog();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }, o => WcfRight.Contains(nameof(IWcfInterface.SaveProcessArch)));

        public ICommand ClearCommand => new Command(o =>
        {
            try
            {
                if (MessageBox.Show($@"Очистка пакетов приведет к отмене текущих операций вы уверены?{Environment.NewLine}Это также приведет к удалению всех связанных файлов", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    wcf.ClearFileManagerList();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }, o => WcfRight.Contains(nameof(IWcfInterface.ClearFileManagerList)) && !isActive);


        public ICommand DeletePackCommand => new Command(o =>
        {
            try
            {
                var items = (List<FilePacket>)o;
                if (items.Count != 0)
                {
                    if (MessageBox.Show($@"Вы уверены что хотите удалить пакет(ы)?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        wcf.DelPack(items.First().guid);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }, o => WcfRight.Contains(nameof(IWcfInterface.DelPack)));

        SaveFileDialog sdf = new SaveFileDialog { Filter = @"Файлы Excel(*.xlsx)|*.xlsx" };

        public ICommand ToXLSCommand => new Command(o =>
        {
            try
            {
                sdf.FileName = $"Прием реестров от {DateTime.Now:dd.MM.yyyy HH mm}";
                if (sdf.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        xlsFilePacket.PrintExcel(List, sdf.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });


        public ICommand RepeatCheckCommand => new Command(o =>
        {
            try
            {
                var selected = (List<FilePacket>)o;
                if (selected.Count != 0)
                {
                    if (MessageBox.Show(selected.Count != 1 ? $"Вы уверены что хотите повторить проверку {selected.Count} пакетов?" : "Вы уверены что хотите повторить проверку пакета?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
        }, o => WcfRight.Contains(nameof(IWcfInterface.RepeatClosePac)));


        public ICommand BreakTimeoutCommand => new Command(o =>
        {
            try
            {
                var selected = (List<FilePacket>)o;
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
        }, o => WcfRight.Contains(nameof(IWcfInterface.StopTimeAway)));

        public ICommand BreakProcessCommand => new Command(o =>
        {
            try
            {
                var selected = (List<FilePacket>)o;
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
        }, o => WcfRight.Contains(nameof(IWcfInterface.BreakProcessPac)));

        public ICommand ShowDetailCommand => new Command(o =>
        {
            try
            {
                var selected = (List<FilePacket>)o;
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
        });


        #region  INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public interface IExcelFilePacket
    {
        void PrintExcel(IEnumerable<FilePacket> FilePackets, string path);
    }

    public class ExcelFilePacket : IExcelFilePacket
    {
       public void PrintExcel(IEnumerable<FilePacket> FilePackets, string path)
        {
            try
            {
                var sort_list = FilePackets.OrderBy(FilePacket => FilePacket.CodeMO).ToList();

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
                        var cec = new ENUM_COUNT<FileType>();
                        var CurrRow = Rowindex;
                    
                        foreach (var fi in pack.Files)
                        {
                            MRow = xls.GetRow(Rowindex);
                          
                            if(fi.Type.HasValue)
                                cec.AddCount(fi.Type.Value);
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

                        xls.PrintCell(CurrRow, 4, cec.GetString(null), cec[FileType.H] != 0 ? StyleValue : StyleErr);
                        index++;
                    }
                    xls.AutoSizeColumns(1, 6);
                    xls.SetColumnWidth(7, 55);
                    xls.SetColumnWidth(8, 55);


                    xls.PrintCell(Rowindex, 2, "", StyleValue);
                    xls.PrintCell(Rowindex, 3, "", StyleValue);
                    xls.PrintCell(Rowindex, 1, $"Всего: {sort_list.Count} пакетов", StyleValue);
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
        private class ENUM_COUNT<T>
        {
            Dictionary<T, int> CountDic = new Dictionary<T, int>();
            public ENUM_COUNT()
            {
                foreach (var value in (T[]) typeof(T).GetEnumValues())
                {
                    CountDic.Add(value, 0);
                }
            }

            public void AddCount(T val)
            {
                if (CountDic.ContainsKey(val))
                    CountDic[val]++;
            }

            public string GetString(params T[] filter)
            {
                return string.Join(",", CountDic.Where(x => (filter == null || filter.Contains(x.Key)) && x.Value!=0).Select(x => $"{x.Key.ToString()}({x.Value})"));
            }

            public int this[T val] => CountDic[val];

        }
    }


    public class ArrayParamConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.ToArray();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

  

}

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using ClientServiceWPF.Class;
using ServiceLoaderMedpomData;
using System.IO;
using System.IO.Compression;
using System.Windows.Forms;
using ExcelManager;
using ServiceLoaderMedpomData.EntityMP_V31;
using MessageBox = System.Windows.MessageBox;

namespace ClientServiceWPF.MEK_RESULT
{
    /// <summary>
    /// Логика взаимодействия для ExportFile.xaml
    /// </summary>
    public partial class ExportFile : Window
    {
        private CollectionViewSource CollectionViewSourceFiles;
        private CollectionViewSource CollectionViewSourceFilesLOG;
        public ExportFile()
        {
            InitializeComponent();
            CollectionViewSourceFiles = (CollectionViewSource) FindResource("CollectionViewSourceFiles");
            CollectionViewSourceFilesLOG = (CollectionViewSource)FindResource("CollectionViewSourceFilesLOG");
        }

        public List<EXPORTZGLV> Files { get; set; }= new List<EXPORTZGLV>();


        private bool IsTemp1 => radioButtonTEMP1.IsChecked == true;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DatePickerPERIOD.SelectedDate.HasValue)
            {
                var th = new Thread(Refresh) {IsBackground = true};
                var dt = DatePickerPERIOD.SelectedDate.Value;
                DatePickerXLS_START.SelectedDate = new DateTime(dt.Year, dt.Month,1);
                DatePickerXLS_END.SelectedDate = new DateTime(dt.Year, dt.Month, DateTime.DaysInMonth(dt.Year, dt.Month));
                th.Start( new RefreshThreadParam {dt = dt, IsTemp1 = IsTemp1 });
            }
            else
            {
                throw new Exception("Не выбран период");
            }
        }

        class RefreshThreadParam
        {
            public DateTime dt { get; set; }
            public bool IsTemp1 { get; set; }
        }
        private void Refresh(object obj)
        {
            try
            {
                this.Dispatcher.Invoke(() => {
                    StatusProgressBar1.IsIndeterminate = true;
                    ButtonRefresh.IsEnabled = false;
                });
                var par = (RefreshThreadParam)obj;
                var oda = new OracleDataAdapter($"select * from V_EXPORT_H_ZGLV{(par.IsTemp1?"_TEMP1":"")} t where t.month = {par.dt.Month}  and  t.year = {par.dt.Year}", new OracleConnection(AppConfig.Property.ConnectionString));
                var tbl = new DataTable();
                oda.Fill(tbl);
                Files.Clear();
                Files.AddRange(EXPORTZGLV.Get(tbl.Select()));
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally
            {
                this.Dispatcher.Invoke(() =>
                {
                    StatusProgressBar1.IsIndeterminate = false;
                    ButtonRefresh.IsEnabled = true;
                    CollectionViewSourceFiles?.View?.Refresh();
                });
            }
        }

        private void buttonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (Files.Count != 0)
            {
                var max = Files.Max(x => x.IsSelect);
                Files.ForEach(x => x.IsSelect = !max);
                CollectionViewSourceFiles.View.Refresh();
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            StartExportReestr();
        }

        class SUM_XLS_XML
        {
            public decimal SUM_XLS { get; set; }
            public decimal SUM_XML { get; set; }
        }

        class ParamThGetFilesToMOTh
        {
            public ParamThGetFilesToMOTh(List<EXPORTZGLV> List, int ThreadCount, string Path, bool IsTemp1, bool IsSMO,DateTime D_START_XLS, DateTime D_END_XLS)
            {
                this.List = List;
                this.ThreadCount = ThreadCount;
                this.Path = Path;
                this.IsTemp1 = IsTemp1;
                this.IsSMO = IsSMO;
                this.D_END_XLS = D_END_XLS;
                this.D_START_XLS = D_START_XLS;
                this.SUM = new Dictionary<string, SUM_XLS_XML>();
            }
            public List<EXPORTZGLV> List { get; set; }
            public  int ThreadCount { get; set; }
            public string Path { get; set; }
            public bool IsTemp1 { get; set; }
            public bool IsSMO { get; set; }
            public bool IsEndFile { get; set; }
            public bool isEndXLS { get; set; }
            public Dictionary<string, SUM_XLS_XML> SUM { get; set; } 
            public DateTime D_START_XLS { get; set; }
            public DateTime D_END_XLS { get; set; }
        }

        FolderBrowserDialog fbd = new FolderBrowserDialog();

        public void StartExportReestr()
        {
            try
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    buttonSave.IsEnabled = false;
                    var selected = Files.Where(x => x.IsSelect).ToList();
                    
                    var isSMO = radioButtonToSMO.IsChecked == true;
                    DateTime START_XLS = DateTime.Now;
                    DateTime END_XLS = DateTime.Now;
                    if (isSMO)
                    {
                        if (DatePickerXLS_START.SelectedDate.HasValue && DatePickerXLS_END.SelectedDate.HasValue)
                        {
                            START_XLS = DatePickerXLS_START.SelectedDate.Value;
                            END_XLS = DatePickerXLS_END.SelectedDate.Value;
                        }
                        else
                        {
                            throw  new Exception("Укажите даты в файлах Excel");
                        }
                    }
                    selected.ForEach(x=>x.Finish = false);
                    var par = new ParamThGetFilesToMOTh(selected, Convert.ToInt32(textBoxThreadCount.Text),fbd.SelectedPath, IsTemp1, isSMO, START_XLS, END_XLS);
                    if (par.ThreadCount <= 0 || par.ThreadCount > 10)
                        throw new Exception("Количество потоков должно быть от 1 до 10");
                    if (par.List.Count == 0)
                        throw new Exception("Не выбрано не одного файла");

                    
                    var th = new Thread(GetFiles) { IsBackground = true };
                    th.Start(par);
                    if (par.IsSMO)
                    {
                        var thXLSX = new Thread(ExcelThread) {IsBackground = true};
                        thXLSX.Start(par);
                    }
                    else
                    {
                        par.isEndXLS = true;
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Ошибка при подготовке: {ex.Message}");
            }
        }

        class F002
        {
            public static List<F002> Get(IEnumerable<DataRow> rows)
            {
                return rows.Select(Get).ToList();
            }
             static F002 Get(DataRow row)
            {
                try
                {
                    var item = new F002();
                    item.SMOCOD = row["SMOCOD"].ToString();
                    return item;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получения F002: {ex.Message}",ex);
                }
            }
            public string SMOCOD { get; set; }

        }
        class XLS_TABLE
        {

            public static List<XLS_TABLE> Get(IEnumerable<DataRow> rows)
            {
                return rows.Select(Get).ToList();
            }
            static XLS_TABLE Get(DataRow row)
            {
                try
                {
                    var item = new XLS_TABLE();
                    item.FIO = row["FIO"].ToString();
                    item.W = row["W"].ToString();
                    item.POLIS = row["POLIS"].ToString();
                    item.DOC = row["DOC"].ToString();
                    item.SNILS = row["SNILS"].ToString();
                    item.DR = row["DR"].ToString();
                    if (row["VIDPOM"] !=DBNull.Value)
                        item.VIDPOM = Convert.ToInt32(row["VIDPOM"]);
                    item.DATE_IN = row["DATE_IN"].ToString();
                    item.DATE_OUT = row["DATE_OUT"].ToString();
                    if (row["PROFIL"] != DBNull.Value)
                        item.PROFIL = Convert.ToInt32(row["PROFIL"]);
                    if (row["PRVS"] != DBNull.Value)
                        item.PRVS = Convert.ToInt32(row["PRVS"]);
                    item.DS = row["DS"].ToString();
                    if (row["KOL_USL"] != DBNull.Value)
                        item.KOL_USL = Convert.ToDecimal(row["KOL_USL"]);
                    if (row["TARIF"] != DBNull.Value)
                        item.TARIF = Convert.ToDecimal(row["TARIF"]);
                    
                    item.SUMV_USL = Convert.ToDecimal(row["SUMV_USL"]);
                    if (row["IDCASE"] != DBNull.Value)
                        item.IDCASE = Convert.ToInt32(row["IDCASE"]);
                    item.USL_OK = Convert.ToInt32(row["USL_OK"]);
                    if (row["RSLT"] != DBNull.Value)
                        item.RSLT = Convert.ToInt32(row["RSLT"]);

                    return item;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получения XLS_TABLE: {ex.Message}", ex);
                }
            }
            public string FIO { get; set; }
            public string W { get; set; }
            public string POLIS { get; set; }
            public string DOC { get; set; }
            public string SNILS { get; set; }
            public string DR { get; set; }
            public int? VIDPOM { get; set; }
            public string DATE_IN { get; set; }
            public string DATE_OUT { get; set; }
            public int? PROFIL { get; set; }
            public int? PRVS { get; set; }
            public string DS { get; set; }
            public decimal? KOL_USL { get; set; }
            public decimal? TARIF { get; set; }
            public decimal SUMV_USL { get; set; }
            public int IDCASE { get; set; }
            public  int USL_OK { get; set; }
            public int? RSLT { get; set; }
        }
        private List<F002> GetF002()
        {
            var conn = new OracleConnection(AppConfig.Property.ConnectionString);
            StatusProgressBar1.Dispatcher.Invoke(() => { StatusTextBlock1.Text = "Запрос страховых организаций"; });
            var oda = new OracleDataAdapter($@"select t.* from NSI.F002 t where t.TF_OKATO = '76000' and sysdate between t.d_begin and  nvl(t.d_end,sysdate)", conn);
            var tbl = new DataTable();
            oda.Fill(tbl);
            return F002.Get(tbl.Select());
        }

        private void AddSum(ParamThGetFilesToMOTh param,string CODE_MO, decimal SUM, bool isXLS)
        {
            lock (param.SUM)
            {
                if (!param.SUM.ContainsKey(CODE_MO))
                {
                    param.SUM.Add(CODE_MO, new SUM_XLS_XML());
                }
                if(isXLS)
                    param.SUM[CODE_MO].SUM_XLS += SUM;
                else
                    param.SUM[CODE_MO].SUM_XML += SUM;
            }
        }

   

        void GetFiles(object obj)
        {
            var param = (ParamThGetFilesToMOTh)obj;
            try
            {
                StatusProgressBar1.Dispatcher.Invoke(() =>
                {
                    buttonSave.IsEnabled = false;
                    param.List.ForEach(x =>
                    {
                        x.Logs.Clear();
                        x.RaisePropertyChanged("Logs");
                    });
                });
              
                var tm = new TaskManager(param.ThreadCount);
                var index = 0;
                var count = param.List.Count;
               
                List<F002> smoList = null;
                if (param.IsSMO)
                {
                    StatusProgressBar1.Dispatcher.Invoke(() => { StatusTextBlock1.Text = "Запрос справочника F002"; });
                    smoList = GetF002();
                }

                foreach (var fi in param.List)
                {
                    try
                    {
                        fi.SUM = 0;
                        fi.PathArc.Clear();
                        TaskItem tmitem = null;
                        while (tmitem == null)
                        {
                            tmitem = tm.FreeItem;
                            if (tmitem == null)
                            {
                                Thread.Sleep(500);
                            }
                        }

                        tmitem.TSK = Task.Factory.StartNew(() =>
                        {
                            try
                            {
                                StatusProgressBar1.Dispatcher.Invoke(() => { fi.InWork = true; });
                               
                                if (param.IsSMO && smoList != null)
                                {
                                    foreach (var smo in smoList)
                                    {
                                        GetFile(fi, param.Path, param.IsTemp1, smo.SMOCOD);
                                    }
                                }
                                else
                                {
                                    GetFile(fi, param.Path, param.IsTemp1, null);
                                }
                                AddSum(param, fi.CODE_MO, fi.SUM ?? 0, false);

                                StatusProgressBar1.Dispatcher.Invoke(() =>
                                {
                                    fi.InWork = false;
                                    fi.Finish = true;
                                    fi.RaisePropertyChanged("IsTypeLog");
                                });
                            }
                            catch (Exception ex)
                            {
                                fi.AddLog(LogType.Error, $"Ошибка при выгрузке данных: {ex.Message}");
                                StatusProgressBar1.Dispatcher.Invoke(() =>
                                {
                                    fi.InWork = false;
                                    fi.Finish = true;
                                    fi.RaisePropertyChanged("IsTypeLog");
                                });
                            }

                        });
                        tmitem.Free = false;

                        StatusProgressBar1.Dispatcher.Invoke(() =>
                        {
                            StatusTextBlock1.Text = $"Выгрузка {index} из {count}";
                            StatusProgressBar1.Maximum = count;
                            StatusProgressBar1.Value = index;
                        });
                    }
                    catch (Exception ex)
                    {
                        StatusProgressBar1.Dispatcher.Invoke(() => { fi.AddLog(LogType.Error, $"Ошибка {ex.Source}: {ex.FullError()}"); });
                    }

                    index++;
                }

                StatusProgressBar1.Dispatcher.Invoke(() =>
                {
                    StatusTextBlock1.Text = "Ожидание завершения потоков";
                    StatusProgressBar1.IsIndeterminate = true;
                });

                while (!tm.IsSTOP)
                {
                    Thread.Sleep(500);
                }

                StatusProgressBar1.Dispatcher.Invoke(() => { StatusProgressBar1.IsIndeterminate = false; });

                if (!param.IsSMO)
                {
                    var GLIST = param.List.Where(x => x.PathArc.Count!=0).GroupBy(x => new { x.YEAR, x.MONTH, x.CODE_MO });
                    var countGR = GLIST.Count();
                    var i = 0;
                    foreach (var gr in GLIST)
                    {
                        StatusProgressBar1.Dispatcher.Invoke(() =>
                        {
                            StatusTextBlock1.Text = $"Сбор файлов в архив: {gr.Key.CODE_MO} за {gr.Key.MONTH}.{gr.Key.YEAR}";
                            StatusProgressBar1.Maximum = countGR;
                            StatusProgressBar1.Value = i;
                        });
                        var NAME_ARC = Path.Combine(param.Path, $"Результаты МЭК {gr.Key.CODE_MO} за {gr.Key.MONTH:D2}.{gr.Key.YEAR}.ZIP");
                        using (var archive = ZipFile.Open(NAME_ARC, ZipArchiveMode.Create))
                        {
                            foreach (var item in gr)
                            {
                                foreach (var file in item.PathArc)
                                {
                                    StatusProgressBar1.Dispatcher.Invoke(() => { StatusTextBlock2.Text = $"Добавление {file}"; });
                                    archive.CreateEntryFromFile(file, Path.GetFileName(file));
                                    File.Delete(file);
                                }
                            }
                        }
                        i++;
                    }
                }

                param.IsEndFile = true;
            }
            catch (Exception ex)
            {
                StatusProgressBar2.Dispatcher.Invoke(() =>
                {
                    MessageBox.Show(ex.Message);
                });
            }
            finally
            {
                EndProcces(param);
            }

        }
      
        void ExcelThread(object obj)
        {

            var param = (ParamThGetFilesToMOTh)obj;
            try
            {
                List<F002> smoList = null;
                smoList = GetF002();
                var list = param.List.GroupBy(x => x.CODE_MO).OrderBy(x => x.Key);

                foreach (var pack in list)
                {
                    var sumxls = CreateExcel(param, pack.ToList(), smoList, param.Path);
                    AddSum(param, pack.Key, sumxls, true);
                }

                StatusProgressBar2.Dispatcher.Invoke(() =>
                {
                    StatusProgressBar2.Value = 0;
                    StatusTextBlock3.Text = StatusTextBlock2.Text = "";
                });
                param.isEndXLS = true;
            }
            catch (Exception ex)
            {
                StatusProgressBar2.Dispatcher.Invoke(() => { MessageBox.Show(ex.Message); });
            }
            finally
            {
                EndProcces(param);
            }
        }


       

     
        void EndProcces(ParamThGetFilesToMOTh par)
        {
           if (par.isEndXLS && par.IsEndFile)
           {
               var Err = new List<string>();
                if (par.IsSMO)
                {
                  
                    var err_count = par.List.Count(x => (x.IsTypeLog ?? LogType.Info) != LogType.Info);
                    if (err_count != 0)
                        Err.Add($@"Выгрузка содержит {err_count} ошибочных файлов");

                    foreach (var t in par.SUM)
                    {
                        var SUM_XML = Math.Round(t.Value.SUM_XML, 2);
                        var SUM_XLS = Math.Round(t.Value.SUM_XLS, 2);
                        if (SUM_XML != SUM_XLS)
                        {
                            Err.Add($@"Ошибка контроля сумм XML и XLS для МО = {t.Key} сумма XML ={SUM_XML} сумма XLS = {SUM_XLS}");
                        }
                    }
                }
                StatusProgressBar1.Dispatcher.Invoke(() =>
                {
                    buttonSave.IsEnabled = true;
                    StatusTextBlock1.Text = StatusTextBlock2.Text = "";
                    StatusProgressBar2.Maximum = StatusProgressBar1.Maximum = 1;
                    StatusProgressBar2.Value = StatusProgressBar1.Value = 0;

                    if (CustomMessageBox.Show($"Завершено. Показать файлы?{(Err.Count!=0? $"{Environment.NewLine}{string.Join(Environment.NewLine, Err.Select(x=>x))}":"")}", "") == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(par.Path);
                    }
                    
                });

                 
           }
        }


        decimal CreateExcel(ParamThGetFilesToMOTh param,List<EXPORTZGLV> List, List<F002> smoList, string path)
        {
            //Формируем EXCEL
            try
            {
                var CODE_MO = List.First().CODE_MO;
                var CurrentYear = List.First().YEAR;
                var CurrentMonth = List.First().MONTH;
                StatusProgressBar1.Dispatcher.Invoke(() => { StatusTextBlock2.Text = $"Формирование EXCEL: {CODE_MO}"; });
                decimal SUM_IN_XLS = 0;
                using (var conn = new OracleConnection(AppConfig.Property.ConnectionString))
                {
                    var oda1 = new OracleDataAdapter($@"select NAM_MOK ||' ОГРН: '|| ogrn NAME from nsi.f003 t  where t.mcod = '{CODE_MO}'", conn);
                    var tbl1 = new DataTable();
                    oda1.Fill(tbl1);
                    var MO = tbl1.Rows[0]["Name"].ToString();
                   
                    foreach (var strah in smoList)
                    {
                        StatusProgressBar1.Dispatcher.Invoke(() => { StatusTextBlock3.Text = $"Запрос данных для: {strah.SMOCOD}"; });
                        var oda = new OracleDataAdapter($"select * from v_export_excel_from where h_zglv_id in ({string.Join(",", List.Select(x => x.ZGLV_ID))}) and smo = '{strah.SMOCOD}'", conn);
                        var tbl = new DataTable();
                        oda.Fill(tbl);
                        var ExcelTable = XLS_TABLE.Get(tbl.Select());
                        var EXL_PATH = Path.Combine(path, "XLS", $"{CODE_MO} Реестр за {new DateTime(CurrentYear, CurrentMonth, 1):MMMMMMMMMMM yyyy} для подписи в {strah.SMOCOD}.XLSX");
                        if (!Directory.Exists(Path.GetDirectoryName(EXL_PATH)))
                            Directory.CreateDirectory(Path.GetDirectoryName(EXL_PATH));
                        if (ExcelTable.Count != 0)
                            SUM_IN_XLS += PrintExcel(ExcelTable, param.D_START_XLS, param.D_END_XLS,  strah.SMOCOD, MO, EXL_PATH);
                    }
                }


                StatusProgressBar1.Dispatcher.Invoke(() => {
                    StatusTextBlock2.Text = "Формирование EXCEL завершено";
                    StatusTextBlock3.Text = "";
                });
                return SUM_IN_XLS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Ошибка выгрузки XLS: {ex.Message}");
                return 0;
            }
        }


      

        decimal PrintExcel(List<XLS_TABLE> tbl, DateTime Start, DateTime End, string SMO, string MO, string path)
        {
            var Template = Path.Combine(LocalFolder, "TEMPLATE", "Template_Svod_Reestr.xlsx");
            File.Copy(Template, path);
            var filename = Path.GetFileName(path);

            using (var efm = new ExcelManager.ExcelOpenXML())
            {
                efm.OpenFile(path, 0);
                efm.PrintCell(2, 9, MO, null);
                efm.PrintCell(3, 9, SMO, null);
                efm.PrintCell(4, 1, $"за период с {Start:dd.MM.yyyy} по {End:dd.MM.yyyy}", null);
                var StyleLeftText = efm.CreateType(new FontOpenXML() {size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Left}, new BorderOpenXML(), null);
                var StyleCenterNumeric = efm.CreateType(new FontOpenXML() {size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Center, Format = (uint) DefaultNumFormat.F4}, new BorderOpenXML(), null);
             
                decimal usl_1 = 0;
                decimal usl_2 = 0;
                decimal usl_3 = 0;
                decimal usl_4 = 0;
                StatusProgressBar1.Dispatcher.Invoke(() =>
                {
                    StatusProgressBar2.Maximum = tbl.Count;
                    StatusProgressBar2.Value = 0;
                    StatusTextBlock3.Text = $@"0/{tbl.Count}";
                });


                var index = 0;
                uint currRows = 7;
                var countSheet = 1;

                var baseCOL = efm.Columns;
                var H1 = efm.GetRow(6);
                var H2 = efm.GetRow(7);
             

                foreach (var row in tbl)
                {
                    currRows++;
                    index++;
                    StatusProgressBar1.Dispatcher.Invoke(() =>
                    {
                        StatusProgressBar2.Value = index;
                        StatusTextBlock3.Text = $@"{index}/{tbl.Count}";
                    });


                    //Если больше 65535
                    if (currRows == 1048576)
                    {
                        throw  new Exception("Не проверенно 2 и более листа");
                        countSheet++;
                        efm.AddSheet($"Реестр {countSheet}");
                        foreach (var col in baseCOL)
                        {
                            efm.SetColumnWidth(col.Key, col.Value.Col.Width);
                        }

                        efm.CreateRow(1, H1.r.OuterXml);
                        efm.CreateRow(2, H2.r.OuterXml);
                        currRows = 3;
                    }


                    var MRow = efm.GetRow(currRows);

                    efm.PrintCell(MRow, 1, index, StyleLeftText);
                    efm.PrintCell(MRow, 2, row.FIO, StyleLeftText);
                    efm.PrintCell(MRow, 3, row.W, StyleLeftText);
                    efm.PrintCell(MRow, 4, row.POLIS, StyleLeftText);
                    efm.PrintCell(MRow, 5, row.DOC, StyleLeftText);
                    efm.PrintCell(MRow, 6, row.SNILS, StyleLeftText);
                    efm.PrintCell(MRow, 7, row.DR, StyleLeftText);
                    efm.PrintCell(MRow, 8, row.VIDPOM, StyleLeftText);
                    efm.PrintCell(MRow, 9, row.DATE_IN, StyleLeftText);
                    efm.PrintCell(MRow, 10, row.DATE_OUT, StyleLeftText);
                    efm.PrintCell(MRow, 11, row.PROFIL, StyleLeftText);
                    efm.PrintCell(MRow, 12, row.PRVS, StyleLeftText);
                    efm.PrintCell(MRow, 13, row.DS, StyleLeftText);
                    efm.PrintCell(MRow, 14, row.RSLT, StyleLeftText);
                    efm.PrintCell(MRow, 15, row.KOL_USL, StyleLeftText);
                        efm.PrintCell(MRow, 16,row.TARIF, StyleCenterNumeric);
                        efm.PrintCell(MRow, 17,row.SUMV_USL, StyleCenterNumeric);
                        efm.PrintCell(MRow, 18, row.IDCASE, StyleLeftText);
                  

                    switch (row.USL_OK)
                    {
                        case 1:
                            usl_1 += row.SUMV_USL;
                            break;
                        case 2:
                            usl_2 += row.SUMV_USL;
                            break;
                        case 3:
                            usl_3 += row.SUMV_USL;
                            break;
                        case 4:
                            usl_4 += row.SUMV_USL;
                            break;
                    }
                }

                currRows += 3;


                var style = efm.CreateType(new FontOpenXML() {size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Left}, null, null);
                var style1 = efm.CreateType(new FontOpenXML() {size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Center, Format = (uint) DefaultNumFormat.F4, Bold = true}, null, null);
                var style2 = efm.CreateType(new FontOpenXML() {size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Center, Format = (uint) DefaultNumFormat.F4}, null, null);


                efm.PrintCell(currRows, 1, "ИТОГО К ОПЛАТЕ:", style);
                efm.PrintCell(currRows, 3, Convert.ToDouble(usl_1 + usl_2 + usl_3 + usl_4), style1);
                efm.PrintCell(currRows, 5, RusCurrency.Str(Math.Round(Convert.ToDouble(usl_1 + usl_2 + usl_3 + usl_4), 2)), style);
                efm.AddMergedRegion(new CellRangeAddress(currRows, 1, currRows, 2));
                efm.AddMergedRegion(new CellRangeAddress(currRows, 3, currRows, 4));
                currRows++;
                efm.PrintCell(currRows, 1, "в том числе стационарная помощь:", style);
                efm.PrintCell(currRows, 3, Convert.ToDouble(usl_1), style2);
                efm.PrintCell(currRows, 5, RusCurrency.Str(Math.Round(Convert.ToDouble(usl_1), 2)), style);
                efm.AddMergedRegion(new CellRangeAddress(currRows, 1, currRows, 2));
                efm.AddMergedRegion(new CellRangeAddress(currRows, 3, currRows, 4));
                currRows++;
                efm.PrintCell(currRows, 1, "амбул.-поликлиническая помощь:", style);
                efm.PrintCell(currRows, 3, Convert.ToDouble(usl_3), style2);
                efm.PrintCell(currRows, 5, RusCurrency.Str(Math.Round(Convert.ToDouble(usl_3), 2)), style);
                efm.AddMergedRegion(new CellRangeAddress(currRows, 1, currRows, 2));
                efm.AddMergedRegion(new CellRangeAddress(currRows, 3, currRows, 4));
                currRows++;

                efm.PrintCell(currRows, 1, "дневной стационар:", style);
                efm.PrintCell(currRows, 3, Convert.ToDouble(usl_2), style2);
                efm.PrintCell(currRows, 5, RusCurrency.Str(Math.Round(Convert.ToDouble(usl_2), 2)), style);
                efm.AddMergedRegion(new CellRangeAddress(currRows, 1, currRows, 2));
                efm.AddMergedRegion(new CellRangeAddress(currRows, 3, currRows, 4));
                currRows++;

                efm.PrintCell(currRows, 1, "скорая медицинская помощь:", style);
                efm.PrintCell(currRows, 3, Convert.ToDouble(usl_4), style2);
                efm.PrintCell(currRows, 5, RusCurrency.Str(Math.Round(Convert.ToDouble(usl_4), 2)), style);
                efm.AddMergedRegion(new CellRangeAddress(currRows, 1, currRows, 2));
                efm.AddMergedRegion(new CellRangeAddress(currRows, 3, currRows, 4));
                StatusProgressBar1.Dispatcher.Invoke(() => { StatusTextBlock3.Text = $"Сохранение файла {filename}"; });
                efm.Save();
                StatusProgressBar1.Dispatcher.Invoke(() => { StatusTextBlock3.Text = $"Сохранение файла {filename} завершено"; });
                return usl_1 + usl_2 + usl_3 + usl_4;
            }
        }

        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;

        private void AddLogInvoke(EXPORTZGLV fi, LogType t, params string[] Message)
        {
            Dispatcher.Invoke(() => { fi.AddLog(t, Message); });

        }

        void GetFile(EXPORTZGLV fi, string ExportFolder, bool isTEMP1, string SMO)
        {
            try
            {
                var PathSchema= Path.Combine(LocalFolder, string.IsNullOrEmpty(SMO) ? "EXPORT_MO_XSD" : "EXPORT_SMO_XSD");
                var conn = new OracleConnection(AppConfig.Property.ConnectionString);
                //Определение типа файла
                AddLogInvoke(fi, LogType.Info, "Определение типа файла");
                var fp = ParseFileName.Parse(fi.FILENAME);
                if (fp.IsNull)
                {
                    throw new Exception("Ошибка при определении типа файла");
                }
                var PATH_XSD = "";
                CheckXMLValidator CXL = null;

                CXL = new CheckXMLValidator(VersionMP.V3_1);
                switch (fp.FILE_TYPE.ToFileType())
                {
                    case FileType.DD:
                    case FileType.DF:
                    case FileType.DO:
                    case FileType.DP:
                    case FileType.DR:
                    case FileType.DS:
                    case FileType.DU:
                    case FileType.DV:
                        PATH_XSD = Path.Combine(PathSchema, "D31.xsd");
                        break;
                    case FileType.H:
                        PATH_XSD = Path.Combine(PathSchema, "H31.xsd");
                        break;
                    case FileType.T:
                        PATH_XSD = Path.Combine(PathSchema, "T31.xsd");
                        break;
                    case FileType.C:
                        PATH_XSD = Path.Combine(PathSchema, "C31.xsd");
                        break;
                }

                var isSMO = !string.IsNullOrEmpty(SMO);
                var TEMP = isTEMP1 ? "_TEMP1" : "";
                var PRED = isSMO ?  $"SMO = '{SMO}'": "IsZK = 1";
                AddLogInvoke(fi, LogType.Info, "Формирование файла");
                //Заголовок текущего файла
                var zglvid = fi.ZGLV_ID;

                //Запрашиваем заголовки
                //ZGLV
                //Запрос
                var ZGLV = new DataTable();
                var oda = new OracleDataAdapter($@"select * from V_EXPORT_H_ZGLV{TEMP} t where zglv_id = {zglvid}", conn);
                AddLogInvoke(fi, LogType.Info, "Запрос заголовка");
                oda.Fill(ZGLV);
                if (ZGLV.Rows.Count != 1)
                {
                    throw new Exception($"Для файла {fi.FILENAME} Вернулось больше 1го заголовка или 0");
                }
                //СЧЕТ----------------------------------              
                AddLogInvoke(fi, LogType.Info, "Запрос счета");
                oda = new OracleDataAdapter($@"select * from V_EXPORT_H_SCHET{TEMP} t where zglv_id = {zglvid}", conn);
                var SCHET = new DataTable();
                oda.Fill(SCHET);
                if (SCHET.Rows.Count != 1)
                {
                    throw new Exception($"Для файла {fi.FILENAME} Вернулось больше 1го счета или 0");
                }

                //----------------------------------------------------
                AddLogInvoke(fi, LogType.Info, "Запрос записей");
                //ZAP+PAC+Z_SL-------------------------------------------------
                var ZAP = new DataTable();
                oda = new OracleDataAdapter($@"select * from V_EXPORT_H_ZAP{TEMP} t where zglv_id = {zglvid} and {PRED}", conn);
                oda.Fill(ZAP);
                if (ZAP.Rows.Count == 0)
                {
                    AddLogInvoke(fi, LogType.Info, $"Для файла {fi.FILENAME} Вернулось 0 записей. СМО = {SMO}");
                    return;
                }

               
                //-------------------------------------------------------------------------  
                var SLUCH = new DataTable();
                var SANK = new DataTable();

                AddLogInvoke(fi, LogType.Info, "Запрос случаев");
                foreach (var sl in GetIDFromDataTable(ZAP, "SLUCH_Z_ID"))
                {
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_SLUCH{TEMP} t where SLUCH_Z_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(SLUCH);

                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_SANK{TEMP}  t where SLUCH_Z_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(SANK);
                }

                var NAZR = new DataTable();
                var DS2_N = new DataTable();
                var SL_KOEF = new DataTable();
                var USL = new DataTable();
                var ONK_USLtbl = new DataTable();
                var LEK_PR = new DataTable();
                var NAPR = new DataTable();
                var B_PROT = new DataTable();
                var H_CONS = new DataTable();
                var DS2 = new DataTable();
                var DS3 = new DataTable();
                var CRIT = new DataTable();
                AddLogInvoke(fi, LogType.Info, "Запрос данных в случае");
                var B_DIAG = new DataTable();
                foreach (var sl in GetIDFromDataTable(SLUCH, "SLUCH_ID"))
                {
                    //NAZR
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_NAZR{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(NAZR);
                    //DS2_N
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS2_N{TEMP} t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(DS2_N);
                    //KOEF
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_SL_KOEF{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(SL_KOEF);
                    //USL
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_USL{TEMP} t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(USL);
                    //ONK_USL_PR
                    oda = new OracleDataAdapter($"select * from V_EXPORT_ONK_USL{TEMP} t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(ONK_USLtbl);
                    //LEK_PR
                    oda = new OracleDataAdapter($"select * from V_EXPORT_LEK_PR{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(LEK_PR);
                    //LEK_PR
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_NAPR{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(NAPR);
                    //B_PROT
                    oda = new OracleDataAdapter($"select * from V_EXPORT_B_PROT{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(B_PROT);
                    //H_CONS
                    oda = new OracleDataAdapter($"select * from V_EXPORT_CONS{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(H_CONS);
                    //H_CONS
                    oda = new OracleDataAdapter($"select * from V_EXPORT_B_DIAG{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(B_DIAG);
                    //DS2
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS2{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(DS2);
                    //DS3
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS3{TEMP} t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(DS3);
                    //CRIT
                    oda = new OracleDataAdapter($"select * from V_EXPORT_H_CRIT{TEMP}  t where SLUCH_ID in ({string.Join(",", sl)})", conn);
                    oda.Fill(CRIT);
                }

                //L_ZGLV
                AddLogInvoke(fi, LogType.Info, "Запрос заголовка перс данных");
                var L_ZGLV = new DataTable();
                oda = new OracleDataAdapter($@"select * from V_EXPORT_L_ZGLV{TEMP} t where t.FILENAME1 = '{fi.FILENAME}'", conn);
                oda.Fill(L_ZGLV);

                if (L_ZGLV.Rows.Count != 1)
                {
                    throw new Exception("Запрос заголовка перс данных вернул более 1го или не одного заголовка");
                }

                //L_PERS
                AddLogInvoke(fi, LogType.Info, "Запрос персональных данных");
                var PERS = new DataTable();
                oda = new OracleDataAdapter($@"select distinct h_zglv_id, filename1, pers_id, zglv_id, id_pac, fam, im, ot, w, dr, fam_p, im_p, ot_p, w_p, dr_p, mr, doctype, docser, docnum, snils, okatog, okatop, dost, dost_p, fam_tfoms, im_tfoms, ot_tfoms, dr_tfoms, rokato, renp, rqogrn, rdbeg, tel, {(isSMO? "comentp_new comentp" : "comentp")}, docdate, docorg, iszk from V_EXPORT_L_PERS{TEMP} t where {PRED}  and H_ZGLV_ID = {zglvid}", conn);
                oda.Fill(PERS);
                AddLogInvoke(fi, LogType.Info, "Создание файла L");
                var fileL = CreateFileL(L_ZGLV, PERS);
                AddLogInvoke(fi, LogType.Info, "Создание файла H");
                var file = CreateFile(ZGLV, SCHET, ZAP, SLUCH, USL, NAZR, SANK, SL_KOEF, DS2_N, NAPR, B_PROT, B_DIAG, H_CONS, ONK_USLtbl, LEK_PR, DS2, DS3, CRIT);
                file.SCHET.PLAT = "75";

                if (string.IsNullOrEmpty(SMO))
                {
                    file.ClearSMO_DATE();
                }

               

                file.SCHET.SUMMAV = file.ZAP.Sum(x => x.Z_SL.SL.Sum(sl => sl.USL.Sum(us => us.SUMV_USL)));
                file.SCHET.SUMMAP = file.ZAP.Sum(x => x.Z_SL.SUMP);
                file.SCHET.SANK_MEK = file.ZAP.Sum(x => x.Z_SL.SANK.Sum(san => san.S_SUM));
                var result = CheckFile(file, fileL);
                if (result.Length != 0)
                {
                    AddLogInvoke(fi, LogType.Error, "Результат контроля целостности");
                    AddLogInvoke(fi, LogType.Error, result);
                }

                var month = fp.MM.PadLeft(2, '0');
                var Year = fp.YY;
                var newnameH = GetFileName(fp, Year, month, false, SMO);
                var newnameL = GetFileName(fp, Year, month, true,SMO);



                file.ZGLV.FILENAME = newnameH;
                fileL.ZGLV.FILENAME = newnameL;
                fileL.ZGLV.FILENAME1 = newnameH;





                var pathfile = Path.Combine(ExportFolder, $"{newnameH}.xml");
                AddLogInvoke(fi, LogType.Info, $"Сохранение файла {pathfile}");
                var st = File.Create(pathfile);
                file.WriteXml(st);
                st.Close();
                var pathfileL = Path.Combine(ExportFolder, $"{newnameL}.xml");
                AddLogInvoke(fi, LogType.Info, $"Сохранение файла {pathfileL}");
                st = File.Create(pathfileL);
                fileL.WriteXml(st);
                st.Close();
                AddLogInvoke(fi, LogType.Info, "Проверка схемы основного файла");
              
                var sc = new SchemaChecking();
                var err = sc.CheckXML(pathfile, PATH_XSD, CXL);
                if(err.Count!=0)
                    AddLogInvoke(fi, LogType.Error, err.Select(x => x.MessageOUT).ToArray());

                AddLogInvoke(fi, LogType.Info, "Проверка схемы файла персональных данных");
                var L_XSD_PATH = Path.Combine(PathSchema, "L31.xsd");
                if (fileL.ZGLV.VERSION == "3.2")
                    L_XSD_PATH = Path.Combine(PathSchema, "L32.xsd");

                err = sc.CheckXML(pathfileL, L_XSD_PATH, CXL);
                if (err.Count != 0)
                    AddLogInvoke(fi, LogType.Error, err.Select(x => x.MessageOUT).ToArray());
             
                var PATH_ARCIVE = Path.Combine(ExportFolder, $"{newnameH}.ZIP");
                if(!string.IsNullOrEmpty(SMO))
                    PATH_ARCIVE = Path.Combine(ExportFolder, SMO, newnameH[0].ToString(),$"{newnameH}.ZIP");
                if (!Directory.Exists(Path.GetDirectoryName(PATH_ARCIVE)))
                    Directory.CreateDirectory(Path.GetDirectoryName(PATH_ARCIVE));
                fi.AddLog(LogType.Info, $"Упаковка архива: {PATH_ARCIVE}");
                if (File.Exists(PATH_ARCIVE))
                {
                    if (MessageBox.Show($@"Файл {PATH_ARCIVE} существует. Заменить?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        throw new Exception("Операция отменена пользователем");
                    }
                }
                else
                    ToArchive(PATH_ARCIVE, pathfile, pathfileL);

                fi.PathArc.Add(PATH_ARCIVE);
                if (!fi.SUM.HasValue)
                    fi.SUM = file.SCHET.SUMMAV;
                else
                    fi.SUM += file.SCHET.SUMMAV;
            }
            catch (Exception ex)
            {
                AddLogInvoke(fi, LogType.Error, $"Ошибка {ex.Source}: {ex.FullError()}");
            }
        }

      

        public static List<int[]> GetIDFromDataTable(DataTable tbl, string column_name)
        {
            return GetIDFromDataTable(tbl.Select(), column_name);

        }
        public static List<int[]> GetIDFromDataTable(IEnumerable<DataRow> rows, string column_name)
        {
            const int countList = 500;
            var rez = new List<int[]>();
            var rez_sub = new List<int>();
            var count = 0;
            foreach (DataRow r in rows)
            {
                rez_sub.Add(Convert.ToInt32(r[column_name]));
                count++;
                if (count == countList)
                {
                    count = 0;
                    rez.Add(rez_sub.ToArray());
                    rez_sub = new List<int>();
                }
            }
            if (count != 0)
            {
                rez.Add(rez_sub.ToArray());
            }
            return rez;

        }



        private PERS_LIST CreateFileL(DataTable ZGLVtbl, DataTable PERStbl)
        {
            var item = new PERS_LIST();
            item.ZGLV = PERSZGLV.Get(ZGLVtbl.Rows[0]);
            foreach (DataRow row in PERStbl.Rows)
            {
                item.PERS.Add(PERS.Get(row));
            }
            return item;
        }

        ZL_LIST CreateFile(DataTable ZGLVtbl, DataTable SCHETtbl, DataTable ZAPtbl, DataTable SLUCHtbl, DataTable USLtbl, DataTable NAZRtbl, DataTable SANKtbl,
            DataTable KOEFtbl, DataTable DS2_Ntbl, DataTable NAPRtbl, DataTable B_PROTtbl, DataTable B_DIAGtbl, DataTable H_CONStbl, DataTable ONK_USLtbl, DataTable LEK_PRtbl,
            DataTable DS2, DataTable DS3, DataTable CRIT)
        {
            var step = 0;
            try
            {
                var file = new ZL_LIST();
                file.ZGLV = ZGLV.Get(ZGLVtbl.Rows[0]);
                file.ZGLV.SD_Z = ZAPtbl.Rows.Count;
                file.SCHET = SCHET.Get(SCHETtbl.Rows[0]);
                file.SCHET.YEAR = Convert.ToDecimal(SCHETtbl.Rows[0]["YEAR_BASE"]);
                file.SCHET.MONTH = Convert.ToDecimal(SCHETtbl.Rows[0]["MONTH_BASE"]);

                step = 1;
                foreach (DataRow row_z in ZAPtbl.Rows)
                {
                    var z = ZAP.Get(row_z);
                    z.PACIENT = PACIENT.Get(row_z);
                    z.Z_SL = Z_SL.Get(row_z);
                    file.ZAP.Add(z);
                    foreach (var sank_row in SANKtbl.Select($"SLUCH_Z_ID = {z.Z_SL.SLUCH_Z_ID}"))
                    {
                        var san = SANK.Get(sank_row);
                        z.Z_SL.SANK.Add(san);
                    }

                    step = 2;
                    foreach (var sl_row in SLUCHtbl.Select($"SLUCH_Z_ID = {z.Z_SL.SLUCH_Z_ID}"))
                    {
                        var sl = SL.Get(sl_row, DS2.Select($"SLUCH_ID = {sl_row["SLUCH_ID"]}"), DS3.Select($"SLUCH_ID = {sl_row["SLUCH_ID"]}"), CRIT.Select($"SLUCH_ID = {sl_row["SLUCH_ID"]}", "ORD"));
                        z.Z_SL.SL.Add(sl);
                        step = 3;
                        foreach (var usl_row in USLtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            var us = USL.Get(usl_row);
                            sl.USL.Add(us);
                        }
                        step = 4;
                        foreach (var onk_usl_row in ONK_USLtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            var o_us = ONK_USL.Get(onk_usl_row);
                            sl.ONK_SL.ONK_USL.Add(o_us);
                            foreach (var lek_pr_row in LEK_PRtbl.Select($"ONK_USL_ID = {o_us.ONK_USL_ID}"))
                            {
                                var lek = LEK_PR.Get(lek_pr_row);
                                o_us.LEK_PR.Add(lek);
                            }
                        }
                        step = 5;


                        foreach (var napr_row in NAPRtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            sl.NAPR.Add(NAPR.Get(napr_row));
                        }
                        step = 6;
                        foreach (var cons_row in H_CONStbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            sl.CONS.Add(CONS.Get(cons_row));
                        }

                        step = 8;
                        if (NAZRtbl != null)
                            foreach (var naz_row in NAZRtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.NAZ.Add(NAZR.Get(naz_row));
                            }
                        step = 9;
                        if (KOEFtbl != null)
                            foreach (var koef_row in KOEFtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.KSG_KPG.SL_KOEF.Add(SL_KOEF.Get(koef_row));
                            }
                        step = 10;
                        if (B_PROTtbl != null)
                            foreach (var prot_row in B_PROTtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.ONK_SL.B_PROT.Add(B_PROT.Get(prot_row));
                            }
                        step = 11;
                        if (B_DIAGtbl != null)
                            foreach (var diag_row in B_DIAGtbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.ONK_SL.B_DIAG.Add(B_DIAG.Get(diag_row));
                            }
                        step = 12;
                        if (DS2_Ntbl != null)
                            foreach (var d2_row in DS2_Ntbl.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                            {
                                sl.DS2_N.Add(DS2_N.Get(d2_row));
                            }
                    }
                }


                return file;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при формировании класса H:{ex.Message} step {step}");
            }

        }

        string GetFileName(MatchParseFileName fp, string Year, string month, bool IsFileL, string SMO)
        {
           
            var type = fp.FILE_TYPE.ToFileType();
            var newnameH = $"{fp.FILE_TYPE}{fp.Pi}{fp.Ni}{(string.IsNullOrEmpty(SMO)? "T75" : $"S{SMO}")}_{Year}{month}{fp.NN}";
            var newnameL = $"L{newnameH.Substring(1)}";
            if (type.Contains(FileType.T, FileType.C))
            {
                newnameL = $"L{newnameH}";
            }
            return IsFileL ? newnameL : newnameH;

        }


        private string[] CheckFile(ZL_LIST main, PERS_LIST Pers)
        {
            var result = new List<string>();
            //Проверка что у всех Id_pac есть id_pac в файле перс данных
            foreach (var p in main.ZAP.Select(x => x.PACIENT))
            {
                var t = Pers.PERS.FirstOrDefault(x => x.ID_PAC == p.ID_PAC);
                if (t == null)
                    result.Add($"Для ID_PAC = {p.ID_PAC} не найдена соответствующая информация в файле персональных данных");
            }
            return result.ToArray();
        }
        /// <summary>
        /// Создать в архив ZIP
        /// </summary>
        /// <param name="PathFile">Путь к архиву</param>
        /// <param name="path">Пути к файлам добавляемых в архив</param>
        private void ToArchive(string PathFile, params string[] path)
        {
            using (var archive = ZipFile.Open(PathFile, ZipArchiveMode.Create))
            {
                foreach (var str in path)
                {
                    archive.CreateEntryFromFile(str, Path.GetFileName(str));
                }
            }
            foreach (var str in path)
            {
                File.Delete(str);
            }
        }

        private void ExportFile_OnLoaded(object sender, RoutedEventArgs e)
        {
            DatePickerPERIOD.SelectedDate = DateTime.Now.AddMonths(-1);
        }

        private void ButtonUpdateLog_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CollectionViewSourceFilesLOG.View.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemReset_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                CollectionViewSourceFiles.View.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class EXPORTZGLV:INotifyPropertyChanged
    {
        private bool _InWork;

        public bool InWork
        {
            get { return _InWork; }
            set
            {
                _InWork = value;
                RaisePropertyChanged("InWork");
            }
        }
        public bool IsSelect { get; set; } = true;
        public int ZGLV_ID { get; set; }
        public string CODE_MO { get; set; }
        public string FILENAME { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }

        public int SUM_XLS { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
        public static List<EXPORTZGLV> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static EXPORTZGLV Get(DataRow row)
        {
            try
            {
                var item = new EXPORTZGLV
                {
                    ZGLV_ID = Convert.ToInt32(row["ZGLV_ID"]),
                    FILENAME = Convert.ToString(row["FILENAME"]),
                    CODE_MO = Convert.ToString(row["CODE_MO"]),
                    YEAR = Convert.ToInt32(row["YEAR"]),
                    MONTH = Convert.ToInt32(row["MONTH"])
                };
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения EXPORTZGLV: {ex.Message}", ex);
            }
        }

        public List<LogItem> Logs { get; set; } = new List<LogItem>();
        public void AddLog(LogType t,params string[] m)
        {
            Logs.AddRange(m.Select(x=>new LogItem(t,x)));
            RaisePropertyChanged("Logs");
        }
        public decimal? SUM { get; set; }
        public LogType? IsTypeLog
        {
            get
            {
                if (Logs.Count(x => x.Type == LogType.Error) != 0)
                    return LogType.Error;
                if (Logs.Count(x => x.Type == LogType.Warning) != 0)
                    return LogType.Warning;
                if (Logs.Count(x => x.Type == LogType.Info) != 0)
                    return LogType.Info;
                return null;
            }
        }
        private bool _Finish;

        public bool Finish
        {
            get { return _Finish; }
            set
            {
                _Finish = value;
                RaisePropertyChanged("Finish");
            }
        }
        public List<string> PathArc { get; set; } = new List<string>();
    }


    public enum LogType
    {
        Info = 0,
        Error = 1,
        Warning = 2
    }
    public class LogItem
    {
        public LogItem(LogType _Type, string _Message)
        {
            Type = _Type;
            Message = _Message;
        }
        public string Message { get; set; }
        public LogType Type { get; set; }
    }
    public class TaskItem
    {
        public int ID { get; set; }

        public bool Free { get; set; } = true;

        public Task TSK { get; set; }
    }
    public class TaskManager
    {
        private List<TaskItem> tasks;
        public TaskManager(int count)
        {
            tasks = new List<TaskItem>();
            for (var i = 0; i < count; i++)
            {
                tasks.Add(new TaskItem());
            }
        }
        public TaskItem FreeItem
        {
            get
            {
                Check();
                return tasks.FirstOrDefault(x => x.Free);
            }
        }

        public void Check()
        {
            foreach (var t in tasks)
            {
                if (t.TSK != null)
                {
                    if (t.TSK.IsCompleted)
                    {
                        t.Free = true;
                    }
                }
                else
                    t.Free = true;
            }
        }

        public bool IsSTOP
        {
            get
            {
                Check();
                return tasks.Count == tasks.Count(x => x.Free);
            }
        }
    }

  
}

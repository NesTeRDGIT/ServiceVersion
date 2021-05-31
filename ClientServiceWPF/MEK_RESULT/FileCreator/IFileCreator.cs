using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ExcelManager;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.EntityMP_V31;
using LogType = ClientServiceWPF.Class.LogType;

namespace ClientServiceWPF.MEK_RESULT.FileCreator
{
    public  interface IFileCreator
    {
         FileCreatorResult GetFileXML(V_EXPORT_H_ZGLVRow item, string ExportFolder, bool isTEMP1, string SMO, IProgress<ProgressFileCreator> progress);
         decimal GetFileXLS(List<V_EXPORT_H_ZGLVRow> List, List<F002> smoList, string path, DateTime D_START_XLS, DateTime D_END_XLS, IProgress<string> progress);
    }

    public class ProgressFileCreator
    {
        public ProgressFileCreator(LogType Type, string[] Message)
        {
            this.Type = Type;
            this.Message = Message;
        }
        public LogType Type { get; set; }
        public string[] Message { get; set; }
    }

 

    public class FileCreatorResult
    {
        public static FileCreatorResult CreateNotResult()
        {
            return new FileCreatorResult {Result = false};
        }

        public static FileCreatorResult CreateResult(string PathARC, decimal SUM)
        {
            return new FileCreatorResult {Result = true, PathARC = PathARC, SUM = SUM};
        }

        public bool Result { get; set; }
        public string PathARC { get; set; }
        public decimal SUM { get; set; }
    }

    public class FileCreator: IFileCreator
    {
        private IExportFileRepository exportFileRepository;
        public  FileCreator(IExportFileRepository exportFileRepository)
        {
            this.exportFileRepository = exportFileRepository;
        }
        private  string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;

        #region CreateXML
        private void AddLogInvoke(IProgress<ProgressFileCreator> progress, LogType t, params string[] Message)
        {
            progress?.Report(new ProgressFileCreator(t, Message));

        }
        public FileCreatorResult GetFileXML(V_EXPORT_H_ZGLVRow item, string ExportFolder, bool isTEMP1, string SMO, IProgress<ProgressFileCreator> progress)
        {
            try
            {
                var PathSchema = Path.Combine(LocalFolder, string.IsNullOrEmpty(SMO) ? "EXPORT_MO_XSD" : "EXPORT_SMO_XSD");
                var conn = exportFileRepository.CreateConnection();
                //Определение типа файла
                AddLogInvoke(progress, LogType.Info, "Определение типа файла");
                var fp = ParseFileName.Parse(item.FILENAME);
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
                var PRED = isSMO ? $"SMO = '{SMO}'" : "IsZK = 1";
                AddLogInvoke(progress, LogType.Info, "Формирование файла");
                //Заголовок текущего файла
                var zglvid = item.ZGLV_ID;

                //Запрашиваем заголовки
                //ZGLV
                //Запрос
                AddLogInvoke(progress, LogType.Info, "Запрос заголовка");
                var ZGLV = exportFileRepository.V_EXPORT_H_ZGLV(item.ZGLV_ID, isTEMP1, conn);
                if (ZGLV.Rows.Count != 1)
                {
                    throw new Exception($"Для файла {item.FILENAME} Вернулось больше 1го заголовка или 0");
                }
                //СЧЕТ----------------------------------              
                AddLogInvoke(progress, LogType.Info, "Запрос счета");

                var SCHET = exportFileRepository.V_EXPORT_H_SCHET(item.ZGLV_ID, isTEMP1, conn);
                if (SCHET.Rows.Count != 1)
                {
                    throw new Exception($"Для файла {item.FILENAME} Вернулось больше 1го счета или 0");
                }

                //----------------------------------------------------
                AddLogInvoke(progress, LogType.Info, "Запрос записей");
                //ZAP+PAC+Z_SL-------------------------------------------------
                var ZAP = exportFileRepository.V_EXPORT_H_ZAP(item.ZGLV_ID, SMO, isTEMP1, conn);
                if (ZAP.Rows.Count == 0)
                {
                    AddLogInvoke(progress, LogType.Info, $"Для файла {item.FILENAME} Вернулось 0 записей. СМО = {SMO}");
                    return FileCreatorResult.CreateNotResult();
                }


                //-------------------------------------------------------------------------  
                var SLUCH = new DataTable();
                var SANK = new DataTable();

                AddLogInvoke(progress, LogType.Info, "Запрос случаев");
                foreach (var sl in GetIDFromDataTable(ZAP, "SLUCH_Z_ID"))
                {
                    var items = sl.ToList();
                    SLUCH.Merge(exportFileRepository.V_EXPORT_H_SLUCH(items, isTEMP1, conn));
                    SANK.Merge(exportFileRepository.V_EXPORT_H_SANK(items, isTEMP1, conn));
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
                AddLogInvoke(progress, LogType.Info, "Запрос данных в случае");
                var B_DIAG = new DataTable();
                foreach (var sl in GetIDFromDataTable(SLUCH, "SLUCH_ID"))
                {
                    var items = sl.ToList();

                    NAZR.Merge(exportFileRepository.V_EXPORT_H_NAZR(items, isTEMP1, conn));
                    DS2_N.Merge(exportFileRepository.V_EXPORT_H_DS2_N(items, isTEMP1, conn));
                    SL_KOEF.Merge(exportFileRepository.V_EXPORT_H_SL_KOEF(items, isTEMP1, conn));
                    USL.Merge(exportFileRepository.V_EXPORT_H_USL(items, isTEMP1, conn));
                    ONK_USLtbl.Merge(exportFileRepository.V_EXPORT_ONK_USL(items, isTEMP1, conn));
                    LEK_PR.Merge(exportFileRepository.V_EXPORT_LEK_PR(items, isTEMP1, conn));
                    NAPR.Merge(exportFileRepository.V_EXPORT_H_NAPR(items, isTEMP1, conn));
                    B_PROT.Merge(exportFileRepository.V_EXPORT_B_PROT(items, isTEMP1, conn));
                    H_CONS.Merge(exportFileRepository.V_EXPORT_CONS(items, isTEMP1, conn));
                    B_DIAG.Merge(exportFileRepository.V_EXPORT_B_DIAG(items, isTEMP1, conn));
                    DS2.Merge(exportFileRepository.V_EXPORT_H_DS2(items, isTEMP1, conn));
                    DS3.Merge(exportFileRepository.V_EXPORT_H_DS3(items, isTEMP1, conn));
                    CRIT.Merge(exportFileRepository.V_EXPORT_H_CRIT(items, isTEMP1, conn));
                }

                //L_ZGLV
                AddLogInvoke(progress, LogType.Info, "Запрос заголовка перс данных");
                var L_ZGLV = exportFileRepository.V_EXPORT_L_ZGLV(item.FILENAME, isTEMP1, conn);
                if (L_ZGLV.Rows.Count != 1)
                {
                    throw new Exception("Запрос заголовка перс данных вернул более 1го или не одного заголовка");
                }

                //L_PERS
                AddLogInvoke(progress, LogType.Info, "Запрос персональных данных");
                var PERS = exportFileRepository.V_EXPORT_L_PERS(item.ZGLV_ID, SMO, isTEMP1, conn);
                AddLogInvoke(progress, LogType.Info, "Создание файла L");
                var fileL = CreateFileL(L_ZGLV, PERS);
                AddLogInvoke(progress, LogType.Info, "Создание файла H");
                var file = CreateFile(ZGLV, SCHET, ZAP, SLUCH, USL, NAZR, SANK, SL_KOEF, DS2_N, NAPR, B_PROT, B_DIAG, H_CONS, ONK_USLtbl, LEK_PR, DS2, DS3, CRIT);

                var month = fp.MM.PadLeft(2, '0');
                var Year = fp.YY;

                var newnameH = GetFileName(fp, Year, month, false, SMO);
                var newnameL = GetFileName(fp, Year, month, true, SMO);

                ModernFileH(file, newnameH, SMO);
                ModernFileL(fileL, newnameL, newnameH);


                AddLogInvoke(progress, LogType.Info, "Контроля целостности");
                var result = CheckFile(file, fileL);
                if (result.Length != 0)
                {
                    AddLogInvoke(progress, LogType.Error, "Ошибки контроля целостности");
                    AddLogInvoke(progress, LogType.Error, result);
                }

                var pathfile = Path.Combine(ExportFolder, $"{newnameH}.xml");
                var pathfileL = Path.Combine(ExportFolder, $"{newnameL}.xml");
                AddLogInvoke(progress, LogType.Info, $"Сохранение файла {pathfile}");
                using (var st = File.Create(pathfile))
                {
                    file.WriteXml(st);
                    st.Close();
                }
                AddLogInvoke(progress, LogType.Info, $"Сохранение файла {pathfileL}");
                using (var st = File.Create(pathfileL))
                {
                    fileL.WriteXml(st);
                    st.Close();
                }
                AddLogInvoke(progress, LogType.Info, "Проверка схемы основного файла");

                var sc = new SchemaChecking();
                var err = sc.CheckXML(pathfile, PATH_XSD, CXL);
                if (err.Count != 0)
                    AddLogInvoke(progress, LogType.Error, err.Select(x => x.MessageOUT).ToArray());

                AddLogInvoke(progress, LogType.Info, "Проверка схемы файла персональных данных");
                var L_XSD_PATH = Path.Combine(PathSchema, "L31.xsd");
                if (fileL.ZGLV.VERSION == "3.2")
                    L_XSD_PATH = Path.Combine(PathSchema, "L32.xsd");
                err = sc.CheckXML(pathfileL, L_XSD_PATH, CXL);

                if (err.Count != 0)
                    AddLogInvoke(progress, LogType.Error, err.Select(x => x.MessageOUT).ToArray());

                var PATH_ARCIVE = Path.Combine(ExportFolder, $"{newnameH}.ZIP");
                if (!string.IsNullOrEmpty(SMO))
                    PATH_ARCIVE = Path.Combine(ExportFolder, SMO, newnameH[0].ToString(), $"{newnameH}.ZIP");
                if (!Directory.Exists(Path.GetDirectoryName(PATH_ARCIVE)))
                    Directory.CreateDirectory(Path.GetDirectoryName(PATH_ARCIVE));
                AddLogInvoke(progress, LogType.Info, $"Упаковка архива: {PATH_ARCIVE}");
                if (File.Exists(PATH_ARCIVE))
                {
                    if (MessageBox.Show($@"Файл {PATH_ARCIVE} существует. Заменить?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                    {
                        throw new Exception("Операция отменена пользователем");
                    }
                }
                else
                    ToArchive(PATH_ARCIVE, pathfile, pathfileL);

                return FileCreatorResult.CreateResult(PATH_ARCIVE, file.SCHET.SUMMAV);
            }
            catch (Exception ex)
            {
                AddLogInvoke(progress, LogType.Error, $"Ошибка {ex.Source}: {ex.FullError()}");
                return FileCreatorResult.CreateNotResult();
            }
        }
        private void ModernFileH(ZL_LIST file, string FileName, string SMO)
        {
            file.SCHET.PLAT = string.IsNullOrEmpty(SMO) ? "75" : SMO;
            if (string.IsNullOrEmpty(SMO))
            {
                file.ClearSMO_DATA();
            }
            file.SCHET.SUMMAV = file.ZAP.Sum(x => x.Z_SL.SL.Sum(sl => sl.USL.Sum(us => us.SUMV_USL)));
            file.SCHET.SUMMAP = file.ZAP.Sum(x => x.Z_SL.SUMP);
            file.SCHET.SANK_MEK = file.ZAP.Sum(x => x.Z_SL.SANK.Sum(san => san.S_SUM));
            file.ZGLV.FILENAME = FileName;
        }
        private void ModernFileL(PERS_LIST fileL, string newnameL, string newnameH)
        {
            fileL.ZGLV.FILENAME = newnameL;
            fileL.ZGLV.FILENAME1 = newnameH;
        }
        private static IEnumerable<IEnumerable<long>> GetIDFromDataTable(DataTable tbl, string column_name)
        {
            return GetIDFromDataTable(tbl.Select(), column_name);
        }
        private static IEnumerable<IEnumerable<long>> GetIDFromDataTable(IEnumerable<DataRow> rows, string column_name)
        {
            const int countList = 500;
            var rez = new List<IEnumerable<long>>();
            var rez_sub = new List<long>();
            var count = 0;
            foreach (var r in rows)
            {
                rez_sub.Add(Convert.ToInt64(r[column_name]));
                count++;
                if (count == countList)
                {
                    count = 0;
                    rez.Add(rez_sub);
                    rez_sub = new List<long>();
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
            var newnameH = $"{fp.FILE_TYPE}{fp.Pi}{fp.Ni}{(string.IsNullOrEmpty(SMO) ? "T75" : $"S{SMO}")}_{Year}{month}{fp.NN}";
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
        #endregion
        #region CreateXLS
        public decimal GetFileXLS(List<V_EXPORT_H_ZGLVRow> List, List<F002> smoList, string path, DateTime D_START_XLS, DateTime D_END_XLS, IProgress<string> progress)
        {
            //Формируем EXCEL
            try
            {
                var CODE_MO = List.First().CODE_MO;
                var CurrentYear = List.First().YEAR;
                var CurrentMonth = List.First().MONTH;
                var ZGLV_ID = List.Select(x => x.ZGLV_ID).ToList();
                decimal SUM_IN_XLS = 0;
                var MO = exportFileRepository.GetF003Name(CODE_MO);

                foreach (var strah in smoList)
                {
                    progress?.Report($"Запрос данных для: {strah.SMOCOD}");
                    var ExcelTable = exportFileRepository.V_EXPORT_EXCEL_FROM(ZGLV_ID, strah.SMOCOD);
                    var EXL_PATH = Path.Combine(path, "XLS", $"{CODE_MO} Реестр за {new DateTime(CurrentYear, CurrentMonth, 1):MMMMMMMMMMM yyyy} для подписи в {strah.SMOCOD}.XLSX");

                    if (!Directory.Exists(Path.GetDirectoryName(EXL_PATH)))
                        Directory.CreateDirectory(Path.GetDirectoryName(EXL_PATH));
                    if (ExcelTable.Count != 0)
                        SUM_IN_XLS += PrintExcel(ExcelTable, D_START_XLS, D_END_XLS, strah.SMOCOD, MO, EXL_PATH, progress);
                }
                progress?.Report("Формирование EXCEL завершено");
                return SUM_IN_XLS;
            }
            catch (Exception ex)
            {
                MessageBox.Show($@"Ошибка выгрузки XLS: {ex.Message}");
                return 0;
            }
        }
        decimal PrintExcel(List<XLS_TABLE> tbl, DateTime Start, DateTime End, string SMO, string MO, string path, IProgress<string> progress)
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
                var StyleLeftText = efm.CreateType(new FontOpenXML() { size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), null);
                var StyleCenterNumeric = efm.CreateType(new FontOpenXML() { size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Center, Format = (uint)DefaultNumFormat.F4 }, new BorderOpenXML(), null);

                decimal usl_1 = 0;
                decimal usl_2 = 0;
                decimal usl_3 = 0;
                decimal usl_4 = 0;
                progress?.Report(@"0/{tbl.Count}");

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
                    progress?.Report($@"{index}/{tbl.Count}");

                    //Если больше 1048576
                    if (currRows == 1048576)
                    {
                        throw new Exception("Не проверенно 2 и более листа");
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
                    efm.PrintCell(MRow, 16, row.TARIF, StyleCenterNumeric);
                    efm.PrintCell(MRow, 17, row.SUMV_USL, StyleCenterNumeric);
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


                var style = efm.CreateType(new FontOpenXML() { size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Left }, null, null);
                var style1 = efm.CreateType(new FontOpenXML() { size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Center, Format = (uint)DefaultNumFormat.F4, Bold = true }, null, null);
                var style2 = efm.CreateType(new FontOpenXML() { size = 10, fontname = "Times New Roman", HorizontalAlignment = HorizontalAlignmentV.Center, Format = (uint)DefaultNumFormat.F4 }, null, null);


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

                progress?.Report($"Сохранение файла {filename}");
                efm.Save();
                progress?.Report($"Сохранение файла {filename} завершено");
                return usl_1 + usl_2 + usl_3 + usl_4;
            }
        }
        #endregion

    }

}

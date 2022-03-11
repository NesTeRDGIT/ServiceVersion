using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using ExcelManager;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using ServiceLoaderMedpomData.EntityMP_V31;
using LogType = ClientServiceWPF.Class.LogType;

namespace ClientServiceWPF.MEK_RESULT.FileCreator
{
    public  interface IFileCreator
    {
        FileCreatorResult GetFileXML(V_EXPORT_H_ZGLVRow item, string ExportFolder, DBSource source, TypeFileCreate typeFileCreate, string SMO, SLUCH_PARAM sluchParam, string NN_FFOMS_DX,string sufix, Action<ProgressFileCreator> progress);
        decimal GetFileXLS(List<V_EXPORT_H_ZGLVRow> List, List<F002> smoList, string path, DateTime D_START_XLS, DateTime D_END_XLS, IProgress<string> progress);
        Task CreateFileAsync(V_EXPORT_H_ZGLVRowVM item, string Folder, DBSource source, TypeFileCreate typeFileCreate, SLUCH_PARAM sluchParam, List<F002> smoList, int OrderInMonth, int Order);
        void CreateFile(V_EXPORT_H_ZGLVRowVM item, string Folder, DBSource source, TypeFileCreate typeFileCreate, SLUCH_PARAM sluchParam, List<F002> smoList, int OrderInMonth, int Order);
    }
    public class ProgressFileCreator
    {
        public ProgressFileCreator(LogType type, string[] message)
        {
            this.Type = type;
            this.Message = message;
        }
        public LogType Type { get; set; }
        public string[] Message { get; set; }
    }
    public class FileNameFFOMSDxParam
    {
        /// <summary>
        /// Порядковый номер файла в месяц. Присваивается в порядке возрастания, начиная со значения «1», увеличиваясь на единицудля каждого следующего пакета в данном отчетном периоде
        /// </summary>
        public int OrderInMonth { get; set; }
        /// <summary>
        /// Порядковый  номер  пакета.  Присваивается  в  порядке  возрастания,  начиная  со значения «1», увеличиваясь на единицу для каждого следующего пакета в данном отчетном периоде.
        /// </summary>
        public int Order { get; set; }
    }
    public class FileCreatorResult
    {
        public static FileCreatorResult CreateNotResult()
        {
            return new FileCreatorResult {Result = false};
        }

        public static FileCreatorResult CreateResult(string PathARC, decimal SUM,string SMO)
        {
            return new FileCreatorResult {Result = true, PathARC = PathARC, SUM = SUM, SMO= SMO};
        }

        public bool Result { get; set; }
        public string PathARC { get; set; }
        public decimal SUM { get; set; }
        public string SMO { get; set; }
        public ZL_LIST FileH { get; set; }
        public PERS_LIST FileL { get; set; }

    }
    public class FileCreator : IFileCreator
    {
        private DateTime DT_2021_08 = new DateTime(2021, 8, 1);
        private DateTime DT_2022_01 = new DateTime(2022, 1, 1);
        private IExportFileRepository exportFileRepository;

        public FileCreator(IExportFileRepository exportFileRepository)
        {
            this.exportFileRepository = exportFileRepository;
        }

        private string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;

        #region CreateXML
        private void AddLogInvoke(Action<ProgressFileCreator> progress, LogType t, params string[] Message)
        {
            progress?.Invoke(new ProgressFileCreator(t, Message));
        }

        public FileCreatorResult GetFileXML(V_EXPORT_H_ZGLVRow item, string ExportFolder, DBSource source, TypeFileCreate typeFileCreate, string SMO, SLUCH_PARAM sluchParam, string NN_FFOMS_DX, string sufix, Action<ProgressFileCreator> progress)
        {
            try
            {
                var PathSchema = "";
                switch (typeFileCreate)
                {
                    case TypeFileCreate.SMO:
                        PathSchema = Path.Combine(LocalFolder, "EXPORT_SMO_XSD");
                        break;
                    case TypeFileCreate.MEK_P_P_SMO:
                        if (item.S_ZGLV_ID.Length == 0)
                            throw new Exception("Не найден указатель на акт");
                        PathSchema = Path.Combine(LocalFolder, "EXPORT_SMO_XSD");
                        break;
                    case TypeFileCreate.MEK_P_P_MO:
                        if (item.S_ZGLV_ID.Length == 0)
                            throw new Exception("Не найден указатель на акт");
                        PathSchema = Path.Combine(LocalFolder, "EXPORT_MO_XSD");
                        break;
                    case TypeFileCreate.SLUCH:
                        PathSchema = Path.Combine(LocalFolder, sluchParam.isSMO ? "EXPORT_SMO_XSD" : "EXPORT_FFOMS_XSD");
                        break;
                    case TypeFileCreate.MO:
                        PathSchema = Path.Combine(LocalFolder, "EXPORT_MO_XSD");
                        break;
                    case TypeFileCreate.FFOMSDx:
                    case TypeFileCreate.TEST_COVID:
                        PathSchema = Path.Combine(LocalFolder, "EXPORT_FFOMS_XSD");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(typeFileCreate), typeFileCreate, null);
                }

                var conn = exportFileRepository.CreateConnection();
                //Определение типа файла
                AddLogInvoke(progress, LogType.Info, "Определение типа файла");
                var fp = ParseFileName.Parse(item.FILENAME);
                if (fp.IsNull)
                {
                    throw new Exception("Ошибка при определении типа файла");
                }

                var PATH_XSD = "";
               

                var dtFile = new DateTime(item.YEAR_BASE, item.MONTH_BASE, 1);
               
                
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
                    case FileType.DA:
                    case FileType.DB:
                        PATH_XSD = Path.Combine(PathSchema, dtFile >= DT_2021_08 ? "D31_2021_08.xsd" : "D31.xsd");
                        break;
                    case FileType.H:
                        PATH_XSD = Path.Combine(PathSchema, dtFile >= DT_2022_01 ? "H32.xsd" : "H31.xsd");
                        if (typeFileCreate == TypeFileCreate.TEST_COVID)
                        {
                            PATH_XSD = Path.Combine(PathSchema, "HW32.xsd");
                        }
                        break;
                    case FileType.T:
                        PATH_XSD = Path.Combine(PathSchema, "T31.xsd");
                        break;
                    case FileType.C:
                        PATH_XSD = Path.Combine(PathSchema, "C31.xsd");
                        break;
                }

                var isSMO = !string.IsNullOrEmpty(SMO);

                AddLogInvoke(progress, LogType.Info, "Формирование файла");

                //Запрашиваем заголовки
                //ZGLV
                //Запрос





                AddLogInvoke(progress, LogType.Info, "Запрос заголовка");
                var ZGLV = exportFileRepository.V_EXPORT_H_ZGLV(item.ZGLV_ID, source, conn);
                if (ZGLV.Rows.Count != 1)
                {
                    throw new Exception($"Для файла {item.FILENAME} Вернулось больше 1го заголовка или 0");
                }

                //СЧЕТ----------------------------------              
                AddLogInvoke(progress, LogType.Info, "Запрос счета");

                var SCHET = exportFileRepository.V_EXPORT_H_SCHET(item.ZGLV_ID, source, conn);
                if (SCHET.Rows.Count != 1)
                {
                    throw new Exception($"Для файла {item.FILENAME} Вернулось больше 1го счета или 0");
                }
                //----------------------------------------------------
                AddLogInvoke(progress, LogType.Info, "Запрос услуг");
                var zglvid = typeFileCreate.In(TypeFileCreate.MEK_P_P_SMO, TypeFileCreate.MEK_P_P_MO) ? item.S_ZGLV_ID : new[] { item.ZGLV_ID };
                var onlyValidFFOMS = typeFileCreate == TypeFileCreate.FFOMSDx;
                var onlyDop = typeFileCreate.In(TypeFileCreate.MEK_P_P_SMO, TypeFileCreate.MEK_P_P_MO);
                var testCovid = typeFileCreate.In(TypeFileCreate.TEST_COVID);
                var USL = exportFileRepository.V_EXPORT_H_USL(zglvid, SMO, sluchParam.SLUCH_Z_ID, onlyValidFFOMS, onlyDop, testCovid, source, conn);
                
                if (USL.Rows.Count == 0)
                {
                    AddLogInvoke(progress, LogType.Info, $"Для файла {item.FILENAME} Вернулось 0 записей. СМО = {SMO}");
                    return FileCreatorResult.CreateNotResult();
                }

                AddLogInvoke(progress, LogType.Info, "Запрос данных об услугах");
                var MED_DEV = new DataTable();
                var MR_USL = new DataTable();
                foreach (var us in GetIDFromDataTable(USL, "USL_ID"))
                {
                    var items = us.ToList();
                    MR_USL.Merge(exportFileRepository.V_EXPORT_H_MR_USL(items, source, conn));
                    MED_DEV.Merge(exportFileRepository.V_EXPORT_H_MED_DEV(items, source, conn));
                }

              
              

                AddLogInvoke(progress, LogType.Info, "Запрос случаев");
                var SLUCH = new DataTable();
                foreach (var sl in GetIDFromDataTable(USL, "SLUCH_ID"))
                {
                    var items = sl.ToList();
                    SLUCH.Merge(exportFileRepository.V_EXPORT_H_SLUCH_BY_SLUCH_ID(items, source, conn));                
                }

                AddLogInvoke(progress, LogType.Info, "Запрос данных в случае");
                var NAZR = new DataTable();
                var DS2_N = new DataTable();
                var SL_KOEF = new DataTable();              
                var ONK_USLtbl = new DataTable();
                var LEK_PR = new DataTable();
                var LEK_PR_SL = new DataTable();
                var NAPR = new DataTable();
                var B_PROT = new DataTable();
                var H_CONS = new DataTable();
                var DS2 = new DataTable();
                var DS3 = new DataTable();
                var CRIT = new DataTable();             
                var B_DIAG = new DataTable();

                foreach (var sl in GetIDFromDataTable(SLUCH, "SLUCH_ID"))
                {
                    var items = sl.ToList();
                    NAZR.Merge(exportFileRepository.V_EXPORT_H_NAZR(items, source, conn));
                    DS2_N.Merge(exportFileRepository.V_EXPORT_H_DS2_N(items, source, conn));
                    SL_KOEF.Merge(exportFileRepository.V_EXPORT_H_SL_KOEF(items, source, conn));                 
                    ONK_USLtbl.Merge(exportFileRepository.V_EXPORT_ONK_USL(items, source, conn));
                    LEK_PR.Merge(exportFileRepository.V_EXPORT_LEK_PR(items, source, conn));
                    NAPR.Merge(exportFileRepository.V_EXPORT_H_NAPR(items, source, conn));
                    B_PROT.Merge(exportFileRepository.V_EXPORT_B_PROT(items, source, conn));
                    H_CONS.Merge(exportFileRepository.V_EXPORT_CONS(items, source, conn));
                    B_DIAG.Merge(exportFileRepository.V_EXPORT_B_DIAG(items, source, conn));
                    DS2.Merge(exportFileRepository.V_EXPORT_H_DS2(items, source, conn));
                    DS3.Merge(exportFileRepository.V_EXPORT_H_DS3(items, source, conn));
                    CRIT.Merge(exportFileRepository.V_EXPORT_H_CRIT(items, source, conn));
                    LEK_PR_SL.Merge(exportFileRepository.V_EXPORT_LEK_PR_SL(items, source, conn));
                }
               
               
                

                AddLogInvoke(progress, LogType.Info, "Запрос записей");
                var ZAP = new DataTable();
                var SANK = new DataTable();
                var EXPERTIZE = new DataTable();
                foreach (var sl in GetIDFromDataTable(SLUCH, "SLUCH_Z_ID"))
                {
                    ZAP.Merge(exportFileRepository.V_EXPORT_H_ZAP(sl, source, conn));
                    SANK.Merge(exportFileRepository.V_EXPORT_H_SANK(sl, source, conn));
                    if (isSMO)
                        EXPERTIZE.Merge(exportFileRepository.V_EXPORT_H_EXPERTIZE(sl, source, conn));
                }
                //L_ZGLV
                AddLogInvoke(progress, LogType.Info, "Запрос заголовка перс данных");
                var L_ZGLV = exportFileRepository.V_EXPORT_L_ZGLV(item.FILENAME, source, conn);
                if (L_ZGLV.Rows.Count != 1)
                {
                    throw new Exception("Запрос заголовка перс данных вернул более 1го или не одного заголовка");
                }

                //L_PERS

                AddLogInvoke(progress, LogType.Info, "Запрос персональных данных");
                var PERS = new DataTable();
                foreach (var sl in GetIDFromDataTable(ZAP, "PERS_ID"))
                {
                    PERS.Merge(exportFileRepository.V_EXPORT_L_PERS(sl, isSMO, source, conn));
                }

                AddLogInvoke(progress, LogType.Info, "Создание файла L");
                var fileL = CreateFileL(L_ZGLV, PERS);
                AddLogInvoke(progress, LogType.Info, "Создание файла H");
                var file = CreateFile(ZGLV, SCHET, ZAP, SLUCH, USL, NAZR, SANK, isSMO ? EXPERTIZE : null, SL_KOEF, DS2_N, NAPR, B_PROT, B_DIAG, H_CONS, ONK_USLtbl, LEK_PR, DS2, DS3, CRIT, MR_USL, MED_DEV, LEK_PR_SL);

                var month = fp.MM.PadLeft(2, '0');
                var Year = fp.YY;

                var newnameH = GetFileName(fp, Year, month, false, typeFileCreate.In(TypeFileCreate.SMO, TypeFileCreate.MEK_P_P_SMO) ? SMO : null, typeFileCreate == TypeFileCreate.FFOMSDx ? NN_FFOMS_DX : null, typeFileCreate == TypeFileCreate.TEST_COVID);
                var newnameL = GetFileName(fp, Year, month, true, typeFileCreate.In(TypeFileCreate.SMO, TypeFileCreate.MEK_P_P_SMO) ? SMO : null, typeFileCreate == TypeFileCreate.FFOMSDx ? NN_FFOMS_DX : null, typeFileCreate == TypeFileCreate.TEST_COVID);

                ModernFileH(file, newnameH, SMO, typeFileCreate, sluchParam);
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
                    if (typeFileCreate.In(TypeFileCreate.FFOMSDx, TypeFileCreate.TEST_COVID))
                    {
                        file.WriteXmlCustom(st);
                    }
                    else
                    {
                        file.WriteXml(st);
                    }
                    st.Close();
                }
                if (typeFileCreate.In(TypeFileCreate.TEST_COVID))
                {
                    ExtZLLIST.ChangeNamespace(pathfile);
                }
                
                AddLogInvoke(progress, LogType.Info, $"Сохранение файла {pathfileL}");
                using (var st = File.Create(pathfileL))
                {
                    fileL.WriteXml(st);
                    st.Close();
                }


                AddLogInvoke(progress, LogType.Info, "Проверка схемы файла персональных данных");
                var sc = new SchemaChecking();
                var CXL = new CheckXMLValidator(VersionMP.V3_1);
                CXL.P_INFO = new Dictionary<string, PacientInfo>();

                var L_XSD_PATH = Path.Combine(PathSchema, "L31.xsd");
                if (fileL.ZGLV.VERSION == "3.2")
                    L_XSD_PATH = Path.Combine(PathSchema, "L32.xsd");
                var err = sc.CheckXML(pathfileL, L_XSD_PATH, CXL);

                if (err.Count != 0)
                    AddLogInvoke(progress, LogType.Error, err.Select(x => x.MessageOUT).ToArray());



                AddLogInvoke(progress, LogType.Info, "Проверка схемы основного файла");            

                err = sc.CheckXML(pathfile, PATH_XSD, CXL);
              
                //2021 году не работала проверка схемы на C_ZAB
                if (Convert.ToInt32(SCHET.Rows[0]["YEAR"]) == 2021)
                {
                    err.RemoveAll(x => x.ERR_CODE.In("C_ZAB_EMPTY"));
                }

              

                
                if (err.Count != 0)
                {
                    if (!typeFileCreate.In(TypeFileCreate.FFOMSDx, TypeFileCreate.TEST_COVID) || err.Count(x => !x.ERR_CODE.In("ERR_ZS_1", "ERR_ZS_2")) != 0)
                    {
                        AddLogInvoke(progress, LogType.Error, err.Select(x => x.MessageOUT).ToArray());
                    }
                }
            

                var PATH_ARCIVE = Path.Combine(ExportFolder, $"{newnameH}.ZIP");
                if (typeFileCreate.In(TypeFileCreate.SMO, TypeFileCreate.MEK_P_P_SMO) && !string.IsNullOrEmpty(SMO))
                    PATH_ARCIVE = Path.Combine(ExportFolder, SMO, newnameH[0].ToString(), $"{newnameH}.ZIP");

                if (typeFileCreate.In(TypeFileCreate.FFOMSDx))
                {
                    if (!string.IsNullOrEmpty(sufix))
                    {
                        PATH_ARCIVE = Path.Combine(ExportFolder, newnameH.StartsWith("DP") || newnameH.StartsWith("DA") ? "1 этап" : "прочее", sufix, $"{newnameH}.ZIP");
                    }

                    else
                    {
                        PATH_ARCIVE = Path.Combine(ExportFolder, newnameH.StartsWith("DP") || newnameH.StartsWith("DA") ? "1 этап" : "прочее", $"{newnameH}.ZIP");
                    }
                }


                var dir = Path.GetDirectoryName(PATH_ARCIVE);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
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

                var res = FileCreatorResult.CreateResult(PATH_ARCIVE, file.SCHET.SUMMAV, SMO);
                if (typeFileCreate == TypeFileCreate.SLUCH)
                {
                    res.FileH = file;
                    res.FileL = fileL;
                }

                return res;
            }
            catch (Exception ex)
            {
                AddLogInvoke(progress, LogType.Error, $"Ошибка {ex.Source}: {ex.FullError()}");
                return FileCreatorResult.CreateNotResult();
            }
        }

        private void ModernFileH(ZL_LIST file, string FileName, string SMO, TypeFileCreate typeFileCreate, SLUCH_PARAM sluchParam)
        {
            switch (typeFileCreate)
            {
                case TypeFileCreate.SMO:
                case TypeFileCreate.MEK_P_P_SMO:
                    file.SCHET.PLAT = SMO;
                    break;
                case TypeFileCreate.MO:
                case TypeFileCreate.MEK_P_P_MO:
                    file.SCHET.PLAT = "75";
                    file.ClearSMO_DATA();
                    break;
                case TypeFileCreate.SLUCH:
                    if (sluchParam.isSMO)
                    {
                        file.SCHET.PLAT = SMO;
                    }
                    else
                    {
                        file.ClearForFFOMS_DATA();
                        file.SetENP_REG_IN_NPOLIS();
                    }
                    break;
                case TypeFileCreate.FFOMSDx:
                    file.SCHET.PLAT = SMO;
                    file.ZGLV.DATA = DateTime.Now.Date;
                    file.ClearForFFOMS_DATA();
                    break;
                case TypeFileCreate.TEST_COVID:
                    ChangeUslTestCovid(file);
                    file.ClearForFFOMS_DATA();
                    file.ClearSUMP();
                    file.ReCalcSUMV();
                    file.ZGLV.DATA = DateTime.Now.Date;
                    file.SCHET.PLAT = null;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(typeFileCreate), typeFileCreate, null);
            }

            file.SCHET.SUMMAV = file.ZAP.Sum(x => x.Z_SL.SL.Sum(sl => sl.USL.Sum(us => us.SUMV_USL)));
            var sumpList = file.ZAP.Select(x => x.Z_SL).Where(x => x.SUMP.HasValue).ToList();
            if (sumpList.Count != 0)
                file.SCHET.SUMMAP = sumpList.Sum(x => x.SUMP);
            var SANK_ITList = file.ZAP.Select(x => x.Z_SL).Where(x => x.SANK_IT.HasValue).ToList();
            if (SANK_ITList.Count != 0)
                file.SCHET.SANK_MEK = SANK_ITList.Sum(x => x.SANK_IT);
            file.ZGLV.FILENAME = FileName;
        }

        private void ChangeUslTestCovid(ZL_LIST file)
        {
            foreach (var zap in file.ZAP)
            {
                foreach (var sl in zap.Z_SL.SL)
                {
                    var remUsl = new List<USL>();
                    var addUsl = new List<USL>();
                    foreach (var us in sl.USL)
                    {                       
                        if(us.CODE_USL.In("611270","611271"))
                        {
                            var newUSL = new USL();
                            newUSL.CopyFrom(us);
                            newUSL.CODE_USL = "A26.08.027.001";
                            var newUSL1 = new USL();
                            newUSL1.CopyFrom(us);
                            newUSL1.TARIF = newUSL1.SUMV_USL  = 0;
                            newUSL1.CODE_USL = "A26.08.046.001";

                            remUsl.Add(us);
                            addUsl.Add(newUSL);
                            addUsl.Add(newUSL1);
                        }
                    }
                    foreach(var us in remUsl)
                    {
                        sl.USL.Remove(us);
                    }
                    sl.USL.AddRange(addUsl);
                    int idserv = 1;
                    foreach (var us in sl.USL)
                    {
                        us.IDSERV = idserv.ToString();
                        idserv++;
                    }
                }
            }
        }

        private void ModernFileL(PERS_LIST fileL, string newnameL, string newnameH)
        {
            fileL.ZGLV.FILENAME = newnameL;
            fileL.ZGLV.FILENAME1 = newnameH;
        }

        private static IEnumerable<IEnumerable<long>> GetIDFromDataTable(DataTable tbl, string column_name)
        {
            var items = tbl.Select().Select(x => Convert.ToInt64(x[column_name])).Distinct().ToList();
            return items.ToPartition(500);
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

        private ZL_LIST CreateFile(DataTable ZGLVtbl, DataTable SCHETtbl, DataTable ZAPtbl, DataTable SLUCHtbl, DataTable USLtbl, DataTable NAZRtbl, DataTable SANKtbl, DataTable Expertizetbl,
            DataTable KOEFtbl, DataTable DS2_Ntbl, DataTable NAPRtbl, DataTable B_PROTtbl, DataTable B_DIAGtbl, DataTable H_CONStbl, DataTable ONK_USLtbl, DataTable LEK_PRtbl,
            DataTable DS2, DataTable DS3, DataTable CRIT, DataTable MR_USL, DataTable MEDDEV, DataTable LEK_PR_SL)
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

                    if (Expertizetbl != null)
                    {
                        foreach (var exp_row in Expertizetbl.Select($"SLUCH_Z_ID = {z.Z_SL.SLUCH_Z_ID}"))
                        {
                            var exp = EXPERTISE.Get(exp_row);
                            z.Z_SL.EXPERTISE.Add(exp);
                        }
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
                            foreach (var mr_row in MR_USL.Select($"USL_ID = {us.USL_ID}"))
                            {
                                us.MR_USL_N.Add(MR_USL_N.Get(mr_row));
                            }
                            foreach (var mr_row in MEDDEV.Select($"USL_ID = {us.USL_ID}"))
                            {
                                us.MED_DEV.Add(MED_DEV.Get(mr_row));
                            }
                            sl.USL.Add(us);
                        }



                        step = 4;
                        sl.LEK_PR = new List<LEK_PR_H>();
                        foreach (var lek_pr_sl_row in LEK_PR_SL.Select($"SLUCH_ID = {sl.SLUCH_ID}"))
                        {
                            
                            sl.LEK_PR.Add(LEK_PR_H.Get(lek_pr_sl_row));
                        }

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

        private string GetFileName(MatchParseFileName fp, string Year, string month, bool IsFileL, string SMO, string NN_FFOMS_DX,bool isHW)
        {
            var type = fp.FILE_TYPE.ToFileType();
            var newnameH = $"{fp.FILE_TYPE}{(isHW? "W":"")}{fp.Pi}{fp.Ni}{(string.IsNullOrEmpty(SMO) ? "T75" : $"S{SMO}")}_{Year}{month}{NN_FFOMS_DX ?? $"{fp.NN}"}";
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
                        /*countSheet++;
                        efm.AddSheet($"Реестр {countSheet}");
                        foreach (var col in baseCOL)
                        {
                            efm.SetColumnWidth(col.Key, col.Value.Col.Width);
                        }

                        efm.CreateRow(1, H1.r.OuterXml);
                        efm.CreateRow(2, H2.r.OuterXml);
                        currRows = 3;*/
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


        public void CreateFile(V_EXPORT_H_ZGLVRowVM item, string Folder, DBSource source, TypeFileCreate typeFileCreate, SLUCH_PARAM sluchParam, List<F002> smoList, int OrderInMonth, int Order)
        {
            try
            {
                var result = new List<FileCreatorResult>();
                var pr = new Action<ProgressFileCreator>(pfc =>
                {
                    DispatcherHelper.Dispatcher.Invoke(() =>
                    {
                        foreach (var mes in pfc.Message)
                        {
                            item.Logs.Add(new LogItem(pfc.Type, mes));
                        }
                    });
                });

                if (smoList == null)
                    result.Add(GetFileXML(item.Item, Folder, source, typeFileCreate, item.Item.SMO, sluchParam, null, "", pr));
                else
                {
                    var i = 1;
                    foreach (var smo in smoList)
                    {
                        var r = GetFileXML(item.Item, Folder, source, typeFileCreate, smo.SMOCOD, sluchParam, $"{OrderInMonth}{Order}{i}", Order != 1 || i != 1 ? $"ПОРЯДОК{Order}" : "", pr);
                        result.Add(r);
                        if (r.Result)
                        {
                            i++;
                        }
                    }
                }

                DispatcherHelper.Dispatcher.Invoke(() =>
                {
                    var validResult = result.Where(x => x.Result).ToList();
                    item.SUM = validResult.Sum(x => x.SUM);
                    item.PathArc.AddRange(validResult.Select(x => x.PathARC).ToList());
                    item.Results.AddRange(validResult);
                });
            }
            catch (Exception ex)
            {
                DispatcherHelper.Dispatcher.Invoke(() => { item.AddLogs(LogType.Error, $"Ошибка при выгрузке данных: {ex.Message}"); });
            }
        }

        public Task CreateFileAsync(V_EXPORT_H_ZGLVRowVM item, string Folder, DBSource source, TypeFileCreate typeFileCreate, SLUCH_PARAM sluchParam, List<F002> smoList, int OrderInMonth, int Order)
        {
            return new Task(() => CreateFile(item, Folder, source, typeFileCreate, sluchParam, smoList, OrderInMonth, Order));
        }
    }


    public class ProgressItemDouble
    {
        public ProgressItem progress1 { get; set; } = new ProgressItem();
        public ProgressItem progress2 { get; set; } = new ProgressItem();

    }

    public interface IFileCombiner
    {
        void CreateSMOMail(List<V_EXPORT_H_ZGLVRowVM> items, string folder, TypeFileCreate typeFileCreate, CancellationToken cancel, IProgress<ProgressItemDouble> progress);
        void CreateMOMail(List<V_EXPORT_H_ZGLVRowVM> items, string folder, TypeFileCreate typeFileCreate, CancellationToken cancel, IProgress<ProgressItemDouble> progress);
        void CreateSolidFile(List<V_EXPORT_H_ZGLVRowVM> items, string folder, string newFileName);
    }


    /// <summary>
    /// Сборщик файлов в посылки в организации 
    /// </summary>
    public class FileCombiner: IFileCombiner
    {
        /// <summary>
        /// Формировать пакет для СМО
        /// </summary>
        /// <param name="items"></param>
        /// <param name="folder"></param>
        /// <param name="typeFileCreate"></param>
        /// <param name="cancel"></param>
        /// <param name="progress"></param>
        public void CreateSMOMail(List<V_EXPORT_H_ZGLVRowVM> items, string folder, TypeFileCreate typeFileCreate, CancellationToken cancel, IProgress<ProgressItemDouble> progress)
        {
            var progressItem = new ProgressItemDouble();
            var groupList = items.SelectMany(x => x.Results.Select(y => new { x.Item.YEAR_SANK, x.Item.MONTH_SANK, Result = y })).Where(x => x.Result.Result).GroupBy(x => new { x.Result.SMO, x.YEAR_SANK, x.MONTH_SANK }).ToList();
            var countGr = groupList.Count;
            var i = 1;
            progressItem.progress1.SetValues(countGr, 0, "Сбор файлов в архив");
            progress?.Report(progressItem);

            var removePath = new List<string>();
            foreach (var gr in groupList)
            {
                cancel.ThrowIfCancellationRequested();
                var nameArchive = $"{(typeFileCreate == TypeFileCreate.SMO ? "Реестры" : "МЭК прошлых периодов")} {gr.Key.SMO} за {gr.Key.MONTH_SANK:D2}.{gr.Key.YEAR_SANK}.ZIP";
                var pathArchive = Path.Combine(folder, nameArchive);
                progressItem.progress1.SetTextValue(i, $"Сбор файлов в архив: {nameArchive}");
                progress?.Report(progressItem);

                using (var archive = ZipFile.Open(pathArchive, ZipArchiveMode.Create))
                {
                    foreach (var item in gr)
                    {
                        var file = item.Result.PathARC;
                        progressItem.progress2.Text = $"Добавление {file}";
                        progress?.Report(progressItem);
                        archive.CreateEntryFromFile(file, Path.GetFileName(file));
                        File.Delete(file);
                        removePath.Add(file);
                    }
                }
                i++;
            }
            progressItem.progress2.Text = "Очистка каталогов";
            progress?.Report(progressItem);
            FilesHelper.RemoveFileAndDirAsync(folder, removePath.ToArray());
            progressItem.progress2.Clear();
            progressItem.progress1.Clear();
            progress?.Report(progressItem);
        }
        public void CreateMOMail(List<V_EXPORT_H_ZGLVRowVM> items, string folder, TypeFileCreate typeFileCreate, CancellationToken cancel, IProgress<ProgressItemDouble> progress)
        {
            var progressItem = new ProgressItemDouble();
            var groupList = items.Where(x => x.PathArc.Count != 0).GroupBy(x => new { x.Item.YEAR_SANK, x.Item.MONTH_SANK, x.Item.CODE_MO }).ToList();
            var countGr = groupList.Count;
            var i = 1;
            progressItem.progress1.SetValues(countGr, 0, "Сбор файлов в архив");
            progress?.Report(progressItem);
            foreach (var gr in groupList)
            {
                cancel.ThrowIfCancellationRequested();
                var nameArchive = $"{(typeFileCreate == TypeFileCreate.MO ? "Результаты МЭК" : "МЭК прошлых периодов")} {gr.Key.CODE_MO} за {gr.Key.MONTH_SANK:D2}.{gr.Key.YEAR_SANK}.ZIP";
                var pathArchive = Path.Combine(folder, nameArchive);

                progressItem.progress1.SetTextValue(i, $"Сбор файлов в архив: {nameArchive}");
                progress?.Report(progressItem);


                using (var archive = ZipFile.Open(pathArchive, ZipArchiveMode.Create))
                {
                    foreach (var item in gr)
                    {
                        foreach (var file in item.PathArc)
                        {
                            progressItem.progress2.Text = $"Добавление {file}";
                            progress?.Report(progressItem);
                            archive.CreateEntryFromFile(file, Path.GetFileName(file));
                            File.Delete(file);
                        }
                    }
                }
                i++;
            }
            progressItem.progress2.Clear();
            progressItem.progress1.Clear();
            progress?.Report(progressItem);
        }
        public void CreateSolidFile(List<V_EXPORT_H_ZGLVRowVM> items, string folder,string newFileName)
        {
            var file = new ZL_LIST
            {
                SCHET = new SCHET
                {
                    CODE = 0,
                    CODE_MO = "0",
                    DSCHET = DateTime.Now,
                    NSCHET = "0",
                    PLAT = "75",
                    MONTH = DateTime.Now.Month,
                    YEAR = DateTime.Now.Year
                },
                ZGLV = new ZGLV
                {
                    DATA = DateTime.Now,
                    FILENAME = newFileName,
                    VERSION = "3.1"

                }
            };
            foreach (var item in items.SelectMany(x => x.Results).Select(x => x.FileH))
            {
                file.ZAP.AddRange(item.ZAP);
            }

            var i = 1;
            foreach (var z in file.ZAP)
            {
                z.N_ZAP = i;
                i++;
                if (z.Z_SL.SLUCH_Z_ID.HasValue)
                    z.Z_SL.IDCASE = z.Z_SL.SLUCH_Z_ID.Value;
            }
            file.SCHET.SUMMAV = file.ZAP.Sum(x => x.Z_SL.SL.Sum(sl => sl.USL.Sum(us => us.SUMV_USL)));
            file.SCHET.SUMMAP = file.ZAP.Sum(x => x.Z_SL.SUMP);
            file.SCHET.SANK_MEK = file.ZAP.Sum(x => x.Z_SL.SANK.Sum(san => san.S_SUM));


            file.ZGLV.SD_Z = file.ZAP.Count;
            using (var st = File.Create(Path.Combine(folder, $"{newFileName}.xml")))
            {
                file.WriteXml(st);
                st.Close();
            }
        }
    }



    public static class Ext
    {
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
        {
            for (var i = 0; i < Math.Ceiling(source.Count / (double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }
    }
}

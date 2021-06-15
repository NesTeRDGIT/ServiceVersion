using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.Office.Interop.Excel;
using ServiceLoaderMedpomData.EntityMP_V31;
using DataTable = System.Data.DataTable;

namespace ServiceLoaderMedpomData
{

    public class TransferTableRESULT
    {
        public string Table { get; set; }
        public List<string> Colums { get; set; }
    }

    public class V_XML_CHECK_FILENAMErow
    {
        public static V_XML_CHECK_FILENAMErow Get(DataRow row)
        {
            try
            {
                var item = new V_XML_CHECK_FILENAMErow();
                item.FILENAME = Convert.ToString(row["FILENAME"]);
                item.CODE = Convert.ToString(row["CODE"]);
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.DOP_FLAG = Convert.ToBoolean(row["DOP_FLAG"]);
                item.YEAR = Convert.ToInt32(row["YEAR"]);
                item.MONTH = Convert.ToInt32(row["MONTH"]);
                item.NSCHET = Convert.ToString(row["NSCHET"]);
                item.DSCHET = Convert.ToDateTime(row["DSCHET"]);
                item.ZGLV_ID = Convert.ToInt32(row["ZGLV_ID"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения V_XML_CHECK_FILENAME: {ex.Message}", ex);
            }
        }
        public string FILENAME { get; set; }
        public string CODE { get; set; }
        public string CODE_MO { get; set; }
        public bool DOP_FLAG { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string NSCHET { get; set; }
        public DateTime DSCHET { get; set; }
        public int ZGLV_ID { get; set; }



    }

    public class V_ErrorViewRow
    {
        public static V_ErrorViewRow Get(DataRow row)
        {
            try
            {
                var item = new V_ErrorViewRow();
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToInt32(row["SLUCH_ID"]);
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.FAM = Convert.ToString(row["FAM"]);
                item.IM = Convert.ToString(row["IM"]);
                item.OT = Convert.ToString(row["OT"]);
                if (row["DR"] != DBNull.Value)
                    item.DR = Convert.ToString(row["DR"]);
                item.ID_PAC = Convert.ToString(row["ID_PAC"]);
                if (row["VPOLIS"] != DBNull.Value)
                    item.VPOLIS = Convert.ToInt32(row["VPOLIS"]);
                item.SPOLIS = Convert.ToString(row["SPOLIS"]);
                item.NPOLIS = Convert.ToString(row["NPOLIS"]);
                item.LPU_1 = Convert.ToString(row["LPU_1"]);
                item.NHISTORY = Convert.ToString(row["NHISTORY"]);
                if (row["USL_OK"] != DBNull.Value)
                    item.USL_OK = Convert.ToInt32(row["USL_OK"]);
                if (row["DATE_1"] != DBNull.Value)
                    item.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                if (row["DATE_2"] != DBNull.Value)
                    item.DATE_2 = Convert.ToDateTime(row["DATE_2"]);
                item.IDDOKT = Convert.ToString(row["IDDOKT"]);
                item.ERR = Convert.ToString(row["ERR"]);
                item.ERR_TYPE = Convert.ToInt32(row["ERR_TYPE"]);
                item.FILENAME = Convert.ToString(row["FILENAME"]);
                item.BAS_EL = Convert.ToString(row["BAS_EL"]);
                item.N_ZAP = Convert.ToString(row["N_ZAP"]);
                item.IDCASE = Convert.ToString(row["IDCASE"]);
                item.SL_ID = Convert.ToString(row["SL_ID"]);
                item.ID_SERV = Convert.ToString(row["ID_SERV"]);
                item.OSHIB = Convert.ToInt32(row["OSHIB"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения V_ErrorViewRow: {ex.Message}", ex);
            }
        }
        public int? SLUCH_ID { get; set; }
        public string CODE_MO { get; set; }
        public string FAM { get; set; }
        public string IM { get; set; }
        public string OT { get; set; }
        public string DR { get; set; }
        public string ID_PAC { get; set; }
        public int? VPOLIS { get; set; }
        public string SPOLIS { get; set; }
        public string NPOLIS { get; set; }
        public string LPU_1 { get; set; }
        public string NHISTORY { get; set; }
        public int? USL_OK { get; set; }
        public DateTime? DATE_1 { get; set; }
        public DateTime? DATE_2 { get; set; }
        public string IDDOKT { get; set; }
        public string ERR { get; set; }
        public int? ERR_TYPE { get; set; }
        public string FILENAME { get; set; }
        public string BAS_EL { get; set; }
        public string N_ZAP { get; set; }
        public string IDCASE { get; set; }
        public string SL_ID { get; set; }
        public string ID_SERV { get; set; }
        public int OSHIB { get; set; }
    }

    public class SVOD_FILE_Row
    {
        public static SVOD_FILE_Row Get(DataRow row)
        {
            try
            {
                var item = new SVOD_FILE_Row();
                item.FILENAME = Convert.ToString(row["FILENAME"]);
                if (row["SUM"] != DBNull.Value)
                    item.SUM = Convert.ToDecimal(row["SUM"]);
                if (row["SUM_MEK"] != DBNull.Value)
                    item.SUM_MEK = Convert.ToDecimal(row["SUM_MEK"]);
                if (row["CSLUCH"] != DBNull.Value)
                    item.CSLUCH = Convert.ToInt32(row["CSLUCH"]);
                if (row["CUSL"] != DBNull.Value)
                    item.CUSL = Convert.ToInt32(row["CUSL"]);
                item.COM = Convert.ToString(row["COM"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения SVOD_FILE_Row: {ex.Message}", ex);
            }
        }
        public string FILENAME { get; set; }
        public decimal? SUM { get; set; }
        public decimal? SUM_MEK { get; set; }
        public int? CSLUCH { get; set; }
        public int? CUSL { get; set; }
        public string COM { get; set; }
    }

    public class STAT_VIDMP_Row
    {
        public static STAT_VIDMP_Row Get(DataRow row)
        {
            try
            {
                var item = new STAT_VIDMP_Row();
                item.PS = Convert.ToString(row["PS"]);
                item.NAME = Convert.ToString(row["NAME"]);
                item.C_ZS = Convert.ToInt32(row["C_ZS"]);
                item.C_SL = Convert.ToInt32(row["C_SL"]);
                item.SUMV = Convert.ToDecimal(row["SUMV"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения STAT_VIDMP_Row: {ex.Message}", ex);
            }
        }
        public string PS { get; set; }
        public string NAME { get; set; }
        public int C_ZS { get; set; }
        public int C_SL { get; set; }
        public decimal SUMV { get; set; }
    }
    public class STAT_FULL_Row
    {
        public static STAT_FULL_Row Get(DataRow row)
        {
            try
            {
                var item = new STAT_FULL_Row();
                item.USL_OK = Convert.ToString(row["USL_OK"]);
                item.N_KSG = Convert.ToString(row["N_KSG"]);
                item.KSG = Convert.ToString(row["KSG"]);
                item.SL = Convert.ToInt32(row["SL"]);
                item.KOL_USL = Convert.ToDecimal(row["KOL_USL"]);
                item.SUMV = Convert.ToDecimal(row["SUMV"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения STAT_FULL_Row: {ex.Message}", ex);
            }
        }
        public string USL_OK { get; set; }
        public string N_KSG { get; set; }
        public string KSG { get; set; }
        public int SL { get; set; }
        public decimal SUMV { get; set; }
        public decimal KOL_USL { get; set; }
    }


    public class F014Row
    {
        public static List<F014Row> Get(IEnumerable<DataRow> rows)
        {
            try
            {
                return rows.Select(Get).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получение F014Row:{ex.Message}", ex);
            }
        }

        private static F014Row Get(DataRow row)
        {
            try
            {
                var item = new F014Row
                {
                    KOD = Convert.ToInt32(row["KOD"]),
                    DATEBEG = Convert.ToDateTime(row["DATEBEG"])
                };
                if (row["DATEEND"] != DBNull.Value)
                    item.DATEEND = Convert.ToDateTime(row["DATEEND"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получение F014Row:{ex.Message}", ex);
            }
        }

        public int KOD { get; set; }
        public DateTime DATEBEG { get; set; }
        public DateTime? DATEEND { get; set; }
    }
    public class F006Row
    {
        public static List<F006Row> Get(IEnumerable<DataRow> rows)
        {
            try
            {
                return rows.Select(row => Get(row)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получение F006Row:{ex.Message}", ex);
            }
        }

        public static F006Row Get(DataRow row)
        {
            try
            {
                var item = new F006Row();
                item.IDVID = Convert.ToInt32(row["IDVID"]);
                item.DATEBEG = Convert.ToDateTime(row["DATEBEG"]);
                if (row["DATEEND"] != DBNull.Value)
                    item.DATEEND = Convert.ToDateTime(row["DATEEND"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получение F006Row:{ex.Message}", ex);
            }
        }

        public int IDVID { get; set; }
        public DateTime DATEBEG { get; set; }
        public DateTime? DATEEND { get; set; }
    }
    public class ExpertRow
    {
        public static List<ExpertRow> Get(IEnumerable<DataRow> rows)
        {
            try
            {
                return rows.Select(row => Get(row)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получение F006Row:{ex.Message}", ex);
            }
        }

        public static ExpertRow Get(DataRow row)
        {
            try
            {
                var item = new ExpertRow();
                item.N_EXPERT = Convert.ToString(row["N_EXPERT"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получение ExpertRow:{ex.Message}", ex);
            }
        }

        public string N_EXPERT { get; set; }
    }

    public interface IRepository : IDisposable
    {
        void InsertFile(ZL_LIST zl, PERS_LIST pe);
        void TruncALL();
        Task<bool> DeleteTemp100TASK(string code_mo);
        TransferTableRESULT TransferTable(string tableFrom, string ownerFrom, string tableTo, string ownerTo);
        void BeginTransaction();
        void Commit();
        void Rollback();
        List<V_ErrorViewRow> GetErrorView();
        List<SVOD_FILE_Row> SVOD_FILE_TEMP99();
        List<STAT_VIDMP_Row> STAT_VIDMP_TEMP99();
        List<STAT_FULL_Row> STAT_FULL_TEMP99();
        void Checking(TableName nameTBL, CheckingList list, CancellationToken cancel, ref string STAT);
        string GetNameLPU(string R_COD);
        List<V_XML_CHECK_FILENAMErow> GetZGLV_BYFileName(string FileName);
        List<V_XML_CHECK_FILENAMErow> GetZGLV_BYCODE_CODE_MO(int code, string MO, int YEAR);
        List<FindSluchItem> Get_IdentInfo(ZL_LIST ZL, FileItem fi, Dispatcher dispatcher);
        List<F006Row> GetF006();
        List<F014Row> GetF014();
        List<ExpertRow> GetEXPERTS();
        Dictionary<long, List<FindSANKItem>> GetSank(ZL_LIST ZL, FileItem fi, Dispatcher dispatcher = null);
        List<FindSANKItem> FindACT(string NUM_ACT, DateTime D_ACT, string SMO);
        int AddSankZGLV(string FILENAME, int CODE, int CODE_MO, int FLAG_MEE, int YEAR, int MONTH, int YEAR_SANK, int MONTH_SANK, int ZGLV_ID_BASE, string SMO, bool DOP_FLAG, bool isNotFinish);
        int UpdateSLUCH_Z_SANK_ZGLV_ID(IEnumerable<Z_SL> Items, decimal SANK_ZGLV_ID, bool isRewrite = false);
        void UpdateSankZGLV(int ZGLV_ID, int ZGLV_ID_BASE);
        bool LoadSANK(FileItem fi, ZL_LIST ZL, decimal? S_ZGLV_ID, bool setSUMP, bool isRewrite, Dispatcher dispatcher = null, List<FindSluchItem> IdentInfo = null);
    }

   

    public enum TableName
    {
        /// <summary>
        /// Таблица заголовков
        /// </summary>
        ZGLV,
        /// <summary>
        /// Таблица счета
        /// </summary>
        SCHET,
        /// <summary>
        /// Таблица записей
        /// </summary>
        ZAP,
        /// <summary>
        /// Таблица пациентов
        /// </summary>
        PACIENT,
        /// <summary>
        /// Таблица случаев
        /// </summary>
        SLUCH,
        /// <summary>
        /// Таблица санкций
        /// </summary>
        SANK,
        /// <summary>
        /// Таблица услуг
        /// </summary>
        USL,
        /// <summary>
        /// Таблица заголовков персональных данных
        /// </summary>
        L_ZGLV,
        /// <summary>
        /// Таблица персональных данных
        /// </summary>
        L_PERS,
        DS2_N,
        NAZR,
        KSLP,
        SLUCH_Z,
        B_DIAG,
        B_PROT,
        NAPR,
        ONK_USL,
        CONS,
        LEK_PR,
        LEK_PR_DATE_INJ,
        EXP,
        DS2,
        DS3,
        CRIT
    }
    public class TableInfo
    {
        public string TableName { get; set; } = "";
        public string SeqName { get; set; } = "";
        public string SchemaName { get; set; } = "";
        public string FullTableName => $"{(string.IsNullOrEmpty(SchemaName) ? "" : $"{SchemaName}.")}{TableName}";
    }
    public class FindSluchItem
    {
        public static FindSluchItem Get(DataRow row)
        {
            try
            {
                var item = new FindSluchItem
                {
                    SLUCH_Z_ID = Convert.ToInt64(row["SLUCH_Z_ID"]),
                    SLUCH_ID = Convert.ToInt64(row["SLUCH_ID"]),
                    SL_ID = Convert.ToString(row["SL_ID"]),
                    IDCASE = Convert.ToInt64(row["IDCASE"]),
                    DATE_1 = Convert.ToDateTime(row["DATE_1"]),
                    DATE_2 = Convert.ToDateTime(row["DATE_2"]),
                    NHISTORY = Convert.ToString(row["NHISTORY"]),
                    SUM_M = Convert.ToDecimal(row["SUM_M"]),
                    DS1 = Convert.ToString(row["DS1"])
                };
                if (row["USL_OK"] != DBNull.Value)
                    item.USL_OK = Convert.ToInt32(row["USL_OK"]);
                if (row["RSLT"] != DBNull.Value)
                    item.RSLT = Convert.ToInt32(row["RSLT"]);
                if (row["SLUCH_Z_ID_MAIN"] != DBNull.Value)
                    item.SLUCH_Z_ID_MAIN = Convert.ToInt64(row["SLUCH_Z_ID_MAIN"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения FindSluchItem:{e.Message}", e);
            }
        }

        public FindSluchItem()
        {

        }
        public long SLUCH_ID { get; set; }
        public long SLUCH_Z_ID { get; set; }
        public long? SLUCH_Z_ID_MAIN { get; set; }
        public string SL_ID { get; set; }
        public long IDCASE { get; set; }
        public DateTime DATE_1 { get; set; }
        public DateTime DATE_2 { get; set; }
        public int? USL_OK { get; set; }
        public string NHISTORY { get; set; }
        public decimal SUM_M { get; set; }
        public string DS1 { get; set; }
        public int? RSLT { get; set; }

    }

    public enum TFindSANKItem
    {
        /// <summary>
        /// Санкции на текущий случай
        /// </summary>
        Main,
        /// <summary>
        /// Санкция родителя
        /// </summary>
        Parent,
        /// <summary>
        /// Санкция исправленных случаев
        /// </summary>
        Child,
        /// <summary>
        /// Братские санкции(случай исправленный, санкции других исправленных)
        /// </summary>
        Brother
    }

    public class FindSANKItem
    {
        public static FindSANKItem Get(DataRow row)
        {
            try
            {
                var item = new FindSANKItem
                {
                    SANK_ID = Convert.ToInt64(row["SANK_ID"]),
                    SLUCH_Z_ID = Convert.ToInt64(row["SLUCH_Z_ID"]),
                    S_SUM = Convert.ToDecimal(row["S_SUM"]),
                    S_TIP = Convert.ToInt32(row["S_TIP"]),
                    S_OSN = Convert.ToInt32(row["S_OSN"]),
                    DATE_ACT = Convert.ToDateTime(row["DATE_ACT"]),
                    NUM_ACT = Convert.ToString(row["NUM_ACT"]),
                    YEAR_SANK = Convert.ToInt32(row["YEAR_SANK"]),
                    MONTH_SANK = Convert.ToInt32(row["MONTH_SANK"]),
                    FILENAME = Convert.ToString(row["FILENAME"]),
                    DATE_INVITE = Convert.ToDateTime(row["DATE_INVITE"])
                };
                if (row["SLUCH_Z_ID_MAIN"] != DBNull.Value)
                    item.SLUCH_Z_ID_MAIN = Convert.ToInt64(row["SLUCH_Z_ID_MAIN"]);

                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения FindSANKItem:{e.Message}", e);
            }
        }

        public TFindSANKItem Type { get; set; }
        public long SANK_ID { get; set; }
        public long SLUCH_Z_ID { get; set; }
        public long? SLUCH_Z_ID_MAIN { get; set; }
        public decimal S_SUM { get; set; }
        public int S_TIP { get; set; }
        public int S_OSN { get; set; }
        public DateTime DATE_ACT { get; set; }
        public string NUM_ACT { get; set; }
        public int YEAR_SANK { get; set; }
        public int MONTH_SANK { get; set; }
        public string FILENAME { get; set; }
        public DateTime DATE_INVITE { get; set; }


    }


    public static partial class Ext
    {
        public static List<List<long>> ToPartition(this List<long> items, int Count)
        {
            var result = new List<List<long>>();
            var tempList = new List<long>();
            foreach (var item in items)
            {
                tempList.Add(item);
                if (tempList.Count == Count)
                {
                    result.Add(tempList);
                    tempList = new List<long>();
                }
            }
            if (tempList.Count != 0)
                result.Add(tempList);
            return result;
        }
    }
}

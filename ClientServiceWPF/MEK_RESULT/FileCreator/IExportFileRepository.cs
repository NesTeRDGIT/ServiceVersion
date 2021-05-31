using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.MEK_RESULT.FileCreator
{
    public interface IExportFileRepository
    {
        List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_ZGLV(bool IsTemp1, int Month, int Year);
        DataTable V_EXPORT_H_ZGLV(int zglvid, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_SCHET(int zglvid, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_ZAP(int zglvid, string SMO, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_SLUCH(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_SANK(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_NAZR(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_DS2_N(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_SL_KOEF(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_USL(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_ONK_USL(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_LEK_PR(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_NAPR(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_B_PROT(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_B_DIAG(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_DS2(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_DS3(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_H_CRIT(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_CONS(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_L_ZGLV(string FILENAME, bool IsTemp1, OracleConnection conn = null);
        DataTable V_EXPORT_L_PERS(int zglvid, string SMO, bool IsTemp1, OracleConnection conn = null);
        OracleConnection CreateConnection();
        List<F002> GetF002();
        string GetF003Name(string CODE_MO);
        List<XLS_TABLE> V_EXPORT_EXCEL_FROM(IEnumerable<int> ZGLV_ID, string SMO);
    }

    public class ExportFileRepository : IExportFileRepository
    {
        private string ConnectionString { get; }
        public ExportFileRepository(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        public List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_ZGLV(bool IsTemp1, int Month, int Year)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_ZGLV{(IsTemp1 ? "_TEMP1" : "")} t where t.month = {Month}  and  t.year = {Year}", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Select().Select(V_EXPORT_H_ZGLVRow.Get).ToList();
                }
            }
        }
        public DataTable V_EXPORT_H_ZGLV(int zglvid, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_H_ZGLV{(IsTemp1 ? "_TEMP1" : "")} t where zglv_id = {zglvid}", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if(conn==null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_SCHET(int zglvid, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_H_SCHET{(IsTemp1 ? "_TEMP1" : "")} t where zglv_id = {zglvid}", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_ZAP(int zglvid, string SMO, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_H_ZAP{(IsTemp1 ? "_TEMP1" : "")} t where zglv_id = {zglvid} and {(!string.IsNullOrEmpty(SMO) ? $"SMO = '{SMO}'" : "IsZK = 1")}", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_SLUCH(IEnumerable<long> sl, bool IsTemp1,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_SLUCH{(IsTemp1 ? "_TEMP1" : "")} t where SLUCH_Z_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_SANK(IEnumerable<long> sl, bool IsTemp1,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_SANK{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_Z_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_NAZR(IEnumerable<long> sl, bool IsTemp1,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_NAZR{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_DS2_N(IEnumerable<long> sl, bool IsTemp1,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS2_N{(IsTemp1 ? "_TEMP1" : "")} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_SL_KOEF(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_SL_KOEF{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_USL(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_USL{(IsTemp1 ? "_TEMP1" : "")} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_ONK_USL(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_ONK_USL{(IsTemp1 ? "_TEMP1" : "")} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_LEK_PR(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_LEK_PR{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_NAPR(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_NAPR{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_B_PROT(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_B_PROT{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_B_DIAG(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_B_DIAG{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_DS2(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS2{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_DS3(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS3{(IsTemp1 ? "_TEMP1" : "")} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_H_CRIT(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_CRIT{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_CONS(IEnumerable<long> sl, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_CONS{(IsTemp1 ? "_TEMP1" : "")}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_L_ZGLV(string FILENAME, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_L_ZGLV{(IsTemp1 ? "_TEMP1" : "")} t where t.FILENAME1 = '{FILENAME}'", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }
        public DataTable V_EXPORT_L_PERS(int zglvid,string SMO, bool IsTemp1, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                var isSMO = !string.IsNullOrEmpty(SMO);
                var PRED = isSMO ? $"SMO = '{SMO}'" : "IsZK = 1";
                using (var oda = new OracleDataAdapter($@"select distinct h_zglv_id, filename1, pers_id, zglv_id, id_pac, fam, im, ot, w, dr, fam_p, im_p, ot_p, w_p, dr_p, mr, doctype, docser, docnum, snils, okatog, okatop, dost, dost_p, fam_tfoms, im_tfoms, ot_tfoms, dr_tfoms, rokato, renp, rqogrn, rdbeg, tel, {(isSMO ? "comentp_new comentp" : "comentp")}, docdate, docorg, iszk from V_EXPORT_L_PERS{(IsTemp1 ? "_TEMP1" : "")} t where {PRED}  and H_ZGLV_ID = {zglvid}", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
            catch (Exception)
            {
                if (conn == null)
                    con.Dispose();
                throw;
            }
        }

        public OracleConnection CreateConnection()
        {
            return new OracleConnection(ConnectionString);
        }

        public List<F002> GetF002()
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($@"select t.* from NSI.F002 t where t.TF_OKATO = '76000' and sysdate between t.d_begin and  nvl(t.d_end,sysdate)", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return F002.Get(tbl.Select());
                }
            }
        }
        public string  GetF003Name(string CODE_MO)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($@"select NAM_MOK ||' ОГРН: '|| ogrn NAME from nsi.f003 t  where t.mcod = '{CODE_MO}'", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Rows[0]["Name"].ToString();
                }
            }
        }

        public List<XLS_TABLE> V_EXPORT_EXCEL_FROM(IEnumerable<int> ZGLV_ID, string SMO)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from v_export_excel_from where h_zglv_id in ({string.Join(",", ZGLV_ID)}) and smo = '{SMO}'", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return XLS_TABLE.Get(tbl.Select());
                }
            }
        }

    }


    public class V_EXPORT_H_ZGLVRow
    {
        
        public int ZGLV_ID { get; set; }
        public string CODE_MO { get; set; }
        public string FILENAME { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public static List<V_EXPORT_H_ZGLVRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static V_EXPORT_H_ZGLVRow Get(DataRow row)
        {
            try
            {
                var item = new V_EXPORT_H_ZGLVRow
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
                throw new Exception($"Ошибка получения V_EXPORT_H_ZGLVRow: {ex.Message}", ex);
            }
        }
    }

    public class F002
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
                throw new Exception($"Ошибка получения F002: {ex.Message}", ex);
            }
        }

        public string SMOCOD { get; set; }

    }
    public class XLS_TABLE
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
                if (row["VIDPOM"] != DBNull.Value)
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
        public int USL_OK { get; set; }
        public int? RSLT { get; set; }
    }
}

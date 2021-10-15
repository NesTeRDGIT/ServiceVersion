using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.MEK_RESULT.FileCreator
{
    public enum DBSource
    {
        TEMP100,
        TEMP1,
        MAIN
    }

    public enum TypeFileCreate
    {
        SMO,
        MO,
        SLUCH,
        FFOMSDx,
        MEK_P_P_SMO,
        MEK_P_P_MO
    }

    public class PERIOD_PARAM
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class SLUCH_PARAM
    {
        public long[] SLUCH_Z_ID { get; set; }
        public bool isSMO { get; set; }
        public bool OneFile { get; set; }
        public string NewFileName { get; set; }
    }


    public static class ExtDBSource
    {
        public static string ToPrefixBD(this DBSource val)
        {
            switch (val)
            {
                case DBSource.TEMP100: return "_TEMP100";
                case DBSource.TEMP1: return "_TEMP1";
                case DBSource.MAIN: return "";
                default:
                    throw new ArgumentOutOfRangeException(nameof(val), val, null);
            }
        }
        public static bool In(this TypeFileCreate val, params TypeFileCreate[] values)
        {
            return values.Contains(val);
        }
    }

    public interface IExportFileRepository
    {
        List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_ZGLV(DBSource source, TypeFileCreate tfc, PERIOD_PARAM periodParam, SLUCH_PARAM sluchParam );
        DataTable V_EXPORT_H_ZGLV(int zglvid, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_SCHET(int zglvid, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_ZAP(int[] zglvid, string SMO, long[] SLUCH_Z_ID, bool OnlyValidFFOMS,bool OnlyDOP, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_SLUCH(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_SANK(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_NAZR(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_DS2_N(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_SL_KOEF(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_USL(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_ONK_USL(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_LEK_PR(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_NAPR(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_B_PROT(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_B_DIAG(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_DS2(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_DS3(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_CRIT(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_CONS(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_L_ZGLV(string FILENAME, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_L_PERS(IEnumerable<long> pers_id, bool IsNewComment, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_EXPERTIZE(IEnumerable<long> sl, DBSource source, OracleConnection conn = null);
        DataTable V_EXPORT_H_MR_USL(IEnumerable<long> us, DBSource source, OracleConnection conn = null);

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

        public List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_ZGLV(DBSource source, TypeFileCreate tfc, PERIOD_PARAM periodParam, SLUCH_PARAM sluchParam)
        {
            switch (tfc)
            {
                case TypeFileCreate.SMO:
                case TypeFileCreate.MO:
                    if (periodParam == null)
                        throw new Exception("Не указаны параметры периода");
                    return V_EXPORT_H_ZGLV(source, periodParam);
                case TypeFileCreate.SLUCH:
                    if (sluchParam == null)
                        throw new Exception("Не указаны параметры поиска по случаям");
                    return V_EXPORT_H_ZGLV(source, sluchParam);
                case TypeFileCreate.FFOMSDx:
                    if (periodParam == null)
                        throw new Exception("Не указаны параметры периода");
                    return V_EXPORT_H_ZGLVOnlyDx(source, periodParam);
                case TypeFileCreate.MEK_P_P_SMO:
                    if (periodParam == null)
                        throw new Exception("Не указаны параметры периода");
                    return V_EXPORT_H_DOP_SMO(periodParam);
                case TypeFileCreate.MEK_P_P_MO:
                    if (periodParam == null)
                        throw new Exception("Не указаны параметры периода");
                    return V_EXPORT_H_DOP_MO(periodParam);

                default:
                    throw new ArgumentOutOfRangeException(nameof(tfc), tfc, null);
            }
        }

        private List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_ZGLV(DBSource source, PERIOD_PARAM periodParam)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_ZGLV{source.ToPrefixBD()} t where t.month = {periodParam.Month}  and  t.year = {periodParam.Year}", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Select().Select(V_EXPORT_H_ZGLVRow.Get).ToList();
                }
            }
        }
        private List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_ZGLVOnlyDx(DBSource source, PERIOD_PARAM periodParam)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_ZGLV{source.ToPrefixBD()} t where t.month = {periodParam.Month}  and  t.year = {periodParam.Year} and t.filename like 'D%'", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Select().Select(V_EXPORT_H_ZGLVRow.Get).ToList();
                }
            }
        }
        private List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_ZGLV(DBSource source, SLUCH_PARAM sluchParam)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select distinct hz.* from V_EXPORT_H_ZGLV{source.ToPrefixBD()} hz " +
                                                       $"inner join  V_EXPORT_H_ZAP{source.ToPrefixBD()} z on z.schet_id = hz.schet_id " +
                                                       $"where z.SLUCH_Z_ID in ({string.Join(",",sluchParam.SLUCH_Z_ID)})", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Select().Select(V_EXPORT_H_ZGLVRow.Get).ToList();
                }
            }
        }

        private List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_DOP_SMO(PERIOD_PARAM periodParam)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select  hz.* from V_EXPORT_H_ZGLV_DOP hz where DOP = 1 and month_sank = {periodParam.Month}  and  year_sank = {periodParam.Year} ", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Select().Select(V_EXPORT_H_ZGLVRow.Get).ToList();
                }
            }
        }

        private List<V_EXPORT_H_ZGLVRow> V_EXPORT_H_DOP_MO(PERIOD_PARAM periodParam)
        {
            using (var con = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select zglv_id, VERSION, DATA, FILENAME, SD_Z, COMMENT_ZGLV,MONTH_SANK MONTH,YEAR_SANK YEAR, CODE_MO, SCHET_ID, YEAR_BASE, MONTH_BASE, DOP, YEAR_SANK, MONTH_SANK, null SMO, wm_concat(S_ZGLV_ID) S_ZGLV_ID from V_EXPORT_H_ZGLV_DOP hz where DOP = 1 and month_sank = {periodParam.Month}  and  year_sank = {periodParam.Year} " +
                                                       $"group by zglv_id, VERSION, DATA, FILENAME, SD_Z, COMMENT_ZGLV, MONTH, YEAR, CODE_MO, SCHET_ID, YEAR_BASE, MONTH_BASE, DOP, YEAR_SANK, MONTH_SANK", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Select().Select(V_EXPORT_H_ZGLVRow.Get).ToList();
                }
            }
        }

        public DataTable V_EXPORT_H_ZGLV(int zglvid, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_H_ZGLV{source.ToPrefixBD()} t where zglv_id = {zglvid}", conn))
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


   

        public DataTable V_EXPORT_H_SCHET(int zglvid, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_H_SCHET{source.ToPrefixBD()} t where zglv_id = {zglvid}", conn))
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
        public DataTable V_EXPORT_H_ZAP(int[] zglvid, string SMO, long[] SLUCH_Z_ID, bool OnlyValidFFOMS, bool OnlyDOP, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            return OnlyDOP ? V_EXPORT_H_ZAP_ONLY_DOP(zglvid, SMO, conn) : V_EXPORT_H_ZAP_MAIN(zglvid, SMO, SLUCH_Z_ID, OnlyValidFFOMS, source, conn);
        }

        private DataTable V_EXPORT_H_ZAP_MAIN(int[] zglvid, string SMO, long[] SLUCH_Z_ID,bool OnlyValidFFOMS,DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_H_ZAP{source.ToPrefixBD()} t where zglv_id in ({string.Join(",", zglvid)}) and {(!string.IsNullOrEmpty(SMO) ? $"SMO = '{SMO}'" : "IsZK = 1")}  {(SLUCH_Z_ID?.Length>0 ? $"and SLUCH_Z_ID in ({string.Join(",",SLUCH_Z_ID)})" : "")} {(OnlyValidFFOMS? "and ValidFFOMS = 1" : "")}", conn))
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

        public DataTable V_EXPORT_H_ZAP_ONLY_DOP(int[] S_ZGLV_ID, string SMO, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select distinct z.* from xml_h_sank_zglv_v3 sz
inner join xml_h_sank_v3 san on san.s_zglv_id = sz.zglv_id
inner join V_EXPORT_H_ZAP z on z.sluch_z_id = san.sluch_z_id
where sz.zglv_id in ({string.Join(",", S_ZGLV_ID)}) and {(!string.IsNullOrEmpty(SMO) ? $"z.SMO = '{SMO}'" : "z.IsZK = 1")}", conn))
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

        public DataTable V_EXPORT_H_SLUCH(IEnumerable<long> sl, DBSource source,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_SLUCH{source.ToPrefixBD()} t where SLUCH_Z_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_SANK(IEnumerable<long> sl, DBSource source,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_SANK{source.ToPrefixBD()}  t where SLUCH_Z_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_EXPERTIZE(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_EXPERTIZE{source.ToPrefixBD()}  t where SLUCH_Z_ID in ({string.Join(",", sl)})", conn))
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

        public DataTable V_EXPORT_H_MR_USL(IEnumerable<long> us, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_MR_USL{source.ToPrefixBD()}  t where USL_ID in ({string.Join(",", us)})", conn))
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

        public DataTable V_EXPORT_H_NAZR(IEnumerable<long> sl, DBSource source,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_NAZR{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_DS2_N(IEnumerable<long> sl, DBSource source,  OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS2_N{source.ToPrefixBD()} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_SL_KOEF(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_SL_KOEF{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_USL(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_USL{source.ToPrefixBD()} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_ONK_USL(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_ONK_USL{source.ToPrefixBD()} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_LEK_PR(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_LEK_PR{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_NAPR(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_NAPR{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_B_PROT(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_B_PROT{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_B_DIAG(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_B_DIAG{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_DS2(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS2{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_DS3(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_DS3{source.ToPrefixBD()} t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_H_CRIT(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_H_CRIT{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_CONS(IEnumerable<long> sl, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($"select * from V_EXPORT_CONS{source.ToPrefixBD()}  t where SLUCH_ID in ({string.Join(",", sl)})", conn))
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
        public DataTable V_EXPORT_L_ZGLV(string FILENAME, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select * from V_EXPORT_L_ZGLV{source.ToPrefixBD()} t where t.FILENAME1 = '{FILENAME}'", conn))
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
        public DataTable V_EXPORT_L_PERS(IEnumerable<long> pers_id, bool IsNewComment, DBSource source, OracleConnection conn = null)
        {
            var con = conn ?? new OracleConnection(ConnectionString);
            try
            {
                using (var oda = new OracleDataAdapter($@"select distinct h_zglv_id, filename1, pers_id, zglv_id, id_pac, fam, im, ot, w, dr, fam_p, im_p, ot_p, w_p, dr_p, mr, doctype, docser, docnum, snils, okatog, okatop, dost, dost_p, fam_tfoms, im_tfoms, ot_tfoms, dr_tfoms, rokato, renp, rqogrn, rdbeg, tel, {(IsNewComment ? "comentp_new comentp" : "comentp")}, docdate, docorg, iszk from V_EXPORT_L_PERS{source.ToPrefixBD()} t where    PERS_ID in ({string.Join(",", pers_id)})", conn))
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
        public int[] S_ZGLV_ID { get; set; }
        public string CODE_MO { get; set; }
        public string FILENAME { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public int YEAR_BASE { get; set; }
        public int MONTH_BASE { get; set; }
        public bool DOP { get; set; }
        public string SMO { get; set; } = null;

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
                    MONTH = Convert.ToInt32(row["MONTH"]),
                    YEAR_BASE = Convert.ToInt32(row["YEAR_BASE"]),
                    MONTH_BASE = Convert.ToInt32(row["MONTH_BASE"]),
                    DOP = Convert.ToBoolean(row[nameof(DOP)])
                   
                };
                item.S_ZGLV_ID = row[nameof(S_ZGLV_ID)].ToString().Split(',').Where(x=>!string.IsNullOrEmpty(x)).Select(x=>Convert.ToInt32(x)).ToArray();
                if (row[nameof(SMO)] != DBNull.Value)
                    item.SMO = Convert.ToString(row[nameof(SMO)]);
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

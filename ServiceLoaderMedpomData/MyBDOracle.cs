using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                throw new Exception($"Ошибка получения V_XML_CHECK_FILENAME: {ex.Message}",ex);
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
                if(row["SLUCH_ID"]!=DBNull.Value)
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
        public string  FAM { get; set; }
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
        public decimal KOL_USL  { get; set; }
    }

    public interface IRepository: IDisposable
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
    }

    public class MYBDOracleNEW : IRepository
    {

        OracleConnection con;
        string ConnStr = "";
        //Имя таблиц
        TableInfo H_ZGLV;
        TableInfo H_SCHET;
        TableInfo H_SANK;
        TableInfo H_PAC;
        TableInfo H_ZAP;
        TableInfo H_USL;
        TableInfo H_SLUCH;
        TableInfo L_ZGLV;
        TableInfo L_PERS;
        TableInfo H_DS2_N;
        TableInfo H_NAZR;
        TableInfo H_Z_SLUCH;
        TableInfo H_KSLP;
        TableInfo xml_errors;
        TableInfo H_B_DIAG;
        TableInfo H_B_PROT;
        TableInfo H_NAPR;
        TableInfo H_ONK_USL;
        TableInfo H_LEK_PR;
        TableInfo H_LEK_DATE_INJ;
        TableInfo H_CONS;
        TableInfo H_CODE_EXP;
        TableInfo H_CRIT;
        TableInfo H_DS2;
        TableInfo H_DS3;
        int curr_month;
        int curr_year;


        public MYBDOracleNEW(string _con, TableInfo _H_ZGLV, TableInfo _H_SCHET, TableInfo _H_SANK, TableInfo _H_CODE_EXP, TableInfo _H_PAC, TableInfo _H_ZAP,
        TableInfo _H_USL, TableInfo _H_SLUCH, TableInfo _H_DS2, TableInfo _H_DS3, TableInfo _H_CRIT, TableInfo _H_Z_SLUCH, TableInfo _H_KSLP, TableInfo _L_ZGLV,
        TableInfo _L_PERS,
        TableInfo _H_DS2_N,
        TableInfo _H_NAZR, TableInfo _H_B_DIAG, TableInfo _H_B_PROT, TableInfo _H_NAPR,
        TableInfo _H_ONK_USL, TableInfo _H_LEK_PR, TableInfo _H_LEK_DATE_INJ, TableInfo _H_CONS,
        TableInfo _xml_errors,
        DateTime curr_month)
        {
            this.curr_month = curr_month.Month;
            this.curr_year = curr_month.Year;
            con = new OracleConnection(_con);
            ConnStr = _con;
            /////
            //Читаем имена таблиц в бд
            H_ZGLV = _H_ZGLV;
            H_SCHET = _H_SCHET;
            H_SANK = _H_SANK;
            H_PAC = _H_PAC;
            H_ZAP = _H_ZAP;
            H_USL = _H_USL;
            H_SLUCH = _H_SLUCH;
            L_ZGLV = _L_ZGLV;
            L_PERS = _L_PERS;
            H_ZGLV = _H_ZGLV;
            H_ZGLV = _H_ZGLV;
            H_DS2_N = _H_DS2_N;
            H_NAZR = _H_NAZR;
            H_Z_SLUCH = _H_Z_SLUCH;
            H_KSLP = _H_KSLP;
            xml_errors = _xml_errors;
            H_B_DIAG = _H_B_DIAG;
            H_B_PROT = _H_B_PROT;
            H_NAPR = _H_NAPR;
            H_ONK_USL = _H_ONK_USL;
            H_LEK_PR = _H_LEK_PR;
            H_LEK_DATE_INJ = _H_LEK_DATE_INJ;
            H_CONS = _H_CONS;
            H_CODE_EXP = _H_CODE_EXP;
            H_DS2 = _H_DS2;
            H_DS3 = _H_DS3;
            H_CRIT = _H_CRIT;
        }

        private  List<OracleConnection> Cons = new List<OracleConnection>();
        private  List<OracleCommand> CMDs = new List<OracleCommand>();
        private bool IsDispose = false;
        private OracleCommand NewOracleCommand(string cmdText, OracleConnection con)
        {
            if(IsDispose)
                throw  new Exception("Невозможно создать команду - получен сигнал уничтожения объекта");
            var t = new OracleCommand(cmdText, con);
            CMDs.Add(t);
            return t;
        }

        private void  RemoveOracleCommand(OracleCommand cmd)
        {
            if (IsDispose)
                return;
            var t =  CMDs.FirstOrDefault(x => x == cmd);
           if (t != null)
           {
               CMDs.Remove(t);
           }
        }

        private OracleConnection NewOracleConnection(string connectionString)
        {
            if (IsDispose)
                throw new Exception("Невозможно создать подключение - получен сигнал уничтожения объекта");
            var t = new OracleConnection(connectionString);
            Cons.Add(t);
            return t;
        }


        private void RemoveOracleConnection(OracleConnection conn)
        {
            if (IsDispose)
                return;
            var t = Cons.FirstOrDefault(x => x == conn);
            if (t != null)
            {
                Cons.Remove(t);
            }
        }

        public int AddSankZGLV(string FILENAME, int CODE, int CODE_MO, int FLAG_MEE, int YEAR, int MONTH, int YEAR_SANK, int MONTH_SANK, int ZGLV_ID_BASE, string SMO, bool DOP_FLAG, bool isNotFinish)
        {
            var cmd = NewOracleCommand($@"insert into xml_h_sank_zglv_v3
  (filename, code, code_mo, flag_mee, year, month, month_sank, year_sank,zglv_id_base,smo,DOP_FLAG,isNotFinish)
values
  (:filename, :code, :code_mo, :flag_mee, :year, :month, :month_sank, :year_sank,:zglv_id_base,:smo,:DOP_FLAG,:isNotFinish)
returning  zglv_id into :id", con);

            cmd.Parameters.Add("filename", FILENAME);
            cmd.Parameters.Add("code", CODE);
            cmd.Parameters.Add("code_mo", CODE_MO);
            cmd.Parameters.Add("flag_mee", FLAG_MEE);
            cmd.Parameters.Add("year", YEAR);
            cmd.Parameters.Add("month", MONTH);
            cmd.Parameters.Add("month_sank", MONTH_SANK);
            cmd.Parameters.Add("year_sank", YEAR_SANK);
            cmd.Parameters.Add("zglv_id_base", ZGLV_ID_BASE);
            cmd.Parameters.Add("smo", SMO);
            cmd.Parameters.Add("DOP_FLAG", DOP_FLAG? 1:0);
            cmd.Parameters.Add("isNotFinish", isNotFinish ? 1 : 0); 
            cmd.Parameters.Add("id", OracleDbType.Decimal, ParameterDirection.ReturnValue);

            if (Transaction)
                cmd.Transaction = Tran;

            if (!Transaction)
                cmd.Connection.Open();
            cmd.ExecuteNonQuery();

            if (!Transaction)
                cmd.Connection.Close();

            RemoveOracleCommand(cmd);
            var x = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)cmd.Parameters["id"].Value).Value);
            return x;

        }

        public void UpdateSankZGLV(int ZGLV_ID, int ZGLV_ID_BASE)
        {
            var cmd = NewOracleCommand(@"update xml_h_sank_zglv_v3 t set zglv_id_base = :ZGLV_ID_BASE
where t.zglv_id = :ZGLV_ID", con);
            cmd.Parameters.Add("ZGLV_ID_BASE", ZGLV_ID_BASE);
            cmd.Parameters.Add("ZGLV_ID", ZGLV_ID);
            if (Transaction)
                cmd.Transaction = Tran;
            else
                cmd.Connection.Open();
            cmd.ExecuteNonQuery();
            if (!Transaction)
                cmd.Connection.Close();
            RemoveOracleCommand(cmd);
        }
        public void InsertFile(ZL_LIST zl, PERS_LIST pe)
        {
            Dictionary<string, decimal?> per;
            try
            {
                per = Insert(pe);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка переноса файла перс. данных: {ex.Message}", ex);
            }

            foreach (var p in zl.ZAP.Select(x => x.PACIENT))
            {
                if (!per.ContainsKey(p.ID_PAC))
                {
                    throw new Exception("Нет данных о PERS_ID для ID_PAC =" + p.ID_PAC);
                }
                p.PERS_ID = per[p.ID_PAC].Value;
            }

            try
            {
                Insert(zl);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка переноса основного файла: {ex.Message}", ex);
            }

        }


        /// <summary>
        /// Вставить файл в базу(сначала транзакцию открыть)
        /// </summary>
        /// <param name="zl"></param>
        void Insert(ZL_LIST zl)
        {
            var ZS = zl.ZAP.SelectMany(x => x.Z_SL_list);
            var pacient = zl.ZAP.Select(x => x.PACIENT);
            var sluch = ZS.SelectMany(x => x.SL);
            var usl = sluch.SelectMany(x => x.USL);
            var onk = sluch.Select(x => x.ONK_SL);
            var sank = ZS.SelectMany(x => x.SANK);
            var onk_usl = sluch.Where(x => x.ONK_SL != null).SelectMany(x => x.ONK_SL.ONK_USL);
            var lek_pr = onk_usl.SelectMany(x => x.LEK_PR);
            zl.SetID(GetSec(H_ZGLV.SeqName, 1), GetSec(H_SCHET.SeqName, 1), GetSec(H_ZAP.SeqName, zl.ZAP.Count), GetSec(H_PAC.SeqName, pacient.Count()),
                GetSec(H_Z_SLUCH.SeqName, ZS.Count()), GetSec(H_SLUCH.SeqName, sluch.Count()),
                GetSec(H_USL.SeqName, usl.Count()), GetSec(H_SANK.SeqName, sank.Count()), GetSec(H_ONK_USL.SeqName, onk_usl.Count()), GetSec(H_LEK_PR.SeqName, lek_pr.Count()));
          
            if (zl.ZGLV.Vers < 3.1m)
            {
                var er = IDENTY_SANK_SLUCH_ID(ZS);
                if (er.Count != 0)
                {
                    throw new Exception($"Ошибка при поиске санкций: {string.Join(",", er)}");
                }
            }

            InsertZGLV(zl.ZGLV);
            InsertSCHET(zl.SCHET);
            InsertZAP(zl.ZAP);
            InsertPACINET(pacient);
            InsertZ_SLUCH(ZS);
            InsertSLUCH(sluch);
            InsertUSL(usl);
            InsertNAZR(sluch.SelectMany(x => x.NAZ));
            InsertNAPR(sluch.SelectMany(x => x.NAPR));
            InsertONK_USL(onk_usl);
            InsertLEK_PR(lek_pr);
            InsertDATE_INJ(lek_pr.SelectMany(x => x.DATE_INJ));
            InsertCONS(sluch.SelectMany(x => x.CONS));
            InsertB_PROT(sluch.Where(x => x.ONK_SL != null).SelectMany(y => y.ONK_SL.B_PROT));
            InsertB_DIAG(sluch.Where(x => x.ONK_SL != null).SelectMany(y => y.ONK_SL.B_DIAG));
            InsertSL_KOEF(sluch.Where(x => x.KSG_KPG != null).SelectMany(y => y.KSG_KPG.SL_KOEF));
            InsertSANK(sank);
            InsertH_DS2_N(sluch.SelectMany(x => x.DS2_N));
            InsertH_DS2(sluch.SelectMany(x => x.GetDS2()));
            InsertH_DS3(sluch.SelectMany(x => x.GetDS3()));
            InsertCRIT(sluch.Where(x => x.KSG_KPG != null).SelectMany(y => y.KSG_KPG.GetCRIT_SLUCH_ID(y.SLUCH_ID)));
            InsertCODE_EXP(sank.SelectMany(y => y.CODE_EXP));
        }
        Dictionary<string, decimal?> Insert(PERS_LIST pe)
        {
            pe.SetID(GetSec(L_ZGLV.SeqName, 1), GetSec(L_PERS.SeqName, pe.PERS.Count));
            InsertL_ZGLV(pe.ZGLV);
            InsertPERS(pe.PERS);
            return pe.PERS.ToDictionary(x => x.ID_PAC, x => x.PERS_ID);
        }
        #region ВСТАВКА ТАБЛИЦ
        void InsertL_ZGLV(PERSZGLV item)
        {
            try
            {
                var cmd = NewOracleCommand(
                    $@"insert into {L_ZGLV.FullTableName} (DATA, FILENAME, FILENAME1, VERSION, ZGLV_ID)
                                                                                  values
  (:DATA, :FILENAME, :FILENAME1, :VERSION, :ZGLV_ID)", con);
                if (Transaction)
                    cmd.Transaction = Tran;

                cmd.Parameters.Add("DATA", item.DATA);
                cmd.Parameters.Add("FILENAME", item.FILENAME.ToUpper());
                cmd.Parameters.Add("FILENAME1", item.FILENAME1.ToUpper());
                cmd.Parameters.Add("ZGLV_ID", item.VERSION);
                cmd.Parameters.Add("VERSION", item.ZGLV_ID);
                var t = cmd.ExecuteNonQuery();
                if (t != 1) throw new Exception("Не полная вставка L_ZGLV");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertZGLV: {ex.Message}", ex);
            }
        }
        void InsertPERS(IEnumerable<PERS> Items)
        {
            try
            {
                if (!Items.Any()) return;
                var cmd = NewOracleCommand(
                    $@"insert into {L_PERS.FullTableName} (COMENTP, DOCNUM, DOCSER, DOCTYPE,DOST,DOST_P,DR,DR_P,FAM,FAM_P,ID_PAC,IM,IM_P,MR,OKATOG,OKATOP,OT,OT_P,SNILS,TEL,W,W_P,PERS_ID,ZGLV_ID,DOCDATE,DOCORG)
                                                                                  values
                                                                 (:COMENTP, :DOCNUM, :DOCSER, :DOCTYPE,:DOST,:DOST_P,:DR,:DR_P,:FAM,:FAM_P,:ID_PAC,:IM,:IM_P,:MR,:OKATOG,:OKATOP,:OT,:OT_P,:SNILS,:TEL,:W,:W_P,:PERS_ID,:ZGLV_ID,:DOCDATE,:DOCORG)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.BindByName = true;
                cmd.ArrayBindCount = Items.Count();
                cmd.Parameters.Add("COMENTP", Items.Select(x => x.COMENTP).ToArray());
                cmd.Parameters.Add("DOCNUM", Items.Select(x => x.DOCNUM == null ? x.DOCNUM : x.DOCNUM.ToUpper()).ToArray());
                cmd.Parameters.Add("DOCSER", Items.Select(x => x.DOCSER == null ? x.DOCSER : x.DOCSER.ToUpper()).ToArray());
                cmd.Parameters.Add("DOCTYPE", Items.Select(x => x.DOCTYPE).ToArray());
                cmd.Parameters.Add("DOST", Items.Select(x => x.DOSTstr()).ToArray());
                cmd.Parameters.Add("DOST_P", Items.Select(x => x.DOST_Pstr()).ToArray());
                cmd.Parameters.Add("DR", Items.Select(x => x.DR).ToArray());
                cmd.Parameters.Add("DR_P", OracleDbType.Date, Items.Select(x => x.DR_P ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("FAM", Items.Select(x => x.FAM == null ? x.FAM : x.FAM.ToUpper()).ToArray());
                cmd.Parameters.Add("FAM_P", Items.Select(x =>  x.FAM_P == null ? x.FAM_P : x.FAM_P.ToUpper()).ToArray());
                cmd.Parameters.Add("ID_PAC", Items.Select(x => x.ID_PAC).ToArray());
                cmd.Parameters.Add("IM", Items.Select(x => x.IM == null ? x.IM : x.IM.ToUpper()).ToArray());
                cmd.Parameters.Add("IM_P", Items.Select(x => x.IM_P == null ? x.IM_P : x.IM_P.ToUpper()).ToArray());
                cmd.Parameters.Add("MR", Items.Select(x => x.MR).ToArray());
                cmd.Parameters.Add("OKATOG", Items.Select(x => x.OKATOG).ToArray());
                cmd.Parameters.Add("OKATOP", Items.Select(x => x.OKATOP).ToArray());
                cmd.Parameters.Add("OT", Items.Select(x => x.OT == null ? x.OT : x.OT.ToUpper()).ToArray());
                cmd.Parameters.Add("OT_P", Items.Select(x => x.OT_P == null ? x.OT_P : x.OT_P.ToUpper()).ToArray());
                cmd.Parameters.Add("SNILS", Items.Select(x => x.SNILS).ToArray());
                cmd.Parameters.Add("TEL", Items.Select(x => x.TEL).ToArray());
                cmd.Parameters.Add("W", Items.Select(x => x.W).ToArray());
                cmd.Parameters.Add("W_P", OracleDbType.Decimal, Items.Select(x => x.W_P ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PERS_ID", OracleDbType.Decimal, Items.Select(x => x.PERS_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ZGLV_ID", OracleDbType.Decimal, Items.Select(x => x.ZGLV_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("DOCDATE", OracleDbType.Date, Items.Select(x => x.DOCDATE ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DOCORG", Items.Select(x => x.DOCORG == null ? x.DOCORG : x.DOCORG.ToUpper()).ToArray());

                var t = cmd.ExecuteNonQuery();
                RemoveOracleCommand(cmd);
                if (t != Items.Count()) throw new Exception($"Не полная вставка PERS вставлено {t} из {Items.Count()}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertPERS: {ex.Message}", ex);
            }
        }
        void InsertZGLV(ZGLV item)
        {
            try
            {
                var cmd = NewOracleCommand(
                    $@"insert into {H_ZGLV.FullTableName} (zglv_id, version, data, filename, sd_z)
                                                                                  values
  (:zglv_id, :version, :data, :filename, :sd_z)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.Parameters.Add("zglv_id", item.ZGLV_ID);
                cmd.Parameters.Add("version", item.VERSION);
                cmd.Parameters.Add("data", item.DATA);
                cmd.Parameters.Add("filename", item.FILENAME.ToUpper());
                cmd.Parameters.Add("sd_z", item.SD_Z);
                var t = cmd.ExecuteNonQuery();
                RemoveOracleCommand(cmd);
                if (t != 1) throw new Exception("Не полная вставка ZGLV");
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertZGLV: {ex.Message}", ex);
            }

        }
        void InsertSCHET(SCHET item)
        {
            try
            {
                var cmd = NewOracleCommand(
                    $@"insert into {H_SCHET.FullTableName} 
(CODE, CODE_MO, COMENTS, DISP, DSCHET,MONTH,NSCHET,PLAT,SANK_EKMP,SANK_MEE,SANK_MEK,SCHET_ID,SUMMAP,SUMMAV,YEAR,ZGLV_ID,DOP_FLAG,YEAR_BASE, MONTH_BASE)
                              values
(:CODE, :CODE_MO, :COMENTS, :DISP, :DSCHET,:MONTH,:NSCHET,:PLAT,:SANK_EKMP,:SANK_MEE,:SANK_MEK,:SCHET_ID,:SUMMAP,:SUMMAV,:YEAR,:ZGLV_ID,:DOP_FLAG,:YEAR_BASE,:MONTH_BASE)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.Parameters.Add("CODE", item.CODE);
                cmd.Parameters.Add("CODE_MO", item.CODE_MO);
                cmd.Parameters.Add("COMENTS", item.COMENTS);
                cmd.Parameters.Add("DISP", item.DISP);
                cmd.Parameters.Add("DSCHET", item.DSCHET);
                cmd.Parameters.Add("MONTH", item.MONTH);
                cmd.Parameters.Add("NSCHET", item.NSCHET);
                cmd.Parameters.Add("PLAT", item.PLAT);
                cmd.Parameters.Add("SANK_EKMP", item.SANK_EKMP);
                cmd.Parameters.Add("SANK_MEE", item.SANK_MEE);
                cmd.Parameters.Add("SANK_MEK", item.SANK_MEK);
                cmd.Parameters.Add("SCHET_ID", item.SCHET_ID);
                cmd.Parameters.Add("SUMMAP", item.SUMMAP);
                cmd.Parameters.Add("SUMMAV", item.SUMMAV);
                cmd.Parameters.Add("YEAR", item.YEAR);
                cmd.Parameters.Add("ZGLV_ID", item.ZGLV_ID);
                cmd.Parameters.Add("DOP_FLAG", item.DOP_FLAG);

               cmd.Parameters.Add("YEAR_BASE",  item.YEAR_BASE ?? item.YEAR);
               cmd.Parameters.Add("MONTH_BASE", item.MONTH_BASE ?? item.MONTH);


                var t = cmd.ExecuteNonQuery();
                if (t != 1) throw new Exception("Не полная вставка SCHET");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertSCHET: {ex.Message}", ex);
            }
        }
        void InsertZAP(IEnumerable<ZAP> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_ZAP.FullTableName} (N_ZAP, PR_NOV, SCHET_ID, ZAP_ID)
                                                                                  values
                                                                                  (:N_ZAP, :PR_NOV, :SCHET_ID, :ZAP_ID)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.BindByName = true;
                cmd.ArrayBindCount = Items.Count();
                cmd.Parameters.Add("N_ZAP", Items.Select(x => x.N_ZAP).ToArray());
                cmd.Parameters.Add("PR_NOV", Items.Select(x => x.PR_NOV).ToArray());
                cmd.Parameters.Add("SCHET_ID", OracleDbType.Decimal, Items.Select(x => x.SCHET_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ZAP_ID", OracleDbType.Decimal, Items.Select(x => x.ZAP_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка ZAP вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertZAP: {ex.Message}", ex);
            }
        }
        void InsertPACINET(IEnumerable<PACIENT> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand(
                    $@"insert into {H_PAC.FullTableName} (ID_PAC, INV, MSE, NOVOR, NPOLIS, PACIENT_ID, SMO, SMO_NAM,  SMO_OGRN ,SMO_OK, SPOLIS, ST_OKATO,  VNOV_D, VPOLIS, ZAP_ID,SMO_TFOMS,PERS_ID)
                                                                                  values
                                                                                  (:ID_PAC, :INV, :MSE, :NOVOR, :NPOLIS, :PACIENT_ID, :SMO, :SMO_NAM,  :SMO_OGRN ,:SMO_OK, :SPOLIS, :ST_OKATO,  :VNOV_D, :VPOLIS, :ZAP_ID,:SMO_TFOMS,:PERS_ID)", con);

                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("ID_PAC", Items.Select(x => x.ID_PAC).ToArray());
                cmd.Parameters.Add("INV", OracleDbType.Decimal, Items.Select(x => x.INV ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("MSE", OracleDbType.Decimal, Items.Select(x => x.MSE ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NOVOR", Items.Select(x => x.NOVOR).ToArray());
                cmd.Parameters.Add("NPOLIS", Items.Select(x => x.NPOLIS == null ? x.NPOLIS : x.NPOLIS.ToUpper()).ToArray());
                cmd.Parameters.Add("PACIENT_ID", OracleDbType.Decimal, Items.Select(x => x.PACIENT_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SMO", Items.Select(x => x.SMO).ToArray());
                cmd.Parameters.Add("SMO_NAM", Items.Select(x => x.SMO_NAM).ToArray());
                cmd.Parameters.Add("SMO_OGRN", Items.Select(x => x.SMO_OGRN).ToArray());
                cmd.Parameters.Add("SMO_OK", Items.Select(x => x.SMO_OK).ToArray());
                cmd.Parameters.Add("SPOLIS", Items.Select(x => x.SPOLIS == null ? x.SPOLIS : x.SPOLIS.ToUpper()).ToArray());
                cmd.Parameters.Add("ST_OKATO", Items.Select(x => x.ST_OKATO).ToArray());
                cmd.Parameters.Add("VNOV_D", OracleDbType.Decimal, Items.Select(x => x.VNOV_D ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("VPOLIS", Items.Select(x => x.VPOLIS).ToArray());
                cmd.Parameters.Add("ZAP_ID", OracleDbType.Decimal, Items.Select(x => x.ZAP_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SMO_TFOMS", Items.Select(x => x.SMO_TFOMS).ToArray());
                cmd.Parameters.Add("PERS_ID", OracleDbType.Decimal, Items.Select(x => x.PERS_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка PACIENT вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertPACINET: {ex.Message}", ex);
            }
        }
        void InsertZ_SLUCH(IEnumerable<Z_SL> Items)
        {
            try
            {
                if (!Items.Any()) return;
                var cmd = NewOracleCommand($@"insert into {H_Z_SLUCH.FullTableName} 
(DATE_Z_1,DATE_Z_2,FOR_POM,IDCASE,IDSP,ISHOD,KD_Z,LPU,NPR_DATE,NPR_MO,OPLATA,OS_SLUCH,P_OTK,RSLT,RSLT_D,SANK_IT,SLUCH_Z_ID,SUMP,SUMV,USL_OK,VBR,VB_P,VIDPOM,VNOV_M, ZAP_ID, PACIENT_ID)
values
(:DATE_Z_1,:DATE_Z_2,:FOR_POM,:IDCASE,:IDSP,:ISHOD,:KD_Z,:LPU,:NPR_DATE,:NPR_MO,:OPLATA,:OS_SLUCH,:P_OTK,:RSLT,:RSLT_D,:SANK_IT,:SLUCH_Z_ID,:SUMP,:SUMV,:USL_OK,:VBR,:VB_P,:VIDPOM,:VNOV_M, :ZAP_ID,:PACIENT_ID)", con);

                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("DATE_Z_1", Items.Select(x => x.DATE_Z_1).ToArray());
                cmd.Parameters.Add("DATE_Z_2", Items.Select(x => x.DATE_Z_2).ToArray());
                cmd.Parameters.Add("FOR_POM", OracleDbType.Decimal, Items.Select(x => x.FOR_POM ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("IDCASE", Items.Select(x => x.IDCASE).ToArray());
                cmd.Parameters.Add("IDSP", Items.Select(x => x.IDSP).ToArray());
                cmd.Parameters.Add("ISHOD", OracleDbType.Decimal, Items.Select(x => x.ISHOD ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("KD_Z", OracleDbType.Decimal, Items.Select(x => x.KD_Z ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("LPU", Items.Select(x => x.LPU).ToArray());
                cmd.Parameters.Add("NPR_DATE", OracleDbType.Date, Items.Select(x => x.NPR_DATE ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NPR_MO", Items.Select(x => x.NPR_MO).ToArray());
                cmd.Parameters.Add("OPLATA", OracleDbType.Decimal, Items.Select(x => x.OPLATA ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("OS_SLUCH", Items.Select(x => x.OS_SLUCHstr()).ToArray());
                cmd.Parameters.Add("P_OTK", OracleDbType.Decimal, Items.Select(x => x.P_OTK ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("RSLT", OracleDbType.Decimal, Items.Select(x => x.RSLT ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("RSLT_D", OracleDbType.Decimal, Items.Select(x => x.RSLT_D ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SANK_IT", OracleDbType.Decimal, Items.Select(x => x.SANK_IT ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("SLUCH_Z_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_Z_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SUMP", OracleDbType.Decimal, Items.Select(x => x.SUMP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SUMV", Items.Select(x => x.SUMV).ToArray());
                cmd.Parameters.Add("USL_OK", OracleDbType.Decimal, Items.Select(x => x.USL_OK ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("VBR", OracleDbType.Decimal, Items.Select(x => x.VBR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("VB_P", OracleDbType.Decimal, Items.Select(x => x.VB_P ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("VIDPOM", Items.Select(x => x.VIDPOM).ToArray());

                cmd.Parameters.Add("VNOV_M", Items.Select(x => x.VNOV_Mstr()).ToArray());
                cmd.Parameters.Add("ZAP_ID", OracleDbType.Decimal, Items.Select(x => x.ZAP_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PACIENT_ID", OracleDbType.Decimal, Items.Select(x => x.PACIENT_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);



                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка Z_SLUCH вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertZ_SLUCH: {ex.Message}", ex);
            }
        }
        void InsertSLUCH(IEnumerable<SL> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_SLUCH.FullTableName} 
(CODE_MES1,CODE_MES2, COMENTSL, DATE_1, DATE_2, DET, DN, DS0, DS1, DS1_PR,C_ZAB, DS_ONK,ED_COL,
            IDDOKT, KD, BZTSZ, IT_SL, KOEF_D, KOEF_U, KOEF_UP, KOEF_Z, KSG_PG, N_KSG, SL_K, VER_KSG,LPU_1,METOD_HMP,NHISTORY, PODR, PROFIL, PROFIL_K, PRVS,
             PR_D_N, P_CEL, P_PER, REAB, SLUCH_ID, SLUCH_Z_ID, SL_ID, SUM_M, TAL_D, TAL_NUM,TAL_P,TARIF, VERS_SPEC, VID_HMP, PACIENT_ID, DS1_T, MTSTZ, ONK_M, ONK_N, ONK_T, SOD, STAD,
            K_FR, WEI, HEI, BSA,EXTR,SUM_MP)
values
(:CODE_MES1,:CODE_MES2,:COMENTSL,:DATE_1,:DATE_2,:DET,:DN,:DS0,:DS1,:DS1_PR,:C_ZAB, :DS_ONK,:ED_COL,
            :IDDOKT,:KD,:BZTSZ,:IT_SL,:KOEF_D,:KOEF_U,:KOEF_UP,:KOEF_Z,:KSG_PG,:N_KSG,:SL_K,:VER_KSG,:LPU_1,:METOD_HMP,:NHISTORY,:PODR,:PROFIL,:PROFIL_K,:PRVS,
:PR_D_N,:P_CEL,:P_PER,:REAB,:SLUCH_ID,:SLUCH_Z_ID,:SL_ID,:SUM_M,:TAL_D,:TAL_NUM,:TAL_P,:TARIF,:VERS_SPEC,:VID_HMP, :PACIENT_ID,:DS1_T, :MTSTZ, :ONK_M, :ONK_N, :ONK_T, :SOD, :STAD,
:K_FR, :WEI, :HEI, :BSA,:EXTR, :SUM_MP)", con);


                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("CODE_MES1", Items.Select(x => x.CODE_MES1str()).ToArray());
                cmd.Parameters.Add("CODE_MES2", Items.Select(x => x.CODE_MES2).ToArray());
                cmd.Parameters.Add("COMENTSL", Items.Select(x => x.COMENTSL).ToArray());
                cmd.Parameters.Add("DATE_1", Items.Select(x => x.DATE_1).ToArray());
                cmd.Parameters.Add("DATE_2", Items.Select(x => x.DATE_2).ToArray());
                cmd.Parameters.Add("DET", OracleDbType.Decimal, Items.Select(x => x.DET ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DN", OracleDbType.Decimal, Items.Select(x => x.DN ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("DS0", Items.Select(x => x.DS0).ToArray());
                cmd.Parameters.Add("DS1", Items.Select(x => x.DS1).ToArray());

                cmd.Parameters.Add("DS1_PR", OracleDbType.Decimal, Items.Select(x => x.DS1_PR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("C_ZAB", OracleDbType.Decimal, Items.Select(x => x.C_ZAB ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DS_ONK", OracleDbType.Decimal, Items.Select(x => x.DS_ONK ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ED_COL", OracleDbType.Decimal, Items.Select(x => x.ED_COL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);



                cmd.Parameters.Add("IDDOKT", Items.Select(x => x.IDDOKT).ToArray());
                cmd.Parameters.Add("KD", OracleDbType.Decimal, Items.Select(x => x.KD ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("BZTSZ", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG?.BZTSZ ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("IT_SL", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG != null ? x.KSG_KPG.IT_SL ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("KOEF_D", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG?.KOEF_D ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("KOEF_U", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG?.KOEF_U ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("KOEF_UP", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG?.KOEF_UP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("KOEF_Z", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG?.KOEF_Z ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("KSG_PG", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG?.KSG_PG ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("N_KSG", OracleDbType.Varchar2, Items.Select(x => x.KSG_KPG != null ? x.KSG_KPG.N_KSG : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SL_K", OracleDbType.Decimal, Items.Select(x => x.KSG_KPG?.SL_K ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("VER_KSG", OracleDbType.Varchar2, Items.Select(x => x.KSG_KPG?.VER_KSG ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("LPU_1", Items.Select(x => x.LPU_1).ToArray());
                cmd.Parameters.Add("METOD_HMP", OracleDbType.Decimal, Items.Select(x => x.METOD_HMP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("NHISTORY", Items.Select(x => x.NHISTORY).ToArray());
                cmd.Parameters.Add("PODR", OracleDbType.Decimal, Items.Select(x => x.PODR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PROFIL", OracleDbType.Decimal, Items.Select(x => x.PROFIL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PROFIL_K", OracleDbType.Decimal, Items.Select(x => x.PROFIL_K ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PRVS", OracleDbType.Decimal, Items.Select(x => x.PRVS ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);



                cmd.Parameters.Add("PR_D_N", OracleDbType.Decimal, Items.Select(x => x.PR_D_N ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("P_CEL", Items.Select(x => x.P_CEL).ToArray());
                cmd.Parameters.Add("P_PER", OracleDbType.Decimal, Items.Select(x => x.P_PER ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("REAB", OracleDbType.Decimal, Items.Select(x => x.REAB ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_Z_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_Z_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SL_ID", Items.Select(x => x.SL_ID).ToArray());
                cmd.Parameters.Add("SUM_M", Items.Select(x => x.SUM_M).ToArray());
                cmd.Parameters.Add("TAL_D", OracleDbType.Date, Items.Select(x => x.TAL_D ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("TAL_NUM", Items.Select(x => x.TAL_NUM).ToArray());
                cmd.Parameters.Add("TAL_P", OracleDbType.Date, Items.Select(x => x.TAL_P ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("TARIF", OracleDbType.Decimal, Items.Select(x => x.TARIF ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("VERS_SPEC", Items.Select(x => x.VERS_SPEC).ToArray());
                cmd.Parameters.Add("VID_HMP", Items.Select(x => x.VID_HMP).ToArray());
                cmd.Parameters.Add("PACIENT_ID", OracleDbType.Decimal, Items.Select(x => x.PACIENT_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);


                cmd.Parameters.Add("DS1_T", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.DS1_T ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("MTSTZ", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.MTSTZ ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("ONK_M", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.ONK_M ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ONK_N", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.ONK_N ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ONK_T", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.ONK_T ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SOD", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.SOD ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("STAD", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.STAD ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("K_FR", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.K_FR ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("WEI", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.WEI ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("HEI", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.HEI ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("BSA", OracleDbType.Decimal, Items.Select(x => x.ONK_SL != null ? x.ONK_SL.BSA ?? (object)DBNull.Value : (object)DBNull.Value).ToArray(), ParameterDirection.Input);


                cmd.Parameters.Add("EXTR", OracleDbType.Decimal, Items.Select(x => x.EXTR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SUM_MP", OracleDbType.Decimal, Items.Select(x => x.SUM_MP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);


                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка SLUCH вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertSLUCH: {ex.Message}", ex);
            }
        }
        void InsertUSL(IEnumerable<USL> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_USL.FullTableName} 
(CODE_MD,CODE_USL,COMENTU,DATE_IN,DATE_OUT, DET, DS, IDSERV, KOL_USL, LPU, LPU_1, NPL, USL_ID, PODR, PROFIL,PRVS,P_OTK, SLUCH_ID, TARIF, VID_VME, SUMP_USL, SUMV_USL)
values
(:CODE_MD,:CODE_USL,:COMENTU,:DATE_IN,:DATE_OUT, :DET, :DS, :IDSERV, :KOL_USL, :LPU, :LPU_1, :NPL,:USL_ID,:PODR, :PROFIL,:PRVS,:P_OTK, :SLUCH_ID, :TARIF,:VID_VME,:SUMP_USL,:SUMV_USL)", con);


                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("CODE_MD", Items.Select(x => x.CODE_MD).ToArray());
                cmd.Parameters.Add("CODE_USL", Items.Select(x => x.CODE_USL).ToArray());
                cmd.Parameters.Add("COMENTU", Items.Select(x => x.COMENTU).ToArray());
                cmd.Parameters.Add("DATE_IN", Items.Select(x => x.DATE_IN).ToArray());
                cmd.Parameters.Add("DATE_OUT", Items.Select(x => x.DATE_OUT).ToArray());
                cmd.Parameters.Add("DET", OracleDbType.Decimal, Items.Select(x => x.DET ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DS", Items.Select(x => x.DS).ToArray());
                cmd.Parameters.Add("IDSERV", Items.Select(x => x.IDSERV).ToArray());
                cmd.Parameters.Add("KOL_USL", OracleDbType.Decimal, Items.Select(x => x.KOL_USL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("LPU", Items.Select(x => x.LPU).ToArray());
                cmd.Parameters.Add("LPU_1", Items.Select(x => x.LPU_1).ToArray());
                cmd.Parameters.Add("NPL", OracleDbType.Decimal, Items.Select(x => x.NPL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("PODR", OracleDbType.Decimal, Items.Select(x => x.PODR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PROFIL", OracleDbType.Decimal, Items.Select(x => x.PROFIL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PRVS", OracleDbType.Decimal, Items.Select(x => x.PRVS ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("P_OTK", OracleDbType.Decimal, Items.Select(x => x.P_OTK ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SUMV_USL", Items.Select(x => x.SUMV_USL).ToArray());
                cmd.Parameters.Add("TARIF", OracleDbType.Decimal, Items.Select(x => x.TARIF ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("USL_ID", OracleDbType.Decimal, Items.Select(x => x.USL_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("VID_VME", Items.Select(x => x.VID_VME).ToArray());
                cmd.Parameters.Add("SUMP_USL", OracleDbType.Decimal, Items.Select(x => x.SUMP_USL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка USL вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertUSL: {ex.Message}", ex);
            }
        }
        void InsertNAZR(IEnumerable<NAZR> Items)
        {
            try
            {
                if (Items.Count() == 0) return;

                var cmd = NewOracleCommand($@"insert into {H_NAZR.FullTableName} 
(NAZ_N, NAZ_PK, NAZ_PMP, NAZ_R, NAZ_SP, NAZ_V, SLUCH_ID, NAZ_USL, NAPR_DATE, NAPR_MO)
values
(:NAZ_N, :NAZ_PK, :NAZ_PMP, :NAZ_R, :NAZ_SP, :NAZ_V, :SLUCH_ID, :NAZ_USL, :NAPR_DATE, :NAPR_MO)", con);


                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("NAZ_N", Items.Select(x => x.NAZ_N).ToArray());
                cmd.Parameters.Add("NAZ_PK", OracleDbType.Decimal, Items.Select(x => x.NAZ_PK ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NAZ_PMP", OracleDbType.Decimal, Items.Select(x => x.NAZ_PMP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NAZ_R", Items.Select(x => x.NAZ_R).ToArray());
                cmd.Parameters.Add("NAZ_SP", OracleDbType.Decimal, Items.Select(x => x.NAZ_SP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NAZ_V", OracleDbType.Decimal, Items.Select(x => x.NAZ_V ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                cmd.Parameters.Add("NAZ_USL", OracleDbType.Varchar2, Items.Select(x => x.NAZ_USL).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NAPR_DATE", OracleDbType.Date, Items.Select(x => x.NAPR_DATE ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NAPR_MO", OracleDbType.Varchar2, Items.Select(x => x.NAPR_MO).ToArray(), ParameterDirection.Input);



                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка NAZR вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertNAZR: {ex.Message}", ex);
            }
        }
        void InsertNAPR(IEnumerable<NAPR> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_NAPR.FullTableName} 
(MET_ISSL, NAPR_DATE, NAPR_USL, NAPR_V,NAPR_MO, SLUCH_ID, IDSERV,USL_ID)
values
(:MET_ISSL, :NAPR_DATE, :NAPR_USL, :NAPR_V,:NAPR_MO, :SLUCH_ID, :IDSERV,:USL_ID)", con);

                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("MET_ISSL", OracleDbType.Decimal, Items.Select(x => x.MET_ISSL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NAPR_DATE", Items.Select(x => x.NAPR_DATE).ToArray());
                cmd.Parameters.Add("NAPR_USL", Items.Select(x => x.NAPR_USL).ToArray());
                cmd.Parameters.Add("NAPR_V", Items.Select(x => x.NAPR_V).ToArray());
                cmd.Parameters.Add("NAPR_MO", Items.Select(x => x.NAPR_MO).ToArray());
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("IDSERV", Items.Select(x => x.IDSERV).ToArray());
                cmd.Parameters.Add("USL_ID", OracleDbType.Decimal, Items.Select(x => x.USL_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка NAPR вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertNAPR: {ex.Message}", ex);
            }
        }
        void InsertB_PROT(IEnumerable<B_PROT> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_B_PROT.FullTableName} 
(D_PROT,PROT,SLUCH_ID)
values
(:D_PROT,:PROT,:SLUCH_ID)", con);

                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("D_PROT", Items.Select(x => x.D_PROT).ToArray());
                cmd.Parameters.Add("PROT", Items.Select(x => x.PROT).ToArray());
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка B_PROT вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertB_PROT: {ex.Message}", ex);
            }
        }
        void InsertB_DIAG(IEnumerable<B_DIAG> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_B_DIAG.FullTableName} 
( DIAG_CODE, DIAG_RSLT, DIAG_TIP, DIAG_DATE, SLUCH_ID, REC_RSLT)
values
(:DIAG_CODE,:DIAG_RSLT,:DIAG_TIP,:DIAG_DATE,:SLUCH_ID,:REC_RSLT)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("DIAG_CODE", OracleDbType.Decimal, Items.Select(x => x.DIAG_CODE).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DIAG_RSLT", OracleDbType.Decimal, Items.Select(x => x.DIAG_RSLT ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DIAG_TIP", OracleDbType.Decimal, Items.Select(x => x.DIAG_TIP).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DIAG_DATE", OracleDbType.Date, Items.Select(x => x.DIAG_DATE).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("REC_RSLT", OracleDbType.Decimal, Items.Select(x => x.REC_RSLT ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка B_DIAG вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertB_DIAG: {ex.Message}", ex);
            }
        }
        void InsertCONS(IEnumerable<CONS> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_CONS.FullTableName} 
(PR_CONS, DT_CONS, SLUCH_ID, IDSERV)
values
(:PR_CONS, :DT_CONS, :SLUCH_ID, :IDSERV)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;

                cmd.Parameters.Add("PR_CONS", OracleDbType.Decimal, Items.Select(x => x.PR_CONS ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DT_CONS", OracleDbType.Date, Items.Select(x => x.DT_CONS ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("IDSERV", OracleDbType.Varchar2, Items.Select(x => x.IDSERV).ToArray(), ParameterDirection.Input);

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка CONS вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertCONS: {ex.Message}", ex);
            }
        }
        void InsertONK_USL(IEnumerable<ONK_USL> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_ONK_USL.FullTableName} 
(USL_TIP, HIR_TIP, LEK_TIP_L, LEK_TIP_V, LUCH_TIP, SLUCH_ID, ONK_USL_ID,IDSERV, PPTR)
values
(:USL_TIP, :HIR_TIP, :LEK_TIP_L, :LEK_TIP_V, :LUCH_TIP, :SLUCH_ID, :ONK_USL_ID,:IDSERV,:PPTR)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.Parameters.Add("USL_TIP", OracleDbType.Decimal, Items.Select(x => x.USL_TIP).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("HIR_TIP", OracleDbType.Decimal, Items.Select(x => x.HIR_TIP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("LEK_TIP_L", OracleDbType.Decimal, Items.Select(x => x.LEK_TIP_L ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("LEK_TIP_V", OracleDbType.Decimal, Items.Select(x => x.LEK_TIP_V ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("LUCH_TIP", OracleDbType.Decimal, Items.Select(x => x.LUCH_TIP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ONK_USL_ID", OracleDbType.Decimal, Items.Select(x => x.ONK_USL_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("IDSERV", OracleDbType.Varchar2, Items.Select(x => x.IDSERV).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PPTR", OracleDbType.Decimal, Items.Select(x => x.PPTR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка ONK_USL вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertONK_USL: {ex.Message}", ex);
            }
        }
        void InsertLEK_PR(IEnumerable<LEK_PR> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_LEK_PR.FullTableName} 
(LEK_PR_ID,ONK_USL_ID, REGNUM,CODE_SH)
values
(:LEK_PR_ID, :ONK_USL_ID, :REGNUM,:CODE_SH)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.Parameters.Add("LEK_PR_ID", OracleDbType.Decimal, Items.Select(x => x.LEK_PR_ID).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ONK_USL_ID", OracleDbType.Decimal, Items.Select(x => x.ONK_USL_ID).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("REGNUM", OracleDbType.Varchar2, Items.Select(x => x.REGNUM).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("CODE_SH", OracleDbType.Varchar2, Items.Select(x => x.CODE_SH).ToArray(), ParameterDirection.Input);

                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка LEK_PR вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertLEK_PR: {ex.Message}", ex);
            }
        }
        void InsertDATE_INJ(IEnumerable<DATE_INJ> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_LEK_DATE_INJ.FullTableName} 
(LEK_PR_ID, DATE_INJ)
values
(:LEK_PR_ID, :DATE_INJ)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.Parameters.Add("LEK_PR_ID", OracleDbType.Decimal, Items.Select(x => x.LEK_PR_ID).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("DATE_INJ", OracleDbType.Date, Items.Select(x => x.VALUE).ToArray(), ParameterDirection.Input);


                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка ONK_USL вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertDATE_INJ: {ex.Message}", ex);
            }
        }
        void InsertSL_KOEF(IEnumerable<SL_KOEF> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_KSLP.FullTableName} 
(IDSL,Z_SL,SLUCH_ID)
values
(:IDSL,:Z_SL,:SLUCH_ID)", con);
                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("IDSL", Items.Select(x => x.IDSL).ToArray());
                cmd.Parameters.Add("Z_SL", Items.Select(x => x.Z_SL).ToArray());
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка SL_KOEF вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertSL_KOEF: {ex.Message}", ex);
            }
        }
        void InsertSANK(IEnumerable<SANK> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_SANK.FullTableName} 
(SANK_ID,SLUCH_ID,SLUCH_Z_ID,S_CODE,S_COM,S_IST, S_OSN, S_SUM, S_TIP,S_YEAR,S_MONTH,S_TEM, S_PLAN, S_FINE, S_IDSERV,S_ZGLV_ID,DATE_ACT,NUM_ACT,SL_ID)
values
(:SANK_ID,:SLUCH_ID,:SLUCH_Z_ID,:S_CODE,:S_COM,:S_IST, :S_OSN, :S_SUM, :S_TIP, :S_YEAR,:S_MONTH,:S_TEM, :S_PLAN, :S_FINE,:S_IDSERV,:S_ZGLV_ID,:DATE_ACT,:NUM_ACT,:SL_ID)", con);

                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("SANK_ID", OracleDbType.Decimal, Items.Select(x => x.SANK_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_Z_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_Z_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("S_CODE", Items.Select(x => x.S_CODE).ToArray());
                cmd.Parameters.Add("S_COM", Items.Select(x => x.S_COM).ToArray());
                cmd.Parameters.Add("S_IST", Items.Select(x => x.S_IST).ToArray());
                cmd.Parameters.Add("S_OSN", Items.Select(x => x.S_OSN).ToArray());
                cmd.Parameters.Add("S_SUM", Items.Select(x => x.S_SUM).ToArray());
                cmd.Parameters.Add("S_TIP", Items.Select(x => x.S_TIP).ToArray());
                cmd.Parameters.Add("S_YEAR", OracleDbType.Decimal, Items.Select(x => x.S_YEAR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("S_MONTH", OracleDbType.Decimal, Items.Select(x => x.S_MONTH ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("S_TEM", OracleDbType.Decimal, Items.Select(x => x.S_TEM ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("S_PLAN", OracleDbType.Decimal, Items.Select(x => x.S_PLAN ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("S_FINE", OracleDbType.Decimal, Items.Select(x => x.S_FINE ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("S_IDSERV", Items.Select(x => x.S_IDSERV).ToArray());
                cmd.Parameters.Add("S_ZGLV_ID", OracleDbType.Decimal, Items.Select(x => x.S_ZGLV_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);


                cmd.Parameters.Add("DATE_ACT", OracleDbType.Date, Items.Select(x => x.DATE_ACT ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("NUM_ACT", OracleDbType.Varchar2, Items.Select(x => x.NUM_ACT).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SL_ID", OracleDbType.Varchar2, Items.Select(x => x.GetSL_ID).ToArray(), ParameterDirection.Input);



                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка SANK вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertSANK: {ex.Message}", ex);
            }
        }
        void InsertCODE_EXP(IEnumerable<CODE_EXP> Items)
        {
            try
            {
                if (Items.Count() == 0) return;
                var cmd = NewOracleCommand($@"insert into {H_CODE_EXP.FullTableName} 
(SANK_ID,CODE_EXP)
values
(:SANK_ID,:CODE_EXP)", con);

                if (Transaction)
                    cmd.Transaction = Tran;
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("SANK_ID", OracleDbType.Decimal, Items.Select(x => x.SANK_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("CODE_EXP", Items.Select(x => x.VALUE).ToArray());

                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception(
                    $"Не полная вставка CODE_EXP вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertCODE_EXP: {ex.Message}", ex);
            }
        }
        void InsertH_DS2_N(IEnumerable<DS2_N> Items)
        {
            try
            {
                if (!Items.Any()) return;
                var cmd = NewOracleCommand($@"insert into {H_DS2_N.FullTableName} 
(DS2,DS2_PR,PR_DS2_N,SLUCH_ID)
values
(:DS2,:DS2_PR,:PR_DS2_N,:SLUCH_ID)", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("DS2", Items.Select(x => x.DS2).ToArray());
                cmd.Parameters.Add("DS2_PR", OracleDbType.Decimal, Items.Select(x => x.DS2_PR ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("PR_DS2_N", OracleDbType.Decimal, Items.Select(x => x.PR_DS2_N ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                if (Transaction)
                    cmd.Transaction = Tran;
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка DS2_N вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertH_DS2_N: {ex.Message}", ex);
            }
        }

        void InsertH_DS2(IEnumerable<DS_SLUCH_ID> Items)
        {
            try
            {
                if (!Items.Any()) return;
                var cmd = NewOracleCommand($@"insert into {H_DS2.FullTableName} 
(DS2,SLUCH_ID)
values
(:DS2,:SLUCH_ID)", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("DS2", Items.Select(x => x.DS).ToArray());
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                if (Transaction)
                    cmd.Transaction = Tran;
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка DS2 вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertH_DS2: {ex.Message}", ex);
            }
        }
        void InsertH_DS3(IEnumerable<DS_SLUCH_ID> Items)
        {
            try
            {
                if (!Items.Any()) return;
                var cmd = NewOracleCommand($@"insert into {H_DS3.FullTableName} 
(DS3,SLUCH_ID)
values
(:DS3,:SLUCH_ID)", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("DS3", Items.Select(x => x.DS).ToArray());
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                if (Transaction)
                    cmd.Transaction = Tran;
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка DS3 вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertH_DS3: {ex.Message}", ex);
            }
        }
        void InsertCRIT(IEnumerable<CRIT_SLUCH_ID> Items)
        {
            try
            {
                if (!Items.Any()) return;
                var cmd = NewOracleCommand($@"insert into {H_CRIT.FullTableName} 
(CRIT,SLUCH_ID,ORD )
values
(:CRIT,:SLUCH_ID,:ORD)", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.BindByName = true;
                cmd.Parameters.Add("CRIT", Items.Select(x => x.CRIT).ToArray());
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("ORD", OracleDbType.Decimal, Items.Select(x => x.ORD ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                if (Transaction)
                    cmd.Transaction = Tran;
                var t = cmd.ExecuteNonQuery();
                if (t != Items.Count()) throw new Exception($"Не полная вставка CRIT вставлено {t} из {Items.Count()}");
                RemoveOracleCommand(cmd);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в InsertCRIT: {ex.Message}", ex);
            }
        }

        #endregion




        public Task<bool> DeleteTemp100TASK(string code_mo)
        {
            return Task.Factory.StartNew(() =>
            {
                using (var connect = NewOracleConnection(ConnStr))
                {
                    using (var cmd = NewOracleCommand(
              @"begin DELETE_REESTR_TEMP100(:code_mo); end;", connect))
                    {
                        try
                        {
                            cmd.Parameters.Add("code_mo", code_mo);
                            connect.Open();
                            cmd.ExecuteScalar();
                            connect.Close();
                            RemoveOracleCommand(cmd);
                            RemoveOracleConnection(connect);
                            return true;
                        }
                        catch (Exception)
                        {
                            RemoveOracleCommand(cmd);
                            RemoveOracleConnection(connect);
                            throw;
                        }
                    }
                }
            });
        }
        public List<SVOD_FILE_Row> SVOD_FILE_TEMP99()
        {
            using (var oda = new OracleDataAdapter("select * from SVOD_FILE_TEMP99", con))
            {
                var tbl = new DataTable();
                oda.Fill(tbl);
                return tbl.Select().Select(SVOD_FILE_Row.Get).ToList();
            }
        }

       

        public List<STAT_VIDMP_Row> STAT_VIDMP_TEMP99()
        {
            using (var oda = new OracleDataAdapter("select * from STAT_VIDMP_TEMP99", con))
            {
                var tbl = new DataTable();
                oda.Fill(tbl);
                return tbl.Select().Select(STAT_VIDMP_Row.Get).ToList();
            }
        }
        
        public List<STAT_FULL_Row> STAT_FULL_TEMP99()
        {
            using (var oda = new OracleDataAdapter("select * from STAT_FULL_TEMP99", con))
            {
                var tbl = new DataTable();
                oda.Fill(tbl);
                return tbl.Select().Select(STAT_FULL_Row.Get).ToList();
            }
        }
        /// <summary>
        /// Найти случаи в БД
        /// </summary>
        /// <param name="ZL">Файл</param>
        /// <param name="fi">Указатель</param>
        /// <param name="control">Контрол для использование диспетчера(если fi привязан)</param>
        /// <returns></returns>
        public bool IdentySluch(ZL_LIST ZL, FileItem fi,  System.Windows.Forms.Control control = null, List<FindSluchItem> IdentInfo = null)
        {
            try
            {
                if(IdentInfo==null)
                    IdentInfo = Get_IdentInfo(ZL, fi, control);
                SetID(IdentInfo, ZL.GetHashTable());
                var t = ZL.ZAP.SelectMany(x => x.Z_SL_list).Where(x => x.SL.Count(y => !y.SLUCH_ID.HasValue) != 0).ToList();
                foreach (var row in t)
                {
                    foreach (var sl in row.SL)
                    {
                        fi.FileLog.WriteLn($"Не удалось найти случай IDCASE={row.IDCASE} SL_ID {sl.SL_ID}: {row.TagComment}{sl.TagComment}");
                    }
                }
                return !t.Any();
            }
            catch (Exception ex)
            {
                fi.FileLog.WriteLn("Ошибка при идентификации случаев: " + ex.StackTrace + ex.Message);
                fi.InvokeComment("Ошибка при идентификации случаев: " + ex.Message, control);
                return false;
            }
        }

        public bool IdentySluch(ZL_LIST ZL, FileItem fi, DispatcherObject control = null, List<FindSluchItem> IdentInfo = null)
        {
            try
            {
                if (IdentInfo == null)
                    IdentInfo = Get_IdentInfo(ZL, fi, control);
                SetID(IdentInfo, ZL.GetHashTable());
                var t = ZL.ZAP.SelectMany(x => x.Z_SL_list).Where(x => x.SL.Count(y => !y.SLUCH_ID.HasValue) != 0).ToList();
                foreach (var row in t)
                {
                    foreach (var sl in row.SL)
                    {
                        fi.FileLog.WriteLn($"Не удалось найти случай IDCASE={row.IDCASE} SL_ID {sl.SL_ID}: {row.TagComment}{sl.TagComment}");
                    }
                }
                return !t.Any();
            }
            catch (Exception ex)
            {
                fi.FileLog.WriteLn($"Ошибка при идентификации случаев: {ex.StackTrace}{ex.Message}");
                fi.InvokeComm($"Ошибка при идентификации случаев: {ex.Message}", control);
                return false;
            }
        }

   

        public Dictionary<int, List<FindSANKItem>> GetSank(ZL_LIST ZL, FileItem fi, System.Windows.Forms.Control control = null)
        {
            var sank = new Dictionary<int, List<FindSANKItem>>();
            var zslarray = ZL.ZAP.SelectMany(x => x.Z_SL_list).Select(x => Convert.ToInt32(x.SLUCH_Z_ID)).ToList();
            var listSLUCH_Z_ID = new List<int>();
            for (var i = 0; i < zslarray.Count; i++)
            {
                var z_sl = zslarray[i];
              
                fi.InvokeComment($"Сбор санкций {i+1}/{zslarray.Count}", control);
                sank.Add(z_sl, new List<FindSANKItem>());
                listSLUCH_Z_ID.Add(z_sl);
                if (listSLUCH_Z_ID.Count == 500 || i+1 == zslarray.Count)
                {
                    var sanks = GetSANK(listSLUCH_Z_ID);
                    foreach (var san in sanks)
                    {
                        sank[san.SLUCH_Z_ID].Add(san);
                    }
                    listSLUCH_Z_ID.Clear();
                }
            }

            return sank;
        }
        public Dictionary<int, List<FindSANKItem>> GetSank(ZL_LIST ZL, FileItem fi, DispatcherObject control = null)
        {
            var sank = new Dictionary<int, List<FindSANKItem>>();
            var zslarray = ZL.ZAP.SelectMany(x => x.Z_SL_list).Select(x => Convert.ToInt32(x.SLUCH_Z_ID)).ToList();
            var listSLUCH_Z_ID = new List<int>();
            for (var i = 0; i < zslarray.Count; i++)
            {
                var z_sl = zslarray[i];

                fi.InvokeComm($"Сбор санкций {i + 1}/{zslarray.Count}", control);
                sank.Add(z_sl, new List<FindSANKItem>());
                listSLUCH_Z_ID.Add(z_sl);
                if (listSLUCH_Z_ID.Count == 500 || i + 1 == zslarray.Count)
                {
                    var sanks = GetSANK(listSLUCH_Z_ID);
                    foreach (var san in sanks)
                    {
                        sank[san.SLUCH_Z_ID].Add(san);
                    }
                    listSLUCH_Z_ID.Clear();
                }
            }

            return sank;
        }

        public List<FindSANKItem> FindACT(string NUM_ACT, DateTime D_ACT,string SMO)
        {
            var sank_tbl = H_SANK.FullTableName;
            var oda = new OracleDataAdapter($@"select san.SLUCH_Z_ID, san.S_SUM, san.S_TIP, san.S_OSN, san.DATE_ACT, san.NUM_ACT, sz.YEAR_SANK, sz.MONTH_SANK, sz.FILENAME, sz.DATE_INVITE from {sank_tbl} san
inner join xml_h_sank_zglv_v3 sz on (sz.zglv_id = san.s_zglv_id)
where san.date_act = :date_act and san.num_act = :num_act and sz.SMO = :smo", con);
            oda.SelectCommand.Parameters.Add(new OracleParameter("date_act", D_ACT));
            oda.SelectCommand.Parameters.Add(new OracleParameter("num_act", NUM_ACT));
            oda.SelectCommand.Parameters.Add(new OracleParameter("SMO", SMO));


            if (!Transaction)
                oda.SelectCommand.Connection.Open();
            var tbl = new DataTable();
            oda.Fill(tbl);
            if (!Transaction)
                oda.SelectCommand.Connection.Close();

            return tbl.Select().Select(x => new FindSANKItem(
                Convert.ToInt32(x["SLUCH_Z_ID"]),
                Convert.ToDecimal(x["S_SUM"]),
                Convert.ToInt32(x["S_TIP"]),
                Convert.ToInt32(x["S_OSN"]),
                (DateTime)x["DATE_ACT"],
                x["NUM_ACT"].ToString(),
                Convert.ToInt32(x["YEAR_SANK"]),
                Convert.ToInt32(x["MONTH_SANK"]),
                x["FILENAME"].ToString(),
                (DateTime)x["DATE_INVITE"])).ToList();
        }

        private void SetID(List<FindSluchItem> IDCASEs, Dictionary<decimal, Z_SL> tab)
        {
            foreach (var IDCASE in IDCASEs)
            {
                var dr_ZSL = tab[IDCASE.IDCASE];

                var dr = dr_ZSL.SL.Where(x => x.SL_ID == IDCASE.SL_ID).ToList();
                var isErr = false;
                if (dr.Count > 1)
                {
                    dr_ZSL.TagComment = $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} найдено более 1го случая. Доступны: {string.Join(",", dr_ZSL.SL.Select(x => x.SL_ID))}";
                    continue;
                }
                if (dr.Count == 0)
                {
                    dr_ZSL.TagComment = $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} не найдено случая.  Доступны: {string.Join(",", dr_ZSL.SL.Select(x => x.SL_ID))}";
                    continue;
                }

                dr_ZSL.SLUCH_Z_ID = IDCASE.SLUCH_Z_ID;
                var dr_sl = dr[0];
                if (dr_sl.DATE_1 != IDCASE.DATE_1)
                {
                    isErr = true;
                    dr_sl.TagComment +=$"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} DATE_1({dr_sl.DATE_1:MM.dd.yyyy}) не соответствует DATE_1({IDCASE.DATE_1:MM.dd.yyyy}) в БД";
                }
                if (dr_sl.DATE_2 != IDCASE.DATE_2)
                {
                    isErr = true;
                    dr_sl.TagComment +=$"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} DATE_2({dr_sl.DATE_2:MM.dd.yyyy}) не соответствует DATE_2({IDCASE.DATE_2:MM.dd.yyyy}) в БД";
                }
                if (dr_sl.NHISTORY != IDCASE.NHISTORY)
                {
                    isErr = true;
                    dr_sl.TagComment +=$"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} NHISTORY({dr_sl.NHISTORY}) не соответствует NHISTORY({IDCASE.NHISTORY}) в БД";
                }
                if (dr_sl.SUM_M != IDCASE.SUM_M)
                {
                    isErr = true;
                    dr_sl.TagComment +=$"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} SUM_M({dr_sl.SUM_M}) не соответствует SUM_M({IDCASE.SUM_M}) в БД";
                }


                if (dr_sl.DS1 != IDCASE.DS1 && dr_sl.DS1!=null)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} DS1({dr_sl.DS1}) не соответствует DS1({IDCASE.DS1}) в БД";
                }
                if (dr_ZSL.RSLT != IDCASE.RSLT && dr_ZSL.RSLT.HasValue)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} RSLT({dr_ZSL.RSLT}) не соответствует RSLT({IDCASE.RSLT}) в БД";
                }

                if (isErr) continue;

                dr_sl.SLUCH_ID = IDCASE.SLUCH_ID;
                dr_sl.SLUCH_Z_ID = IDCASE.SLUCH_Z_ID;
                foreach (var urow in dr_sl.USL)
                {
                    urow.SLUCH_ID = IDCASE.SLUCH_ID;
                }
                foreach (var urow in dr_ZSL.SANK)
                {
                    urow.SLUCH_Z_ID = IDCASE.SLUCH_Z_ID;
                }
            }
        }



        public List<FindSluchItem> Get_IdentInfo(ZL_LIST ZL, FileItem fi, System.Windows.Forms.Control control = null)
        {
            var result = new List<FindSluchItem>();
            fi.FileLog.WriteLn("Идентификация случаев");
            if (!fi.ZGLV_ID.HasValue) throw new Exception("Нет указателя на счет");

            var ZGLV_ID = fi.ZGLV_ID.Value;
            var idcases = new List<decimal>();
            var zslarray = ZL.ZAP.SelectMany(x => x.Z_SL_list).ToList();

            for (var i = 0; i < zslarray.Count; i++)
            {
                var z_sl = zslarray[i];

                fi.InvokeComment($"Идентификация {i + 1}/{zslarray.Count}", control);
                idcases.Add(z_sl.IDCASE);
                if (idcases.Count == 500 || i == zslarray.Count - 1)
                {
                    result.AddRange(GetID_CASE(idcases, ZGLV_ID)); 
                    idcases.Clear();
                }
            }
            fi.FileLog.WriteLn("Идентификация завершена");
            fi.InvokeComment("Идентификация завершена", control);
            return result;
        }

        public List<FindSluchItem> Get_IdentInfo(ZL_LIST ZL, FileItem fi, DispatcherObject control = null)
        {
            var result = new List<FindSluchItem>();
            fi.FileLog.WriteLn("Идентификация случаев");
            if (!fi.ZGLV_ID.HasValue) throw new Exception("Нет указателя на счет");

            var ZGLV_ID = fi.ZGLV_ID.Value;
            var idcases = new List<decimal>();
            var zslarray = ZL.ZAP.SelectMany(x => x.Z_SL_list).ToList();

            for (var i = 0; i < zslarray.Count; i++)
            {
                var z_sl = zslarray[i];

                fi.InvokeComm($"Идентификация {i + 1}/{zslarray.Count}", control);
                idcases.Add(z_sl.IDCASE);
                if (idcases.Count == 500 || i == zslarray.Count - 1)
                {
                    result.AddRange(GetID_CASE(idcases, ZGLV_ID));
                    idcases.Clear();
                }
            }
            fi.FileLog.WriteLn("Идентификация завершена");
            fi.InvokeComm("Идентификация завершена", control);
            return result;
        }
        private List<FindSluchItem> GetID_CASE(IEnumerable<decimal> idcase, int ZGLV_ID)
        {
            var sluch_z = H_Z_SLUCH.FullTableName;
            var sluch = H_SLUCH.FullTableName;
            var zap = H_ZAP.FullTableName;
            var schet = H_SCHET.FullTableName;

            var oda = new OracleDataAdapter($@"select t.sluch_id,zs.sluch_z_id, to_char(nvl(t.sl_id,'0')) sl_id,zs.idcase,t.date_1,t.date_2,zs.usl_ok,t.nhistory,nvl(nvl(t.sum_m_tfoms,t.sum_m),nvl(zs.sumv_tfoms,zs.sumv))  sum_m, t.ds1, zs.rslt from {sluch} t
inner join {sluch_z} zs on (zs.sluch_z_id = t.sluch_z_id)
inner join {zap} z on (z.zap_id = zs.zap_id)
inner join {schet} s on (s.schet_id = z.schet_id)
where zs.idcase  in ({string.Join(",", idcase)}) and s.zglv_id = :zglv_id", con);
            oda.SelectCommand.Parameters.Add("zglv_id", ZGLV_ID);
            if (!Transaction)
                oda.SelectCommand.Connection.Open();
            var tbl = new DataTable();
            oda.Fill(tbl);
            if (!Transaction)
                oda.SelectCommand.Connection.Close();

            return  tbl.Select().Select(x => new FindSluchItem(
                Convert.ToInt64(x["SLUCH_Z_ID"]),
                Convert.ToInt64(x["sluch_id"]), 
                Convert.ToString(x["SL_ID"]), 
                Convert.ToInt64(x["idcase"]),
                (DateTime) x["date_1"], 
                (DateTime) x["date_2"],
                x["USL_OK"]==DBNull.Value? (int?)null : Convert.ToInt32(x["USL_OK"]), 
                x["nhistory"].ToString(),
                Convert.ToDecimal(x["sum_m"]),
                Convert.ToString(x["DS1"]),
                x["RSLT"] == DBNull.Value ? (int?)null : Convert.ToInt32(x["RSLT"]))).ToList();
        }

        private List<FindSANKItem> GetSANK(IEnumerable<int> SLUCH_Z_ID)
        {
            var sank_tbl = H_SANK.FullTableName;
            var oda = new OracleDataAdapter($@"select san.SLUCH_Z_ID, san.S_SUM, san.S_TIP, san.S_OSN, san.DATE_ACT, san.NUM_ACT, sz.YEAR_SANK, sz.MONTH_SANK, sz.FILENAME, sz.DATE_INVITE from {sank_tbl} san
inner join xml_h_sank_zglv_v3 sz on (sz.zglv_id = san.s_zglv_id)
where san.SLUCH_Z_ID  in ({string.Join(",", SLUCH_Z_ID)}) and IsNOTFINISH=0", con);
          
            if (!Transaction)
                oda.SelectCommand.Connection.Open();
            var tbl = new DataTable();
            oda.Fill(tbl);
            if (!Transaction)
                oda.SelectCommand.Connection.Close();

            return tbl.Select().Select(x => new FindSANKItem(
               Convert.ToInt32(x["SLUCH_Z_ID"]),
               Convert.ToDecimal(x["S_SUM"]),
               Convert.ToInt32(x["S_TIP"]),
               Convert.ToInt32(x["S_OSN"]),
               (DateTime)x["DATE_ACT"],
               x["NUM_ACT"].ToString(),
               Convert.ToInt32(x["YEAR_SANK"]),
               Convert.ToInt32(x["MONTH_SANK"]),
               x["FILENAME"].ToString(),
               (DateTime)x["DATE_INVITE"])).ToList();
        }


        public bool LoadSANK(FileItem fi, ZL_LIST ZL, decimal? S_ZGLV_ID, bool setSUMP, bool isRewrite, System.Windows.Forms.Control control = null, List<FindSluchItem> IdentInfo=null)
        {
            fi.FileLog.WriteLn("Чтение файла " + fi.FileName);
            fi.FileLog.WriteLn("Подготовка к переносу в БД " + fi.FileName);
            fi.InvokeComment("Обработка пакета: Подготовка к переносу в БД " + fi.FileName, control);


            var rez_ind = IdentySluch(ZL, fi, control, IdentInfo);
            if (rez_ind == false)
            {
                fi.InvokeComment("Не полная идентификация в загрузке отказано", control);
                fi.FileLog.WriteLn("Не полная идентификация в загрузке отказано");
                return false;
            }
            fi.InvokeComment("Подготовка к загрузке санкции", control);

            fi.FileLog.WriteLn("Подготовка к загрузке санкции");
            if (!S_ZGLV_ID.HasValue)
                throw new Exception("S_ZGLV_ID не указан");

            var Z_SL = ZL.ZAP.SelectMany(x => x.Z_SL_list).ToList();
            var SL = Z_SL.SelectMany(x => x.SL).ToList();
            var SANK = Z_SL.SelectMany(x => x.SANK).ToList();
            var USL = SL.SelectMany(x => x.USL).ToList();
            var EXP = SANK.SelectMany(x => x.CODE_EXP).Where(x=>!string.IsNullOrEmpty(x.VALUE));


            //Если до версии 3.1 то ставим на санкции случай
     
            if (ZL.ZGLV.Vers < 3.1m)
            {
                fi.FileLog.WriteLn("Подготовка к загрузке санкции: идентификация случая для старых файлов");
                var er = IDENTY_SANK_SLUCH_ID(Z_SL);
                if (er.Count != 0)
                {
                    foreach (var e in er)
                    {
                        fi.FileLog.WriteLn(e);
                    }
                    return false;
                }
            }
        


            var SANK_ID = GetSec(H_SANK.SeqName, SANK.Count());
            foreach (var san in SANK)
            {
                san.S_ZGLV_ID = S_ZGLV_ID;
                san.SANK_ID = SANK_ID;
                foreach(var c in san.CODE_EXP)
                {
                    c.SANK_ID = san.SANK_ID;
                }
                SANK_ID++;

            }
            fi.FileLog.WriteLn("Загрузка санкций");
            InsertSANK(SANK);
            InsertCODE_EXP(EXP);
            //СуммП ставим
            fi.InvokeComment("Внесение суммы принятой", control);
            fi.FileLog.WriteLn("Внесение суммы принятой");
            if (setSUMP)
            {
                var sl_count = UpdateSLUCH_SUM_P(SL, isRewrite);
                var zsl_count = UpdateSLUCH_Z_SUM_P(Z_SL, isRewrite);
                var zsl_ZGLV_count = UpdateSLUCH_Z_SANK_ZGLV_ID(Z_SL, S_ZGLV_ID.Value, isRewrite);
                var usl_count = UpdateUSL_SUM_P(USL, isRewrite);
                var err = true;
                if (SL.Count != sl_count)
                {
                    fi.InvokeComment("Не полное внесение суммы принятой для случаев", control);
                    fi.FileLog.WriteLn(
                        $"Не полное внесение суммы принятой для случаев: внесено {sl_count} из {SL.Count}");
                    err = false;
                }

                if (Z_SL.Count != zsl_count)
                {
                    fi.InvokeComment("Не полное внесение суммы принятой для законченных случаев", control);
                    fi.FileLog.WriteLn(
                        $"Не полное внесение суммы принятой для законченных случаев: внесено {zsl_count} из {Z_SL.Count}");
                    err = false;
                }

                if (USL.Count != usl_count)
                {
                    fi.InvokeComment("Не полное внесение суммы принятой для услуг", control);
                    fi.FileLog.WriteLn(
                        $"Не полное внесение суммы принятой для услуг: внесено {usl_count} из {USL.Count}");
                    err = false;
                }
                if (Z_SL.Count != zsl_ZGLV_count)
                {
                    fi.InvokeComment("Не полное внесение заголовка на случай", control);
                    fi.FileLog.WriteLn(
                        $"Не полное внесение заголовка на случай: внесено {zsl_ZGLV_count} из {Z_SL.Count}");
                    err = false;
                }

                if (!err)
                    return false;
            }
            fi.InvokeComment("Загрузка завершена", control);
            fi.FileLog.WriteLn("Загрузка завершена");
            return true;
        }

        public bool LoadSANK(FileItem fi, ZL_LIST ZL, decimal? S_ZGLV_ID, bool setSUMP, bool isRewrite, DispatcherObject control = null, List<FindSluchItem> IdentInfo = null)
        {
            fi.FileLog.WriteLn("Чтение файла " + fi.FileName);
            fi.FileLog.WriteLn("Подготовка к переносу в БД " + fi.FileName);
            fi.InvokeComm("Обработка пакета: Подготовка к переносу в БД " + fi.FileName, control);


            var rez_ind = IdentySluch(ZL, fi, control, IdentInfo);
            if (rez_ind == false)
            {
                fi.InvokeComm("Не полная идентификация в загрузке отказано", control);
                fi.FileLog.WriteLn("Не полная идентификация в загрузке отказано");
                return false;
            }
            fi.InvokeComm("Подготовка к загрузке санкции", control);

            fi.FileLog.WriteLn("Подготовка к загрузке санкции");
            if (!S_ZGLV_ID.HasValue)
                throw new Exception("S_ZGLV_ID не указан");

            var Z_SL = ZL.ZAP.SelectMany(x => x.Z_SL_list).ToList();
            var SL = Z_SL.SelectMany(x => x.SL).ToList();
            var SANK = Z_SL.SelectMany(x => x.SANK).ToList();
            var USL = SL.SelectMany(x => x.USL).ToList();
            var EXP = SANK.SelectMany(x => x.CODE_EXP).Where(x => !string.IsNullOrEmpty(x.VALUE));


            //Если до версии 3.1 то ставим на санкции случай

            if (ZL.ZGLV.Vers < 3.1m)
            {
                fi.FileLog.WriteLn("Подготовка к загрузке санкции: идентификация случая для старых файлов");
                var er = IDENTY_SANK_SLUCH_ID(Z_SL);
                if (er.Count != 0)
                {
                    foreach (var e in er)
                    {
                        fi.FileLog.WriteLn(e);
                    }
                    return false;
                }
            }



            var SANK_ID = GetSec(H_SANK.SeqName, SANK.Count());
            foreach (var san in SANK)
            {
                san.S_ZGLV_ID = S_ZGLV_ID;
                san.SANK_ID = SANK_ID;
                foreach (var c in san.CODE_EXP)
                {
                    c.SANK_ID = san.SANK_ID;
                }
                SANK_ID++;

            }
            fi.FileLog.WriteLn("Загрузка санкций");
            InsertSANK(SANK);
            InsertCODE_EXP(EXP);
            //СуммП ставим
            fi.InvokeComm("Внесение суммы принятой", control);
            fi.FileLog.WriteLn("Внесение суммы принятой");
            if (setSUMP)
            {
                var sl_count = UpdateSLUCH_SUM_P(SL, isRewrite);
                var zsl_count = UpdateSLUCH_Z_SUM_P(Z_SL, isRewrite);
                var zsl_ZGLV_count = UpdateSLUCH_Z_SANK_ZGLV_ID(Z_SL, S_ZGLV_ID.Value, isRewrite);
                var usl_count = UpdateUSL_SUM_P(USL, isRewrite);
                var err = true;
                if (SL.Count != sl_count)
                {
                    fi.InvokeComm("Не полное внесение суммы принятой для случаев", control);
                    fi.FileLog.WriteLn($"Не полное внесение суммы принятой для случаев: внесено {sl_count} из {SL.Count}");
                    err = false;
                }

                if (Z_SL.Count != zsl_count)
                {
                    fi.InvokeComm("Не полное внесение суммы принятой для законченных случаев", control);
                    fi.FileLog.WriteLn($"Не полное внесение суммы принятой для законченных случаев: внесено {zsl_count} из {Z_SL.Count}");
                    err = false;
                }

                if (USL.Count != usl_count)
                {
                    fi.InvokeComm("Не полное внесение суммы принятой для услуг", control);
                    fi.FileLog.WriteLn($"Не полное внесение суммы принятой для услуг: внесено {usl_count} из {USL.Count}");
                    err = false;
                }
                if (Z_SL.Count != zsl_ZGLV_count)
                {
                    fi.InvokeComm("Не полное внесение заголовка на случай", control);
                    fi.FileLog.WriteLn(
                        $"Не полное внесение заголовка на случай: внесено {zsl_ZGLV_count} из {Z_SL.Count}");
                    err = false;
                }

                if (!err)
                    return false;
            }
            fi.InvokeComm("Загрузка завершена", control);
            fi.FileLog.WriteLn("Загрузка завершена");
            return true;
        }
        public List<string> IDENTY_SANK_SLUCH_ID(IEnumerable<Z_SL> Z_SL)
        {
            try
            {
                var err = new List<string>();
                foreach (var row in Z_SL)
                {
                    foreach (var rowS in row.SANK)
                    {
                        decimal? fSLUCH_ID = null;
                        if (rowS.SL_ID.Count == 1)
                        {
                            var sls = row.SL.FirstOrDefault(x => x.SL_ID.ToString() == rowS.SL_ID[0]);
                            if (sls != null)
                            {
                                fSLUCH_ID = sls.SLUCH_ID;
                            }
                        }
                        rowS.SLUCH_ID = fSLUCH_ID;
                        if (!rowS.SLUCH_ID.HasValue)
                        {
                            err.Add($"Не удалось найти случай санкции {row.IDCASE}|S_CODE={rowS.S_CODE}");

                        }
                    }
                }

                return err;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в IDENTY_SANK_SLUCH_ID:{ex.Message}", ex);
            }
        }

        /// <summary>
        /// Сумма принятая на законченых случаях
        /// </summary>
        /// <param name="tbl">Таблица с полями SUMP, OPLATA, SLUCH_Z_ID_BASE </param>
        public int UpdateSLUCH_Z_SUM_P(IEnumerable<Z_SL> Items, bool isRewrite = false)
        {
            try
            {
                var cmd = NewOracleCommand($@"update {H_Z_SLUCH.FullTableName} t set t.SUMP = :SUMP, t.oplata = :oplata where t.sluch_z_id = :sluch_z_id{(isRewrite ? "" : " and SUMP is null")}", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.Parameters.Add("SUMP", OracleDbType.Decimal, Items.Select(x => x.SUMP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("oplata", OracleDbType.Decimal, Items.Select(x => x.OPLATA ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("sluch_z_id", OracleDbType.Decimal, Items.Select(x => x.SLUCH_Z_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                var Count = cmd.ExecuteNonQuery();
                RemoveOracleCommand(cmd);
                return Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в UpdateSLUCH_Z_SUM_P: {ex.Message}", ex);
            }
        }
        public int UpdateSLUCH_Z_SANK_ZGLV_ID(IEnumerable<Z_SL> Items, decimal SANK_ZGLV_ID, bool isRewrite = false)
        {
            try
            {
                var cmd = NewOracleCommand($@"update {H_Z_SLUCH.FullTableName} t set t.SANK_ZGLV_ID = :SANK_ZGLV_ID  where t.sluch_z_id = :sluch_z_id{(isRewrite ? "" : " and SANK_ZGLV_ID is null")}", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.Parameters.Add("SANK_ZGLV_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_Z_ID.HasValue ? SANK_ZGLV_ID : (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("sluch_z_id", OracleDbType.Decimal, Items.Select(x => x.SLUCH_Z_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                var Count = cmd.ExecuteNonQuery();
                RemoveOracleCommand(cmd);
                return Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в UpdateSLUCH_Z_SANK_ZGLV_ID: {ex.Message}", ex);
            }
        }
        /// <summary>
        /// Сумма принятая на услугах
        /// </summary>
        /// <param name="tbl">Таблица с полями SUMP_USL, SLUCH_ID_BASE, idserv </param>
        public int UpdateUSL_SUM_P(IEnumerable<USL> Items, bool isRewrite = false)
        {
            try
            {
                var cmd = NewOracleCommand($@"update {H_USL.FullTableName} t set t.SUMP_USL = :SUMP_USL  where t.sluch_id = :sluch_id and t.idserv = :idserv{(isRewrite? "" : " and SUMP_USL is null")}", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.Parameters.Add("SUMP_USL", OracleDbType.Decimal, Items.Select(x => x.SUMP_USL ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("sluch_id", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("idserv", Items.Select(x => x.IDSERV).ToArray());
                var Count = cmd.ExecuteNonQuery();
                RemoveOracleCommand(cmd);
                return Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в UpdateUSL_SUM_P: {ex.Message}", ex);
            }

        }
        /// <summary>
        /// Сумма принятая на случаях
        /// </summary>
        /// <param name="tbl">Таблица с полями SUM_MP, SLUCH_ID_BASE</param>
        public int UpdateSLUCH_SUM_P(IEnumerable<SL> Items, bool isRewrite = false)
        {
            try
            {
                var cmd = NewOracleCommand($@"update {H_SLUCH.FullTableName} t set t.SUM_MP = :SUM_MP  where t.sluch_id = :sluch_id{(isRewrite ? "" : " and SUM_MP is null")}", con);
                cmd.ArrayBindCount = Items.Count();
                cmd.Parameters.Add("SUM_MP", OracleDbType.Decimal, Items.Select(x => x.SUM_MP ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);
                cmd.Parameters.Add("SLUCH_ID", OracleDbType.Decimal, Items.Select(x => x.SLUCH_ID ?? (object)DBNull.Value).ToArray(), ParameterDirection.Input);

                var Count = cmd.ExecuteNonQuery();
                RemoveOracleCommand(cmd);
                return Count;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка в UpdateSLUCH_SUM_P: {ex.Message}", ex);
            }
        }


        void Trunc(TableName nameTBL)
        {
            var table = new TableInfo();

            switch (nameTBL)
            {
                case TableName.L_PERS: table = L_PERS; break;
                case TableName.L_ZGLV: table = L_ZGLV; break;
                case TableName.PACIENT: table = H_PAC; break;
                case TableName.SANK: table = H_SANK; break;
                case TableName.SCHET: table = H_SCHET; break;
                case TableName.SLUCH: table = H_SLUCH; break;
                case TableName.USL: table = H_USL; break;
                case TableName.ZAP: table = H_ZAP; break;
                case TableName.ZGLV: table = H_ZGLV; break;
                case TableName.NAZR: table = H_NAZR; break;
                case TableName.DS2_N: table = H_DS2_N; break;
                case TableName.KSLP: table = H_KSLP; break;
                case TableName.SLUCH_Z: table = H_Z_SLUCH; break;
                case TableName.B_DIAG: table = H_B_DIAG; break;
                case TableName.B_PROT: table = H_B_PROT; break;
                case TableName.NAPR: table = H_NAPR; break;
                case TableName.LEK_PR: table = H_LEK_PR; break;
                case TableName.LEK_PR_DATE_INJ: table = H_LEK_DATE_INJ; break;
                case TableName.ONK_USL: table = H_ONK_USL; break;
                case TableName.CONS: table = H_CONS; break;
                case TableName.EXP: table = H_CODE_EXP; break;
                case TableName.DS2: table = H_DS2; break;
                case TableName.DS3: table = H_DS3; break;
                case TableName.CRIT: table = H_CRIT; break;
            }
          
            var FKlist = new DataTable();
            //запрос списка внешних ключей
            using (var oda = new OracleDataAdapter($@"select constraint_name, table_name
                                                    from user_constraints 
                                                    where r_constraint_name in
                                                    (
                                                    select constraint_name
                                                    from user_constraints
                                                    where constraint_type in ('P','U')
                                                    and table_name = upper('{table.TableName}') and owner = upper('{table.SchemaName}'))", con))
            {
                oda.Fill(FKlist);
            }
            //Отключаем все ключи
            foreach (DataRow row in FKlist.Rows)
            {
                using (var cmddisabled = NewOracleCommand($"alter table {row["table_name"]} disable CONSTRAINT  {row["constraint_name"]}", con))
                {
                    con.Open();
                    cmddisabled.ExecuteNonQuery();
                    con.Close();
                    RemoveOracleCommand(cmddisabled);
                }
            }

            using (var cmd = NewOracleCommand($"truncate table {table.FullTableName}", con))
            {
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                RemoveOracleCommand(cmd);
            }


            //Включаем все ключи
            foreach (DataRow row in FKlist.Rows)
            {
                using (var cmddisabled = NewOracleCommand($"alter table {row["table_name"]} enable CONSTRAINT  {row["constraint_name"]}", con))
                {
                    con.Open();
                    cmddisabled.ExecuteNonQuery();
                    con.Close();
                    RemoveOracleCommand(cmddisabled);
                }
            }
        }

        /// <summary>
        /// Отчистить таблицу
        /// </summary>
        /// <param name="nameTBL">Имя таблицы</param>
        public void TruncALL()
        {

            Trunc(TableName.LEK_PR_DATE_INJ);
            Trunc(TableName.LEK_PR);
            Trunc(TableName.CONS);
            Trunc(TableName.ONK_USL);
            Trunc(TableName.EXP);
            Trunc(TableName.SANK);
            Trunc(TableName.B_PROT);
            Trunc(TableName.B_DIAG);
            Trunc(TableName.NAPR);
            Trunc(TableName.USL);
            Trunc(TableName.NAZR);
            Trunc(TableName.DS2_N);
            Trunc(TableName.KSLP);
            Trunc(TableName.DS2);
            Trunc(TableName.DS3);
            Trunc(TableName.CRIT);
            Trunc(TableName.SLUCH);
            Trunc(TableName.SLUCH_Z);
            Trunc(TableName.PACIENT);
            Trunc(TableName.ZAP);
            Trunc(TableName.SCHET);
            Trunc(TableName.L_PERS);
            Trunc(TableName.L_ZGLV);
            Trunc(TableName.ZGLV);


        }

        public List<V_XML_CHECK_FILENAMErow> GetZGLV_BYFileName(string FileName)
        {
            var tblRes = new DataTable();
            using (var oda = new OracleDataAdapter($@"select * from V_XML_CHECK_FILENAME t where upper(t.filename) = :FILENAME", con))
            {
                oda.SelectCommand.Parameters.Add("FILENAME", FileName.ToUpper());
                oda.Fill(tblRes);
            }
            return tblRes.Select().Select(V_XML_CHECK_FILENAMErow.Get).ToList();
        }
        public List<V_XML_CHECK_FILENAMErow> GetZGLV_BYCODE_CODE_MO(int code, string MO, int YEAR)
        {
            var tblRes = new DataTable();

            using (
            var oda = new OracleDataAdapter(@"select t.* from V_XML_CHECK_FILENAME t where t.code = :code and t.code_mo = :code_mo and t.YEAR = :YEAR", con))
            {
                oda.SelectCommand.Parameters.Add("code", code);
                oda.SelectCommand.Parameters.Add("code_mo", MO);
                oda.SelectCommand.Parameters.Add("YEAR", YEAR);
                oda.Fill(tblRes);
            }
            return tblRes.Select().Select(V_XML_CHECK_FILENAMErow.Get).ToList();
        }

       

        public static string GetNameLPU(string R_COD, OracleConnection con)
        {
            using (var oda = new OracleDataAdapter(@"select * from nsi.lpu_s t where KOD_R = " + R_COD, con))
            {
                var tbl = new DataTable();
                oda.Fill(tbl);
                return tbl.Rows.Count != 0 ? tbl.Rows[0]["LPU"].ToString() : "";
            }
        }

        public string GetNameLPU(string R_COD)
        {
            using (var conn = new OracleConnection(ConnStr))
            {
                using (var oda = new OracleDataAdapter($@"select * from nsi.lpu_s t where KOD_R = {R_COD}", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Rows.Count != 0 ? tbl.Rows[0]["LPU"].ToString() : "";
                }
              
            }
         
        }

        public void Checking(TableName nameTBL, CheckingList list, CancellationToken cancel, ref string STAT)
        {
            using (var CONN = NewOracleConnection(ConnStr))
            {
                CONN.Open();
                using (var cmd = NewOracleCommand("", CONN))
                {
                    try
                    {
                        var l = list[nameTBL];
                        for (var i = 0; i < l.Count; i++)
                        {
                            var proc = l[i];
                            if (proc.STATE)
                            {
                                STAT = $"Обработка пакета: ФЛК и сбор ошибок: {nameTBL} {i + 1}/{l.Count}{proc.NAME_ERR}";

                                cmd.CommandText = $"begin {proc.NAME_PROC}({string.Join(",", proc.listParam.Select(x => $":{x.Name}"))}); end;";
                                cmd.CommandType = CommandType.Text;
                                cmd.Parameters.Clear();
                                foreach (var par in proc.listParam)
                                {
                                    var value = GetValueOrclParam(par);
                                    cmd.Parameters.Add(par.Name, par.Type, value, ParameterDirection.Input);
                                }

                                try
                                {
                                    if (cancel.IsCancellationRequested) throw new CancelException();
                                    cmd.ExecuteScalar();
                                    proc.Excist = StateExistProcedure.Exist;
                                }
                                catch (CancelException)
                                {
                                    throw;
                                }
                                catch (Exception ex)
                                {
                                    proc.Comment = ex.Message;
                                    proc.Excist = StateExistProcedure.NotExcist;
                                }
                            }
                        }

                        STAT = $"Обработка пакета: ФЛК и сбор ошибок: {nameTBL} завершено сбор ошибок";
                    }
                    finally
                    {
                        RemoveOracleCommand(cmd);
                        RemoveOracleConnection(CONN);
                    }
                }
                CONN.Close();
            }
        }


        private object GetValueOrclParam(OrclParam par)
        {
            var value = "";
            if (par.ValueType == TypeParamValue.value)
            {
                value = par.value;
            }
            else
            {

                switch (par.ValueType)
                {
                    case TypeParamValue.TABLE_NAME_ZGLV:
                        value = H_ZGLV.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_ZAP:
                        value = H_ZAP.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_USL:
                        value = H_USL.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_SLUCH:
                        value = H_SLUCH.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_SCHET:
                        value = H_SCHET.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_SANK:
                        value = H_SANK.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_PACIENT:
                        value = H_PAC.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_L_ZGLV:
                        value = L_ZGLV.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_L_PERS:
                        value = L_PERS.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_NAZR:
                        value = H_NAZR.FullTableName;
                        break;
                    case TypeParamValue.TABLE_NAME_DS2_N:
                        value = H_DS2_N.FullTableName;
                        break;
                    case TypeParamValue.CurrMonth:
                        value = curr_month.ToString();
                        break;
                    case TypeParamValue.CurrYear:
                        value = curr_year.ToString();
                        break;
                }
            }

            if (par.Type == OracleDbType.Int32)
                return Convert.ToInt32(value);
            return value;
        }
        /// <summary>
        /// Получить ошибки из v_xml_errors
        /// </summary>
        /// <returns>Таблица из View</returns>
        public List<V_ErrorViewRow> GetErrorView()
        {
            using (var View = NewOracleCommand($"select * from {xml_errors.FullTableName}", con))
            {
                View.CommandType = CommandType.Text;
                var oda = new OracleDataAdapter(View);
                var tbl = new DataTable();
                oda.Fill(tbl);
                RemoveOracleCommand(View);
                return tbl.Select().Select(V_ErrorViewRow.Get).ToList();
            }
        }

      

        /// <summary>
        /// Перенос таблицы
        /// </summary>
        /// <param name="tableFrom"></param>
        /// <param name="ownerFrom"></param>
        /// <param name="tableTo"></param>
        /// <param name="ownerTo"></param>
        /// <returns>Не перенесенные поля</returns>
        public TransferTableRESULT TransferTable(string tableFrom, string ownerFrom, string tableTo, string ownerTo)
        {
            var tbl = new DataTable();
     
            using (var oda = new OracleDataAdapter($@"select * from {ownerTo}.{tableTo} t where rownum = -1", con))
            {
                oda.Fill(tbl);
            }
            var ColumnTo = tbl.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();
            tbl = new DataTable();
            using (var oda = new OracleDataAdapter($@"select * from {ownerFrom}.{tableFrom} t where rownum = -1", con))
            {
                oda.Fill(tbl);
            }

            var ColumnFROM = tbl.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToList();

            var validateCol = ColumnFROM.Where(x => ColumnTo.Contains(x));
            var NOTvalidateCol = ColumnFROM.Where(x => !ColumnTo.Contains(x));

           var statement = string.Join(",", validateCol);
           var sql = $@"insert into {ownerTo}.{tableTo} ({statement}) select {statement} from {ownerFrom}.{tableFrom}";

           

            using (var cmd = NewOracleCommand(sql, con))
            {
                con.Open();
                cmd.ExecuteScalar();
                con.Close();
                RemoveOracleCommand(cmd);
            }
            return new TransferTableRESULT {Table = $"{ownerFrom}.{tableFrom}", Colums = NOTvalidateCol.ToList()}; 
        }

        /// <summary>
        /// Получить значение из sequence. И увеличить его на Count
        /// </summary>
        /// <param name="sec_name">Имя Sequence</param>
        /// <param name="count">Увеличение на count</param>
        /// <returns>Значение sequence</returns>
        public int GetSec(string sec_name, int count)
        {
            if (count == 0)
                return 0;
            int x;
            using (var conn = NewOracleConnection(ConnStr))
            {
                using (var cmd = NewOracleCommand($"select {sec_name}.nextval from dual", conn))
                {
                    cmd.Connection.Open();
                    x = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 1)
                    {
                        cmd.CommandText = $"alter sequence {sec_name} increment by {(count - 1)}";
                        cmd.ExecuteScalar();
                        cmd.CommandText = $"select {sec_name}.nextval from dual";
                        cmd.ExecuteScalar();
                    }
                    cmd.CommandText = $"alter sequence {sec_name} increment by 1";
                    cmd.ExecuteScalar();
                    cmd.Connection.Close();
                    RemoveOracleCommand(cmd);
                }
                RemoveOracleConnection(conn);
            }
            
            return x;
        }

        bool Transaction;
        OracleTransaction Tran;
        public void BeginTransaction()
        {
            con.Open();
            Transaction = true;
            Tran = con.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public void Commit()
        {
            if (Tran == null) return;
            Tran.Commit();
            con.Close();
            Transaction = false;
        }

        public void Rollback()
        {
            if (Tran == null) return;
            Tran.Rollback();
            con.Close();
            Transaction = false;
        }
   
        public void Dispose()
        {
            IsDispose = true;
            foreach (var cmd in CMDs)
            {
                if (cmd.Connection.State == ConnectionState.Open)
                {
                    cmd.Cancel();
                }

                while (cmd.Connection.State== ConnectionState.Open)
                {
                    Thread.Sleep(1000);
                }
                cmd.Dispose();
            }
            foreach (var cmd in Cons)
            {
                cmd.Dispose();
            }

            Tran?.Dispose();
            con?.Dispose();
        }
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
        public FindSluchItem(Int64 SLUCH_Z_ID, Int64 SLUCH_ID, string SL_ID, Int64 IDCASE, DateTime DATE_1, DateTime DATE_2, int? USL_OK, string NHISTORY, decimal SUM_M, string DS1, int? RSLT)
        {
            this.SLUCH_Z_ID = SLUCH_Z_ID;
            this.SLUCH_ID = SLUCH_ID;
            this.SL_ID = SL_ID;
            this.IDCASE = IDCASE;
            this.DATE_1 = DATE_1;
            this.DATE_2 = DATE_2;
            this.USL_OK = USL_OK;
            this.NHISTORY = NHISTORY;
            this.SUM_M = SUM_M;
            this.DS1 = DS1;
            this.RSLT = RSLT;
        }
        public Int64 SLUCH_ID { get; set; }
        public Int64 SLUCH_Z_ID { get; set; }
        public string SL_ID { get; set; }
        public Int64 IDCASE { get; set; }
        public DateTime DATE_1 { get; set; }
        public DateTime DATE_2 { get; set; }
        public int? USL_OK { get; set; }
        public string NHISTORY { get; set; }
        public decimal SUM_M { get; set; }
        public string DS1 { get; set; }
        public int? RSLT { get; set; }

    }
    public class FindSANKItem
    {
        public FindSANKItem(int SLUCH_Z_ID, decimal S_SUM, int S_TIP, int S_OSN, DateTime DATE_ACT, string NUM_ACT, int YEAR_SANK, int MONTH_SANK, string FILENAME, DateTime DATE_INVITE)
        {
            this.SLUCH_Z_ID = SLUCH_Z_ID;
            this.S_SUM = S_SUM;
            this.S_TIP = S_TIP;
            this.S_OSN = S_OSN;
            this.DATE_ACT = DATE_ACT;
            this.NUM_ACT = NUM_ACT;
            this.YEAR_SANK = YEAR_SANK;
            this.MONTH_SANK = MONTH_SANK;
            this.FILENAME = FILENAME;
            this.DATE_INVITE = DATE_INVITE;
        }
        public int SLUCH_Z_ID { get; set; }
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

}

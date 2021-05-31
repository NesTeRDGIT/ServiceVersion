using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF.MEK_RESULT.ACTMEK
{

    public class MO_ITEM:INotifyPropertyChanged
    {
        public static List<MO_ITEM> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static MO_ITEM Get(DataRow row)
        {
            try
            {
                var item = new MO_ITEM();
                item.CODE_MO = row["CODE_MO"].ToString();
                item.NAME_MOK = row["NAME_MOK"].ToString();
                item.YEAR = Convert.ToInt32(row["YEAR"]);
                item.MONTH = Convert.ToInt32(row["MONTH"]);
                item.N_ACT = row["N_ACT"].ToString();
                item.D_ACT = Convert.ToDateTime(row["D_ACT"]);
                item.DATE_INVITE = Convert.ToDateTime(row["DATE_INVITE"]);
                item.N_SCHET = row["N_SCHET"].ToString();
                if (row["D_SCHET"] != DBNull.Value)
                    item.D_SCHET = Convert.ToDateTime(row["D_SCHET"]);
                item.SMO = row["SMO"].ToString();
                item.NAME_SMOK = row["NAME_SMOK"].ToString();
                item.ZGLV_ID_BASE = row["ZGLV_ID_BASE"].ToString().Split(',').Select(x => Convert.ToInt32(x)).ToArray();
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения MO_ITEM: {ex.Message}", ex);
            }
        }

        private bool _IsSelect = true;

        public bool IsSelect
        {
            get => _IsSelect;
            set
            {
                _IsSelect = value;
                RaisePropertyChanged();
            }
        }

        public string CODE_MO { get; set; }
        public string NAME_MOK { get; set; }
        public string NAME_SMOK { get; set; }
        public string SMO { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string N_ACT { get; set; }
        public DateTime D_ACT { get; set; }
        public string N_SCHET { get; set; }
        public DateTime? D_SCHET { get; set; }
        public DateTime DATE_INVITE { get; set; }
        public int[] ZGLV_ID_BASE { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class MP_VOLUME_ITEM
    {
        public static List<MP_VOLUME_ITEM> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static MP_VOLUME_ITEM Get(DataRow row)
        {
            try
            {
                var item = new MP_VOLUME_ITEM();
                item.VOLUME_VALUE_ID = row["VOLUME_VALUE_ID"].ToString();
                item.RUB = row["RUB"].ToString();
                item.NAME = row["NAME"].ToString();
                item.KOL = Convert.ToDecimal(row["KOL"]);
                item.SUM = Convert.ToDecimal(row["SUM"]);
                item.KOL_MEK = Convert.ToDecimal(row["KOL_MEK"]);
                item.SUM_MEK = Convert.ToDecimal(row["SUM_MEK"]);
                item.KOL_P = Convert.ToDecimal(row["KOL_P"]);
                item.SUM_P = Convert.ToDecimal(row["SUM_P"]);

                if (row["PROFIL"] != DBNull.Value)
                    item.PROFIL = Convert.ToInt32(row["PROFIL"]);
                item.PROFIL_NAME = Convert.ToString(row["PROFIL_NAME"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения MP_VOLUME_ITEM: {ex.Message}", ex);
            }
        }

        public string VOLUME_VALUE_ID { get; set; }
        public string RUB { get; set; }
        public string NAME { get; set; }
        public decimal KOL { get; set; }
        public decimal SUM { get; set; }
        public decimal KOL_MEK { get; set; }
        public decimal SUM_MEK { get; set; }
        public decimal KOL_P { get; set; }
        public decimal SUM_P { get; set; }
        public int? PROFIL { get; set; }
        public string PROFIL_NAME { get; set; }
    }
    public class MP_DEFECT_ITEM
    {
        public static List<MP_DEFECT_ITEM> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static MP_DEFECT_ITEM Get(DataRow row)
        {
            try
            {
                var item = new MP_DEFECT_ITEM();
                item.OSN = row["OSN"].ToString();
                item.NAME = row["NAME"].ToString();
                item.COMM = row["COMM"].ToString();
                item.IsTARIF = Convert.ToBoolean(row["IsTARIF"]);
                item.IsLIC = Convert.ToBoolean(row["IsLIC"]);
                item.S_SUM = Convert.ToDecimal(row["S_SUM"]);
                item.isPVOL = Convert.ToBoolean(row["isPVOL"]);

                item.IDCASE = Convert.ToString(row["IDCASE"]);
                item.PODR = Convert.ToString(row["PODR"]);
                item.POLIS = Convert.ToString(row["POLIS"]);
                item.MKB = Convert.ToString(row["MKB"]);
                item.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                item.DATE_2 = Convert.ToDateTime(row["DATE_2"]);

                item.USL_OK = Convert.ToInt32(row["USL_OK"]);
                item.OTD = Convert.ToString(row["OTD"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения MP_DEFECT_ITEM: {ex.Message}", ex);
            }
        }


        public string IDCASE { get; set; }
        public string PODR { get; set; }
        public string OTD { get; set; }
        public string POLIS { get; set; }
        public string MKB { get; set; }
        public DateTime DATE_1 { get; set; }
        public DateTime DATE_2 { get; set; }

        public string OSN { get; set; }
        public string NAME { get; set; }
        public string COMM { get; set; }
        public decimal S_SUM { get; set; }

        public int USL_OK { get; set; }
        public bool IsTARIF { get; set; }
        public bool IsLIC { get; set; }
        /// <summary>
        /// Превышение объемов
        /// </summary>
        public bool isPVOL { get; set; }
    }
    public class MO_FOND_INFO
    {
        public static MO_FOND_INFO Get(DataRow row)
        {
            try
            {
                var item = new MO_FOND_INFO();
                item.AMB_S = Convert.ToDecimal(row["AMB_S"]);
                item.AMB_K = Convert.ToInt32(row["AMB_K"]);
                item.AMB_STANDART = Convert.ToDecimal(row["AMB_STANDART"]);
                item.SCOR_S = Convert.ToDecimal(row["SCOR_S"]);
                item.SCOR_K = Convert.ToInt32(row["SCOR_K"]);
                item.SCOR_STANDART = Convert.ToDecimal(row["SCOR_STANDART"]);
                item.FAP_S = Convert.ToDecimal(row["FAP_S"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения MO_FOND_INFO: {ex.Message}", ex);
            }
        }

        public decimal AMB_S { get; set; }
        public int AMB_K { get; set; }
        public decimal AMB_STANDART { get; set; }
        public decimal SCOR_S { get; set; }
        public int SCOR_K { get; set; }
        public decimal SCOR_STANDART { get; set; }
        public decimal FAP_S { get; set; }

        public static MO_FOND_INFO operator +(MO_FOND_INFO item1, MO_FOND_INFO item2)
        {
            return new MO_FOND_INFO
            {
                AMB_S = item1.AMB_S + item2.AMB_S,
                AMB_K = item1.AMB_K + item2.AMB_K,
                AMB_STANDART = item1.AMB_STANDART + item2.AMB_STANDART,
                SCOR_S = item1.SCOR_S + item2.SCOR_S,
                SCOR_K = item1.SCOR_K + item2.SCOR_K,
                SCOR_STANDART = item1.SCOR_STANDART + item2.SCOR_STANDART,
                FAP_S = item1.FAP_S + item2.FAP_S
            };
        }
    }
    public class MO_SMO
    {
        public MO_SMO(string MO, string SMO)
        {
            this.MO = MO;
            this.SMO = SMO;
        }
        public string SMO { get; set; }
        public string MO { get; set; }
    }
    public class СrossingHeadRow
    {
        public СrossingHeadRow(CrossingRow row)
        {
            YEAR = row.YEAR;
            MONTH = row.MONTH;
            CODE_MO = row.CODE_MO;
            NAM_MOK = row.NAM_MOK;
        }

        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string CODE_MO { get; set; }
        public string NAM_MOK { get; set; }

        private int HashCode => $"{CODE_MO}{YEAR}{MONTH}".GetHashCode();

        public override int GetHashCode()
        {
            return HashCode;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as СrossingHeadRow);
        }

        public bool Equals(СrossingHeadRow obj)
        {
            return obj != null && obj.HashCode == this.HashCode;
        }

    }
    public class CrossingRow
    {
        public static List<CrossingRow> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static CrossingRow Get(DataRow row)
        {
            try
            {
                var item = new CrossingRow();
                item.YEAR = Convert.ToInt32(row["YEAR"]);
                item.MONTH = Convert.ToInt32(row["MONTH"]);
                item.SLUCH_Z_ID = Convert.ToInt64(row["SLUCH_Z_ID"]);
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.NAM_MOK = Convert.ToString(row["NAM_MOK"]);
                item.POLIS = Convert.ToString(row["POLIS"]);
                item.FAM = Convert.ToString(row["FAM"]);
                item.IM = Convert.ToString(row["IM"]);
                item.OT = Convert.ToString(row["OT"]);
                item.DR = Convert.ToDateTime(row["DR"]);
                item.IDCASE = Convert.ToInt64(row["IDCASE"]);
                item.NHISTORY = Convert.ToString(row["NHISTORY"]);
                item.USL_OK = Convert.ToInt32(row["USL_OK"]);
                item.USL_OK_NAME = Convert.ToString(row["USL_OK_NAME"]);
                item.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                item.DATE_2 = Convert.ToDateTime(row["DATE_2"]);
                item.CROS_CODE_MO = Convert.ToString(row["CROS_CODE_MO"]);
                item.CROS_NAM_MOK = Convert.ToString(row["CROS_NAM_MOK"]);
                item.CROS_NHISTORY = Convert.ToString(row["CROS_NHISTORY"]);
                item.CROS_USL_OK = Convert.ToInt32(row["CROS_USL_OK"]);
                item.CROS_USL_OK_NAME = Convert.ToString(row["CROS_USL_OK_NAME"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения СrossingRow: {ex.Message}", ex);
            }
        }

        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public long SLUCH_Z_ID { get; set; }
        public string CODE_MO { get; set; }
        public string NAM_MOK { get; set; }
        public string POLIS { get; set; }
        public string FAM { get; set; }
        public string IM { get; set; }
        public string OT { get; set; }
        public DateTime DR { get; set; }
        public long IDCASE { get; set; }
        public string NHISTORY { get; set; }
        public int USL_OK { get; set; }
        public string USL_OK_NAME { get; set; }
        public DateTime DATE_1 { get; set; }
        public DateTime DATE_2 { get; set; }
        public string CROS_CODE_MO { get; set; }
        public string CROS_NAM_MOK { get; set; }
        public string CROS_NHISTORY { get; set; }
        public int CROS_USL_OK { get; set; }
        public string CROS_USL_OK_NAME { get; set; }
    }

  

    public interface IMEKRepository
    {
        List<MO_ITEM> GetMO_ITEM(int YEAR, int MONTH);
        MO_FOND_INFO FindFOND_INFO(MO_ITEM item);
        List<MP_VOLUME_ITEM> FindVOLUME(MO_ITEM item);
        List<MP_DEFECT_ITEM> FindDEFECT(MO_ITEM item);
        Dictionary<MO_SMO, decimal> FindNAPR_FROM_MO(int YEAR, int MONTH);
        Dictionary<СrossingHeadRow, List<CrossingRow>> GetV_Сrossing(int YEAR, int MONTH);
    }

    public class MEKRepository: IMEKRepository
    {
        private string ConnectionString;
        public MEKRepository(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        public List<MO_ITEM> GetMO_ITEM(int YEAR, int MONTH)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from table(ACT_MEK.ACT_MEK_ZGLV({YEAR},{MONTH}))", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return MO_ITEM.Get(tbl.Select());
                }
            }
        }
        public MO_FOND_INFO FindFOND_INFO(MO_ITEM item)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from table(ACT_MEK.ACT_MEK_FOND_INFO({item.YEAR},{item.MONTH},'{item.CODE_MO}',{item.SMO}))", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    if (tbl.Rows.Count > 1)
                        throw new Exception($"Ошибка в таблице FOND_INFO для {item.CODE_MO}+{item.SMO} на {item.MONTH}.{item.YEAR}");
                    return tbl.Rows.Count == 1 ? MO_FOND_INFO.Get(tbl.Rows[0]) : new MO_FOND_INFO();
                }
            }
        }
        public List<MP_VOLUME_ITEM> FindVOLUME(MO_ITEM item)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from table(ACT_MEK.ACT_MEK_VOLUME({item.YEAR},{item.MONTH},'{item.CODE_MO}','{item.SMO}', intArray({string.Join(",", item.ZGLV_ID_BASE)})))", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return MP_VOLUME_ITEM.Get(tbl.Select());
                }
            }
        }
        public List<MP_DEFECT_ITEM> FindDEFECT(MO_ITEM item)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from table(ACT_MEK.ACT_MEK_DEFECT({item.YEAR},{item.MONTH},'{item.CODE_MO}','{item.SMO}', intArray({string.Join(",", item.ZGLV_ID_BASE)})))", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return MP_DEFECT_ITEM.Get(tbl.Select());
                }
            }
        }
        public Dictionary<MO_SMO, decimal> FindNAPR_FROM_MO(int YEAR, int MONTH)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from table(ACT_MEK.ACT_MEK_NAPR_FROM_MO({YEAR},{MONTH}))", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl.Rows.Cast<DataRow>().ToDictionary(row => new MO_SMO(Convert.ToString(row["CODE_MO"]), Convert.ToString(row["SMO"])), row => Convert.ToDecimal(row["SUMP"]));
                }
            }
        }

        public Dictionary<СrossingHeadRow, List<CrossingRow>> GetV_Сrossing(int YEAR, int MONTH)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($"select * from table(ACT_MEK.crossing({YEAR},{MONTH}))", conn))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    var dic = new Dictionary<СrossingHeadRow, List<CrossingRow>>();
                    var list = CrossingRow.Get(tbl.Select());
                    foreach (var row in list)
                    {
                        var h = new СrossingHeadRow(row);
                        if (!dic.ContainsKey(h))
                            dic.Add(h, new List<CrossingRow>());
                        dic[h].Add(row);
                    }
                    return dic;
                }
            }

          
        }
    }
}

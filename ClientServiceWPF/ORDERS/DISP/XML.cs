using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ClientServiceWPF.ORDERS.ORD104;

namespace ClientServiceWPF.ORDERS.DISP
{
    [XmlRoot]
    public class ZL_LIST
    {
        [XmlElement] public ZGLV ZGLV { get; set; } = new ZGLV();
        [XmlElement(ElementName = "ZAP")] 
        public List<ZAP> ZAP { get; set; } = new List<ZAP>();
    }
    [XmlRoot]
    public class ZGLV
    {
        [XmlElement(DataType = "date")]
        public DateTime DATA { get; set; }
        [XmlElement]
        public string  FILENAME { get; set; }
    }


    [XmlRoot]
    public class ZAP
    {
        [XmlElement]
        public int N_ZAP { get; set; }
        [XmlElement]
        public PERS PERS { get; set; }
        [XmlElement]
        public COV19 COV19 { get; set; }
        [XmlElement]
        public Z_SL Z_SL { get; set; }
        [XmlElement] 
        public List<DISPN> DISPN { get; set; } = new List<DISPN>();
        [XmlElement]
        public List<SL> SL { get; set; } = new List<SL>();
        [XmlElement]
        public DISP DISP { get; set; }
        [XmlElement]
        public DISP DISP21 { get; set; }

    }

    [XmlRoot]
    public class PERS
    {

        public static PERS Get(SqlDataReader reader)
        {
            try
            {
                var item = new PERS();
                if (reader["FAM"] != DBNull.Value)
                    item.FAM = Convert.ToString(reader["FAM"]);
                if (reader["IM"] != DBNull.Value)
                    item.IM = Convert.ToString(reader["IM"]);
                if (reader["OT"] != DBNull.Value)
                    item.OT = Convert.ToString(reader["OT"]);
                if (reader["DOCTYPE"] != DBNull.Value)
                    item.DOCTYPE = Convert.ToString(reader["DOCTYPE"]);
                if (reader["DOCS"] != DBNull.Value)
                    item.DOCSER = Convert.ToString(reader["DOCS"]);
                if (reader["DOCN"] != DBNull.Value)
                    item.DOCNUM = Convert.ToString(reader["DOCN"]);

                if (reader["SNILS"] != DBNull.Value)
                    item.SNILS = Convert.ToString(reader["SNILS"]);
                if (reader["VPOLIS"] != DBNull.Value)
                    item.VPOLIS = Convert.ToInt32(reader["VPOLIS"]);
                if (reader["ENP"] != DBNull.Value)
                    item.ENP = Convert.ToString(reader["ENP"]);
                if (reader["NPOLIS"] != DBNull.Value)
                    item.NPOLIS = Convert.ToString(reader["NPOLIS"]);
                if (reader["SPOLIS"] != DBNull.Value)
                    item.SPOLIS = Convert.ToString(reader["SPOLIS"]);

                if (reader["PACTUAL"] != DBNull.Value)
                    item.PACTUAL = Convert.ToInt32(reader["PACTUAL"]);

                if (reader["REASON"] != DBNull.Value)
                    item.REASON = Convert.ToInt32(reader["REASON"]);

                if (reader["DSTOP"] != DBNull.Value)
                    item.DSTOP = Convert.ToDateTime(reader["DSTOP"]);
                if (reader["DS"] != DBNull.Value)
                    item.DDEATH = Convert.ToDateTime(reader["DS"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения PERS: {e.Message}", e);
            }

        }
        [XmlElement(IsNullable = false)]
        public string FAM { get; set; }
        [XmlElement(IsNullable = false)]
        public string IM { get; set; }
        [XmlElement(IsNullable = false)]
        public string OT { get; set; }
        [XmlElement(IsNullable = false)]
        public string DOCTYPE { get; set; }
        [XmlElement(IsNullable = false)]
        public string DOCSER { get; set; }
        [XmlElement(IsNullable = false)]
        public string DOCNUM { get; set; }
        [XmlElement(IsNullable = false)]
        public string SNILS { get; set; }
        [XmlElement(IsNullable = false)]
        public int VPOLIS { get; set; }
        [XmlElement(IsNullable = false)]
        public string ENP { get; set; }
        [XmlElement(IsNullable = false)]
        public string NPOLIS { get; set; }
        [XmlElement(IsNullable = false)]
        public string SPOLIS { get; set; }
        [XmlElement]
        public int PACTUAL { get; set; }
        [XmlElement]
        public int REASON { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime? DSTOP { get; set; }
        public bool ShouldSerializeDSTOP()
        {
            return DSTOP.HasValue;
        }
        [XmlElement(DataType = "date")]
        public DateTime? DDEATH { get; set; }
        public bool ShouldSerializeDDEATH()
        {
            return DDEATH.HasValue;
        }

    }
    [XmlRoot]
    public class COV19
    {
        public static COV19 Get(DataRow row)
        {
            try
            {
                var item = new COV19();
                if (row["ENP_REG"] != DBNull.Value)
                    item.ENP_REG = Convert.ToString(row["ENP_REG"]);
                if (row["USL_OK"] != DBNull.Value)
                    item.USL_OK = Convert.ToInt32(row["USL_OK"]);
                if (row["DSC"] != DBNull.Value)
                    item.DSC = Convert.ToString(row["DSC"]);
                if (row["DSCDATE"] != DBNull.Value)
                    item.DSCDATE = Convert.ToDateTime(row["DSCDATE"]);
                if (row["SEVERITY"] != DBNull.Value)
                    item.SEVERITY = Convert.ToInt32(row["SEVERITY"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения COV19: {e.Message}", e);
            }

        }

        [XmlIgnore]
        public  string ENP_REG { get; set; }
        [XmlIgnore]
        public int USL_OK { get; set; }

        [XmlElement]
        public string DSC { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DSCDATE { get; set; }
        [XmlElement]
        public int? SEVERITY { get; set; }
        public bool ShouldSerializeSEVERITY()
        {
            return SEVERITY.HasValue;
        }
    }

    [XmlRoot]
    public class Z_SL
    {
        public static Z_SL Get(DataRow row)
        {
            try
            {
                var item = new Z_SL();
                if (row["ENP_REG"] != DBNull.Value)
                    item.ENP_REG = Convert.ToString(row["ENP_REG"]);
                if (row["DATE_Z_1"] != DBNull.Value)
                    item.DATE_Z_1 = Convert.ToDateTime(row["DATE_Z_1"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения Z_SL: {e.Message}", e);
            }
        }
        [XmlIgnore]
        public string ENP_REG { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DATE_Z_1 { get; set; }
    }

    [XmlRoot]
    public class DISPN
    {
        public static DISPN Get(DataRow row)
        {
            try
            {
                var item = new DISPN();
                if (row["ENP_REG"] != DBNull.Value)
                    item.ENP_REG = Convert.ToString(row["ENP_REG"]);
                if (row["Is2021"] != DBNull.Value)
                    item.Is2021 = Convert.ToBoolean(row["Is2021"]);
                if (row["IsSL"] != DBNull.Value)
                    item.IsSL = Convert.ToBoolean(row["IsSL"]);
                if (row["DSG"] != DBNull.Value)
                    item.DSG = Convert.ToInt32(row["DSG"]);
                if (row["DS"] != DBNull.Value)
                    item.DS = Convert.ToString(row["DS"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения DISPN: {e.Message}", e);
            }

        }
        [XmlIgnore]
        public string ENP_REG { get; set; }
        [XmlIgnore]
        public bool Is2021 { get; set; }
        [XmlIgnore]
        public bool IsSL { get; set; }

        [XmlElement]
        public int DSG { get; set; }
        [XmlElement]
        public string DS { get; set; }
    }


    [XmlRoot]
    public class SL
    {
        [XmlElement]
        public int DSG { get; set; }
        [XmlElement]
        public string DS { get; set; }
    }

    [XmlRoot]
    public class DISP
    {
        public static DISP Get(DataRow row)
        {
            try
            {
                var item = new DISP();
                if (row["ENP_REG"] != DBNull.Value)
                    item.ENP_REG = Convert.ToString(row["ENP_REG"]);
                if (row["DATEDISP"] != DBNull.Value)
                    item.DATEDISP = Convert.ToDateTime(row["DATEDISP"]);
                if (row["DISPTYPE"] != DBNull.Value)
                    item.DISPTYPE = Convert.ToString(row["DISPTYPE"]);
                if (row["RES"] != DBNull.Value)
                    item.RES = Convert.ToInt32(row["RES"]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения DISP: {e.Message}", e);
            }

        }
        [XmlIgnore]
        public string ENP_REG { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DATEDISP { get; set; }
        [XmlElement]
        public string DISPTYPE { get; set; }
        [XmlElement]
        public int RES { get; set; }
    }

    public static class Ex
    {
        public static void WriteXml(this ZL_LIST zl, Stream st)
        {
            var ser = new XmlSerializer(typeof(ZL_LIST));
            var set = new XmlWriterSettings {Encoding = Encoding.GetEncoding("Windows-1251"), Indent = true};
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var xml = XmlWriter.Create(st, set))
            {
                ser.Serialize(xml, zl, ns);
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ClientServiceWPF.ORDERS.ORD23
{
    [XmlRoot("ISP_OB")]
    public class ISP_OB
    {
        [XmlElement("ZGLV", Form = XmlSchemaForm.Unqualified)]
        public ZGLV ZGLV { get; set; } = new ZGLV();
        [XmlElement("SVD", Form = XmlSchemaForm.Unqualified)]
        public SVD SVD { get; set; } = new SVD();
        public List<ZAP> PODR { get; set; } = new List<ZAP>();
    }
    public class ZGLV
    {
        public string VERSION { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATA { get; set; }
        public string FILENAME { get; set; }
        [XmlElement("FIRSTNAME", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string FIRSTNAME { get; set; }
    }
    public class SVD
    {
        public decimal CODE { get; set; }
        public decimal YEAR { get; set; }
        public decimal MONTH { get; set; }

    }
    public class PODR
    {
        public List<ZAP> ZAP { get; set; } = new List<ZAP>();
    }
    [XmlRoot(ElementName = "ZAP")]
    public class ZAP
    {
        public decimal N_ZAP { get; set; }
        public PACIENT PACIENT { get; set; } = new PACIENT();
        public SLUCH SLUCH { get; set; } = new SLUCH();
    }
    public class PACIENT
    {
        public static PACIENT Get(DataRow row)
        {
            try
            {
                PACIENT item = new PACIENT();
                if (row["DR"] != DBNull.Value)
                    item.DR = Convert.ToDateTime(row["DR"]);
                if (row["NPOLIS"] != DBNull.Value)
                    item.NPOLIS = Convert.ToString(row["NPOLIS"]);
                if (row["SPOLIS"] != DBNull.Value)
                    item.SPOLIS = Convert.ToString(row["SPOLIS"]);
                if (row["VPOLIS"] != DBNull.Value)
                    item.VPOLIS = Convert.ToDecimal(row["VPOLIS"]);
                if (row["W"] != DBNull.Value)
                    item.W = Convert.ToDecimal(row["W"]);
                if (row["VZST"] != DBNull.Value)
                    item.VZST = Convert.ToDecimal(row["VZST"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка получения PACIENT: {0}", ex.Message), ex);
            }
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? VPOLIS { get; set; }
        public bool ShouldSerializeVPOLIS()
        {
            return VPOLIS.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SPOLIS { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string NPOLIS { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? W { get; set; }
        public bool ShouldSerializeW()
        {
            return W.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date", IsNullable = true)]
        public DateTime? DR { get; set; }
        public bool ShouldSerializeDR()
        {
            return DR.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? VZST { get; set; }
        public bool ShouldSerializeVZST()
        {
            return VZST.HasValue;
        }



    }
    public class SLUCH
    {
        public static SLUCH Get(DataRow row, IEnumerable<DataRow> DS2, IEnumerable<DataRow> DS3, IEnumerable<DataRow> SL_K, IEnumerable<DataRow> USL, IEnumerable<DataRow> CRIT)
        {
            try
            {
                SLUCH item = new SLUCH();
                if (row["FOR_POM"] != DBNull.Value)
                    item.FOR_POM = Convert.ToDecimal(row["FOR_POM"]);
                if (row["IDCASE"] != DBNull.Value)
                    item.IDCASE = Convert.ToDecimal(row["IDCASE"]);
                if (row["LPU"] != DBNull.Value)
                    item.LPU = Convert.ToString(row["LPU"]);
                if (row["PODR"] != DBNull.Value)
                    item.PODR = Convert.ToString(row["PODR"]);
                if (row["DATE_1"] != DBNull.Value)
                    item.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                if (row["DATE_2"] != DBNull.Value)
                    item.DATE_2 = Convert.ToDateTime(row["DATE_2"]);

                if (row["DS1"] != DBNull.Value)
                    item.DS1 = Convert.ToString(row["DS1"]);

                foreach (var ds2 in DS2)
                {
                    item.DS2.Add(ds2["DS2"].ToString());
                }
                foreach (var ds3 in DS3)
                {
                    item.DS3.Add(ds3["DS3"].ToString());
                }

                if (row["RSLT"] != DBNull.Value)
                    item.RSLT = Convert.ToDecimal(row["RSLT"]);
                if (row["K_KSG"] != DBNull.Value)
                    item.K_KSG = Convert.ToString(row["K_KSG"]);
                if (row["KSG_PG"] != DBNull.Value)
                    item.KSG_PG = Convert.ToDecimal(row["KSG_PG"]);
                int x = 0;
                foreach (var c in CRIT)
                {
                    /*
                         item.DKK1 = Convert.ToString(c["CRIT"]);
                     if (x == 1)
                         item.DKK2 = Convert.ToString(c["CRIT"]);
                     x++;*/
                    item.CRIT.Add(Convert.ToString(c["CRIT"]));
                }

                if (row["K_FR"] != DBNull.Value)
                    item.K_FR = Convert.ToDecimal(row["K_FR"]);

                if (row["UR_K"] != DBNull.Value)
                    item.UR_K = Convert.ToDecimal(row["UR_K"]);

                if (row["SL_K"] != DBNull.Value)
                    item.SL_K = Convert.ToDecimal(row["SL_K"]);
                if (row["IT_SL"] != DBNull.Value)
                    item.IT_SL = Convert.ToDecimal(row["IT_SL"]);
                if (row["UR_K"] != DBNull.Value)
                    item.UR_K = Convert.ToDecimal(row["UR_K"]);


                foreach (var slk in SL_K)
                {
                    item.SL_KOEF.Add(SL_KOEFItem.Get(slk));
                }
                if (row["SUM_KSG"] != DBNull.Value)
                    item.SUM_KSG = Convert.ToDecimal(row["SUM_KSG"]);


                if (row["SUM_DIAL"] != DBNull.Value)
                    item.SUM_DIAL = Convert.ToDecimal(row["SUM_DIAL"]);
                if (row["SUM_IT"] != DBNull.Value)
                    item.SUM_IT = Convert.ToDecimal(row["SUM_IT"]);
                if (row["PVT"] != DBNull.Value)
                    item.PVT = Convert.ToDecimal(row["PVT"]);

                foreach (var us in USL)
                {
                    item.USL.Add(USLItem.Get(us));
                }

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка получения SLUCH: {0}", ex.Message), ex);
            }
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? IDCASE { get; set; }
        public bool ShouldSerializeIDCASE()
        {
            return IDCASE.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? FOR_POM { get; set; }
        public bool ShouldSerializeFOR_POM()
        {
            return FOR_POM.HasValue;
        }
        public string LPU { get; set; }
        public string PODR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_2 { get; set; }
        public string DS1 { get; set; }
        [XmlElement("DS2", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> DS2 { get; set; } = new List<string>();
        [XmlElement("DS3", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> DS3 { get; set; } = new List<string>();
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? RSLT { get; set; }
        public bool ShouldSerializeRSLT()
        {
            return RSLT.HasValue;
        }
        public string K_KSG { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? KSG_PG { get; set; }
        public bool ShouldSerializeKSG_PG()
        {
            return KSG_PG.HasValue;
        }
        //убрал для 2019
        /* [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
         public string DKK1 { get; set; }
         [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
         public string DKK2 { get; set; }
         */
        [XmlElement("CRIT", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> CRIT { get; set; } = new List<string>();

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? K_FR { get; set; }
        public bool ShouldSerializeK_FR()
        {
            return K_FR.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? UR_K { get; set; }
        public bool ShouldSerializeUR_K()
        {
            return UR_K.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SL_K { get; set; }
        public bool ShouldSerializeSL_K()
        {
            return SL_K.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? IT_SL { get; set; }
        public bool ShouldSerializeIT_SL()
        {
            return IT_SL.HasValue;
        }
        [XmlElement("SL_KOEF", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<SL_KOEFItem> SL_KOEF { get; set; } = new List<SL_KOEFItem>();

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUM_KSG { get; set; }
        public bool ShouldSerializeSUM_KSG()
        {
            return SUM_KSG.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUM_DIAL { get; set; }
        public bool ShouldSerializeSUM_DIAL()
        {
            return SUM_DIAL.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUM_IT { get; set; }
        public bool ShouldSerializeSUM_IT()
        {
            return SUM_IT.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PVT { get; set; }
        public bool ShouldSerializePVT()
        {
            return PVT.HasValue;
        }

        [XmlElement("USL", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<USLItem> USL { get; set; } = new List<USLItem>();
    }
    public class SL_KOEFItem
    {
        public decimal IDSL { get; set; }
        public decimal Z_SL { get; set; }

        public static SL_KOEFItem Get(DataRow row)
        {
            try
            {
                SL_KOEFItem item = new SL_KOEFItem();
                if (row["IDSL"] != DBNull.Value)
                    item.IDSL = Convert.ToDecimal(row["IDSL"]);
                if (row["Z_SL"] != DBNull.Value)
                    item.Z_SL = Convert.ToDecimal(row["Z_SL"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка получения SL_KOEFItem: {0}", ex.Message), ex);
            }
        }
    }
    public class USLItem
    {
        public static USLItem Get(DataRow row)
        {
            try
            {
                USLItem item = new USLItem();
                if (row["IDSERV"] != DBNull.Value)
                    item.IDSERV = Convert.ToString(row["IDSERV"]);
                if (row["CODE_USL"] != DBNull.Value)
                    item.CODE_USL = Convert.ToString(row["CODE_USL"]);
                if (row["KOL_USL"] != DBNull.Value)
                    item.KOL_USL = Convert.ToDecimal(row["KOL_USL"]);
                if (row["SUM_USL"] != DBNull.Value)
                    item.SUM_USL = Convert.ToDecimal(row["SUM_USL"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Ошибка получения USLItem: {0}", ex.Message), ex);
            }
        }

        public string IDSERV { get; set; }
        public string CODE_USL { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? KOL_USL { get; set; }
        public bool ShouldSerializeKOL_USL()
        {
            return KOL_USL.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUM_USL { get; set; }
        public bool ShouldSerializeSUM_USL()
        {
            return SUM_USL.HasValue;
        }
    }
    public static class Ex
    {

        public static void WriteXml(this ISP_OB zl, Stream st)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ISP_OB));
            XmlWriterSettings set = new XmlWriterSettings();

            set.Encoding = System.Text.Encoding.GetEncoding("Windows-1251");
            set.Indent = true;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriter xml = XmlWriter.Create(st, set);
            ser.Serialize(xml, zl, ns);
        }
        public static string FullMessage(this Exception ex)
        {
            string str = "";
            str = ex.Message;
            if (ex.InnerException != null)
                str += Environment.NewLine + ex.InnerException.FullMessage();
            return str;

        }

    }

}

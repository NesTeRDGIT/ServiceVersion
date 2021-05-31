using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ClientServiceWPF.ORDERS.ORD104
{
    public class MR_OB
    {
        [XmlElement("ZGLV", Form = XmlSchemaForm.Unqualified)]
        public ZGLV ZGLV { get; set; } = new ZGLV();
        [XmlElement("SVD", Form = XmlSchemaForm.Unqualified)]
        public SVD SVD { get; set; } = new SVD();
        public List<IT_SV> OB_SV { get; set; } = new List<IT_SV>();

        public List<ZAP> PODR { get; set; } = new List<ZAP>();
    }
    [XmlRoot("MR_OB")]
    public class ER_OB
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
    public class IT_SV
    {
        public decimal N_SV { get; set; }
        public decimal USL_OK { get; set; }
        public decimal? FOR_POM { get; set; }
        public bool ShouldSerializeFOR_POM()
        {
            return FOR_POM.HasValue;
        }
        [XmlElement("AP_TYPE", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string AP_TYPE { get; set; }
        [XmlElement("VZS_IT", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<VZS_IT> VZS_IT { get; set; } = new List<VZS_IT>();
    }
    public class VZS_IT
    {
        public decimal VZST { get; set; }
        public decimal OT_NAIM { get; set; }
        public decimal ZBL_IT { get; set; }
        public decimal SMR_IT { get; set; }

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

        public EKMP EKMP { get; set; } = null;
        public bool ShouldSerializeEKMP()
        {
            return EKMP != null;
        }

        public decimal? NO_EKMP { get; set; }

        public bool ShouldSerializeNO_EKMP()
        {
            return NO_EKMP.HasValue;
        }
    }
    public class EKMP
    {
        [XmlElement("PROBLEM", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> PROBLEM { get; set; } = new List<string>();
        public decimal TYPE { get; set; }

        public decimal? NO_PROBLEM { get; set; }
        public bool ShouldSerializeNO_PROBLEM()
        {
            return NO_PROBLEM.HasValue;
        }
    }
    public class PACIENT
    {
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DR { get; set; }
        [XmlIgnore]
        public decimal VZRS { get; set; }
    }
    public class SLUCH
    {
        [XmlIgnore]
        public int SLUCH_ID { get; set; }
        [XmlIgnore]
        public int SLUCH_Z_ID { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_1 { get; set; }
        public string DS1 { get; set; }
        public decimal RSLT { get; set; }
        public bool ShouldSerializeFOR_POM()
        {
            return FOR_POM.HasValue;
        }
        public decimal? FOR_POM { get; set; }
        [XmlElement("AP_TYPE", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string AP_TYPE { get; set; }
        [XmlIgnore]
        public string USL_OK { get; set; }
        [XmlIgnore]
        public bool isLETAL { get; set; }
        [XmlIgnore]
        public decimal OT_NAIM { get; set; }
    }
    public static class Ex
    {

        public static void WriteXml(this MR_OB zl, Stream st)
        {
            var ser = new XmlSerializer(typeof(MR_OB));
            var set = new XmlWriterSettings();

            set.Encoding = System.Text.Encoding.GetEncoding("Windows-1251");
            set.Indent = true;
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var xml = XmlWriter.Create(st, set);
            ser.Serialize(xml, zl, ns);
        }

        public static void WriteXml(this ER_OB zl, Stream st)
        {
            var ser = new XmlSerializer(typeof(ER_OB));
            var set = new XmlWriterSettings();

            set.Encoding = System.Text.Encoding.GetEncoding("Windows-1251");
            set.Indent = true;
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var xml = XmlWriter.Create(st, set);
            ser.Serialize(xml, zl, ns);
        }
        public static void setPROBLEM(this EKMP zl, string PROBLEM)
        {
            if (string.IsNullOrEmpty(PROBLEM) || PROBLEM == "0")
                zl.NO_PROBLEM = 1;
            else
            {
                foreach (var str in PROBLEM.Split(','))
                {
                    if (!string.IsNullOrEmpty(str.Trim()) && str.Trim() != "0")
                        zl.PROBLEM.Add(str.Trim());
                }
            }

        }
        public static string FullMessage(this Exception ex)
        {
            var str = "";
            str = ex.Message;
            if (ex.InnerException != null)
                str += Environment.NewLine + ex.InnerException.FullMessage();
            return str;

        }

    }
}

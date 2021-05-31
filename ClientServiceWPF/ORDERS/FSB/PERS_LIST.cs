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

namespace ClientServiceWPF.ORDERS.FSB
{

    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public class PERS_LIST
    {
        public PERS_LIST()
        {
            PERS = new List<PERS>();
            ZGLV = new ZGLV();
        }
        [XmlElement("ZGLV", Form = XmlSchemaForm.Unqualified)]
        public ZGLV ZGLV { get; set; }

        [XmlElement("PERS", Form = XmlSchemaForm.Unqualified)]
        public List<PERS> PERS { get; set; }
    }
    [Serializable]
    [XmlType(AnonymousType = true)]
    public class ZGLV
    {
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATA { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal YEAR { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal MONTH { get; set; }
    }


    [Serializable]
    public class PERS
    {
        public static PERS Get(DataRow row)
        {
            try
            {
                var item = new PERS();
                if (row["DOCNUM"] != DBNull.Value)
                    item.DOCNUM = row["DOCNUM"].ToString();
                if (row["DOCSER"] != DBNull.Value)
                    item.DOCSER = row["DOCSER"].ToString();
                if (row["DOCTYPE"] != DBNull.Value)
                    item.DOCTYPE = row["DOCTYPE"].ToString();
                if (row["DR"] != DBNull.Value)
                    item.DR = Convert.ToDateTime(row["DR"]);
                if (row["FAM"] != DBNull.Value)
                    item.FAM = row["FAM"].ToString();
                if (row["IM"] != DBNull.Value)
                    item.IM = row["IM"].ToString();
                if (row["MR"] != DBNull.Value)
                    item.MR = row["MR"].ToString();
                if (row["OKATOG"] != DBNull.Value)
                    item.OKATOG = row["OKATOG"].ToString();
                if (row["OKATOP"] != DBNull.Value)
                    item.OKATOP = row["OKATOP"].ToString();
                if (row["OT"] != DBNull.Value)
                    item.OT = row["OT"].ToString();
                if (row["SNILS"] != DBNull.Value)
                    item.SNILS = row["SNILS"].ToString();
                if (row["TEL"] != DBNull.Value)
                    item.TEL = row["TEL"].ToString();
                if (row["W"] != DBNull.Value)
                    item.W = Convert.ToDecimal(row["W"]);
                if (row["CODE_MO"] != DBNull.Value)
                    item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения PERS:{ex.Message}");
            }
        }

        public PERS()
        {

        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string FAM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string IM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string OT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal W { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DR { get; set; }

        [XmlElement("TEL", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string TEL { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string MR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DOCTYPE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DOCSER { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DOCNUM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SNILS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string OKATOG { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string OKATOP { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string CODE_MO { get; set; }

    }



    public static class ExtZLLIST
    {
        public static void WriteXml(this PERS_LIST zl, Stream st)
        {
            var ser = new XmlSerializer(typeof(PERS_LIST));
            var set = new XmlWriterSettings {Encoding = Encoding.GetEncoding("Windows-1251"), Indent = true};
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            var xml = XmlWriter.Create(st, set);
            ser.Serialize(xml, zl, ns);
        }


    }
}

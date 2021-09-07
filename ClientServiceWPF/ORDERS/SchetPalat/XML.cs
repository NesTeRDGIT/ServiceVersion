using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;


namespace ClientServiceWPF.ORDERS.SchetPalat
{
    [XmlRoot]
    public class OutputData
    {
        [XmlElement(ElementName = "ZAP")]
        public List<ZAP> ZAP { get; set; } = new List<ZAP>();
    }
    [XmlRoot]
    public class ZAP
    {
        [XmlElement] public string ID_PAC { get; set; } = "";
        [XmlElement] public string KOD_TF { get; set; } = "";
        [XmlElement] public string IDPOL { get; set; } = "";
        [XmlElement] public string GR { get; set; } = "";
        [XmlElement] public string USL_OK { get; set; } = "";
        [XmlElement] public string VIDPOM { get; set; } = "";
        [XmlElement] public string FOR_POM { get; set; } = "";
        [XmlElement] public string LPU { get; set; } = "";
        [XmlElement] public string LPU_PAC { get; set; } = "";
        [XmlElement] public string RSLT { get; set; } = "";
        [XmlElement] public string ISHOD { get; set; } = "";
        [XmlElement] public string PROFIL { get; set; } = "";
        [XmlElement] public string IDSP { get; set; } = "";
        [XmlElement] public string DS0 { get; set; } = "";
        [XmlElement] public string DS1 { get; set; }

        [XmlElement]
        public string DS2
        {
            get => string.Join(";", DS2_LIST);
            set => DS2_LIST.AddRange(value.Split(';'));
        }




        [XmlElement] public string DS3
        {
            get => string.Join(";", DS3_LIST);
            set => DS3_LIST.AddRange(value.Split(';'));
        }
        [XmlElement] public string C_ZAB { get; set; } = "";
        [XmlElement] public string DN { get; set; } = "";
        [XmlElement(DataType = "date")] 
        public DateTime DATE_1 { get; set; }
        [XmlElement(DataType = "date")]
        public DateTime DATE_2 { get; set; }
        [XmlElement] public string DS_ONK { get; set; } = "";
        [XmlElement] public string RSLT_D { get; set; } = "";
        [XmlElement] public string PR_DS { get; set; } = "";

        [XmlIgnore]
        public string ENP_REG { get; set; } = "";
        [XmlIgnore]
        public long SLUCH_ID { get; set; }

        [XmlIgnore] public List<string> DS2_LIST { get; set; } = new List<string>();
        [XmlIgnore] public List<string> DS3_LIST { get; set; } = new List<string>();

        public static ZAP Get(DbDataReader row)
        {
            try
            {
                var z = new ZAP();
                z.ID_PAC = row["ID_PAC"].ToString();
                z.KOD_TF = row["KOD_TF"].ToString();
                z.IDPOL = row["IDPOL"].ToString();
                z.GR = row["GR"].ToString();
                z.USL_OK = row["USL_OK"].ToString();
                z.VIDPOM = row["VIDPOM"].ToString();
                z.FOR_POM = row["FOR_POM"].ToString();
                z.LPU = row["LPU"].ToString();
                z.LPU_PAC = row["LPU_PAC"].ToString(); 
                z.RSLT = row["RSLT"].ToString();
                z.ISHOD = row["ISHOD"].ToString();
                z.PROFIL = row["PROFIL"].ToString();
                z.IDSP = row["IDSP"].ToString();

                z.DS0 = row["DS0"].ToString();
                z.DS1 = row["DS1"].ToString();
             

                z.C_ZAB = row["C_ZAB"].ToString();

                z.DN = row["DN"].ToString();
                z.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                z.DATE_2 = Convert.ToDateTime(row["DATE_2"]); 
                z.DS_ONK = row["DS_ONK"].ToString();
                z.RSLT_D = row["RSLT_D"].ToString();
                z.PR_DS = row["PR_DS"].ToString();
                z.ENP_REG = row["ENP_REG"].ToString();
                z.SLUCH_ID = Convert.ToInt64(row["SLUCH_ID"]);
                return z;

            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения ZAP: {ex.Message}", ex);
            }
        }

    }


    public class MapZap
    {
        public string ENP { get; set; }
        public string ID_PAC { get; set; }
        public long SLUCH_ID { get; set; }
    }


    public static class Ex
    {
        public static void WriteXml(this OutputData zl, Stream st)
        {
            var ser = new XmlSerializer(typeof(OutputData));
            var set = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var xml = XmlWriter.Create(st, set))
            {
                ser.Serialize(xml, zl, ns);
            }
        }

        public static void WriteXml(this List<MapZap> zl, Stream st)
        {
            var ser = new XmlSerializer(typeof(List<MapZap>));
            var set = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (var xml = XmlWriter.Create(st, set))
            {
                ser.Serialize(xml, zl, ns);
            }
        }

    }
}

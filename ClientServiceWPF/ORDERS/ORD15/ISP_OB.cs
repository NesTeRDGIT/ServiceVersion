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

namespace ClientServiceWPF.ORDERS.ORD15
{
    public class ISP_OB
    {
        [XmlElement("ZGLV", Form = XmlSchemaForm.Unqualified)]
        public ZGLV ZGLV { get; set; } = new ZGLV();
        [XmlElement("SVD", Form = XmlSchemaForm.Unqualified)]
        public SVD SVD { get; set; } = new SVD();
        [XmlElement("OB_SV", Form = XmlSchemaForm.Unqualified)]
        public List<OB_SV> OB_SV { get; set; } = new List<OB_SV>();
        [XmlElement("ZAP", Form = XmlSchemaForm.Unqualified)]
        public List<ZAP> ZAP { get; set; } = new List<ZAP>();

        public void GetOBSV()
        {
            int N_SV = 1;
            var t = ZAP.Select(x => x.SLUCH).GroupBy(x => x.LPU);
            foreach (var g in t)
            {
                var ob_sv = new OB_SV() { N_SV = N_SV, MO_SV = g.Key, IT_SV = new List<IT_SV>() };
                OB_SV.Add(ob_sv);
                foreach (var g1 in g.GroupBy(x => x.OT_NAIM))
                {
                    IT_SV it_sv = new IT_SV() { OT_NAIM = g1.Key, PR_SV = new List<PR_SV>() };
                    ob_sv.IT_SV.Add(it_sv);

                    foreach (var g11 in g1.GroupBy(x => x.PROFIL))
                    {
                        PR_SV pr_sv = new PR_SV() { PROFIL_MP = g11.Key };
                        it_sv.PR_SV.Add(pr_sv);
                        pr_sv.R_KOL = g11.Where(x => !x.ISMTR).Count();
                        pr_sv.R_S_KOL = g11.Where(x => !x.ISMTR).Sum(x => x.SUM);

                        pr_sv.R_KOL_M = g11.Where(x => x.ISMTR).Count();
                        pr_sv.R_S_KOL_M = g11.Where(x => x.ISMTR).Sum(x => x.SUM);

                    }
                }
                N_SV++;
            }
            ///суммируем 5 = 5+7
            foreach (var o in OB_SV)
            {
                var itsv5 = o.IT_SV.Where(x => x.OT_NAIM == 5).FirstOrDefault();
                var itsv7 = o.IT_SV.Where(x => x.OT_NAIM == 7).FirstOrDefault();
                if (itsv7 == null && itsv7 == null) continue;
                if (itsv5 == null)
                {
                    itsv5 = new IT_SV() { OT_NAIM = 5 };
                    o.IT_SV.Add(itsv5);
                }

                foreach (var pr7 in itsv7.PR_SV)
                {
                    var pr5 = itsv5.PR_SV.Where(x => x.PROFIL_MP == pr7.PROFIL_MP).FirstOrDefault();
                    if (pr5 == null)
                    {
                        pr5 = new PR_SV() { PROFIL_MP = pr7.PROFIL_MP };
                        itsv5.PR_SV.Add(pr5);
                    }
                    pr5.R_KOL += pr7.R_KOL;
                    pr5.R_KOL_M += pr7.R_KOL_M;
                    pr5.R_S_KOL += pr7.R_S_KOL;
                    pr5.R_S_KOL_M += pr7.R_S_KOL_M;
                }
            }
            ///суммируем 9 = 9+11
            foreach (var o in OB_SV)
            {
                var itsv5 = o.IT_SV.Where(x => x.OT_NAIM == 9).FirstOrDefault();
                var itsv7 = o.IT_SV.Where(x => x.OT_NAIM == 11).FirstOrDefault();
                if (itsv7 == null && itsv7 == null) continue;
                if (itsv5 == null)
                {
                    itsv5 = new IT_SV() { OT_NAIM = 9 };
                    o.IT_SV.Add(itsv5);
                }

                foreach (var pr7 in itsv7.PR_SV)
                {
                    var pr5 = itsv5.PR_SV.Where(x => x.PROFIL_MP == pr7.PROFIL_MP).FirstOrDefault();
                    if (pr5 == null)
                    {
                        pr5 = new PR_SV() { PROFIL_MP = pr7.PROFIL_MP };
                        itsv5.PR_SV.Add(pr5);
                    }
                    pr5.R_KOL += pr7.R_KOL;
                    pr5.R_KOL_M += pr7.R_KOL_M;
                    pr5.R_S_KOL += pr7.R_S_KOL;
                    pr5.R_S_KOL_M += pr7.R_S_KOL_M;
                }
            }
        }

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

        public decimal OBLM { get; set; }
    }
    public class OB_SV
    {
        [XmlElement(IsNullable = true)]
        public decimal? N_SV { get; set; }
        public bool ShouldSerializeN_SV()
        {
            return N_SV.HasValue;
        }
        [XmlElement(IsNullable = false)]
        public string MO_SV { get; set; }
        [XmlElement("IT_SV", Form = XmlSchemaForm.Unqualified)]
        public List<IT_SV> IT_SV { get; set; } = new List<IT_SV>();
    }
    public class IT_SV
    {
        [XmlElement(IsNullable = true)]
        public decimal? OT_NAIM { get; set; }
        public bool ShouldSerializeOT_NAIM()
        {
            return OT_NAIM.HasValue;
        }
        [XmlElement("PR_SV", Form = XmlSchemaForm.Unqualified)]
        public List<PR_SV> PR_SV { get; set; } = new List<PR_SV>();

    }
    public class PR_SV
    {
        public decimal PROFIL_MP { get; set; }
        public decimal R_KOL { get; set; }
        public decimal R_S_KOL { get; set; }
        public decimal R_KOL_M { get; set; }
        public decimal R_S_KOL_M { get; set; }

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
                if (row["W"] != DBNull.Value)
                    item.W = Convert.ToDecimal(row["W"]);
                if (row["SMO_OK"] != DBNull.Value)
                    item.SMO_OK = Convert.ToString(row["SMO_OK"]);
                if (row["VZST"] != DBNull.Value)
                    item.VZST = Convert.ToDecimal(row["VZST"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения PACIENT: " + ex.Message);
            }
        }
        public string SMO_OK { get; set; }
        public decimal? W { get; set; }
        public bool ShouldSerializeW()
        {
            return W.HasValue;
        }

        public decimal? VZST { get; set; }
        public bool ShouldSerializeVZST()
        {
            return VZST.HasValue;
        }
    }
    public class SLUCH
    {
        public static SLUCH Get(DataRow row)
        {
            try
            {
                SLUCH item = new SLUCH();
                if (row["DATE_I"] != DBNull.Value)
                    item.DATE_I = Convert.ToDecimal(row["DATE_I"]);
                if (row["FOR_POM"] != DBNull.Value)
                    item.FOR_POM = Convert.ToDecimal(row["FOR_POM"]);
                if (row["IDCASE"] != DBNull.Value)
                    item.IDCASE = Convert.ToString(row["IDCASE"]);

                if (row["ISMTR"] != DBNull.Value)
                    item.ISMTR = Convert.ToBoolean(row["ISMTR"]);
                if (row["LPU"] != DBNull.Value)
                    item.LPU = Convert.ToString(row["LPU"]);
                if (row["METOD_HMP"] != DBNull.Value)
                    item.METOD_HMP = Convert.ToString(row["METOD_HMP"]);
                if (row["OT_NAIM"] != DBNull.Value)
                    item.OT_NAIM = Convert.ToDecimal(row["OT_NAIM"]);
                if (row["PCEL"] != DBNull.Value)
                    item.PCEL = Convert.ToDecimal(row["PCEL"]);
                if (row["PROFIL"] != DBNull.Value)
                    item.PROFIL = Convert.ToDecimal(row["PROFIL"]);
                if (row["SUM"] != DBNull.Value)
                    item.SUM = Convert.ToDecimal(row["SUM"]);
                if (row["USL_OK"] != DBNull.Value)
                    item.USL_OK = Convert.ToDecimal(row["USL_OK"]);
                if (row["VIDPOM"] != DBNull.Value)
                    item.VIDPOM = Convert.ToDecimal(row["VIDPOM"]);
                if (row["VID_HMP"] != DBNull.Value)
                    item.VID_HMP = Convert.ToString(row["VID_HMP"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения SLUCH: " + ex.Message);
            }
        }



        [XmlElement(IsNullable = false)]
        public string IDCASE { get; set; }
        public decimal? USL_OK { get; set; }
        public bool ShouldSerializeUSL_OK()
        {
            return USL_OK.HasValue;
        }

        public decimal? VIDPOM { get; set; }
        public bool ShouldSerializeVIDPOM()
        {
            return VIDPOM.HasValue;
        }
        public bool ShouldSerializeFOR_POM()
        {
            return FOR_POM.HasValue;
        }
        public decimal? FOR_POM { get; set; }
        public decimal? PCEL { get; set; }
        public bool ShouldSerializePCEL()
        {
            return PCEL.HasValue;
        }

        [XmlElement(IsNullable = false)]
        public string VID_HMP { get; set; }
        [XmlElement(IsNullable = false)]
        public string METOD_HMP { get; set; }

        [XmlElement(IsNullable = false)]
        public string LPU { get; set; }


        public decimal PROFIL { get; set; }


        public decimal? DATE_I { get; set; }
        public bool ShouldSerializeDATE_1()
        {
            return DATE_I.HasValue;
        }
        public decimal SUM { get; set; }


        [XmlIgnore]
        public decimal? OT_NAIM { get; set; }
        [XmlIgnore]
        public bool ISMTR { get; set; }


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

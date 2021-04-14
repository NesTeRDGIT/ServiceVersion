using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Globalization;

namespace ServiceLoaderMedpomData.EntityMP_V31
{

    [Serializable]     
    [XmlRootAttribute(Namespace = "", IsNullable = false)]
    public  class ZL_LIST
    {

        public void SetSUMP()
        {
            
            foreach(var zs in ZAP.SelectMany(x=>x.Z_SL_list))
            {
                if (zs.OPLATA == 1)
                {
                    foreach(var sl in zs.SL)
                    {
                        if(!sl.SUM_MP.HasValue)
                            sl.SUM_MP = sl.SUM_M;
                        foreach(var us in sl.USL)
                        {
                            if (!us.SUMP_USL.HasValue)
                                us.SUMP_USL = us.SUMV_USL;
                        }
                    }
                }

                if (zs.OPLATA == 2)
                {
                    foreach (var sl in zs.SL)
                    {
                        if (!sl.SUM_MP.HasValue)
                            sl.SUM_MP = 0;
                        foreach (var us in sl.USL)
                        {
                            if (!us.SUMP_USL.HasValue)
                                us.SUMP_USL = 0;
                        }
                    }
                }
            }
        }

        public void ClearSMO_DATE()
        {
            foreach (var z in ZAP)
            {
                z.PACIENT.LPU_REG = null;

                foreach (var z_sl in z.Z_SL_list)
                {
                    z_sl.SLUCH_Z_ID = null;
                    foreach (var san in z_sl.SANK)
                    {
                        san.S_OSN_TFOMS = null;
                    }
                    foreach (var sl in z_sl.SL)
                    {
                        sl.SUM_MP = null;
                        foreach (var us in sl.USL)
                        {
                            us.PS_VOLUME = null;
                            us.BSP = null;
                            us.NOT_VR = null;
                            us.SUMP_USL = null;
                        }
                    }
                }
            }
        }

        public static ZL_LIST GetZL_LIST(VersionMP vers, string filepach)
        {
            ZL_LIST zl = null;
            switch (vers)
            {
                case VersionMP.V2_1: zl = new ZL_LIST(new ServiceLoaderMedpomData.EntityMP_V3.ZL_LIST(ServiceLoaderMedpomData.EntityMP_V2.ZL_LIST.ReadFromFile(filepach))); break;
                case VersionMP.V3_0: zl = new ZL_LIST(ServiceLoaderMedpomData.EntityMP_V3.ZL_LIST.ReadFromFile(filepach)); break;
                case VersionMP.V3_1: zl = ZL_LIST.ReadFromFile(filepach); break;
                default:
                    break;
            }
            return zl;
        }
        public static ZL_LIST ReadFromFile(string Path)
        {
            Stream st = null;
            try
            {
                XmlSerializer ser = null;
                ser = new XmlSerializer(typeof(ZL_LIST));
                st = System.IO.File.OpenRead(Path);
                var zl = (ZL_LIST)ser.Deserialize(st);
               
                return zl;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                st?.Dispose();
            }
        }
    

        public ZL_LIST()
        {
            this.ZAP = new List<ZAP>();
            this.SCHET = new SCHET();
            this.ZGLV = new ZGLV();
        }

        public ZL_LIST(EntityMP_V3.ZL_LIST zl2)
        {
            this.ZAP = new List<ZAP>();
            this.SCHET = new SCHET();
            this.ZGLV = new ZGLV();

            ZGLV.VERSION = zl2.ZGLV.VERSION;
            ZGLV.DATA = zl2.ZGLV.DATA;
            ZGLV.FILENAME = zl2.ZGLV.FILENAME;
            ZGLV.SD_Z = zl2.ZGLV.SD_Z;
            ZGLV.ZGLV_ID = zl2.ZGLV.ZGLV_ID;

            SCHET.CODE = zl2.SCHET.CODE;
            SCHET.CODE_MO = zl2.SCHET.CODE_MO;
            SCHET.COMENTS = zl2.SCHET.COMENTS;
            SCHET.DISP = zl2.SCHET.DISP;
            SCHET.DSCHET = zl2.SCHET.DSCHET;
            SCHET.MONTH = zl2.SCHET.MONTH;
            SCHET.NSCHET = zl2.SCHET.NSCHET;
            SCHET.PLAT = zl2.SCHET.PLAT;
            SCHET.SANK_EKMP = zl2.SCHET.SANK_EKMP;
            SCHET.SANK_MEE = zl2.SCHET.SANK_MEE;
            SCHET.SANK_MEK = zl2.SCHET.SANK_MEK;
            SCHET.SUMMAP = zl2.SCHET.SUMMAP;
            SCHET.SUMMAV = zl2.SCHET.SUMMAV;
            SCHET.YEAR = zl2.SCHET.YEAR;
            SCHET.DOP_FLAG = zl2.SCHET.DOP_FLAG;
            SCHET.SCHET_ID = zl2.SCHET.SCHET_ID;
            SCHET.ZGLV_ID = zl2.SCHET.ZGLV_ID;


            foreach (EntityMP_V3.ZAP z2 in zl2.ZAP)
            {
                ZAP z = new ZAP();
                ZAP.Add(z);

                z.N_ZAP = z2.N_ZAP;
                z.PR_NOV = z2.PR_NOV;
                z.PACIENT = new PACIENT();
                z.PACIENT.ID_PAC = z2.PACIENT.ID_PAC;
                z.PACIENT.MSE = z2.PACIENT.MSE;
                z.PACIENT.NOVOR = z2.PACIENT.NOVOR;
                z.PACIENT.NPOLIS = z2.PACIENT.NPOLIS;
                z.PACIENT.SMO = z2.PACIENT.SMO;
                z.PACIENT.SMO_NAM = z2.PACIENT.SMO_NAM;
                z.PACIENT.SMO_OGRN = z2.PACIENT.SMO_OGRN;
                z.PACIENT.SMO_OK = z2.PACIENT.SMO_OK;
                z.PACIENT.SPOLIS = z2.PACIENT.SPOLIS;
                z.PACIENT.ST_OKATO = z2.PACIENT.ST_OKATO;
                z.PACIENT.VNOV_D = z2.PACIENT.VNOV_D;
                z.PACIENT.VPOLIS = z2.PACIENT.VPOLIS;
                z.PACIENT.INV = z2.PACIENT.INV;
                z.PACIENT.PACIENT_ID = z2.PACIENT.PACIENT_ID;
                z.PACIENT.ZAP_ID = z2.PACIENT.ZAP_ID;
                z.PACIENT.SMO_TFOMS = z2.PACIENT.SMO_TFOMS;


                foreach (var z_sl_2 in z2.Z_SL_list)
                {
                    var Z_SL = new Z_SL();
                    z.Z_SL_list.Add(Z_SL);
                    Z_SL.DATE_Z_1 = z_sl_2.DATE_Z_1;
                   Z_SL.DATE_Z_2 = z_sl_2.DATE_Z_2;
                   Z_SL.FOR_POM = z_sl_2.FOR_POM;
                    Z_SL.IDCASE = z_sl_2.IDCASE;
                    Z_SL.IDSP = z_sl_2.IDSP;
                    Z_SL.ISHOD = z_sl_2.ISHOD;
                    Z_SL.KD_Z = z_sl_2.KD_Z;
                    Z_SL.LPU = z_sl_2.LPU;
                    Z_SL.NPR_DATE = z_sl_2.NPR_DATE;
                    Z_SL.NPR_MO = z_sl_2.NPR_MO;
                    Z_SL.OPLATA = z_sl_2.OPLATA;
                    Z_SL.OS_SLUCH = z_sl_2.OS_SLUCH;
                    Z_SL.PACIENT_ID = z_sl_2.PACIENT_ID;
                    Z_SL.P_OTK = z_sl_2.P_OTK;
                    Z_SL.RSLT = z_sl_2.RSLT;
                    Z_SL.RSLT_D = z_sl_2.RSLT_D;
                    Z_SL.SANK_IT = z_sl_2.SANK_IT;
                    Z_SL.SLUCH_Z_ID = z_sl_2.SLUCH_Z_ID;
                    Z_SL.SUMP = z_sl_2.SUMP;
                    Z_SL.SUMV = z_sl_2.SUMV;
                    Z_SL.USL_OK = z_sl_2.USL_OK;
                    Z_SL.VBR = z_sl_2.VBR;
                    Z_SL.VB_P = z_sl_2.VB_P;
                    Z_SL.VIDPOM = z_sl_2.VIDPOM;
                    Z_SL.VNOV_M = z_sl_2.VNOV_M;
                    Z_SL.ZAP_ID = z_sl_2.ZAP_ID;

                    foreach (EntityMP_V3.SL sl2 in z_sl_2.SL)
                    {
                        var SL = new SL();
                        Z_SL.SL.Add(SL);
                        SL.CODE_MES1 = sl2.CODE_MES1;
                        SL.CODE_MES2 = sl2.CODE_MES2;
                        SL.COMENTSL = sl2.COMENTSL;
                        SL.DATE_1 = sl2.DATE_1;
                        SL.DATE_2 = sl2.DATE_2;
                        SL.DET = sl2.DET;
                        SL.DN = sl2.DN;
                        SL.DS0 = sl2.DS0;
                        SL.DS1 = sl2.DS1;
                        SL.DS1_PR = sl2.DS1_PR;
                        SL.DS2 = sl2.DS2;
                        SL.DS3 = sl2.DS3;
                        SL.DS_ONK = sl2.DS_ONK;
                        SL.ED_COL = sl2.ED_COL;
                        SL.EXTR = sl2.EXTR;
                        SL.IDDOKT = sl2.IDDOKT;
                        SL.KD = sl2.KD;
                        SL.LPU_1 = sl2.LPU_1;
                        SL.METOD_HMP = sl2.METOD_HMP;
                        SL.NHISTORY = sl2.NHISTORY;
                        SL.PACIENT_ID = sl2.PACIENT_ID;
                        SL.PODR = sl2.PODR;
                        SL.PROFIL = sl2.PROFIL;
                        SL.PROFIL_K = sl2.PROFIL_K;
                        SL.PRVS = sl2.PRVS;
                        SL.PR_D_N = sl2.PR_D_N;
                        SL.P_CEL = sl2.P_CEL;
                        SL.P_PER = sl2.P_PER;
                        SL.REAB = sl2.REAB;
                        SL.SLUCH_ID = sl2.SLUCH_ID;
                        SL.SLUCH_Z_ID = sl2.SLUCH_Z_ID;
                        SL.SL_ID = sl2.SL_ID.ToString();
                        SL.SUM_M = sl2.SUM_M;
                        SL.SUM_MP = sl2.SUM_MP;
                        SL.TAL_D = sl2.TAL_D;
                        SL.TAL_NUM = sl2.TAL_NUM;
                        SL.TAL_P = sl2.TAL_P;
                        SL.TARIF = sl2.TARIF;
                        SL.VERS_SPEC = sl2.VERS_SPEC;
                        SL.VID_HMP = sl2.VID_HMP;


                        if (sl2.KSG_KPG != null)
                        {
                            SL.KSG_KPG = new KSG_KPG();
                            SL.KSG_KPG.BZTSZ = sl2.KSG_KPG.BZTSZ;
                            if (sl2.KSG_KPG.DKK1 != "" && sl2.KSG_KPG.DKK1 != null)
                                SL.KSG_KPG.CRIT.Add(sl2.KSG_KPG.DKK1);
                            if (sl2.KSG_KPG.DKK2 != "" && sl2.KSG_KPG.DKK2 != null)
                                SL.KSG_KPG.CRIT.Add(sl2.KSG_KPG.DKK2);

                            SL.KSG_KPG.IT_SL = sl2.KSG_KPG.IT_SL;
                            SL.KSG_KPG.KOEF_D = sl2.KSG_KPG.KOEF_D;
                            SL.KSG_KPG.KOEF_U = sl2.KSG_KPG.KOEF_U;
                            SL.KSG_KPG.KOEF_UP = sl2.KSG_KPG.KOEF_UP;
                            SL.KSG_KPG.KOEF_Z = sl2.KSG_KPG.KOEF_Z;
                            SL.KSG_KPG.KSG_PG = sl2.KSG_KPG.KSG_PG;
                            SL.KSG_KPG.N_KSG = sl2.KSG_KPG.N_KSG;
                            SL.KSG_KPG.SL_K = sl2.KSG_KPG.SL_K;
                            foreach (var k in sl2.KSG_KPG.SL_KOEF)
                            {
                                SL.KSG_KPG.SL_KOEF.Add(new SL_KOEF() { IDSL = k.IDSL, SLUCH_ID = k.SLUCH_ID, Z_SL = k.Z_SL });
                            }
                            SL.KSG_KPG.VER_KSG = sl2.KSG_KPG.VER_KSG;
                        }
                        foreach (var ds2_n in sl2.DS2_N)
                        {
                            SL.DS2_N.Add(new DS2_N() { DS2 = ds2_n.DS2, DS2_PR = ds2_n.DS2_PR, PR_DS2_N = ds2_n.PR_DS2_N, SLUCH_ID = ds2_n.SLUCH_ID });
                        }
                        var PR_CONS = sl2.USL.Where(x => x.ONK_USL != null).Where(x => x.ONK_USL.PR_CONS.HasValue);
                        foreach (var cons in PR_CONS)
                        {
                            SL.CONS.Add(new CONS() { PR_CONS = cons.ONK_USL.PR_CONS, IDSERV = cons.IDSERV });
                        }
                        foreach (var usl_nap in sl2.USL.Where(x => x.NAPR.Count != 0))
                        {
                            foreach (var nap in usl_nap.NAPR)
                                SL.NAPR.Add(new NAPR() { MET_ISSL = nap.MET_ISSL, NAPR_DATE = nap.NAPR_DATE, NAPR_USL = nap.NAPR_USL, NAPR_V = nap.NAPR_V, SLUCH_ID = SL.SLUCH_ID, IDSERV = usl_nap.IDSERV });
                        }
                        foreach (var naz in sl2.NAZ)
                        {
                            SL.NAZ.Add(new NAZR() { NAZ_N = naz.NAZ_N, NAZ_PK = naz.NAZ_PK, NAZ_PMP = naz.NAZ_PMP, NAZ_R = naz.NAZ_R, NAZ_SP = naz.NAZ_SP, NAZ_V = naz.NAZ_V });
                        }

                        if (sl2.ONK_SL != null)
                        {
                            SL.ONK_SL = new ONK_SL();
                            SL.ONK_SL.SOD = sl2.ONK_SL.SOD;
                            SL.ONK_SL.STAD = sl2.ONK_SL.STAD;
                            SL.ONK_SL.SLUCH_ID = sl2.ONK_SL.SLUCH_ID;
                            foreach (var onk_us in sl2.USL.Where(x => x.ONK_USL != null))
                            {
                                SL.ONK_SL.ONK_USL.Add(new ONK_USL()
                                {
                                    HIR_TIP = onk_us.ONK_USL.HIR_TIP,
                                    LEK_TIP_L = onk_us.ONK_USL.LEK_TIP_L,
                                    LEK_TIP_V = onk_us.ONK_USL.LEK_TIP_L,
                                    LUCH_TIP = onk_us.ONK_USL.LUCH_TIP,
                                    USL_TIP = onk_us.ONK_USL.USL_TIP,
                                    IDSERV = onk_us.IDSERV
                                });
                            }
                            SL.ONK_SL.ONK_T = sl2.ONK_SL.ONK_T;
                            SL.ONK_SL.ONK_N = sl2.ONK_SL.ONK_N;
                            SL.ONK_SL.ONK_M = sl2.ONK_SL.ONK_M;
                            SL.ONK_SL.MTSTZ = sl2.ONK_SL.MTSTZ;
                            SL.ONK_SL.DS1_T = sl2.ONK_SL.DS1_T;

                            foreach (var d in sl2.ONK_SL.B_DIAG)
                            {
                                SL.ONK_SL.B_DIAG.Add(new B_DIAG() { DIAG_CODE = d.DIAG_CODE, DIAG_RSLT = d.DIAG_RSLT, DIAG_TIP = d.DIAG_TIP, SLUCH_ID = d.SLUCH_ID });
                            }

                            foreach (var d in sl2.ONK_SL.B_PROT)
                            {
                                SL.ONK_SL.B_PROT.Add(new B_PROT() { D_PROT = d.D_PROT, PROT = d.PROT });
                            }


                        }


                        foreach (var san2 in sl2.SANK)
                        {
                            san2.SL_ID.Clear();
                            san2.SL_ID.Add(sl2.SL_ID.ToString());
                            Z_SL.SANK.Add(san2);
                        }

                        foreach (var us2 in sl2.USL)
                        {
                            USL us = new USL();
                            us.CODE_MD = us2.CODE_MD;
                            us.CODE_USL = us2.CODE_USL;
                            us.COMENTU = us2.COMENTU;
                            us.DATE_IN = us2.DATE_IN;
                            us.DATE_OUT = us2.DATE_OUT;
                            us.DET = us2.DET;
                            us.DS = us2.DS;
                            us.IDSERV = us2.IDSERV;
                            us.KOL_USL = us2.KOL_USL;
                            us.LPU = us2.LPU;
                            us.LPU_1 = us2.LPU_1;
                            us.PODR = us2.PODR;
                            us.PROFIL = us2.PROFIL;
                            us.PRVS = us2.PRVS;
                            us.SUMV_USL = us2.SUMV_USL;
                            us.SUMP_USL = us2.SUMP_USL;
                            us.TARIF = us2.TARIF;
                            us.P_OTK = us2.P_OTK;
                            us.NPL = us2.NPL;
                            us.USL_ID = us.USL_ID;
                            us.VID_VME = us2.VID_VME;
                            SL.USL.Add(us);
                        }
                    }
                }
            }


        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public ZGLV ZGLV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public SCHET SCHET { get; set; }

        [XmlElement("ZAP", Form = XmlSchemaForm.Unqualified)]
        public List<ZAP> ZAP { get; set; }
        public void SetID(decimal ZGLV_ID, decimal SCHET_ID, decimal ZAP_ID, decimal PACIENT_ID, decimal SLUCH_Z_ID, decimal SLUCH_ID, decimal USL_ID, decimal SANK_ID, decimal ONK_USL_ID, decimal LEK_PR_ID)
        {
            ZGLV.ZGLV_ID = ZGLV_ID;
            SCHET.SCHET_ID = SCHET_ID;
            SCHET.ZGLV_ID = ZGLV_ID;

            foreach (ZAP z in ZAP)
            {
                z.SCHET_ID = SCHET_ID;
                z.ZAP_ID = ZAP_ID;
                z.PACIENT.ZAP_ID = ZAP_ID;
                z.PACIENT.PACIENT_ID = PACIENT_ID;

               

                foreach (var z_sl in z.Z_SL_list)
                {
                 
                    z_sl.ZAP_ID = ZAP_ID;
                    z_sl.SLUCH_Z_ID = SLUCH_Z_ID;
                    z_sl.PACIENT_ID = PACIENT_ID;



                    foreach (SANK san in z_sl.SANK)
                    {
                        san.SLUCH_Z_ID = SLUCH_Z_ID;
                        san.SANK_ID = SANK_ID;
                        foreach (var ce in san.CODE_EXP)
                        {
                            ce.SANK_ID = san.SANK_ID;
                        }
                        SANK_ID++;
                    }


                    foreach (SL sl in z_sl.SL)
                    {
                        sl.PACIENT_ID = PACIENT_ID;
                        sl.SLUCH_Z_ID = SLUCH_Z_ID;
                        sl.SLUCH_ID = SLUCH_ID;


                        foreach (var ds2_n in sl.DS2_N)
                        {
                            ds2_n.SLUCH_ID = SLUCH_ID;
                        }



                        if (sl.KSG_KPG != null)
                        {
                            foreach (SL_KOEF sl_k in sl.KSG_KPG.SL_KOEF)
                            {
                                sl_k.SLUCH_ID = SLUCH_ID;
                            }
                        }


                        foreach (CONS con in sl.CONS)
                        {
                            con.SLUCH_ID = SLUCH_ID;
                        }




                        foreach (USL u in sl.USL)
                        {
                            u.USL_ID = USL_ID;
                            u.SLUCH_ID = SLUCH_ID;
                            USL_ID++;
                        }

                        if (sl.ONK_SL != null)
                        {
                            sl.ONK_SL.SLUCH_ID = sl.SLUCH_ID;

                            foreach (B_DIAG bd in sl.ONK_SL.B_DIAG)
                            {
                                bd.SLUCH_ID = SLUCH_ID;

                            }
                            foreach (B_PROT bd in sl.ONK_SL.B_PROT)
                            {
                                bd.SLUCH_ID = SLUCH_ID;

                            }
                            foreach (var u in sl.ONK_SL.ONK_USL)
                            {
                                u.SLUCH_ID = SLUCH_ID;

                                var uu = sl.USL.Where(x => x.IDSERV == u.IDSERV).FirstOrDefault();
                                if (uu != null)
                                {
                                    u.USL_ID = uu.USL_ID;
                                }

                                u.ONK_USL_ID = ONK_USL_ID;
                                ONK_USL_ID++;
                                foreach (var lek_pr in u.LEK_PR)
                                {
                                    lek_pr.ONK_USL_ID = u.ONK_USL_ID;
                                    lek_pr.LEK_PR_ID = LEK_PR_ID;
                                    LEK_PR_ID++;
                                    foreach (var t in lek_pr.DATE_INJ)
                                    {
                                        t.LEK_PR_ID = lek_pr.LEK_PR_ID;
                                    }
                                }

                            }

                        }

                        foreach (NAPR npr in sl.NAPR)
                        {
                            npr.SLUCH_ID = SLUCH_ID;
                            var uu = sl.USL.Where(x => x.IDSERV == npr.IDSERV).FirstOrDefault();
                            if (uu != null)
                            {
                                npr.USL_ID = uu.USL_ID;
                            }
                        }


                        foreach (NAZR N in sl.NAZ)
                        {
                            N.SLUCH_ID = SLUCH_ID;
                        }
                        if (sl.KSG_KPG != null)
                        {
                            foreach (SL_KOEF k in sl.KSG_KPG.SL_KOEF)
                            {
                                k.SLUCH_ID = SLUCH_ID;
                            }
                        }


                        SLUCH_ID++;
                    }
                    SLUCH_Z_ID++;
                }
                PACIENT_ID++;
                ZAP_ID++;

            }
        }

        public Dictionary<decimal, Z_SL> GetHashTable()
        {
            Dictionary<decimal, Z_SL> table = new Dictionary<decimal, Z_SL>();
           
            foreach (ZAP z in ZAP)
            {
                foreach(var sl in z.Z_SL_list)
                {
                    table.Add(sl.IDCASE, sl);
                }
                
            }
            return table;
        }

    }


    [Serializable]   
    public partial class ZGLV
    {    

        public static ZGLV Get(DataRow row)
        {
            try
            {
                ZGLV item = new ZGLV();
                
                item.DATA = Convert.ToDateTime(row["DATA"]);
                item.FILENAME = row["FILENAME"].ToString();
                item.SD_Z =Convert.ToDecimal(row["SD_Z"]);
                item.VERSION = row["VERSION"].ToString();
                return item;
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка получения ZGLV: " + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? ZGLV_ID { get; set; }

        [XmlIgnore]
        public decimal Vers
        {
            get
            {
                return Convert.ToDecimal(VERSION, CultureInfo.InvariantCulture);
            }
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string VERSION { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATA { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string FILENAME { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SD_Z { get; set; }
    }

    
    [Serializable]   
    public partial class SCHET
    {
        public static SCHET Get(DataRow row)
        {
            try
            {
                SCHET item = new SCHET();
                item.CODE = Convert.ToDecimal(row["CODE"]);
                item.CODE_MO = row["CODE_MO"].ToString();
                if (row["COMENTS"] != DBNull.Value)
                    item.COMENTS = row["COMENTS"].ToString();
                if (row["DISP"] != DBNull.Value)
                    item.DISP = row["DISP"].ToString();
                if (row["DOP_FLAG"] != DBNull.Value)
                    item.DOP_FLAG = Convert.ToInt32(row["DOP_FLAG"]);

                item.DSCHET = Convert.ToDateTime(row["DSCHET"]);

                item.MONTH = Convert.ToDecimal(row["MONTH"]);
                item.NSCHET = row["NSCHET"].ToString();
                item.PLAT = row["PLAT"].ToString();
                if (row["SANK_EKMP"] != DBNull.Value)
                    item.SANK_EKMP = Convert.ToDecimal(row["SANK_EKMP"]);
                if (row["SANK_MEE"] != DBNull.Value)
                    item.SANK_MEE = Convert.ToDecimal(row["SANK_MEE"]);
                if (row["SANK_MEK"] != DBNull.Value)
                    item.SANK_MEK = Convert.ToDecimal(row["SANK_MEK"]);
                if (row["SCHET_ID"] != DBNull.Value)
                    item.SCHET_ID = Convert.ToDecimal(row["SCHET_ID"]);
                if (row["SUMMAP"] != DBNull.Value)
                    item.SUMMAP = Convert.ToDecimal(row["SUMMAP"]);
                if (row["SUMMAV"] != DBNull.Value)
                    item.SUMMAV = Convert.ToDecimal(row["SUMMAV"]);

                item.YEAR = Convert.ToDecimal(row["YEAR"]);
                if (row["ZGLV_ID"] != DBNull.Value)
                    item.ZGLV_ID = Convert.ToDecimal(row["ZGLV_ID"]);
                return item;
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка получения SCHET:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SCHET_ID { get; set; }
        [XmlIgnore]
        public int? DOP_FLAG { get; set; }
        [XmlIgnore]
        public decimal? YEAR_BASE { get; set; }
        [XmlIgnore]
        public decimal? MONTH_BASE { get; set; }

        [XmlIgnore]
        public decimal? ZGLV_ID { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal CODE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string CODE_MO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal YEAR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal MONTH { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NSCHET { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DSCHET { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public string PLAT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SUMMAV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public string COMENTS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUMMAP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SANK_MEK { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SANK_MEE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SANK_EKMP { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DISP { get; set; }

        #region Пустые элементы
        public bool ShouldSerializeSANK_EKMP()
        {
            return SANK_EKMP.HasValue;
        }
        public bool ShouldSerializeCOMENTS()
        {
            return COMENTS != null;
        }
        public bool ShouldSerializeSUMMAP()
        {
            return SUMMAP.HasValue;
        }
        public bool ShouldSerializeSANK_MEK()
        {
            return SANK_MEK.HasValue;
        }
        public bool ShouldSerializeSANK_MEE()
        {
            return SANK_MEE.HasValue;
        }
        #endregion
    }

    
    [Serializable]
    public partial class ZAP
    {
        public static ZAP Get(DataRow row)
        {
            try
            {
                ZAP item = new ZAP();
                item.N_ZAP = Convert.ToDecimal(row["N_ZAP"]);

                if (row["SCHET_ID"] != DBNull.Value)
                    item.SCHET_ID = Convert.ToDecimal(row["SCHET_ID"]);
                if (row["ZAP_ID"] != DBNull.Value)
                    item.ZAP_ID = Convert.ToDecimal(row["ZAP_ID"]);
                if (row["PR_NOV"] != DBNull.Value)
                    item.PR_NOV = Convert.ToDecimal(row["PR_NOV"]);
                
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения ZAP:" + ex.Message);
            }
        }

        [XmlIgnore]
        public decimal? ZAP_ID { get; set; }
        [XmlIgnore]
        public decimal? SCHET_ID { get; set; }

        public ZAP()
        {
            this.Z_SL_list = new List<Z_SL>();
            this.PACIENT = new PACIENT();
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal N_ZAP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal PR_NOV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public PACIENT PACIENT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public Z_SL Z_SL
        {
            get
            {
                return Z_SL_list[0];
            }
            set
            {
                if (Z_SL_list.Count == 0) Z_SL_list.Add(new Z_SL());
                Z_SL_list[0] = value;
            }
        }
        [XmlIgnore]
        public List<Z_SL> Z_SL_list { get; set; }
    }

    
    [Serializable]
    public partial class PACIENT
    {
        public static PACIENT Get(DataRow row)
        {
            try
            {
                PACIENT item = new PACIENT();
                if (row["ID_PAC"] != DBNull.Value)
                    item.ID_PAC = row["ID_PAC"].ToString();
                if (row["INV"] != DBNull.Value)
                    item.INV = Convert.ToDecimal(row["INV"]);
                if (row["MSE"] != DBNull.Value)
                    item.MSE = Convert.ToDecimal(row["MSE"]);
                if (row["NOVOR"] != DBNull.Value)
                    item.NOVOR = row["NOVOR"].ToString();
                if (row["NPOLIS"] != DBNull.Value)
                    item.NPOLIS = row["NPOLIS"].ToString();
                if (row["PACIENT_ID"] != DBNull.Value)
                    item.PACIENT_ID = Convert.ToDecimal(row["PACIENT_ID"]);
                if (row["SMO"] != DBNull.Value)
                    item.SMO = row["SMO"].ToString();
                if (row["SMO_NAM"] != DBNull.Value)
                    item.SMO_NAM = row["SMO_NAM"].ToString();
                if (row["SMO_OGRN"] != DBNull.Value)
                    item.SMO_OGRN = row["SMO_OGRN"].ToString();
                if (row["SMO_OK"] != DBNull.Value)
                    item.SMO_OK = row["SMO_OK"].ToString();
                if (row["SMO_TFOMS"] != DBNull.Value)
                    item.SMO_TFOMS = row["SMO_TFOMS"].ToString();
                if (row["SPOLIS"] != DBNull.Value)
                    item.SPOLIS = row["SPOLIS"].ToString();
                if (row["ST_OKATO"] != DBNull.Value)
                    item.ST_OKATO = row["ST_OKATO"].ToString();
                if (row["VNOV_D"] != DBNull.Value)
                    item.VNOV_D = Convert.ToDecimal(row["VNOV_D"]);
                if (row["VPOLIS"] != DBNull.Value)
                    item.VPOLIS = Convert.ToDecimal(row["VPOLIS"]);
                if (row["ZAP_ID"] != DBNull.Value)
                    item.ZAP_ID = Convert.ToDecimal(row["ZAP_ID"]);
                if (row["LPU_REG"] != DBNull.Value)
                    item.LPU_REG = Convert.ToString(row["LPU_REG"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения PACIENT:" + ex.Message);
            }
        }
        [XmlIgnore]
        public string SMO_TFOMS { get; set; }
        [XmlIgnore]
        public decimal? PACIENT_ID { get; set; }
        [XmlIgnore]
        public decimal? PERS_ID { get; set; }
        [XmlIgnore]
        public decimal? ZAP_ID { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string ID_PAC { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal VPOLIS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SPOLIS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NPOLIS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string ST_OKATO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SMO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SMO_OGRN { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SMO_OK { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SMO_NAM { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? INV { get; set; }
        public bool ShouldSerializeINV()
        {
            return INV.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? MSE { get; set; }
        public bool ShouldSerializeMSE()
        {
            return MSE.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NOVOR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? VNOV_D { get; set; }
        public bool ShouldSerializeVNOV_D()
        {
            return VNOV_D.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string LPU_REG { get; set; }
    }

    
    [Serializable]
    public  class Z_SL
    {
        public static Z_SL Get(DataRow row)
        {
            try
            {
                Z_SL item = new Z_SL();
                if(row["DATE_Z_1"] !=DBNull.Value)
                    item.DATE_Z_1 = Convert.ToDateTime(row["DATE_Z_1"]);
                if (row["DATE_Z_2"] != DBNull.Value)
                    item.DATE_Z_2 = Convert.ToDateTime(row["DATE_Z_2"]);
                if (row["FOR_POM"] != DBNull.Value)
                    item.FOR_POM = Convert.ToDecimal(row["FOR_POM"]);
                if (row["IDCASE"] != DBNull.Value)
                    item.IDCASE = Convert.ToDecimal(row["IDCASE"]);
                if (row["IDSP"] != DBNull.Value)
                    item.IDSP = Convert.ToDecimal(row["IDSP"]);
                if (row["ISHOD"] != DBNull.Value)
                    item.ISHOD = Convert.ToDecimal(row["ISHOD"]);
                if (row["KD_Z"] != DBNull.Value)
                    item.KD_Z = Convert.ToDecimal(row["KD_Z"]);
                if (row["LPU"] != DBNull.Value)
                    item.LPU = row["LPU"].ToString();
                if (row["NPR_DATE"] != DBNull.Value)
                    item.NPR_DATE = Convert.ToDateTime(row["NPR_DATE"]);
                if (row["NPR_MO"] != DBNull.Value)
                    item.NPR_MO = row["NPR_MO"].ToString();
                if (row["OPLATA"] != DBNull.Value)
                    item.OPLATA = Convert.ToDecimal(row["OPLATA"]);
                if (row["OS_SLUCH"] != DBNull.Value)
                    item.setOS_SLUCH(row["OS_SLUCH"].ToString());
                if (row["PACIENT_ID"] != DBNull.Value)
                    item.PACIENT_ID = Convert.ToDecimal(row["PACIENT_ID"]);
                if (row["P_OTK"] != DBNull.Value)
                    item.P_OTK = Convert.ToDecimal(row["P_OTK"]);
                if (row["RSLT"] != DBNull.Value)
                    item.RSLT = Convert.ToDecimal(row["RSLT"]);
                if (row["RSLT_D"] != DBNull.Value)
                    item.RSLT_D = Convert.ToDecimal(row["RSLT_D"]);
                if (row["SANK_IT"] != DBNull.Value)
                    item.SANK_IT = Convert.ToDecimal(row["SANK_IT"]);
                if (row["SLUCH_Z_ID"] != DBNull.Value)
                    item.SLUCH_Z_ID = Convert.ToDecimal(row["SLUCH_Z_ID"]);
                if (row["SUMP"] != DBNull.Value)
                    item.SUMP = Convert.ToDecimal(row["SUMP"]);
                if (row["SUMV"] != DBNull.Value)
                    item.SUMV = Convert.ToDecimal(row["SUMV"]);
                if (row["USL_OK"] != DBNull.Value)
                    item.USL_OK = Convert.ToDecimal(row["USL_OK"]);
                if (row["VBR"] != DBNull.Value)
                    item.VBR = Convert.ToDecimal(row["VBR"]);
                if (row["VB_P"] != DBNull.Value)
                    item.VB_P = Convert.ToDecimal(row["VB_P"]);
                if (row["VIDPOM"] != DBNull.Value)
                    item.VIDPOM = Convert.ToDecimal(row["VIDPOM"]);
                if (row["VNOV_M"] != DBNull.Value)
                    item.setVNOV_M(row["VNOV_M"].ToString());
                if (row["ZAP_ID"] != DBNull.Value)
                    item.ZAP_ID = Convert.ToDecimal(row["ZAP_ID"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения Z_SL:" + ex.Message);
            }
        }
   

        [XmlIgnore]
        public string TagComment { get; set; } = "";
        [XmlIgnore]
        public decimal? ZAP_ID { get; set; }
      
        public Z_SL()
        {
            this.SL = new List<SL>();
            this.OS_SLUCH = new List<string>();
            this.VNOV_M = new List<string>();
        }



        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal IDCASE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? USL_OK { get; set; }
        public bool ShouldSerializeUSL_OK()
        {
            return USL_OK.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal VIDPOM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal? FOR_POM { get; set; }
        public bool ShouldSerializeFOR_POM()
        {
            return FOR_POM.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string NPR_MO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date", IsNullable = true)]
        public DateTime? NPR_DATE { get; set; }
        public bool ShouldSerializeNPR_DATE()
        {
            return NPR_DATE.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LPU { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? VBR { get; set; }
        public bool ShouldSerializeVBR()
        {
            return VBR.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_Z_1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_Z_2 { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? P_OTK { get; set; }
        public bool ShouldSerializeP_OTK()
        {
            return P_OTK.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? RSLT_D { get; set; }
        public bool ShouldSerializeRSLT_D()
        {
            return RSLT_D.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? KD_Z { get; set; }
        public bool ShouldSerializeKD_Z()
        {
            return KD_Z.HasValue;
        }

        [XmlElement("VNOV_M", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<string> VNOV_M { get; set; }
        public bool ShouldSerializeVNOV_M()
        {
            return VNOV_M.Count != 0;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? RSLT { get; set; }
        public bool ShouldSerializeRSLT()
        {
            return RSLT.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? ISHOD { get; set; }

        public bool ShouldSerializeISHOD()
        {
            return ISHOD.HasValue;
        }

        [XmlElement("OS_SLUCH", Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public List<string> OS_SLUCH { get; set; }


        public bool ShouldSerializeOS_SLUCH()
        {
            return OS_SLUCH.Count != 0;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? VB_P { get; set; }
        public bool ShouldSerializeVB_P()
        {
            return VB_P.HasValue;
        }

        [XmlElement("SL", Form = XmlSchemaForm.Unqualified)]
        public List<SL> SL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal IDSP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SUMV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? OPLATA { get; set; }
        public bool ShouldSerializeOPLATA()
        {
            return OPLATA.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUMP { get; set; }
        public bool ShouldSerializeSUMP()
        {
            return SUMP.HasValue;
        }

        [XmlElement("SANK", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<SANK> SANK { get; set; } = new List<SANK>();



        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SANK_IT { get; set; }
        public bool ShouldSerializeSANK_IT()
        {
            return SANK_IT.HasValue;
        }

        [XmlIgnore]
        public decimal? PACIENT_ID { get;  set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SLUCH_Z_ID { get; set; }
        public bool ShouldSerializeSLUCH_Z_ID()
        {
            return SLUCH_Z_ID.HasValue;
        }

    }


    public class DS_SLUCH_ID
    {
        public string DS { get; set; }
        public decimal? SLUCH_ID { get; set; }
    }

    public class CRIT_SLUCH_ID
    {
        public decimal? ORD { get; set; }
        public string CRIT { get; set; }
        public decimal? SLUCH_ID { get; set; }
    }

    [Serializable]
    public  class SL
    {
        public static SL Get(DataRow row, IEnumerable<DataRow> DS2tbl, IEnumerable<DataRow> DS3tbl, IEnumerable<DataRow> CRIT)
        {
            try
            {
                SL item = new SL();
                if (row["CODE_MES1"] != DBNull.Value)
                    item.setCODE_MES1(row["CODE_MES1"].ToString());
                if (row["CODE_MES2"] != DBNull.Value)
                    item.CODE_MES2 =row["CODE_MES2"].ToString();
                if (row["COMENTSL"] != DBNull.Value)
                    item.COMENTSL = row["COMENTSL"].ToString();
                if (row["DATE_1"] != DBNull.Value)
                    item.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                if (row["DATE_2"] != DBNull.Value)
                    item.DATE_2 = Convert.ToDateTime(row["DATE_2"]);
                if (row["DET"] != DBNull.Value)
                    item.DET = Convert.ToDecimal(row["DET"]);
                if (row["DN"] != DBNull.Value)
                    item.DN = Convert.ToDecimal(row["DN"]);
                if (row["DS0"] != DBNull.Value)
                    item.DS0 = row["DS0"].ToString();
                if (row["DS1"] != DBNull.Value)
                    item.DS1 = row["DS1"].ToString();
                if (row["DS1_PR"] != DBNull.Value)
                    item.DS1_PR =  Convert.ToDecimal(row["DS1_PR"]);
                      
                
             
                if (row["DS_ONK"] != DBNull.Value)
                    item.DS_ONK = Convert.ToDecimal(row["DS_ONK"]);
                if (row["ED_COL"] != DBNull.Value)
                    item.ED_COL = Convert.ToDecimal(row["ED_COL"]);
                if (row["EXTR"] != DBNull.Value)
                    item.EXTR = Convert.ToDecimal(row["EXTR"]);
                if (row["IDDOKT"] != DBNull.Value)
                    item.IDDOKT = row["IDDOKT"].ToString();
                if (row["KD"] != DBNull.Value)
                    item.KD = Convert.ToDecimal(row["KD"]);

                if (row["N_KSG"] != DBNull.Value)
                {
                    item.KSG_KPG = KSG_KPG.Get(row, CRIT);                    
                }
                    

                if (row["LPU_1"] != DBNull.Value)
                    item.LPU_1 = row["LPU_1"].ToString();
                if (row["METOD_HMP"] != DBNull.Value)
                    item.METOD_HMP = Convert.ToDecimal(row["METOD_HMP"]);
                
                if (row["NHISTORY"] != DBNull.Value)
                    item.NHISTORY = row["NHISTORY"].ToString();

                if (row["DS1_T"] != DBNull.Value)
                {
                    item.ONK_SL = ONK_SL.Get(row);
                }

                if (row["PACIENT_ID"] != DBNull.Value)
                    item.PACIENT_ID = Convert.ToDecimal(row["PACIENT_ID"]);
                if (row["PODR"] != DBNull.Value)
                    item.PODR = Convert.ToDecimal(row["PODR"]);
                if (row["PROFIL"] != DBNull.Value)
                    item.PROFIL = Convert.ToDecimal(row["PROFIL"]);
                if (row["PROFIL_K"] != DBNull.Value)
                    item.PROFIL_K = Convert.ToDecimal(row["PROFIL_K"]);
                if (row["PRVS"] != DBNull.Value)
                    item.PRVS = Convert.ToDecimal(row["PRVS"]);
                if (row["PR_D_N"] != DBNull.Value)
                    item.PR_D_N = Convert.ToDecimal(row["PR_D_N"]);
                if (row["P_CEL"] != DBNull.Value)
                    item.P_CEL = row["P_CEL"].ToString();
                if (row["P_PER"] != DBNull.Value)
                    item.P_PER = Convert.ToDecimal(row["P_PER"]);
                if (row["REAB"] != DBNull.Value)
                    item.REAB = Convert.ToDecimal(row["REAB"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                if (row["SLUCH_Z_ID"] != DBNull.Value)
                    item.SLUCH_Z_ID = Convert.ToDecimal(row["SLUCH_Z_ID"]);
                if (row["SL_ID"] != DBNull.Value)
                    item.SL_ID = Convert.ToString(row["SL_ID"]);
                if (row["SUM_M"] != DBNull.Value)
                    item.SUM_M = Convert.ToDecimal(row["SUM_M"]);
                if (row["SUM_MP"] != DBNull.Value)
                    item.SUM_MP = Convert.ToDecimal(row["SUM_MP"]);
                if (row["TAL_D"] != DBNull.Value)
                    item.TAL_D = Convert.ToDateTime(row["TAL_D"]);
                if (row["TAL_NUM"] != DBNull.Value)
                    item.TAL_NUM = row["TAL_NUM"].ToString();
                if (row["TAL_P"] != DBNull.Value)
                    item.TAL_P = Convert.ToDateTime(row["TAL_P"]);
                if (row["TARIF"] != DBNull.Value)
                    item.TARIF = Convert.ToDecimal(row["TARIF"]);
                if (row["VERS_SPEC"] != DBNull.Value)
                    item.VERS_SPEC = row["VERS_SPEC"].ToString();
                if (row["VID_HMP"] != DBNull.Value)
                    item.VID_HMP = row["VID_HMP"].ToString();
                if (row["C_ZAB"] != DBNull.Value)
                    item.C_ZAB = Convert.ToDecimal(row["C_ZAB"]);

                item.DS2.Clear();
                foreach(DataRow ds2 in DS2tbl)
                {
                    item.DS2.Add(ds2["DS2"].ToString());
                }
                item.DS3.Clear();
                foreach (DataRow ds3 in DS3tbl)
                {
                    item.DS3.Add(ds3["DS3"].ToString());
                }
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения SL:" + ex.Message);
            }
        }

        public List<DS_SLUCH_ID> GetDS2()
        {
            var rez = new List<DS_SLUCH_ID>();
            foreach (var str in this.DS2)
            {
                if (string.IsNullOrEmpty(str)) continue;
                rez.Add(new DS_SLUCH_ID { DS = str, SLUCH_ID = this.SLUCH_ID });
            }
            return rez;
        }
        public List<DS_SLUCH_ID> GetDS3()
        {
            var rez = new List<DS_SLUCH_ID>();
            foreach (var str in this.DS3)
            {
                if (string.IsNullOrEmpty(str)) continue;
                rez.Add(new DS_SLUCH_ID { DS = str, SLUCH_ID = this.SLUCH_ID });
            }
            return rez;
        }

        [XmlIgnore]
        public decimal? EXTR { get; set; }
        [XmlIgnore]
        public string TagComment { get; set; } = "";

        [XmlIgnore]
        public decimal? PACIENT_ID { get; set; }
  
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
        [XmlIgnore]
        public decimal? SLUCH_Z_ID { get; set; }
        public SL()
        {
            this.USL = new List<USL>();          
            DS2_N = new List<DS2_N>();
            NAZ = new List<NAZR>();
            this.DS2 = new List<string>();
            this.DS3 = new List<string>();
            this.CODE_MES1 = new List<string>();
            this.NAPR = new List<NAPR>();
            this.CONS = new List<CONS>();
            
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string SL_ID { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string VID_HMP { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? METOD_HMP { get; set; }
        public bool ShouldSerializeMETOD_HMP()
        {
            return METOD_HMP.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string LPU_1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PODR { get; set; }
        public bool ShouldSerializePODR()
        {
            return PODR.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal? PROFIL { get; set; }
        public bool ShouldSerializePROFIL()
        {
            return PROFIL.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PROFIL_K { get; set; }
        public bool ShouldSerializePROFIL_K()
        {
            return PROFIL_K.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? DET { get; set; }
        public bool ShouldSerializeDET()
        {
            return DET.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string P_CEL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true, DataType = "date")]
        public DateTime? TAL_D { get; set; }
        public bool ShouldSerializeTAL_D()
        {
            return TAL_D.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string TAL_NUM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true, DataType = "date")]
        public DateTime? TAL_P { get; set; }
        public bool ShouldSerializeTAL_P()
        {
            return TAL_P.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NHISTORY { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? P_PER { get; set; }
        public bool ShouldSerializeP_PER()
        {
            return P_PER.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_2 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? KD { get; set; }

        public bool ShouldSerializeKD()
        {
            return KD.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DS0 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string DS1 { get; set; }

        [XmlElement("DS2", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> DS2 { get; set; }

        [XmlElement("DS3", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> DS3 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? C_ZAB { get; set; }
        public bool ShouldSerializeC_ZAB()
        {
            return C_ZAB.HasValue;
        }


      
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? DS1_PR { get; set; }
        public bool ShouldSerializeDS1_PR()
        {
            return DS1_PR.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? DS_ONK { get; set; }

        public bool ShouldSerializeDS_ONK()
        {
            return DS_ONK.HasValue;
        }        

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PR_D_N { get; set; }
        public bool ShouldSerializePR_D_N()
        {
            return PR_D_N.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<DS2_N> DS2_N { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<NAZR> NAZ { get; set; }



        
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? DN { get; set; }
        public bool ShouldSerializeDN()
        {
            return DN.HasValue;
        }

        [XmlElement("CODE_MES1", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> CODE_MES1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string CODE_MES2 { get; set; }
        [XmlElement("NAPR", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<NAPR> NAPR { get; set; }

        [XmlElement("CONS", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<CONS> CONS { get; set; }        

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public ONK_SL ONK_SL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public KSG_KPG KSG_KPG { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? REAB { get; set; }
        public bool ShouldSerializeREAB()
        {
            return REAB.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PRVS { get; set; }
        public bool ShouldSerializePRVS()
        {
            return PRVS.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string VERS_SPEC { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string IDDOKT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? ED_COL { get; set; }
        public bool ShouldSerializeED_COL()
        {
            return ED_COL.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? TARIF { get; set; }
        public bool ShouldSerializeTARIF()
        {
            return TARIF.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SUM_M { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUM_MP { get; set; }
        public bool ShouldSerializeSUM_MP()
        {
            return SUM_MP.HasValue;
        }

     

        [XmlElement("USL", Form = XmlSchemaForm.Unqualified)]
        public List<USL> USL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string COMENTSL { get; set; }
    }
    
    [Serializable]
    public class DS2_N
    {
        public static DS2_N Get(DataRow row)
        {
            try
            {
                DS2_N item = new DS2_N();
                if (row["DS2"] != DBNull.Value)
                    item.DS2 = row["DS2"].ToString();
                if (row["DS2_PR"] != DBNull.Value)
                    item.DS2_PR = Convert.ToDecimal(row["DS2_PR"]);
                if (row["PR_DS2_N"] != DBNull.Value)
                    item.PR_DS2_N = Convert.ToDecimal(row["PR_DS2_N"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения DS2_N:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DS2 { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? DS2_PR { get; set; }
        public bool ShouldSerializeDS2_PR()
        {
            return DS2_PR.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PR_DS2_N { get; set; }
        public bool ShouldSerializePR_DS2_N()
        {
            return PR_DS2_N.HasValue;
        }


    }

    public class NAZR
    {
        public static NAZR Get(DataRow row)
        {
            try
            {
                NAZR item = new NAZR();
                if (row["NAZ_N"] != DBNull.Value)
                    item.NAZ_N = Convert.ToDecimal(row["NAZ_N"]);
                if (row["NAZ_PK"] != DBNull.Value)
                    item.NAZ_PK = Convert.ToDecimal(row["NAZ_PK"]);
                if (row["NAZ_PMP"] != DBNull.Value)
                    item.NAZ_PMP = Convert.ToDecimal(row["NAZ_PMP"]);
                if (row["NAZ_R"] != DBNull.Value)
                    item.NAZ_R = Convert.ToDecimal(row["NAZ_R"]);
                if (row["NAZ_SP"] != DBNull.Value)
                    item.NAZ_SP = Convert.ToDecimal(row["NAZ_SP"]);
                if (row["NAZ_V"] != DBNull.Value)
                    item.NAZ_V = Convert.ToDecimal(row["NAZ_V"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
          

                if (row["NAZ_USL"] != DBNull.Value)
                    item.NAZ_USL = Convert.ToString(row["NAZ_USL"]);

                if (row["NAPR_DATE"] != DBNull.Value)
                    item.NAPR_DATE = Convert.ToDateTime(row["NAPR_DATE"]);

                if (row["NAPR_MO"] != DBNull.Value)
                    item.NAPR_MO = Convert.ToString(row["NAPR_MO"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения NAZR:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal NAZ_N { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal NAZ_R { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? NAZ_SP { get; set; }
        public bool ShouldSerializeNAZ_SP()
        {
            return NAZ_SP.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? NAZ_V { get; set; }
        public bool ShouldSerializeNAZ_V()
        {
            return NAZ_V.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string NAZ_USL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true,DataType ="date")]
        public DateTime? NAPR_DATE { get; set; }
        public bool ShouldSerializeNAPR_DATE()
        {
            return NAPR_DATE.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string NAPR_MO { get; set; }
       

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? NAZ_PMP { get; set; }
        public bool ShouldSerializeNAZ_PMP()
        {
            return NAZ_PMP.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? NAZ_PK { get; set; }
        public bool ShouldSerializeNAZ_PK()
        {
            return NAZ_PK.HasValue;
        }


    }

    
    [Serializable]
    public  class ONK_SL
    {
        public static ONK_SL Get(DataRow row)
        {
            try
            {
                ONK_SL item = new ONK_SL();                
                if (row["DS1_T"] != DBNull.Value)
                    item.DS1_T = Convert.ToDecimal(row["DS1_T"]);
                if (row["MTSTZ"] != DBNull.Value)
                    item.MTSTZ = Convert.ToDecimal(row["MTSTZ"]);
                if (row["ONK_M"] != DBNull.Value)
                    item.ONK_M = Convert.ToDecimal(row["ONK_M"]);
                if (row["ONK_N"] != DBNull.Value)
                    item.ONK_N = Convert.ToDecimal(row["ONK_N"]);
                if (row["ONK_T"] != DBNull.Value)
                    item.ONK_T = Convert.ToDecimal(row["ONK_T"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                if (row["SOD"] != DBNull.Value)
                    item.SOD = Convert.ToDecimal(row["SOD"]);
                if (row["STAD"] != DBNull.Value)
                    item.STAD = Convert.ToDecimal(row["STAD"]);

                if (row["K_FR"] != DBNull.Value)
                    item.K_FR = Convert.ToDecimal(row["K_FR"]);
                if (row["WEI"] != DBNull.Value)
                    item.WEI = Convert.ToDecimal(row["WEI"]);
                if (row["HEI"] != DBNull.Value)
                    item.HEI = Convert.ToDecimal(row["HEI"]);
                if (row["BSA"] != DBNull.Value)
                    item.BSA = Convert.ToDecimal(row["BSA"]);


                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения ONK_SL:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
    

        public ONK_SL()
        {
            B_DIAG = new List<B_DIAG>();
            B_PROT = new List<B_PROT>();
            ONK_USL = new List<ONK_USL>();
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? DS1_T { get; set; }
        public bool ShouldSerializeDS1_T()
        {
            return DS1_T.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? STAD { get; set; }
        public bool ShouldSerializeSTAD()
        {
            return STAD.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? ONK_T { get; set; }
        public bool ShouldSerializeONK_T()
        {
            return ONK_T.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? ONK_N { get; set; }
        public bool ShouldSerializeONK_N()
        {
            return ONK_N.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? ONK_M { get; set; }
        public bool ShouldSerializeONK_M()
        {
            return ONK_M.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? MTSTZ { get; set; }
        public bool ShouldSerializeMTSTZ()
        {
            return MTSTZ.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SOD { get; set; }
        public bool ShouldSerializeSOD()
        {
            return SOD.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? K_FR { get; set; }
        public bool ShouldSerializeK_FR()
        {
            return K_FR.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? WEI { get; set; }
        public bool ShouldSerializeWEI()
        {
            return WEI.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? HEI { get; set; }
        public bool ShouldSerializeHEI()
        {
            return HEI.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? BSA { get; set; }
        public bool ShouldSerializeBSA()
        {
            return BSA.HasValue;
        }




        [XmlElement("B_DIAG", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<B_DIAG> B_DIAG { get; set; }

        [XmlElement("B_PROT", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<B_PROT> B_PROT { get; set; }

        [XmlElement("ONK_USL", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<ONK_USL> ONK_USL { get; set; }

    }    
    [Serializable]
    public  class B_DIAG
    {
        public static B_DIAG Get(DataRow row)
        {
            try
            {
                B_DIAG item = new B_DIAG();

                item.DIAG_CODE = Convert.ToDecimal(row["DIAG_CODE"]);
                if (row["DIAG_RSLT"] != DBNull.Value)
                    item.DIAG_RSLT = Convert.ToDecimal(row["DIAG_RSLT"]);
                item.DIAG_TIP = Convert.ToDecimal(row["DIAG_TIP"]);
                item.DIAG_DATE = Convert.ToDateTime(row["DIAG_DATE"]);
                if (row["REC_RSLT"] != DBNull.Value)
                    item.REC_RSLT = Convert.ToDecimal(row["REC_RSLT"]);

                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения B_DIAG:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DIAG_DATE { get; set; }
      

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal DIAG_TIP { get; set; }
     

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal DIAG_CODE { get; set; }
      
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal? DIAG_RSLT { get; set; }
        public bool ShouldSerializeDIAG_RSLT()
        {
            return DIAG_RSLT.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal? REC_RSLT { get; set; }
        public bool ShouldSerializeREC_RSLT()
        {
            return REC_RSLT.HasValue;
        }
    }

    
    [Serializable]
    public partial class B_PROT
    {
        public static B_PROT Get(DataRow row)
        {
            try
            {
                B_PROT item = new B_PROT();
                item.PROT = Convert.ToDecimal(row["PROT"]);
                item.D_PROT = Convert.ToDateTime(row["D_PROT"]);               
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения B_PROT:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
       

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal PROT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime D_PROT { get; set; }
    }

    
    [Serializable]
    public partial class KSG_KPG
    {
        public static KSG_KPG Get(DataRow row, IEnumerable<DataRow> CRIT)
        {
            try
            {
                KSG_KPG item = new KSG_KPG();               
                item.BZTSZ = Convert.ToDecimal(row["BZTSZ"]);

                foreach(var cr in CRIT)
                {
                    item.CRIT.Add(cr["CRIT"].ToString());
                }
                if (row["IT_SL"] != DBNull.Value)
                    item.IT_SL = Convert.ToDecimal(row["IT_SL"]);
                item.KOEF_D = Convert.ToDecimal(row["KOEF_D"]);
                item.KOEF_U = Convert.ToDecimal(row["KOEF_U"]);
                item.KOEF_UP = Convert.ToDecimal(row["KOEF_UP"]);
                item.KOEF_Z = Convert.ToDecimal(row["KOEF_Z"]);
                item.KSG_PG = Convert.ToDecimal(row["KSG_PG"]);
                item.N_KSG = row["N_KSG"].ToString();
                item.SL_K = Convert.ToDecimal(row["SL_K"]);
                item.VER_KSG = Convert.ToDecimal(row["VER_KSG"]);
                
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения KSG_KPG:" + ex.Message);
            }
        }

        public List<CRIT_SLUCH_ID> GetCRIT_SLUCH_ID(decimal? sluch_id)
        {
            List<CRIT_SLUCH_ID> rez = new List<CRIT_SLUCH_ID>();
            int i = 0;
            foreach(var cr in CRIT)
            {
                rez.Add(new CRIT_SLUCH_ID() { CRIT = cr, ORD = i, SLUCH_ID = sluch_id });
                i++;
            }
            return rez;
        }
        public KSG_KPG()
        {
            SL_KOEF = new List<SL_KOEF>();
            CRIT = new List<string>();
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string N_KSG { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal VER_KSG { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal KSG_PG { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal KOEF_Z { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal KOEF_UP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal BZTSZ { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal KOEF_D { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal KOEF_U { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<string> CRIT { get; set; }

      //  [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
       // public string DKK2 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SL_K { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? IT_SL { get; set; }

        public bool ShouldSerializeIT_SL()
        {
            return IT_SL.HasValue;
        }

        [XmlElement("SL_KOEF", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<SL_KOEF> SL_KOEF { get; set; }
    }

    
    [Serializable]
    public partial class SL_KOEF
    {
        public static SL_KOEF Get(DataRow row)
        {
            try
            {
                SL_KOEF item = new SL_KOEF();
                item.IDSL = Convert.ToDecimal(row["IDSL"]);
                item.Z_SL = Convert.ToDecimal(row["Z_SL"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения SL_KOEF:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }

       

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal IDSL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal Z_SL { get; set; }
    }

    


    [Serializable]
    public class SANK
    {
        public SANK()
        {
            CODE_EXP = new List<CODE_EXP>();
            SL_ID = new List<string>();
        }
        public static SANK Get(DataRow row)
        {
            try
            {
                SANK item = new SANK();
                if (row["SANK_ID"] != DBNull.Value)
                    item.SANK_ID = Convert.ToDecimal(row["SANK_ID"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                if (row["S_CODE"] != DBNull.Value)
                    item.S_CODE = row["S_CODE"].ToString();
                if (row["S_COM"] != DBNull.Value)
                    item.S_COM = row["S_COM"].ToString();
                if (row["S_FINE"] != DBNull.Value)
                    item.S_FINE = Convert.ToDecimal(row["S_FINE"]);
                if (row["S_IDSERV"] != DBNull.Value)
                    item.S_IDSERV = row["S_IDSERV"].ToString();
                if (row["S_IST"] != DBNull.Value)
                    item.S_IST = Convert.ToDecimal(row["S_IST"]);
                if (row["S_MONTH"] != DBNull.Value)
                    item.S_MONTH = Convert.ToDecimal(row["S_MONTH"]);
                if (row["S_OSN"] != DBNull.Value)
                    item.S_OSN = Convert.ToDecimal(row["S_OSN"]);
                if (row["S_PLAN"] != DBNull.Value)
                    item.S_PLAN = Convert.ToDecimal(row["S_PLAN"]);
                if (row["S_SUM"] != DBNull.Value)
                    item.S_SUM = Convert.ToDecimal(row["S_SUM"]);
                if (row["S_TEM"] != DBNull.Value)
                    item.S_TEM = Convert.ToDecimal(row["S_TEM"]);
                if (row["S_TIP"] != DBNull.Value)
                    item.S_TIP = Convert.ToDecimal(row["S_TIP"]);
                if (row["S_YEAR"] != DBNull.Value)
                    item.S_YEAR = Convert.ToDecimal(row["S_YEAR"]);
                if (row["S_ZGLV_ID"] != DBNull.Value)
                    item.S_ZGLV_ID = Convert.ToDecimal(row["S_ZGLV_ID"]);
                if (row["DATE_ACT"] != DBNull.Value)
                    item.DATE_ACT = Convert.ToDateTime(row["DATE_ACT"]);
                if (row["NUM_ACT"] != DBNull.Value)
                    item.NUM_ACT = Convert.ToString(row["NUM_ACT"]);
                if (row["S_OSN_TFOMS"] != DBNull.Value)
                    item.S_OSN_TFOMS = Convert.ToString(row["S_OSN_TFOMS"]);
                

                foreach (var exp in row["SL_ID"].ToString().Split(','))
                {
                    if (!string.IsNullOrEmpty(exp))
                    {
                        item.SL_ID.Add(exp);
                    }
                }

                foreach (var exp in row["CODE_EXP"].ToString().Split(','))
                {
                    if(!string.IsNullOrEmpty(exp))
                    {
                        item.CODE_EXP.Add(new EntityMP_V31.CODE_EXP() { SANK_ID = item.SANK_ID, VALUE = exp });
                    }
                }
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения SANK:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? S_ZGLV_ID { get; set; }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
        [XmlIgnore]
        public decimal? SLUCH_Z_ID { get; set; }
        [XmlIgnore]
        public decimal? SANK_ID { get; set; }
     

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string S_CODE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_SUM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_TIP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =false)]
        public List<string> SL_ID { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_OSN { get; set; }
        [XmlElement("DATE_ACT", Form = XmlSchemaForm.Unqualified, IsNullable = true, DataType ="date")]
        public DateTime? DATE_ACT { get; set; }
        public bool ShouldSerializeDATE_ACT()
        {
            return DATE_ACT.HasValue;
        }
        [XmlElement("NUM_ACT", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string NUM_ACT { get; set; }

        [XmlElement("CODE_EXP", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<CODE_EXP> CODE_EXP { get; set; }
      
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string S_COM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_IST { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_YEAR { get; set; }
        public bool ShouldSerializeS_YEAR()
        {
            return S_YEAR.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_MONTH { get; set; }
        public bool ShouldSerializeS_MONTH()
        {
            return S_MONTH.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_PLAN { get; set; }
        public bool ShouldSerializeS_PLAN()
        {
            return S_PLAN.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_TEM { get; set; }
        public bool ShouldSerializeS_TEM()
        {
            return S_TEM.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_FINE { get; set; }
        public bool ShouldSerializeS_FINE()
        {
            return S_FINE.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string S_OSN_TFOMS { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string S_IDSERV { get; set; }

        [XmlIgnore]
        public string GetSL_ID => string.Join(",", SL_ID);
    }

    public class CODE_EXP
    {
        [XmlIgnore]
        public decimal? SANK_ID { get; set; }
        [XmlText]
        public string VALUE { get; set; }
    }


    [Serializable]
    public  class USL
    {
        public static USL Get(DataRow row)
        {
            try
            {
                USL item = new USL();

                if (row["CODE_MD"] != DBNull.Value)
                    item.CODE_MD = row["CODE_MD"].ToString();
                if (row["CODE_USL"] != DBNull.Value)
                    item.CODE_USL = row["CODE_USL"].ToString();
                if (row["COMENTU"] != DBNull.Value)
                    item.COMENTU = row["COMENTU"].ToString();
                if (row["DATE_IN"] != DBNull.Value)
                    item.DATE_IN = Convert.ToDateTime(row["DATE_IN"]);
                if (row["DATE_OUT"] != DBNull.Value)
                    item.DATE_OUT = Convert.ToDateTime(row["DATE_OUT"]);
                if (row["DET"] != DBNull.Value)
                    item.DET = Convert.ToDecimal(row["DET"]);
                if (row["DS"] != DBNull.Value)
                    item.DS = row["DS"].ToString();
                if (row["IDSERV"] != DBNull.Value)
                    item.IDSERV = row["IDSERV"].ToString();
                if (row["KOL_USL"] != DBNull.Value)
                    item.KOL_USL = Convert.ToDecimal(row["KOL_USL"]);
                if (row["LPU"] != DBNull.Value)
                    item.LPU = row["LPU"].ToString();

                if (row["LPU_1"] != DBNull.Value)
                    item.LPU_1 = row["LPU_1"].ToString(); ;
                if (row["NPL"] != DBNull.Value)
                    item.NPL = Convert.ToDecimal(row["NPL"]);
                if (row["PODR"] != DBNull.Value)
                    item.PODR = Convert.ToDecimal(row["PODR"]);
                if (row["PROFIL"] != DBNull.Value)
                    item.PROFIL = Convert.ToDecimal(row["PROFIL"]);
                if (row["PRVS"] != DBNull.Value)
                    item.PRVS = Convert.ToDecimal(row["PRVS"]);

                if (row["P_OTK"] != DBNull.Value)
                    item.P_OTK = Convert.ToDecimal(row["P_OTK"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);

                if (row["SUMP_USL"] != DBNull.Value)
                    item.SUMP_USL = Convert.ToDecimal(row["SUMP_USL"]);



                if (row["SUMV_USL"] != DBNull.Value)
                    item.SUMV_USL = Convert.ToDecimal(row["SUMV_USL"]);
                if (row["TARIF"] != DBNull.Value)
                    item.TARIF = Convert.ToDecimal(row["TARIF"]);
                if (row["USL_ID"] != DBNull.Value)
                    item.USL_ID = Convert.ToDecimal(row["USL_ID"]);
                if (row["VID_VME"] != DBNull.Value)
                    item.VID_VME = row["VID_VME"].ToString();
                if (row["PS_VOLUME"] != DBNull.Value)
                    item.PS_VOLUME = row["PS_VOLUME"].ToString();
                if (row["BSP"] != DBNull.Value)
                    item.BSP = Convert.ToInt32(row["BSP"]);
                if (row["NOT_VR"] != DBNull.Value)
                    item.NOT_VR = Convert.ToInt32(row["NOT_VR"]);


                
                    
                        

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения USL:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
        [XmlIgnore]
        public decimal? USL_ID { get; set; }
       

        public USL()
        {
          
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string IDSERV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LPU { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string LPU_1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PODR { get; set; }
        public bool ShouldSerializePODR()
        {
            return PODR.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PROFIL { get; set; }
        public bool ShouldSerializePROFIL()
        {
            return PROFIL.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string VID_VME { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? DET { get; set; }
        public bool ShouldSerializeDET()
        {
            return DET.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_IN { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_OUT { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? P_OTK { get; set; }
        public bool ShouldSerializeP_OTK()
        {
            return P_OTK.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string CODE_USL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? KOL_USL { get; set; }
        public bool ShouldSerializeKOL_USL()
        {
            return KOL_USL.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? TARIF { get; set; }
        public bool ShouldSerializeTARIF()
        {
            return TARIF.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SUMV_USL { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUMP_USL { get; set; }
        public bool ShouldSerializeSUMP_USL()
        {
            return SUMP_USL.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal? PRVS { get; set; }
        public bool ShouldSerializePRVS()
        {
            return PRVS.HasValue;
        }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string CODE_MD { get; set; }

       
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? NPL { get; set; }

        public bool ShouldSerializeNPL()
        {
            return NPL.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string COMENTU { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string PS_VOLUME { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public int? BSP { get; set; }

        public bool ShouldSerializeBSP()
        {
            return BSP.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public int? NOT_VR { get; set; }

        public bool ShouldSerializeNOT_VR()
        {
            return NOT_VR.HasValue;
        }
    }


    [Serializable]
    public  class NAPR
    {
        public static NAPR Get(DataRow row)
        {
            try
            {
                NAPR item = new NAPR();
                if (row["MET_ISSL"] != DBNull.Value)
                    item.MET_ISSL = Convert.ToDecimal(row["MET_ISSL"]);
                item.NAPR_DATE = Convert.ToDateTime(row["NAPR_DATE"]);
                if (row["NAPR_USL"] != DBNull.Value)
                    item.NAPR_USL = row["NAPR_USL"].ToString();
                if (row["NAPR_MO"] != DBNull.Value)
                    item.NAPR_MO = row["NAPR_MO"].ToString();
                item.NAPR_V = Convert.ToDecimal(row["NAPR_V"]);
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                if (row["IDSERV"] != DBNull.Value)
                    item.IDSERV = Convert.ToString(row["IDSERV"]);
                if (row["USL_ID"] != DBNull.Value)
                    item.USL_ID = Convert.ToDecimal(row["USL_ID"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения NAPR:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? USL_ID { get; set; }

        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
        [XmlIgnore]
        public string IDSERV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime NAPR_DATE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string NAPR_MO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal NAPR_V { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? MET_ISSL { get; set; }

        public bool ShouldSerializeMET_ISSL()
        {
            return MET_ISSL.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string NAPR_USL { get; set; }
    }

    [Serializable]
    public class CONS
    {
        public static CONS Get(DataRow row)
        {
            try
            {
                CONS item = new CONS();
                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                if (row["PR_CONS"] != DBNull.Value)
                    item.PR_CONS = Convert.ToDecimal(row["PR_CONS"]);
                if (row["DT_CONS"] != DBNull.Value)
                    item.DT_CONS = Convert.ToDateTime(row["DT_CONS"]);
                if (row["IDSERV"] != DBNull.Value)
                    item.IDSERV = Convert.ToString(row["IDSERV"]);
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения CONS:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }

        [XmlIgnore]
        public string IDSERV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PR_CONS { get; set; }
        public bool ShouldSerializePR_CONS()
        {
            return PR_CONS.HasValue;
        }
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date", IsNullable = true)]
        public DateTime? DT_CONS { get; set; }
        public bool ShouldSerializeDT_CONS()
        {
            return DT_CONS.HasValue;
        }
    }


    [Serializable]
    public  class ONK_USL
    {
        public ONK_USL()
        {
            LEK_PR = new List<LEK_PR>();
        }

        [XmlIgnore]
        public decimal? USL_ID { get; set; }
        public static ONK_USL Get(DataRow row)
        {
            try
            {
                ONK_USL item = new ONK_USL();
                if (row["HIR_TIP"] != DBNull.Value)
                    item.HIR_TIP = Convert.ToDecimal(row["HIR_TIP"]);
                if (row["LEK_TIP_L"] != DBNull.Value)
                    item.LEK_TIP_L = Convert.ToDecimal(row["LEK_TIP_L"]);
                if (row["LEK_TIP_V"] != DBNull.Value)
                    item.LEK_TIP_V = Convert.ToDecimal(row["LEK_TIP_V"]);
                if (row["LUCH_TIP"] != DBNull.Value)
                    item.LUCH_TIP = Convert.ToDecimal(row["LUCH_TIP"]);
                item.USL_TIP = Convert.ToDecimal(row["USL_TIP"]);
                if (row["IDSERV"] != DBNull.Value)
                    item.IDSERV = Convert.ToString(row["IDSERV"]);

                if (row["SLUCH_ID"] != DBNull.Value)
                    item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                if (row["ONK_USL_ID"] != DBNull.Value)
                    item.ONK_USL_ID = Convert.ToDecimal(row["ONK_USL_ID"]);
                if (row["USL_ID"] != DBNull.Value)
                    item.USL_ID = Convert.ToDecimal(row["USL_ID"]);
                if (row["PPTR"] != DBNull.Value)
                    item.PPTR = Convert.ToDecimal(row["PPTR"]);
                
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения ONK_USL:" + ex.Message);
            }
        }
        [XmlIgnore]
        public string IDSERV { get; set; }

        [XmlIgnore]
        public decimal? SLUCH_ID { get; set; }
        [XmlIgnore]
        public decimal? ONK_USL_ID { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal USL_TIP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]

        public decimal? HIR_TIP { get; set; }
        public bool ShouldSerializeHIR_TIP()
        {
            return HIR_TIP.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? LEK_TIP_L { get; set; }
        public bool ShouldSerializeLEK_TIP_L()
        {
            return LEK_TIP_L.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? LEK_TIP_V { get; set; }
        public bool ShouldSerializeLEK_TIP_V()
        {
            return LEK_TIP_V.HasValue;
        }

        [XmlElement("LEK_PR", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<LEK_PR> LEK_PR { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PPTR { get; set; }
        public bool ShouldSerializePPTR()
        {
            return PPTR.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? LUCH_TIP { get; set; }
        public bool ShouldSerializeLUCH_TIP()
        {
            return LUCH_TIP.HasValue;
        }
    }


    [Serializable]
    public class LEK_PR
    {
        public static LEK_PR Get(DataRow row)
        {
            try
            {
                LEK_PR item = new LEK_PR();
                if(row["REGNUM"] !=DBNull.Value)
                    item.REGNUM = row["REGNUM"].ToString();
                if (row["CODE_SH"] != DBNull.Value)
                    item.CODE_SH = row["CODE_SH"].ToString();
                if (row["ONK_USL_ID"] != DBNull.Value)
                    item.ONK_USL_ID = Convert.ToDecimal( row["ONK_USL_ID"]);


                foreach (var t in row["DATE_INJ"].ToString().Split(',').Where(x=>x!="" && x!=null).Select(x => Convert.ToDateTime(x)).ToList())
                {
                    item.DATE_INJ.Add(new EntityMP_V31.DATE_INJ() { VALUE = t });
                };
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения LEK_PR:" + ex.Message);
            }
        }
    

        [XmlIgnore]
        public decimal? ONK_USL_ID { get; set; }
        [XmlIgnore]
        public decimal? LEK_PR_ID { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string REGNUM { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string CODE_SH { get; set; }
        

        [XmlElement("DATE_INJ", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<DATE_INJ> DATE_INJ { get; set; } = new List<EntityMP_V31.DATE_INJ>();

    }

    public class DATE_INJ
    {
        [XmlIgnore]
        public decimal? LEK_PR_ID { get; set; }
        [XmlText(DataType = "date")]
        public DateTime VALUE { get; set; }

    }



    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public  class PERS_LIST
    {

        public static PERS_LIST LoadFromFile(string Path)
        {
            Stream st = null;
            try
            {
               var ser = new XmlSerializer(typeof(PERS_LIST));
                st = File.OpenRead(Path);
                PERS_LIST pe = (PERS_LIST)ser.Deserialize(st);
                return pe;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);

            }
            finally
            {
                st?.Dispose();
            }
        }

        public PERS_LIST()
        {
            this.PERS = new List<PERS>();
            this.ZGLV = new PERSZGLV();
        }

        [System.Xml.Serialization.XmlElementAttribute("ZGLV", Form = XmlSchemaForm.Unqualified)]
        public PERSZGLV ZGLV { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("PERS", Form = XmlSchemaForm.Unqualified)]
        public List<PERS> PERS { get; set; }


        public void SetID(decimal ZGLV_ID, decimal PERS_ID)
        {
            ZGLV.ZGLV_ID = ZGLV_ID;
            foreach(PERS p in PERS)
            {
                p.ZGLV_ID = ZGLV_ID;
                p.PERS_ID = PERS_ID;
                PERS_ID++;
            }
        }
    }

    
    [Serializable]
    [XmlType(AnonymousType = true)]
    public  class PERSZGLV
    {
        public static PERSZGLV Get(DataRow row)
        {
            try
            {
                PERSZGLV item = new PERSZGLV();

                item.DATA = Convert.ToDateTime(row["DATA"]);
                item.FILENAME = row["FILENAME"].ToString();
                item.FILENAME1 = row["FILENAME1"].ToString();
                item.VERSION = row["VERSION"].ToString();
                if (row["ZGLV_ID"] != DBNull.Value)
                    item.ZGLV_ID = Convert.ToDecimal(row["ZGLV_ID"]);

                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения PERSZGLV:" + ex.Message);
            }
        }
        [XmlIgnore]
        public decimal? ZGLV_ID { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
        public string VERSION { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATA { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
        public string FILENAME { get; set; }

        [System.Xml.Serialization.XmlElementAttribute(Form = XmlSchemaForm.Unqualified)]
        public string FILENAME1 { get; set; }

    }

    
    [Serializable]
    public  class PERS
    {
        public static PERS Get(DataRow row)
        {
            try
            {
                PERS item = new PERS();
                if (row["COMENTP"] != DBNull.Value)
                    item.COMENTP = row["COMENTP"].ToString();
                if (row["DOCNUM"] != DBNull.Value)
                    item.DOCNUM = row["DOCNUM"].ToString();
                if (row["DOCSER"] != DBNull.Value)
                    item.DOCSER = row["DOCSER"].ToString();
                if (row["DOCTYPE"] != DBNull.Value)
                    item.DOCTYPE = row["DOCTYPE"].ToString();
                if (row["DOST"] != DBNull.Value)
                    item.setDOST(row["DOST"].ToString());
                if (row["DOST_P"] != DBNull.Value)
                    item.setDOST_P(row["DOST_P"].ToString());
                if (row["DR"] != DBNull.Value)
                    item.DR = Convert.ToDateTime(row["DR"]);
                if (row["DR_P"] != DBNull.Value)
                    item.DR_P = Convert.ToDateTime(row["DR_P"]);
                if (row["FAM"] != DBNull.Value)
                    item.FAM = row["FAM"].ToString();
                if (row["FAM_P"] != DBNull.Value)
                    item.FAM_P = row["FAM_P"].ToString();
                if (row["ID_PAC"] != DBNull.Value)
                    item.ID_PAC = row["ID_PAC"].ToString();
                if (row["IM"] != DBNull.Value)
                    item.IM = row["IM"].ToString();
                if (row["IM_P"] != DBNull.Value)
                    item.IM_P = row["IM_P"].ToString();
                if (row["MR"] != DBNull.Value)
                    item.MR = row["MR"].ToString();
                if (row["OKATOG"] != DBNull.Value)
                    item.OKATOG = row["OKATOG"].ToString();
                if (row["OKATOP"] != DBNull.Value)
                    item.OKATOP = row["OKATOP"].ToString();
                if (row["OT"] != DBNull.Value)
                    item.OT = row["OT"].ToString();
                if (row["OT_P"] != DBNull.Value)
                    item.OT_P = row["OT_P"].ToString();

                if (row["PERS_ID"] != DBNull.Value)
                    item.PERS_ID = Convert.ToDecimal(row["PERS_ID"]);
                if (row["SNILS"] != DBNull.Value)
                    item.SNILS = row["SNILS"].ToString();
                if (row["TEL"] != DBNull.Value)
                    item.TEL = row["TEL"].ToString();
                if (row["W"] != DBNull.Value)
                    item.W = Convert.ToDecimal(row["W"]);
                if (row["W_P"] != DBNull.Value)
                    item.W_P = Convert.ToDecimal(row["W_P"]);
                if (row["ZGLV_ID"] != DBNull.Value)
                    item.ZGLV_ID = Convert.ToDecimal(row["ZGLV_ID"]);

                if (row["DOCNUM"] != DBNull.Value)
                    item.DOCNUM = Convert.ToString(row["DOCNUM"]);
                if (row["DOCDATE"] != DBNull.Value)
                    item.DOCDATE = Convert.ToDateTime(row["DOCDATE"]);


                return item;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка получения PERS:" + ex.Message);
            }
        }


        [XmlIgnore]
        public decimal? ZGLV_ID { get; set; }

        [XmlIgnore]
        public decimal? PERS_ID { get; set; }

        public PERS()
        {
            this.DOST_P = new List<decimal>();           
            this.DOST = new List<decimal>();
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string ID_PAC { get; set; }

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

        [XmlElement("DOST", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<decimal> DOST { get; set; }

        [XmlElement("TEL", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string TEL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string FAM_P { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string IM_P { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string OT_P { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? W_P { get; set; }

        public bool ShouldSerializeW_P()
        {
            return W_P.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date", IsNullable = true)]
        public DateTime? DR_P { get; set; }
        public bool ShouldSerializeDR_P()
        {
            return DR_P.HasValue;
        }

        [XmlElement("DOST_P", Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public List<decimal> DOST_P { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string MR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DOCTYPE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DOCSER { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DOCNUM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date", IsNullable = true)]
        public DateTime? DOCDATE { get; set; }
        public bool ShouldSerializeDOCDATE()
        {
            return DOCDATE.HasValue;
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DOCORG { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string SNILS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string OKATOG { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string OKATOP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string COMENTP { get; set; }

    }

    
    public static class ExtZLLIST
    {

        public static void WriteXml(this ServiceLoaderMedpomData.EntityMP_V3.ZL_LIST zl, Stream st)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ServiceLoaderMedpomData.EntityMP_V3.ZL_LIST));
            XmlWriterSettings set = new XmlWriterSettings();
            set.Encoding = System.Text.Encoding.GetEncoding("Windows-1251");
            set.Indent = true;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriter xml = XmlWriter.Create(st, set);
            ser.Serialize(xml, zl, ns);
           
        }

    
        public static void WriteXml(this ZL_LIST zl, Stream st)
        {
            XmlSerializer ser = new XmlSerializer(typeof(ZL_LIST));
            XmlWriterSettings set = new XmlWriterSettings();
            set.Encoding = System.Text.Encoding.GetEncoding("Windows-1251");
            set.Indent = true;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriter xml = XmlWriter.Create(st, set);
            ser.Serialize(xml, zl, ns);
        }

        public static void WriteXml(this PERS_LIST zl, Stream st)
        {
            XmlSerializer ser = new XmlSerializer(typeof(PERS_LIST));
            XmlWriterSettings set = new XmlWriterSettings();
            set.Encoding = System.Text.Encoding.GetEncoding("Windows-1251");
            set.Indent = true;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriter xml = XmlWriter.Create(st, set);
            ser.Serialize(xml, zl, ns);
        }

        public static string OS_SLUCHstr(this Z_SL z_sl)
        {
            return Concat(z_sl.OS_SLUCH);
        }

        public static string VNOV_Mstr(this Z_SL z_sl)
        {
            return Concat(z_sl.VNOV_M);
        }

        public static string CODE_MES1str(this SL sl)
        {            
            return Concat(sl.CODE_MES1);
        }
      
        public static string DOSTstr(this PERS sl)
        {
            return Concat(sl.DOST);
        }

        public static string DOST_Pstr(this PERS sl)
        {
            return Concat(sl.DOST_P);
        }

        private static string Concat(List<string> items)
        {
            string str = "";
            for (int i = 0; i < items.Count; i++)
            {
                str += items[i];
                if (i != items.Count - 1)
                {
                    str += ";";
                }
            }
            return str;
        }
        public static string Concat(this List<decimal> items)
        {
            string str = "";
            for (int i = 0; i < items.Count; i++)
            {
                str += items[i];
                if (i != items.Count - 1)
                {
                    str += ";";
                }
            }
            return str;
        }


        public static void setOS_SLUCH(this Z_SL sl, string OS_SLUCH)
        {
            if (OS_SLUCH == "") return;
            foreach (string d in OS_SLUCH.Split(';'))
            {
                if(d.Trim()!="")
                    sl.OS_SLUCH.Add(d.Trim());
            }
        }

        public static void setVNOV_M(this Z_SL sl, string VNOV_M)
        {
            if (VNOV_M == "") return;
            foreach (string d in VNOV_M.Split(';'))
            {
                if (d.Trim() != "")
                    sl.VNOV_M.Add(d.Trim());
            }
        }


        public static void setDOST(this PERS pac, string DOST)
        {
            if (DOST == "") return;
            foreach (string d in DOST.Split(';'))
            {
                if(d.Trim()!="")
                    pac.DOST.Add(Convert.ToDecimal(d.Trim()));
            }
        }

        public static void setDOST_P(this PERS pac, string DOST)
        {
            if (DOST == "") return;
            foreach (string d in DOST.Split(';'))
            {
                if (d.Trim() != "")
                    pac.DOST_P.Add(Convert.ToDecimal(d.Trim()));
            }
        }


      

       

        public static void setCODE_MES1(this SL sl, string CODE_MES1)
        {
            if (CODE_MES1 == "") return;
            foreach (string d in CODE_MES1.Split(';'))
            {
                if (d.Trim() != "")
                    sl.CODE_MES1.Add(d.Trim());
            }
        }


        public static EntityMP_V3.ZL_LIST toZL_LISTV2(this ZL_LIST zl31)
        {
            EntityMP_V3.ZL_LIST l3 = new EntityMP_V3.ZL_LIST();

            l3.ZGLV.VERSION = zl31.ZGLV.VERSION;
            l3.ZGLV.DATA = zl31.ZGLV.DATA;
            l3.ZGLV.FILENAME = zl31.ZGLV.FILENAME;
            l3.ZGLV.SD_Z = zl31.ZGLV.SD_Z;

            l3.SCHET.CODE = zl31.SCHET.CODE;
            l3.SCHET.CODE_MO = zl31.SCHET.CODE_MO;
            l3.SCHET.COMENTS = zl31.SCHET.COMENTS;
            l3.SCHET.DISP = zl31.SCHET.DISP;
            l3.SCHET.DSCHET = zl31.SCHET.DSCHET;
            l3.SCHET.MONTH = zl31.SCHET.MONTH;
            l3.SCHET.NSCHET = zl31.SCHET.NSCHET;
            l3.SCHET.PLAT = zl31.SCHET.PLAT;
            l3.SCHET.SANK_EKMP = zl31.SCHET.SANK_EKMP;
            l3.SCHET.SANK_MEE = zl31.SCHET.SANK_MEE;
            l3.SCHET.SANK_MEK = zl31.SCHET.SANK_MEK;
            l3.SCHET.SUMMAP = zl31.SCHET.SUMMAP;
            l3.SCHET.SUMMAV = zl31.SCHET.SUMMAV;
            l3.SCHET.YEAR = zl31.SCHET.YEAR;

            foreach (EntityMP_V31.ZAP z2 in zl31.ZAP)
            {
                EntityMP_V3.ZAP z = new EntityMP_V3.ZAP();
                l3.ZAP.Add(z);

                z.N_ZAP = z2.N_ZAP;
                z.PR_NOV = z2.PR_NOV;
                z.PACIENT = new EntityMP_V3.PACIENT();
                z.PACIENT.ID_PAC = z2.PACIENT.ID_PAC;
                z.PACIENT.MSE = z2.PACIENT.MSE;
                z.PACIENT.NOVOR = z2.PACIENT.NOVOR;
                z.PACIENT.NPOLIS = z2.PACIENT.NPOLIS;
                z.PACIENT.SMO = z2.PACIENT.SMO;
                z.PACIENT.SMO_NAM = z2.PACIENT.SMO_NAM;
                z.PACIENT.SMO_OGRN = z2.PACIENT.SMO_OGRN;
                z.PACIENT.SMO_OK = z2.PACIENT.SMO_OK;
                z.PACIENT.SPOLIS = z2.PACIENT.SPOLIS;
                z.PACIENT.ST_OKATO = z2.PACIENT.ST_OKATO;
                z.PACIENT.VNOV_D = z2.PACIENT.VNOV_D;
                z.PACIENT.VPOLIS = z2.PACIENT.VPOLIS;
                z.PACIENT.INV = z2.PACIENT.INV;

                foreach (var z_sl2 in z2.Z_SL_list)
                {                    
                    var Z_SL = new EntityMP_V3.Z_SL();
                    z.Z_SL_list.Add(Z_SL);
                    Z_SL.DATE_Z_1 = z_sl2.DATE_Z_1;
                    Z_SL.DATE_Z_2 = z_sl2.DATE_Z_2;
                    Z_SL.FOR_POM = z_sl2.FOR_POM;
                    Z_SL.IDCASE = z_sl2.IDCASE;
                    Z_SL.IDSP = z_sl2.IDSP;
                    Z_SL.ISHOD = z_sl2.ISHOD;
                    Z_SL.KD_Z = z_sl2.KD_Z;
                    Z_SL.LPU = z_sl2.LPU;
                    Z_SL.NPR_DATE = z_sl2.NPR_DATE;
                    Z_SL.NPR_MO = z_sl2.NPR_MO;
                    Z_SL.OPLATA = z_sl2.OPLATA;
                    Z_SL.OS_SLUCH = z_sl2.OS_SLUCH;
                    Z_SL.PACIENT_ID = z_sl2.PACIENT_ID;
                    Z_SL.P_OTK = z_sl2.P_OTK;
                    Z_SL.RSLT = z_sl2.RSLT;
                    Z_SL.RSLT_D = z_sl2.RSLT_D;
                    Z_SL.SANK_IT = z_sl2.SANK_IT;
                    Z_SL.SLUCH_Z_ID = z_sl2.SLUCH_Z_ID;
                    Z_SL.SUMP = z_sl2.SUMP;
                    Z_SL.SUMV = z_sl2.SUMV;
                    Z_SL.USL_OK = z_sl2.USL_OK;
                    Z_SL.VBR = z_sl2.VBR;
                    Z_SL.VB_P = z_sl2.VB_P;
                    Z_SL.VIDPOM = z_sl2.VIDPOM;
                    Z_SL.VNOV_M = z_sl2.VNOV_M;
                    Z_SL.ZAP_ID = z_sl2.ZAP_ID;

                    
                    foreach (var sl31 in z_sl2.SL)
                    {
                        var SL = new EntityMP_V3.SL();
                        Z_SL.SL.Add(SL);
                        SL.CODE_MES1 = sl31.CODE_MES1;
                        SL.CODE_MES2 = sl31.CODE_MES2;
                        SL.COMENTSL = sl31.COMENTSL;



                        SL.DATE_1 = sl31.DATE_1;
                        SL.DATE_2 = sl31.DATE_2;
                        SL.DET = sl31.DET;
                        SL.DN = sl31.DN;
                        SL.DS0 = sl31.DS0;
                        SL.DS1 = sl31.DS1;
                        SL.DS1_PR = sl31.DS1_PR;
                        SL.DS2 = sl31.DS2;

                        foreach (var ds2_n in sl31.DS2_N)
                        {
                            SL.DS2_N.Add(new EntityMP_V3.DS2_N() { DS2 = ds2_n.DS2, DS2_PR = ds2_n.DS2_PR, PR_DS2_N = ds2_n.PR_DS2_N, SLUCH_ID = ds2_n.SLUCH_ID });
                        }

                        SL.DS3 = sl31.DS3;
                        SL.DS_ONK = sl31.DS_ONK;
                        SL.ED_COL = sl31.ED_COL;
                        SL.EXTR = sl31.EXTR;
                        SL.IDDOKT = sl31.IDDOKT;
                        SL.KD = sl31.KD;


                        if (sl31.KSG_KPG != null)
                        {
                            SL.KSG_KPG = new EntityMP_V3.KSG_KPG();
                            SL.KSG_KPG.BZTSZ = sl31.KSG_KPG.BZTSZ;
                            if (sl31.KSG_KPG.CRIT.Count >= 1)
                                SL.KSG_KPG.DKK1 = sl31.KSG_KPG.CRIT[0];
                            if (sl31.KSG_KPG.CRIT.Count >= 2)
                                SL.KSG_KPG.DKK2 = sl31.KSG_KPG.CRIT[1];
                            SL.KSG_KPG.IT_SL = sl31.KSG_KPG.IT_SL;
                            SL.KSG_KPG.KOEF_D = sl31.KSG_KPG.KOEF_D;
                            SL.KSG_KPG.KOEF_U = sl31.KSG_KPG.KOEF_U;
                            SL.KSG_KPG.KOEF_UP = sl31.KSG_KPG.KOEF_UP;
                            SL.KSG_KPG.KOEF_Z = sl31.KSG_KPG.KOEF_Z;
                            SL.KSG_KPG.KSG_PG = sl31.KSG_KPG.KSG_PG;
                            SL.KSG_KPG.N_KSG = sl31.KSG_KPG.N_KSG;
                            SL.KSG_KPG.SL_K = sl31.KSG_KPG.SL_K;
                            foreach (var k in sl31.KSG_KPG.SL_KOEF)
                            {
                                SL.KSG_KPG.SL_KOEF.Add(new EntityMP_V3.SL_KOEF() { IDSL = k.IDSL, SLUCH_ID = k.SLUCH_ID, Z_SL = k.Z_SL });
                            }
                            SL.KSG_KPG.VER_KSG = sl31.KSG_KPG.VER_KSG;
                        }


                        SL.LPU_1 = sl31.LPU_1;
                        SL.METOD_HMP = sl31.METOD_HMP;

                        foreach (var naz in sl31.NAZ)
                        {
                            SL.NAZ.Add(new EntityMP_V3.NAZR() { NAZ_N = naz.NAZ_N, NAZ_PK = naz.NAZ_PK, NAZ_PMP = naz.NAZ_PMP, NAZ_R = naz.NAZ_R, NAZ_SP = naz.NAZ_SP, NAZ_V = naz.NAZ_V });
                        }

                        SL.NHISTORY = sl31.NHISTORY;



                        if (sl31.ONK_SL != null)
                        {
                            SL.ONK_SL = new EntityMP_V3.ONK_SL();
                            SL.ONK_SL.SOD = sl31.ONK_SL.SOD;
                            if (sl31.ONK_SL.STAD.HasValue)
                                SL.ONK_SL.STAD = sl31.ONK_SL.STAD.Value;
                            SL.ONK_SL.SLUCH_ID = sl31.ONK_SL.SLUCH_ID;
                            if (sl31.ONK_SL.ONK_T.HasValue)
                                SL.ONK_SL.ONK_T = sl31.ONK_SL.ONK_T.Value;
                            if (sl31.ONK_SL.ONK_N.HasValue)
                                SL.ONK_SL.ONK_N = sl31.ONK_SL.ONK_N.Value;
                            if (sl31.ONK_SL.ONK_M.HasValue)
                                SL.ONK_SL.ONK_M = sl31.ONK_SL.ONK_M.Value;
                            SL.ONK_SL.MTSTZ = sl31.ONK_SL.MTSTZ;
                            SL.ONK_SL.DS1_T = sl31.ONK_SL.DS1_T;


                            foreach (var d in sl31.ONK_SL.B_DIAG)
                            {
                                SL.ONK_SL.B_DIAG.Add(new EntityMP_V3.B_DIAG() { DIAG_CODE = d.DIAG_CODE, DIAG_RSLT = d.DIAG_RSLT.HasValue ? d.DIAG_RSLT.Value : 0, DIAG_TIP = d.DIAG_TIP, SLUCH_ID = d.SLUCH_ID });
                            }

                            foreach (var d in sl31.ONK_SL.B_PROT)
                            {
                                SL.ONK_SL.B_PROT.Add(new EntityMP_V3.B_PROT() { D_PROT = d.D_PROT, PROT = d.PROT });
                            }


                        }



                        SL.PACIENT_ID = sl31.PACIENT_ID;
                        SL.PODR = sl31.PODR;
                        SL.PROFIL = sl31.PROFIL;
                        SL.PROFIL_K = sl31.PROFIL_K;
                        SL.PRVS = sl31.PRVS;
                        SL.PR_D_N = sl31.PR_D_N;
                        SL.P_CEL = sl31.P_CEL;
                        SL.P_PER = sl31.P_PER;
                        SL.REAB = sl31.REAB;

                        SL.SLUCH_ID = sl31.SLUCH_ID;
                        SL.SLUCH_Z_ID = sl31.SLUCH_Z_ID;
                        SL.SL_ID = sl31.SL_ID;
                        SL.SUM_M = sl31.SUM_M;
                        SL.SUM_MP = sl31.SUM_MP;
                        SL.TAL_D = sl31.TAL_D;
                        SL.TAL_NUM = sl31.TAL_NUM;
                        SL.TAL_P = sl31.TAL_P;
                        SL.TARIF = sl31.TARIF;

                        SL.VERS_SPEC = sl31.VERS_SPEC;
                        SL.VID_HMP = sl31.VID_HMP;




                        var onk_usl_c = 0;
                        var onk_napr_c = 0;
                        foreach (var us2 in sl31.USL)
                        {
                            EntityMP_V3.USL us = new EntityMP_V3.USL();
                            us.CODE_MD = us2.CODE_MD;
                            us.CODE_USL = us2.CODE_USL;
                            us.COMENTU = us2.COMENTU;
                            us.DATE_IN = us2.DATE_IN;
                            us.DATE_OUT = us2.DATE_OUT;
                            us.DET = us2.DET;
                            us.DS = us2.DS;
                            us.IDSERV = us2.IDSERV;
                            us.KOL_USL = us2.KOL_USL;
                            us.LPU = us2.LPU;
                            us.LPU_1 = us2.LPU_1;
                            us.PODR = us2.PODR;
                            us.PROFIL = us2.PROFIL;
                            us.PRVS = us2.PRVS??0;
                            us.SUMV_USL = us2.SUMV_USL;
                            us.TARIF = us2.TARIF;
                            us.VID_VME = us2.VID_VME;
                            us.USL_ID = us2.USL_ID;
                            us.SLUCH_ID = us2.SLUCH_ID;
                            us.P_OTK = us2.P_OTK;
                            us.NPL = us2.NPL;
                            us.SUMP_USL = us2.SUMP_USL;

                            if (sl31.ONK_SL != null)
                            {
                                foreach (var onk_us in sl31.ONK_SL.ONK_USL.Where(x => x.IDSERV == us.IDSERV || x.USL_ID == us.USL_ID))
                                {
                                    us.ONK_USL = new EntityMP_V3.ONK_USL();
                                    us.ONK_USL.HIR_TIP = onk_us.HIR_TIP;
                                    us.ONK_USL.LEK_TIP_L = onk_us.LEK_TIP_L;
                                    us.ONK_USL.LEK_TIP_V = onk_us.LEK_TIP_L;
                                    us.ONK_USL.LUCH_TIP = onk_us.LUCH_TIP;
                                    us.ONK_USL.USL_TIP = onk_us.USL_TIP;
                                    var cons = sl31.CONS.FirstOrDefault(x => x.IDSERV == us.IDSERV);
                                    if (cons != null)
                                        us.ONK_USL.PR_CONS = cons.PR_CONS;
                                    onk_usl_c++;
                                }
                            }

                            foreach (var nap in sl31.NAPR.Where(x => x.IDSERV == us.IDSERV || x.USL_ID == us.USL_ID))
                            {
                                us.NAPR.Add(new EntityMP_V3.NAPR() { MET_ISSL = nap.MET_ISSL, NAPR_DATE = nap.NAPR_DATE, NAPR_USL = nap.NAPR_USL, NAPR_V = nap.NAPR_V });
                                onk_napr_c++;
                            }

                            SL.USL.Add(us);

                        }
                        if (SL.ONK_SL != null)
                            if (sl31.ONK_SL.ONK_USL.Count != onk_usl_c)
                                throw new Exception(
                                    $"Не полное внесение ONK_USL при миграции из V31 в V3 для случая {SL.SLUCH_ID} внесено {onk_usl_c} из {sl31.ONK_SL.ONK_USL.Count}");
                        if (sl31.NAPR.Count != onk_napr_c)
                            throw new Exception(
                                $"Не полное внесение NAPR при миграции из V31 в V3  для случая {SL.SLUCH_ID} внесено {onk_napr_c} из {sl31.NAPR.Count}");

                        int sank_c = 0;
                        foreach (var san2 in z_sl2.SANK)
                        {
                            if (san2.SL_ID.Count == 1)
                            {
                               var sl =  Z_SL.SL.FirstOrDefault(x => x.SL_ID.ToString() == san2.SL_ID[0]);
                                if(sl!=null)
                                {
                                    sl.SANK.Add(san2);
                                    sank_c++;
                                }
                            }
                            SL.SANK.Add(san2);
                        }
                        if (z_sl2.SANK.Count != sank_c)
                            throw new Exception($"Не полное внесение SANK при миграции из V31 в V3  для случая {SL.SLUCH_ID} внесено {sank_c} из {z_sl2.SANK.Count}");

                    }
                }
            }

            return l3;
        }


    }
}

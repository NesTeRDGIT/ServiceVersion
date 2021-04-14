using System;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Collections;
using System.Xml.Schema;
using System.ComponentModel;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace ServiceLoaderMedpomData.EntityMP_V2
{



    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
  
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(Namespace = "", IsNullable = false)]
    public partial class ZL_LIST
    {

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

        #region Private fields
        private ZGLV _zGLV;

        private SCHET _sCHET;

        private List<ZAP> _zAP;
        #endregion

        public ZL_LIST()
        {
            this._zAP = new List<ZAP>();
            this._sCHET = new SCHET();
            this._zGLV = new ZGLV();
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public ZGLV ZGLV
        {
            get
            {
                return this._zGLV;
            }
            set
            {
                this._zGLV = value;
            }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public SCHET SCHET
        {
            get
            {
                return this._sCHET;
            }
            set
            {
                this._sCHET = value;
            }
        }

        [XmlElement("ZAP", Form = XmlSchemaForm.Unqualified)]
        public List<ZAP> ZAP
        {
            get
            {
                return this._zAP;
            }
            set
            {
                this._zAP = value;
            }
        }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class ZGLV
    {

        #region Private fields
        private string _vERSION;

        private DateTime _dATA;

        private string _fILENAME;
        #endregion

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string VERSION
        {
            get
            {
                return this._vERSION;
            }
            set
            {
                this._vERSION = value;
            }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATA
        {
            get
            {
                return this._dATA;
            }
            set
            {
                this._dATA = value;
            }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string FILENAME
        {
            get
            {
                return this._fILENAME;
            }
            set
            {
                this._fILENAME = value;
            }
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SD_Z { get; set; }
       
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class SCHET
    {
        

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

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string PLAT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SUMMAV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string COMENTS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SUMMAP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? SANK_MEK { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SANK_MEE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SANK_EKMP { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = false)]
        public string DISP { get; set; }

    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class ZAP
    {
        public ZAP()
        {
            this.SLUCH = new List<SLUCH>();
            this.PACIENT = new PACIENT();
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal N_ZAP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal PR_NOV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public PACIENT PACIENT { get; set; }

        [XmlElement("SLUCH", Form = XmlSchemaForm.Unqualified)]
        public List<SLUCH> SLUCH { get; set; }
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class PACIENT
    {


        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string ID_PAC { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal VPOLIS { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string SPOLIS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NPOLIS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string ST_OKATO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string SMO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string SMO_OGRN { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string SMO_OK { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string SMO_NAM { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? MSE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NOVOR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? VNOV_D { get; set; }
       
    }

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public class SLUCH
    {

        public SLUCH()
        {
            this.USL = new List<USL>();
            this.SANK = new List<EntityMP_V31.SANK>();
            this.OS_SLUCH = new List<string>();
            this.CODE_MES1 = new List<string>();
            this.VNOV_M = new List<string>();
            this.DS3 = new List<string>();
            this.DS2 = new List<string>();
        }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal IDCASE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? USL_OK { get; set; }
        

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal VIDPOM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified,IsNullable =true)]
        public decimal? FOR_POM { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string VID_HMP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? METOD_HMP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NPR_MO { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? EXTR { get; set; }       

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LPU { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LPU_1 { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? VBR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PODR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PROFIL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? DET { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true, DataType ="date")]
        public DateTime? TAL_D { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true, DataType = "date")]
        public DateTime? TAL_P { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string NHISTORY { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? P_OTK { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_2 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string DS0 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string DS1 { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? DS1_PR { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public List<EntityMP_V3.DS2_N> DS2_N { get; set; } = new List<EntityMP_V3.DS2_N>();

        [XmlElement("DS2", Form = XmlSchemaForm.Unqualified)]
        public List<string> DS2 { get; set; }

        [XmlElement("DS3", Form = XmlSchemaForm.Unqualified)]
        public List<string> DS3 { get; set; }

        [XmlElement("VNOV_M", Form = XmlSchemaForm.Unqualified)]
        public List<string> VNOV_M { get; set; }

        [XmlElement("CODE_MES1", Form = XmlSchemaForm.Unqualified)]
        public List<string> CODE_MES1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string CODE_MES2 { get; set; }


        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? RSLT_D { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public List<decimal> NAZR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public List<decimal> NAZ_SP { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public List<decimal> NAZ_V { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public List<decimal> NAZ_PMP { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public List<decimal> NAZ_PK { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal? PR_D_N { get; set; }



        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable=true)]
        public decimal? RSLT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? ISHOD { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified,IsNullable =true)]
        public decimal? PRVS { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string VERS_SPEC { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string IDDOKT { get; set; }

        [XmlElement("OS_SLUCH", Form = XmlSchemaForm.Unqualified)]
        public List<string> OS_SLUCH { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal IDSP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? ED_COL { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? TARIF { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SUMV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? OPLATA { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? SUMP { get; set; }
        

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? SANK_IT { get; set; }

        [XmlElement("SANK", Form = XmlSchemaForm.Unqualified)]
        public List<EntityMP_V31.SANK> SANK { get; set; }

        [XmlElement("USL", Form = XmlSchemaForm.Unqualified)]
        public List<USL> USL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string COMENTSL { get; set; }
    }


    
 /*   public partial class SANK
    {

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string S_CODE { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_SUM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_TIP { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_OSN { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string S_COM { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal S_IST { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_YEAR { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_MONTH { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? S_PLAN { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_TEM { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? S_FINE { get; set; }


    }*/

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]    
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    public partial class USL
    {
        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string IDSERV { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LPU { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string LPU_1 { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? PODR { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? PROFIL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string VID_VME { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? DET { get; set; }
        
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_IN { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "date")]
        public DateTime DATE_OUT { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string DS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string CODE_USL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable =true)]
        public decimal? KOL_USL { get; set; }
        [XmlElement(Form = XmlSchemaForm.Unqualified, IsNullable = true)]
        public decimal? TARIF { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal SUMV_USL { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public decimal PRVS { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string CODE_MD { get; set; }

        [XmlElement(Form = XmlSchemaForm.Unqualified)]
        public string COMENTU { get; set; }
    }
}

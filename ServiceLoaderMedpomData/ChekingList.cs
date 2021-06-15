using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
namespace ServiceLoaderMedpomData
{
   
    [Serializable]
    [DataContract]
    public enum CustomDbType
    {
        [EnumMember]
        NVarchar2 = 0,
        [EnumMember]
        Varchar2 = 1,
        [EnumMember]
        Int32 = 2,
        [EnumMember]
        Decimal = 3,
        [EnumMember]
        Date = 4
    }
    [Serializable]
    [DataContract]
    public enum TypeParamValue
    {
        [EnumMember]
        value = 0 ,
        [EnumMember]
        TABLE_NAME_ZGLV = 1,
        [EnumMember]
        TABLE_NAME_SCHET = 2,
        [EnumMember]
        TABLE_NAME_ZAP = 3,
        [EnumMember]
        TABLE_NAME_PACIENT = 4,
        [EnumMember]
        TABLE_NAME_SLUCH= 5,
        [EnumMember]
        TABLE_NAME_USL=6,
        [EnumMember]
        TABLE_NAME_SANK=7,
        [EnumMember]
        TABLE_NAME_L_ZGLV=8,
        [EnumMember]
        TABLE_NAME_L_PERS=9,
        [EnumMember]
        CurrMonth=10,
        [EnumMember]
        CurrYear = 11,
        [EnumMember]
        TABLE_NAME_NAZR = 12,
        [EnumMember]
        TABLE_NAME_DS2_N = 13

    }
    [Serializable]
    [DataContract]
    public enum StateExistProcedure
    {
        [EnumMember]
        Exist = 0,
        [EnumMember]
        NotExcist = 1,
        [EnumMember]
        Unknow = 2
    }

    [DataContract]
    [Serializable]
    public class OrclParam
    {
        [DataMember]
        public string Name { get; set; } = "";
        [DataMember]
        public CustomDbType Type { get; set; } = CustomDbType.NVarchar2;
        [DataMember]
        public string value { get; set; } =  "";
        [DataMember]
        public TypeParamValue ValueType { get; set; } = TypeParamValue.value;
        [DataMember]
        public string Comment { get; set; } = "";
        
        public OrclParam()
        {
            Type = CustomDbType.NVarchar2;
            value = "";
            ValueType = TypeParamValue.value;
            Comment = "";
            Name = "";
        }
        public OrclParam(OrclParam clone)
        {
            Name = clone.Name;
            Type = clone.Type;
            value = clone.value;
            Comment = clone.Comment;
            ValueType = clone.ValueType;
        }
        public OrclParam(string _Name, CustomDbType _Type, TypeParamValue _ValueType,string _value, string _Comment)
        {
            Name = _Name;
            Type = _Type;
            value = _value;
            Comment = _Comment;
            ValueType = _ValueType;
        }
        public static TypeParamValue TypeParamValueFromStr(string str)
        {
            if (str == TypeParamValue.value.ToString()) return TypeParamValue.value;
            if (str == TypeParamValue.TABLE_NAME_L_PERS.ToString()) return TypeParamValue.TABLE_NAME_L_PERS;
            if (str == TypeParamValue.TABLE_NAME_ZGLV.ToString()) return TypeParamValue.TABLE_NAME_ZGLV;
            if (str == TypeParamValue.TABLE_NAME_ZAP.ToString()) return TypeParamValue.TABLE_NAME_ZAP;
            if (str == TypeParamValue.TABLE_NAME_USL.ToString()) return TypeParamValue.TABLE_NAME_USL;
            if (str == TypeParamValue.TABLE_NAME_SLUCH.ToString()) return TypeParamValue.TABLE_NAME_SLUCH;
            if (str == TypeParamValue.TABLE_NAME_SCHET.ToString()) return TypeParamValue.TABLE_NAME_SCHET;
            if (str == TypeParamValue.TABLE_NAME_SANK.ToString()) return TypeParamValue.TABLE_NAME_SANK;
            if (str == TypeParamValue.TABLE_NAME_PACIENT.ToString()) return TypeParamValue.TABLE_NAME_PACIENT;
            if (str == TypeParamValue.TABLE_NAME_L_ZGLV.ToString()) return TypeParamValue.TABLE_NAME_L_ZGLV;
            if (str == TypeParamValue.TABLE_NAME_NAZR.ToString()) return TypeParamValue.TABLE_NAME_NAZR;
            if (str == TypeParamValue.TABLE_NAME_DS2_N.ToString()) return TypeParamValue.TABLE_NAME_DS2_N;
            if (str == TypeParamValue.CurrMonth.ToString()) return TypeParamValue.CurrMonth;
            if (str == TypeParamValue.CurrYear.ToString()) return TypeParamValue.CurrYear;
            return TypeParamValue.value;

        }

    }

    [DataContract]
    [Serializable]
    public class OrclProcedure
    {
        public string FULL_NAME => $"{NAME_ERR}({NAME_PROC})";
        [DataMember]
        public string NAME_PROC { get; set; }= "";
        [DataMember]
        public string NAME_ERR { get; set; } = "" ;
        [DataMember]
        public string TYPE_ERR { get; set; } = "" ;
        [DataMember]
        public string Comment { get; set; } = "";
        [DataMember]
        public bool STATE { get; set; } = false;
        [DataMember]
        public StateExistProcedure Excist { get; set; } = StateExistProcedure.Unknow;
        [DataMember]
        public List<OrclParam> listParam { get; set; }

        public OrclProcedure(string _NAME_ERR, string _NAME_PROC, bool _STATE, List<OrclParam> _listParam)
        {
            NAME_PROC = _NAME_PROC;
            NAME_ERR = _NAME_ERR;
            TYPE_ERR = "Ошибка";
            listParam = _listParam;
            STATE = _STATE;
            Excist = StateExistProcedure.Unknow;
        }

        public OrclProcedure(OrclProcedure Clone)
        {
            NAME_PROC = Clone.NAME_PROC;
            NAME_ERR = Clone.NAME_ERR;
            TYPE_ERR = Clone.TYPE_ERR;
            STATE = Clone.STATE;
            Excist = StateExistProcedure.Unknow;
            listParam = new List<OrclParam>();
            if (Clone.listParam != null)
                foreach (var par in Clone.listParam)
                {
                    listParam.Add(new OrclParam(par.Name, par.Type, par.ValueType, par.value, par.Comment));
                }
        }
        public void CopyFrom(OrclProcedure Clone)
        {
            NAME_PROC = Clone.NAME_PROC;
            NAME_ERR = Clone.NAME_ERR;
            TYPE_ERR = Clone.TYPE_ERR;
            STATE = Clone.STATE;
            Excist = StateExistProcedure.Unknow;
            listParam = new List<OrclParam>();
            if (Clone.listParam != null)
                foreach (var par in Clone.listParam)
                {
                    listParam.Add(new OrclParam(par.Name, par.Type, par.ValueType, par.value, par.Comment));
                }
        }


        public OrclProcedure()
        {
            listParam = new List<OrclParam>();
            Excist = StateExistProcedure.Unknow; 
        }
     

  
    }
    [DataContract]
    public class CheckingList
    {
        private const string PERS = "PERS";
        private const string L_ZGLV = "L_ZGLV";
        private const string PACIENT = "PACIENT";
        private const string SANK = "SANK";
        private const string SCHET = "SCHET";
        private const string SLUCH = "SLUCH";
        private const string USL = "USL";
        private const string ZAP = "ZAP";
        private const string ZGLV = "ZGLV";


        public string GetKey(TableName tn)
        {
            switch (tn)
            {
                case TableName.L_PERS:
                    return PERS;
                case TableName.L_ZGLV:
                    return L_ZGLV;
                case TableName.PACIENT:
                    return PACIENT;
                case TableName.SANK:
                    return SANK;
                case TableName.SCHET:
                    return SCHET;
                case TableName.SLUCH:
                    return SLUCH;
                case TableName.USL:
                    return USL;
                case TableName.ZAP:
                    return ZAP;
                case TableName.ZGLV:
                    return ZGLV;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tn), tn, null);
            }
        }

        public TableName GetKey(string tn)
        {
            switch (tn)
            {
                case PERS:
                    return TableName.L_PERS;
                case L_ZGLV :
                    return TableName.L_ZGLV;
                case PACIENT:
                    return TableName.PACIENT;
                case SANK:
                    return TableName.SANK;
                case SCHET:
                    return TableName.SCHET;
                case SLUCH:
                    return TableName.SLUCH;
                case USL:
                    return TableName.USL;
                case ZAP:
                    return TableName.ZAP;
                case ZGLV:
                    return TableName.ZGLV;
                default:
                    throw new ArgumentOutOfRangeException(nameof(tn), tn, null);
            }
        }

        [DataMember]
        List<OrclProcedure> PERS_CHEK;
        [DataMember]
        List<OrclProcedure> L_ZGLV_CHEK;
        [DataMember]
        List<OrclProcedure> PACIENT_CHEK;
        [DataMember]
        List<OrclProcedure> SANK_CHEK;
        [DataMember]
        List<OrclProcedure> SCHET_CHEK;
        [DataMember]
        List<OrclProcedure> SLUCH_CHEK;
        [DataMember]
        List<OrclProcedure> USL_CHEK;
        [DataMember]
        List<OrclProcedure> ZAP_CHEK;
        [DataMember]
        List<OrclProcedure> ZGLV_CHEK;

        public CheckingList()
        {
            PERS_CHEK = new List<OrclProcedure>();
            L_ZGLV_CHEK = new List<OrclProcedure>();
            PACIENT_CHEK = new List<OrclProcedure>();
            SANK_CHEK = new List<OrclProcedure>();
            SCHET_CHEK = new List<OrclProcedure>();
            SLUCH_CHEK = new List<OrclProcedure>();
            USL_CHEK = new List<OrclProcedure>();
            ZAP_CHEK = new List<OrclProcedure>();
            ZGLV_CHEK = new List<OrclProcedure>(); 
            
        }

        public List<OrclProcedure>[] Collection()
        {
            var rez = new List<OrclProcedure>[9];
            rez[0] = ZGLV_CHEK;
            rez[1] = SCHET_CHEK;
            rez[2] = ZAP_CHEK;
            rez[3] = PACIENT_CHEK;
            rez[4] = SLUCH_CHEK;
            rez[5] = USL_CHEK;
            rez[6] = SANK_CHEK;
            rez[7] = L_ZGLV_CHEK;
            rez[8] = PERS_CHEK;

            return rez;
        }

        public List<TableName> GeTableNames()
        {
            return new List<TableName> { TableName.ZGLV , TableName.SCHET, TableName.ZAP, TableName.PACIENT, TableName.SLUCH, TableName.USL, TableName.L_ZGLV,TableName.L_PERS, TableName.SANK };
        }
        public void Add(TableName Tname, string NAME_ERR, string NAME_PROC, bool STATE, List<OrclParam> listParam)
        {
            switch (Tname)
            {
                case TableName.L_PERS:
                    PERS_CHEK.Add(new OrclProcedure(NAME_ERR,NAME_PROC,STATE, listParam));
                    break;
                case TableName.L_ZGLV:
                    L_ZGLV_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
                case TableName.PACIENT:
                    PACIENT_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
                case TableName.SANK:
                    SANK_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
                case TableName.SCHET:
                    SCHET_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
                case TableName.SLUCH:
                    SLUCH_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
                case TableName.USL:
                    USL_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
                case TableName.ZAP:
                    ZAP_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
                case TableName.ZGLV:
                    ZGLV_CHEK.Add(new OrclProcedure(NAME_ERR, NAME_PROC, STATE, listParam));
                    break;
            }
        }        
        public void Add(TableName Tname, OrclProcedure proc)
        {
            switch (Tname)
            {
                case TableName.L_PERS:
                    PERS_CHEK.Add(proc);
                    break;
                case TableName.L_ZGLV:
                    L_ZGLV_CHEK.Add(proc);
                    break;
                case TableName.PACIENT:
                    PACIENT_CHEK.Add(proc);
                    break;
                case TableName.SANK:
                    SANK_CHEK.Add(proc);
                    break;
                case TableName.SCHET:
                    SCHET_CHEK.Add(proc);
                    break;
                case TableName.SLUCH:
                    SLUCH_CHEK.Add(proc);
                    break;
                case TableName.USL:
                    USL_CHEK.Add(proc);
                    break;
                case TableName.ZAP:
                    ZAP_CHEK.Add(proc);
                    break;
                case TableName.ZGLV:
                    ZGLV_CHEK.Add(proc);
                    break;
            }
        }

        public void RemoveAt(TableName Tname, int index)
        {
            switch (Tname)
            {
                case TableName.L_PERS:
                    PERS_CHEK.RemoveAt(index);
                    break;
                case TableName.L_ZGLV:
                    L_ZGLV_CHEK.RemoveAt(index);
                    break;
                case TableName.PACIENT:
                    PACIENT_CHEK.RemoveAt(index);
                    break;
                case TableName.SANK:
                    SANK_CHEK.RemoveAt(index);
                    break;
                case TableName.SCHET:
                    SCHET_CHEK.RemoveAt(index);
                    break;
                case TableName.SLUCH:
                    SLUCH_CHEK.RemoveAt(index);
                    break;
                case TableName.USL:
                    USL_CHEK.RemoveAt(index);
                    break;
                case TableName.ZAP:
                    ZAP_CHEK.RemoveAt(index);
                    break;
                case TableName.ZGLV:
                    ZGLV_CHEK.RemoveAt(index);
                    break;
            }
        }
        public OrclProcedure this[TableName name,int index]
        {
            get
            {
                switch (name)
                {
                    case TableName.L_PERS:
                        return PERS_CHEK[index];
                        
                    case TableName.L_ZGLV:
                        return L_ZGLV_CHEK[index];
                        
                    case TableName.PACIENT:
                        return PACIENT_CHEK[index];;
                        
                    case TableName.SANK:
                        return SANK_CHEK[index];;
                        
                    case TableName.SCHET:
                        return SCHET_CHEK[index];;
                        
                    case TableName.SLUCH:
                        return SLUCH_CHEK[index];;
                        
                    case TableName.USL:
                        return USL_CHEK[index];;
                        
                    case TableName.ZAP:
                        return ZAP_CHEK[index];;
                        
                    case TableName.ZGLV:
                        return ZGLV_CHEK[index];;
                        
                    default :
                        return null;
                }
                
            }
            set
            {
                switch (name)
                {
                    case TableName.L_PERS:
                        PERS_CHEK[index] = value;
                        break;
                    case TableName.L_ZGLV:
                        L_ZGLV_CHEK[index] = value;
                        break;
                    case TableName.PACIENT:
                        PACIENT_CHEK[index] = value;
                        break;
                    case TableName.SANK:
                        SANK_CHEK[index] = value;
                        break;
                    case TableName.SCHET:
                        SCHET_CHEK[index] = value;
                        break;
                    case TableName.SLUCH:
                        SLUCH_CHEK[index] = value;
                        break;
                    case TableName.USL:
                        USL_CHEK[index] = value;
                        break;
                    case TableName.ZAP:
                        ZAP_CHEK[index] = value;
                        break;
                    case TableName.ZGLV:
                        ZGLV_CHEK[index] = value;
                        break;
                    default:
                        break;
                }
            }
        }
        public List<OrclProcedure>  this[TableName name]
        {
            get
            {
                switch (name)
                {
                    case TableName.L_PERS:
                        return PERS_CHEK;
                        
                    case TableName.L_ZGLV:
                        return L_ZGLV_CHEK;
                        
                    case TableName.PACIENT:
                        return PACIENT_CHEK; ;
                        
                    case TableName.SANK:
                        return SANK_CHEK; ;
                        
                    case TableName.SCHET:
                        return SCHET_CHEK; ;
                        
                    case TableName.SLUCH:
                        return SLUCH_CHEK; ;
                        
                    case TableName.USL:
                        return USL_CHEK; ;
                        
                    case TableName.ZAP:
                        return ZAP_CHEK; ;
                        
                    case TableName.ZGLV:
                        return ZGLV_CHEK; ;
                        
                    default:
                        return null;
                }

            }
            set
            {
                switch (name)
                {
                    case TableName.L_PERS:
                        PERS_CHEK = value;
                        break;
                    case TableName.L_ZGLV:
                        L_ZGLV_CHEK = value;
                        break;
                    case TableName.PACIENT:
                        PACIENT_CHEK = value;
                        break;
                    case TableName.SANK:
                        SANK_CHEK = value;
                        break;
                    case TableName.SCHET:
                        SCHET_CHEK = value;
                        break;
                    case TableName.SLUCH:
                        SLUCH_CHEK = value;
                        break;
                    case TableName.USL:
                        USL_CHEK = value;
                        break;
                    case TableName.ZAP:
                        ZAP_CHEK = value;
                        break;
                    case TableName.ZGLV:
                        ZGLV_CHEK = value;
                        break;
                    default:
                        break;
                }
            }
        }
        public int Count(TableName name)
        {
            switch (name)
            {
                case TableName.L_PERS:
                    return PERS_CHEK.Count;
                    
                case TableName.L_ZGLV:
                    return L_ZGLV_CHEK.Count;
                    
                case TableName.PACIENT:
                    return PACIENT_CHEK.Count;
                    
                case TableName.SANK:
                    return SANK_CHEK.Count;
                    
                case TableName.SCHET:
                    return SCHET_CHEK.Count;
                    
                case TableName.SLUCH:
                    return SLUCH_CHEK.Count;
                    
                case TableName.USL:
                    return USL_CHEK.Count;
                    
                case TableName.ZAP:
                    return ZAP_CHEK.Count;
                    
                case TableName.ZGLV:
                    return ZGLV_CHEK.Count;
                    
                default:
                    return 0;
            }
        }
       
      

        public void SaveToFile(string path)
        {
            var mas = new List<List<OrclProcedure>>
            {
                PERS_CHEK,
                L_ZGLV_CHEK,
                PACIENT_CHEK,
                SANK_CHEK,
                SCHET_CHEK,
                SLUCH_CHEK,
                USL_CHEK,
                ZAP_CHEK,
                ZGLV_CHEK
            };
            var serializer = new XmlSerializer(typeof(List<List<OrclProcedure>>));
            using (var writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, mas);
                writer.Close();
            }
        }
        public bool LoadToFile(string path)
        {
           
            using (var fs = new FileStream(path, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(List<List<OrclProcedure>>));
                var mas = (List<List<OrclProcedure>>)serializer.Deserialize(fs);
                fs.Close();
                if (mas.Count < 9) return false;
                PERS_CHEK = mas[0];
                L_ZGLV_CHEK = mas[1];
                PACIENT_CHEK = mas[2];
                SANK_CHEK = mas[3];
                SCHET_CHEK = mas[4];
                SLUCH_CHEK = mas[5];
                USL_CHEK = mas[6];
                ZAP_CHEK = mas[7];
                ZGLV_CHEK = mas[8];
                return true;
            }
        }

      
        public void AddList(TableName name, List<OrclProcedure> list)
        {
            switch (name)
            {
                case TableName.L_PERS:
                    PERS_CHEK.AddRange(list);
                    break;
                case TableName.L_ZGLV:
                    L_ZGLV_CHEK.AddRange(list);
                    break;
                case TableName.PACIENT:
                    PACIENT_CHEK.AddRange(list);
                    break;
                case TableName.SANK:
                    SANK_CHEK.AddRange(list);
                    break;
                case TableName.SCHET:
                    SCHET_CHEK.AddRange(list);
                    break;
                case TableName.SLUCH:
                    SLUCH_CHEK.AddRange(list);
                    break;
                case TableName.USL:
                    USL_CHEK.AddRange(list);
                    break;
                case TableName.ZAP:
                    ZAP_CHEK.AddRange(list);
                    break;
                case TableName.ZGLV:
                    ZGLV_CHEK.AddRange(list);
                    break;
            }
        }


    
    }


    public interface IRepositoryCheckingList
    {
        List<OrclProcedure> GetProcedureFromPack(string namePack);
        CheckingList LoadFromBD();
        void SaveToBD(CheckingList checkingList);
        List<OrclParam> GetParam(string nameProc);
    }

    
}

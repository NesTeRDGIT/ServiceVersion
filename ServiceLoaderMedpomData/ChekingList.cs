using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.IO;
namespace ServiceLoaderMedpomData
{
    
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
        public OracleDbType Type { get; set; } = OracleDbType.NVarchar2;
        [DataMember]
        public string value { get; set; } =  "";
        [DataMember]
        public TypeParamValue ValueType { get; set; } = TypeParamValue.value;
        [DataMember]
        public string Comment { get; set; } = "";
        
        public OrclParam()
        {
            Type = OracleDbType.NVarchar2;
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
        public OrclParam(string _Name, OracleDbType _Type, TypeParamValue _ValueType,string _value, string _Comment)
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
        public static List<OrclParam> GetParam(string nameProc, string con)
        {
            string tmp = "";
            string packname = "";
            string procname = "";
            for (int i = 0; i < nameProc.Length; i++)
            {
                if (nameProc[i] == '.')
                {
                    packname = tmp;
                    tmp = "";
                    continue;
                }

                tmp += nameProc[i];
            }
            procname = tmp;
            OracleDataAdapter oda = new OracleDataAdapter(@"
            SELECT A.OBJECT_NAME, A.PACKAGE_NAME, A.ARGUMENT_NAME, A.DATA_TYPE, a.SEQUENCE
            FROM USER_ARGUMENTS A
            ,ALL_OBJECTS    O
            WHERE A.OBJECT_ID = O.OBJECT_ID
            AND O.OBJECT_NAME = '" + packname + @"'
            AND A.OBJECT_NAME = '" + procname + @"'
            order by a.SEQUENCE", new OracleConnection(con));
            DataTable tbl = new DataTable();
            oda.Fill(tbl);
            List<OrclParam> rez = new List<OrclParam>();
            foreach (DataRow row in tbl.Rows)
            {
                var param = new OrclParam();
                param.Name = row["ARGUMENT_NAME"].ToString();
                param.ValueType = TypeParamValue.value;
                param.Type = OrclProcedure.GetDataType(row["DATA_TYPE"].ToString());
                if (param.Name == "H_SLUCH_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_SLUCH;
                if (param.Name == "H_USL_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_USL;
                if (param.Name == "H_ZGLV_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_ZGLV;
                if (param.Name == "H_SCHET_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_SCHET;
                if (param.Name == "H_ZAP_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_ZAP;
                if (param.Name == "H_PACIENT_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_PACIENT;
                if (param.Name == "L_ZGLV_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_L_ZGLV;
                if (param.Name == "L_PERS_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_L_PERS;
                if (param.Name == "H_NAZR_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_NAZR;
                if (param.Name == "H_DS2_N_TBL_NAME")
                    param.ValueType = TypeParamValue.TABLE_NAME_DS2_N;

                if (procname.Substring(0, 4) == "EMPT")
                {
                    if (param.Name == "ERR")
                        param.value = "H_SLUCH_EMPT; ";
                    if (param.Name == "ERR_PRIM")
                        param.value = "Не заполнено обязательное поле ";
                    if (param.Name == "ERR_TYPE")
                        param.value = "Критическая ошибка. ";
                }
                rez.Add(param);

            }
            return rez;

        }
        public static List<OrclProcedure> GetProcedureFromPack(string namePack, string con)
        {
            var oda = new OracleDataAdapter(@"
                    SELECT procedure_name
                    FROM  USER_PROCEDURES O
                    WHERE  O.OBJECT_NAME = '"+namePack+"'", new OracleConnection(con));
            var tbl = new DataTable();
            oda.Fill(tbl);
           var rez = new List<OrclProcedure>();
            foreach (DataRow row in tbl.Rows)
            {
                var proc = new OrclProcedure();
                proc.Excist = StateExistProcedure.Unknow; 
                proc.NAME_ERR = row["procedure_name"].ToString();
                proc.NAME_PROC = namePack+"."+row["procedure_name"].ToString();
                proc.STATE = true;
                proc.listParam.AddRange(GetParam(proc.NAME_PROC, con));
                rez.Add(proc);

            }
            return rez;

        }
        public static OracleDbType GetDataType(string str)
        {
            if (str == OracleDbType.Varchar2.ToString()) return OracleDbType.Varchar2;
            if (str == OracleDbType.NVarchar2.ToString()) return OracleDbType.NVarchar2;
            if (str == OracleDbType.Int32.ToString()) return OracleDbType.Int32;
            if (str == OracleDbType.Decimal.ToString()) return OracleDbType.Decimal;
            if (str == OracleDbType.Date.ToString()) return OracleDbType.Date;
            if (str == "NUMBER") return OracleDbType.Decimal;
            return OracleDbType.Varchar2;
        }

  
    }
    [DataContract]
    public class ChekingList
    {
        const string PERS = "PERS";
        const string L_ZGLV = "L_ZGLV";
        const string PACIENT = "PACIENT";
        const string SANK = "SANK";
        const string SCHET = "SCHET";
        const string SLUCH = "SLUCH";
        const string USL = "USL";
        const string ZAP = "ZAP";
        const string ZGLV = "ZGLV";
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

        public ChekingList()
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
            List<OrclProcedure>[] rez = new List<OrclProcedure>[9];
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
        private string Connect ;
        public void LoadFromBD(string _Connect)
        {
            PERS_CHEK.Clear();
            L_ZGLV_CHEK.Clear();
            PACIENT_CHEK.Clear();
            SANK_CHEK.Clear();
            SCHET_CHEK.Clear();
            SLUCH_CHEK.Clear();
            USL_CHEK.Clear();
            ZAP_CHEK.Clear();
            ZGLV_CHEK.Clear();
            DataTable tblPROC = new DataTable();
            Connect = _Connect;

                OracleDataAdapter odaPROC = new OracleDataAdapter("select id, name_err, name_proc, type_err, owner, state from errorsproc", Connect);
                odaPROC.Fill(tblPROC);
                foreach (DataRow row in tblPROC.Rows)
                {
                    string tmo = row["OWNER"].ToString();
                    if (row["OWNER"].ToString() == PERS)
                    {
                        AddPers(row);
                    }
                    if (row["OWNER"].ToString() == L_ZGLV)
                    {
                        AddL_ZGLV(row);
                    }
                    if (row["OWNER"].ToString() == SANK)
                    {
                        AddSANK(row);
                    }
                    if (row["OWNER"].ToString() == SCHET)
                    {
                        AddSCHET(row);
                    }
                    if (row["OWNER"].ToString() == SLUCH)
                    {
                        AddSLUCH(row);
                    }
                    if (row["OWNER"].ToString() == USL)
                    {
                        AddUSL(row);
                    }
                    if (row["OWNER"].ToString() == ZAP)
                    {
                        AddZAP(row);
                    }
                    if (row["OWNER"].ToString() == ZGLV)
                    {
                        AddZGLV(row);
                    }
                    if (row["OWNER"].ToString() == PACIENT)
                    {
                        AddPACIENT(row);
                    }
                }
  
        }
        public void SaveToBD(string _Connect)
        {
            DataTable tblPROC = new DataTable();
            Connect = _Connect;
            OracleConnection conn = new OracleConnection(_Connect);

                OracleCommand insertProc = new OracleCommand();
                insertProc.Connection = conn;
                insertProc.CommandText = @"insert into errorsproc
                                                    (id, name_err, name_proc, type_err, owner, state)
                                                    values
                                                    (:v_id, :v_name_err, :v_name_proc, :v_type_err, :v_owner, :v_state)";
                insertProc.Parameters.Add("v_id", OracleDbType.Int32);
                insertProc.Parameters.Add("v_name_err", OracleDbType.NVarchar2);
                insertProc.Parameters.Add("v_name_proc", OracleDbType.NVarchar2);
                insertProc.Parameters.Add("v_type_err", OracleDbType.NVarchar2);
                insertProc.Parameters.Add("v_owner", OracleDbType.NVarchar2);
                insertProc.Parameters.Add("v_state", OracleDbType.Int32);

                OracleCommand insertParam = new OracleCommand();
                insertParam.Connection = conn;
                insertParam.CommandText = @"insert into param_proc
                                            (id_proc, name, datatype, comments, defaultvalue,TYPE_VALUE)
                                            values
                                            (:v_id_proc, :v_name, :v_datatype, :v_comments, :v_defaultvalue,:v_TYPE_VALUE)";
                insertParam.Parameters.Add("v_id_proc", OracleDbType.Int32);
                insertParam.Parameters.Add("v_name", OracleDbType.NVarchar2);
                insertParam.Parameters.Add("v_datatype", OracleDbType.NVarchar2);
                insertParam.Parameters.Add("v_comments", OracleDbType.NVarchar2);
                insertParam.Parameters.Add("v_defaultvalue", OracleDbType.NVarchar2);
                insertParam.Parameters.Add("v_TYPE_VALUE", OracleDbType.NVarchar2);


                OracleCommand trunc = new OracleCommand("truncate table param_proc");
                trunc.Connection = conn;
                conn.Open();
                trunc.ExecuteNonQuery();
                trunc.CommandText = "alter table param_proc disable constraint ID_PROC";
                trunc.ExecuteNonQuery();
                trunc.CommandText = "truncate table errorsproc";
                trunc.ExecuteNonQuery();
                trunc.CommandText = "alter table param_proc enable constraint ID_PROC";
                trunc.ExecuteNonQuery();

                int id_proc = 0;
                foreach (OrclProcedure proc in ZGLV_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = ZGLV;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                foreach (OrclProcedure proc in SCHET_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = SCHET;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                foreach (OrclProcedure proc in ZAP_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = ZAP;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();

                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }


                foreach (OrclProcedure proc in USL_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = USL;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                foreach (OrclProcedure proc in SLUCH_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = SLUCH;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                foreach (OrclProcedure proc in SANK_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = SANK;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                foreach (OrclProcedure proc in PACIENT_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = "Ошибка";
                    insertProc.Parameters["v_owner"].Value = PACIENT;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                foreach (OrclProcedure proc in L_ZGLV_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = L_ZGLV;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                foreach (OrclProcedure proc in PERS_CHEK)
                {
                    insertProc.Parameters["v_id"].Value = id_proc;
                    insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                    insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                    insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                    insertProc.Parameters["v_owner"].Value = PERS;
                    if (proc.STATE)
                        insertProc.Parameters["v_state"].Value = 1;
                    else
                        insertProc.Parameters["v_state"].Value = 0;
                    insertProc.ExecuteNonQuery();
                    foreach (OrclParam param in proc.listParam)
                    {
                        insertParam.Parameters["v_id_proc"].Value = id_proc;
                        insertParam.Parameters["v_name"].Value = param.Name;
                        insertParam.Parameters["v_datatype"].Value = param.Type.ToString();
                        insertParam.Parameters["v_comments"].Value = param.Comment;
                        insertParam.Parameters["v_defaultvalue"].Value = param.value;
                        insertParam.Parameters["v_TYPE_VALUE"].Value = param.ValueType.ToString();
                        insertParam.ExecuteNonQuery();
                    }
                    id_proc++;
                }

                conn.Close();              
        }
        public void SaveToFile(string path)
        {
            List<List<OrclProcedure>> mas = new List<List<OrclProcedure>>();
            mas.Add(PERS_CHEK);
            mas.Add(L_ZGLV_CHEK);
            mas.Add(PACIENT_CHEK);
            mas.Add(SANK_CHEK);
            mas.Add(SCHET_CHEK);
            mas.Add(SLUCH_CHEK);
            mas.Add(USL_CHEK);
            mas.Add(ZAP_CHEK);
            mas.Add(ZGLV_CHEK);
            XmlSerializer serializer = new XmlSerializer(typeof(List<List<OrclProcedure>>));
             TextWriter writer = new StreamWriter(path);
             serializer.Serialize(writer, mas);
             writer.Close();

        }
        public bool LoadToFile(string path)
        {
            FileStream fs = null;
            List<List<OrclProcedure>> mas = new List<List<OrclProcedure>>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<List<OrclProcedure>>));
                fs = new FileStream(path, FileMode.Open);
                mas = (List<List<OrclProcedure>>)serializer.Deserialize(fs);
                fs.Close();
                if (mas.Count < 9) return false;
                PERS_CHEK = mas[0];
                L_ZGLV_CHEK =  mas[1];
                PACIENT_CHEK =  mas[2];
                SANK_CHEK = mas[3];
                SCHET_CHEK = mas[4];
                SLUCH_CHEK = mas[5];
                USL_CHEK = mas[6];
                ZAP_CHEK = mas[7];
                ZGLV_CHEK = mas[8];
                return true;

        }
        private void AddPers(DataRow row)
        {
            DataTable param = GetParam( row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            PERS_CHEK.Add(proc);
        }
        private void AddL_ZGLV(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            L_ZGLV_CHEK.Add(proc);
        }
        private void AddPACIENT(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            PACIENT_CHEK.Add(proc);
        }
        private void AddSCHET(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            SCHET_CHEK.Add(proc);
        }
        private void AddSANK(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            SANK_CHEK.Add(proc);
        }
        private void AddSLUCH(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            SLUCH_CHEK.Add(proc);
        }
        private void AddUSL(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            USL_CHEK.Add(proc);
        }
        private void AddZAP(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            ZAP_CHEK.Add(proc);
        }
        private void AddZGLV(DataRow row)
        {
            DataTable param = GetParam(row["id"].ToString());
            List<OrclParam> list = new List<OrclParam>();
            foreach (DataRow rowParam in param.Rows)
            {
                OrclParam itemParam = new OrclParam();
                itemParam.Name = rowParam["name"].ToString();
                itemParam.Type = OrclProcedure.GetDataType(rowParam["datatype"].ToString());
                itemParam.Comment = rowParam["COMMENTS"].ToString();
                itemParam.value = rowParam["DEFAULTVALUE"].ToString();
                itemParam.ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString());
                list.Add(itemParam);
            }
            OrclProcedure proc = new OrclProcedure();
            proc.NAME_ERR = row["NAME_ERR"].ToString();
            proc.NAME_PROC = row["NAME_PROC"].ToString();
            proc.STATE = Convert.ToBoolean(row["STATE"]);
            proc.TYPE_ERR = row["TYPE_ERR"].ToString();
            proc.listParam = list;
            ZGLV_CHEK.Add(proc);
        }
        private DataTable GetParam(string id)
        {
            DataTable tblPARAM = new DataTable();
            OracleDataAdapter odaPARAM = new OracleDataAdapter("select * from param_proc where id_proc = " + id, Connect);
            odaPARAM.Fill(tblPARAM);
            return tblPARAM;
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
}

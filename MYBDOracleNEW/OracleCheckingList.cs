using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;

namespace MYBDOracle
{


    public class OracleCheckingList: IRepositoryCheckingList
    {
        private string ConnectionString { get; }
        public OracleCheckingList(string ConnectionString)
        {
            this.ConnectionString = ConnectionString;
        }
        public  List<OrclParam> GetParam(string nameProc)
        {
            var packname = "";
            var procname = nameProc;
            
            var splits = nameProc.Split('.');
            if (splits.Length == 2)
            {
                packname = splits[0];
                procname = splits[1];
            }

            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter(@"SELECT A.OBJECT_NAME, A.PACKAGE_NAME, A.ARGUMENT_NAME, A.DATA_TYPE, a.SEQUENCE FROM USER_ARGUMENTS A,ALL_OBJECTS O WHERE A.OBJECT_ID = O.OBJECT_ID AND O.OBJECT_NAME = :packname   AND A.OBJECT_NAME = :procname order by a.SEQUENCE", conn))
                {
                    oda.SelectCommand.Parameters.Add("packname", packname);
                    oda.SelectCommand.Parameters.Add("procname", procname);
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    var rez = new List<OrclParam>();
                    foreach (DataRow row in tbl.Rows)
                    {
                        var param = new OrclParam { Name = row["ARGUMENT_NAME"].ToString(), ValueType = TypeParamValue.value, Type = GetDbType(row["DATA_TYPE"].ToString()) };
                        switch (param.Name)
                        {
                            case "H_SLUCH_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_SLUCH;
                                break;
                            case "H_USL_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_USL;
                                break;
                            case "H_ZGLV_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_ZGLV;
                                break;
                            case "H_SCHET_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_SCHET;
                                break;
                            case "H_ZAP_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_ZAP;
                                break;
                            case "H_PACIENT_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_PACIENT;
                                break;
                            case "L_ZGLV_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_L_ZGLV;
                                break;
                            case "L_PERS_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_L_PERS;
                                break;
                            case "H_NAZR_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_NAZR;
                                break;
                            case "H_DS2_N_TBL_NAME":
                                param.ValueType = TypeParamValue.TABLE_NAME_DS2_N;
                                break;
                        }

                        if (procname.Substring(0, 4) == "EMPT")
                        {
                            switch (param.Name)
                            {
                                case "ERR":
                                    param.value = "H_SLUCH_EMPT; ";
                                    break;
                                case "ERR_PRIM":
                                    param.value = "Не заполнено обязательное поле ";
                                    break;
                                case "ERR_TYPE":
                                    param.value = "Критическая ошибка. ";
                                    break;
                            }
                        }
                        rez.Add(param);
                    }
                    return rez;
                }
            }
            
        }
        public List<OrclProcedure> GetProcedureFromPack(string namePack)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter($@"SELECT procedure_name FROM  USER_PROCEDURES O WHERE  O.OBJECT_NAME = :OBJECT_NAME", conn))
                {
                    oda.SelectCommand.Parameters.Add("OBJECT_NAME", namePack);
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    var rez = new List<OrclProcedure>();
                    foreach (DataRow row in tbl.Rows)
                    {
                        var proc = new OrclProcedure { Excist = StateExistProcedure.Unknow, NAME_ERR = row["procedure_name"].ToString(), NAME_PROC = $"{namePack}.{row["procedure_name"]}", STATE = true };
                        proc.listParam.AddRange(GetParam(proc.NAME_PROC));
                        rez.Add(proc);
                    }
                    return rez;
                }
            }
            
        }
        public static CustomDbType GetDbType(string str)
        {
            if (str == OracleDbType.Varchar2.ToString()) return CustomDbType.Varchar2;
            if (str == OracleDbType.NVarchar2.ToString()) return CustomDbType.NVarchar2;
            if (str == OracleDbType.Int32.ToString()) return CustomDbType.Int32;
            if (str == OracleDbType.Decimal.ToString()) return CustomDbType.Decimal;
            if (str == OracleDbType.Date.ToString()) return CustomDbType.Date;
            if (str == "NUMBER") return CustomDbType.Decimal;
            return CustomDbType.Varchar2;
        }
        public DataTable GetErrorsProc()
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var oda = new OracleDataAdapter(@"select id, name_err, name_proc, type_err, owner, state from errorsproc", conn))
                {
                    var tblPROC = new DataTable();
                    oda.Fill(tblPROC);
                    return tblPROC;
                }
            }
        }
     
        private void Trunc()
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var trunc = new OracleCommand("truncate table param_proc",conn))
                {
                    conn.Open();
                    trunc.ExecuteNonQuery();
                    trunc.CommandText = "alter table param_proc disable constraint ID_PROC";
                    trunc.ExecuteNonQuery();
                    trunc.CommandText = "truncate table errorsproc";
                    trunc.ExecuteNonQuery();
                    trunc.CommandText = "alter table param_proc enable constraint ID_PROC";
                    trunc.ExecuteNonQuery();
                }
            }
        }
        public CheckingList LoadFromBD()
        {
            var res = new CheckingList();
            
            using (var conn = new OracleConnection(ConnectionString))
            {
                var tblPROC = new DataTable();
                var tblParam = new DataTable();
                using (var oda = new OracleDataAdapter("select id, name_err, name_proc, type_err, owner, state from errorsproc", conn))
                {
                    oda.Fill(tblPROC);
                }
                using (var oda = new OracleDataAdapter("select * from param_proc", conn))
                {
                    oda.Fill(tblParam);
                }

                foreach (DataRow row in tblPROC.Rows)
                {
                    var OWNER = row["OWNER"].ToString();
                    var TPARAM = tblParam.Select($"id_proc = {row["id"]}");
                    var tn = res.GetKey(OWNER);

                    var listParam = TPARAM.Select(rowParam => new OrclParam
                        {
                            Name = rowParam["name"].ToString(),
                            Type = GetDbType(rowParam["datatype"].ToString()),
                            Comment = rowParam["COMMENTS"].ToString(),
                            value = rowParam["DEFAULTVALUE"].ToString(),
                            ValueType = OrclParam.TypeParamValueFromStr(rowParam["TYPE_VALUE"].ToString())
                        }).ToList();

                    var proc = new OrclProcedure
                    {
                        NAME_ERR = row["NAME_ERR"].ToString(),
                        NAME_PROC = row["NAME_PROC"].ToString(),
                        STATE = Convert.ToBoolean(row["STATE"]),
                        TYPE_ERR = row["TYPE_ERR"].ToString(),
                        listParam = listParam
                    };
                    res[tn].Add(proc);
                }
            }
            return res;
        }
        public void SaveToBD(CheckingList checkingList)
        {
            using (var conn = new OracleConnection(ConnectionString))
            {
                using (var insertProc = new OracleCommand("insert into errorsproc (id, name_err, name_proc, type_err, owner, state) values (:v_id, :v_name_err, :v_name_proc, :v_type_err, :v_owner, :v_state)", conn))
                {
                    insertProc.Parameters.Add("v_id", OracleDbType.Int32);
                    insertProc.Parameters.Add("v_name_err", OracleDbType.NVarchar2);
                    insertProc.Parameters.Add("v_name_proc", OracleDbType.NVarchar2);
                    insertProc.Parameters.Add("v_type_err", OracleDbType.NVarchar2);
                    insertProc.Parameters.Add("v_owner", OracleDbType.NVarchar2);
                    insertProc.Parameters.Add("v_state", OracleDbType.Int32);
                    using (var insertParam = new OracleCommand("insert into param_proc(id_proc, name, datatype, comments, defaultvalue, TYPE_VALUE) values (:v_id_proc, :v_name, :v_datatype, :v_comments, :v_defaultvalue,:v_TYPE_VALUE)", conn))
                    {
                        insertParam.Parameters.Add("v_id_proc", OracleDbType.Int32);
                        insertParam.Parameters.Add("v_name", OracleDbType.NVarchar2);
                        insertParam.Parameters.Add("v_datatype", OracleDbType.NVarchar2);
                        insertParam.Parameters.Add("v_comments", OracleDbType.NVarchar2);
                        insertParam.Parameters.Add("v_defaultvalue", OracleDbType.NVarchar2);
                        insertParam.Parameters.Add("v_TYPE_VALUE", OracleDbType.NVarchar2);

                        Trunc();
                        conn.Open();
                        var id_proc = 0;

                        foreach (var tableName in checkingList.GeTableNames())
                        {
                            foreach (var proc in checkingList[tableName])
                            {
                                insertProc.Parameters["v_id"].Value = id_proc;
                                insertProc.Parameters["v_name_err"].Value = proc.NAME_ERR;
                                insertProc.Parameters["v_name_proc"].Value = proc.NAME_PROC;
                                insertProc.Parameters["v_type_err"].Value = proc.TYPE_ERR;
                                insertProc.Parameters["v_owner"].Value = checkingList.GetKey(tableName);
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
                        }
                        conn.Close();

                    }

                }
            }
          

           

          
        }
    }

    public static class Ext
    {
        public static OracleDbType ToOracleDbType(this CustomDbType val)
        {
            switch (val)
            {
                case CustomDbType.NVarchar2:
                    return OracleDbType.NVarchar2;
                case CustomDbType.Varchar2:
                    return OracleDbType.Varchar2;
                case CustomDbType.Int32:
                    return OracleDbType.Int32;
                case CustomDbType.Decimal:
                    return OracleDbType.Decimal;
                case CustomDbType.Date:
                    return OracleDbType.Date;
                default:
                    throw new ArgumentOutOfRangeException(nameof(val), val, null);
            }
        }
    }

}

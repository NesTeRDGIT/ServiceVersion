using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MYBDOracle;

namespace MedpomService
{
    public partial class WcfInterface
    {
        public SettingsFolder GetSettingsFolder()
        {
            var set = new SettingsFolder
            {
                ErrorDir = AppConfig.Property.ErrorDir,
                ErrorMessageFile = AppConfig.Property.ErrorMessageFile,
                IncomingDir = AppConfig.Property.IncomingDir,
                InputDir = AppConfig.Property.InputDir,
                ProcessDir = AppConfig.Property.ProcessDir,
                TimePacketOpen = AppConfig.Property.TimePacketOpen,
                ISP = AppConfig.Property.ISP_NAME,
                AddDIRInERROR = AppConfig.Property.AddDIRInERROR
            };
            return set;
        }

        public void SettingsFolder(SettingsFolder set)
        {
            AppConfig.Property.ErrorDir = set.ErrorDir;
            AppConfig.Property.ErrorMessageFile = set.ErrorMessageFile;
            AppConfig.Property.IncomingDir = set.IncomingDir;
            AppConfig.Property.InputDir = set.InputDir;
            AppConfig.Property.ProcessDir = set.ProcessDir;
            AppConfig.Property.TimePacketOpen = set.TimePacketOpen;
            AppConfig.Property.AddDIRInERROR = set.AddDIRInERROR;
            AppConfig.Property.ISP_NAME = set.ISP;
        }

        public SettingConnect GetSettingConnect()
        {
            var sc = new SettingConnect
            {
                xml_l_zglv = AppConfig.Property.xml_l_zglv,
                ConnectingString = AppConfig.Property.ConnectionString,
                schemaOracle = AppConfig.Property.schemaOracle,
                xml_h_pacient = AppConfig.Property.xml_h_pacient,
                xml_h_sank_smo = AppConfig.Property.xml_h_sank,
                xml_h_schet = AppConfig.Property.xml_h_schet,
                xml_h_sluch = AppConfig.Property.xml_h_sluch,
                xml_h_usl = AppConfig.Property.xml_h_usl,
                xml_h_zap = AppConfig.Property.xml_h_zap,
                xml_h_zglv = AppConfig.Property.xml_h_zglv,
                xml_l_pers = AppConfig.Property.xml_l_pers,
                v_xml_error = AppConfig.Property.xml_errors,
                xml_h_nazr = AppConfig.Property.XML_H_NAZR,
                xml_h_ds2_n = AppConfig.Property.XML_H_DS2_N,
                xml_h_z_sluch = AppConfig.Property.xml_h_z_sluch,
                xml_h_kslp = AppConfig.Property.xml_h_kslp,
                xml_h_b_prot = AppConfig.Property.xml_h_b_prot,
                xml_h_b_diag = AppConfig.Property.xml_h_b_diag,
                xml_h_napr = AppConfig.Property.xml_h_napr,
                xml_h_cons = AppConfig.Property.xml_h_cons,
                xml_h_onk_usl = AppConfig.Property.xml_h_onk_usl,
                xml_h_lek_pr = AppConfig.Property.xml_h_lek_pr,
                xml_h_lek_pr_date_inj = AppConfig.Property.xml_h_date_inj,
                xml_h_sank_code_exp = AppConfig.Property.xml_h_sank_code_exp,
                xml_h_ds2 = AppConfig.Property.xml_h_ds2,
                xml_h_ds3 = AppConfig.Property.xml_h_ds3,
                xml_h_crit = AppConfig.Property.xml_h_crit,
                xml_h_mr_usl_n = AppConfig.Property.xml_h_mr_usl_n,
                xml_h_sl_lek_pr = AppConfig.Property.xml_h_sl_lek_pr,
                xml_h_med_dev = AppConfig.Property.xml_h_med_dev
            };
           
            return sc;
        }
        public void SettingConnect(SettingConnect set)
        {
            AppConfig.Property.ConnectionString = set.ConnectingString;
            AppConfig.Property.schemaOracle = set.schemaOracle;
            AppConfig.Property.xml_h_pacient = set.xml_h_pacient;
            AppConfig.Property.xml_h_sank = set.xml_h_sank_smo;
            AppConfig.Property.xml_h_schet = set.xml_h_schet;
            AppConfig.Property.xml_h_sluch = set.xml_h_sluch;
            AppConfig.Property.xml_h_usl = set.xml_h_usl;
            AppConfig.Property.xml_h_zap = set.xml_h_zap;
            AppConfig.Property.xml_h_zglv = set.xml_h_zglv;
            AppConfig.Property.xml_l_pers = set.xml_l_pers;
            AppConfig.Property.xml_l_zglv = set.xml_l_zglv;
            AppConfig.Property.xml_errors = set.v_xml_error;
            AppConfig.Property.XML_H_NAZR = set.xml_h_nazr;
            AppConfig.Property.XML_H_DS2_N = set.xml_h_ds2_n;
            AppConfig.Property.xml_h_b_prot = set.xml_h_b_prot;
            AppConfig.Property.xml_h_b_diag = set.xml_h_b_diag;
            AppConfig.Property.xml_h_napr = set.xml_h_napr;
            AppConfig.Property.xml_h_z_sluch = set.xml_h_z_sluch;
            AppConfig.Property.xml_h_kslp = set.xml_h_kslp;
            AppConfig.Property.xml_h_cons = set.xml_h_cons;
            AppConfig.Property.xml_h_onk_usl = set.xml_h_onk_usl;
            AppConfig.Property.xml_h_lek_pr = set.xml_h_lek_pr;
            AppConfig.Property.xml_h_date_inj = set.xml_h_lek_pr_date_inj;
            AppConfig.Property.xml_h_sank_code_exp = set.xml_h_sank_code_exp;
            AppConfig.Property.xml_h_ds2 = set.xml_h_ds2;
            AppConfig.Property.xml_h_ds3 = set.xml_h_ds3;
            AppConfig.Property.xml_h_crit = set.xml_h_crit;
            AppConfig.Property.xml_h_mr_usl_n = set.xml_h_mr_usl_n;
            AppConfig.Property.xml_h_sl_lek_pr = set.xml_h_sl_lek_pr;
            AppConfig.Property.xml_h_med_dev = set.xml_h_med_dev;

        }

        public BoolResult isConnect(string connectionstring)
        {
            var rez = new BoolResult();
            try
            {
                var con = new OracleConnection(connectionstring);
                con.Open();
                con.Close();
                rez.Result = true;
                rez.Exception = "";
            }
            catch (Exception ex)
            {
                rez.Result = false;
                rez.Exception = ex.Message;
            }
            return rez;
        }

        public TableResult GetTableServer(string OWNER)
        {
            var tr = new TableResult();
            try
            {
                var oda = new OracleDataAdapter($@"SELECT TABLE_NAME FROM ALL_TABLES where OWNER = '{OWNER.ToUpper()}' union all SELECT view_name FROM all_views where owner = '{OWNER.ToUpper()}'", new OracleConnection(AppConfig.Property.ConnectionString));
                var tbl = new DataTable("TABLE");
                oda.Fill(tbl);
                tr.Result = tbl;
            }
            catch (Exception ex)
            {
                tr.Result = null;
                tr.Exception = ex.Message;
            }
            return tr;
        }

        public void SaveProperty()
        {
            try
            {
                AddLog("Изменение конфигурации", LogType.Information);
                AppConfig.Save();

                var dir = Path.GetDirectoryName(PathEXE);

                SchemaCheck.SaveSchemaCollection($"{dir}\\schemaset.dat");
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при сохранении конфигурации: " + ex.Message, LogType.Error);
            }
        }
        public void LoadProperty()
        {
            AppConfig.Load();
            var dir = Path.GetDirectoryName(PathEXE);
            SchemaCheck.LoadSchemaCollection($"{dir}\\schemaset.dat");
        }

        public SchemaCollection GetSchemaCollection()
        {
            return SchemaCheck.GetSchemaCollection();
        }
     

        public void SettingSchemaCollection(SchemaCollection sc)
        {
            SchemaCheck.SetSchemaCollection(sc);
        }
        public void StopTimeAway(int index)
        {
            try
            {
                if (PacketQuery[index].Status == StatusFilePack.Open)
                {
                    PacketQuery[index].StopTime = true;
                }
                else
                {
                    throw new FaultException("Остановка времени возможно только при статусе Open");
                }
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public CheckingList GetCheckingList()
        {
            return processReestr.GetCheckingList();
        }

        public BoolResult SetCheckingList(CheckingList list)
        {
            var rez = new BoolResult();
            AddLog("Изменение конфигурации проверок", LogType.Information);
            try
            {
                if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backupChekingList")))
                {
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backupChekingList"));
                }
                var filename = $"List{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.clf";
                processReestr.GetCheckingList().SaveToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backupChekingList", filename));
                processReestr.SetCheckingList(list);
                processReestr.SaveCheckingList();

            }
            catch (Exception ex)
            {
                rez.Result = false;
                rez.Exception = ex.Message;
                AddLog($"Ошибка при сохранении списка проверок в БД: {ex.Message}", LogType.Error);
                return rez;

            }
            rez.Result = true;
            return rez;
        }


        public List<OrclProcedure> GetProcedureFromPack(string name)
        {
            return repositoryCheckingList.GetProcedureFromPack(name);
        }


        public List<OrclParam> GetParam(string name)
        {
            return repositoryCheckingList.GetParam(name);
        }
        public CheckingList ExecuteCheckAv(CheckingList check)
        {
            try
            {
                var con = new OracleConnection(AppConfig.Property.ConnectionString);
                con.Open();
                foreach (var l in check.Collection())
                {
                    for (var i = 0; i < l.Count; i++)
                    {
                        var proc = l[i];
                        var cmd = new OracleCommand();
                        cmd.Connection = con;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "begin " + proc.NAME_PROC + "(";

                        foreach (var par in proc.listParam)
                        {
                            cmd.CommandText += ":" + par.Name + ",";
                            if (par.ValueType == TypeParamValue.value)
                            {
                                if (par.Type == CustomDbType.Int32)
                                    cmd.Parameters.Add(par.Name, par.Type.ToOracleDbType(), Convert.ToInt32(par.value), ParameterDirection.Input);
                                else
                                    cmd.Parameters.Add(par.Name, par.Type.ToOracleDbType(), par.value, ParameterDirection.Input);
                            }
                            else
                            {
                                var name = "";
                                switch (par.ValueType)
                                {
                                    case TypeParamValue.TABLE_NAME_ZGLV: name = AppConfig.Property.xml_h_zglv; break;
                                    case TypeParamValue.TABLE_NAME_ZAP: name = AppConfig.Property.xml_h_zap; break;
                                    case TypeParamValue.TABLE_NAME_USL: name = AppConfig.Property.xml_h_usl; break;
                                    case TypeParamValue.TABLE_NAME_SLUCH: name = AppConfig.Property.xml_h_sluch; break;
                                    case TypeParamValue.TABLE_NAME_SCHET: name = AppConfig.Property.xml_h_schet; break;
                                    case TypeParamValue.TABLE_NAME_SANK: name = AppConfig.Property.xml_h_sank; break;
                                    case TypeParamValue.TABLE_NAME_PACIENT: name = AppConfig.Property.xml_h_pacient; break;
                                    case TypeParamValue.TABLE_NAME_L_ZGLV: name = AppConfig.Property.xml_l_zglv; break;
                                    case TypeParamValue.TABLE_NAME_L_PERS: name = AppConfig.Property.xml_l_pers; break;
                                    case TypeParamValue.CurrMonth: name = AppConfig.Property.OtchetDate.Month.ToString(); break;
                                    case TypeParamValue.CurrYear: name = AppConfig.Property.OtchetDate.Year.ToString(); break;
                                }


                                if (par.Type == CustomDbType.Int32)
                                    cmd.Parameters.Add(par.Name, par.Type.ToOracleDbType(), Convert.ToInt32(name), ParameterDirection.Input);
                                else
                                    cmd.Parameters.Add(par.Name, par.Type.ToOracleDbType(), name, ParameterDirection.Input);


                            }
                        }
                        if (cmd.CommandText[cmd.CommandText.Length - 1] == ',')
                            cmd.CommandText = cmd.CommandText.Remove(cmd.CommandText.Length - 1, 1);
                        cmd.CommandText += "); end;";
                        try
                        {
                            cmd.ExecuteScalar();
                            proc.Excist = StateExistProcedure.Exist;
                        }
                        catch (Exception ex)
                        {
                            proc.Comment = ex.Message;
                            proc.Excist = StateExistProcedure.NotExcist;
                            proc.STATE = false;
                        }

                    }
                }
                con.Close();
                return check;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при выполнении активных проверок: " + ex.Message, LogType.Error);
                return null;
            }
        }
        public BoolResult LoadCheckListFromBD()
        {
            var br = new BoolResult();
            try
            {
                processReestr.LoadCheckingList();
            }
            catch (Exception ex)
            {
                br.Result = false;
                br.Exception = ex.Message;
                return br;
            }
            br.Result = true;
            return br;
        }

        public void SetSettingTransfer(SettingTransfer st)
        {
            AppConfig.Property.xml_l_zglv_transfer = st.xml_l_zglv;
            AppConfig.Property.xml_l_pers_transfer = st.xml_l_pers;
            AppConfig.Property.xml_h_zglv_transfer = st.xml_h_zglv;
            AppConfig.Property.xml_h_schet_transfer = st.xml_h_schet;
            AppConfig.Property.xml_h_zap_transfer = st.xml_h_zap;
            AppConfig.Property.xml_h_pacient_transfer = st.xml_h_pacient;
            AppConfig.Property.xml_h_sluch_transfer = st.xml_h_sluch;
            AppConfig.Property.xml_h_usl_transfer = st.xml_h_usl;
            AppConfig.Property.xml_h_sank_smo_transfer = st.xml_h_sank_smo;
            AppConfig.Property.xml_h_ds2_n_transfer = st.xml_h_ds2_n_transfer;
            AppConfig.Property.xml_h_nazr_transfer = st.xml_h_nazr_transfer;
            AppConfig.Property.xml_h_z_sluch_transfer = st.xml_h_z_sluch;
            AppConfig.Property.xml_h_kslp_transfer = st.xml_h_kslp;
            AppConfig.Property.xml_h_b_prot_transfer = st.xml_h_b_prot;
            AppConfig.Property.xml_h_b_diag_transfer = st.xml_h_b_diag;
            AppConfig.Property.xml_h_napr_transfer = st.xml_h_napr;
            AppConfig.Property.schemaOracle_transfer = st.schemaOracle;
            AppConfig.Property.TransferBD = st.Transfer;
            AppConfig.Property.xml_h_cons_transfer = st.xml_h_cons;
            AppConfig.Property.xml_h_onk_usl_transfer = st.xml_h_onk_usl;
            AppConfig.Property.xml_h_lek_pr_transfer = st.xml_h_lek_pr;
            AppConfig.Property.xml_h_date_inj_transfer = st.xml_h_lek_pr_date_inj;
            AppConfig.Property.xml_h_sank_code_exp_transfer = st.xml_h_sank_code_exp;
            AppConfig.Property.xml_h_ds2_transfer = st.xml_h_ds2;
            AppConfig.Property.xml_h_ds3_transfer = st.xml_h_ds3;
            AppConfig.Property.xml_h_crit_transfer = st.xml_h_crit;
            AppConfig.Property.xml_h_mr_usl_n_transfer = st.xml_h_mr_usl_n;
            AppConfig.Property.xml_h_sl_lek_pr_transfer = st.xml_h_sl_lek_pr;
            AppConfig.Property.xml_h_med_dev_transfer = st.xml_h_med_dev;
        }

        public SettingTransfer GetSettingTransfer()
        {
            var st = new SettingTransfer();
            st.xml_l_zglv = AppConfig.Property.xml_l_zglv_transfer;
            st.xml_l_pers = AppConfig.Property.xml_l_pers_transfer;
            st.xml_h_zglv = AppConfig.Property.xml_h_zglv_transfer;
            st.xml_h_schet = AppConfig.Property.xml_h_schet_transfer;
            st.xml_h_zap = AppConfig.Property.xml_h_zap_transfer;
            st.xml_h_pacient = AppConfig.Property.xml_h_pacient_transfer;
            st.xml_h_sluch = AppConfig.Property.xml_h_sluch_transfer;
            st.xml_h_usl = AppConfig.Property.xml_h_usl_transfer;
            st.xml_h_sank_smo = AppConfig.Property.xml_h_sank_smo_transfer;
            st.xml_h_ds2_n_transfer = AppConfig.Property.xml_h_ds2_n_transfer;
            st.xml_h_nazr_transfer = AppConfig.Property.xml_h_nazr_transfer;
            st.xml_h_z_sluch = AppConfig.Property.xml_h_z_sluch_transfer;
            st.xml_h_kslp = AppConfig.Property.xml_h_kslp_transfer;
            st.schemaOracle = AppConfig.Property.schemaOracle_transfer;
            st.Transfer = AppConfig.Property.TransferBD;
            st.xml_h_b_prot = AppConfig.Property.xml_h_b_prot_transfer;
            st.xml_h_b_diag = AppConfig.Property.xml_h_b_diag_transfer;
            st.xml_h_napr = AppConfig.Property.xml_h_napr_transfer;
            st.xml_h_cons = AppConfig.Property.xml_h_cons_transfer;
            st.xml_h_onk_usl = AppConfig.Property.xml_h_onk_usl_transfer;
            st.xml_h_lek_pr = AppConfig.Property.xml_h_lek_pr_transfer;
            st.xml_h_lek_pr_date_inj = AppConfig.Property.xml_h_date_inj_transfer;
            st.xml_h_sank_code_exp = AppConfig.Property.xml_h_sank_code_exp_transfer;
            st.xml_h_ds2 = AppConfig.Property.xml_h_ds2_transfer;
            st.xml_h_ds3 = AppConfig.Property.xml_h_ds3_transfer;
            st.xml_h_crit = AppConfig.Property.xml_h_crit_transfer;
            st.xml_h_mr_usl_n = AppConfig.Property.xml_h_mr_usl_n_transfer;
            st.xml_h_sl_lek_pr = AppConfig.Property.xml_h_sl_lek_pr_transfer;
            st.xml_h_med_dev = AppConfig.Property.xml_h_med_dev_transfer;
            return st;
        }
    }
}

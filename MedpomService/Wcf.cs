using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceLoaderMedpomData;
using System.ServiceModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Threading;
using System.IO;
using System.Diagnostics;
using Ionic.Zip;
using System.Security.AccessControl;

using System.IdentityModel.Selectors;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.Security.Principal;

namespace MedpomService
{
    
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.Single,
        IncludeExceptionDetailInFaults = true,
        ConcurrencyMode = ConcurrencyMode.Multiple
    )
    ]
    public partial class WcfInterface : IWcfInterface
    {
        /// <summary>
        /// Список пакетов
        /// </summary>
        private FilesManager FM;

        // private JournalReception jor;
        /// <summary>
        /// Поток переноса в БД
        /// </summary>
        public Thread BDinvite;

        public CancellationTokenSource BDinviteCancellationTokenSource = new CancellationTokenSource();
        /// <summary>
        /// Поток приема XML
        /// </summary>
        private Thread THFilesInviter;
        /// <summary>
        /// Поток приема Архивов
        /// </summary>
        private Thread THArchiveInviter;
        /// <summary>
        /// FileSystemWatcher
        /// </summary>
        private FileSystemWatcher WatcherRar;
      
        /// Список схем для файлов
    
        public SchemaColection SchemaColection;
        /// <summary>
        /// Список проверок
        /// </summary>
        public ChekingList CheckList;
        /// <summary>
        /// Список процедур для переноса
        /// </summary>
        public List<OrclProcedure> ListTransfer = new List<OrclProcedure>();
        /// <summary>
        /// Делегат функции для потока ФЛК
        /// </summary>
        public delegate void ThreadFunc();
        /// <summary>
        /// Переменная делегата
        /// </summary>
        ThreadFunc DBinviteFunc;
        ThreadFunc FilesInvite;
        ThreadFunc ArchiveInviter;

        public delegate void AddFileFunct(string File);
        public event AddFileFunct addFileFunct;

        public delegate void SaveListTransfer();
        public event SaveListTransfer saveListTransfer;

        public delegate void RepeatClosePacDel(int[] index);
        public event RepeatClosePacDel repeatClosePacDel;


        public delegate void BreackProcess(int index);
        public event BreackProcess breackProcess;


        public delegate void SiteFilePack(FilePacket fp);
        public event SiteFilePack siteFilePack;

        public string PathEXE;


        public void BreackProcessPac(int index)
        {
            try
            {
                breackProcess?.Invoke(index);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public void RepeatClosePac(int[] index)
        {
            try
            {
                foreach (var ind in index)
                {
                    if (FM[ind].Status != StatusFilePack.FLKOK && FM[ind].Status != StatusFilePack.FLKERR)
                    {
                        throw new FaultException("Повтор возможен только при статусах: FLKOK,FLKERR");
                    }
                }
                repeatClosePacDel?.Invoke(index);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public void StopTimeAway(int index)
        {
            try
            {
                if (FM[index].Status == StatusFilePack.Open)
                {
                    FM[index].StopTime = true;
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

        public void SetListTransfer(List<OrclProcedure> list, string ProcClear, string ProcClearTransfer, string StatusProc, string StatusProcTransfer)
        {
            ListTransfer = list;
            saveListTransfer?.Invoke();
            AppConfig.Property.PROC_CLEAR = ProcClear;
            AppConfig.Property.PROC_CLEAR_TRANSFER = ProcClearTransfer;
            AppConfig.Property.PROC_STATUS = StatusProc;
            AppConfig.Property.PROC_STATUS_TRANSFER = StatusProcTransfer;
        }
        public List<OrclProcedure> GetListTransfer()
        {
            return ListTransfer;
        }

        public void RunProcListTransfer(int x,int y)
        {
            if (x >= 0 && x < ListTransfer.Count)
            {
                var cmd = new OracleCommand();
                var proc = ListTransfer[x];
                cmd.Connection = new OracleConnection(AppConfig.Property.ConnectionString);
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "begin " + proc.NAME_PROC + "("+y.ToString()+"); end;";

                try
                {
                    cmd.Connection.Open();
                    cmd.ExecuteScalar();
                    cmd.Connection.Close();
                }
                catch (Exception ex)
                {
                    cmd.Connection.Close();
                    throw new FaultException(ex.Message);

                }
            }
            else
            {
                throw new FaultException("Позиция не найдена в списке!" );
            }
        }

        public void SaveTransferList(string PATH)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(List<OrclProcedure>));
            var st = File.Create(PATH);
            ser.Serialize(st, ListTransfer);
            st.Close();
            
        }

        public void LoadTransferList(string PATH)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(List<OrclProcedure>));
            var st = File.OpenRead(PATH);
            ListTransfer = (List<OrclProcedure>)ser.Deserialize(st);
            st.Close();

        }


        public string CheckTableTemp1()
        {
            var oc = new OracleConnection(AppConfig.Property.ConnectionString);
            try
            {
                var cmd = new OracleCommand
                {
                    Connection = oc,
                    CommandType = CommandType.Text,
                    CommandText = "select " + AppConfig.Property.PROC_STATUS_TRANSFER + " from dual"
                };

                oc.Open();
                var  str = (string)cmd.ExecuteScalar();
                oc.Close();
                return str;
                
            }
            catch (Exception ex)
            {
                oc.Dispose();
                throw new FaultException(ex.Message);
            }
        }
        public void ClearBaseTemp1()
        {
            var oc = new OracleConnection(AppConfig.Property.ConnectionString);
            try
            {
                var cmd = new OracleCommand();
                cmd.Connection = oc;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "begin " + AppConfig.Property.PROC_CLEAR_TRANSFER + "; end;";
                oc.Open();
                cmd.ExecuteNonQuery();
                oc.Close();
            }
            catch (Exception ex)
            {
                oc.Dispose();
                throw new FaultException(ex.Message);
            }
        }

        public List<string> GetCheckClearProc()
        {
            var str = new List<string>
            {
                AppConfig.Property.PROC_CLEAR,
                AppConfig.Property.PROC_CLEAR_TRANSFER,
                AppConfig.Property.PROC_STATUS,
                AppConfig.Property.PROC_STATUS_TRANSFER
            };
            return str;
        }


        public string CheckTableTemp100()
        {
            var oc = new OracleConnection(AppConfig.Property.ConnectionString);
            try
            {
                var cmd = new OracleCommand();
                cmd.Connection = oc;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select " + AppConfig.Property.PROC_STATUS + " from dual";
                oc.Open();
                var str = (string)cmd.ExecuteScalar();
                oc.Close();
                return str;

            }
            catch (Exception ex)
            {
                oc.Dispose();
                throw new FaultException(ex.Message);
            }
        }
        public void ClearBaseTemp100()
        {
            var oc = new OracleConnection(AppConfig.Property.ConnectionString);
            try
            {
                var cmd = new OracleCommand();
                cmd.Connection = oc;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "begin " + AppConfig.Property.PROC_CLEAR + "; end;";
                oc.Open();
                cmd.ExecuteNonQuery();
                oc.Close();
            }
            catch (Exception ex)
            {
                oc.Dispose();
                throw new FaultException(ex.Message);
            }
        }

      

        /// <summary>
        /// Установить FileManager
        /// </summary>
        /// <param name="_FM">FileManager</param>
        public void SetFileManager(FilesManager _FM)
        {
            FM = _FM;
        }

      

        public void SetCheckList(ChekingList _list)
        {
            CheckList = _list;
        } 
        /// <summary>
        /// Установить журнал
        /// </summary>
        /// <param name="_jor">JournalReception</param>
        public void SetJornal(JournalReception _jor)
        {
            //jor = _jor;
        }

        /// <summary>
        /// Установить поток приема в БД и ФЛК
        /// </summary>
        /// <param name="_DBinviteFunc">Выполняемая функция</param>
        public void SetThreadBDinvite(ThreadFunc _DBinviteFunc)
        {
            DBinviteFunc = _DBinviteFunc;
        }

        /// <summary>
        /// Установить приемник для файлов
        /// </summary>
        /// <param name="_FilesInvite"></param>
        /// <param name="_WatcherRar">Приемник для ZIP</param>
        /// <param name="_ArchiveInviter"></param>
        public void SetWatcher(ThreadFunc _FilesInvite, FileSystemWatcher _WatcherRar, ThreadFunc _ArchiveInviter)
        {
            WatcherRar = _WatcherRar;
            FilesInvite = _FilesInvite;
            ArchiveInviter = _ArchiveInviter;
        }
      
        
        #region Реализация методов интерфейса Описание в интерефейсе

      

        public List<FilePacket> GetFileManagerList()
        {
            return FM?.Get();
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
            catch(Exception ex)
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
        public TableResult GetTableTransfer()
        {
            var cmdstr = @"SELECT TABLE_NAME
                            FROM ALL_TABLES
                            WHERE (TABLE_NAME = '" + AppConfig.Property.xml_h_pacient_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_sank_smo_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_schet_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_sluch_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_usl_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_zap_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_zglv_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_l_pers_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_nazr_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_ds2_n_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_z_sluch_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_kslp_transfer.ToUpper() + "' or " +

                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_cons_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_onk_usl_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_lek_pr_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_date_inj_transfer.ToUpper() + "' or " +
                                   "TABLE_NAME = '" + AppConfig.Property.xml_h_crit_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_b_diag_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_b_prot_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_napr_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_sank_code_exp_transfer.ToUpper() + "' or " +

                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_ds2_transfer.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_ds3_transfer.ToUpper() + "' or " +

                                  "TABLE_NAME = '" + AppConfig.Property.xml_l_zglv_transfer.ToUpper() + "'" +
                                   ") and OWNER = '" + AppConfig.Property.schemaOracle_transfer.ToUpper() + "'";
            var tbl = new DataTable();
            var tr = new TableResult();
            try
            {
                var oda = new OracleDataAdapter(cmdstr, new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                tbl.TableName = "tbl1";
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
                AddLog("Изменение конфигурации", EventLogEntryType.Information);
                AppConfig.Save();
           
                var dir = Path.GetDirectoryName(PathEXE);
                SchemaColection.SaveToFile(dir + "\\" + "schemaset.dat");
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при сохранении конфигурации: " + ex.Message,EventLogEntryType.Error);
            }
        }
        public void LoadProperty()
        {
            AppConfig.Load();
            var dir = Path.GetDirectoryName(PathEXE);
            SchemaColection.LoadFromFile(dir + "\\" + "schemaset.dat");
        }

        public BoolResult StopProccess()
        {
            AddLog("Попытка приостановки обработки", EventLogEntryType.Information);
            var br = new BoolResult();
                try
                {
                    AppConfig.Property.FILE_ON =false;
                    AppConfig.Save();
                    WatcherRar.EnableRaisingEvents = false;
                    
                }
                catch (Exception ex)
                {
                    br.Result = false;
                    br.Exception = ex.Message;
                    return br;
                }


            br.Result = true;
            AddLog("Обработка остановлена", EventLogEntryType.Information);
            return br;
        }
        public BoolResult StartProccess(bool MainPriem,bool Auto,DateTime dt)
        {
            AppConfig.Property.FILE_ON = true;
            AppConfig.Property.MainTypePriem = MainPriem;
            AppConfig.Property.OtchetDate = dt;
            AppConfig.Property.AUTO = Auto;
            AppConfig.Save();
            AddLog("Попытка запуска обработки",EventLogEntryType.Information);
            var br = new BoolResult();
            if (CheckedDir())
            {
               
                try
                {
                    CheckList.LoadFromBD(AppConfig.Property.ConnectionString);
                    WatcherRar.Path = AppConfig.Property.IncomingDir;
                    WatcherRar.EnableRaisingEvents = AppConfig.Property.AUTO;
                    if (BDinvite == null || !BDinvite.IsAlive)
                    {
                        BDinvite = new Thread(new ThreadStart(DBinviteFunc)) {IsBackground = true};
                        BDinvite.Start();
                        
                    }
                    if (THFilesInviter == null || !THFilesInviter.IsAlive)
                    {

                        THFilesInviter = new Thread(new ThreadStart(FilesInvite)) {IsBackground = true};
                        THFilesInviter.Start();
                      
                    }
                    if (THArchiveInviter == null || !THArchiveInviter.IsAlive)
                    {

                        THArchiveInviter = new Thread(new ThreadStart(ArchiveInviter)) {IsBackground = true};
                        THArchiveInviter.Start();
                    }
          
                }
                catch(Exception ex)
                {
                    AddLog(ex.Message,EventLogEntryType.Error);
                    br.Exception = ex.Message;
                    br.Result = false;
                    return br;
                }
                
            }
            else
            {
                br.Exception = "Ошибка при проверке директории. См. лог службы";
                br.Result = false;
                AddLog("Ошибка при проверке директории.", EventLogEntryType.Error);
                return br;
            }

            br.Result = true;
            AddLog("Обработка запущена",EventLogEntryType.Information);
            return br;
        }
   
        public void SetAutoPriem(bool Auto)
        {
            AppConfig.Property.AUTO = Auto;
            AppConfig.Save();
            WatcherRar.EnableRaisingEvents = AppConfig.Property.AUTO;
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
        }
      
        public SettingsFolder GetSettingsFolder()
        {
            var set = new SettingsFolder();
            set.ErrorDir = AppConfig.Property.ErrorDir;
            set.ErrorMessageFile = AppConfig.Property.ErrorMessageFile;
            set.IncomingDir = AppConfig.Property.IncomingDir;
            set.InputDir = AppConfig.Property.InputDir;
            set.ProcessDir = AppConfig.Property.ProcessDir;
    
            set.TimePacketOpen = AppConfig.Property.TimePacketOpen;
            set.ISP = AppConfig.Property.ISP_NAME;
            set.AddDIRInERROR = AppConfig.Property.AddDIRInERROR;
            return set;
        }
        public SettingConnect GetSettingConnect()
        {
            
            var sc = new SettingConnect();
            sc.xml_l_zglv = AppConfig.Property.xml_l_zglv;
            sc.ConnectingString = AppConfig.Property.ConnectionString;
            sc.schemaOracle = AppConfig.Property.schemaOracle;
            sc.xml_h_pacient = AppConfig.Property.xml_h_pacient;
            sc.xml_h_sank_smo = AppConfig.Property.xml_h_sank;
            sc.xml_h_schet = AppConfig.Property.xml_h_schet;
            sc.xml_h_sluch = AppConfig.Property.xml_h_sluch;
            sc.xml_h_usl = AppConfig.Property.xml_h_usl;
            sc.xml_h_zap = AppConfig.Property.xml_h_zap;
            sc.xml_h_zglv = AppConfig.Property.xml_h_zglv;
            sc.xml_l_pers = AppConfig.Property.xml_l_pers;
            sc.v_xml_error = AppConfig.Property.xml_errors;
            sc.xml_h_nazr = AppConfig.Property.XML_H_NAZR;
            sc.xml_h_ds2_n = AppConfig.Property.XML_H_DS2_N;
            sc.xml_h_z_sluch = AppConfig.Property.xml_h_z_sluch;
            sc.xml_h_kslp = AppConfig.Property.xml_h_kslp;

            sc.xml_h_b_prot = AppConfig.Property.xml_h_b_prot;
            sc.xml_h_b_diag = AppConfig.Property.xml_h_b_diag ;
         
            sc.xml_h_napr = AppConfig.Property.xml_h_napr;

            sc.xml_h_cons = AppConfig.Property.xml_h_cons;
            sc.xml_h_onk_usl= AppConfig.Property.xml_h_onk_usl;
            sc.xml_h_lek_pr = AppConfig.Property.xml_h_lek_pr;
            sc.xml_h_lek_pr_date_inj = AppConfig.Property.xml_h_date_inj;

            sc.xml_h_sank_code_exp =  AppConfig.Property.xml_h_sank_code_exp;
            sc.xml_h_ds2 = AppConfig.Property.xml_h_ds2;
            sc.xml_h_ds3 = AppConfig.Property.xml_h_ds3;
            sc.xml_h_crit = AppConfig.Property.xml_h_crit;

            return sc;
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
            return st;
        }


        public SchemaColection GetSchemaColection()
        {
            return SchemaColection;
        }
        public void SettingSchemaColection(SchemaColection sc)
        {
            SchemaColection = sc;
            SchemaCollectionOnChanged?.Invoke(SchemaColection);
        }

        public delegate void SchemaCollectionchanged(SchemaColection sc);
        public event SchemaCollectionchanged SchemaCollectionOnChanged;
        public bool ArchiveInviterStatus()
        {
            return THArchiveInviter != null && THArchiveInviter.IsAlive;
        }
        public bool FilesInviterStatus()
        {
            return THFilesInviter != null && THFilesInviter.IsAlive;
        }
        public bool FLKInviterStatus()
        {
            return BDinvite != null && BDinvite.IsAlive;
        }

        public List<string> Connect()
        {

            try
            {
                var userName = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
                var fam = "";
                var name = "";
                var ot = "";
                var id = -1;
                if (!BankAcc.AddAcc(id, userName, fam, name, ot, MyOracleProvider.GetSecurityCard(userName)))
                {
                    throw new FaultException("Пользователь " + userName + " уже подключен!");
                }
                BankAcc.SetContext(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name, OperationContext.Current);
                return BankAcc.GetCard(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name);
            }
            catch (FaultException ex)
            {
                throw new FaultException(ex.Message);
            }
            catch (Exception ex)
            {
                AddLog("Connect: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Connect подробнее в логах сервиса");
            }

        }     
        public string[] GetFolderLocal(string path)
        {
            try
            {
                return Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public string[] GetFilesLocal(string path, string pattern)
        {
            try
            {
                return Directory.GetFiles(path, pattern);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public string[] GetLocalDisk()
        {
            return Environment.GetLogicalDrives();
        }

        public EntriesMy[] GetEventLogEntry(int count)
        {
            var rez = new List<EntriesMy>();
            try
            {
                if (EventLog.Exists("MedpomServiceLog"))
                {
                    var EventLog1 = new EventLog {Source = "MedpomServiceLog"};

                    for (var i = 0; i < count; i++)
                    {
                        if (i > EventLog1.Entries.Count - 1)
                            continue;
                        var entry = EventLog1.Entries[EventLog1.Entries.Count - 1 - i];
                        var item = new EntriesMy {Message = entry.Message, TimeGenerated = entry.TimeGenerated};
                        switch (entry.EntryType)
                        {
                            case EventLogEntryType.Error:item.Type = TypeEntries.error;break;
                            case EventLogEntryType.Warning:item.Type = TypeEntries.warning;break;
                            default:
                                item.Type = TypeEntries.message;break;
                        }

                        rez.Add(item);
                    }
                }

                return rez.ToArray();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public void ClearEventLogEntry()
        {
            var EventLog = new EventLog();
            if (EventLog.Exists("MedpomServiceLog"))
            {
                EventLog.Source = "MedpomServiceLog";              
                EventLog.Clear();    
            }
        }
        public ChekingList GetChekingList()
        {
            CheckList.LoadFromBD(AppConfig.Property.ConnectionString);
            return CheckList;
        }
        public BoolResult SetChekingList(ChekingList list)
        {
            var rez = new BoolResult();
            AddLog("Изменение конфигурации проверок",EventLogEntryType.Information);
            try
            {
                if(!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"backupChekingList")))
                {
                    Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"backupChekingList"));
                }
                var filename =$"List{DateTime.Now.Day}.{DateTime.Now.Month}.{DateTime.Now.Year}_{DateTime.Now.Hour}_{DateTime.Now.Minute}_{DateTime.Now.Second}.clf";
                CheckList.SaveToFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "backupChekingList", filename));
                CheckList = list;
                var conn = new OracleConnection(AppConfig.Property.ConnectionString);
                conn.Open();
                conn.Close();
                CheckList.SaveToBD(AppConfig.Property.ConnectionString);

            }
            catch (Exception ex)
            {
                rez.Result = false;
                rez.Exception = ex.Message;
                AddLog("Ошибка при сохранении списка проверок в БД: " + ex.Message, EventLogEntryType.Error);
                return rez;

            }
            rez.Result = true;
            return rez;
        }
        public List<OrclProcedure> GetProcedureFromPack(string name)
        {
            return OrclProcedure.GetProcedureFromPack(name, AppConfig.Property.ConnectionString);
        }
        public List<OrclParam> GetParam(string name)
        {
            return OrclProcedure.GetParam(name, AppConfig.Property.ConnectionString);
        }
        public BoolResult LoadChekListFromBD()
        {
            var br = new BoolResult();
            try
            {
                CheckList.LoadFromBD(AppConfig.Property.ConnectionString);
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
        public ChekingList ExecuteCheckAv(ChekingList check)
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
                                    if (par.Type == OracleDbType.Int32)
                                        cmd.Parameters.Add(par.Name, par.Type, Convert.ToInt32(par.value), ParameterDirection.Input);
                                    else
                                        cmd.Parameters.Add(par.Name, par.Type, par.value, ParameterDirection.Input);
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
                                        case TypeParamValue.CurrYear: name =AppConfig.Property.OtchetDate.Year.ToString(); break;
                                    }                                
                                    
                                   
                                        if (par.Type == OracleDbType.Int32)
                                            cmd.Parameters.Add(par.Name, par.Type, Convert.ToInt32(name), ParameterDirection.Input);
                                        else
                                            cmd.Parameters.Add(par.Name, par.Type, name, ParameterDirection.Input);
  

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
                AddLog("Ошибка при выполнении активных проверок: " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }
        

        public void ClearFileManagerList()
        {
            try
            {
                //Удаление связыных файлов
                for (var i = 0; i < FM.Count; i++)
                {
                    if(Directory.Exists(Path.Combine(AppConfig.Property.ProcessDir, FM[i].codeMOstr)))
                        Directory.Delete(Path.Combine(AppConfig.Property.ProcessDir, FM[i].codeMOstr), true);
                }
                FM.Clear();
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при очистке пакетов: " + ex.Message, EventLogEntryType.Error);
            }

            
        }

        public bool SetPriority(int index, int priority)
        {
            try
            {
                FM[index].Priory = priority;
                return true;
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка присвоения приоритета для i = {index} {ex.Message}", EventLogEntryType.Error);
                return false;
            }

        }

        public bool DelPack(int index)
        {
            try
            {
                FM.Remove(FM[index]);
                return true;
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка удаления для i = {index} {ex.Message}", EventLogEntryType.Error);
                return false;
            }

        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////
#endregion

        /// <summary>
        /// Проверка на существование директорий
        /// </summary>
        /// <returns></returns>
        private bool CheckedDir()
        {
            try
            {
               
                var result = true;
                AddLog("Проверка " + AppConfig.Property.IncomingDir, EventLogEntryType.Information);
                if (!Directory.Exists(AppConfig.Property.IncomingDir))
                {
                    try
                    {
                        Directory.CreateDirectory(AppConfig.Property.IncomingDir);
                    }
                    catch (Exception ex)
                    {
                        AddLog(ex.Message, EventLogEntryType.Error);
                        result = false;
                    }

                }
                AddLog("Проверка " + AppConfig.Property.ErrorMessageFile, EventLogEntryType.Information);
                if (!Directory.Exists(AppConfig.Property.ErrorMessageFile))
                {

                    try
                    {
                        Directory.CreateDirectory(AppConfig.Property.ErrorMessageFile);
                    }
                    catch (Exception ex)
                    {
                        AddLog(ex.Message, EventLogEntryType.Error);
                        result = false;
                    }
                }
                AddLog("Проверка " + AppConfig.Property.ErrorDir, EventLogEntryType.Information);
                if (!Directory.Exists(AppConfig.Property.ErrorDir))
                {

                    try
                    {
                        Directory.CreateDirectory(AppConfig.Property.ErrorDir);
                    }
                    catch (Exception ex)
                    {
                        AddLog(ex.Message, EventLogEntryType.Error);
                        result = false;
                    }

                }
                AddLog("Проверка " + AppConfig.Property.InputDir, EventLogEntryType.Information);
                if (!Directory.Exists(AppConfig.Property.InputDir))
                {

                    try
                    {
                        Directory.CreateDirectory(AppConfig.Property.InputDir);
                    }
                    catch (Exception ex)
                    {
                        AddLog(ex.Message, EventLogEntryType.Error);
                        result = false;
                    }

                }
                AddLog("Проверка " + AppConfig.Property.ProcessDir, EventLogEntryType.Information);
                if (!Directory.Exists(AppConfig.Property.ProcessDir))
                {
                    try
                    {
                        Directory.CreateDirectory(AppConfig.Property.ProcessDir);
                    }
                    catch (Exception ex)
                    {
                        AddLog(ex.Message, EventLogEntryType.Error);
                        result = false;
                    }

                }
            
                return result;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при проверке дирикторий: " + ex.Message,EventLogEntryType.Error);
                return false;
            }
        }
        /// <summary>
        /// Добавить лог
        /// </summary>
        /// <param name="log">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
       static public void AddLog(string log, EventLogEntryType type)
        {
            try
            {
                var el = new EventLog();
                if (!EventLog.SourceExists("MedpomServiceLog"))
                {
                    EventLog.CreateEventSource("MedpomServiceLog", "MedpomServiceLog");
                }
                el.Source = "MedpomServiceLog";
                el.WriteEntry(log, type);
            }

            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Установить колекцию схем
        /// </summary>
        /// <param name="_schemaColection">Коллекция схем</param>
        public void SetSchemaColection(SchemaColection _schemaColection)
        {
            SchemaColection = _schemaColection;
        }



        public void SetUserPriv(string value)
        {
            AppConfig.Property.USER_PRIV = value;
        }

        public bool CheckUserPriv(string value)
        {
            Stream f = File.Create(Path.Combine(AppConfig.Property.ProcessDir, "USER_CHEK"));
            f.Close();
            bool result;
            try
            {
                AddFileSecurity(Path.Combine(AppConfig.Property.ProcessDir, "USER_CHEK"), value, FileSystemRights.FullControl, AccessControlType.Allow);
                File.Delete(Path.Combine(AppConfig.Property.ProcessDir, "USER_CHEK"));
                result = true;
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public string GetUserPriv()
        {
            return AppConfig.Property.USER_PRIV;
        }

        public static void AddFileSecurity(string FileName, string Account, FileSystemRights Rights, AccessControlType ControlType)
        {
            // Create a new FileInfo object.
            var fInfo = new FileInfo(FileName);

            // Get a FileSecurity object that represents the 
            // current security settings.
            var fSecurity = fInfo.GetAccessControl();
            // Add the FileSystemAccessRule to the security settings. 
            fSecurity.AddAccessRule(new FileSystemAccessRule(Account,
                                                            Rights,
                                                            ControlType));

            // Set the new access settings.
            fInfo.SetAccessControl(fSecurity);
            
            
        }


      
        public DataTable GetNotReestr()
        {
            try
            {
                var tbl = new DataTable("V_NOT_REESTR_MEDSERV");
                var oda = new OracleDataAdapter("select * from V_NOT_REESTR_MEDSERV", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе списка не подавших реестры: " + ex.Message, EventLogEntryType.Error);
                return null;



            }
        }
        

        /// <summary>
        /// Сводная по темп еtemp1
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_SMO_TEMP1(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from v_xml_svod_smo_temp1 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе Свод СМО по темп1 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }
        /// <summary>
        /// Сводная по темп 100
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_SMO_TEMP100(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from v_xml_svod_smo_temp100 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе Свод СМО по темп100 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

        /// <summary>
        /// Отчет по дисп по темп 1
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_DISP_TEMP1(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from MEDPOMSERVIS_DISP_TEMP1 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе MEDPOMSERVIS_DISP_TEMP1 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }
        /// <summary>
        /// Отчет по дисп по темп 100
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_DISP_TEMP100(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from MEDPOMSERVIS_DISP_TEMP100 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе MEDPOMSERVIS_DISP_TEMP100 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }


        /// <summary>
        /// Отчет по дисп ITOG
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="YEAR"></param>
        /// <returns></returns>
        public DataTable GetSVOD_DISP_ITOG(DataTable tbl,int YEAR)
        {
            try
            {
                var oda = new OracleDataAdapter("select * from MEDPOMSERVIS_DISP_ITOG t where \"Год\" = "+YEAR.ToString(), new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе MEDPOMSERVIS_DISP_ITOG " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }



         /// <summary>
        /// Отчет по VMP по темп 1
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_VMP_TEMP1(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from mepomservis_vmp_TEMP1 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе mepomservis_vmp_TEMP100 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

         /// <summary>
        /// Отчет по VMP по темп 100
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_VMP_TEMP100(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from mepomservis_vmp_TEMP100 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе mepomservis_vmp_TEMP100 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }


        /// <summary>
        /// Отчет по VMP ITOG
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_VMP_ITOG(DataTable tbl, int YEAR)
        {
            try
            {
                var oda = new OracleDataAdapter($"select * from mepomservis_vmp_itog t where \"Год\" = {YEAR}", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog($"Ошибка при запросе mepomservis_vmp_itog {ex.Message}", EventLogEntryType.Error);
                return null;
            }
        }

        /// <summary>
        /// Отчет по SMP по темп 1
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_SMP_TEMP1(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from mepomservis_smp_TEMP1 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе mepomservis_smp_TEMP100 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

        /// <summary>
        /// Отчет по SMP по темп 100
        /// </summary>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public DataTable GetSVOD_SMP_TEMP100(DataTable tbl)
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from mepomservis_smp_TEMP100 t", new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе mepomservis_smp_TEMP100 " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }


        /// <summary>
        /// Отчет по SMP за ГОД
        /// </summary>
        /// <param name="tbl"></param>
        /// <param name="YEAR"></param>
        /// <returns></returns>
        public DataTable GetSVOD_SMP_ITOG(DataTable tbl,int YEAR)
        {
            try
            {
                var oda = new OracleDataAdapter("select * from  MEPOMSERVIS_SMP_ITOG WHERE \"Год\" = "+YEAR.ToString(), new OracleConnection(AppConfig.Property.ConnectionString));
                oda.Fill(tbl);
                return tbl;
            }
            catch (Exception ex)
            {
                AddLog("Ошибка при запросе MEPOMSERVIS_SMP_ITOG " + ex.Message, EventLogEntryType.Error);
                return null;
            }
        }

       

        #region Права
       public DataTable Roles_GetMethod()
        {
            var tbl = new DataTable("Method");
            var oda = new OracleDataAdapter("select * from MEDPOM_EXIST_METHOD t", new OracleConnection(AppConfig.Property.ConnectionString));
            oda.Fill(tbl);
            return tbl;
           
        }
       
    

       public void Roles_AddMethod(string Name,string Comm)
        {
            var cmd = new OracleCommand(@"insert into medpom_exist_method
  (name, coment)
values
  (:name, :coment)", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":name",Name);
            cmd.Parameters.Add(":coment",Comm);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }
        public void Roles_DeleteMethod(int  id)
        {
            var cmd = new OracleCommand(@"delete medpom_exist_method
 where id = :id", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":id", id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();          
        }
        public void Roles_UpdateMethod(string Name,string Coment,int  id )
          {
              var cmd = new OracleCommand(@"update medpom_exist_method
   set name = :name,
       coment = :coment
 where id = :id", new OracleConnection(AppConfig.Property.ConnectionString));
              cmd.Parameters.Add(":name", Name);
              cmd.Parameters.Add(":coment", Coment);
              cmd.Parameters.Add(":id", id);
              cmd.Connection.Open();
              cmd.ExecuteScalar();
              cmd.Connection.Close();
          }        
        public DataTable  Roles_GetRoles()
        {
            var tbl = new DataTable("ROLES");
            var oda = new OracleDataAdapter("select * from medpom_client_roles t", new OracleConnection(AppConfig.Property.ConnectionString));
            oda.Fill(tbl);
            return tbl;
        }      
        public int Roles_AddRoles(string Name, string Comment)
        {
            var cmd = new OracleCommand(@"insert into medpom_client_roles
  ( role_name, role_comment)
values
  (:role_name, :role_comment)
RETURNING id INTO :id", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":role_name", Name);
            cmd.Parameters.Add(":role_comment", Comment);
            cmd.Parameters.Add("id", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();

            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)cmd.Parameters["id"].Value).ToInt32();
        }        
        public void Roles_DeleteRoles(int id)
        {
            var cmd = new OracleCommand(@"delete medpom_client_roles
 where id = :id", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":id", id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }        
        public void Roles_UpdateRoles(string Name, string Comment, int id)
        {
            var cmd = new OracleCommand(@"update medpom_client_roles
   set role_name = :role_name,
       role_comment = :role_comment
 where id = :id
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":role_name", Name);
            cmd.Parameters.Add(":role_comment", Comment);
            cmd.Parameters.Add(":id", id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }                
        public DataTable Roles_GetRolesClaims()
        {
            var tbl = new DataTable("RolesClaims");
            var oda = new OracleDataAdapter(@"select c.*,m.name,m.coment from medpom_client_claims c
inner join medpom_exist_method m on (c.claims_id = m.id)", new OracleConnection(AppConfig.Property.ConnectionString));
            oda.Fill(tbl);
            return tbl;
        }
        public void Roles_AddClaims(int role_id, int claims_id)
        {
            var cmd = new OracleCommand(@"insert into medpom_client_claims
  (role_id, claims_id)
values
  (:role_id, :claims_id)
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":role_id", role_id);
            cmd.Parameters.Add(":claims_id", claims_id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }
        public void Roles_DeleteClaims(int role_id, int claims_id)
        {
            var cmd = new OracleCommand(@"delete medpom_client_claims
 where role_id = :role_id
   and claims_id = :claims_id
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add("role_id", role_id);
            cmd.Parameters.Add("claims_id", claims_id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }
        public void Roles_UpdateClaims(int id_role, int id_claims, int old_role_id, int old_claims_id)
        {
            var cmd = new OracleCommand(@"update medpom_client_claims
   set role_id = :role_id,
       claims_id = :claims_id
 where role_id = :old_role_id
   and claims_id = :old_claims_id
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":role_id", id_role);
            cmd.Parameters.Add(":claims_id", id_claims);
            cmd.Parameters.Add(":old_role_id", old_role_id);
            cmd.Parameters.Add(":old_claims_id", old_claims_id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }
        public DataTable Roles_GetUsers()
        {
            var tbl = new DataTable("Users");
             var oda = new OracleDataAdapter(@"select * from medpom_client_users", new OracleConnection(AppConfig.Property.ConnectionString));
            oda.Fill(tbl);
            return tbl;
            
        }
        public int Roles_AddUsers(string Name, string password)
        {
            var cmd = new OracleCommand(@"insert into medpom_client_users
  (pass, name)
values
  (:pass, :name)
 RETURNING id INTO :id
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":pass", password);
            cmd.Parameters.Add(":name", Name.ToUpper());
            cmd.Parameters.Add("id", OracleDbType.Decimal, ParameterDirection.Output);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
            return ((Oracle.ManagedDataAccess.Types.OracleDecimal)cmd.Parameters["id"].Value).ToInt32();
        }
        public void Roles_DeleteUsers(int id)
        {
            var cmd = new OracleCommand(@"delete medpom_client_users
 where id = :id
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":id", id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }
        public void Roles_UpdateUsers(string Name, string password, int id)
        {
            var cmd = new OracleCommand(@"update medpom_client_users
   set pass = :pass,
       name = :name
 where id = :id
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":pass", password);
            cmd.Parameters.Add(":name", Name.ToUpper());
            cmd.Parameters.Add(":id",id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }
        public DataTable Roles_GetUsers_Roles()
        {
            var tbl = new DataTable("Users_Roles");
             var oda = new OracleDataAdapter(@"select user_id, role_id,ro.role_name,ro.role_comment from medpom_client_us_rol t
inner join medpom_client_roles ro on (ro.id = t.role_id)", new OracleConnection(AppConfig.Property.ConnectionString));
            oda.Fill(tbl);
            return tbl;
            
        }
        public void Roles_AddUsers_Role(int user_id, int role_id)
        {
            var cmd = new OracleCommand(@" insert into medpom_client_us_rol
  (user_id, role_id)
values
  (:user_id, :role_id)
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":user_id", user_id);
            cmd.Parameters.Add(":role_id", role_id);   
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();

           
        }
        public void Roles_DeleteUsers_Role(int user_id, int role_id)
        {
            var cmd = new OracleCommand(@" delete medpom_client_us_rol
 where user_id = :user_id
   and role_id = :role_id
", new OracleConnection(AppConfig.Property.ConnectionString));
            cmd.Parameters.Add(":user_id", user_id);
            cmd.Parameters.Add(":role_id", role_id);
            cmd.Connection.Open();
            cmd.ExecuteScalar();
            cmd.Connection.Close();
        }

     

        #endregion

        

        ProgressClass progress;
        public void SaveProcessArch()
        {
            if (progress != null)
            {
                if (progress.Active)
                {
                    throw new FaultException("Операция уже выполняется!!!");
                }
                
            }

            progress = new ProgressClass {Active = true};

            var th = new Thread(SaveProgressFolder) {IsBackground = true};
            th.Start();
        }

        private void SaveProgressFolder()
        {
            ZipFile zf = null;
            try
            {
                Directory.CreateDirectory(Path.Combine(AppConfig.Property.InputDir, DateTime.Now.Year.ToString(), DateTime.Now.ToString("MMMMMMMMMMMMM")));
                var num = 1;
                while (File.Exists(Path.Combine(AppConfig.Property.InputDir, DateTime.Now.Year.ToString(), DateTime.Now.ToString("MMMMMMMMMMMMM"), "PROCESS" + num.ToString() + ".zip")))
                {
                    num++;
                }
                zf = new ZipFile(Path.Combine(AppConfig.Property.InputDir, DateTime.Now.Year.ToString(), DateTime.Now.ToString("MMMMMMMMMMMMM"), "PROCESS" + num.ToString() + ".zip"), Encoding.GetEncoding("cp866"));
                zf.AddDirectory(AppConfig.Property.ProcessDir, "PROCESS");
                zf.SaveProgress += zf_SaveProgress;
                zf.Save();
                progress.Max = 0;
                progress.Value = 0;
                progress.TXT = "Завершено";
                progress.Active = false;
            }
            catch (Exception ex)
            {
                zf?.Dispose();
                progress.Max = 0;
                progress.Value = 0;
                progress.TXT = "Ошибка при архивировании PROCESS: " + ex.Message;
                progress.Active = false;
                AddLog("Ошибка при архивировании PROCESS: " + ex.Message, EventLogEntryType.Error);
            }
        }
        public ProgressClass GetProgressClassProcessArch()
        {
            return progress;
        }

        void zf_SaveProgress(object sender, SaveProgressEventArgs e)
        {
            if (e.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
                if (e.CurrentEntry != null)
                {
                    progress.Value = e.EntriesSaved;
                    progress.Max = e.EntriesTotal;
                    progress.TXT = e.CurrentEntry.FileName; 
                }
            if (e.EventType == ZipProgressEventType.Error_Saving )
            {
                if (e.CurrentEntry != null)
                {                  
                    progress.TXT = "Ошибка: "+e.CurrentEntry.FileName+ " "+e.CurrentEntry.Comment;
                }
            }            
        }
       public bool GetTypePriem()
       {
           return AppConfig.Property.MainTypePriem;
       }

       public DateTime GetOtchetDate()
       {
           return AppConfig.Property.OtchetDate;
       }


       public bool GetAutoPriem()
       {
         return AppConfig.Property.AUTO;
       
       }


       public bool GetAutoFileAdd()
       {
           return WatcherRar.EnableRaisingEvents;
       }

        public byte[] GetFile(string CODE_MO,int FILE,TypeDOWLOAD type,long offset)
        {
            try
            {
                var PAC = FM.FindIndexPacket(CODE_MO);
                string PATH;
                switch (type)
                {
                    case TypeDOWLOAD.File:
                        PATH = PAC.Files[FILE].FilePach; break;
                    case TypeDOWLOAD.FileL:
                        PATH = PAC.Files[FILE].filel.FileName; break;
                    case TypeDOWLOAD.FILE_STAT:
                        PATH = PAC.PATH_STAT; break;   
                    case TypeDOWLOAD.FILE_LOG:
                        PATH = PAC.Files[FILE].FileLog.FilePath; break;
                    case TypeDOWLOAD.FILE_L_LOG:
                        PATH = PAC.Files[FILE].filel.FileLog.FilePath; break;
                    case TypeDOWLOAD.XML_OTCHET_H:
                        PATH = PAC.Files[FILE].PATH_LOG_XML; break;
                    case TypeDOWLOAD.XML_OTCHET_L:
                        PATH = PAC.Files[FILE].filel.PATH_LOG_XML; break;
                    case TypeDOWLOAD.ZIP_ARCHIVE:
                        PATH = PAC.PATH_ZIP; break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                var count = 1024 * 1024 * 3;
                var buff = new byte[count];
                using (Stream st = File.Open(PATH, FileMode.Open))
                {
                    st.Position = offset;
                    var readByte = st.Read(buff, 0, count);
                    st.Close();
                    return readByte < count ? GetPartByte(buff, 0, readByte) : buff;
                }
            }
            catch (Exception ex)
            {
                AddLog($"GetFile{CODE_MO}: {ex.Message}", EventLogEntryType.Error);
                throw new FaultException("Ошибка передачи файла");
            }
            
        }
        public long GetFileLength(string CODE_MO, int FILE, TypeDOWLOAD type)
        {
            try
            {
                var PAC = FM.FindIndexPacket(CODE_MO);
                string PATH;
                switch (type)
                {
                    case TypeDOWLOAD.File:
                        PATH = PAC.Files[FILE].FilePach; break;
                    case TypeDOWLOAD.FileL:
                        PATH = PAC.Files[FILE].filel.FileName; break;
                    case TypeDOWLOAD.FILE_STAT:
                        PATH = PAC.PATH_STAT; break;                   
                    case TypeDOWLOAD.FILE_LOG:
                        PATH = PAC.Files[FILE].FileLog.FilePath; break;
                    case TypeDOWLOAD.FILE_L_LOG:
                        PATH = PAC.Files[FILE].filel.FileLog.FilePath; break;
                    case TypeDOWLOAD.XML_OTCHET_H:
                        PATH = PAC.Files[FILE].PATH_LOG_XML; break;
                    case TypeDOWLOAD.XML_OTCHET_L:
                        PATH = PAC.Files[FILE].filel.PATH_LOG_XML; break;
                    case TypeDOWLOAD.ZIP_ARCHIVE:
                        PATH = PAC.PATH_ZIP; break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                Stream st = File.Open(PATH, FileMode.Open);
                var rsl = st.Length;
                st.Close();
                return rsl;
            }
            catch (Exception ex)
            {
                AddLog($"GetFileLength: {ex.Message}", EventLogEntryType.Error);
                throw new FaultException("Ошибка при передачи файла");
            }

        }

        private byte[] GetPartByte(byte[] owner, int from, int to)
        {
            var list = new List<byte>();
            for (var i = from; i < to; i++)
                list.Add(owner[i]); 
            return list.ToArray();
        }

        public void AddListFile(List<string> List)
        {

            if (AppConfig.Property.AUTO == false && AppConfig.Property.FILE_ON)
            {

                var ListORD =List.OrderBy(x => ParseFileName.Parse(Path.GetFileNameWithoutExtension(x)).Ni).ToList();
                foreach (var str in ListORD)
                {
                    addFileFunct?.Invoke(str);
                }
            }
            else
            {
                throw new FaultException("Не возможно добавить файлы в список т.к. не соблюдены условия работы службы: Прием в ручном режиме");
            }
        }



        #region Счета фактуры
        public DataTable GetID_SPOSOB()
        {
            try
            {

                var oda = new OracleDataAdapter("select * from nsi.SPOSOB_OPL order by ID_SPOSOB", new OracleConnection(AppConfig.Property.ConnectionString));
                var SPOSOB_OPL = new DataTable("SPOSOB_OPL");
                oda.Fill(SPOSOB_OPL);
                return SPOSOB_OPL;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }


        public DataTable GetVIDMP()
        {
            try
            {

                var oda = new OracleDataAdapter("select * from nsi.VIDMP order by num_order", new OracleConnection(AppConfig.Property.ConnectionString));
                var SPOSOB_OPL = new DataTable("VIDMP");
                oda.Fill(SPOSOB_OPL);
                return SPOSOB_OPL;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public DataTable GetMUR_FIN()
        {
            try
            {
                var oda = new OracleDataAdapter("select code_mo,fin_s+fin_ch sum, smo from nsi.MUR_FIN", new OracleConnection(AppConfig.Property.ConnectionString));
                var SPOSOB_OPL = new DataTable("MUR_FIN");
                oda.Fill(SPOSOB_OPL);
                return SPOSOB_OPL;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public DataTable GetMUR_FIN_SMP()
        {
            try
            {
                var oda = new OracleDataAdapter("select code_mo,fin_s+fin_ch sum, smo from nsi.MUR_FIN_SMP", new OracleConnection(AppConfig.Property.ConnectionString));
                var SPOSOB_OPL = new DataTable("MUR_FIN_SMP");
                oda.Fill(SPOSOB_OPL);
                return SPOSOB_OPL;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public DataTable Getf003()
        {
            try
            {
                var oda = new OracleDataAdapter(@"select mcod,nam_mok from nsi.f003 t
where tf_okato = '76000'  and sysdate between t.d_begin and nvl(t.d_end,sysdate)", new OracleConnection(AppConfig.Property.ConnectionString));
                var SPOSOB_OPL = new DataTable("f003");
                oda.Fill(SPOSOB_OPL);
                return SPOSOB_OPL;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }


        public DataTable Getf002()
        {
            try
            {
                var oda = new OracleDataAdapter(@"select t.smocod,t.nam_smok from nsi.F002 t
where t.tf_okato = '76000' and sysdate between t.d_begin and nvl(t.d_end,sysdate)", new OracleConnection(AppConfig.Property.ConnectionString));
                var SPOSOB_OPL = new DataTable("f002");
                oda.Fill(SPOSOB_OPL);
                return SPOSOB_OPL;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }


        public DataTable GetV_XML_H_FAKTURA()
        {
            try
            {
                var oda = new OracleDataAdapter(@"select * from V_XML_H_FAKTURA", new OracleConnection(AppConfig.Property.ConnectionString));
                var SPOSOB_OPL = new DataTable("V_XML_H_FAKTURA");
                oda.Fill(SPOSOB_OPL);
                return SPOSOB_OPL;
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
        #endregion


        public ServiceLoaderMedpomData.Version GetVersion()
        {
            try
            {
                var ver = new ServiceLoaderMedpomData.Version();
                var curr_dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (File.Exists(Path.Combine(curr_dir, "CLIENT_UPDATE", "version.xml")))
                {

                    ver.LoadFromFile(Path.Combine(curr_dir, "CLIENT_UPDATE", "version.xml"));
                    return ver;
                }

                throw new FaultException("Файл версии не найден на сервере!");
            }
            catch (Exception ex)
            {
                AddLog("GetVersion: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в GetVersion подробнее в логах сервиса");
            }
        }

        public byte[] LoadFileUpdate(FileAndMD5 file, int offset, int count)
        {
            try
            {
                var curr_dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var FilePath = Path.Combine(curr_dir, "CLIENT_UPDATE", file.Name);
                if (File.Exists(FilePath))
                {
                    Stream st = File.OpenRead(FilePath);


                    var buffer = new byte[count];
                    st.Position = offset;
                    var readByte = st.Read(buffer, 0, count);

                    if (readByte < count)
                    {
                        return GetPartByte(buffer, 0, readByte);
                    }
                    st.Close();
                    return buffer;
                }
                else
                {
                    throw new FaultException("Файл не найден на сервере!");
                }

            }
            catch (Exception ex)
            {

                AddLog("LoadFileUpdate: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в LoadFileUpdate подробнее в логах сервиса");
            }
        }
        public bool Ping()
        {
            return true;
        }

      
        public FilePacketAndOrder GetPackForMO(string code_mo)
        {
            var t = GetFileManagerList().FirstOrDefault(x => x.codeMOstr == code_mo);
            var res = new FilePacketAndOrder() { FP = t, ORDER = FM.Order(t) };
            return res;
        }
       
        public void AddFilePacketForMO(FilePacket fp)
        {
            fp.StopTime = true;
            fp.IST = IST.SITE;
            fp.Date = DateTime.Now;
                siteFilePack?.Invoke(fp);
        }
        public delegate void eventRegisterNewFileManager(USER us);
        public event eventRegisterNewFileManager RaiseRegisterNewFileManager;

        public delegate void eventUnRegisterNewFileManager(USER us);
        public event eventUnRegisterNewFileManager RaiseUnRegisterNewFileManager;

        private USER GETUSER
        {
            get
            {
                var name = OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name;
                return BankAcc.GetUSER(name.ToUpper());
            }
        }

        public void RegisterNewFileManager()
        {
            var us = GETUSER;
            if (us != null)
            {
                RaiseRegisterNewFileManager?.Invoke(us);
            }
        }

        public void UnRegisterNewFileManager()
        {
            var us = GETUSER;
            if (us != null)
            {
                RaiseUnRegisterNewFileManager?.Invoke(us);
            }
        }
    } 



  

    public class MyCustomUserNameValidator : UserNamePasswordValidator
    {
        public List<string> card = new List<string>();
        public override void Validate(string userName, string password)
        {
            int id;
            try
            {

                if (!MyOracleProvider.CheckUser(userName, password, out id))
                {
                    if (MedpomService.SysLog.ToUpper() == userName.ToUpper() && MedpomService.SysPass == password)
                    {
                    }
                    else
                    {
                        throw new FaultException("Неверный логин или пароль");
                    }

                }
            }
            catch (Exception ex)
            {
                if (MedpomService.SysLog.ToUpper() == userName.ToUpper() && MedpomService.SysPass == password)
                {
                    return;
                }
                throw new FaultException(ex.Message);
            }
            
        }
    }


   

    public class MyOracleProvider
    {

        public static bool CheckUser(string USER, string PASS, out int ID)
        {
            try
            {
               // WcfInterface.AddLog(USER + PASS, EventLogEntryType.Error);
                var oda = new OracleDataAdapter("select * from MEDPOM_CLIENT_USERS t where upper(t.name) = '" + USER.ToUpper() + "' and pass = '" + PASS+"'", new OracleConnection(AppConfig.Property.ConnectionString));
                var tbl = new DataTable();
                //WcfInterface.AddLog("select * from MEDPOM_CLIENT_USERS t where upper(t.name) = '" + USER.ToUpper() + "' and pass = '" + PASS+"'", EventLogEntryType.Error);
                oda.Fill(tbl);
               // WcfInterface.AddLog(tbl.Rows.Count.ToString(), EventLogEntryType.Error);
                if (tbl.Rows.Count == 0)
                {
                    ID = -1;
                    return false;
                }
                else
                {
                    ID = Convert.ToInt32(tbl.Rows[0]["ID"]);
                    return true;
                }
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Ошибка проверки пользователя: " + ex.Message, EventLogEntryType.Error);
                ID = -1;
                return false;       
            }
        }
        public static List<string> GetSecurityCard(string name)
        {
            try
            {
                if (name.ToUpper() == MedpomService.SysLog.ToUpper())
                {
                    return GetFullRight();
                }
                // return new List<string>();
                var oda = new OracleDataAdapter(@"select distinct  met.name   from medpom_client_users u
inner join medpom_client_us_rol rol on (rol.user_id = u.id)
inner join medpom_client_claims cl on (cl.role_id = rol.role_id)
inner join medpom_exist_method met on (met.id = cl.claims_id)
where upper(u.name) = '"+ name.ToUpper() + "'", new OracleConnection(AppConfig.Property.ConnectionString));
                var tbl = new DataTable();
                oda.Fill(tbl);
                var str = new List<string>();
                foreach (DataRow row in tbl.Rows)
                {
                    str.Add(row["NAME"].ToString());
                }
              
                return str;
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Ошибка получения карточки: " + ex.Message, EventLogEntryType.Error);
                return new List<string>();              
            }
        }

        private static List<string> GetFullRight()
        {
            var List = new List<string>();
            var Met = ReflectClass.MethodReflectInfo<IWcfInterface>();
            foreach (var mi in Met)
            {
                List.Add(mi.Name);

            }
            return List;


        }
    }

    public class AuthorizationPolicy : IAuthorizationPolicy
    {
        string id;

        public List<string> card;
        public AuthorizationPolicy()
        {
            id = Guid.NewGuid().ToString();           
        

        }
      
        public bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            if (evaluationContext.Properties.ContainsKey("Identities"))
            {
                var identities =
                    evaluationContext.Properties["Identities"] as IList<IIdentity>;
                if (identities != null && identities.Count > 0)
                {
                    evaluationContext.Properties["Principal"] = new CustomPrincipal(identities[0]);
                    return true;
                }
            }

           
      
            
            foreach (var cs in evaluationContext.ClaimSets)
                if (cs.FindClaims(ClaimTypes.Name, Rights.Identity).Any())
                {
                    return true;
                }
            return false;
        }

        public ClaimSet Issuer => ClaimSet.System;

        public string Id => id;
    }


    public class MyServiceAuthorizationManager : ServiceAuthorizationManager
    {
        const string url =  "http://tempuri.org/IWcfInterface/";
        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            if (operationContext.ServiceSecurityContext.IsAnonymous)
                return true;
            foreach (var cs in operationContext.ServiceSecurityContext.AuthorizationContext.ClaimSets)
            {
                if (cs.Issuer == ClaimSet.System)
                {
                    //return true;
                    foreach(var c in cs.FindClaims(ClaimTypes.Name,Rights.PossessProperty))
                    {

                        var Reght = BankAcc.GetCard(operationContext.ServiceSecurityContext.PrimaryIdentity.Name);
                        if (Reght != null)
                            return BankAcc.GetCard(operationContext.ServiceSecurityContext.PrimaryIdentity.Name).Contains(
                                operationContext.IncomingMessageHeaders.Action.Replace(url, "")) || operationContext.IncomingMessageHeaders.Action.Replace(url, "") == "Connect";
                        return operationContext.IncomingMessageHeaders.Action.Replace(url, "") == "Connect";
                    }
                }
            }
            return false;

            
        }
    }
    
    public class CustomPrincipal : IPrincipal
    {
        private readonly IIdentity _identity;
        public CustomPrincipal(IIdentity identity)
        {
            _identity = identity;
           
        }
        public IIdentity Identity => _identity;

        public bool IsInRole(string role)
        {
            return false;
        }

      
    }

    public class BankAcc
    {
        public static Dictionary<string,USER> Cards_user = new Dictionary<string,USER>();
        public static List<string> GetCard(string user)
        {
            user = user.ToUpper();
            return Cards_user.ContainsKey(user) ? Cards_user[user].list : null;
        }



        public static bool AddAcc(int id, string userName, string fam, string im, string ot, List<string> list)
        {
            userName = userName.ToUpper();
            if (Cards_user.ContainsKey(userName))
            {
                var context = Cards_user[userName].Context;
                if (context == null)
                {
                    Cards_user.Remove(userName);
                }
                else
                {
                    var st = context.Channel.State;
                    st = CommunicationState.Opened;
                    if (st != CommunicationState.Opened)
                    {
                        context.Channel.Abort();
                        Cards_user.Remove(userName);
                    }
                    else
                    {
                        try
                        {
                            var t = context.GetCallbackChannel<IWcfInterfaceCallback>();
                            t.PING();
                            return false;
                        }
                        catch(Exception)
                        {
                            context.Channel.Abort();
                            Cards_user.Remove(userName);
                        }
                    }
                }
            }

            var us = new USER
            {
                ID = id,
                NAME = userName,
                FAM = fam,
                IM = im,
                OT = ot,
                list = list
            };
            Cards_user.Add(userName, us);
            return true;
        }

        public static USER FindByID(int id)
        {
            var fvalue = Cards_user.FirstOrDefault(x => x.Value.ID == id);
            return fvalue.Value;
        }

        public static void SetContext(string userName, OperationContext Context)
        {
            userName = userName.ToUpper();
            if (Cards_user.ContainsKey(userName))
            {
                Cards_user[userName].Context = Context;
            }
        }
        public static int GetID(string user)
        {
            user = user.ToUpper();
            if (Cards_user.ContainsKey(user))
            {
                return Cards_user[user].ID;
            }
            return -1;
        }

        public static USER GetUSER(string user)
        {
            user = user.ToUpper();
            if (Cards_user.ContainsKey(user))
            {
                return Cards_user[user];
            }
            return null;
        }

        public static void DeleteUser(string user)
        {
            user = user.ToUpper();
            if (Cards_user.ContainsKey(user))
            {
                Cards_user.Remove(user.ToUpper());
            }
        }

        public static IEnumerable<USER> GetEnumerable()
        {
           return Cards_user.Select(x => x.Value);
        }
    }
    
}

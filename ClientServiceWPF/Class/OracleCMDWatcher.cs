using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Threading;

namespace ClientServiceWPF.Class
{
    /// <summary>
    /// Контролирует прогресс выполнения процедуры используя ACTION в v$session
    /// Т.е. процедуры должны менять этот параметр для информирования, через dbms_application_info.set_action
    /// </summary>
    public class OracleCMDWatcher:IDisposable
    {
        string SID;      
        Task worker;
        CancellationTokenSource cancel;
        string connStr;
        public OracleCMDWatcher(OracleConnection con, string connStr)
        {
            if(con.State != ConnectionState.Open)
            {
                throw new Exception("Подключение должно быть открыто!");
            }
            this.connStr = connStr;
            SID = GetSID(con);
        }
        public void StartWatch(int Time, IProgress<string> progress)
        {
            if(worker!=null && worker.Status== TaskStatus.Running)
            {
                throw new Exception("Мониторинг уже запущен");
            }        
            cancel = new CancellationTokenSource();
            worker = Task.Run(async () =>
            {
                while(!cancel.IsCancellationRequested)
                {
                    try
                    {
                        progress?.Report(GetAction());
                        await Task.Delay(Time);
                    }
                    catch(Exception ex)
                    {
                        progress?.Report($"Ошибка в контроле прогресса: {ex.Message}");
                    }
                   
                }
            }, cancel.Token);
        }

        public void StopWatch()
        {
            cancel.Cancel();
        }


        private string GetSID(OracleConnection con)
        {
            using (var cmd = new OracleCommand("SELECT SID FROM V$SESSION WHERE AUDSID = Sys_Context('USERENV', 'SESSIONID')", con))
            {
                return Convert.ToString(cmd.ExecuteScalar());
            }
        }

        private string GetAction()
        {
            using (var con = new OracleConnection(connStr))
            {
                using (var cmd = new OracleCommand("select t.ACTION from v$session t where t.SID = :SID", con))
                {
                    cmd.Parameters.Add("SID", SID);
                    con.Open();
                    return Convert.ToString(cmd.ExecuteScalar());
                }
            }
        }

        public void Dispose()
        {
            StopWatch();
            
        }
    }
}

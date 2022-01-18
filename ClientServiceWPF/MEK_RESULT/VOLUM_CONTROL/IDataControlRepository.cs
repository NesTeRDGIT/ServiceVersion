using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ClientServiceWPF.Class;

using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.MEK_RESULT.VOLUM_CONTROL
{
    public interface IDataControlRepository
    {
        Task<DateTime[]> GetPeriodTemp100Async();
        Task ClearTemp100Async(Progress<string> progress);
        Task<DateTime[]> GetPeriodTemp1Async();
        Task ClearTemp1Async(Progress<string> progress);
        Task TransferTemp100Async(Progress<string> progress);

    }



    public class DataControlRepository : IDataControlRepository
    {
        private string connStr;

        public DataControlRepository(string connStr)
        {
            this.connStr = connStr;
        }

        public void ClearTemp100(Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(connStr))
                {
                    using (var cmd = new OracleCommand("DataControlMP.ClearTemp100", con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, connStr);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }

        }
        public Task ClearTemp100Async(Progress<string> progress)
        {
            return Task.Run(() => ClearTemp100(progress));
        }



        public Task<DateTime[]> GetPeriodTemp100Async()
        {
            return Task.Run(() => GetPeriodTemp100());
        }

        private DateTime[] GetPeriodTemp100()
        {
            var listResult = new List<DateTime>();
            using (var con = new OracleConnection(connStr))
            {
                using (var cmd = new OracleCommand("select * from table(DataControlMP.GetPeriodTemp100)", con))
                {

                    con.Open();
                    var read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        listResult.Add(Convert.ToDateTime(read["PERIOD"]));
                    }
                    con.Close();
                    return listResult.ToArray();
                }
            }
        }

        public void ClearTemp1(Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(connStr))
                {
                    using (var cmd = new OracleCommand("DataControlMP.ClearTemp1", con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, connStr);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }

        }

        public Task ClearTemp1Async(Progress<string> progress)
        {
            return Task.Run(() => ClearTemp1(progress));
        }

        public Task<DateTime[]> GetPeriodTemp1Async()
        {
            return Task.Run(() => GetPeriodTemp1());
        }


        private DateTime[] GetPeriodTemp1()
        {
            var listResult = new List<DateTime>();
            using (var con = new OracleConnection(connStr))
            {
                using (var cmd = new OracleCommand("select * from table(DataControlMP.GetPeriodTemp1)", con))
                {

                    con.Open();
                    var read = cmd.ExecuteReader();
                    while (read.Read())
                    {
                        listResult.Add(Convert.ToDateTime(read["PERIOD"]));
                    }
                    con.Close();
                    return listResult.ToArray();
                }
            }
        }

        public Task TransferTemp100Async(Progress<string> progress)
        {

            return Task.Run(() => TransferTemp100(progress));
        }

        public void TransferTemp100(Progress<string> progress)
        {

            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(connStr))
                {
                    using (var cmd = new OracleCommand("DataControlMP.TransferTemp100", con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, connStr);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }
    }




}

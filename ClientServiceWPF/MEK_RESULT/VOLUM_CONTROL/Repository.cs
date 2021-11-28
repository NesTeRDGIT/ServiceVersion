using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServiceWPF.Class;
using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.MEK_RESULT.VOLUM_CONTROL
{

    public interface IRepositoryVolumeControl
    {
        Task<List<LIMIT_RESULTRow>> GET_VRAsync();
      
        Task<List<LIMIT_RESULTRow>> GET_VRAsync(int year, int month);
    }
    public class RepositoryVolumeControl : IRepositoryVolumeControl
    {

        public Task<List<LIMIT_RESULTRow>> GET_VRAsync()
        {
            return Task.Run(() =>
            {
                var oda = new OracleDataAdapter("select * from table(volum_control.GET_VR)", AppConfig.Property.ConnectionString);
                var tbl = new DataTable();
                oda.Fill(tbl);
                return LIMIT_RESULTRow.Get(tbl.Select());
            });
        }

        public Task<List<LIMIT_RESULTRow>> GET_VRAsync(int year,int month)
        {
            return Task.Run(() =>
            {
                var oda = new OracleDataAdapter("select * from table(volum_control.GET_VRPeriod(:year,:month))", AppConfig.Property.ConnectionString);
                oda.SelectCommand.Parameters.Add("year", year);
                oda.SelectCommand.Parameters.Add("month", month);
                var tbl = new DataTable();
                oda.Fill(tbl);
                return LIMIT_RESULTRow.Get(tbl.Select());
            });
        }


    }
}

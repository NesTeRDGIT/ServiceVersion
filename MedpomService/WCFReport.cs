using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLoaderMedpomData;

namespace MedpomService
{
    public partial class WcfInterface
    {
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
                AddLog($"Ошибка при запросе списка не подавших реестры: {ex.Message}", LogType.Error);
                return null;
            }
        }
    }
}

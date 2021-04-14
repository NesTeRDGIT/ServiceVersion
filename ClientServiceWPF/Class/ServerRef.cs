using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.Class
{
    public class ServerRef
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string DBAPrivilege { get; set; }
        public string HOST { get; set; }
        public string BD { get; set; }
        public string PORT { get; set; }

        private const string Match = @"^(?<HOST>.*):(?<PORT>.*)\/(?<BD>.*)$";


        private static Match ParceDataSource(string val)
        {
            var reg = new Regex(Match);
            return reg.Match(val);
        }


        public static ServerRef ParseDataSource(string connectionString)
        {
            var res = new ServerRef();
            var ocsb = new OracleConnectionStringBuilder(connectionString);
            res.UserID = ocsb.UserID;
            res.DBAPrivilege = ocsb.DBAPrivilege==""? "NORMAL" : ocsb.DBAPrivilege;
            res.Password = ocsb.Password;
            var Parce = ParceDataSource(ocsb.DataSource);
            res.HOST = Parce.Groups["HOST"].Value;
            res.PORT = Parce.Groups["PORT"].Value;
            res.BD = Parce.Groups["BD"].Value;
            return res;
        }

    }
}

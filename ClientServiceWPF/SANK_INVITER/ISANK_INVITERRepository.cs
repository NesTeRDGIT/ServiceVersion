using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.SANK_INVITER
{
    public interface ISANK_INVITERRepository
    {
        void OpenConnection();
        void CloseConnection();
        int? FindZGLV(string CODE_MO, int CODE, int YEAR, bool DOP_REESTR, DateTime DSCHET, string NSCHET);
    }


    public class SANK_INVITERRepository : ISANK_INVITERRepository
    {
        private readonly string ConnectionString;
        private OracleDataAdapter odaFindZGLV;
        private OracleConnection conn;
        public SANK_INVITERRepository(string ConnectionString, string xml_h_schet)
        {
            this.ConnectionString = ConnectionString;
            conn = new OracleConnection(ConnectionString);
            odaFindZGLV = new OracleDataAdapter($@"select zglv_id from {xml_h_schet} t where t.code_mo = :code_mo and t.code = :code and t.year_base = :year and nvl(t.dop_flag,0) = :DOP and t.DSCHET = :DSCHET and  t.NSCHET = :NSCHET", conn);
            odaFindZGLV.SelectCommand.Parameters.Add("CODE_MO", OracleDbType.Varchar2);
            odaFindZGLV.SelectCommand.Parameters.Add("CODE", OracleDbType.Decimal);
            odaFindZGLV.SelectCommand.Parameters.Add("YEAR", OracleDbType.Decimal);
            odaFindZGLV.SelectCommand.Parameters.Add("DOP", OracleDbType.Decimal);
            odaFindZGLV.SelectCommand.Parameters.Add("DSCHET", OracleDbType.Date);
            odaFindZGLV.SelectCommand.Parameters.Add("NSCHET", OracleDbType.Varchar2);
        }
        public void CloseConnection()
        {
            conn.Close();
        }
        public int? FindZGLV(string CODE_MO, int CODE,int YEAR,bool DOP_REESTR, DateTime DSCHET,string NSCHET)
        {
            var tbl = new DataTable();
            odaFindZGLV.SelectCommand.Parameters["CODE"].Value = CODE;
            odaFindZGLV.SelectCommand.Parameters["CODE_MO"].Value = CODE_MO;
            odaFindZGLV.SelectCommand.Parameters["YEAR"].Value = YEAR;
            odaFindZGLV.SelectCommand.Parameters["DOP"].Value = DOP_REESTR ? 1 : 0;
            odaFindZGLV.SelectCommand.Parameters["DSCHET"].Value = DSCHET;
            odaFindZGLV.SelectCommand.Parameters["NSCHET"].Value = NSCHET;
            odaFindZGLV.Fill(tbl);

            if (tbl.Rows.Count == 1)
                return Convert.ToInt32(tbl.Rows[0][0]);

            return null;
        }

        public void OpenConnection()
        {
            conn.Open();
        }
    }
}

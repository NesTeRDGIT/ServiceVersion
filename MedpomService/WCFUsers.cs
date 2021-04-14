using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using System.ServiceModel;

namespace MedpomService
{
    public partial class WcfInterface
    {
       
        public List<METHOD> Roles_GetMethod_NEW()
        {
            var tbl = new DataTable("Method");
            var sda = new OracleDataAdapter("select * from MEDPOM_EXIST_METHOD t", new OracleConnection(AppConfig.Property.ConnectionString));
            sda.Fill(tbl);
            return METHOD.Get(tbl.Select());
        }
        public void Roles_EditMethod_NEW(TypeEdit te, List<METHOD> items)
        {
            try
            {
                switch (te)
                {
                    case TypeEdit.Delete: items.ForEach(Roles_DeleteMethod); break;
                    case TypeEdit.New: items.ForEach(Roles_AddMethod); break;
                    case TypeEdit.Update: items.ForEach(Roles_UpdateMethod); break;
                }
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Roles_EditMethod: {ex.Message}", EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_EditMethod подробнее в логах сервиса");
            }
        }
        public List<ROLES> Roles_GetRoles_NEW()
        {
            try
            {
                var tbl = new DataTable("ROLES");
                var sda = new OracleDataAdapter("select * from medpom_client_roles t", new OracleConnection(AppConfig.Property.ConnectionString));
                sda.Fill(tbl);
                var role_method = new DataTable();
                sda = new OracleDataAdapter(@"select * from medpom_client_claims", new OracleConnection(AppConfig.Property.ConnectionString));
                sda.Fill(role_method);
                var ro = ROLES.Get(tbl.Select());
                foreach (var r in ro)
                {
                    r.METHOD = role_method.Select($"ROLE_ID = {r.ID}").Select(x => Convert.ToInt32(x["CLAIMS_ID"])).ToList();
                }

                return ro;
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_GetRoles: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_GetRoles подробнее в логах сервиса");
            }
        }
        public void Roles_EditRoles_NEW(TypeEdit te, List<ROLES> items)
        {
            try
            {
                switch (te)
                {
                    case TypeEdit.Delete: items.ForEach(Roles_DeleteRoles);  break;
                    case TypeEdit.New: items.ForEach(Roles_AddRoles); break;
                    case TypeEdit.Update: items.ForEach(Roles_UpdateRoles);  break;
                }
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_EditRoles: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_EditRoles подробнее в логах сервиса");
            }
        }
        public List<USERS> Roles_GetUsers_NEW()
        {
            try
            {
                var tbl = new DataTable();
                var sda = new OracleDataAdapter(@"select * from medpom_client_users", new OracleConnection(AppConfig.Property.ConnectionString));
                sda.Fill(tbl);
                var user_role = new DataTable();
                sda = new OracleDataAdapter(@"select * from MEDPOM_CLIENT_US_ROL", new OracleConnection(AppConfig.Property.ConnectionString));
                sda.Fill(user_role);
                var us = USERS.Get(tbl.Select());
                foreach (var u in us)
                {
                    u.ROLES = user_role.Select($"USER_ID = {u.ID}").Select(x => Convert.ToInt32(x["ROLE_ID"])).ToList();
                }
                return us;
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_GetUsers: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_GetUsers подробнее в логах сервиса");
            }
        }
        public void Roles_EditUsers_NEW(TypeEdit te, List<USERS> items)
        {
            try
            {
                switch (te)
                {
                    case TypeEdit.New: items.ForEach(Roles_AddUsers); break;
                    case TypeEdit.Update: items.ForEach(Roles_UpdateUsers); break;
                }
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_EditUsers: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_EditUsers подробнее в логах сервиса");
            }
        }
        private void Roles_AddMethod(METHOD item)
        {
            try
            {
                var cmd = new OracleCommand(@"insert into medpom_exist_method (name, coment) values   (:name, :coment)", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("name", item.NAME);
                cmd.Parameters.Add("coment", item.COMENT);
                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_AddMethod: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_AddMethod подробнее в логах сервиса");
            }
        }
        private void Roles_DeleteMethod(METHOD item)
        {
            try
            {
                var cmd = new OracleCommand(@"delete medpom_exist_method where id = :id", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("id", item.ID);
                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_DeleteMethod: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_DeleteMethod подробнее в логах сервиса");
            }
        }
        private void Roles_UpdateMethod(METHOD item)
        {
            try
            {
                var cmd = new OracleCommand(@"update medpom_exist_method
   set name = :name,
       coment = :coment
 where id = :id", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("name", item.NAME);
                cmd.Parameters.Add("coment", item.COMENT);
                cmd.Parameters.Add("id", item.ID);
                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();

            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_UpdateMethod: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_UpdateMethod подробнее в логах сервиса");
            }
        }
        private void Roles_AddRoles(ROLES item)
        {
            try
            {
                var cmd = new OracleCommand(@"insert into medpom_client_roles
  ( role_name, role_comment)
values
  (:role_name, :role_comment)  RETURNING id INTO :id", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("role_name", item.ROLE_NAME);
                cmd.Parameters.Add("role_comment", item.ROLE_COMMENT);
                cmd.Parameters.Add("ID", OracleDbType.Decimal, ParameterDirection.Output);



                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();



                item.ID = Convert.ToInt32(((Oracle.ManagedDataAccess.Types.OracleDecimal)cmd.Parameters["ID"].Value).Value);
                Roles_Update_Roles_Method(item.ID, item.METHOD.ToArray());
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Roles_AddRoles: {ex.Message}", EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_AddRoles подробнее в логах сервиса");
            }
        }
        private void Roles_DeleteRoles(ROLES item)
        {
            try
            {
                var cmd = new OracleCommand(@"delete medpom_client_roles  where id = :id", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("id", item.ID);
                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog($"Roles_DeleteRoles: {ex.Message}", EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_DeleteRoles подробнее в логах сервиса");
            }
        }
        private void Roles_UpdateRoles(ROLES item)
        {
            try
            {
                var cmd = new OracleCommand(@"update medpom_client_roles  set ROLE_NAME = :ROLE_NAME,  ROLE_COMMENT = :ROLE_COMMENT  where id = :id
", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("ROLE_NAME", item.ROLE_NAME);
                cmd.Parameters.Add("ROLE_COMMENT", item.ROLE_COMMENT);
                cmd.Parameters.Add("id", item.ID);
                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();

                Roles_Update_Roles_Method(item.ID, item.METHOD.ToArray());
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_UpdateRoles: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_UpdateRoles подробнее в логах сервиса");
            }
        }
        private void Roles_AddUsers(USERS item)
        {
            try
            {
                var cmd = new OracleCommand(@"insert into MEDPOM_CLIENT_USERS
  (NAME, PASS)
values
  (:NAME, :PASS)
  RETURNING id INTO :id", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("NAME", item.NAME.ToUpper());
                cmd.Parameters.Add("PASS", item.PASS);
                cmd.Parameters.Add("id", OracleDbType.Decimal, ParameterDirection.Output);

                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();
                var id = ((Oracle.ManagedDataAccess.Types.OracleDecimal)cmd.Parameters["id"].Value).Value;
                item.ID = Convert.ToInt32(id);
                Roles_UpdateUsers_Role(item.ID, item.ROLES.ToArray());
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_AddUsers: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_AddUsers подробнее в логах сервиса");
            }
        }
        private void Roles_UpdateUsers(USERS item)
        {
            try
            {
                var cmd = new OracleCommand(@"update MEDPOM_CLIENT_USERS
   set 
NAME = :NAME, 
PASS = :PASS
 where id = :id
", new OracleConnection(AppConfig.Property.ConnectionString));
                cmd.Parameters.Add("NAME", item.NAME.ToUpper());
                cmd.Parameters.Add("PASS", item.PASS);
                cmd.Parameters.Add("id", item.ID);
                cmd.Connection.Open();
                cmd.ExecuteScalar();
                cmd.Connection.Close();

                Roles_UpdateUsers_Role(item.ID, item.ROLES.ToArray());
            }
            catch (Exception ex)
            {
                WcfInterface.AddLog("Roles_UpdateUsers: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_UpdateUsers подробнее в логах сервиса");
            }
        }
        private void Roles_UpdateUsers_Role(int user_id, int[] role_id)
        {
            OracleConnection CONN = null;
            OracleTransaction TRAN = null;
            try
            {
                CONN = new OracleConnection(AppConfig.Property.ConnectionString);
                CONN.Open();
                TRAN = CONN.BeginTransaction();
                var cmd = new OracleCommand(@" delete medpom_client_us_rol where user_id = :user_id", CONN) { Transaction = TRAN };
                cmd.Parameters.Add("user_id", user_id);
                cmd.ExecuteScalar();

                if (role_id.Length != 0)
                {
                    cmd = new OracleCommand(@" insert into medpom_client_us_rol (user_id,role_id) values (:user_id,:role_id)", CONN) { ArrayBindCount = role_id.Length, Transaction = TRAN };
                    cmd.Parameters.Add("", OracleDbType.Int32, role_id.Select(x => user_id).ToArray(), ParameterDirection.Input);
                    cmd.Parameters.Add("", OracleDbType.Int32, role_id, ParameterDirection.Input);
                    cmd.ExecuteScalar();

                }
                TRAN.Commit();
                CONN.Close();
            }
            catch (Exception ex)
            {
                TRAN?.Rollback();
                CONN?.Dispose();
                WcfInterface.AddLog("Roles_UpdateUsers_Role: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_UpdateUsers_Role подробнее в логах сервиса");
            }

        }
        private void Roles_Update_Roles_Method(int role_id, int[] method_id)
        {
            OracleConnection CONN = null;
            OracleTransaction TRAN = null;
            try
            {
                CONN = new OracleConnection(AppConfig.Property.ConnectionString);
                CONN.Open();
                TRAN = CONN.BeginTransaction();

                var cmd = new OracleCommand(@" delete MEDPOM_CLIENT_CLAIMS where ROLE_ID = :ROLE_ID", CONN) { Transaction = TRAN }; ;
                cmd.Parameters.Add("ROLE_ID", role_id);

                cmd.ExecuteScalar();

                if (method_id.Length != 0)
                {
                    cmd = new OracleCommand(@"insert into MEDPOM_CLIENT_CLAIMS (CLAIMS_ID,ROLE_ID) values (:CLAIMS_ID,:ROLE_ID)", CONN)
                    {
                        ArrayBindCount = method_id.Length,
                        Transaction = TRAN
                    };
                    cmd.Parameters.Add("", OracleDbType.Int32, method_id, ParameterDirection.Input);
                    cmd.Parameters.Add("", OracleDbType.Int32, method_id.Select(x => role_id).ToArray(), ParameterDirection.Input);
                    cmd.ExecuteScalar();
                }

                TRAN.Commit();
                CONN.Close();
            }
            catch (Exception ex)
            {
                TRAN?.Rollback();
                CONN?.Dispose();
                WcfInterface.AddLog("Roles_Update_Roles_Method: " + ex.Message, EventLogEntryType.Error);
                throw new FaultException("Ошибка в Roles_Update_Roles_Method подробнее в логах сервиса");
            }
        }
    }



}

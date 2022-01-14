using System;
using System.Collections.Generic;
using System.Linq;
using ServiceLoaderMedpomData;
using System.ServiceModel;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Diagnostics;
using System.IdentityModel.Selectors;
using System.IdentityModel.Policy;
using System.IdentityModel.Claims;
using System.Security.Principal;

namespace MedpomService
{





    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,IncludeExceptionDetailInFaults = true,ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class WcfInterface : IWcfInterface
    {
        private IProcessReestr processReestr;
        private ISchemaCheck SchemaCheck;
        private IFileInviter FileInviter;
        private IPacketQuery PacketQuery;
        private ILogger Logger;
        private MyOracleProvider MyOracleProvider;
        private IRepositoryCheckingList repositoryCheckingList { get; }
        public WcfInterface(IProcessReestr processReestr, ISchemaCheck SchemaCheck, IFileInviter FileInviter, IPacketQuery PacketQuery, ILogger Logger)
        {
            this.processReestr = processReestr;
            this.SchemaCheck = SchemaCheck;
            this.FileInviter = FileInviter;
            this.PacketQuery = PacketQuery;
            this.Logger = Logger;
            MyOracleProvider = new MyOracleProvider(Logger);
        }

        private USER GETUSER => BankAcc.GetUSER(OperationContext.Current.ServiceSecurityContext.PrimaryIdentity.Name.ToUpper());
        private string PathEXE => Process.GetCurrentProcess().MainModule?.FileName;
      
        public bool Ping()
        {
            return true;
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
                    throw new FaultException($"Пользователь {userName} уже подключен!");
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
                AddLog($"Connect: {ex.Message}", LogType.Error);
                throw new FaultException("Ошибка в Connect подробнее в логах сервиса");
            }
        }

        /// <summary>
        /// Добавить лог
        /// </summary>
        /// <param name="log">Текст сообщения</param>
        /// <param name="type">Тип сообщения</param>
        private void AddLog(string log, LogType type)
        {
            Logger.AddLog(log, type);
        }


    }





    public class MyCustomUserNameValidator : UserNamePasswordValidator
    {
        private MyOracleProvider MyOracleProvider;
        public MyCustomUserNameValidator(MyOracleProvider MyOracleProvider)
        {
            this.MyOracleProvider = MyOracleProvider;
        }
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
        private ILogger Logger;

        public MyOracleProvider(ILogger Logger)
        {
            this.Logger = Logger;
        }
        public  bool CheckUser(string USER, string PASS, out int ID)
        {
            try
            {
                var oda = new OracleDataAdapter("select * from MEDPOM_CLIENT_USERS t where upper(t.name) = '" + USER.ToUpper() + "' and pass = '" + PASS+"'", new OracleConnection(AppConfig.Property.ConnectionString));
                var tbl = new DataTable();
                oda.Fill(tbl);
                if (tbl.Rows.Count == 0)
                {
                    ID = -1;
                    return false;
                }
                ID = Convert.ToInt32(tbl.Rows[0]["ID"]);
                return true;
            }
            catch (Exception ex)
            {
                Logger.AddLog("Ошибка проверки пользователя: " + ex.Message, LogType.Error);
                ID = -1;
                return false;       
            }
        }
        public  List<string> GetSecurityCard(string name)
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
                Logger.AddLog("Ошибка получения карточки: " + ex.Message, LogType.Error);
                return new List<string>();              
            }
        }

        private List<string> GetFullRight()
        {
            var Met = ReflectClass.MethodReflectInfo<IWcfInterface>();
            return Met.Select(mi => mi.Name).ToList();
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

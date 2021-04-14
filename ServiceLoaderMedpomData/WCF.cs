using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Diagnostics;


namespace ServiceLoaderMedpomData
{
    /// <summary>
    /// Интерефейся WCF. Для взаимодействия клиента со службой
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IWcfInterfaceCallback),
         SessionMode = SessionMode.Required)]
    public interface IWcfInterface
    {
        /// <summary>
        /// Получить лист пакетов 
        /// </summary>
        /// <returns>Лист пакетов</returns>
        [OperationContract]
        List<FilePacket> GetFileManagerList();
       
        /// <summary>
        /// Настроить конфигурацию папок
        /// </summary>
        /// <param name="set">Конфигурация папок</param>
        [OperationContract]
        void SettingsFolder(SettingsFolder set);
        /// <summary>
        /// Настроить конфигурацию подключения
        /// </summary>
        /// <param name="set">Конфигурация подключения</param>
        [OperationContract]
        void SettingConnect(SettingConnect set);
        /// <summary>
        /// Получить конфигурацию папок
        /// </summary>
        /// <returns>Конфигурация</returns>
        [OperationContract]
        SettingsFolder GetSettingsFolder();
        /// <summary>
        /// Получить конфигурацию подключения
        /// </summary>
        /// <returns>Конфигурация</returns>
        [OperationContract]
        SettingConnect GetSettingConnect();
        /// <summary>
        /// Проверить подключение
        /// </summary>
        /// <param name="connectionstring">Строка подключения</param>
        /// <returns>Результат</returns>
        [OperationContract]
        BoolResult isConnect(string connectionstring);//
        /// <summary>
        /// Получить список таблиц
        /// </summary>
        /// <returns>Таблица с коментарием</returns>
        [OperationContract]
        TableResult GetTableServer(string OWNER);
        /// <summary>
        /// Сохранить свойства службы
        /// </summary>
        [OperationContract]
        void SaveProperty();
        /// <summary>
        /// Загрузить свойства службы
        /// </summary>
        [OperationContract]
        void LoadProperty();
        /// <summary>
        /// Остановить обработку
        /// </summary>
        /// <returns>Удачно или нет</returns>
        [OperationContract]
        BoolResult StopProccess();
        /// <summary>
        /// Запуск обработки
        /// </summary>
        /// <returns>Удачно или нет</returns>
        [OperationContract]
        BoolResult StartProccess(bool MainPriem, bool Auto, DateTime dt);

        /// <summary>
        /// Управление авто приемом
        /// </summary>
        /// <param name="Auto">Включить или выключить</param>
        /// <returns></returns>
        [OperationContract]
        void SetAutoPriem(bool Auto);

        [OperationContract]
        bool GetTypePriem();

        [OperationContract]
        DateTime GetOtchetDate();
        [OperationContract]
        bool GetAutoPriem();
        /// <summary>
        /// Получить колекцию схем
        /// </summary>
        /// <returns>Класс колекция схем</returns>
        [OperationContract]
        SchemaColection GetSchemaColection();
        /// <summary>
        /// Задать коллекцию схем
        /// </summary>
        /// <param name="sc">Класс колекция схем</param>
        [OperationContract]
        void SettingSchemaColection(SchemaColection sc);

        /// <summary>
        /// Установить список процедур для переноса 
        /// </summary>
        /// <param name="list"></param>
        [OperationContract]
        void SetListTransfer(List<OrclProcedure> list, string ProcClear, string ProcClearTransfer, string StatusProc, string StatusProcTransfer);
        /// <summary>
        /// Получить список процедур для переноса
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<OrclProcedure> GetListTransfer();
        /// <summary>
        /// Остановить время ожидания
        /// </summary>
        /// <param name="index"></param>
        [OperationContract]
         void StopTimeAway(int index);
        /// <summary>
        /// Выполнить функцию переноса
        /// </summary>
        /// <param name="x">Порядок функции </param>
        /// <param name="y">0 - если проверка 1 если выполнить</param>
        [OperationContract]
        void RunProcListTransfer(int x, int y);
        /// <summary>
        /// Статус обработки архивов
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool ArchiveInviterStatus();
        /// <summary>
        /// Статус обработки файлов
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool FilesInviterStatus();//
        /// <summary>
        /// Статус обработки флк пакетов
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool FLKInviterStatus();
        /// <summary>
        /// Пинг для проверки службы WCF
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<string> Connect();

        /// <summary>
        /// Получить список дириктории
        /// </summary>
        /// <param name="path">Путь</param>
        /// <returns>Список директорий</returns>
        [OperationContract]
        string[] GetFolderLocal(string path);
        /// <summary>
        /// Получиться список файлов
        /// </summary>
        /// <param name="path">Путь к каталогу</param>
        /// <param name="pattern">Шаблон файлов (*.*,*.txt....)</param>
        /// <returns></returns>
        [OperationContract]
        string[] GetFilesLocal(string path, string pattern);
        /// <summary>
        /// Получиться список локальных дисков
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string[] GetLocalDisk();
        /// <summary>
        /// Получить лог программы
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        EntriesMy[] GetEventLogEntry(int count);
        /// <summary>
        /// Отчистка лога
        /// </summary>
        [OperationContract]
        void ClearEventLogEntry();
        /// <summary>
        /// Получить список проверок
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ChekingList GetChekingList();
        /// <summary>
        /// Задать список проверок
        /// </summary>
        /// <param name="list">Список</param>
        /// <returns></returns>
        [OperationContract]
        BoolResult SetChekingList(ChekingList list);
        /// <summary>
        /// Получить список процедур из Пакета Oracle
        /// </summary>
        /// <param name="name">Имя пакета</param>
        /// <returns></returns>
        [OperationContract]
        List<OrclProcedure> GetProcedureFromPack(string name);
        /// <summary>
        /// Получить список параметров из процедуры Oracle
        /// </summary>
        /// <param name="name">Имя процедуры</param>
        /// <returns></returns>
        [OperationContract]
        List<OrclParam> GetParam(string name);
        /// <summary>
        /// Выполнить проверки
        /// </summary>
        /// <param name="check">Список проверок</param>
        /// <returns></returns>
        [OperationContract]
        ChekingList ExecuteCheckAv(ChekingList check);
        /// <summary>
        /// Загрузить проверки из БД
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BoolResult LoadChekListFromBD();
        /// <summary>
        /// Очистка журнала
        /// </summary>
        
        [OperationContract]
        void SetSettingTransfer(SettingTransfer st);
        [OperationContract]
        SettingTransfer GetSettingTransfer();
        [OperationContract]
        TableResult GetTableTransfer();
        [OperationContract]
        void ClearFileManagerList();
        [OperationContract]
        bool SetPriority(int index, int priority);
        [OperationContract]
        bool DelPack(int index);
        [OperationContract]
        void SetUserPriv(string value);
        [OperationContract]
        bool CheckUserPriv(string value);
        [OperationContract]
        string GetUserPriv();
   
        [OperationContract]
        DataTable GetNotReestr();
      
        [OperationContract]
        DataTable GetSVOD_SMO_TEMP1(DataTable tbl);

        [OperationContract]
        DataTable GetSVOD_SMO_TEMP100(DataTable tbl);
        [OperationContract]
        DataTable GetSVOD_DISP_TEMP100(DataTable tbl);
        [OperationContract]
        DataTable GetSVOD_DISP_TEMP1(DataTable tbl);
        [OperationContract]
        DataTable GetSVOD_VMP_TEMP1(DataTable tbl);
        [OperationContract]
        DataTable GetSVOD_VMP_TEMP100(DataTable tbl);
        [OperationContract]
        DataTable GetSVOD_SMP_TEMP1(DataTable tbl);
        [OperationContract]
        DataTable GetSVOD_SMP_TEMP100(DataTable tbl);
        [OperationContract]
        DataTable GetSVOD_DISP_ITOG(DataTable tbl, int YEAR);
        [OperationContract]
        DataTable GetSVOD_SMP_ITOG(DataTable tbl, int YEAR);
        [OperationContract]
        DataTable GetSVOD_VMP_ITOG(DataTable tbl, int YEAR);
        [OperationContract]
        DataTable Roles_GetMethod();
       
        [OperationContract]
        void Roles_AddMethod(string Name, string Comm);
        [OperationContract]
        void Roles_DeleteMethod(int id);
        [OperationContract]
        void Roles_UpdateMethod(string Name, string Coment, int id);
        [OperationContract]
        DataTable Roles_GetRoles();
        [OperationContract]
        int Roles_AddRoles(string Name, string Comment);
        [OperationContract]
        void Roles_DeleteRoles(int id);
        [OperationContract]
        void Roles_UpdateRoles(string Name, string Comment, int id);
        [OperationContract]
        DataTable Roles_GetRolesClaims();
        [OperationContract]
        void Roles_AddClaims(int role_id, int claims_id);
        [OperationContract]
        void Roles_DeleteClaims(int role_id, int claims_id);
        [OperationContract]
        void Roles_UpdateClaims(int role_id, int claims_id, int old_role_id, int old_claims_id);


        [OperationContract]
        DataTable Roles_GetUsers();
        [OperationContract]
        int Roles_AddUsers(string Name, string password);
        [OperationContract]
        void Roles_DeleteUsers(int id);
        [OperationContract]
        void Roles_UpdateUsers(string Name, string password, int id);


        [OperationContract]
        DataTable Roles_GetUsers_Roles();
        [OperationContract]
        void Roles_AddUsers_Role(int user_id, int role_id);
        [OperationContract]
        void Roles_DeleteUsers_Role(int user_id, int role_id);





     



        [OperationContract]
        void SaveProcessArch();

        [OperationContract]
        ProgressClass GetProgressClassProcessArch();
        /// <summary>
        /// Авто захват файлов
        /// </summary>
        [OperationContract]
        bool GetAutoFileAdd();


        /* [OperationContract]
         ImpersonInfo GetImpersonInfo();
         [OperationContract]
         void SetImpersonInfo(ImpersonInfo ii);
         [OperationContract]
         bool CheckImpersonInfo(ImpersonInfo ii);
         */
        [OperationContract]
        byte[] GetFile(string CODE_MO, int FILE, TypeDOWLOAD type, long offset);

        [OperationContract]
        long GetFileLength(string CODE_MO, int FILE, TypeDOWLOAD type);

        [OperationContract]
        void AddListFile(List<string> List);
        [OperationContract]
        string CheckTableTemp1();
        [OperationContract]
        void ClearBaseTemp1();
        [OperationContract]
        List<string> GetCheckClearProc();

        [OperationContract]
        string CheckTableTemp100();
        [OperationContract]
        void ClearBaseTemp100();


        #region Счета фактуры
        [OperationContract]
        DataTable GetID_SPOSOB();
        [OperationContract]
        DataTable GetVIDMP();
        [OperationContract]
        DataTable GetMUR_FIN();
        [OperationContract]
        DataTable GetMUR_FIN_SMP();
        [OperationContract]
        DataTable Getf003();
        [OperationContract]
        DataTable Getf002();
        [OperationContract]
        DataTable GetV_XML_H_FAKTURA();

        [OperationContract]
        void RepeatClosePac(int[] index);
        #endregion
        [OperationContract]
        void BreackProcessPac(int index);
        [OperationContract]
        ServiceLoaderMedpomData.Version GetVersion();

        [OperationContract]
        byte[] LoadFileUpdate(FileAndMD5 file, int offset, int count);

        [OperationContract]
        bool Ping();


        [OperationContract]
        FilePacketAndOrder GetPackForMO(string code_mo);
        [OperationContract]
        void AddFilePacketForMO(FilePacket fp);

        /// <summary>
        /// Подписаться на событие NewFileManager 
        /// </summary>
        [OperationContract]
        void RegisterNewFileManager();
        /// <summary>
        /// Отписаться от событие NewFileManager 
        /// </summary>
        [OperationContract]
        void UnRegisterNewFileManager();







        [OperationContract]
        void Roles_EditUsers_NEW(TypeEdit te, List<USERS> items);

        [OperationContract]
        List<USERS> Roles_GetUsers_NEW();

        [OperationContract]
        void Roles_EditRoles_NEW(TypeEdit te, List<ROLES> items);

        [OperationContract]
        List<ROLES> Roles_GetRoles_NEW();

        [OperationContract]
        void Roles_EditMethod_NEW(TypeEdit te, List<METHOD> items);

        [OperationContract]
        List<METHOD> Roles_GetMethod_NEW();

    }


    [ServiceContract]
    public interface IWcfInterfaceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NewNotifi();
        [OperationContract(IsOneWay = true)]
        void NewPackState(string CODE_MO);
        [OperationContract(IsOneWay = true)]
        void NewFileManager();
        [OperationContract(IsOneWay = true)]
        void PING();

    }
    public class ProgressClass
    {
        public bool Active = false;
        public int Max;
        public int Value;
        public string TXT;
    }

    public  enum TypeDOWLOAD
    {
        FileL,
        File,
        FILE_STAT,
        FILE_LOG,
        FILE_L_LOG,      
        XML_OTCHET_H,
        XML_OTCHET_L,
        ZIP_ARCHIVE

    }

    /// <summary>
    /// Настройки папок
    /// </summary>
    public class SettingsFolder
    {
        public string IncomingDir;
        public string InputDir;
 
        public string ProcessDir;
        public int TimePacketOpen;
        public string ErrorDir;
        public string ErrorMessageFile;
        public string AddDIRInERROR;
        public string ISP;
       
    }
    /// <summary>
    /// Настройки подключения
    /// </summary>
    public class SettingConnect
    {
        public string ConnectingString;
        public string xml_h_pacient;
        public string xml_h_sank_smo;
        public string xml_h_schet;
        public string xml_h_sluch;
        public string xml_h_usl;
        public string xml_h_zap;
        public string xml_h_zglv;
        public string xml_l_pers;
        public string xml_l_zglv;
        public string v_xml_error;
        public string xml_h_nazr;
        public string xml_h_ds2_n;
        public string xml_h_kslp;
        public string xml_h_z_sluch;

        public string xml_h_cons;
        public string xml_h_onk_usl;
        public string xml_h_lek_pr;
        public string xml_h_lek_pr_date_inj;

        


        public string xml_h_b_diag;
        public string xml_h_b_prot;
        public string xml_h_napr;

        public string xml_h_sank_code_exp;
        public string xml_h_ds2;
        public string xml_h_ds3;
        public string xml_h_crit;

        public string schemaOracle;
    }

    public class SettingTransfer
    {
        public string xml_h_pacient;
        public string xml_h_sank_smo;
        public string xml_h_schet;
        public string xml_h_sluch;
        public string xml_h_usl;
        public string xml_h_zap;
        public string xml_h_zglv;
        public string xml_l_pers;
        public string xml_l_zglv;
        public string schemaOracle;
        public string xml_h_ds2_n_transfer;
        public string xml_h_nazr_transfer;
        public string xml_h_kslp;
        public string xml_h_z_sluch;
       
        public string xml_h_b_diag;
        public string xml_h_b_prot;
        public string xml_h_napr;

        public string xml_h_cons;
        public string xml_h_onk_usl;
        public string xml_h_lek_pr;
        public string xml_h_lek_pr_date_inj;
        public string xml_h_sank_code_exp;
        public string xml_h_ds2;
        public string xml_h_ds3;
        public string xml_h_crit;
        public bool Transfer;
    }


    /// <summary>
    /// Результат bool с коментарием
    /// </summary>
    public class BoolResult
    {
        public bool Result;
        public string Exception;
    }
    /// <summary>
    /// Результат таблица с коментарием
    /// </summary>
    public class TableResult
    {
        public DataTable Result;
        public string Exception;
    }
    /// <summary>
    /// Установки месяца
    /// </summary>
    public class MonthSet
    {
        public int month;
        public bool autoflag;
    }
    /// <summary>
    /// Тип сообщения
    /// </summary>
    public enum TypeEntries
    {
        message = 0,
        error = 1,
        warning = 2
    }
    /// <summary>
    /// Мой Entries упрощеный
    /// </summary>
    public class EntriesMy
    {
         public DateTime TimeGenerated { get; set; }
         public string Message { get; set; }
         public TypeEntries Type { get; set; } 
    }

     public   enum TypeInvite
    {
        SMO,
        MO
    }

     public class ImpersonInfo
     {
         public string Domen { get; set; }
         public string Login { get; set; }
         public string Password { get; set; }
     }



     [DataContractFormat]
     public class USER
     {
         public int ID;
         public string NAME;
         public string FAM = "";
         public string IM = "";
         public string OT = "";
         public string FIO => $"{FAM.ToUpper()} {IM.ToUpper()} {OT.ToUpper()}".Trim();

         public string FAMandInitial =>$"{FAM.ToUpper()} {IM.Substring(0, 1).ToUpper()}.{OT.Substring(0, 1).ToUpper()}.".Trim();

         public List<string> list;
         public string Comment = "";
         public OperationContext Context;
        public bool IsOpen
        {
            get
            {
                if (Context == null) return false;
                if (Context.Channel.State != CommunicationState.Opened)
                    return false;
                try
                {
                    Context.GetCallbackChannel<IWcfInterfaceCallback>().PING();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }

     public class USER_STATUS
     {
         public int ID { get; set; }
         public string NAME { get; set; }
         public string Comment { get; set; }
         public CommunicationState State { get; set; }
     }

     public class CancelException:Exception
     {

     }
}

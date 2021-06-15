using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Data;
using System.Runtime.Serialization;


namespace ServiceLoaderMedpomData
{
    /// <summary>
    /// Интерфейс WCF. Для взаимодействия клиента со службой
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IWcfInterfaceCallback),SessionMode = SessionMode.Required)]
    public interface IWcfInterface
    {
        #region  Управление обработкой
        /// <summary>
        /// Получить список пакетов
        /// </summary>
        /// <returns>Лист пакетов</returns>
        [OperationContract]
        List<FilePacket> GetFileManagerList();
        /// <summary>
        /// Очистка списка пакетов
        /// </summary>
        [OperationContract]
        void ClearFileManagerList();
        /// <summary>
        /// Остановить обработку
        /// </summary>
        /// <returns>Удачно или нет</returns>
        [OperationContract]
        BoolResult StopProcess();
        /// <summary>
        /// Запуск обработки
        /// </summary>
        /// <returns>Удачно или нет</returns>
        [OperationContract]
        BoolResult StartProcess(bool MainPriem, bool Auto, DateTime dt);
        /// <summary>
        /// Управление авто приемом
        /// </summary>
        /// <param name="Auto">Включить или выключить</param>
        /// <returns></returns>
        [OperationContract]
        void SetAutoPriem(bool Auto);
        /// <summary>
        /// Получить статус приема
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        StatusPriem GetStatusInvite();
        /// <summary>
        /// Получить лог программы
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        EntriesMy[] GetEventLogEntry(int count);
        /// <summary>
        /// Очистка лога
        /// </summary>
        [OperationContract]
        void ClearEventLogEntry();
        /// <summary>
        /// Установить приоритет обработки для пакета
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="priority"></param>
        /// <returns></returns>
        [OperationContract]
        bool SetPriority(Guid guid, int priority);
        /// <summary>
        /// Удалить пакет
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [OperationContract]
        bool DelPack(Guid guid);
        /// <summary>
        /// Сохранение папки обработки
        /// </summary>
        [OperationContract]
        void SaveProcessArch();
        /// <summary>
        /// Прогресс процесса сохранения папки обработки
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ProgressClass GetProgressClassProcessArch();
        /// <summary>
        /// Добавить файлы в обработку
        /// </summary>
        /// <param name="List"></param>
        [OperationContract]
        void AddListFile(List<string> List);
        /// <summary>
        /// Повторить обработку пакета
        /// </summary>
        /// <param name="guid"></param>
        [OperationContract]
        void RepeatClosePac(Guid[] guid);
        /// <summary>
        /// Прервать обработку пакета
        /// </summary>
        /// <param name="guid"></param>
        [OperationContract]
        void BreakProcessPac(Guid guid);
        /// <summary>
        /// Подписаться на событие NewFileManager(изменения листа пакетов)
        /// </summary>
        [OperationContract]
        void RegisterNewFileManager();
        /// <summary>
        /// Отписаться от событие NewFileManager(изменения листа пакетов)
        /// </summary>
        [OperationContract]
        void UnRegisterNewFileManager();
        #endregion

        #region Параметры работы
        /// <summary>
        /// Получить конфигурацию папок
        /// </summary>
        /// <returns>Конфигурация</returns>
        [OperationContract]
        SettingsFolder GetSettingsFolder();
        /// <summary>
        /// Настроить конфигурацию папок
        /// </summary>
        /// <param name="set">Конфигурация папок</param>
        [OperationContract]
        void SettingsFolder(SettingsFolder set);
        /// <summary>
        /// Получить конфигурацию подключения
        /// </summary>
        /// <returns>Конфигурация</returns>
        [OperationContract]
        SettingConnect GetSettingConnect();
        /// <summary>
        /// Настроить конфигурацию подключения
        /// </summary>
        /// <param name="set">Конфигурация подключения</param>
        [OperationContract]
        void SettingConnect(SettingConnect set);
        /// <summary>
        /// Проверить подключение
        /// </summary>
        /// <param name="connectionstring">Строка подключения</param>
        /// <returns>Результат</returns>
        [OperationContract]
        BoolResult isConnect(string connectionstring);
        /// <summary>
        /// Получить список таблиц
        /// </summary>
        /// <returns>Таблица с комментарием</returns>
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
        /// Получить коллекцию схем
        /// </summary>
        /// <returns>Класс коллекция схем</returns>
        [OperationContract]
        SchemaCollection GetSchemaCollection();
        /// <summary>
        /// Задать коллекцию схем
        /// </summary>
        /// <param name="sc">Класс коллекцию схем</param>
        [OperationContract]
        void SettingSchemaCollection(SchemaCollection sc);
        /// <summary>
        /// Установить время ожидания пакета
        /// </summary>
        /// <param name="index"></param>
        [OperationContract]
        void StopTimeAway(int index);
        /// <summary>
        /// Получить список проверок
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CheckingList GetCheckingList();
        /// <summary>
        /// Задать список проверок
        /// </summary>
        /// <param name="list">Список</param>
        /// <returns></returns>
        [OperationContract]
        BoolResult SetCheckingList(CheckingList list);
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
        /// Выполнить проверки(Тестирование запуска)
        /// </summary>
        /// <param name="check">Список проверок</param>
        /// <returns></returns>
        [OperationContract]
        CheckingList ExecuteCheckAv(CheckingList check);
        /// <summary>
        /// Загрузить проверки из БД
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        BoolResult LoadCheckListFromBD();
        /// <summary>
        /// Установить параметры переноса в TEMP100
        /// </summary>
        /// <param name="st"></param>
        [OperationContract]
        void SetSettingTransfer(SettingTransfer st);
        /// <summary>
        /// Получить параметры переноса в TEMP100
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        SettingTransfer GetSettingTransfer();
        #endregion

        #region Получение файловой системы сервера
        /// <summary>
        /// Получить список директории
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
      
        #endregion

        #region Отчеты
        /// <summary>
        /// Список МО не подавших реестры
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        DataTable GetNotReestr();
        #endregion

        #region Управление ролевой системой
        [OperationContract]
        void Roles_EditUsers(TypeEdit te, List<USERS> items);
        [OperationContract]
        List<USERS> Roles_GetUsers();
        [OperationContract]
        void Roles_EditRoles(TypeEdit te, List<ROLES> items);
        [OperationContract]
        List<ROLES> Roles_GetRoles();
        [OperationContract]
        void Roles_EditMethod(TypeEdit te, List<METHOD> items);
        [OperationContract]
        List<METHOD> Roles_GetMethod();
        #endregion

        #region Обновление программы
        /// <summary>
        /// Получить данные о файлах
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Version GetVersion();
        /// <summary>
        /// Загрузить файл
        /// </summary>
        /// <param name="file"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        byte[] LoadFileUpdate(FileAndMD5 file, int offset, int count);
        [OperationContract]
        byte[] GetFile(string CODE_MO, int FILE, TypeDOWLOAD type, long offset);
        [OperationContract]
        long GetFileLength(string CODE_MO, int FILE, TypeDOWLOAD type);

        #endregion

        #region Связь с сервисом
        /// <summary>
        /// Пинг для проверки службы WCF
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        bool Ping();
        /// <summary>
        /// Подключение
        /// </summary>
        /// <returns>Набор доступных методов</returns>
        [OperationContract]
        List<string> Connect();
        #endregion

        #region Взаимодейстиве с сайтом
        /// <summary>
        /// Получить текущий пакет для МО
        /// </summary>
        /// <param name="code_mo"></param>
        /// <returns></returns>
        [OperationContract]
        FilePacketAndOrder GetPackForMO(string code_mo);
        /// <summary>
        /// Добавить пакет на обработку
        /// </summary>
        /// <param name="fp"></param>
        [OperationContract]
        void AddFilePacketForMO(FilePacket fp);

        #endregion
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

     [DataContract]
     public class StatusPriem
     {
         public StatusPriem(bool TypePriem, DateTime OtchetDate, bool AutoPriem,bool ActiveAutoPriem, bool THArchiveInviter, bool FilesInviterStatus, bool FLKInviterStatus)
         {
             this.TypePriem = TypePriem;
             this.OtchetDate = OtchetDate;
             this.AutoPriem = AutoPriem;
             this.THArchiveInviter = THArchiveInviter;
             this.FilesInviterStatus = FilesInviterStatus;
             this.FLKInviterStatus = FLKInviterStatus;
             this.ActiveAutoPriem = ActiveAutoPriem;
         }
        /// <summary>
        /// Тип приема(True - Основной, False - предварительный)
        /// </summary>
        [DataMember]
        public bool TypePriem { get; set; }
        /// <summary>
        /// Отчетный период
        /// </summary>
        [DataMember]
        public DateTime OtchetDate { get; set; }
        /// <summary>
        /// Режим захвата файлов(True - автоматический, False - ручной)
        /// </summary>
        [DataMember]
        public bool AutoPriem { get; set; }
        /// <summary>
        /// Активность потока обработки архивов
        /// </summary>
        [DataMember]
        public bool THArchiveInviter { get; set; }
        /// <summary>
        /// Активность потока обработки файлов
        /// </summary>
        [DataMember]
        public bool FilesInviterStatus { get; set; }
        /// <summary>
        /// Активность потока обработки ФЛК
        /// </summary>
        [DataMember]
        public bool FLKInviterStatus { get; set; }
        /// <summary>
        /// Активность слушателя появления файлов
        /// </summary>
        [DataMember]
        public bool ActiveAutoPriem { get; set; }
     }
}

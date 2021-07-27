using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using System.Windows.Threading;
using System.Xml.Serialization;
using Ionic.Zip;
using ServiceLoaderMedpomData.Annotations;

namespace ServiceLoaderMedpomData
{
   

    #region Структуры для Парсера
    /// <summary>
    /// Вид PI
    /// </summary>
    public enum Penum
    {
        /// <summary>
        /// ТФОМС
        /// </summary>
        T = 0,
        /// <summary>
        /// Страховые организации
        /// </summary>
        S = 1,
        /// <summary>
        /// Медицинские организации
        /// </summary>
        M = 2
    }

    public static class ParseFileName
    {
        private static string patternType = "^(C|T|H|D[D|F|O|P|R|S|U|V|A|B]|L[T|D|F|O|P|R|S|U|V|C|A|B]|L)";
        private static string patternFROM = "^(M|T|S)";
        private static string patternFROM_N_TF = "^([0-9]{2,2})";
        private static string FirstOT = "M[0-9]{6,6}|T[0-9]{2,2}|S[0-9]{5,5}";
        private static string patternFROM_MO_TF = "^([0-9]{6,6})";
        private static string patternFROM_SMO = "([0-9]{5,5})";
        private static string patternSPACE = "^_";
        private static string patternNN = "^([0-9]+$)";
        private static string patternYY = "^([0-9]{2,2})";
        private static string patternMM = "^(0[1-9]|1[012])";
        public static MatchParseFileName Parse(string FileName)
        {
            FileName = Path.GetFileNameWithoutExtension(FileName);
            var res = new MatchParseFileName { FileName = FileName };
            //Ищем 1 отправителя(исключить косяк с форматами типа LS75001...считает за LS файл 
            int End_Type_Simbol = FileName.Length;
            var match = FindFirstOT(FileName);
            if (match.Success) End_Type_Simbol = match.Index;



            match = FindFILE_TYPE(FileName.Substring(0, End_Type_Simbol));
            if (!match.Success) return res;
            res.FILE_TYPE = match.Value;

            FileName = FileName.Remove(0, match.Length);
            match = FindORG(FileName);
            if (!match.Success) return res;
            res.Pi = match.Value.ToPenum();

            FileName = FileName.Remove(0, match.Length);
            match = FindORG_CODE(FileName, res.Pi);
            if (!match.Success) return res;
            res.Ni = match.Value;

            FileName = FileName.Remove(0, match.Length);
            match = FindORG(FileName);
            if (!match.Success) return res;
            res.Pp = match.Value.ToPenum();

            FileName = FileName.Remove(0, match.Length);
            match = FindORG_CODE(FileName, res.Pp);
            if (!match.Success) return res;
            res.Np = match.Value;


            FileName = FileName.Remove(0, match.Length);
            match = FindSpace(FileName);
            if (!match.Success) return res;


            FileName = FileName.Remove(0, match.Length);
            match = FindYY(FileName);
            if (!match.Success) return res;
            res.YY = match.Value;


            FileName = FileName.Remove(0, match.Length);
            match = FindMM(FileName);
            if (!match.Success) return res;
            res.MM = match.Value;


            FileName = FileName.Remove(0, match.Length);
            match = FindNN(FileName);
            if (!match.Success) return res;
            res.NN = match.Value;

            res.Success = true;
            return res;
        }


        private static Match FindFirstOT(string val)
        {
            var reg = new Regex(FirstOT);
            return reg.Match(val);
        }

        private static Match FindFILE_TYPE(string val)
        {
            var reg = new Regex(patternType);
            return reg.Match(val);
        }

        private static Match FindORG(string val)
        {
            var reg = new Regex(patternFROM);
            return reg.Match(val);
        }

        private static Match FindORG_CODE(string val, Penum? type)
        {
            if (!type.HasValue) throw new Exception("Ошибка в выборе маски кода организации");
            var pat = "";
            switch (type.Value)
            {
                case Penum.T: pat = patternFROM_N_TF; break;
                case Penum.M: pat = patternFROM_MO_TF; break;
                case Penum.S: pat = patternFROM_SMO; break;
            }
            var reg = new Regex(pat);
            return reg.Match(val);
        }

        private static Match FindSpace(string val)
        {
            var reg = new Regex(patternSPACE);
            return reg.Match(val);
        }

        private static Match FindYY(string val)
        {
            var reg = new Regex(patternYY);
            return reg.Match(val);
        }

        private static Match FindMM(string val)
        {
            var reg = new Regex(patternMM);
            return reg.Match(val);
        }

        private static Match FindNN(string val)
        {
            var reg = new Regex(patternNN);
            return reg.Match(val);
        }

        public static Penum? ToPenum(this string val)
        {
            switch (val)
            {
                case "T": return Penum.T;
                case "M": return Penum.M;
                case "S": return Penum.S;
            }

            return null;
        }


        public static string ToStr(this Penum? val)
        {
            return val?.ToString();
        }
        public static FileType ToFileType(this string val)
        {
            switch (val)
            {
                case "DD": return FileType.DD;
                case "DF": return FileType.DF;
                case "DO": return FileType.DO;
                case "DP": return FileType.DP;
                case "DR": return FileType.DR;
                case "DS": return FileType.DS;
                case "DU": return FileType.DU;
                case "DV": return FileType.DV;
                case "LD": return FileType.LD;
                case "LF": return FileType.LF;
                case "LO": return FileType.LO;
                case "LP": return FileType.LP;
                case "LR": return FileType.LR;
                case "LS": return FileType.LS;
                case "LU": return FileType.LU;
                case "LV": return FileType.LV;
                case "LT": return FileType.LT;
                case "LC": return FileType.LC;
                case "H": return FileType.H;
                case "T": return FileType.T;
                case "L": return FileType.LH;
                case "C": return FileType.C;
                case "DA": return FileType.DA;
                case "DB": return FileType.DB;
                case "LA": return FileType.LA;
                case "LB": return FileType.LB;
            }

            throw new Exception($"Не удалось получить тип файла из строки{val}");
        }

        public static bool Contains(this FileType val, params FileType[] list)
        {
            return list.Contains(val);
        }
    }


    public class MatchParseFileName
    {
        /// <summary>
        /// Успешное 
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Тип файла
        /// </summary>
        public string FILE_TYPE { get; set; }

        /// <summary>
        /// тип организации отправителя(T-ТФОМС,M-МО,S-СМО)
        /// </summary>
        public Penum? Pi { get; set; }
        /// <summary>
        /// номер организации отправителя
        /// </summary>
        public string Ni { get; set; }
        /// <summary>
        /// тип организации получателя(T-ТФОМС,M-МО,S-СМО)
        /// </summary>
        public Penum? Pp { get; set; }
        /// <summary>
        /// номер организации получателя
        /// </summary>
        public string Np { get; set; }
        /// <summary>
        ///  отчетный год
        /// </summary>
        public string YY { get; set; }
        /// <summary>
        /// отчетный месяц
        /// </summary>
        public string MM { get; set; }
        /// <summary>
        /// порядковый номер файла
        /// </summary>
        public string NN { get; set; }

        public bool IsNull => FileName == null || FILE_TYPE == null || Pi == null || Ni == null || Pp == null ||Np == null || YY == null || MM == null || NN == null;

        public string ErrText =>
$@"Наименование файла должно быть 
[Type][Pi][Ni][Pp][Np]_[YY][MM][NN], где
[Type] - тип файла,
[Pi] - тип организации отправителя(T-ТФОМС,M-МО,S-СМО),
[Ni] - номер организации отправителя,
[Pp] - тип организации получателя(T-ТФОМС,M-МО,S-СМО),
[Np] - номер организации получателя,
[YY] - отчетный год,
[MM] - отчетный месяц,
[NN] - порядковый номер файла.
В данном файле[{FileName}]:
[Type] - [{FILE_TYPE ?? "Нет значения"}]
[Pi] - [{Pi.ToStr() ?? "Нет значения"}]
[Ni] - [{Ni ?? "Нет значения"}]
[Pp] - [{Pp.ToStr() ?? "Нет значения"}]
[Np] - [{Np ?? "Нет значения"}],
[YY] - [{YY ?? "Нет значения"}],
[MM] - [{MM ?? "Нет значения"}],
[NN] - [{NN ?? "Нет значения"}].";

    }

   
    #endregion
    /// <summary>
    /// Тип файла
    /// </summary>
    [Serializable]
    public enum FileType//В модуле схема коллектор зашито их количество и в настройках и не трогать порядок
    {
        H = 0,
        T = 1,
        DP = 2,
        DV = 3,
        DO = 4,
        DS = 5,
        DU = 6,
        DF = 7,
        DD = 8,
        DR = 9,
        LH = 10,
        LT = 11,
        LP = 12,
        LV = 13,
        LO = 14,
        LS = 15,
        LU = 16,
        LF = 17,
        LD = 18,
        LR = 19,
        C = 20,
        LC = 21,
        DA = 22,
        DB = 23,
        LA = 24,
        LB = 25
    }

    /// <summary>
    /// Статус пакета
    /// </summary>
     [Serializable]
    public enum StatusFilePack
    {
        /// <summary>
        /// Открыт
        /// </summary>
        Open = 0,
        /// <summary>
        /// Закрыт
        /// </summary>
        Close = 1,
        /// <summary>
        /// Проверен на схему
        /// </summary>
        XMLSchemaOK = 2,
        /// <summary>
        /// ФЛК пройден
        /// </summary>
        FLKOK = 3,
        /// <summary>
        /// ФЛК ошибка
        /// </summary>
        FLKERR = 4
    }
    /// <summary>
    /// Шаг обработки файла
    /// </summary>
    [Serializable]
    public enum StepsProcess
    {
        /// <summary>
        /// Файл не принят
        /// </summary>
        NotInvite = 0,
        /// <summary>
        /// Файл принят
        /// </summary>
        Invite = 1,
        /// <summary>
        /// Схема верна
        /// </summary>
        XMLxsd = 2,
        /// <summary>
        /// Ошибка при проверки схемы
        /// </summary>
        ErrorXMLxsd = 3,
        /// <summary>
        /// Флк пройден
        /// </summary>
        FlkOk = 4,
        /// <summary>
        /// Ошибка ФЛК
        /// </summary>
        FlkErr = 5
    }
    [Serializable]
    public enum VersionMP
    {
        NONE = 0,        
        V2_1 = 1,
        V3_0 = 2,
        V3_1 = 3,
        V3_2 = 4
    }
    /// <summary>
    /// Элемент файл
    /// </summary>
    [Serializable]
    [DataContract]
    public class FileItemBase :INotifyPropertyChanged
    {
        public bool isD => FileName[0].ToString().ToUpper()[0] == 'D';
        [DataMember]
        public string SIGN_ISP { get; set; }
        [DataMember]
        public string SIGN_BUH { get; set; }
        [DataMember]
        public string SIGN_DIR { get; set; }
        public List<ErrorProtocolXML> ErrList { get; set; }
        public void AddError(ErrorProtocolXML item)
        {
            ErrList.Add(item);
            PropChange("ErrList");
        }
        public void AddError(IEnumerable<ErrorProtocolXML> items)
        {
            foreach (var s in items)
                ErrList.Add(s);
            PropChange("ErrList");
        }
        [DataMember]
        public string PATH_LOG_XML { get; set; } = "";
        public FileItemBase()
        {
        }
      
        [DataMember]
        private string _FileName;
        
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName
        {
            get => _FileName;
            set
            {
                _FileName = value;
                PropChange("FileName");
            }
        }
        [DataMember]
        public bool IsArchive { get; set; }
        [DataMember]
        private FileType? _Type;

        /// <summary>
        /// Тип файла
        /// </summary>
        public FileType? Type
        {
            get => _Type;
            set
            {
                _Type = value;
                PropChange("Type");
            }
        }

        [DataMember]
        private StepsProcess _Process;
        /// <summary>
        /// Шаг обработки
        /// </summary>
        public StepsProcess Process
        {
            get => _Process;
            set
            {
                _Process = value;
                PropChange("Process");
            }
        }
        /// <summary>
        /// Комментарии
        /// </summary>
        /// 
        [DataMember]
        public string _Comment;
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment
        {
            get => _Comment;
            set
            {
                _Comment = value;
                PropChange("Comment");
            }
        }

        public string CommentAndLog
        {
            set { Comment = value;FileLog?.WriteLn(value); }
        }

      
       
        [DataMember]
        private DateTime _DateCreate;
        /// <summary>
        /// Дата создания
        /// </summary>
        public DateTime DateCreate
        {
            get => _DateCreate;
            set
            {
                _DateCreate = value;
                PropChange("DateCreate");
            }
        }
       
        [DataMember]
        private string _FilePach;
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FilePach
        {
            get => _FilePach;
            set
            {
                _FilePach = value;
                PropChange("FilePach");
            }
        }
    
        public void PropChange(string Name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private VersionMP _Version;
        public VersionMP Version
        {
            get => _Version;
            set
            {
                _Version = value;
                PropChange("Version");
            }
        }

        /// <summary>
        /// Лог файл
        /// </summary>
        /// 
        [DataMember]
        private LogFile _FileLog;

        /// <summary>
        /// Класс лог файла
        /// </summary>
        public LogFile FileLog
        {
            get => _FileLog;
            set
            {
                _FileLog = value;
                PropChange("FileLog");
            }
        }
        [DataMember]
        public object Tag { get; set; }
        [DataMember]
        int? zglv_id;
        public int? ZGLV_ID
        {
            get => zglv_id;


            set
            {
                zglv_id = value;
                PropChange("ZGLV_ID");
            }
        }
        [DataMember]
        bool? dop_reestr;
        public bool? DOP_REESTR
        {
            get => dop_reestr;


            set
            {
                dop_reestr = value;
                PropChange("DOP_REESTR");
            }
        }

        public void CopyFrom(FileItemBase item)
        {
            SIGN_DIR = item.SIGN_DIR;
            SIGN_BUH = item.SIGN_DIR;
            SIGN_ISP = item.SIGN_ISP;
            PATH_LOG_XML = item.PATH_LOG_XML;
            FileName = item.FileName;
            IsArchive = item.IsArchive;
            Type = item.Type;
            Process = item.Process;
            Comment = item.Comment;
            DateCreate = item.DateCreate;
            FilePach = item.FilePach;
            Version = item.Version;
            ZGLV_ID = item.ZGLV_ID;
            DOP_REESTR = item.DOP_REESTR;
            FileLog = item.FileLog;
            if (item.ErrList != null)
            {
                ErrList = new List<ErrorProtocolXML>();
                ErrList.AddRange(item.ErrList);

            }
          
        }
    }
    /// <summary>
    /// Элемент файл H
    /// </summary>
     [Serializable]
    [DataContract]
    public class FileL : FileItemBase
    {
        public FileL()
        {
        }
    }
    /// <summary>
    /// Элемент базового списка
    /// </summary>
    [Serializable]
    [DataContract]
    public class FileItem : FileItemBase
    {
        public FileItem()
        {
        }

        [DataMember]
        private FileL FileL;      
        /// <summary>
        /// Файл H
        /// </summary>
        public FileL filel
        {
            get => FileL;
            set
            {
                FileL = value;
                PropChange("filel");
            }
        }

        public void CopyFrom(FileItem item)
        {
            base.CopyFrom(item);
            filel = item.filel;
        }

        public StepsProcess FullProcess
        {
            set
            {
                Process = value;
                if (filel != null)
                    filel.Process = value;
            }
        }

        public void WriteLnFull(string mess) 
        {
            FileLog.WriteLn(mess);
            filel?.FileLog.WriteLn(mess);
        }
    }
    [Serializable]
    public enum IST
    {
        MAIL = 1 ,
        SITE = 2
    }

    /// <summary>
    /// Пакет с файлами
    /// </summary>
    /// 

    [Serializable]
    [DataContract]
    public class FilePacket : IComparable<FilePacket>, INotifyPropertyChanged
    {
        public bool IsErrorFiles => Files.Count(x => x.Process.In(StepsProcess.ErrorXMLxsd, StepsProcess.Invite, StepsProcess.FlkErr, StepsProcess.NotInvite)) != 0;

        [DataMember] private Guid _guid { get; set; } = Guid.NewGuid();
        public Guid guid
        {
            get => _guid;
            set
            {
                _guid = value; OnPropertyChanged();
            }
        }
        [DataMember] private decimal? _ID;

        /// <summary>
        /// ID
        /// </summary>
        public decimal? ID
        {
            get => _ID;
            set
            {
                _ID = value;
                OnPropertyChanged();
            }
        }

        [DataMember] private IST _IST = IST.MAIL;

        /// <summary>
        /// Источник поступления
        /// </summary>
        public IST IST
        {
            get => _IST;
            set
            {
                _IST = value;
                OnPropertyChanged();
            }
        }

        [DataMember] string warnning = "";

        /// <summary>
        /// Предупреждения на сайт
        /// </summary>
        public string WARNNING
        {
            get => warnning;
            set
            {
                warnning = value;
                ChangeStat();
                OnPropertyChanged();
            }
        }

        [DataMember] private string _PATH_STAT = "";

        /// <summary>
        /// Путь к файлу статистики
        /// </summary>
        public string PATH_STAT
        {
            get => _PATH_STAT;
            set
            {
                _PATH_STAT = value;
                OnPropertyChanged();
            }
        }

        [DataMember] private string _PATH_ZIP = "";

        /// <summary>
        /// Путь к архиву ответа
        /// </summary>
        public string PATH_ZIP
        {
            get => _PATH_ZIP;
            set
            {
                _PATH_ZIP = value;
                OnPropertyChanged();
            }
        }

        [DataMember] private bool _StopTime;

        /// <summary>
        /// Флаг остановки ожидания
        /// </summary>
        /// 
        public bool StopTime
        {
            get => _StopTime;
            set
            {
                _StopTime = value;
                OnPropertyChanged();
            }
        }

        public FilePacket()
        {
            CodeMO = "";
            Files = new List<FileItem>();
            Priory = 0;
            StopTime = false;
            changeSiteStatus = null;
        }

        /// <summary>
        /// Открыть для добавления все файлы лога
        /// </summary>
        public void OpenLogFiles()
        {
            foreach (var f in Files)
            {
                f.FileLog?.Append();
                f.filel?.FileLog?.Append();
            }
        }

        /// <summary>
        /// Открыть для записи(со сбросом данных) все файлы лога
        /// </summary>
        public void ResetLogFiles()
        {
            foreach (var f in Files)
            {
                f.FileLog?.Reset();
                f.filel?.FileLog?.Reset();
            }
        }

        /// <summary>
        /// Закрыть все файлы лога
        /// </summary>
        public void CloserLogFiles()
        {
            foreach (var f in Files)
            {
                f.FileLog?.Close();
                f.filel?.FileLog?.Close();
            }
        }

        [DataMember] int _Priory;

        /// <summary>
        /// Приоритет обработки
        /// </summary>
        public int Priory
        {
            get => _Priory;
            set
            {
                _Priory = value;
                OnPropertyChanged();
            }
        }

        [DataMember] StatusFilePack _Status;

        /// <summary>
        /// Статус пакета
        /// </summary>
        public StatusFilePack Status
        {
            get => _Status;
            set
            {
                _Status = value;
                ChangeStat();
                OnPropertyChanged();
            }
        }

        [DataMember] string _CaptionMO;

        /// <summary>
        /// Наименование организации
        /// </summary>
        public string CaptionMO
        {
            get => _CaptionMO;
            set
            {
                _CaptionMO = value;
                OnPropertyChanged();
            }
        }

        [DataMember] public DateTime _Date;

        /// <summary>
        /// Дата поступления
        /// </summary>
        public DateTime Date
        {
            get => _Date;
            set
            {
                _Date = value;
                OnPropertyChanged();
            }
        }

       

        [DataMember]
        string _CodeMO;

        /// <summary>
        /// Код МО массивом
        /// </summary>
        public string CodeMO
        {

            get => _CodeMO;
            set
            {
                _CodeMO = value;
                OnPropertyChanged();
            }
        }

        [DataMember] public string _Comment;

        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment
        {
            get => _Comment;
            set
            {
                _Comment = value;
                OnPropertyChanged();
            }
        }

        [DataMember] string _CommentSite = "";

        /// <summary>
        /// Комментарий на сайт
        /// </summary>
        /// 
        public string CommentSite
        {
            get => _CommentSite;
            set
            {
                _CommentSite = value;
                ChangeStat();
                OnPropertyChanged();
            }
        }

        [DataMember] List<FileItem> _Files;

        /// <summary>
        /// Состав файлов пакета
        /// </summary>
        public List<FileItem> Files
        {
            get => _Files;
            set
            {
                _Files = value;
                OnPropertyChanged();
            }
        }





        public int CompareTo(FilePacket other)
        {
            return CodeMO.CompareTo(other.CodeMO);
        }


        public delegate void ChangeSiteStatus(FilePacket fp);

        public event ChangeSiteStatus changeSiteStatus;

        public bool IschangeSiteStatus => changeSiteStatus != null;

        private void ChangeStat()
        {
            changeSiteStatus?.Invoke(this);
        }

        public void WriteAllLog(string Text)
        {
            foreach (var item in this.Files)
            {
                item.FileLog.WriteLn(Text);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CopyFrom(FilePacket item)
        {
            this.ID = item.ID;
            this.guid = item.guid;
            this.IST = item.IST;
            this.WARNNING = item.WARNNING;
            this.PATH_STAT = item.PATH_STAT;
            this.PATH_ZIP = item.PATH_ZIP;
            this.StopTime = item.StopTime;
            this.Priory = item.Priory;
            this.Status = item.Status;
            this.Date = item.Date;
            this.CaptionMO = item.CaptionMO;
            this.CodeMO = item.CodeMO;
            this.Comment = item.Comment;
            this.CommentSite = item.CommentSite;
            this.Files.Clear();
            this.Files.AddRange(item.Files.Select(x=>
            {
                var f = new FileItem();
                f.CopyFrom(x);
                return f;
            }));
            OnPropertyChanged(nameof(this.IsErrorFiles));
            
        }
    }
    public class FilePacketAndOrder
    {
        public FilePacket FP { get; set; }
        public int ORDER { get; set; }
    }
    /// <summary>
    /// Менеджер файлов
    /// </summary>
    [Serializable]
    public class FilesManager:MarshalByRefObject, INotifyCollectionChanged
    {
        private List<FilePacket> Files; 
        public FilesManager()
        {
            Files = new List<FilePacket>();
        }
        /// <summary>
        /// Добавить пакет в список
        /// </summary>
        /// <param name="item">Пакет</param>
        public void Add(FilePacket item)
        {
            CheckGUID(item);
            Files.Add(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

     

      
        /// <summary>
        /// Удалить пакет из списка
        /// </summary>
        /// <param name="item">Пакет</param>
        public void Remove(FilePacket item)
        {
            Files.Remove(item);
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }
        
        public FilePacket FindPacket(string CodeMO)
        {
            return Files.FirstOrDefault(FP => FP.CodeMO == CodeMO);
        }
        public FilePacket FindPacket(Guid guid)
        {
            return Files.FirstOrDefault(FP => FP.guid == guid);
        }
        public FilePacket this[int index]
        {
            get => Files[index];
            set => Files[index] = value;
        }
        /// <summary>
        /// Получить список пакетов
        /// </summary>
        /// <returns>Список пакетов</returns>
        public List<FilePacket> Get()
        {
            return Files;
        }
        /// <summary>
        /// Количество пакетов в списке
        /// </summary>
        public int Count => Files.Count;

        public void Clear()
        {
            Files.Clear();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
       
    
        public void SaveToFile(string Path)
        {
            using (var sw = new FileStream(Path, FileMode.Create))
            {
                var xml = new XmlSerializer(typeof(List<FilePacket>));
                xml.Serialize(sw, this.Files);
                sw.Close();
            }
        }

        public void LoadToFile(string Path)
        {
            using (var sw = new FileStream(Path, FileMode.Open))
            {
                var xml = new XmlSerializer(typeof(List<FilePacket>));
                Files = (List<FilePacket>)xml.Deserialize(sw);
                sw.Close();
            }
            CheckGUID_ALL();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        private void CheckGUID(FilePacket item)
        {
            while (Files.Any(x => x.guid == item.guid && x != item))
            {
                item.guid = Guid.NewGuid();
            }
        }

        private void CheckGUID_ALL()
        {
            foreach (var item in Files)
            {
                CheckGUID(item);
            }
        }
        public FilePacket GetIndexHighPriority()
        {
            var MaxPrior = -1;
            FilePacket result  = null;
            foreach (var item in Files.Where(item => item.Status == StatusFilePack.XMLSchemaOK).Where(item => item.Priory > MaxPrior))
            {
                MaxPrior = item.Priory;
                result = item;
            }
            return result;
        }

        public int Order( FilePacket fp)
        {
            var MaxPrior = 0;
            var result = 0;
            var countall = 0;
            result = 0;
            foreach (var t in Files.Where(t => t.Status == StatusFilePack.XMLSchemaOK))
            {
                if (t.Priory == fp.Priory)
                {
                    countall++;
                    if (t == fp)
                        result = countall;
                }
                 
                if (t.Priory > fp.Priory)
                {
                    MaxPrior++;
                }
            }
            result = result - 1 + MaxPrior;
            return result;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }


     

    public static partial class  Ext
    {
      
        public static void InvokeComm(this FileItem item, string COMM, DispatcherObject win)
        {
            win.Dispatcher.Invoke(() => { item.Comment = COMM; });
        }
        public static void InvokeComm(this FileItem item, string COMM, Dispatcher dispatcher)
        {
            dispatcher.Invoke(() => { item.Comment = COMM; });
        }

        public static bool In(this StepsProcess item,params StepsProcess[] items)
        {
            return items.Contains(item);
        }
    }



  
}

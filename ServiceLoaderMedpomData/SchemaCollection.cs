using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Globalization;

namespace ServiceLoaderMedpomData
{
    [XmlRoot("dictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));
            var wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty) return;

            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                var key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                var value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (var key in Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                var value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
    }


    /// <summary>
    /// Схема для группы файлов
    /// </summary>

    [Serializable]
    [DataContract]
    public class VersionSchemaElement
    {
        [DataMember]
        public SerializableDictionary<FileType, List<SchemaElementValue>> SchemaElements { get; set; }
        public List<string> VersionsZGLV { get; set; }
        public VersionSchemaElement()
        {
            SchemaElements = new SerializableDictionary<FileType, List<SchemaElementValue>>();

            foreach (var ft in (FileType[])Enum.GetValues(typeof(FileType)))
            {
                SchemaElements.Add(ft, new List<SchemaElementValue>());
            }
            VersionsZGLV = new List<string>();
        }
        public static void Check(VersionSchemaElement item)
        {
            foreach (var ft in (FileType[])Enum.GetValues(typeof(FileType)))
            {
                if (!item.SchemaElements.ContainsKey(ft))
                    item.SchemaElements.Add(ft, new List<SchemaElementValue>());
            }
        }

        public void AddAndCheck(FileType ft, SchemaElementValue item)
        {
            if(SchemaElements[ft].Count(x=> Overlap(item.DATE_B,item.DATE_E??DateTime.Now, x.DATE_B,x.DATE_E??DateTime.Now))!=0)
                throw  new Exception("Невозможно добавить элемент т.к. он имеет пересечение периодов с другими элементами");
            SchemaElements[ft].Add(item);
        }
        public void ClearSchema(FileType ft)
        {
            SchemaElements[ft].Clear();
        }


        public bool Overlap(DateTime startDate1, DateTime endDate1, DateTime startDate2, DateTime endDate2)
        {
            if (endDate1 >= startDate2 && endDate2 >= startDate1)
            {
                return true;
            }

            if (startDate1 <= endDate2 && startDate2 <= startDate1)
            {
                return true;
            }

            return false;
        }

    }
    [Serializable]
    [DataContract]
    public class SchemaElementValue
    {
        [DataMember]
        public  string Value { get; set; }
        [DataMember]
        public  DateTime DATE_B { get; set; }
        [DataMember]
        public  DateTime? DATE_E { get; set; }
    }

    /// <summary>
    /// Набор схем
    /// </summary>
    [DataContract]
    [Serializable]
    public class SchemaCollection
    {
        //Схемы
        [DataMember]
        private SerializableDictionary<VersionMP, VersionSchemaElement> Schemas { get; set; }
        public SchemaCollection()
        {
            newSc();
        }
        public SchemaCollectionFindResult FindSchema(string Version, DateTime dt, FileType ft)
        {
            var sc = Schemas.Where(x => x.Value.VersionsZGLV.Contains(Version)).ToList();
            if (sc.Count != 1)
                return new SchemaCollectionFindResult{Result = false, Exception = $"Не допустимая версия документа: {Version}"};

            if (!sc[0].Value.SchemaElements.ContainsKey(ft))
                return new SchemaCollectionFindResult{ Result = false, Exception = $"Для версии документа {Version} нет схемы для файла {ft.ToString()}"};

            var value = sc[0].Value.SchemaElements[ft]
                .FindAll(x => dt >= x.DATE_B && (dt <= x.DATE_E || !x.DATE_E.HasValue));

            if (value.Count > 1)
                return new SchemaCollectionFindResult
                {
                    Result = false,
                    Exception = $"Для версии документа {Version} файла {ft.ToString()} найдено более 1 схемы документа"
                };

            if (value.Count == 0)
                return new SchemaCollectionFindResult
                {
                    Result = false,
                    Exception =
                        $"Для версии документа {Version} файла {ft.ToString()} " +
                        $"не найдено ни" +
                        $" одной схемы. Файл от {dt:MM-yyyy}, доступны схемы:{Environment.NewLine}{string.Join(Environment.NewLine, sc[0].Value.SchemaElements[ft].Select(x => $"с {x.DATE_B:dd-MM-yyyy}{(x.DATE_E.HasValue ? $" по {x.DATE_E:dd-MM-yyyy}" : "")}"))}"
                };
            return new SchemaCollectionFindResult() {Result = true, Value = value[0], Vers = sc[0].Key};
        }
        public VersionMP FindVersion(string Version)
        {
            var sc = Schemas.Where(x => x.Value.VersionsZGLV.Contains(Version)).ToList();
            return sc.Count != 1 ? VersionMP.NONE : sc[0].Key;
        }
        void newSc()
        {
            Schemas = new SerializableDictionary<VersionMP, VersionSchemaElement>();
            foreach (var v in (VersionMP[]) Enum.GetValues(typeof(VersionMP)))
            {
                Schemas.Add(v, new VersionSchemaElement());
            }
        }
        void Check()
        {
            foreach (var v in (VersionMP[])Enum.GetValues(typeof(VersionMP)))
            {
                if (!Schemas.ContainsKey(v))
                {
                    Schemas.Add(v, new VersionSchemaElement());
                }
                else
                {
                    if (Schemas[v].VersionsZGLV == null)
                        Schemas[v].VersionsZGLV = new List<string>();

                }
                VersionSchemaElement.Check(Schemas[v]);
            }
        }
        public List<VersionMP> Versions => Schemas.Keys.ToList();
        public List<SchemaElementValue> this[VersionMP version, FileType _type]
        {
            get => Schemas[version].SchemaElements[_type];
            set => Schemas[version].SchemaElements[_type] = value;
        }
        public VersionSchemaElement this[VersionMP version] => Schemas[version];
        public bool ContainsVersion(VersionMP key)
        {
            return Schemas.ContainsKey(key);
        }

        /// <summary>
        /// Сохранить настройки в файл
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public void SaveToFile(string path)
        {
            var serializer = new XmlSerializer(typeof(SerializableDictionary<VersionMP, VersionSchemaElement>));
            TextWriter writer = new StreamWriter(path);
            serializer.Serialize(writer, Schemas);
            writer.Close();
        }
        /// <summary>
        /// Загрузить настройки из файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        public bool LoadFromFile(string path)
        {
            FileStream fs = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SerializableDictionary<VersionMP, VersionSchemaElement>));
                fs = new FileStream(path, FileMode.Open);
                Schemas = (SerializableDictionary<VersionMP, VersionSchemaElement>)serializer.Deserialize(fs);
                fs.Close();
                Check();
                return true;
            }
            catch (Exception)
            {
                fs?.Close();
                newSc();
                return false;
            }
        }
    }

    public class SchemaCollectionFindResult
    {
        public  bool Result { get; set; }
        public SchemaElementValue Value { get; set; }
        public VersionMP Vers { get; set; }
        public  string Exception { get; set; }

    }
    /// <summary>
    /// Класс ошибки при проверке схемы
    /// </summary>
    public class ErrorProtocolXML
    {
        public long? SLUCH_Z_ID { get; set; }
        public int OSHIB { get; set; }
        public string IM_POL { get; set; } = "";
        public string BAS_EL { get; set; } = "";
        public string N_ZAP { get; set; } = "";
        public string IDCASE { get; set; } = "";
        public string SL_ID { get; set; } = "";
        public string ID_SERV { get; set; } = "";
        public string Comment { get; set; } = "";
        public string Line { get; set; }
        public string column { get; set; }
        public string ERR_CODE { get; set; }

        public ErrorProtocolXML()
        {
        }

        public string MessageOUT => $"[{Line}][{column}]: {Comment}";
    }

    public class CheckXMLValidator
    {
        public CheckXMLValidator(VersionMP v, bool IsMTRProtocol =false, bool IsValidateRef = true)
        {
            this.IsMTRProtocol = IsMTRProtocol;
            Version = v;
            this.IsValidateRef = IsValidateRef;
        }
        public VersionMP Version { get; set; }
        public bool IsMTRProtocol { get; set; }
        public bool IsValidateRef { get; set; }
    }
 

    
    public class SchemaChecking
    {
        // лист ошибок
        private List<ErrorProtocolXML> ListER { get; set; }
        //Хранитель путь к текущему элементу в дереве XML
        List<string> listnode { get; set; } =  new List<string>(); 
        string filepath { get; set; } = "";
        LogFile FileLog { get; set; }
        bool ShowErrText { get; set; } //результат при проверки схемы
        string n_zap { get; set; } //Номер текущей записи
        string id_case { get; set; } //номер случая
        string id_serv { get; set; } //номер услуги
        string id_pac { get; set; } //номер ID_PAC
        string sl_id { get; set; } = ""; //номер SL_ID
        bool fileL { get; set; }
        public SchemaChecking()
        {
            ListER = new List<ErrorProtocolXML>();
          
        }
        //формирование файла флк
        public static void XMLfileFLK(string pathToXml,string FNAME_1, List<ErrorProtocolXML> ListER)
        {
            var textWritter = new XmlTextWriter(pathToXml, Encoding.GetEncoding("windows-1251"));

            textWritter.WriteStartDocument();
            textWritter.WriteStartElement("FLK_P");
            textWritter.WriteEndElement();
            textWritter.Close();

            var document = new XmlDocument();
            document.Load(pathToXml);

            XmlNode element = document.CreateElement("FNAME");
            element.InnerText = Path.GetFileName(pathToXml);
            document.DocumentElement.AppendChild(element);

            element = document.CreateElement("FNAME_1");
            element.InnerText = Path.GetFileName(FNAME_1);
            document.DocumentElement.AppendChild(element);

            foreach (var item in ListER)
            {
                XmlNode PR = document.CreateElement("PR");
                document.DocumentElement.AppendChild(PR);

                XmlNode OSHIB = document.CreateElement("OSHIB");
                OSHIB.InnerText = item.OSHIB.ToString();
                PR.AppendChild(OSHIB);

                if (item.IM_POL != item.BAS_EL && item.IM_POL != "")
                {
                    XmlNode IM_POL = document.CreateElement("IM_POL");
                    IM_POL.InnerText = item.IM_POL;
                    PR.AppendChild(IM_POL);
                }

                if (item.BAS_EL != "")
                {
                    XmlNode BAS_EL = document.CreateElement("BAS_EL");
                    BAS_EL.InnerText = item.BAS_EL;
                    PR.AppendChild(BAS_EL);
                }
                if (item.N_ZAP != "")
                {
                    XmlNode N_ZAP = document.CreateElement("N_ZAP");
                    N_ZAP.InnerText = item.N_ZAP;
                    PR.AppendChild(N_ZAP);
                }

                if (item.IDCASE != "")
                {
                    XmlNode IDCASE = document.CreateElement("IDCASE");
                    IDCASE.InnerText = item.IDCASE;
                    PR.AppendChild(IDCASE);
                }
                if (item.ID_SERV != "")
                {
                    XmlNode ID_SERV = document.CreateElement("IDSERV");
                    ID_SERV.InnerText = item.ID_SERV;
                    PR.AppendChild(ID_SERV);
                }
                if (item.SL_ID != "")
                {
                    XmlNode SL_ID = document.CreateElement("SL_ID");
                    SL_ID.InnerText = item.SL_ID;
                    PR.AppendChild(SL_ID);
                }
                if (item.Comment != "")
                {
                    XmlNode Comment = document.CreateElement("COMMENT");
                    Comment.InnerText = item.Comment.Length > 250 ? item.Comment.Substring(0, 250) : item.Comment;
                    PR.AppendChild(Comment);
                }
            }
            document.Save(pathToXml);
        }

        /// <summary>
        /// Проверка на соответствие схеме
        /// </summary>
        /// <param name="_File">Файл для проверки</param>
        /// <param name="PathXSD">Схема</param>
        /// <param name="isvalidate">Подключать валидатор</param>
        /// <returns>0-ошибка при проверке 1 проверка успешна</returns>
        /// 
        public bool CheckSchema(FileItemBase _File, string PathXSD, bool isValidateRef = true)
        {
            var prevnodes = "";
            var LineNumber = 0;
            var LinePosition = 0;
            try
            {
                FileLog = _File.FileLog;
                filepath = _File.FilePach;
                fileL = false;
                ShowErrText = true;
                FileLog.WriteLn("Проверка файла на соответствие схеме:");
                var validate = new CheckXMLValidator(_File.Version, false, isValidateRef);
                var res = CheckXML(filepath, PathXSD, validate);
                var resul = res.Count == 0;
                _File.DOP_REESTR = DOP_REESTR;
                if (resul)
                    _File.FileLog.WriteLn("Файл составлен правильно");
                else
                {
                    var pathToXml = Path.Combine(Path.GetDirectoryName(FileLog.FilePath), $"{Path.GetFileNameWithoutExtension(filepath)}FLK.xml");
                    XMLfileFLK(pathToXml, Path.GetFileName(filepath), ListER);
                    _File.PATH_LOG_XML = pathToXml;
                }
                return resul;
            }
            catch (Exception ex)
            {
                _File.FileLog.WriteLn($"Ошибка при проверке документа на соответствие схеме [{LineNumber},{LinePosition}]{prevnodes}: {ex.Message}");
                return false;
            }
        }

        private bool? DOP_REESTR { get; set; }
        public List<ErrorProtocolXML> GetProtokol => ListER;

        public List<ErrorProtocolXML> CheckXML(Stream st, string XSD, CheckXMLValidator CXV = null)
        {
            var prevnodes = "";
            var LineNumber = 0;
            var LinePosition = 0;
            var count_pr_nov1 = 0;
            var count_pr_nov0 = 0;
            try
            {
                ListER.Clear();
                listnode.Clear();
                var XMLSettings = new XmlReaderSettings();
                XMLSettings.Schemas.Add(null, XSD);
                XMLSettings.ValidationType = ValidationType.Schema;
                XMLSettings.ValidationEventHandler += SettingsValidationEventHandler;
                
                var NS = "";
                var ns_error = false;
                foreach (XmlSchema schema in XMLSettings.Schemas.Schemas())  // foreach is used to simplify the example
                {
                    NS = schema.TargetNamespace ?? "";
                }

                IValidatorXML validate = null;
                if (CXV != null)
                {
                    if (CXV.IsMTRProtocol)
                    {
                        validate = new ProtocolValidator31(ErrorAction);
                    }
                    else
                    {
                        switch (CXV.Version)
                        {
                            case VersionMP.V2_1:
                                validate = new MyValidatorV2(ErrorAction);
                                break;
                            case VersionMP.V3_0:
                                validate = new MyValidatorV3(ErrorAction);
                                break;
                            case VersionMP.V3_1:
                                validate = new MyValidatorV31(ErrorAction, CXV.IsValidateRef);
                                break;
                            default:
                                validate = new MyValidatorV31(ErrorAction, CXV.IsValidateRef);
                                break;
                        }
                    }
                }

                var currdep = -1;
             
                using (var r = new XmlTextReader(st))
                {
                    using (var reader = XmlReader.Create(r, XMLSettings))
                    {
                        while (reader.Read())
                        {
                            LineNumber = ((IXmlLineInfo)reader).LineNumber;
                            LinePosition = ((IXmlLineInfo)reader).LinePosition;
                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                switch (prevnodes)
                                {
                                    //Просто для адрисации
                                    case "N_ZAP":
                                        n_zap =reader.Value;
                                        break;
                                    case "IDSERV":
                                        id_serv = reader.Value;
                                        break;
                                    case "IDCASE":
                                        id_case = reader.Value;
                                        break;
                                    case "SL_ID":
                                        sl_id = reader.Value;
                                        break;
                                    case "ID_PAC":
                                        id_pac = reader.Value;
                                        break;
                                    //Проверка доп или нет
                                    case "PR_NOV":
                                    {
                                        if (reader.Value == "1")
                                            count_pr_nov1++;
                                        else
                                            count_pr_nov0++;
                                        break;
                                    }
                                }
                            }
                            //проверка NameSpace и слежение за стрктурой(глубиной погружения)
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (!ns_error && NS != reader.NamespaceURI)
                                {
                                    ErrorAction(XmlSeverityType.Error, ((IXmlLineInfo)reader).LineNumber, ((IXmlLineInfo)reader).LinePosition, $"Неверный Namespace=\"{reader.NamespaceURI}\" ожидается \"{NS}\"", "XML");
                                    ns_error = true;
                                }
                                if (reader.Depth != currdep)
                                {
                                    currdep = reader.Depth;
                                    listnode.Add(reader.Name);
                                }
                                else
                                {
                                    currdep = reader.Depth;
                                    listnode[listnode.Count - 1] = reader.Name;
                                }
                                prevnodes = reader.Name;
                            }
                            if (reader.NodeType == XmlNodeType.EndElement && currdep != reader.Depth)
                            {
                                currdep = reader.Depth;
                                listnode.RemoveAt(listnode.Count - 1);
                            }
                            validate?.Check(reader);
                        }
                        validate?.Close();
                    }
                }

                DOP_REESTR = null;
                if (count_pr_nov0 > 0 && count_pr_nov1 == 0)
                    DOP_REESTR = false;
                if (count_pr_nov0 == 0 && count_pr_nov1 > 0)
                    DOP_REESTR = true;
            }
            catch(Exception ex)
            {
                ListER.Add(new ErrorProtocolXML { Comment =$"Ошибка при проверке документа на соответствие схеме[{LineNumber},{LinePosition}]{prevnodes}: {ex.Message}"});
                FileLog?.WriteLn($"Ошибка при проверке документа на соответствие схеме [{LineNumber},{LinePosition}]{prevnodes}: {ex.Message}");
            }
            return ListER;
        }
        public List<ErrorProtocolXML> CheckXML(string zl, string XSD, CheckXMLValidator CXV = null)
        {
            using (var sr = new StreamReader(zl, Encoding.GetEncoding(1251)))
            {
                return CheckXML(sr.BaseStream, XSD, CXV);
            }
        }

        public static string GetELEMENT(string path, string ElementName)
        {
            using (Stream FileSteam = new FileStream(path, FileMode.Open, FileAccess.Read))
            {                
                using (var reader = XmlReader.Create(FileSteam))
                {
                    var value = "";
                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == ElementName)
                            {
                                reader.Read();
                                value = reader.Value;
                                break;
                            }
                        }
                    }
                    reader.Close();
                    FileSteam.Close();
                    return value;
                }
            }
        }

        public static Dictionary<string, string> GetELEMENTs(string path, params string[] ElementName)
        {
            var res = ElementName.ToDictionary(item => item, item => "");
            var FindElement = new List<string>();
           
            using (Stream FileSteam = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = XmlReader.Create(FileSteam))
                {
                    while (reader.Read())
                    {
                        if (reader.NodeType != XmlNodeType.Element) continue;
                        foreach (var el in ElementName)
                        {
                            if (reader.Name != el) continue;
                            reader.Read();
                            res[el] = reader.Value;
                            FindElement.Add(el);
                        }
                        if (FindElement.Count == ElementName.Length)
                            break;
                    }
                    reader.Close();
                    FileSteam.Close();
                    return res;
                }
            }
        }


        //В случае ошибки по схеме
        private void SettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            var reader = sender as XmlReader;
            var name = reader != null? reader.Name :  "";
            ErrorAction(e.Severity, e.Exception.LineNumber, e.Exception.LinePosition, e.Message, name);
        }
        void ErrorAction(XmlSeverityType Severity, int LineNumber, int LinePosition, string Message, string NamePol, string ERR_CODE = "")
        {
            if (ShowErrText)
            {
                ShowErrText = false;
                FileLog?.WriteLn("!!!Отказано в приеме файла полностью. Файл не соответствует схеме!!!");
            }

            switch (Severity)
            {
                case XmlSeverityType.Error:
                    FileLog?.WriteLn($"ERROR: [{LineNumber},{LinePosition}] {Message}");
                    break;
                case XmlSeverityType.Warning:
                    FileLog?.WriteLn($"WARNING: [{LineNumber},{LinePosition}] {Message}");
                    break;
            }

            var item = new ErrorProtocolXML {OSHIB = 41, IM_POL = NamePol, Comment = Message, Line =  LineNumber.ToString(),column =  LinePosition.ToString(), ERR_CODE = ERR_CODE };

            if (fileL) item.Comment += $"ID_PAC = {id_pac}";
            if (listnode.Count >= 2)
                item.BAS_EL = listnode[listnode.Count - 2];
            if (listnode.Contains("ZAP"))
                item.N_ZAP = n_zap;
            if (listnode.Contains("SLUCH") || listnode.Contains("Z_SL"))
                item.IDCASE = id_case;
            if (listnode.Contains("SL"))
                item.SL_ID = sl_id;
            if (listnode.Contains("PERS"))
                item.N_ZAP = id_pac;
            if (listnode.Contains("USL"))
                item.ID_SERV = id_serv;
            ListER.Add(item);
        }

    }

    delegate void ErrorActionEvent(XmlSeverityType Severity, int LineNumber, int LinePosition, string Message, string NamePol, string ERR_CODE = "");

    interface IValidatorXML
    {
        event ErrorActionEvent Error;
        void Close();
        void Check(XmlReader reader);
    }

    class MyValidatorV2 : IValidatorXML
    {

        class NAZR
        {
            public int NAZR_C1 { get; set; }
            public int NAZR_C2 { get; set; }
            public int NAZR_C3 { get; set; }
            public int NAZR_C4 { get; set; }
            public int NAZR_C5 { get; set; }
            public int NAZR_C6 { get; set; }
            public int NAZ_SP_C { get; set; }
            public int NAZ_V_C { get; set; }
            public int NAZ_PMP_C { get; set; }
            public int NAZ_PK_C { get; set; }
            public int Line_number { get; set; }
            public int Line_position { get; set; }
            public NAZR()
            {
                NAZR_C1 = 0;
                NAZR_C2 = 0;
                NAZR_C3 = 0;
                NAZR_C4 = 0;
                NAZR_C5 = 0;
                NAZR_C6 = 0;
                NAZ_SP_C = 0;
                NAZ_V_C = 0;
                NAZ_PMP_C = 0;
                NAZ_PK_C = 0;
            }
            public void Clear()
            {
                NAZR_C1 = 0;
                NAZR_C2 = 0;
                NAZR_C3 = 0;
                NAZR_C4 = 0;
                NAZR_C5 = 0;
                NAZR_C6 = 0;
                NAZ_SP_C = 0;
                NAZ_V_C = 0;
                NAZ_PMP_C = 0;
                NAZ_PK_C = 0;
            }

            public bool Check => NAZ_SP_CHECK && NAZ_V_CHECK && NAZ_PMP_CHECK && NAZ_PK_CHECK;

            public bool NAZ_SP_CHECK => NAZR_C1 + NAZR_C2 == NAZ_SP_C;

            public bool NAZ_V_CHECK => NAZR_C3 == NAZ_V_C;
            public bool NAZ_PMP_CHECK => NAZR_C4 + NAZR_C5 == NAZ_PMP_C;
            public bool NAZ_PK_CHECK => NAZR_C6 == NAZ_PK_C;
        }
        NAZR nazr = new NAZR();
        bool startNazr;
        bool endNazr;


        int? SD_Z;
        int SL_COUNT;
        int SD_Z_Line;
        int SD_Z_POS;


        double? SUMMAV;
        int SUMMAV_Line;
        int SUMMAV_POS;

        double SUMV;
        string prevnodes = "";
        public MyValidatorV2(ErrorActionEvent err)
        {
            Error = err;
        }

        public event ErrorActionEvent Error;

        public void Check(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Text)
            {
                endNazr = true;

                if (prevnodes == "SD_Z")
                {
                    SD_Z = Convert.ToInt32(reader.Value);
                    var xmlInfo = (IXmlLineInfo)reader;
                    SD_Z_Line = xmlInfo.LineNumber;
                    SD_Z_POS = xmlInfo.LinePosition;
                }
                if (prevnodes == "SUMMAV")
                {

                    SUMMAV = Convert.ToDouble(reader.Value, new NumberFormatInfo() { NumberDecimalSeparator = "." });
                    var xmlInfo = (IXmlLineInfo)reader;
                    SUMMAV_Line = xmlInfo.LineNumber;
                    SUMMAV_POS = xmlInfo.LinePosition;

                }
                if (prevnodes == "SUMV")
                {
                    SUMV += Convert.ToDouble(reader.Value, new NumberFormatInfo() { NumberDecimalSeparator = "." });
                }


                if (prevnodes == "IDCASE")
                {
                    SL_COUNT++;

                }


                if (prevnodes == "NAZR")
                {
                    if (startNazr == false)
                        nazr.Clear();
                    startNazr = true;
                    endNazr = false;
                    switch (Convert.ToInt16(reader.Value))
                    {
                        case 1: nazr.NAZR_C1++; break;
                        case 2: nazr.NAZR_C2++; break;
                        case 3: nazr.NAZR_C3++; break;
                        case 4: nazr.NAZR_C4++; break;
                        case 5: nazr.NAZR_C5++; break;
                        case 6: nazr.NAZR_C6++; break;
                    }
                    var xmlInfo = (IXmlLineInfo)reader;
                    nazr.Line_number = xmlInfo.LineNumber;
                    nazr.Line_position = xmlInfo.LinePosition;
                }
                if (prevnodes == "NAZ_SP")
                {
                    nazr.NAZ_SP_C++;
                    if (!startNazr)
                    {
                        var xmlInfo = (IXmlLineInfo)reader;
                        Error(XmlSeverityType.Error, xmlInfo.LineNumber, xmlInfo.LinePosition, "Некорректное заполнение сегмента NAZR. NAZ_SP не ожидается", "NAZR");
                    }
                    endNazr = false;
                }
                if (prevnodes == "NAZ_V")
                {
                    nazr.NAZ_V_C++;
                    if (!startNazr)
                    {
                        var xmlInfo = (IXmlLineInfo)reader;
                        Error(XmlSeverityType.Error, xmlInfo.LineNumber, xmlInfo.LinePosition, "Некорректное заполнение сегмента NAZR. NAZ_V не ожидается", "NAZR");
                    }
                    endNazr = false;
                }
                if (prevnodes == "NAZ_PMP")
                {
                    nazr.NAZ_PMP_C++;
                    if (!startNazr)
                    {
                        var xmlInfo = (IXmlLineInfo)reader;
                        Error(XmlSeverityType.Error, xmlInfo.LineNumber, xmlInfo.LinePosition, "Некорректное заполнение сегмента NAZR. NAZ_PMP не ожидается", "NAZR");
                    }
                    endNazr = false;
                }
                if (prevnodes == "NAZ_PK")
                {
                    nazr.NAZ_PK_C++;
                    if (!startNazr)
                    {
                        var xmlInfo = (IXmlLineInfo)reader;
                        Error(XmlSeverityType.Error, xmlInfo.LineNumber, xmlInfo.LinePosition, "Некорректное заполнение сегмента NAZR. NAZ_PK не ожидается", "NAZR");
                    }
                    endNazr = false;
                }

                if (endNazr && startNazr)
                {
                    endNazr = false;
                    startNazr = false;
                    if (!nazr.Check)
                    {
                        var str = "Неверное количество ";
                        if (!nazr.NAZ_V_CHECK)
                        {
                            str += "NAZ_V";
                        }
                        if (!nazr.NAZ_SP_CHECK)
                        {
                            str += " NAZ_SP";
                        }
                        if (!nazr.NAZ_PMP_CHECK)
                        {
                            str += " NAZ_PMP";
                        }
                        if (!nazr.NAZ_PK_CHECK)
                        {
                            str += " NAZ_PK";
                        }

                        Error(XmlSeverityType.Error, nazr.Line_number, nazr.Line_position, "Некорректное заполнение сегмента NAZR. " + str, "NAZR");
                    }
                    nazr.Clear();
                }



            }
            if (reader.NodeType == XmlNodeType.Element)
            { prevnodes = reader.Name; }
        }

        public void Close()
        {
            if (SD_Z.HasValue)
            {
                if (SD_Z != SL_COUNT)
                {
                    Error(XmlSeverityType.Error, SD_Z_Line, SD_Z_POS, "Кол-во случаев в реестре " + SL_COUNT + ", однако SD_Z = " + SD_Z.Value, "SD_Z");
                }
            }

            if (SUMMAV.HasValue)
            {
                if (SUMMAV.Value != Math.Round(SUMV, 2))
                {
                    Error(XmlSeverityType.Error, SUMMAV_Line, SUMMAV_POS, "Сумма случаев в реестре " + Math.Round(SUMV, 2) + ", однако SUMMAV = " + SUMMAV.Value, "SD_Z");
                }
            }

        }
    }

    class MyValidatorV3 : IValidatorXML
    {

        int? SD_Z;
        int SL_COUNT;
        int SD_Z_Line;
        int SD_Z_POS;


        double? SUMMAV;
        int SUMMAV_Line;
        int SUMMAV_POS;

        double SUMV;
        string prevnodes = "";
        public MyValidatorV3(ErrorActionEvent err)
        {
            Error = err;
        }

        public event ErrorActionEvent Error;

    
        public void Check(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Text)
            {

                if (prevnodes == "SD_Z")
                {
                    SD_Z = Convert.ToInt32(reader.Value);
                    var xmlInfo = (IXmlLineInfo)reader;
                    SD_Z_Line = xmlInfo.LineNumber;
                    SD_Z_POS = xmlInfo.LinePosition;
                }
                if (prevnodes == "SUMMAV")
                {

                    SUMMAV = Convert.ToDouble(reader.Value, new NumberFormatInfo() { NumberDecimalSeparator = "." });
                    var xmlInfo = (IXmlLineInfo)reader;
                    SUMMAV_Line = xmlInfo.LineNumber;
                    SUMMAV_POS = xmlInfo.LinePosition;

                }
                if (prevnodes == "SUMV")
                {
                    SUMV += Convert.ToDouble(reader.Value, new NumberFormatInfo() { NumberDecimalSeparator = "." });
                }


                if (prevnodes == "IDCASE")
                {
                    SL_COUNT++;
                }


            }

            if (reader.NodeType == XmlNodeType.Element)
            { prevnodes = reader.Name; }
        }

        public void Close()
        {
            if (SD_Z.HasValue)
            {
                if (SD_Z != SL_COUNT)
                {
                    Error(XmlSeverityType.Error, SD_Z_Line, SD_Z_POS, "Кол-во случаев в реестре " + SL_COUNT + ", однако SD_Z = " + SD_Z.Value, "SD_Z");
                }
            }

            if (SUMMAV.HasValue)
            {
                if (SUMMAV.Value != Math.Round(SUMV, 2))
                {
                    Error(XmlSeverityType.Error, SUMMAV_Line, SUMMAV_POS, "Сумма случаев в реестре " + Math.Round(SUMV, 2) + ", однако SUMMAV = " + SUMMAV.Value, "SUMMAV");
                }
            }

        }
    }

    class  PositionRecord
    {
        public int LINE { get; set; }
        public int POS { get; set; }

        public void Set(XmlReader reader)
        {
            var xmlInfo = (IXmlLineInfo)reader;
            LINE = xmlInfo.LineNumber;
            POS = xmlInfo.LinePosition;
        }
        public void Clear()
        {
            LINE = 0;
            POS = 0;
        }

        public static PositionRecord Get(XmlReader reader)
        {
            var item = new PositionRecord();
            var xmlInfo = (IXmlLineInfo)reader;
            item.LINE = xmlInfo.LineNumber;
            item.POS = xmlInfo.LinePosition;
            return item;
        }
    }





    class XML_Element<T>
    {
        public XML_Element()
        {
            this.POS = new PositionRecord();
        }
        public XML_Element(T value, PositionRecord POS)
        {
            this.value = value;
            this.POS = POS;
        }

        public void Clear()
        {
            POS.Clear();
            value = default(T);
        }
        public T value { get; set; }
        public PositionRecord POS { get; set; }

    }

    enum XML_FileType
    {
        NONE,
        H,
        C,
        T,
        D,
        MTR
    }

    class XML_SCHET_item
    {
        public XML_Element<int> YEAR { get; set; } = new XML_Element<int>();
        public XML_Element<int> MONTH { get; set; } = new XML_Element<int>();
        public XML_Element<string> FILENAME { get; set; } = new XML_Element<string>();
        public XML_Element<string> OKATO_OMS { get; set; } = new XML_Element<string>();
        public XML_Element<int?> SD_Z { get; set; } = new XML_Element<int?>();
        public XML_Element<decimal?> SUMMAV { get; set; } = new XML_Element<decimal?>();
        public XML_Element<string> REF { get; set; } = new XML_Element<string>();

        public decimal SUMV_SUM { get; set; }
        public int IDCASE_COUNT { get; set; }
        public XML_FileType TypeFile
        {
            get
            {
                if (!string.IsNullOrEmpty(OKATO_OMS.value))
                    return XML_FileType.MTR;
                switch (FILENAME?.value?[0])
                {
                    case 'C': return XML_FileType.C;
                    case 'D': return XML_FileType.D;
                    case 'T': return XML_FileType.T;
                    case 'H': return XML_FileType.H;
                    default:
                        return XML_FileType.NONE;
                }
            }
        }
        public DateTime DateFile
        {

            get
            {
                if (YEAR.value <= 0 || MONTH.value <= 0 || MONTH.value > 12)
                    return DateTime.Now.Date;
                return new DateTime(YEAR.value, MONTH.value, 1);
            }
        }

        public void Clear()
        {
            YEAR.Clear();
            MONTH.Clear();
            FILENAME.Clear();
            OKATO_OMS.Clear();
            SD_Z.Clear();
            SUMMAV.Clear();
            SUMV_SUM = 0;
            IDCASE_COUNT = 0;
            REF.Clear();
        }

    }

    class XML_Z_SL_item
    {
        
        public XML_Element<string> FIRST_IDCASE { get; set; } = new XML_Element<string>();
        public XML_Element<string> PR_NOV { get; set; } = new XML_Element<string>();
        public XML_Element<string> P_OTK { get;  set; } = new XML_Element<string>();
        public XML_Element<string> RSLT_D { get;  set; } = new XML_Element<string>();
        public XML_Element<decimal> SUMV { get; set; } = new XML_Element<decimal>();
        public XML_Element<string> USL_OK { get; set; } = new XML_Element<string>();
        public decimal SUM_M_SUM { get;  set; }
        public decimal SUMV_USL_SUM { get;  set; }
        public void Clear()
        {
            P_OTK.Clear();
            RSLT_D.Clear();
            SUMV.Clear();
            USL_OK.Clear();
            SUM_M_SUM = 0;
            SUMV_USL_SUM = 0;
            PR_NOV.Clear();
            FIRST_IDCASE.Clear();
        }
    }
    class XML_Pacient_item
    {

        public XML_Element<int> VPOLIS { get; set; } = new XML_Element<int>();
        public XML_Element<string> SPOLIS { get; set; } = new XML_Element<string>();
        public XML_Element<string> NPOLIS { get; set; } = new XML_Element<string>();
        public XML_Element<string> ENP { get; set; } = new XML_Element<string>();
        public void Clear()
        {
            VPOLIS.Clear();
            SPOLIS.Clear();
            NPOLIS.Clear();
            ENP.Clear();
        }
    }
    class XML_MR_USL_N_item
    {

        public XML_Element<int?> PRVS { get; set; } = new XML_Element<int?>();
        public XML_Element<string> CODE_MD { get; set; } = new XML_Element<string>();
        public void Clear()
        {
            PRVS.Clear();
            CODE_MD.Clear();
        }
    }
    class XML_SL_item
    {
        public XML_Element<string> DS1 { get; set; } = new XML_Element<string>();
        public XML_Element<string> TARIF { get; set; } = new XML_Element<string>();
        public List<XML_Element<string>> DS2 { get; set; } = new List<XML_Element<string>>();
        public XML_Element<string> DS_ONK { get; set; } = new XML_Element<string>();
        public XML_Element<decimal> SUM_M { get; set; } = new XML_Element<decimal>();
        public XML_Element<string> REAB { get; set; } = new XML_Element<string>();
        public XML_Element<string> C_ZAB { get; set; } = new XML_Element<string>();
        public XML_Element<string> PR_D_N { get; set; } = new XML_Element<string>();
        public bool IsCONS { get; set; }
        public bool IsONK_SL { get; set; }

        public void Clear()
        {
            DS1.Clear();
            TARIF.Clear();
            DS2.Clear();
            DS_ONK.Clear();
            SUM_M.Clear();
            REAB.Clear();
            C_ZAB.Clear();
            PR_D_N.Clear();
            IsCONS = false;
            IsONK_SL = false;
        }
    }

    class XML_USL_item
    {
        public XML_Element<string> CODE_MD { get; set; } = new XML_Element<string>();
        public XML_Element<string> PRVS { get; set; } = new XML_Element<string>();
        public XML_Element<string> P_OTK { get; set; } = new XML_Element<string>();
        public XML_Element<decimal> SUMV_USL { get; set; } = new XML_Element<decimal>();
        public void Clear()
        {
            CODE_MD.Clear();
            PRVS.Clear();
            P_OTK.Clear();
            SUMV_USL.Clear();
        }

    }

    class XML_SANK_item
    {
        public XML_Element<int> S_TIP { get; set; } = new XML_Element<int>();
        public XML_Element<int> S_OSN { get; set; } = new XML_Element<int>();
        public XML_Element<decimal> S_SUM { get; set; } = new XML_Element<decimal>();
        public List<XML_Element<string>> CODE_EXP { get; set; } = new List<XML_Element<string>>();
        public List<XML_Element<string>> SL_ID { get; set; } = new List<XML_Element<string>>();
        public void Clear()
        {
            S_TIP.Clear();
            S_OSN.Clear();
            S_SUM.Clear();
            CODE_EXP.Clear();
            SL_ID.Clear();
        }
    }


    class MyValidatorV31 : IValidatorXML
    {
        private bool IsValidateRef { get;  }
        private readonly DateTime DT_04_2020 = new DateTime(2020, 04, 01);
        private readonly DateTime DT_03_2021 = new DateTime(2021, 03, 01);
        private readonly DateTime DT_08_2021 = new DateTime(2021, 08, 01);
        private XML_SCHET_item SCHET { get;  }= new XML_SCHET_item();
        private XML_Z_SL_item Z_SL { get;  } = new XML_Z_SL_item();
        private XML_SL_item SL { get;  } = new XML_SL_item();
        private XML_USL_item USL { get;  } = new XML_USL_item();
        private XML_SANK_item SANK { get;  } = new XML_SANK_item();
        private XML_Pacient_item PACIENT { get;  } = new XML_Pacient_item();
        private XML_MR_USL_N_item MR_USL_N { get; } = new XML_MR_USL_N_item();
        


        public MyValidatorV31(ErrorActionEvent err, bool IsValidateRef)
        {
            Error = err;
            this.IsValidateRef = IsValidateRef;
        }

        public event ErrorActionEvent Error;

        private XML_Element<string> CreateStringXML_Element(XmlReader r)
        {
            return new XML_Element<string>(r.Value, PositionRecord.Get(r));
        }
        private XML_Element<int> CreateIntXML_Element(XmlReader r)
        {
            int val;
            int.TryParse(r.Value, out val);
            return new XML_Element<int>(val, PositionRecord.Get(r));
        }
        private XML_Element<int?> CreateIntNullXML_Element(XmlReader r)
        {
            int val;
            return int.TryParse(r.Value, out val) ? new XML_Element<int?>(val, PositionRecord.Get(r)) : new XML_Element<int?>(null, PositionRecord.Get(r));
        }
        private XML_Element<decimal> CreateDecimalXML_Element(XmlReader r)
        {
            decimal val;
            decimal.TryParse(r.Value,NumberStyles.Float, new NumberFormatInfo() { NumberDecimalSeparator = "." }, out val);
            return new XML_Element<decimal>(val, PositionRecord.Get(r));
        }
        private XML_Element<decimal?> CreateDecimalNullXML_Element(XmlReader r)
        {
            decimal val;
            return decimal.TryParse(r.Value, NumberStyles.Float, new NumberFormatInfo() { NumberDecimalSeparator = "." }, out val) ? new XML_Element<decimal?>(val, PositionRecord.Get(r)) : new XML_Element<decimal?>(null, PositionRecord.Get(r));
        }

        public void Check(XmlReader reader)
        {
            depthXml.NextNode(reader);
            switch (reader.NodeType)
            {
                case XmlNodeType.Text:
                    switch (depthXml.Path)
                    {
                        case "ZL_LIST/SCHET/YEAR":
                                SCHET.YEAR = CreateIntXML_Element(reader);
                            break;
                        case "ZL_LIST/SCHET/MONTH":
                                SCHET.MONTH = CreateIntXML_Element(reader);
                            break;
                        case "ZL_LIST/ZGLV/FILENAME":
                                SCHET.FILENAME = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZGLV/OKATO_OMS":
                                SCHET.OKATO_OMS = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZGLV/SD_Z":
                                SCHET.SD_Z = CreateIntNullXML_Element(reader);
                            break;
                        case "ZL_LIST/SCHET/SUMMAV":
                                SCHET.SUMMAV = CreateDecimalNullXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/PACIENT/VPOLIS":
                            PACIENT.VPOLIS = CreateIntXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/PACIENT/SPOLIS":
                            PACIENT.SPOLIS = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/PACIENT/NPOLIS":
                            PACIENT.NPOLIS = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/PACIENT/ENP":
                            PACIENT.ENP = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SUMV":
                                Z_SL.SUMV = CreateDecimalXML_Element(reader);
                                SCHET.SUMV_SUM += Z_SL.SUMV.value;
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/SUM_M":
                                SL.SUM_M = CreateDecimalXML_Element(reader);
                                Z_SL.SUM_M_SUM += SL.SUM_M.value;
                            break;
                        case "ZL_LIST/ZAP/Z_SL/IDCASE":
                                SCHET.IDCASE_COUNT++;
                            break;
                        case "ZL_LIST/ZAP/Z_SL/FIRST_IDCASE":
                                Z_SL.FIRST_IDCASE = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/PR_NOV":
                                Z_SL.PR_NOV = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/DS1":
                                SL.DS1 = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/TARIF":
                                SL.TARIF = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/DS2":
                                SL.DS2.Add(CreateStringXML_Element(reader));
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/DS_ONK":
                                SL.DS_ONK = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/USL_OK":
                                Z_SL.USL_OK = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/REAB":
                                SL.REAB = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/C_ZAB":
                                SL.C_ZAB = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/RSLT_D":
                                Z_SL.RSLT_D = CreateStringXML_Element(reader); 
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/USL/PRVS":
                                USL.PRVS = CreateStringXML_Element(reader); 
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/PR_D_N":
                                SL.PR_D_N = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/P_OTK":
                                Z_SL.P_OTK = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/USL/SUMV_USL":
                                USL.SUMV_USL = CreateDecimalXML_Element(reader);
                                Z_SL.SUMV_USL_SUM += USL.SUMV_USL.value;
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/USL/P_OTK":
                                USL.P_OTK = CreateStringXML_Element(reader); 
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/USL/CODE_MD":
                                USL.CODE_MD = CreateStringXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SANK/S_SUM":
                                SANK.S_SUM = CreateDecimalXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SANK/SL_ID":
                                SANK.SL_ID.Add(CreateStringXML_Element(reader));
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SANK/CODE_EXP":
                                SANK.CODE_EXP.Add(CreateStringXML_Element(reader));
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SANK/S_TIP":
                                SANK.S_TIP = CreateIntXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SANK/S_OSN":
                                SANK.S_OSN = CreateIntXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/USL/MR_USL_N/PRVS":
                            MR_USL_N.PRVS = CreateIntNullXML_Element(reader);
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/USL/MR_USL_N/CODE_MD":
                            MR_USL_N.CODE_MD = CreateStringXML_Element(reader);
                            break;
                    }

                    break;
                case XmlNodeType.Element:
                    switch (depthXml.Path)
                    {
                        case "ZL_LIST/ZAP/Z_SL/SL/ONK_SL":
                                SL.IsONK_SL = true;
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/CONS":
                                SL.IsCONS = true;
                            break;
                     
                        case "ZL_LIST/SCHET/REF":
                            SCHET.REF = CreateStringXML_Element(reader);
                            SCHET.REF.value = "true";
                            break;

                    }
                    break;
   
                case XmlNodeType.EndElement:
                    switch (depthXml.Path)
                    {
                        case "ZL_LIST/ZAP/Z_SL/SL/USL":
                                CheckUSL();
                                USL.Clear();
                            break;
                        case "ZL_LIST/ZAP/Z_SL":
                                CheckZ_SL();
                                Z_SL.Clear();
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SANK":
                                CheckSANK();
                                SANK.Clear();
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL":
                                CheckONK();
                                SL.Clear();
                            break;
                        case "ZL_LIST/ZAP/PACIENT":
                            CheckPACIENT();
                            PACIENT.Clear();
                            break;
                        case "ZL_LIST/ZAP/Z_SL/SL/USL/MR_USL_N":
                            CheckMR_USL_N();
                            MR_USL_N.Clear();
                            break;
                    }
                    break;
            }

           


        }
        DepthXML depthXml = new DepthXML();


        private void CheckSANK()
        {
            if (SANK.S_SUM.value != 0 && SANK.SL_ID.Count == 0)
                Error(XmlSeverityType.Error, SANK.S_SUM.POS.LINE, SANK.S_SUM.POS.POS, "Для S_SUM<>0 SL_ID обязательно к заполнению", "SANK");
            if (SANK.S_TIP.value.IsEKMP() && SANK.CODE_EXP.Count == 0 && !SANK.S_OSN.value.In(43,242))
                Error(XmlSeverityType.Error, SANK.S_OSN.POS.LINE, SANK.S_OSN.POS.POS, @"Для санкций ЭКМП CODE_EXP обязательно к заполнению", "SANK");
         
        }

        private void CheckZ_SL()
        {
            if (Math.Round(Z_SL.SUMV.value, 2) != Math.Round(Z_SL.SUM_M_SUM, 2))
                Error(XmlSeverityType.Error, Z_SL.SUMV.POS.LINE, Z_SL.SUMV.POS.POS, $"Сумма законченного случая({Math.Round(Z_SL.SUMV.value, 2)}) не равна сумме случаев({Math.Round(Z_SL.SUM_M_SUM, 2)})", "SUMV");
            if (SCHET.TypeFile != XML_FileType.MTR)
            {
                if (Math.Round(Z_SL.SUMV.value, 2) != Math.Round(Z_SL.SUMV_USL_SUM, 2))
                    Error(XmlSeverityType.Error, Z_SL.SUMV.POS.LINE, Z_SL.SUMV.POS.POS, $"Сумма законченного случая({Math.Round(Z_SL.SUMV.value, 2)}) не равна сумме услуг({Math.Round(Z_SL.SUMV_USL_SUM, 2)})", "SUMV");

                
                var isRef = !string.IsNullOrEmpty(SCHET.REF.value);
                if (Z_SL.PR_NOV.value == "1" && !isRef && IsValidateRef)
                    Error(XmlSeverityType.Error, Z_SL.PR_NOV.POS.LINE, Z_SL.PR_NOV.POS.POS, $"Признак исправленной записи = 1 недопустим без указания тэга SCHET\\REF", "SUMV","ERR_ZS_1");
                if (Z_SL.PR_NOV.value == "0" && isRef && IsValidateRef)
                    Error(XmlSeverityType.Error, Z_SL.PR_NOV.POS.LINE, Z_SL.PR_NOV.POS.POS, $"Признак исправленной записи = 0 недопустим при указании тэга SCHET\\REF", "SUMV", "ERR_ZS_2");

                var isFIRST_IDCASE = !string.IsNullOrEmpty(Z_SL.FIRST_IDCASE.value);
                if (isFIRST_IDCASE && !isRef && IsValidateRef)
                    Error(XmlSeverityType.Error, Z_SL.PR_NOV.POS.LINE, Z_SL.PR_NOV.POS.POS, $"Поле FIRST_IDCASE не подлежит заполнению без указания тэга SCHET\\REF", "SUMV");
                if (!isFIRST_IDCASE && isRef && IsValidateRef)
                    Error(XmlSeverityType.Error, Z_SL.PR_NOV.POS.LINE, Z_SL.PR_NOV.POS.POS, $"Поле FIRST_IDCASE обязательно к заполнению при указании тэга SCHET\\REF", "SUMV");


            }
        }
        private void CheckUSL()
        {
            if (SCHET.TypeFile == XML_FileType.D && SCHET.DateFile >= DT_04_2020 && SCHET.DateFile <DT_08_2021)
            {
                if (USL.P_OTK.value == "0" && string.IsNullOrEmpty(USL.CODE_MD.value))
                    Error(XmlSeverityType.Error, USL.P_OTK.POS.LINE, USL.P_OTK.POS.POS, "Поле CODE_MD обязательно к заполнению при USL\\P_OTK = 0", "CODE_MD");
                if (USL.P_OTK.value == "1" && !string.IsNullOrEmpty(USL.CODE_MD.value))
                    Error(XmlSeverityType.Error, USL.CODE_MD.POS.LINE, USL.CODE_MD.POS.POS, "Поле CODE_MD не подлежит заполнению при USL\\P_OTK = 1", "CODE_MD");
                if (USL.P_OTK.value == "0" && string.IsNullOrEmpty(USL.PRVS.value))
                    Error(XmlSeverityType.Error, USL.P_OTK.POS.LINE, USL.P_OTK.POS.POS, "Поле PRVS обязательно к заполнению при USL\\P_OTK = 0", "PRVS");
                if (USL.P_OTK.value == "1" && !string.IsNullOrEmpty(USL.PRVS.value))
                    Error(XmlSeverityType.Error, USL.PRVS.POS.LINE, USL.PRVS.POS.POS, "Поле PRVS не подлежит заполнению при USL\\P_OTK = 1", "PRVS");
            }
        }

        private void CheckPACIENT()
        {
            if (SCHET.TypeFile == XML_FileType.D && SCHET.DateFile >= DT_08_2021)
            {
                switch (PACIENT.VPOLIS.value)
                {
                    case 3:
                        if (!string.IsNullOrEmpty(PACIENT.SPOLIS.value))
                            Error(XmlSeverityType.Error, PACIENT.SPOLIS.POS.LINE, PACIENT.SPOLIS.POS.POS, "Поле SPOLIS не подлежит заполнению при VPOLIS = 3", "SPOLIS", "ERR_PAC_1");
                        if (!string.IsNullOrEmpty(PACIENT.NPOLIS.value))
                            Error(XmlSeverityType.Error, PACIENT.NPOLIS.POS.LINE, PACIENT.NPOLIS.POS.POS, "Поле NPOLIS не подлежит заполнению при VPOLIS = 3", "NPOLIS", "ERR_PAC_2");
                        if (string.IsNullOrEmpty(PACIENT.ENP.value))
                            Error(XmlSeverityType.Error, PACIENT.VPOLIS.POS.LINE, PACIENT.VPOLIS.POS.POS, "Поле ENP обязательно к заполнению при VPOLIS = 3", "ENP", "ERR_PAC_3");
                        break;
                    case 2:
                        if (!string.IsNullOrEmpty(PACIENT.SPOLIS.value))
                            Error(XmlSeverityType.Error, PACIENT.SPOLIS.POS.LINE, PACIENT.SPOLIS.POS.POS, "Поле SPOLIS не подлежит заполнению при VPOLIS = 2", "SPOLIS", "ERR_PAC_4");
                        if (string.IsNullOrEmpty(PACIENT.NPOLIS.value))
                            Error(XmlSeverityType.Error, PACIENT.VPOLIS.POS.LINE, PACIENT.VPOLIS.POS.POS, "Поле NPOLIS обязательно к заполнению при VPOLIS = 2", "NPOLIS", "ERR_PAC_5");
                        break;
                    case 1:
                        if (string.IsNullOrEmpty(PACIENT.SPOLIS.value))
                            Error(XmlSeverityType.Error, PACIENT.VPOLIS.POS.LINE, PACIENT.VPOLIS.POS.POS, "Поле SPOLIS обязательно к заполнению при VPOLIS = 1", "SPOLIS", "ERR_PAC_6");
                        if (string.IsNullOrEmpty(PACIENT.NPOLIS.value))
                            Error(XmlSeverityType.Error, PACIENT.VPOLIS.POS.LINE, PACIENT.VPOLIS.POS.POS, "Поле NPOLIS обязательно к заполнению при VPOLIS = 1", "NPOLIS", "ERR_PAC_7");
                        if (!string.IsNullOrEmpty(PACIENT.ENP.value))
                            Error(XmlSeverityType.Error, PACIENT.ENP.POS.LINE, PACIENT.ENP.POS.POS, "Поле ENP не подлежит заполнению при VPOLIS = 1", "ENP", "ERR_PAC_8");
                        break;

                }

            }
        }
        private void CheckMR_USL_N()
        {
            if (SCHET.TypeFile == XML_FileType.D && SCHET.DateFile >= DT_08_2021)
            {
                if (USL.P_OTK.value == "0" && string.IsNullOrEmpty(MR_USL_N.CODE_MD.value))
                    Error(XmlSeverityType.Error, USL.P_OTK.POS.LINE, USL.P_OTK.POS.POS, "Поле CODE_MD обязательно к заполнению при USL\\P_OTK = 0", "CODE_MD", "ERR_MR_USL_N_1");
                if (USL.P_OTK.value == "1" && !string.IsNullOrEmpty(MR_USL_N.CODE_MD.value))
                    Error(XmlSeverityType.Error, MR_USL_N.CODE_MD.POS.LINE, MR_USL_N.CODE_MD.POS.POS, "Поле CODE_MD не подлежит заполнению при USL\\P_OTK = 1", "CODE_MD", "ERR_MR_USL_N_2");
                if (USL.P_OTK.value == "0" && !MR_USL_N.PRVS.value.HasValue)
                    Error(XmlSeverityType.Error, USL.P_OTK.POS.LINE, USL.P_OTK.POS.POS, "Поле PRVS обязательно к заполнению при USL\\P_OTK = 0", "PRVS", "ERR_MR_USL_N_3");
                if (USL.P_OTK.value == "1" && MR_USL_N.PRVS.value.HasValue)
                    Error(XmlSeverityType.Error, MR_USL_N.PRVS.POS.LINE, MR_USL_N.PRVS.POS.POS, "Поле PRVS не подлежит заполнению при USL\\P_OTK = 1", "PRVS", "ERR_MR_USL_N_4");
            }
        }

        private DateTime DT_10_2021 = new DateTime(2021, 10, 1);
        private void CheckONK()
        {
            var tf = SCHET.TypeFile;
            var DateFile = SCHET.DateFile;
          
            if (!string.IsNullOrEmpty(SL.DS1.value))
            {
                
                var DS1likeZ = SL.DS1.value.StartsWith("Z");
                var DS1likeC = SL.DS1.value.StartsWith("C");

                var DS1U11 = SL.DS1.value == "U11" || SL.DS1.value == "U11.9";

                var DS1likeD70_C97C00_C80 = SL.DS1.value.StartsWith("D70") && SL.DS2.Count(x => x.value.StartsWith("C97") || x.value.Substring(0, 3).Between("C00", "C80")) != 0 && DateFile < DT_04_2020;
                var DS1likeD00D09 = SL.DS1.value.Substring(0, 3).Between("D00","D09");
                var DS1likeD45D47 = SL.DS1.value.Substring(0, 3).Between("D45","D47") && DateFile>= DT_03_2021;

                if (tf == XML_FileType.H)
                {
                    if (DS1likeC)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Основной диагноз C* для файла H", "DS1");
                    if (DS1likeD70_C97C00_C80)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Основной диагноз D70* и сопутствующий C00-C80 или C97* для файла H", "DS1");
                    if (DS1likeD45D47)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Основной диагноз D45-D47 для файла H", "DS1");
                    if (DS1likeD00D09)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Основной диагноз D00-D09 для файла H", "DS1");
                    if (!DS1likeZ && Z_SL.USL_OK.value == "3" && string.IsNullOrEmpty(SL.C_ZAB.value) && !(SCHET.YEAR.value==2018 && SCHET.MONTH.value ==9) && DateFile< DT_10_2021)
                    {
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Характер основного заболевания(C_ZAB) обязателен к заполнению при оказании амбулаторной помощи, если DS1 не входит в рубрику Z", "C_ZAB");
                    }
                    if (!DS1likeZ && !DS1U11 && Z_SL.USL_OK.value == "3" && string.IsNullOrEmpty(SL.C_ZAB.value)  && DateFile >= DT_10_2021)
                    {
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Характер основного заболевания(C_ZAB) обязателен к заполнению при оказании амбулаторной помощи, если DS1 не входит в рубрику Z  и не соответствует кодам диагноза U11 и U11.9", "C_ZAB");
                    }
                }
                if (tf == XML_FileType.T)
                {
                    var needCONS= DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47 || SL.DS_ONK.value == "1";

                    if (needCONS && !SL.IsCONS)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Сведения о проведении консилиума(CONS) обязательны к заполнению при DS_ONK = 1 или C00.0<= DS1<D10 или D45<=DS1<D48", "CONS");
                    if (!needCONS && SL.IsCONS)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Сведения о проведении консилиума(CONS) не подлежат заполнению при DS_ONK = 0 и (DS1<C00 или D10<= DS1<D45 или DS1>=D48) ", "CONS");
                    var needONK_SL = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needONK_SL && !SL.IsONK_SL)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Сведения о случае лечения онк.заболевания(ONK_SL) обязательно к заполнению при C00.0<= DS1<D10 или D45<=DS1<D48", "ONK_SL");
                    if (!needONK_SL && SL.IsONK_SL)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Сведения о случае лечения онк.заболевания(ONK_SL) не подлежат к заполнению при DS1<C00 или D10<= DS1<D45 или DS1>=D48", "ONK_SL");

                    var needC_ZAB = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needC_ZAB && string.IsNullOrEmpty(SL.C_ZAB.value))
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Характер основного заболевания(C_ZAB) обязателен к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "C_ZAB");

                    var needTARIF = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needTARIF && string.IsNullOrEmpty(SL.TARIF.value))
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Тариф(TARIF) обязателен к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "TARIF");
                }

                if (tf == XML_FileType.C)
                {
                    if (SL.DS_ONK.value == "0")
                    {
                        if (!DS1likeC && !DS1likeD00D09 && !DS1likeD70_C97C00_C80 && !DS1likeD45D47)
                            Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, $"Основной диагноз не C*/D00-D09{(DateFile < DT_04_2020? "/D70*":"")}{(DateFile >= DT_03_2021 ? "/D45-D47" : "")} для файла С при DS_ONK = 0", "DS1");
                    }
                    var needCONS = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47 || SL.DS_ONK.value == "1";
                    if (needCONS && !SL.IsCONS)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, $"Сведения о проведении консилиума(CONS) обязательно к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "CONS");
                    if (!needCONS && SL.IsCONS)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, $"Сведения о проведении консилиума(CONS) не подлежат заполнению при DS1<C00 или D10<=DS1<D45 или DS1>=D48", "CONS");
                    var needONK_SL = (DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47) && Z_SL.USL_OK.value != "4" && SL.REAB.value != "1" && SL.DS_ONK.value != "1";
                    if (needONK_SL && !SL.IsONK_SL)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, $"Сведения о случае лечения онк.заболевания(ONK_SL) обязательно к заполнению при (C00.0<=DS1<D10 или D45<=DS1<D48) и USL_OK<>4 и REAB<>1 и DS_ONK=0", "ONK_SL");
                    if (!needONK_SL && SL.IsONK_SL)
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, $"Сведения о случае лечения онк.заболевания(ONK_SL) не подлежат заполнению при DS1<C00 или D10<=DS1<D45 или DS1>=D48 или USL_OK=4 или REAB=1", "ONK_SL");
                    var needC_ZAB = (DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47) && Z_SL.USL_OK.value != "4";
                    if (needC_ZAB && string.IsNullOrEmpty(SL.C_ZAB.value))
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, $"Характер основного заболевания(C_ZAB) обязателен к заполнению при (C00.0<=DS1<D10 или D45<=DS1<D48) и (USL_OK<>4)", "C_ZAB");
                    var needTARIF = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needTARIF && string.IsNullOrEmpty(SL.TARIF.value))
                        Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Тариф(TARIF) обязателен к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "TARIF");
                }
            }

            if (tf== XML_FileType.D   && DateFile>=DT_04_2020)
            {
                if (Z_SL.P_OTK.value == "0" && string.IsNullOrEmpty(Z_SL.RSLT_D.value))
                    Error(XmlSeverityType.Error, Z_SL.P_OTK.POS.LINE, Z_SL.P_OTK.POS.POS, "Поле RSLT_D обязательно к заполнению при P_OTK = 0", "RSLT_D");
                if (Z_SL.P_OTK.value == "1" && !string.IsNullOrEmpty(Z_SL.RSLT_D.value))
                    Error(XmlSeverityType.Error, Z_SL.RSLT_D.POS.LINE, Z_SL.RSLT_D.POS.POS, "Поле RSLT_D не подлежит заполнению при P_OTK = 1", "CODE_MD");
                if(Z_SL.P_OTK.value == "0" && string.IsNullOrEmpty(SL.DS1.value))
                    Error(XmlSeverityType.Error, Z_SL.P_OTK.POS.LINE, Z_SL.P_OTK.POS.POS, "Поле DS1 обязательно к заполнению при P_OTK = 0", "DS1");
                if (!string.IsNullOrEmpty(SL.DS1.value) && string.IsNullOrEmpty(SL.PR_D_N.value))
                    Error(XmlSeverityType.Error, SL.DS1.POS.LINE, SL.DS1.POS.POS, "Поле PR_D_N обязательно к заполнению при DS1 не пустое", "DS1");
            }
        }

        private void CheckSCHET()
        {
            if (SCHET.SD_Z.value.HasValue)
            {
                if (SCHET.SD_Z.value.Value != SCHET.IDCASE_COUNT)
                {
                    Error(XmlSeverityType.Error, SCHET.SD_Z.POS.LINE, SCHET.SD_Z.POS.POS, $"Кол-во случаев в реестре {SCHET.IDCASE_COUNT}, однако SD_Z = {SCHET.SD_Z.value.Value}", "SD_Z");
                }
            }
            if (SCHET.SUMMAV.value.HasValue)
            {
                if (SCHET.SUMMAV.value.Value != Math.Round(SCHET.SUMV_SUM, 2))
                {
                    Error(XmlSeverityType.Error, SCHET.SUMMAV.POS.LINE, SCHET.SUMMAV.POS.POS, $"Сумма случаев в реестре {Math.Round(SCHET.SUMV_SUM, 2)}, однако SUMMAV = {SCHET.SUMMAV.value.Value}", "SUMMAV");
                }
            }
        }


        public void Close()
        {
            CheckSCHET();
        }
    }


    public class DepthXML
    {
        List<string> Nodes = new List<string>();
        private int Depth = -1;
        private void AddDepth(string Name)
        {
            Depth++;
            Nodes.Add(Name);
        }

        private void RemoveDepth()
        {
            Depth--;
            Nodes.RemoveAt(Nodes.Count-1);
        }

        private void SetLast(string Name)
        {
            Nodes.Remove(Nodes.Last());
            Nodes.Add(Name);
        }
        
        public string Path => string.Join("/", Nodes);
        public void NextNode(XmlReader r)
        {
           
            if (r.NodeType == XmlNodeType.Element)
            {
                var comp = r.Depth.CompareTo(Depth);
                switch (comp)
                {
                    case 0: SetLast(r.Name); break;
                    case 1: AddDepth(r.Name); break;
                   default:
                       throw new Exception($"Непредвиденное поведение при поддержке метки XML: {r.NodeType}:{r.Name}");
                   
                }
            }

            if (r.NodeType == XmlNodeType.EndElement)
            {
                var comp = r.Depth.CompareTo(Depth);
                switch (comp)
                {
                    case -1: RemoveDepth(); break;
                    case 0: break;
                    default:
                        throw new Exception($"Непредвиденное поведение при поддержке метки XML: {r.NodeType}:{r.Name}");
                }
            }
        }

    }


    class ProtocolValidator31 : IValidatorXML
    {
      
        public ProtocolValidator31(ErrorActionEvent err)
        {
            Error = err;
        }

        public event ErrorActionEvent Error;
       
        private PositionRecord SANK_POS = new PositionRecord();
        private string prevnodes;

        private decimal S_SUM;
        private decimal S_TIP;
        private decimal? S_OSN;
        private int C_SANK_SL_ID;
        private int C_SANK_CODE_EXP;
        private string NUM_ACT;
        private string DATE_ACT;

        public void Close()
        {
        }

        public void Check(XmlReader reader)
        {

            switch (reader.NodeType)
            {
                case XmlNodeType.Text:

                    switch (prevnodes)
                    {
                        case "S_SUM":
                            SANK_POS.Set(reader);
                            if (reader.Value != "")
                                S_SUM = Convert.ToDecimal(reader.Value, new NumberFormatInfo() {NumberDecimalSeparator = "."});
                            break;
                        case "SL_ID":
                            if (reader.Value != "" && S_SUM != 0)
                                C_SANK_SL_ID++;
                            break;
                        case "CODE_EXP":
                            if (!string.IsNullOrEmpty(reader.Value))
                                C_SANK_CODE_EXP++;
                            break;
                        case "NUM_ACT":
                            NUM_ACT = reader.Value;
                            break;
                        case "DATE_ACT":
                            DATE_ACT = reader.Value;
                            break;
                        case "S_TIP":
                            if (reader.Value != "")
                                S_TIP = Convert.ToDecimal(reader.Value);
                            break;
                        case "S_OSN":
                            if (reader.Value != "")
                                S_OSN = Convert.ToDecimal(reader.Value);
                            break;
                    }

                    break;
                case XmlNodeType.Element:
                    prevnodes = reader.Name;
                    break;
                case XmlNodeType.EndElement:
                    switch (reader.Name)
                    {
                       
                        case "SANK":
                            if (S_SUM != 0 && C_SANK_SL_ID == 0)
                                Error(XmlSeverityType.Error, SANK_POS.LINE, SANK_POS.POS, "Для S_SUM<>0 SL_ID обязательно к заполнению", "SANK");
                            if (S_TIP >= 30 && C_SANK_CODE_EXP == 0)
                                Error(XmlSeverityType.Error, SANK_POS.LINE, SANK_POS.POS, @"Для санкций ЭКМП CODE_EXP обязательно к заполнению", "SANK");
                           /* if (S_SUM!=0  && string.IsNullOrEmpty(NUM_ACT))
                                Error(XmlSeverityType.Error, SANK_POS.LINE, SANK_POS.POS, @"NUM_ACT обязательно к заполнению при S_SUM<>0", "SANK");
                            if (S_SUM != 0 && string.IsNullOrEmpty(DATE_ACT))
                                Error(XmlSeverityType.Error, SANK_POS.LINE, SANK_POS.POS, @"DATE_ACT обязательно к заполнению при S_SUM<>0", "SANK");*/
                            if (S_SUM != 0 && !S_OSN.HasValue)
                                Error(XmlSeverityType.Error, SANK_POS.LINE, SANK_POS.POS, @"S_OSN обязательно к заполнению при S_SUM<>0", "SANK");

                            C_SANK_SL_ID = 0;
                            S_SUM = 0;
                            C_SANK_CODE_EXP = 0;
                            S_TIP = 0;
                            S_OSN = null;
                            DATE_ACT = null;
                            NUM_ACT = null;
                            break;
                    }
                    break;
            }
        }
    }


    public static partial class  Ext
    {
        public static bool Between(this string val,string val1, string val2)
        {
          return string.Compare(val, val1, StringComparison.Ordinal) >= 0 && String.Compare(val, val2, StringComparison.Ordinal) <= 0;
        }

        public static bool IsMEK(this int val)
        {
            return val.ToString().StartsWith("1");
        }
        public static bool IsMEE(this int val)
        {
            return val.ToString().StartsWith("2") || val.ToString().StartsWith("5");
        }
        public static bool IsEKMP(this int val)
        {
            return val.ToString().StartsWith("3") || val.ToString().StartsWith("4") || val.ToString().StartsWith("7") || val.ToString().StartsWith("8");
        }
        public static bool In(this int val, params int[] values)
        {
            return values.Contains(val);
        }
    }
}

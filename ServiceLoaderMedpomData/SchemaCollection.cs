using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Globalization;
using System.Data;
using System.Net;

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
    public class SchemaColection
    {
        //Схемы
        [DataMember]
        private SerializableDictionary<VersionMP, VersionSchemaElement> Schemas { get; set; }
        public SchemaColection()
        {
            newSc();
        }

        public SchemaColectionFindResult FindSchema(string Version, DateTime dt, FileType ft)
        {
            var sc = Schemas.Where(x => x.Value.VersionsZGLV.Contains(Version)).ToList();
            if (sc.Count != 1)
                return new SchemaColectionFindResult{Result = false, Exception = $"Не допустимая версия документа: {Version}"};

            if (!sc[0].Value.SchemaElements.ContainsKey(ft))
                return new SchemaColectionFindResult
                    {Result = false, Exception = $"Для версии документа {Version} нет схемы для файла {ft.ToString()}"};

            var value = sc[0].Value.SchemaElements[ft]
                .FindAll(x => dt >= x.DATE_B && (dt <= x.DATE_E || !x.DATE_E.HasValue));

            if (value.Count > 1)
                return new SchemaColectionFindResult
                {
                    Result = false,
                    Exception = $"Для версии документа {Version} файла {ft.ToString()} найдено более 1 схемы документа"
                };

            if (value.Count == 0)
                return new SchemaColectionFindResult
                {
                    Result = false,
                    Exception =
                        $"Для версии документа {Version} файла {ft.ToString()} " +
                        $"не найдено ни" +
                        $" одной схемы. Файл от {dt:MM-yyyy}, доступны схемы:{Environment.NewLine}{string.Join(Environment.NewLine, sc[0].Value.SchemaElements[ft].Select(x => $"с {x.DATE_B:dd-MM-yyyy}{(x.DATE_E.HasValue ? $" по {x.DATE_E:dd-MM-yyyy}" : "")}"))}"
                };
            return new SchemaColectionFindResult() {Result = true, Value = value[0], Vers = sc[0].Key};
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
                VersionSchemaElement.Check(Schemas[VersionMP.V3_1]);
            }
        }

        public List<VersionMP> Versions => Schemas.Keys.ToList();

        public List<SchemaElementValue> this[VersionMP version, FileType _type]
        {
            get { return Schemas[version].SchemaElements[_type]; }
            set
            {
                Schemas[version].SchemaElements[_type] = value;
            }
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

    public class SchemaColectionFindResult
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
        public ErrorProtocolXML()
        {
        }

        public string MessageOUT => $"[{Line}][{column}]: {Comment}";
    }

    public class CheckXMLValidator
    {
        public CheckXMLValidator(VersionMP v, bool IsMTRProtocol =false)
        {
            this.IsMTRProtocol = IsMTRProtocol;
            Version = v;
        }
        public VersionMP Version { get; set; }
        public bool IsMTRProtocol { get; set; }
    }

    public class SchemaChecking
    {

        public enum PrichinAv
        {
            EXEPT,
            NOT_FOUND
        }

        List<ErrorProtocolXML> ListER;// лист ошибок
        List<string> listnode; //Хранитель путь к текущему элементу в дереве XML
        //FileItem FileItem;//Файл для проверки  
        string filepath = "";
        LogFile FileLog;
        bool resul; //результат при проверки схемы
        Decimal n_zap; //Номер текущей записи
        string id_case = "";//номер случая
        string id_serv = ""; //номер услуги
        string id_pac = ""; //номер ID_PAC
        string sl_id = ""; //номер SL_ID
        bool fileL;
        public SchemaChecking()
        {
            ListER = new List<ErrorProtocolXML>();
            listnode = new List<string>();
        }



        /// <summary>
        /// Проверка доступности файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>true - файл доступен, false - файл недоступен</returns>        
        public static bool CheckFileAv(string path, out PrichinAv? Pr)
        {
            Stream stream = null;
            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read);
                stream.Close();
                stream.Dispose();
                Pr = null;
                return true;
            }
            catch (FileNotFoundException)
            {
                Pr = PrichinAv.NOT_FOUND;
                stream?.Dispose();
                return false;
            }
            catch (Exception)
            {
                Pr = PrichinAv.EXEPT;
                stream?.Dispose();
                return false;
            }
        }


        /// <summary>
        /// Проверка доступности файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>true - файл доступен, false - файл недоступен</returns>        
        public static bool CheckFileAv(string path)
        {
            try
            {
                Stream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                stream.Close();

                return true;
            }
            catch (FileNotFoundException)
            {

                return false;
            }
            catch (Exception)
            {

                return false;
            }
        }
        public static bool CheckDirAv(string path)
        {
            try
            {
                var rzlt = true; ;
                foreach (var F in Directory.GetFiles(path))
                {
                    if (!SchemaChecking.CheckFileAv(F))
                    {
                        rzlt = false;
                    }
                }
                return rzlt;
            }
            catch (FileNotFoundException)
            {

                return false;
            }
            catch (Exception)
            {

                return false;
            }
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
        /// <param name="Schemas">Колекция схем</param>
        /// <returns>0-ошибка при проверке 1 проверка успешна</returns>
        /// 
        public bool CheckSchema(FileItemBase _File, string PathXSD, bool isvalidate = true)
        {
            var prevnodes = "";
            var LineNumber = 0;
            var LinePosition = 0;
            var step = 0;
            try
            {

                fileL = false;
                listnode.Clear();
                ListER.Clear();

                FileLog = _File.FileLog;
                filepath = _File.FilePach;
                //путь к доку
                var path = _File.FilePach;
                //путь к схеме
                var pathXSD = PathXSD;
                //путь к логу
                var pathLog = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".LOG";
                //проверка доступности файла
                SchemaChecking.PrichinAv? pr;
                while (!CheckFileAv(path, out pr)) { };//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                step++;
                var XMLSettings = new XmlReaderSettings();
                XMLSettings.Schemas.Add(null, pathXSD);
                XMLSettings.ValidationType = ValidationType.Schema;
                XMLSettings.ValidationEventHandler += new ValidationEventHandler(SettingsValidationEventHandler);

                var NS = "";
                var ns_error = false;
                foreach (XmlSchema schema in XMLSettings.Schemas.Schemas())  // foreach is used to simplify the example
                {
                    NS = schema.TargetNamespace == null ? "" : schema.TargetNamespace;
                }
                step++;
               
                using (var r = new XmlTextReader(new StreamReader(path, Encoding.GetEncoding(1251))))
                {
                    _File.FileLog.WriteLn("Проверка файла на соответствие схеме:");
                    using (var reader = XmlReader.Create(r, XMLSettings))
                    {
                        resul = true;
                        var currdep = -1;

                        var count_pr_nov1 = 0;
                        var count_pr_nov0 = 0;
                        IValidatorXML validate = null;
                        if (isvalidate)
                        {
                            switch (_File.Version)
                            {
                                case VersionMP.V2_1: validate = new MyValidatorV2(ErrorAction); break;
                                case VersionMP.V3_0: validate = new MyValidatorV3(ErrorAction); break;
                                case VersionMP.V3_1: validate = new MyValidatorV31(ErrorAction); break;
                                default:
                                    validate = new MyValidatorV31(ErrorAction);
                                    break;
                            }
                        }

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
                                        n_zap = Convert.ToDecimal(reader.Value);
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
                                        if (Convert.ToDecimal(reader.Value) == 1)
                                            count_pr_nov1++;
                                        else
                                            count_pr_nov0++;
                                        break;
                                    }
                                }
                            }
                            //Пользовательский валидатор


                            //проверка NameSpace и слежение за стрктурой(глубиной погружения)
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (!ns_error && NS != (reader.NamespaceURI == null ? "" : reader.NamespaceURI))
                                {
                                    ErrorAction(XmlSeverityType.Error, LineNumber, LinePosition,$"Неверный Namespace=\"{reader.NamespaceURI}\" ожидается \"{NS}\"", "XML");
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

                        step++;

                        reader.Close();
                        r.Close();
                        validate?.Close();

                        if (count_pr_nov0 > 0 && count_pr_nov1 == 0)
                            _File.DOP_REESTR = false;
                        if (count_pr_nov0 == 0 && count_pr_nov1 > 0)
                            _File.DOP_REESTR = true;

                        if (resul)
                            _File.FileLog.WriteLn("Файл составлен правильно");
                        else
                        {
                            var pathToXml = Path.Combine(Path.GetDirectoryName(FileLog.FilePath), Path.GetFileNameWithoutExtension(filepath) + "FLK.xml");
                            XMLfileFLK(pathToXml, Path.GetFileName(filepath), ListER);
                            _File.PATH_LOG_XML = pathToXml;
                        }
                        return resul;
                    }
                }
            }
            catch (Exception ex)
            {
                _File.FileLog.WriteLn($"Ошибка при проверке документа на соответствие схеме step{step} [{LineNumber},{LinePosition}]{prevnodes}: {ex.Message}");
                return false;
            }
        }
        public List<ErrorProtocolXML> GetProtokol => ListER;

        public List<ErrorProtocolXML> CheckXML(Stream st, string XSD, CheckXMLValidator CXV = null)
        {
            var prevnodes = "";
            var LineNumber = 0;
            var LinePosition = 0;
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



                var currdep = -1;
             
                using (var r = new XmlTextReader(st))
                {
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
                                    validate = new MyValidatorV31(ErrorAction);
                                    break;
                                default:
                                    validate = new MyValidatorV31(ErrorAction);
                                    break;
                            }
                        }
                    }

                    using (var reader = XmlReader.Create(r, XMLSettings))
                    {
                        while (reader.Read())
                        {
                            LineNumber = ((IXmlLineInfo)reader).LineNumber;
                            LinePosition = ((IXmlLineInfo)reader).LinePosition;
                            if (reader.NodeType == XmlNodeType.Text)
                            {
                                //Просто для адрисации
                                if (prevnodes == "N_ZAP")
                                    n_zap = Convert.ToDecimal(reader.Value);
                                if (prevnodes == "IDSERV")
                                    id_serv = reader.Value;
                                if (prevnodes == "IDCASE")
                                    id_case = reader.Value;
                                if (prevnodes == "SL_ID")
                                    sl_id = reader.Value;
                                if (prevnodes == "ID_PAC")
                                    id_pac = reader.Value;
                             
                             

                                //проверка NameSpace и слежение за стрктурой(глубиной погружения)
                            }
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (!ns_error && NS != (reader.NamespaceURI == null ? "" : reader.NamespaceURI))
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
            }
            catch(Exception ex)
            {
                ListER.Add(new ErrorProtocolXML() { Comment =$"Ошибка при проверке документа на соответствие схеме[{LineNumber},{LinePosition}]{prevnodes}: {ex.Message}"});
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

        public static string GetCode_fromXML(string path, string ElementName)
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

        public static Dictionary<string, string> GetCode_fromXML(string path, params string[] ElementName)
        {
            var res = new Dictionary<string, string>();
   
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
                            if (res.ContainsKey(el))
                            {
                                res[el] = reader.Value;
                            }
                            else
                            {
                                res.Add(el, reader.Value);
                            }
                            if(res.Count== ElementName.Length)
                                break;
                        }
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
            var name = "";
            var reader = sender as XmlReader;
            if (reader != null)
                name = reader.Name;
            ErrorAction(e.Severity, e.Exception.LineNumber, e.Exception.LinePosition, e.Message, name);
        }

        void ErrorAction(XmlSeverityType Severity, int LineNumber, int LinePosition, string Message, string NamePol)
        {
            if (resul)
            {
                resul = false;
                FileLog?.WriteLn("!!!Отказано в приеме файла полностью. Файл не соответствует схеме!!!");
            }

            switch (Severity)
            {
                case XmlSeverityType.Error:
                    FileLog?.WriteLn("ERROR: [" + LineNumber + "," + LinePosition + "] " + Message);
                    break;
                case XmlSeverityType.Warning:
                    FileLog?.WriteLn("WARNING: [" + LineNumber + "," + LinePosition + "] " + Message);
                    break;
            }

            var item = new ErrorProtocolXML {OSHIB = 41, IM_POL = NamePol, Comment = Message, Line =  LineNumber.ToString(),column =  LinePosition.ToString() };

            if (fileL) item.Comment += $"ID_PAC = {id_pac}";
            if (listnode.Count >= 2)
            {
                item.BAS_EL = listnode[listnode.Count - 2];
            }



            if (listnode.Contains("ZAP"))
                item.N_ZAP = n_zap.ToString();
            if (listnode.Contains("SLUCH") || listnode.Contains("Z_SL"))
                item.IDCASE = id_case;
            if (listnode.Contains("SL"))
                item.SL_ID = sl_id;
            if (listnode.Contains("PERS"))
                item.N_ZAP = id_pac;

            if (listnode.Contains("USL"))
                item.ID_SERV = id_serv.ToString();

            ListER.Add(item);
            //e.Exception.Data.Values;
        }

    }

    delegate void ErrorActionEvent(XmlSeverityType Severity, int LineNumber, int LinePosition, string Message, string NamePol);

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
    }

    class MyValidatorV31 : IValidatorXML
    {
        private bool Hfile;
        private bool Cfile;
        private bool Tfile;
        private bool Dfile;
        private bool mtrFile;

        private string YEAR;
        private string MONTH;

        private int? SD_Z;
        PositionRecord SD_Z_POS = new PositionRecord();
        private int SL_COUNT;

        private decimal? SUMMAV;
        PositionRecord SUMMAV_POS = new PositionRecord();
        private decimal SUMV_SUM;



        public MyValidatorV31(ErrorActionEvent err)
        {
            Error = err;
        }

        public event ErrorActionEvent Error;

        private string DS1 = "";
        private PositionRecord DS1_POS = new PositionRecord();

        private string TARIF = "";
        private PositionRecord TARIF_POS = new PositionRecord();

        private List<string> DS2 = new List<string>();
        private string DS_ONK = "";

        private PositionRecord SANK_POS = new PositionRecord();
       

        private decimal S_SUM;
        private decimal S_TIP;
        private decimal? S_OSN;
        private decimal SUMV;
        private PositionRecord SUMV_POS = new PositionRecord();
        private decimal SUM_M_SUM;
        private decimal SUMV_USL_SUM;
        private int C_SANK_SL_ID;
        private int C_SANK_CODE_EXP;

        private bool cons;
        private string usl_ok = "";
        private string reab = "";
        private string c_zab = "";
        private bool onk_sl;
        private string prevnodes = "";

        

        private string RSLT_D;
        private PositionRecord RSLT_D_POS = new PositionRecord();
        private string PR_D_N;
       // private PositionRecord PR_D_N_POS = new PositionRecord();
        private string PRVS_USL;
        private PositionRecord PRVS_USL_POS = new PositionRecord();
        private string CODE_MD;
        private PositionRecord CODE_MD_POS = new PositionRecord();
        private string P_OTK;
        private PositionRecord P_OTK_POS = new PositionRecord();
        private string P_OTK_USL;
        private PositionRecord P_OTK_USL_POS = new PositionRecord();
        private  bool isZ_SL { get; set; }
        private bool isSL { get; set; }
        private bool isUSL { get; set; }
        private bool isSCHET { get; set; }

        public void Check(XmlReader reader)
        {

            switch (reader.NodeType)
            {
                case XmlNodeType.Text:

                    switch (prevnodes)
                    {
                        case "YEAR":
                            if (isSCHET)
                                YEAR = reader.Value;
                            break;
                        case "MONTH":
                            if (isSCHET)
                            {
                                MONTH = reader.Value;
                                SetDate();
                            }
                                
                            break;
                        case "FILENAME":
                            var FILENAME = reader.Value.ToUpper();
                            if (FILENAME != "")
                            {
                                if (FILENAME[0] == 'H')
                                    Hfile = true;
                                if (FILENAME[0] == 'C')
                                    Cfile = true;
                                if (FILENAME[0] == 'T')
                                    Tfile = true;
                                if (FILENAME[0] == 'D')
                                    Dfile = true;
                            }

                            break;
                        case "OKATO_OMS":
                            mtrFile = true;
                            break;
                        case "SD_Z":
                            SD_Z = Convert.ToInt32(reader.Value);
                            SD_Z_POS.Set(reader);
                            break;
                        case "SUMMAV":
                            SUMMAV = Convert.ToDecimal(reader.Value,new NumberFormatInfo() {NumberDecimalSeparator = "."});
                            SUMMAV_POS.Set(reader);
                            break;

                        case "SUMV":
                            SUMV = Convert.ToDecimal(reader.Value,new NumberFormatInfo() {NumberDecimalSeparator = "."});
                            SUMV_SUM += SUMV;
                            SUMV_POS.Set(reader);
                            break;
                        case "SUM_M":
                            SUM_M_SUM += Convert.ToDecimal(reader.Value,new NumberFormatInfo() {NumberDecimalSeparator = "."});
                            break;
                        case "SUMV_USL":
                            SUMV_USL_SUM += Convert.ToDecimal(reader.Value,new NumberFormatInfo() {NumberDecimalSeparator = "."});
                            break;
                        case "IDCASE":
                            SL_COUNT++;
                            break;
                        case "DS1":
                            if (isSL)
                            {
                                DS1 = reader.Value;
                                if (DS1.Length < 3) DS1 = "";
                                DS1_POS.Set(reader);
                            }
                            break;

                        case "TARIF":
                            if (isSL)
                            {
                                TARIF = reader.Value;
                                TARIF_POS.Set(reader);
                            }
                            break;
                        case "DS2":
                            if (reader.Value.Length >= 3) DS2.Add(reader.Value);
                            break;
                        case "DS_ONK":
                            DS_ONK = reader.Value;
                            break;
                        case "USL_OK":
                            usl_ok = reader.Value;
                            break;
                        case "REAB":
                            reab = reader.Value;
                            break;
                        case "C_ZAB":
                            c_zab = reader.Value;
                            break;
                        case "RSLT_D":
                            RSLT_D = reader.Value;
                            RSLT_D_POS.Set(reader);
                            break;
                        case "PRVS":
                            if (isUSL)
                            {
                                PRVS_USL = reader.Value;
                                PRVS_USL_POS.Set(reader);
                            }
                            break;
                        case "PR_D_N":
                            if (isSL)
                            {
                                PR_D_N = reader.Value;
                              //  PR_D_N_POS.Set(reader);
                            }
                            break;
                        case "P_OTK":
                            if (isZ_SL && !isUSL)
                            {
                                P_OTK = reader.Value;
                                P_OTK_POS.Set(reader);
                            }
                            if (isUSL)
                            {
                                P_OTK_USL = reader.Value;
                                P_OTK_USL_POS.Set(reader);
                            }
                            break;
                        case "CODE_MD":
                            if (isUSL)
                            {
                                CODE_MD = reader.Value;
                                CODE_MD_POS.Set(reader);
                            }
                                
                            break;
                        case "S_SUM":
                            SANK_POS.Set(reader);
                            if (reader.Value != "")
                                S_SUM = Convert.ToDecimal(reader.Value,
                                    new NumberFormatInfo() {NumberDecimalSeparator = "."});
                            break;
                        case "SL_ID":
                            if (reader.Value != "" && S_SUM != 0)
                                C_SANK_SL_ID++;
                            break;
                        case "CODE_EXP":
                            if (!string.IsNullOrEmpty(reader.Value))
                                C_SANK_CODE_EXP++;
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
                    switch (reader.Name)
                    {
                        case "ONK_SL":
                            onk_sl = true;
                            break;
                        case "CONS":
                            cons = true;
                            break;
                        case "SL":
                            isSL = true;
                            break;
                        case "USL":
                            isUSL = true;
                            break;
                        case "Z_SL":
                            isZ_SL = true;
                            break;
                        case "SCHET":
                            isSCHET = true;
                            break;
                    }
                    break;
   
                case XmlNodeType.EndElement:
                    switch (reader.Name)
                    {
                        case "USL":
                            CheckUSL();
                            isUSL = false;
                            PRVS_USL = CODE_MD = P_OTK_USL = "";
                            PRVS_USL_POS.Clear();
                            CODE_MD_POS.Clear();
                            P_OTK_USL_POS.Clear();
                            break;
                        case "Z_SL":
                            isZ_SL = false;
                            break;
                        case "SUMV":
                            if (Math.Round(SUMV, 2) != Math.Round(SUM_M_SUM, 2))
                                Error(XmlSeverityType.Error, SUMV_POS.LINE, SUMV_POS.POS,$"Сумма законченного случая({Math.Round(SUMV, 2)}) не равна сумме случаев({Math.Round(SUM_M_SUM, 2)})","SUMV");
                            if (!mtrFile)
                            {
                                if (Math.Round(SUMV, 2) != Math.Round(SUMV_USL_SUM, 2))
                                    Error(XmlSeverityType.Error, SUMV_POS.LINE, SUMV_POS.POS,$"Сумма законченного случая({Math.Round(SUMV, 2)}) не равна сумме услуг({Math.Round(SUMV_USL_SUM, 2)})","SUMV");
                            }
                            SUMV = SUMV_USL_SUM = SUM_M_SUM = 0;
                            break;
                        case "SANK":
                            if (S_SUM != 0 && C_SANK_SL_ID == 0)
                                Error(XmlSeverityType.Error, SANK_POS.LINE, SANK_POS.POS,"Для S_SUM<>0 SL_ID обязательно к заполнению", "SANK");
                            if (S_TIP >= 30 && C_SANK_CODE_EXP == 0 && S_OSN != 43)
                                Error(XmlSeverityType.Error, SANK_POS.LINE, SANK_POS.POS,@"Для санкций ЭКМП CODE_EXP обязательно к заполнению", "SANK");
                            C_SANK_SL_ID = 0;
                            S_SUM = 0;
                            C_SANK_CODE_EXP = 0;
                            S_TIP = 0;
                            S_OSN = null;
                            break;
                        case "SL":
                            CheckONK();
                            DS1 = PR_D_N = P_OTK = usl_ok = DS_ONK = reab = c_zab = RSLT_D =  PR_D_N=TARIF =  "";
                            TARIF_POS.Clear();
                            RSLT_D_POS.Clear();
                            P_OTK_POS.Clear();
                            DS1_POS.Clear();
                            DS2.Clear();
                            cons = onk_sl = false;
                            isSL = false;
                            break;
                        case "SCHET":
                            isSCHET = false;
                            break;
                    }
                    break;
            }
        }

        private void CheckUSL()
        {
            if (Dfile && DateFile >= DT_04_2020)
            {
                if (P_OTK_USL == "0" && string.IsNullOrEmpty(CODE_MD))
                    Error(XmlSeverityType.Error, P_OTK_USL_POS.LINE, P_OTK_USL_POS.POS, "Поле CODE_MD обязательно к заполнению при USL\\P_OTK = 0", "CODE_MD");
                if (P_OTK_USL == "1" && !string.IsNullOrEmpty(CODE_MD))
                    Error(XmlSeverityType.Error, CODE_MD_POS.LINE, CODE_MD_POS.POS, "Поле CODE_MD не подлежит заполнению при USL\\P_OTK = 1", "CODE_MD");
                if (P_OTK_USL == "0" && string.IsNullOrEmpty(PRVS_USL))
                    Error(XmlSeverityType.Error, P_OTK_USL_POS.LINE, P_OTK_USL_POS.POS, "Поле PRVS обязательно к заполнению при USL\\P_OTK = 0", "PRVS");
                if (P_OTK_USL == "1" && !string.IsNullOrEmpty(PRVS_USL))
                    Error(XmlSeverityType.Error, PRVS_USL_POS.LINE, PRVS_USL_POS.POS, "Поле PRVS не подлежит заполнению при USL\\P_OTK = 1", "PRVS");
            }
        }
        private void CheckONK()
        {
            if (DS1 != "")
            {
                var DS1likeZ = DS1.StartsWith("Z");
                var DS1likeC = DS1.StartsWith("C");

                var DS1likeD70_C97C00_C80 = DS1.StartsWith("D70") && DS2.Count(x => x.StartsWith("C97") || x.Substring(0, 3).Between("C00", "C80")) != 0 && DateFile < DT_04_2020;
                var DS1likeD00D09 = DS1.Substring(0, 3).Between("D00","D09");
                var DS1likeD45D47 = DS1.Substring(0, 3).Between("D45","D47") && DateFile>= DT_03_2021;

                if (Hfile)
                {
                    if (DS1likeC)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Основной диагноз C* для файла H", "DS1");
                    if (DS1likeD70_C97C00_C80)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Основной диагноз D70* и сопутствующий C00-C80 или C97* для файла H", "DS1");
                    if (DS1likeD45D47)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Основной диагноз D45-D47 для файла H", "DS1");
                    if (DS1likeD00D09)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Основной диагноз D00-D09 для файла H", "DS1");
                    if (!DS1likeZ && usl_ok == "3" && c_zab == "" && !(YEAR=="2018" && MONTH=="9"))
                    {
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Характер основного заболевания(C_ZAB) обязателен к заполнению при оказании амбулаторной помощи, если DS1 не входит в рубрику Z", "C_ZAB");
                    }
                }
                if (Tfile)
                {
                    var needCONS= DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47 || DS_ONK == "1";

                    if (needCONS && !cons)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Сведения о проведении консилиума(CONS) обязательны к заполнению при DS_ONK = 1 или C00.0<= DS1<D10 или D45<=DS1<D48", "CONS");
                    if (!needCONS && cons)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Сведения о проведении консилиума(CONS) не подлежат заполнению при DS_ONK = 0 и (DS1<C00 или D10<= DS1<D45 или DS1>=D48) ", "CONS");
                    var needONK_SL = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needONK_SL && !onk_sl)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Сведения о случае лечения онк.заболевания(ONK_SL) обязательно к заполнению при C00.0<= DS1<D10 или D45<=DS1<D48", "ONK_SL");
                    if (!needONK_SL && onk_sl)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Сведения о случае лечения онк.заболевания(ONK_SL) не подлежат к заполнению при DS1<C00 или D10<= DS1<D45 или DS1>=D48", "ONK_SL");
                    var needC_ZAB = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needC_ZAB && string.IsNullOrEmpty(c_zab))
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Характер основного заболевания(C_ZAB) обязателен к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "C_ZAB");
                    var needTARIF = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needTARIF && string.IsNullOrEmpty(TARIF))
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Тариф(TARIF) обязателен к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "TARIF");
                }

                if (Cfile)
                {
                    if (DS_ONK == "0")
                    {
                        if (!DS1likeC && !DS1likeD00D09 && !DS1likeD70_C97C00_C80 && !DS1likeD45D47)
                            Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, $"Основной диагноз не C*/D00-D09{(DateFile < DT_04_2020? "/D70*":"")}{(DateFile >= DT_03_2021 ? "/D45-D47" : "")} для файла С при DS_ONK = 0", "DS1");
                    }
                    var needCONS = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47 || DS_ONK == "1";
                    if (needCONS && !cons)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, $"Сведения о проведении консилиума(CONS) обязательно к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "CONS");
                    if (!needCONS && cons)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, $"Сведения о проведении консилиума(CONS) не подлежат заполнению при DS1<C00 или D10<=DS1<D45 или DS1>=D48", "CONS");
                    var needONK_SL = (DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47) && usl_ok != "4" && reab != "1" && DS_ONK != "1";
                    if (needONK_SL && !onk_sl)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, $"Сведения о случае лечения онк.заболевания(ONK_SL) обязательно к заполнению при (C00.0<=DS1<D10 или D45<=DS1<D48) и USL_OK<>4 и REAB<>1 и DS_ONK=0", "ONK_SL");
                    if (!needONK_SL && onk_sl)
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, $"Сведения о случае лечения онк.заболевания(ONK_SL) не подлежат заполнению при DS1<C00 или D10<=DS1<D45 или DS1>=D48 или USL_OK=4 или REAB=1", "ONK_SL");
                    var needC_ZAB = (DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47) && usl_ok != "4";
                    if (needC_ZAB && string.IsNullOrEmpty(c_zab))
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, $"Характер основного заболевания(C_ZAB) обязателен к заполнению при (C00.0<=DS1<D10 или D45<=DS1<D48) и (USL_OK<>4)", "C_ZAB");
                    var needTARIF = DS1likeC || DS1likeD00D09 || DS1likeD70_C97C00_C80 || DS1likeD45D47;
                    if (needTARIF && string.IsNullOrEmpty(TARIF))
                        Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Тариф(TARIF) обязателен к заполнению при C00.0<=DS1<D10 или D45<=DS1<D48", "TARIF");
                }
            }

            if (Dfile && DateFile>=DT_04_2020)
            {
                if (P_OTK == "0" && string.IsNullOrEmpty(RSLT_D))
                    Error(XmlSeverityType.Error, P_OTK_POS.LINE, P_OTK_POS.POS, "Поле RSLT_D обязательно к заполнению при P_OTK = 0", "RSLT_D");
                if (P_OTK == "1" && !string.IsNullOrEmpty(RSLT_D))
                    Error(XmlSeverityType.Error, RSLT_D_POS.LINE, RSLT_D_POS.POS, "Поле RSLT_D не подлежит заполнению при P_OTK = 1", "CODE_MD");
                if(P_OTK == "0" && string.IsNullOrEmpty(DS1))
                    Error(XmlSeverityType.Error, P_OTK_POS.LINE, P_OTK_POS.POS, "Поле DS1 обязательно к заполнению при P_OTK = 0", "DS1");
                if (!string.IsNullOrEmpty(DS1) && string.IsNullOrEmpty(PR_D_N))
                    Error(XmlSeverityType.Error, DS1_POS.LINE, DS1_POS.POS, "Поле PR_D_N обязательно к заполнению при DS1 не пустое", "DS1");
            }
        }

        private DateTime DateFile = DateTime.Now.Date;
        private DateTime DT_04_2020 = new DateTime(2020, 04, 01);
        private DateTime DT_03_2021 = new DateTime(2021, 03, 01);

        private void SetDate()
        {
            if (!string.IsNullOrEmpty(YEAR) && !string.IsNullOrEmpty(MONTH))
                DateFile = new DateTime(Convert.ToInt32(YEAR), Convert.ToInt32(MONTH), 1);

        }


        public void Close()
        {
            if (SD_Z.HasValue)
            {
                if (SD_Z != SL_COUNT)
                {
                    Error(XmlSeverityType.Error, SD_Z_POS.LINE, SD_Z_POS.POS,$"Кол-во случаев в реестре {SL_COUNT}, однако SD_Z = {SD_Z.Value}", "SD_Z");
                }
            }

            if (SUMMAV.HasValue)
            {
                if (SUMMAV.Value != Math.Round(SUMV_SUM, 2))
                {
                    Error(XmlSeverityType.Error, SUMV_POS.LINE, SUMMAV_POS.POS,$"Сумма случаев в реестре {Math.Round(SUMV_SUM, 2)}, однако SUMMAV = {SUMMAV.Value}", "SUMMAV");
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
         
    }
}

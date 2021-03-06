using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF.SchemaEditor
{

    /// <summary>
    /// Тип элемента
    /// </summary>
    public enum TypeElement   //Тип элемента 
    {
        /// <summary>
        /// Обязательный
        /// </summary>
        O = 0,
        /// <summary>
        /// Обязательный множественый
        /// </summary>    
        OM = 1,
        /// <summary>
        /// Не обязательный множественый
        /// </summary>
        NM = 2,
        /// <summary>
        /// Не обязательный
        /// </summary>     
        N = 3,
        /// <summary>
        /// Условно-обязательный множественый
        /// </summary>
        YM = 4,
        /// <summary>
        /// Условно-обязательный
        /// </summary>
        Y = 5
    }
    //Классы типов данных
    /// <summary>
    /// Абстрактный тип данных в схеме
    /// </summary>
    [Serializable]
    public abstract class TypeS
    {
        public abstract string toSTR();
        public abstract string toSTRRUS();
    }
    /// <summary>
    /// Комплекстный тип данных (Содержит другие типы)
    /// </summary>
    [Serializable]
    public class TypeSComplex : TypeS
    {
        public override string toSTR()
        {
            return "S";
        }
        public override string toSTRRUS()
        {
            return "Комплексный тип";
        }
    }
    /// <summary>
    /// Тип данных число
    /// </summary>
    [Serializable]
    public class TypeSDigit : TypeS
    {
        /// <summary>
        /// Число знакомест
        /// </summary>
        public int ZnakMest { get; set; }
        /// <summary>
        /// В том числе после запятой
        /// </summary>
        public int ZnakMestPosDot { get; set; }
        /// <summary>
        /// Значения энумератора(Варианты)
        /// </summary>
        public List<int> Enum { get; set; }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_ZnakMest">Число знакомест</param>
        /// <param name="_ZnakMestPosDot">В том числе после запятой</param>
        /// <param name="_Enum">Значения энумератора(Варианты)</param>
        public TypeSDigit(int _ZnakMest, int _ZnakMestPosDot, List<int> _Enum)
        {
            ZnakMest = _ZnakMest;
            ZnakMestPosDot = _ZnakMestPosDot;
            Enum = new List<int>(_Enum);
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public TypeSDigit()
        {
            ZnakMest = 1;
            ZnakMestPosDot = 0;
            Enum = new List<int>();
        }

        public override string toSTR()
        {
            var result = ZnakMestPosDot == 0 ? $"N({ZnakMest})" : $"N({ZnakMest},{ZnakMestPosDot})";
            if (Enum.Count == 0) return result;
            result += $" {{{string.Join(",", Enum)}}}";
            return result;
        }
        public override string toSTRRUS()
        {
            var result = ZnakMestPosDot == 0 ? $"Число({ZnakMest} знаков)" : $"Число({ZnakMest} знаков,{ZnakMestPosDot} знаков после запятой)";
            if (Enum.Count == 0) return result;
            result += $" Значения{{{string.Join(",", Enum)}}}";
            return result;
        }
    }
    /// <summary>
    /// Тип данных строка
    /// </summary>
    [Serializable]
    public class TypeSString : TypeS
    {
        /// <summary>
        /// Количество символов
        /// </summary>
        public int ZnakMest { get; set; } = 1;
        /// <summary>
        /// Варианты значений
        /// </summary>
        public List<string> Enum { get; set; } = new List<string>();

        public TypeSString()
        {
            ZnakMest = 1;
        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_ZnakMest">Количество символов</param>
        /// <param name="_Enum">Варианты значений</param>
        public TypeSString(int _ZnakMest, List<string> _Enum)
        {
            ZnakMest = _ZnakMest;
            Enum = new List<string>(_Enum);
        }
        public override string toSTR()
        {
            var result = $"T({ZnakMest})";

            if (Enum.Count == 0) return result;
            result += $"{{{string.Join(",", Enum)}}}";
            return result;
        }
        public override string toSTRRUS()
        {
            var result = $"Текст ({ZnakMest} знаков)";
            if (Enum.Count == 0) return result;
            result += $" Значения {{{string.Join(",", Enum)}}}";
            return result;
        }
    }

    /// <summary>
    /// Тип данных дата
    /// </summary>
    [Serializable]
    public class TypeSDate : TypeS
    {
        public override string toSTR()
        {

            return "D";
        }
        public override string toSTRRUS()
        {
            return "Дата";
        }
    }


    /// <summary>
    /// Тип данных дата
    /// </summary>
    [Serializable]
    public class TypeSTime : TypeS
    {
        public override string toSTR()
        {
            return "Time";
        }
        public override string toSTRRUS()
        {
            return "Время";
        }
    }
    /// <summary>
    /// Элемент класса схема
    /// </summary>
    [XmlInclude(typeof(TypeSComplex)), XmlInclude(typeof(TypeSDigit)), XmlInclude(typeof(TypeSString)), XmlInclude(typeof(TypeSDate)), XmlInclude(typeof(TypeSTime))]
    public class SchemaElement
    {

        /// <summary>
        /// Наименование элемента
        /// </summary>

        public string name { get; set; }
        /// <summary>
        /// Тип элемента
        /// </summary>
      
        public TypeElement Type { get; set; }
        /// <summary>
        /// Индекс
        /// </summary>
      
        public bool Unique { get; set; }
        /// <summary>
        /// Уникальность по всему документу или в пределах родителя
        /// </summary>
      
        public bool UniqueGlobal { get; set; }

        private TypeS _format;
        /// <summary>
        /// Тип данных элемента
        /// </summary>
        public TypeS format
        {
            get => _format;
            set
            {
                _format = value;
                if (!(_format is TypeSComplex))
                {
                    Elements = null;
                }
                else
                {
                    if (Elements == null)
                        Elements = new List<SchemaElement>();
                }
            }
        }
        /// <summary>
        /// Вложеные элементы
        /// </summary>
        public List<SchemaElement> Elements { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void CopyFrom(SchemaElement item)
        {
            this.name = item.name;
            this.Type = item.Type;
            this.Unique = item.Unique;
            this.UniqueGlobal = item.UniqueGlobal;
            this.format = item.format;
            if (!(format is TypeSComplex))
                Elements = null;
        }
    }

    /// <summary>
    /// Схема XML
    /// </summary>
    public class XMLSchemaFile
    {

        /// <summary>
        /// Список элементов
        /// </summary>
        public List<SchemaElement> SchemaElements { get; set; }


        /// <summary>
        /// Схема
        /// </summary>
        private XmlSchema schema;

        public XMLSchemaFile()
        {
            SchemaElements = new List<SchemaElement>();
        }


        public List<SchemaElement> FindName(string Name)
        {
            return FindName(Name, SchemaElements);
        }


        private List<SchemaElement> FindName(string Name, List<SchemaElement> items)
        {
            var result = new List<SchemaElement>();
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (item.name?.ToUpper() == Name)
                    {
                        result.Add(item);
                    }
                    result.AddRange(FindName(Name, item.Elements));
                }
            }

            return result;
        }


        public void InsertAfter(SchemaElement from, SchemaElement item)
        {
            if (item.format is TypeSComplex)
            {
                if(item.Elements ==null)
                    item.Elements = new List<SchemaElement>();
            }
            else
            {
                item.Elements = null;
            }
         
            if (from==null)
                SchemaElements.Add(item);
            else
            {
                var path = FindPath(from);
                var parent = path[path.Count - 2];
                parent.Elements.Insert(parent.Elements.IndexOf(from)+1, item);
            }
        }

        public List<SchemaElement> FindPath(SchemaElement item)
        {
            var root = new SchemaElement() { Elements = SchemaElements };
            var parent = FindPath(root, item);
            return parent;
        }

        private List<SchemaElement> FindPath(SchemaElement root,SchemaElement item)
        {
            var result = new List<SchemaElement>();
            if (root.Elements != null)
            {
                foreach (var el in root.Elements)
                {
                    if (el == item)
                    {
                        result.Add(root);
                        result.Add(el);
                    }
                    else
                    {
                        var child = FindPath(el, item);
                        if (child.Count != 0)
                        {
                            result.Add(root);
                            result.AddRange(child);
                        }
                    }
                }
            }
            return result;
        }
        /// <summary>
        /// Выбор элемента
        /// </summary>
        /// <param name="index">Массив индексов. Как адрес элемента в дереве. Пример - ([1,1,5,6])</param>
        /// <returns>Элемент</returns>
        public SchemaElement this[int[] index]
        {
            get
            {

                var currentparent = SchemaElements;
                for (var i = 0; i < index.Length - 1; i++)
                {
                    currentparent = currentparent[index[i]].Elements;
                }
                if (currentparent == null) throw new Exception("NULL");
                return currentparent[index[index.Length - 1]];
            }
            set
            {
                var currentparent = SchemaElements;
                for (var i = 0; i < index.Length - 1; i++)
                {
                    currentparent = currentparent[index[i]].Elements;
                }
                if (currentparent == null) throw new Exception("NULL"); ;
                currentparent[index[index.Length - 1]] = value;

            }

        }
        /// <summary>
        /// количества корней (хоть он и 1 должен быть)
        /// </summary>
        public int CountRoot => SchemaElements.Count;

        /// <summary>
        /// Количество элементов у выбраного
        /// </summary>
        /// <param name="index">Массив индексов. Как адрес элемента в дереве. Пример - ([1,1,5,6])</param>
        /// <returns>количество</returns>
        public int Count(int[] index)
        {
            if (index.Length == 0) return CountRoot;
            var currentparent = index.Aggregate(SchemaElements, (current, t) => current[t].Elements);
            return currentparent.Count;

        }
        /// <summary>
        /// Сохранить в файл
        /// </summary>
        /// <param name="fileName">Путь</param>
        /// <returns>true - Сохранено false - Ошибка сохранения</returns>
        public bool SaveToFile(string fileName)
        {
            try
            {
                var writer = new XmlSerializer(typeof(List<SchemaElement>));
                var file = File.Create(fileName);
                writer.Serialize(file, SchemaElements);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            return true;

        }
        /// <summary>
        /// Загрузить из файла
        /// </summary>
        /// <param name="fileName">Путь</param>
        /// <returns>true Загружено false - ошибка загрузки</returns>
        public bool LoadFromFile(string fileName)
        {
            XmlSerializer writer;
            System.IO.FileStream file = null;
            try
            {
                writer = new XmlSerializer(typeof(List<SchemaElement>));
                file = System.IO.File.Open(fileName, System.IO.FileMode.Open);
                SchemaElements = (List<SchemaElement>)writer.Deserialize(file);
                file.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (file != null) file.Close();
                return false;
            }
            return true;
        }
        /// <summary>
        /// Очистка схемы
        /// </summary>
        public void Clear()
        {
            SchemaElements.Clear();
        }
 
        public void RemoveAt(SchemaElement item)
        {
            var path = FindPath(item);
            var parent = path[path.Count - 2];
            parent.Elements.Remove(item);
        }

        public bool ElementUp(SchemaElement item)
        {
            var path = FindPath(item);
            var parent = path[path.Count - 2];
            if (parent.Elements.IndexOf(item) != 0)
            {
                int indexCurr = parent.Elements.IndexOf(item);
                var back = parent.Elements[indexCurr - 1];
                parent.Elements[indexCurr - 1] = parent.Elements[indexCurr];
                parent.Elements[indexCurr] = back;
                return true;
            }

            return false;
        }

        public bool ElementDown(SchemaElement item)
        {
            var path = FindPath(item);
            var parent = path[path.Count - 2];
            if (parent.Elements.IndexOf(item) != parent.Elements.Count-1)
            {
                int indexCurr = parent.Elements.IndexOf(item);
                var back = parent.Elements[indexCurr + 1];
                parent.Elements[indexCurr + 1] = parent.Elements[indexCurr];
                parent.Elements[indexCurr] = back;
                return true;
            }
            return false;
        }
       

        /// <summary>
        /// Создание простого типа числа
        /// </summary>
        /// <param name="value">Тип данных число</param>
        /// <returns>Простой тип</returns>
        private XmlSchemaSimpleType CreateTypeDec(TypeSDigit value)
        {
            var type = new XmlSchemaSimpleType();
            
            //Создаем класс ограничений
            var restriction = new XmlSchemaSimpleTypeRestriction
            {
                BaseTypeName = new XmlQualifiedName("decimal", "http://www.w3.org/2001/XMLSchema")
            };
            //ограничение на количество символов
            var totalDigitsFacet = new XmlSchemaTotalDigitsFacet
            {
                Value = value.ZnakMest.ToString()
            };
            restriction.Facets.Add(totalDigitsFacet);
         
            var fractionDigitsFacet = new XmlSchemaFractionDigitsFacet
            {
                Value = value.ZnakMestPosDot.ToString()
            };
            restriction.Facets.Add(fractionDigitsFacet);

            var minInclusive = new XmlSchemaMinInclusiveFacet
            {
                Value = "0"
            };
            restriction.Facets.Add(minInclusive);

            var maxExclusive = new XmlSchemaMaxExclusiveFacet
            {
                Value = POV(value.ZnakMest - value.ZnakMestPosDot)
            };
            restriction.Facets.Add(maxExclusive);

            if (value.Enum.Count != 0)
            {
                foreach (var items in value.Enum)
                {
                    var enums = new XmlSchemaEnumerationFacet
                    {
                        Value = items.ToString()
                    };
                    restriction.Facets.Add(enums);
                }
            }
            type.Content = restriction;
            return type;
        }

        private string POV(int MEST)
        {
            var rzlt = "1";
            for (var i = 0; i < MEST; i++)
            {
                rzlt += "0";
            }
            return rzlt;
        }

        /// <summary>
        /// Создание простого типа строки
        /// </summary>
        /// <param name="value">Тип данных строка</param>
        /// <returns>Простой тип строки</returns>
        private XmlSchemaSimpleType CreateTypeStr(TypeSString value, TypeElement TypeEl)
        {
            var type = new XmlSchemaSimpleType();
            //Создаем класс ограничений
            var restriction = new XmlSchemaSimpleTypeRestriction
            {
                BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
            };
            var maxLengthFacet = new XmlSchemaMaxLengthFacet
            {
                Value = value.ZnakMest.ToString()
            };
            restriction.Facets.Add(maxLengthFacet);


            var minLengthFacet = new XmlSchemaMinLengthFacet();
            if (TypeEl == TypeElement.O || TypeEl == TypeElement.OM)
                minLengthFacet.Value = "1";
            else
                minLengthFacet.Value = "0";
            restriction.Facets.Add(minLengthFacet);

            if (value.Enum.Count != 0)
            {
                foreach (var item in value.Enum)
                {
                    var enums = new XmlSchemaEnumerationFacet
                    {
                        Value = item.Trim()
                    };
                    restriction.Facets.Add(enums);
                }
            }
            type.Content = restriction;
            return type;
        }

        /// <summary>
        /// Создание простого типа даты
        /// </summary>
        /// <param name="value">Тип данных дата</param>
        /// <returns>Простой тип данных даты</returns>
        private XmlSchemaSimpleType CreateTypeDate(TypeSDate value)
        {
            var type = new XmlSchemaSimpleType();
            var restriction = new XmlSchemaSimpleTypeRestriction
            {
                BaseTypeName = new XmlQualifiedName("date", "http://www.w3.org/2001/XMLSchema")
            };
            //  <xsd:restriction base="xsd:date">
            //  <xsd:pattern value="((000[1-9])|(00[1-9][0-9])|(0[1-9][0-9]{2})|([1-9][0-9]{3}))-((0[1-9])|(1[012]))-((0[1-9])|([12][0-9])|(3[01]))" /> 
            //  <xsd:maxInclusive value="9999-12-31" /> 
            //  <xsd:minInclusive value="0001-01-01" /> 
            // </xsd:restriction>
            var pat = new XmlSchemaPatternFacet
            {
                Value = "((000[1-9])|(00[1-9][0-9])|(0[1-9][0-9]{2})|([1-9][0-9]{3}))-((0[1-9])|(1[012]))-((0[1-9])|([12][0-9])|(3[01]))"
            };
            var max = new XmlSchemaMaxExclusiveFacet { Value = "2030-12-31" };
            var min = new XmlSchemaMinExclusiveFacet { Value = "1899-12-31" };
            restriction.Facets.Add(pat);
            restriction.Facets.Add(max);
            restriction.Facets.Add(min);
            type.Content = restriction;
            return type;
        }

        private XmlSchemaSimpleType CreateTypeTime(TypeSTime value)
        {
            var type = new XmlSchemaSimpleType();
            var restriction = new XmlSchemaSimpleTypeRestriction
            {
                BaseTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema")
            };
            //  <xsd:restriction base="xsd:date">
            //  <xsd:pattern value="((000[1-9])|(00[1-9][0-9])|(0[1-9][0-9]{2})|([1-9][0-9]{3}))-((0[1-9])|(1[012]))-((0[1-9])|([12][0-9])|(3[01]))" /> 
            //  <xsd:maxInclusive value="9999-12-31" /> 
            //  <xsd:minInclusive value="0001-01-01" /> 
            // </xsd:restriction>
            var pat = new XmlSchemaPatternFacet { Value = "^([0-1]?[0-9]|2[0-3]):[0-5][0-9]:[0-5][0-9]$" };
            restriction.Facets.Add(pat);
            type.Content = restriction;
            return type;
        }


        private XmlSchemaComplexType CreateXmlSchemaComplexType(SchemaElement se, int index,string targetNamespace)
        {
            //комплекстный тип
            var complexType = new XmlSchemaComplexType();
            //Симплконтент
            var con = new XmlSchemaSimpleContent();

            //2 атрибута 
            var d = new XmlDocument();
            var att1 = d.CreateAttribute("ColumnName", "urn:schemas-microsoft-com:xml-msdata");
            att1.Value = se.name;
            var att2 = d.CreateAttribute("Ordinal", "urn:schemas-microsoft-com:xml-msdata");
            att2.Value = "0";
            var att3 = d.CreateAttribute("Orders", "urn:schemas-microsoft-com:xml-msprop");
            att3.Value = index.ToString();
            var xmlatt = new List<XmlAttribute>{ att1 , att2 };
            if (se.Type == TypeElement.YM || se.Type == TypeElement.NM || se.Type == TypeElement.OM)
                xmlatt.Add(att3);
            con.UnhandledAttributes = xmlatt.ToArray();


            var simpleContentExtension = new XmlSchemaSimpleContentExtension
            {
                BaseTypeName = new XmlQualifiedName(se.name, targetNamespace)
            };

            con.Content = simpleContentExtension;
            complexType.ContentModel = con;
            return complexType;
        }
        /// <summary>
        /// Создание элемента схемы
        /// </summary>
        /// <param name="se">Элемент класса</param>
        /// <returns>Элемент схемы</returns>
        private XmlSchemaElement CreateSchemaElement(SchemaElement se, int index,string targetNamespace)
        {
            var element = new XmlSchemaElement();
          
            XmlDocument d;
            XmlAttribute[] xmlatt;
            switch (se.Type)
            {
                case TypeElement.O:
                    element.MinOccurs = 1;
                    break;
                case TypeElement.OM:

                    element.MinOccurs = 1;
                    element.MaxOccursString = "unbounded";
                    break;
                case TypeElement.Y:
                case TypeElement.N:
                    element.MinOccurs = 0;
                    element.IsNillable = true;
                    break;
                case TypeElement.NM:
                case TypeElement.YM:
                    element.MinOccurs = 0;
                    element.MaxOccursString = "unbounded";
                    element.IsNillable = true;
                    break;
                default:
                    break;

            }


            element.Name = se.name;

            if (se.format is TypeSTime)
            {
                if (se.Type == TypeElement.NM || se.Type == TypeElement.OM || se.Type == TypeElement.YM)
                {
                    element.SchemaType = CreateXmlSchemaComplexType(se, index, targetNamespace);
                    var type = CreateTypeTime(se.format as TypeSTime);
                    type.Name = se.name;
                    schema.Items.Add(type);
                }
                else
                {
                    element.SchemaType = CreateTypeTime(se.format as TypeSTime);
                }
            }


            if (se.format is TypeSDate)
            {
                if (se.Type == TypeElement.NM || se.Type == TypeElement.OM || se.Type == TypeElement.YM)
                {
                    element.SchemaType = CreateXmlSchemaComplexType(se, index, targetNamespace);
                    var type = CreateTypeDate(se.format as TypeSDate);
                    type.Name = se.name;
                    schema.Items.Add(type);
                }
                else
                {
                    element.SchemaType = CreateTypeDate(se.format as TypeSDate);
                }
            }

            if (se.format is TypeSDigit)
            {
                if (se.Type == TypeElement.NM || se.Type == TypeElement.OM || se.Type == TypeElement.YM)
                {
                    element.SchemaType = CreateXmlSchemaComplexType(se, index, targetNamespace);
                    var type = CreateTypeDec(se.format as TypeSDigit);
                    type.Name = se.name;
                    schema.Items.Add(type);
                }
                else
                {
                    element.SchemaType = CreateTypeDec(se.format as TypeSDigit);
                }
            }

            if (se.format is TypeSString)
                if (se.Type == TypeElement.NM || se.Type == TypeElement.OM || se.Type == TypeElement.YM)
                {
                    element.SchemaType = CreateXmlSchemaComplexType(se, index, targetNamespace);
                    var type = CreateTypeStr(se.format as TypeSString, se.Type);
                    type.Name = se.name;
                    schema.Items.Add(type);
                }
                else
                {
                    element.SchemaType = CreateTypeStr(se.format as TypeSString, se.Type);
                }


            if (se.format is TypeSComplex)
            {
                element.SchemaType = CreateComplexType(se.Elements, targetNamespace);
                d = new XmlDocument();
                var att3 = d.CreateAttribute("Orders", "urn:schemas-microsoft-com:xml-msprop");

                att3.Value = index.ToString();
                xmlatt = new XmlAttribute[1];
                xmlatt.SetValue(att3, 0);
                element.UnhandledAttributes = xmlatt;

                foreach (var itemM in se.Elements)
                {
                    if (itemM.format is TypeSComplex)
                    {
                        foreach (var item in itemM.Elements)
                        {
                            if (item.format is TypeSString || item.format is TypeSDigit)
                            {
                                if (item.Unique && !item.UniqueGlobal)
                                {
                                    var un = new XmlSchemaUnique
                                    {
                                        Name = item.name,
                                        Selector = new XmlSchemaXPath { XPath = itemM.name }
                                    };
                                    un.Fields.Add(new XmlSchemaXPath { XPath = item.name });
                                    element.Constraints.Add(un);
                                }

                            }
                        }
                    }
                }
            }

            return element;

        }

        /// <summary>
        /// Создание комплексного типа
        /// </summary>
        /// <param name="elements">Дочерние элементы</param>
        /// <param name="targetNamespace"></param>
        /// <returns>Комплексный тип</returns>
        private XmlSchemaComplexType CreateComplexType(List<SchemaElement> elements,string targetNamespace)
        {
            var schemaComplexType = new XmlSchemaComplexType();
          
            var sequence = new XmlSchemaSequence();
          
            var index_ = 0;
            foreach (var item in elements)
            {
                sequence.Items.Add(CreateSchemaElement(item, index_, targetNamespace));
                if (!(item.Type == TypeElement.NM || item.Type == TypeElement.YM || item.Type == TypeElement.OM || item.format is TypeSComplex))
                    index_++;
            }
            schemaComplexType.Particle = sequence;
            
            return schemaComplexType;
        }

        class Unic
        {
            public string Selector { get; set; }
            public string Field { get; set; }
        }

        List<Unic> GetGlobalIndex()
        {
            var un = new List<Unic>();
            foreach (var item in SchemaElements)
            {
                if (item.format is TypeSComplex)
                {
                    un.InsertRange(0, GetGlobalIndex("", item.Elements));
                }
                if (item.UniqueGlobal && item.Unique)
                {
                    un.Add(new Unic() { Selector = "", Field = item.name });
                }

            }
            return un;
        }
        List<Unic> GetGlobalIndex(string Selector, List<SchemaElement> list)
        {
            var un = new List<Unic>();
            foreach (var item in list)
            {
                if (item.format is TypeSComplex)
                {
                    if (Selector == "")
                        un.InsertRange(0, GetGlobalIndex(item.name, item.Elements));
                    else
                        un.InsertRange(0, GetGlobalIndex(Selector + "/" + item.name, item.Elements));
                }
                if (item.UniqueGlobal && item.Unique)
                {
                    un.Add(new Unic() { Selector = Selector, Field = item.name });
                }

            }
            return un;
        }



        /// <summary>
        /// Компиляция схемы
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>true - компиляция удачна false - ошибка компиляции</returns>
        public bool Compile(string path, string targetNamespace)
        {
            targetNamespace = targetNamespace == "" ? null : targetNamespace;
            schema = new XmlSchema();
            schema.Namespaces.Add("msdata", "urn:schemas-microsoft-com:xml-msdata");
            schema.Namespaces.Add("msprop", "urn:schemas-microsoft-com:xml-msprop");
            schema.TargetNamespace = targetNamespace;
            schema.ElementFormDefault = XmlSchemaForm.Qualified;
            //Компиляция элементов
            var index = 0;
            foreach (var item in SchemaElements)
            {
                var xml1 = new XmlSchemaElement();
                var xml = CreateSchemaElement(item, index, targetNamespace);
                index++;

                xml1.Name = xml.Name;
                xml1.SchemaType = xml.SchemaType;
                
             

                var t = GetGlobalIndex();
                foreach (var u in t)
                {
                    var un = new XmlSchemaUnique { Name = u.Field, Selector = new XmlSchemaXPath() { XPath = u.Selector } };
                    
                    un.Fields.Add(new XmlSchemaXPath() { XPath = u.Field });
                    xml1.Constraints.Add(un);
                }

                schema.Items.Add(xml1);
            }
            //Компиляция файла
            compileEr = true;

            var set = new XmlSchemaSet();
            set.Add(schema);
            
            set.ValidationEventHandler += ValidationCallback;
            set.Compile();
            //schema.Compile(ValidationCallback);
            var file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);

            var xwriter = new XmlTextWriter(file, new UTF8Encoding());
            xwriter.Formatting = Formatting.Indented;
            schema.Write(xwriter);
            file.Close();
            //////////////////////////////////////////////
            return compileEr;

        }
        bool compileEr = true;
        /// <summary>
        /// Событие в случае ошибки в схеме
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Args</param>
        private void ValidationCallback(object sender, ValidationEventArgs args)
        {
            switch (args.Severity)
            {
                case XmlSeverityType.Warning:
                    MessageBox.Show($@"WARNING:{args.Message} {args.Exception.Source}");
                    break;
                case XmlSeverityType.Error:
                    MessageBox.Show($@"ERROR:{args.Message} {args.Exception.Source}");
                    compileEr = false;
                    break;
            }
        }


    }

    public static class Ext
    {
        public static  string toRusName(this TypeElement te)
        {
            switch (te)
            {
                case TypeElement.O:
                    return "Обязательный";
                case TypeElement.OM:
                    return "Обязательный множественный";
                case TypeElement.NM:
                    return "Не обязательный множественный";
                case TypeElement.N:
                    return "Не обязательный";
                case TypeElement.YM:
                    return "Условно - обязательный множественный";
                case TypeElement.Y:
                    return "Условно - обязательный";
                default:
                    throw new ArgumentOutOfRangeException(nameof(te), te, null);
            }
        }
    }


}

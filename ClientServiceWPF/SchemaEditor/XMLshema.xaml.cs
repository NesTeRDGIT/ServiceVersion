using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using System.Windows.Input;

using System.Xml;
using System.Xml.Serialization;


using ServiceLoaderMedpomData.Annotations;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using  ClientServiceWPF.Class;

namespace ClientServiceWPF.SchemaEditor
{
    /// <summary>
    /// Логика взаимодействия для XMLshema.xaml
    /// </summary>
    public partial class XMLshema : Window
    {
        private XMLSchemaVM VM;
        public XMLshema()
        {
            InitializeComponent();
            VM = (XMLSchemaVM)FindResource("VM");

        }
        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ListBoxEnumDigit_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.SelectedDigitEnum = ListBoxEnumDigit.SelectedItems.Cast<int>().ToList();
        }


        private void ListBoxStringEnum_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.SelectedStringEnum = ListBoxStringEnum.SelectedItems.Cast<string>().ToList();
        }
    }


    public class XMLSchemaVM:INotifyPropertyChanged
    {
        private XMLSchemaFile XMLSchema { get; set; }= new XMLSchemaFile();

        string _StatusOperation;

        public string StatusOperation
        {
            get { return _StatusOperation; }
            set { _StatusOperation = value; OnPropertyChanged();}
        }

        private void SetTimeStatusOperation(string value,int MS)
        {
            StatusOperation = value;
            Task.Run(() =>
            {
                var t = Task.Delay(MS);
                t.Wait();
                _StatusOperation = "";
                OnPropertyChanged(nameof(StatusOperation));
            });
        }

        public class SPRRow
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }
      

        public List<SPRRow> TypeElementSPR 
        {
            get
            {
                var SPR = new List<SPRRow>();
                foreach (var te in (TypeElement[])Enum.GetValues(typeof(TypeElement)))
                {
                    SPR.Add(new SPRRow() { Name = te.toRusName(), Value = te });
                }
                return SPR;
            }
        }

        public List<SPRRow> formatEnumSPR
        {
            get
            {
                var SPR = new List<SPRRow>();
                foreach (var te in (formatEnum[])Enum.GetValues(typeof(formatEnum)))
                {
                    SPR.Add(new SPRRow() { Name = te.toRusName(), Value = te });
                }
                return SPR;
            }
        }

        public ObservableCollection<SchemaElementVM> Elements { get; private set ; } = new ObservableCollection<SchemaElementVM>();

        private void RefreshElements()
        {
            Elements.Clear();
            foreach (var item in XMLSchema.SchemaElements.Select(x => CreateSchemaElementVM(x, SchemaElementVM_Propertyselect, IsSelectedList, IsExpandedList)))
            {
                Elements.Add(item);
            }
        }


        private SchemaElementVM CreateSchemaElementVM(SchemaElement sc, PropertyChangedEventHandler callback, List<SchemaElement> IsSelected, List<SchemaElement> IsExpanded)
        {
            var item = new SchemaElementVM(sc);
           
            if (IsSelected.Contains(sc))
                item.IsSelected = true;
            if (IsExpanded.Contains(sc))
                item.IsExpanded = true;
            if (sc.Elements != null)
            {
                item.Elements = new ObservableCollection<SchemaElementVM>( sc.Elements.Select(x=> CreateSchemaElementVM(x, callback, IsSelected, IsExpanded)));
            }
            item.PropertyChanged += callback;
            return item;
        }

        List<SchemaElement> IsExpandedList = new List<SchemaElement>();
        List<SchemaElement> IsSelectedList = new List<SchemaElement>();

        private void SchemaElementVM_Propertyselect(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as SchemaElementVM;
            if (e.PropertyName == nameof(SchemaElementVM.IsSelected) && item!=null)
            {
                if (item.IsSelected)
                    CurrentElement = item;

                if (item.IsSelected)
                {
                    if (!IsSelectedList.Contains(item.baseElement))
                        IsSelectedList.Add(item.baseElement);
                }
                else
                {
                    if (IsSelectedList.Contains(item.baseElement))
                        IsSelectedList.Remove(item.baseElement);
                }

            }
            if (e.PropertyName == nameof(SchemaElementVM.IsExpanded) && item != null)
            {
                if (item.IsExpanded)
                {
                    if(!IsExpandedList.Contains(item.baseElement))
                        IsExpandedList.Add(item.baseElement);
                }
                else
                {
                    if (IsExpandedList.Contains(item.baseElement))
                        IsExpandedList.Remove(item.baseElement);
                }
            }
        }


        private string _CurrentPath = "";
        public string CurrentPath
        {
            get { return _CurrentPath; }
            set { _CurrentPath = value;OnPropertyChanged(); }
        }

        public ICommand Save => new Command(o =>
        {
            if (string.IsNullOrEmpty(CurrentPath))
            {
                SaveAs.Execute(null);
            }
            else
            {
                XMLSchema.SaveToFile(CurrentPath);
                SetTimeStatusOperation("Сохранено", 5000);
            }
        });
        public ICommand SaveAs => new Command(o =>
        { 
            var sfd = new SaveFileDialog() { Filter = "Файлы проекта схемы(*.pxsd)|*.pxsd" };
            if (sfd.ShowDialog() == true)
            {
                XMLSchema.SaveToFile(sfd.FileName);
                CurrentPath = sfd.FileName;
                SetTimeStatusOperation("Сохранено", 5000);
            }
        });
        public ICommand Open => new Command(o =>
        {
            var ofd = new OpenFileDialog() { Filter = "Файлы проекта схемы(*.pxsd)|*.pxsd" };
            if (ofd.ShowDialog() == true)
            {
                XMLSchema.LoadFromFile(ofd.FileName);
                CurrentPath = ofd.FileName;
                RefreshElements();
                OnPropertyChanged(nameof(Elements));
            }
        });
        public ICommand Compile => new Command(o =>
        {
            try
            {
                var sfd = new SaveFileDialog {Filter = "Файлы схемы(*.xsd)|*.xsd", FileName = !string.IsNullOrEmpty(CurrentPath) ? System.IO.Path.GetFileNameWithoutExtension(CurrentPath) : ""};
                if (sfd.ShowDialog() == true)
                    if (XMLSchema.Compile(sfd.FileName))
                    {
                        MessageBox.Show(@"Создание схемы успешно!");
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand New => new Command(o =>
        {
            CurrentPath = "";
            XMLSchema.Clear();
            RefreshElements();
            OnPropertyChanged(nameof(Elements));
        });
        public ICommand AddDigitEnumCommand => new Command(o =>
        {
            try
            {
                CurrentElement.AddEnumDigit(Convert.ToInt32(o));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        });
        public ICommand AddStringEnumCommand => new Command(o =>
        {
            try
            {
                CurrentElement.AddEnumString(o.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public IList<int> SelectedDigitEnum { get; set; }
        public ICommand RemoveDigitEnumCommand => new Command(o =>
        {
            try
            {
                CurrentElement.RemoveEnumDigit(SelectedDigitEnum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public IList<string> SelectedStringEnum { get; set; }
        public ICommand RemoveStringEnumCommand => new Command(o =>
        {
            try
            {
                CurrentElement.RemoveEnumString(SelectedStringEnum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand CommitCommand => new Command(o =>
        {
            try
            {
                CurrentElement.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand MoveUpCommand => new Command(o =>
        {
            try
            {
                var curr = CurrentElement;
                XMLSchema.ElementUp(curr.baseElement);
                RefreshElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand MoveDownCommand => new Command(o =>
        {
            try
            {
                XMLSchema.ElementDown(CurrentElement.baseElement);
                RefreshElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand RefreshCommand => new Command(o =>
        {
            try
            {
                RefreshElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand DeleteCommand => new Command(o =>
        {
            try
            {
                XMLSchema.RemoveAt(CurrentElement.baseElement);
                RefreshElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ClearCommand => new Command(o =>
        {
            try
            {
                XMLSchema.Clear();
                Elements.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand AddNodeCommand => new Command(o =>
        {
            try
            {
                var curr = CurrentElement;
                var newElement = new SchemaElement() {name = "Новый узел"};
                if(curr!=null)
                    newElement.CopyFrom(curr.baseElement);
                XMLSchema.InsertAfter(curr?.baseElement, newElement);
                RefreshElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand AddChildCommand => new Command(o =>
        {
            try
            {
                var curr = CurrentElement;
                var newElement = new SchemaElement() { name = "Новый узел", Type = CurrentElement.Type, format = CurrentElement.format};
                if (curr?.format is TypeSComplex)
                    curr.baseElement.Elements.Add(newElement);
                RefreshElements();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand CopyCommand => new Command(o =>
        {
            try
            {
                var current = CurrentElement;
                if (current != null)
                    Clipboard.SetText(ToString(current.baseElement));
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        });
        public ICommand PasteCommand => new Command(o =>
        {
            try
            {
                var current = CurrentElement;
                var xml = Clipboard.GetText();
                var newItem = FromString(xml);
                if (current != null && newItem != null)
                    XMLSchema.InsertAfter(current.baseElement, newItem);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            finally
            {
                RefreshElements();
            }
        });
        string ToString(SchemaElement sc)
        {
            using (var ms = new MemoryStream())
            {
                var ser = new XmlSerializer(typeof(SchemaElement));
                ser.Serialize(ms, sc);
                ms.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(ms))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        SchemaElement FromString(string str)
        {
            using (var sr = new StringReader(str))
            {
                using (var xml = XmlReader.Create(sr))
                {
                    var ser = new XmlSerializer(typeof(SchemaElement));
                    return (SchemaElement)ser.Deserialize(xml);
                }
            }

        }




        class FindPredicat<T>
        {
            public string FindText { get; set; }
            public List<T> FindNODE { get; set; } = new List<T>();
            private int CurrItem { get; set; }

            public void Clear()
            {
                FindText = null;
                FindNODE.Clear();
                CurrItem = -1;
            }

            public T GetNEXT()
            {
                CurrItem++;
                if (CurrItem < FindNODE.Count) return FindNODE[CurrItem];
                return default(T);
            }

        }

        FindPredicat<SchemaElementVM> FindP = new FindPredicat<SchemaElementVM>();

        public ICommand FindNodeCommand => new Command(o =>
        {
            try
            {
                var findstr = o.ToString();
                if (!string.IsNullOrEmpty(findstr))
                {

                    if (FindP.FindText != findstr)
                    {
                        FindP.Clear();
                        FindP.FindText = findstr;
                        var items = FindName(findstr, Elements);
                        foreach (var item in items)
                        {
                            FindP.FindNODE.Add(item);
                        }
                    }

                    var nod = FindP.GetNEXT();
                    if (nod != null)
                    {
                        ShowElement(nod);
                    }
                    else
                    {
                        MessageBox.Show(@"Поиск достиг конца списка");
                        FindP.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                FindP.Clear();
            }
        });

        private SchemaElementVM FindParent(SchemaElementVM item, IEnumerable<SchemaElementVM> source)
        {
            foreach (var s_item in source)
            {
                if (s_item.Elements.Contains(item))
                    return s_item;
                var t = FindParent(item, s_item.Elements);
                if (t != null)
                    return t;
            }
            return null;

        }


        private void ShowElement(SchemaElementVM item)
        {
            var parent = item;
            do
            {
                parent = FindParent(parent, Elements);
                if (parent != null)
                {
                    parent.IsExpanded = true;
                }
            }
            while (parent != null);
            item.IsSelected = true;
        }

        private List<SchemaElementVM> FindName(string Name, IEnumerable<SchemaElementVM> items)
        {
            var result = new List<SchemaElementVM>();
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

       




          

        SchemaElementVM _CurrentElement;

        public SchemaElementVM CurrentElement
        {
            get { return _CurrentElement;}
            set { _CurrentElement?.Rollback(); _CurrentElement = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public enum formatEnum
    {
        TypeSString,
        TypeSComplex,
        TypeSDigit,
        TypeSDate,
        TypeSTime
    }

    public static class ExtSchema
    {
        public static string toRusName(this formatEnum fe)
        {
            switch (fe)
            {
                case formatEnum.TypeSString:
                    return "Строка";
                case formatEnum.TypeSComplex:
                    return "Комплексный";
                case formatEnum.TypeSDigit:
                    return "Число";
                case formatEnum.TypeSDate:
                    return "Дата(ГГГГ-ММ-ДД)";
                case formatEnum.TypeSTime:
                    return "Время(HH: MM:SS)";
                default:
                    throw new ArgumentOutOfRangeException(nameof(fe), fe, null);
            }
        }
    }

    public class SchemaElementVM:INotifyPropertyChanged
    {
        public SchemaElement baseElement { get; set; }
        public SchemaElementVM(SchemaElement item)
        {
            baseElement = item;
            Rollback();
            RefreshEnums();
            
        }

        private void RefreshEnums()
        {
            EnumDigit.Clear();
            if (TypeSDigit != null)
                foreach (var e in TypeSDigit.Enum)
                {
                    EnumDigit.Add(e);
                }

            EnumString.Clear();
            if (TypeSString != null)
                foreach (var e in TypeSString.Enum)
                {
                    EnumString.Add(e);
                }
        }

        public TypeElement Type => baseElement.Type;
        public string name => baseElement.name;
        public TypeS format => baseElement.format;
        public bool Unique => baseElement.Unique;
        public bool UniqueGlobal => baseElement.UniqueGlobal;
        public string formatStr => baseElement.format?.toSTRRUS();


        public bool HasChild => baseElement.format is TypeSComplex;


        public TypeSString TypeSString => format as TypeSString;
        public TypeSDigit TypeSDigit => format as TypeSDigit;


        public ObservableCollection<int> EnumDigit { get; set; } = new ObservableCollection<int>();

        public ObservableCollection<string> EnumString{ get; set; } = new ObservableCollection<string>();


        public void AddEnumDigit(int value)
        {
            if (TypeSDigit != null)
            {
                EnumDigit.Add(value);
            }
        }

        public void AddEnumString(string value)
        {
            if (TypeSString != null)
            {
                EnumString.Add(value);
            }
        }


        public void RemoveEnumDigit(IEnumerable<int> value)
        {
            if (TypeSDigit != null)
            {
                foreach (var val in value)
                {
                    EnumDigit.Remove(val);
                }
            }
        }

        public void RemoveEnumString(IEnumerable<string> value)
        {
            if (TypeSString != null)
            {
                foreach (var val in value)
                {
                    EnumString.Remove(val);
                }
            }
        }


        private bool _IsExpanded;
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set { _IsExpanded = value; OnPropertyChanged(); }
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected;}
            set { _IsSelected = value;OnPropertyChanged(); }
        }
        public ObservableCollection<SchemaElementVM> Elements { get; set; } = new ObservableCollection<SchemaElementVM>();
        private TypeElement _TypeEdit;
        public  TypeElement TypeEdit
        {
            get { return _TypeEdit; }
            set { _TypeEdit = value;OnPropertyChanged(); }
        }
        private string _nameEdit;
        public  string nameEdit
        {
            get { return _nameEdit; }
            set { _nameEdit = value; OnPropertyChanged(); }
        }
        private TypeS _formatEdit;
        public  TypeS formatEdit
        {
            get { return _formatEdit; }
            set { _formatEdit = value; OnPropertyChanged();OnPropertyChanged("format_type"); }
        }
        public formatEnum format_type
        {
            get
            {
                if (formatEdit is TypeSString)
                    return formatEnum.TypeSString;
                if (formatEdit is TypeSComplex)
                    return formatEnum.TypeSComplex;
                if (formatEdit is TypeSDigit)
                    return formatEnum.TypeSDigit;
                if (formatEdit is TypeSDate)
                    return formatEnum.TypeSDate;
                if (formatEdit is TypeSTime)
                    return formatEnum.TypeSTime;
                throw new Exception("Error format_type");
            }
            set
            {
                switch (value)
                {
                    case formatEnum.TypeSString:
                        formatEdit = new TypeSString();
                        break;
                    case formatEnum.TypeSComplex:
                        formatEdit = new TypeSString();
                        break;
                    case formatEnum.TypeSDigit:
                        formatEdit = new TypeSDigit();
                        break;
                    case formatEnum.TypeSDate:
                        formatEdit = new TypeSDate();
                        break;
                    case formatEnum.TypeSTime:
                        formatEdit = new TypeSTime();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }
        private bool _UniqueEdit;
        public  bool UniqueEdit
        {
            get { return _UniqueEdit; }
            set { _UniqueEdit = value; OnPropertyChanged(); }
        }
        private bool _UniqueGlobalEdit;
        public  bool UniqueGlobalEdit
        {
            get { return _UniqueGlobalEdit; }
            set { _UniqueGlobalEdit = value; OnPropertyChanged(); }
        }

        public void Commit()
        {
            baseElement.Type = TypeEdit;
            baseElement.name = nameEdit;
            baseElement.format = formatEdit;
            baseElement.Unique = UniqueEdit;
            baseElement.UniqueGlobal = UniqueGlobalEdit;
            if (TypeSString != null)
            {
                TypeSString.Enum.Clear();
                TypeSString.Enum.AddRange(EnumString);
            }
            if (TypeSDigit != null)
            {
                TypeSDigit.Enum.Clear();
                TypeSDigit.Enum.AddRange(EnumDigit);
            }

            OnPropertyChanged(nameof(Type));
            OnPropertyChanged(nameof(name));
            OnPropertyChanged(nameof(format));
            OnPropertyChanged(nameof(Unique));
            OnPropertyChanged(nameof(UniqueGlobal));



        }
        public void Rollback()
        {
            TypeEdit = baseElement.Type;
            nameEdit = baseElement.name;
            formatEdit = baseElement.format;
            UniqueEdit = baseElement.Unique;
            UniqueGlobalEdit = baseElement.UniqueGlobal;
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

   


    public class ConvectorTypeEnum : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TypeSComplex)
                return TypeEnum.COMPLEX;
            if (value is TypeSDigit)
                return TypeEnum.NUMBER;
            if (value is TypeSString)
                return TypeEnum.STRING;
            if (value is TypeSDate)
                return TypeEnum.DATE;
            if (value is TypeSTime)
                return TypeEnum.DATETIME;
            return TypeEnum.NONE;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ConvectorEnum : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valD = value as TypeSDigit;
            if (valD != null)
            {
                if (valD.Enum == null) return "";
                return valD.Enum.Count != 0 ? $"{{{string.Join(",", valD.Enum)}}}" : "";
            }


            var valS = value as TypeSString;
            if (valS != null)
            {
                if (valS.Enum == null) return "";
                return valS.Enum.Count != 0 ? $"{{{string.Join(",", valS.Enum)}}}" : "";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public enum TypeEnum
    {
        NONE,
        NUMBER,
        STRING,
        COMPLEX,
        DATE,
        DATETIME
    }


}

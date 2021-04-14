using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF.SchemaEditor
{
    /// <summary>
    /// Логика взаимодействия для XMLshema.xaml
    /// </summary>
    public partial class XMLshema : Window, INotifyPropertyChanged
    {
        private XMLSchemaFile XMLshemaMy = new XMLSchemaFile();
        private CollectionViewSource CVSSchema;
        public List<SchemaElement> CurrentElements => XMLshemaMy.SchemaElements;
        public XMLshema()
        {
            InitializeComponent();
            CVSSchema = (CollectionViewSource) FindResource("CVSSchema");
        }

        public string CurrentPath
        {
            get { return TextBoxPath.Text;}
            set { TextBoxPath.Text = value; }
        }



        private void MenuItemNew_OnClick(object sender, RoutedEventArgs e)
        {
            TextBoxPath.Text = "";
        }

        private OpenFileDialog ofd = new OpenFileDialog() {Filter = "Файлы проекта схемы(*.pxsd)|*.pxsd"};
        private SaveFileDialog sfd = new SaveFileDialog() {Filter = "Файлы проекта схемы(*.pxsd)|*.pxsd"};
        private SaveFileDialog sfd_XSD = new SaveFileDialog() { Filter = "Файлы схемы(*.xsd)|*.xsd" };
        private void MenuItemOpen_OnClick(object sender, RoutedEventArgs e)
        {
            if (ofd.ShowDialog() == true)
            {
                XMLshemaMy.LoadFromFile(ofd.FileName);
                CurrentPath = ofd.FileName;
                OnPropertyChanged("CurrentElements");
                //RefreshTreeView();
            }
        }

        private void MenuItemSave_OnClick(object sender, RoutedEventArgs e)
        {
            if (TextBoxPath.Text == "")
            {
                if (sfd.ShowDialog() == true)
                {

                    XMLshemaMy.SaveToFile(sfd.FileName);
                    CurrentPath = sfd.FileName;
                }
            }
            else
            {
                XMLshemaMy.SaveToFile(TextBoxPath.Text);
            }
        }

        private void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
        {
            if (sfd.ShowDialog() == true)
            {
                XMLshemaMy.SaveToFile(sfd.FileName);
                CurrentPath = sfd.FileName;
            }
        }

        private void MenuItemCompile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                sfd_XSD.FileName = System.IO.Path.GetFileNameWithoutExtension(CurrentPath);
                if (sfd_XSD.ShowDialog() == true)
                    if (XMLshemaMy.Compile(sfd_XSD.FileName))
                    {
                        MessageBox.Show(@"Создание схемы успешно!");
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
         
        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void buttonNewNode_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxName.Text))
                {
                    MessageBox.Show("Имя не может быть пустым");
                    return;
                }
                var current = currentElement;
                XMLshemaMy.InsertAfter(current, GetElement());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateTrieView();
            }
        }

        private void buttonNewChild_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxName.Text))
                {
                    MessageBox.Show("Имя не может быть пустым");
                    return;
                }

                if (currentElement.Elements == null)
                    return;

                var current = currentElement;

                var item = GetElement();
                current.Elements.Add(item);
                current.IsExpanded = true;
                item.IsSelected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateTrieView();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        SchemaElement currentElement => TreeViewSchema.SelectedItem as SchemaElement;
        private void TreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetElement(currentElement);
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

        private void SetElement(SchemaElement item)
        {
            if (item == null)
            {
                item = new SchemaElement();
                item.format = new TypeSComplex();
            }
                
            TextBoxName.Text = item.name;
            switch (item.Type)
            {
                case TypeElement.O: ComboBoxType.SelectedIndex = 0; break;
                case TypeElement.OM: ComboBoxType.SelectedIndex = 1; break;
                case TypeElement.N: ComboBoxType.SelectedIndex = 2; break;
                case TypeElement.NM: ComboBoxType.SelectedIndex = 3; break;
                case TypeElement.Y: ComboBoxType.SelectedIndex = 4; break;
                case TypeElement.YM: ComboBoxType.SelectedIndex = 5; break;
                default:
                    throw new Exception($"Неизвестный тип элемента: {item.Type}");
            }

            TextBoxFormat.Text = item.format.toSTRRUS();
            var type = TypeEnum.NONE;
            var formatString = item.format as TypeSString;
            var formatNumber = item.format as TypeSDigit;
            var formatDate = item.format as TypeSDate;
            var formatComplex = item.format as TypeSComplex;
            var formatDateTime = item.format as TypeSTime;


            TabControlType.SelectedIndex = -1;
            var listEn = new List<object>();
            
            if (formatString != null)
            {
                type = TypeEnum.STRING;
                listEn = formatString.Enum.Select(x => (object) x).ToList();
                TabItemString.IsSelected = true;
            }

            if (formatNumber != null)
            {
                type = TypeEnum.NUMBER;
                listEn = formatNumber.Enum.Select(x => (object)x).ToList();
                TabItemNumber.IsSelected = true;
            }
            if (formatDate != null) type = TypeEnum.DATE;
            buttonNewChild.IsEnabled = false;
            if (formatComplex != null)
            {
                type = TypeEnum.COMPLEX;
                buttonNewChild.IsEnabled = true;
            }
            if (formatDateTime != null) type = TypeEnum.DATETIME;

            switch (type)
            {
                case TypeEnum.STRING: ComboBoxDataType.SelectedIndex = 0; break;
                case TypeEnum.NUMBER: ComboBoxDataType.SelectedIndex = 1; break;
                case TypeEnum.DATE: ComboBoxDataType.SelectedIndex = 2; break;
                case TypeEnum.COMPLEX: ComboBoxDataType.SelectedIndex = 3; break;
                case TypeEnum.DATETIME: ComboBoxDataType.SelectedIndex = 4; break;
                default:
                    throw new Exception($"Неизвестный тип данных элемента: {item.format}");
            }


            SetStringParam(formatString);
            SetNumberParam(formatNumber);
          
            CheckBoxIndex.IsChecked = item.Unique;
            CheckBoxIndexGlobal.IsChecked = item.UniqueGlobal;
            SetEnum(listEn);
        }

        private SchemaElement GetElement()
        {
            var item = new SchemaElement();
            item.name = TextBoxName.Text;

            switch (ComboBoxType.SelectedIndex)
            {
                case 0: item.Type =  TypeElement.O; break;
                case 1: item.Type = TypeElement.OM; break;
                case 2: item.Type = TypeElement.N; break;
                case 3: item.Type = TypeElement.NM; break;
                case 4: item.Type = TypeElement.Y; break;
                case 5: item.Type = TypeElement.YM; break;
                default:
                    throw new Exception($"Неизвестный тип элемента: {item.Type}");
            }
            var type = TypeEnum.NONE;

            switch (ComboBoxDataType.SelectedIndex)
            {
                case 0: item.format = new TypeSString(); type = TypeEnum.STRING;  break;
                case 1: item.format = new TypeSDigit(); type = TypeEnum.NUMBER; break;
                case 2: item.format = new TypeSDate(); type = TypeEnum.DATE;  break;
                case 3: item.format = new TypeSComplex(); type = TypeEnum.COMPLEX; break;
                case 4: item.format = new TypeSTime(); type = TypeEnum.DATETIME;  break;
                default:
                    throw new Exception($"Неизвестный тип данных элемента: {item.format}");
            }

            switch (type)
            {
                case TypeEnum.STRING: GetStringParam(item.format as TypeSString); break;
                case TypeEnum.NUMBER: GetNumberParam(item.format as TypeSDigit); break;
            }
           item.Unique = CheckBoxIndex.IsChecked==true;
           item.UniqueGlobal = CheckBoxIndexGlobal.IsChecked == true;

           return item;
        }

        private void SetStringParam(TypeSString item)
        {
            TextBoxStringCountAll.Text = item!=null? item.ZnakMest.ToString() :"";
           
        }
        private void SetNumberParam(TypeSDigit item)
        {
            TextBoxNumberCountAll.Text = item != null ? item.ZnakMest.ToString() : "";
            TextBoxNumberCountPoint.Text = item != null ? item.ZnakMestPosDot.ToString() : "";
          
        }

        private void GetStringParam(TypeSString item)
        {
            item.ZnakMest = Convert.ToInt32(TextBoxStringCountAll.Text);
            item.Enum = ListBoxEnums.Items.Cast<string>().ToList();

        }
        private void GetNumberParam(TypeSDigit item)
        {
            item.ZnakMest = Convert.ToInt32(TextBoxNumberCountAll.Text);
            item.ZnakMestPosDot = Convert.ToInt32(TextBoxNumberCountPoint.Text);
            item.Enum = ListBoxEnums.Items.Cast<string>().Select(x=>Convert.ToInt32(x)).ToList();
        }


        private void SetEnum(List<object> List)
        {
            ListBoxEnums.Items.Clear();
            if (List != null)
                foreach (var en in List)
                {
                    ListBoxEnums.Items.Add(en);
                }
        }

        private void buttonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateTrieView();
        }

        private void UpdateTrieView()
        {
            CVSSchema.View.Refresh();
        }

        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var curr = currentElement;
                if (curr != null)
                {
                    XMLshemaMy.RemoveAt(curr);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateTrieView();
            }
        }

        private void ButtonDeleteAll_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены что хотите очистить список?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    XMLshemaMy.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateTrieView();
            }
        }

        private void buttonAccept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TextBoxName.Text))
                {
                    MessageBox.Show("Имя не может быть пустым");
                    return;
                }

                var current = currentElement;
                current.CopyFrom(GetElement());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateTrieView();
            }
        }

        private void buttonAddEnum_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var value = TextBoxEnum.Text.Trim();
                if (string.IsNullOrEmpty(value))
                {
                    MessageBox.Show("Не возможно добавить пустое перечисляемое значение");
                    return;
                }
                if(!ListBoxEnums.Items.Contains(value))
                    ListBoxEnums.Items.Add(value);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemEnumDelete_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var list = ListBoxEnums.SelectedItems.Cast<object>().ToList();
                foreach (var item in list)
                {
                    if (ListBoxEnums.Items.Contains(item))
                        ListBoxEnums.Items.Remove(item);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class FindPredicat
        {
            public string FindText { get; set; }
            public List<List<SchemaElement>> FindNODE { get; set; } = new List<List<SchemaElement>>();
            private int CurrItem { get; set; }

            public void Clear()
            {
                FindText = null;
                FindNODE.Clear();
                CurrItem = -1;
            }

            public List<SchemaElement> GetNEXT()
            {
                CurrItem++;
                if (CurrItem < FindNODE.Count) return FindNODE[CurrItem];
                return null;
            }

        }

        FindPredicat FindP = new FindPredicat();

        private void buttonFind_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var findstr = textBoxFind.Text.ToUpper();
                if (!string.IsNullOrEmpty(findstr))
                {
                 
                    if (FindP.FindText != findstr)
                    {
                        FindP.Clear();
                        FindP.FindText = findstr;
                        var items = XMLshemaMy.FindName(findstr);
                        foreach (var item in items)
                        {
                            FindP.FindNODE.Add( XMLshemaMy.FindPath(item));
                        }
                    }

                    var nod = FindP.GetNEXT();
                    if (nod != null)
                    {
                        SelectedNodes(nod);
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
        }

        private void SelectedNodes(List<SchemaElement> items)
        {
            for (var i = 0; i < items.Count-1; i++)
            {
                items[i].IsExpanded = true;
            }
            items.Last().IsSelected = true;
          
        }

        private void ButtonUp_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var current = currentElement;
                XMLshemaMy.ElementUp(current);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateTrieView();
            }
        }

        private void ButtonDown_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var current = currentElement;
                XMLshemaMy.ElementDown(current);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                UpdateTrieView();
            }
        }

        private void ComboBoxDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabItemNumber != null && TabItemString != null)
            {
                TabItemNumber.IsSelected = false;
                TabItemString.IsSelected = false;
                switch (ComboBoxDataType.SelectedIndex)
                {
                    case 0: TabItemString.IsSelected = true; break;
                    case 1: TabItemNumber.IsSelected = true;  break;
                    default: break;

                }
            }
         
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

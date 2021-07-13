using System;
using System.Collections.Generic;
using System.Linq;
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
using ServiceLoaderMedpomData;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для RemoteFolderDialog.xaml
    /// </summary>
    public partial class RemoteFolderDialog : Window
    {
        IWcfInterface wcf => LoginForm.wcf;
        string path;
        public string selectpath = "";
        public List<string> FileNames = new List<string>();

        bool OnlyFolder;
        bool OneFolder = false;

        private CollectionViewSource CollectionViewSourceFiles;
        public List<RemoteFolderDialogFileItem> FileItems { get; set; } = new List<RemoteFolderDialogFileItem>();
        public RemoteFolderDialog(string _path, bool OnlyFolder = false, bool OneFolder = false)
        {
            InitializeComponent();
            CollectionViewSourceFiles = (CollectionViewSource) FindResource("CollectionViewSourceFiles");
            this.OnlyFolder = OnlyFolder;
            this.OneFolder = OneFolder;

            try
            {
                setDrive();
                SetPath(_path);
                setlist(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void setDrive()
        {
            var drive = wcf.GetLocalDisk();
            comboBoxDrive.Items.Clear();
            foreach (var str in drive)
            {
                comboBoxDrive.Items.Add(str);
            }
        }

        private void CreateList(List<string> Folders,List<string> Files)
        {
            FileItems.Clear();
            FileItems.Add(new RemoteFolderDialogFileItem("...", FileItemType.Return));

            foreach (var fold in Folders)
            {
                FileItems.Add(new RemoteFolderDialogFileItem(fold, FileItemType.Folder));
            }

            foreach (var file in Files)
            {
                FileItemType type;
                switch (System.IO.Path.GetExtension(file)?.ToUpper())
                {
                    case ".ZIP": type = FileItemType.ZIP; break;
                    case ".XML": type = FileItemType.XML; break;
                    case ".XSD": type = FileItemType.XSD; break;
                    default: type = FileItemType.FILE; break;
                }

                FileItems.Add(new RemoteFolderDialogFileItem(file, type));
            }
            CollectionViewSourceFiles.View.Refresh();
        }

        void SetPath(string _path)
        {
            if (string.IsNullOrEmpty(_path) && comboBoxDrive.Items.Count != 0)
            {
                path = comboBoxDrive.Items[1].ToString();
            }
            else
            {
                path = _path;
            }
        }

        public bool IsFile(FileItemType item)
        {
            switch (item)
            {
                case FileItemType.FILE: return true;
                case FileItemType.Return: return false;
                case FileItemType.XML: return true;
                case FileItemType.ZIP: return true;
                case FileItemType.XSD: return true;
                default: return false;
            }
        }


        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            if (OnlyFolder)
            {
                selectpath = textBoxPath.Text;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                foreach (RemoteFolderDialogFileItem lvi in listView.SelectedItems)
                {
                    if (IsFile(lvi.Type))
                        FileNames.Add(lvi.Path);

                }

                if (FileNames.Count == 0)
                {
                    MessageBox.Show($@"Не выбрано не одного файла!");
                }
                else
                {
                    selectpath = FileNames[0];
                    this.DialogResult = true;
                    this.Close();
                }

            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            if (listView.SelectedItems.Count != 0 && !OneFolder)
            {
                if (listView.SelectedItems[0] is RemoteFolderDialogFileItem item)
                {
                    if (IsFile(item.Type)) return;
                    if (item.Type== FileItemType.Return)
                    {
                        if (System.IO.Path.GetDirectoryName(path) == null) return;
                        path = System.IO.Path.GetDirectoryName(path);
                        setlist(path);
                    }
                    else
                    {
                        path = System.IO.Path.Combine(path, item.Name);
                        setlist(path);
                    }
                }
            }
        }


        void setlist(string selectPath)
        {
            textBoxPath.Text = path;
            comboBoxDrive.Text = path;
          
            var Folders = wcf.GetFolderLocal(selectPath).ToList();
            var Files = new List<string>();
            if (!OnlyFolder)
            {
                 Files.AddRange(wcf.GetFilesLocal(selectPath, "*.XML"));
                 Files.AddRange(wcf.GetFilesLocal(selectPath, "*.ZIP"));
                 Files.AddRange(wcf.GetFilesLocal(selectPath, "*.XSD"));
            }
            CreateList(Folders, Files);
        }

        private void comboBoxDrive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (OneFolder) return;
                path = comboBoxDrive.Text;
                setlist(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBoxDrive_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !OneFolder)
                comboBoxDrive_SelectionChanged(comboBoxDrive,null);
        }
    }


    public enum FileItemType
    {
        XML,
        ZIP,
        FILE,
        XSD,
        Return,
        Folder
    }
    public class RemoteFolderDialogFileItem
    {
        public RemoteFolderDialogFileItem(string Path, FileItemType Type)
        {
            this.Path = Path;
            this.Type = Type;
        }
        public string Path { get; set; }
        public string Name => System.IO.Path.GetFileName(Path);
        public FileItemType Type { get; set; }
    }



}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для ShowFileItem.xaml
    /// </summary>
    public partial class ShowFileItem : Window, INotifyPropertyChanged
    {
        public ShowFileItemVM VM { get; set; } = new ShowFileItemVM(LoginForm.wcf);
        private FilePacket pack;

        private IWcfInterface wcf => LoginForm.wcf;
        public List<ServiceLoaderMedpomData.FileItem> FileItems => pack.Files;

        private CollectionViewSource CVSFileList;
        public ShowFileItem(FilePacket pack)
        {
            this.pack = pack;
            InitializeComponent();
            VM.SetPack(pack); 
            CVSFileList = (CollectionViewSource) FindResource("CVSFileList");
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            CVSFileList.View?.Refresh();
        }

        private  System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
        private void ButtonDownload_Click(object sender, RoutedEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var th = new Thread(DownloadThread) { IsBackground = true };
                th.Start(folderBrowserDialog1.SelectedPath);
            }
        }

        public void DownloadThread(object _path)
        {
            try
            {
                var path = _path.ToString();
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                ProgressBarMain.Dispatcher.Invoke(() =>
                {
                    ProgressBarMain.Value = 0;
                    ProgressBarMain.Maximum = 3;
                    LabelMain.Content = @"Загрузка FILE_STAT и FLK_ERROR";

                });
               

                if (!string.IsNullOrEmpty(pack.PATH_STAT))
                {
                    DOWNLOADFILE(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.PATH_STAT)), pack.CodeMO, 0, TypeDOWLOAD.FILE_STAT);
                }

                ProgressBarMain.Dispatcher.Invoke(() =>
                {
                    ProgressBarMain.Value = 1;
                    ProgressBarMain.Maximum = 3;
                    LabelMain.Content = @"Загрузка XML";

                });

                //ОСНОВНЫЕ ФАЙЛЫ
                for (var i = 0; i < pack.Files.Count; i++)
                {
                    DOWNLOADFILE(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].FilePach)), pack.CodeMO, i, TypeDOWLOAD.File);
                }


                //ФАЙЛЫ L
                for (int i = 0; i < pack.Files.Count; i++)
                {
                    if (pack.Files[i].filel != null)
                        DOWNLOADFILE(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].filel.FilePach)), pack.CodeMO, i, TypeDOWLOAD.File);
                }


                ProgressBarMain.Dispatcher.Invoke(() =>
                {
                    ProgressBarMain.Value = 2;
                    ProgressBarMain.Maximum = 3;
                    LabelMain.Content = @"Загрузка LOG";
                });
                //ФАЙЛЫ ЛОГОВ
                for (int i = 0; i < pack.Files.Count; i++)
                {
                    if (pack.Files[i].filel != null)
                        if (pack.Files[i].filel.FileLog != null)
                            DOWNLOADFILE(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].filel.FileLog.FilePath)), pack.CodeMO, i, TypeDOWLOAD.FILE_L_LOG);

                    if (pack.Files[i].filel != null)
                        if (!string.IsNullOrEmpty(pack.Files[i].filel.PATH_LOG_XML))
                            DOWNLOADFILE(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].filel.PATH_LOG_XML)), pack.CodeMO, i, TypeDOWLOAD.XML_OTCHET_L);

                    if (pack.Files[i].FileLog != null)
                        DOWNLOADFILE(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].FileLog.FilePath)), pack.CodeMO, i, TypeDOWLOAD.FILE_LOG);

                    if (!string.IsNullOrEmpty(pack.Files[i].PATH_LOG_XML))
                        DOWNLOADFILE(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].PATH_LOG_XML)), pack.CodeMO, i, TypeDOWLOAD.XML_OTCHET_H);
                }

                ProgressBarMain.Dispatcher.Invoke(() =>
                {
                    ProgressBarMain.Value = 0;
                    ProgressBarMain.Maximum = 3;
                    LabelMain.Content = @"Завершено";
                    if (MessageBox.Show(@"Показать файлы?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(path);
                    }

                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


        void DOWNLOADFILE(string FileName, string CODE_MO, int indFile, TypeDOWLOAD type)
        {
            var st = System.IO.File.Create(FileName);
            var length = wcf.GetFileLength(CODE_MO, indFile, type);
            ProgressBarDownload.Dispatcher.Invoke(() =>
            {
                ProgressBarDownload.Value = 0;
                ProgressBarDownload.Maximum = Convert.ToInt32(length);
                LabelDownload.Content = $@"Загрузка: {System.IO.Path.GetFileName(FileName)}";
            });

            byte[] buff = wcf.GetFile(CODE_MO, indFile, type, 0);
            int countREAD = 0;



            while (buff.Length != 0)
            {
                countREAD += buff.Length;
                ProgressBarDownload.Dispatcher.Invoke(() =>
                {
                    ProgressBarDownload.Value = countREAD;
                    ProgressBarDownload.Maximum = Convert.ToInt32(length);

                });
                st.Write(buff, 0, buff.Length);
                buff = wcf.GetFile(CODE_MO, indFile, type, countREAD);

            }
            st.Close();

            ProgressBarDownload.Dispatcher.Invoke(() =>
            {
                ProgressBarDownload.Value = 0;
                ProgressBarDownload.Maximum = Convert.ToInt32(length);
                LabelDownload.Content = @"Завершено";
            });
        }

        private List<ServiceLoaderMedpomData.FileItem> selectedItems => DataGridFileList.SelectedCells.Select(x => (ServiceLoaderMedpomData.FileItem) x.Item).Distinct().ToList();

        private void MenuItemShowMain_Click(object sender, RoutedEventArgs e)
        {
            var select = selectedItems;
            if (select.Count != 0)
            {
                try
                {
                    var path = select[0].FilePach;
                    if (Properties.Settings.Default.ISVIRTUALPATH)
                    {
                        path = System.IO.Path.Combine(Properties.Settings.Default.VIRTUALPATH, System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)), System.IO.Path.GetFileName(path));
                    }
                    ShowSelectedInExplorer.FileOrFolder(path);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuItemShowPers_Click(object sender, RoutedEventArgs e)
        {
            var select = selectedItems;
            if (select.Count != 0)
            {
                try
                {
                    var path = select[0].filel.FilePach;
                    if (Properties.Settings.Default.ISVIRTUALPATH)
                    {
                        path = System.IO.Path.Combine(Properties.Settings.Default.VIRTUALPATH, System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)), System.IO.Path.GetFileName(path));
                    }
                    ShowSelectedInExplorer.FileOrFolder(path);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public List<ServiceLoaderMedpomData.FileItem> SelectedFileItems => DataGridFileList.SelectedCells.Select(x => (ServiceLoaderMedpomData.FileItem)x.Item).Distinct().ToList();

        private void DataGridFileList_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SelectedFileItems));
        }
        #region  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

      
    }


    public class ShowFileItemVM:INotifyPropertyChanged
    {
        private FilePacket _pack;
        public FilePacket pack
        {
            get => _pack;
            private set { _pack = value; OnPropertyChanged(); }
        }

        private IWcfInterface wcf;
        private Dispatcher dispatcher;
        public ShowFileItemVM(IWcfInterface wcf)
        {
            this.wcf = wcf;
            dispatcher = Dispatcher.CurrentDispatcher;
        }

        public void SetPack(FilePacket pack)
        {
            this.pack = pack;
        }



        public ICommand ShowFileInFolder => new Command(o =>
        {
            try
            {
                var select = (List<ServiceLoaderMedpomData.FileItem>)o;
                if (select.Count != 0)
                {
                    try
                    {
                        var Paths = new List<string>();
                        foreach (var item in select)
                        {
                            Paths.Add(ConvertToVirtualPath(item.FilePach));
                            if(item.filel!=null)
                                Paths.Add(ConvertToVirtualPath(item.filel.FilePach));
                        }
                        ShowSelectedInExplorer.FilesOrFolders(Paths);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ShowLogsInFolder => new Command(o =>
        {
            try
            {
                var select = (List<ServiceLoaderMedpomData.FileItem>)o;
                if (select.Count != 0)
                {
                    try
                    {
                        var Paths = new List<string>();
                        foreach (var item in select)
                        {
                            if(item.FileLog!=null)
                                Paths.Add(ConvertToVirtualPath(item.FileLog.FilePath));
                            if (item.filel?.FileLog != null)
                                Paths.Add(ConvertToVirtualPath(item.filel.FileLog.FilePath));
                        }
                        ShowSelectedInExplorer.FilesOrFolders(Paths);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        private string ConvertToVirtualPath(string path)
        {
            var res = path;
            if (Properties.Settings.Default.ISVIRTUALPATH)
            {
                res = System.IO.Path.Combine(Properties.Settings.Default.VIRTUALPATH, System.IO.Path.GetFileName(System.IO.Path.GetDirectoryName(path)), System.IO.Path.GetFileName(path));
            }
            return res;
        }
        private System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
        public ICommand SaveFileLocal => new Command(async o =>
        {
            try
            {
                if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    await DownloadAsync(fbd.SelectedPath);
                    if (MessageBox.Show(@"Показать файлы?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(fbd.SelectedPath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

      
        public ProgressItem DownloadProgress { get; set; } = new ProgressItem();
        private async Task DownloadAsync(string path)
        {
            await Task.Run(() => { Download(path); });
        }
        private void Download(string path)
        {
            try
            {
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                dispatcher.Invoke(() =>
                {
                    DownloadProgress.Value = 0;
                    DownloadProgress.Maximum = 3;
                    DownloadProgress.Text = @"Загрузка FILE_STAT и FLK_ERROR";

                });
                if (!string.IsNullOrEmpty(pack.PATH_STAT))
                {
                    DownloadFile(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.PATH_STAT)), pack.CodeMO, 0, TypeDOWLOAD.FILE_STAT);
                }
                dispatcher.Invoke(() =>
                {
                    DownloadProgress.Value = 1;
                    DownloadProgress.Text = "Загрузка XML";
                });
                //ОСНОВНЫЕ ФАЙЛЫ
                for (var i = 0; i < pack.Files.Count; i++)
                {
                    DownloadFile(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].FilePach)), pack.CodeMO, i, TypeDOWLOAD.File);
                    if (pack.Files[i].filel != null)
                        DownloadFile(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].filel.FilePach)), pack.CodeMO, i, TypeDOWLOAD.FileL);
                }
                dispatcher.Invoke(() =>
                {
                    DownloadProgress.Value = 2;
                    DownloadProgress.Text = @"Загрузка LOG";
                });
                //ФАЙЛЫ ЛОГОВ
                for (var i = 0; i < pack.Files.Count; i++)
                {
                    if (pack.Files[i].filel != null)
                        if (pack.Files[i].filel.FileLog != null)
                            DownloadFile(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].filel.FileLog.FilePath)), pack.CodeMO, i, TypeDOWLOAD.FILE_L_LOG);
                    if (pack.Files[i].filel != null)
                        if (!string.IsNullOrEmpty(pack.Files[i].filel.PATH_LOG_XML))
                            DownloadFile(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].filel.PATH_LOG_XML)), pack.CodeMO, i, TypeDOWLOAD.XML_OTCHET_L);
                    if (pack.Files[i].FileLog != null)
                        DownloadFile(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].FileLog.FilePath)), pack.CodeMO, i, TypeDOWLOAD.FILE_LOG);
                    if (!string.IsNullOrEmpty(pack.Files[i].PATH_LOG_XML))
                        DownloadFile(System.IO.Path.Combine(path, System.IO.Path.GetFileName(pack.Files[i].PATH_LOG_XML)), pack.CodeMO, i, TypeDOWLOAD.XML_OTCHET_H);
                }

                dispatcher.Invoke(() =>
                {
                    DownloadProgress.Value = 0;
                    DownloadProgress.Text = @"Завершено";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public ProgressItem DownloadFileProgress { get; set; } = new ProgressItem();
       
        private void DownloadFile(string FileName, string CODE_MO, int indFile, TypeDOWLOAD type)
        {
            var st = System.IO.File.Create(FileName);
            var length = wcf.GetFileLength(CODE_MO, indFile, type);
            dispatcher.Invoke(() =>
            {
                DownloadFileProgress.Value = 0;
                DownloadFileProgress.Maximum = Convert.ToInt32(length);
                DownloadFileProgress.Text = $@"Загрузка: {System.IO.Path.GetFileName(FileName)}";
            });

            var buff = wcf.GetFile(CODE_MO, indFile, type, 0);
            var countREAD = 0;
            while (buff.Length != 0)
            {
                countREAD += buff.Length;
                dispatcher.Invoke(() =>
                {
                    DownloadFileProgress.Value = countREAD;
                });
                st.Write(buff, 0, buff.Length);
                buff = wcf.GetFile(CODE_MO, indFile, type, countREAD);
            }
            st.Close();

            dispatcher.Invoke(() =>
            {
                DownloadFileProgress.Value = 0;
                DownloadFileProgress.Text = @"Завершено";
            });
        }

       


        #region  INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }

}

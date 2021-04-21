using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
using ClientServiceWPF.Class;
using ServiceLoaderMedpomData;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для ShowFileItem.xaml
    /// </summary>
    public partial class ShowFileItem : Window
    {
        private FilePacket pack;

        private IWcfInterface wcf => LoginForm.wcf;
        public List<ServiceLoaderMedpomData.FileItem> FileItems => pack.Files;

        private CollectionViewSource CVSFileList;
        public ShowFileItem(FilePacket pack)
        {
            this.pack = pack;
            InitializeComponent();
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
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using ServiceLoaderMedpomData;

namespace ClientServiceWPF.Launcher
{
    /// <summary>
    /// Логика взаимодействия для Launcher.xaml
    /// </summary>
    public partial class Launcher : Window
    {
        private IWcfInterface wcf => LoginForm.wcf;
        public Launcher()
        {
            InitializeComponent();

            string curr_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            DeleteFolder = System.IO.Path.Combine(curr_dir, "DELETE");
            UpdateFolder = System.IO.Path.Combine(curr_dir, "UPDATE");
        }

        string DeleteFolder;
        string UpdateFolder;


        ObservableCollection<UpdateFile> FileForUpdate = new ObservableCollection<UpdateFile>();
        public ObservableCollection<UpdateFile> GetList => FileForUpdate;

        private void CheckUpdate()
        {
            try
            {
                ProgressMainIsInd = true;
                MainText = "Проверка обновлений";
                this.Dispatcher?.Invoke(new Action(() =>
                {
                    try
                    {
                        FileForUpdate.Clear();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("(LV1):" + ex.Message);
                    }
                }));
                ServiceLoaderMedpomData.Version version = null;
                try
                {
                    version = wcf.GetVersion();
                }
                catch (Exception ex)
                {
                    throw new Exception("(LV2):" + ex.Message);
                }
                var curr_dir = "";
                try
                {
                    curr_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                }
                catch (Exception ex)
                {
                    throw new Exception("(LV3):" + ex.Message);
                }
                MainText = "Формирование списка обновлений";
                foreach (FileAndMD5 file in version.FileList)
                {
                    var PathFile = System.IO.Path.Combine(curr_dir, file.Name);
                    if (File.Exists(PathFile))
                    {
                        if (ServiceLoaderMedpomData.Version.GetMd5Hash(PathFile) != file.MD5)
                        {
                            Dispatcher?.Invoke(new Action(() =>
                            {
                                FileForUpdate.Add(new UpdateFile(file, StatusUpdate.New));
                            }));

                        }
                    }
                    else
                    {
                        Dispatcher?.Invoke(new Action(() =>
                        {
                            FileForUpdate.Add(new UpdateFile(file, StatusUpdate.New));
                        }));

                    }
                }
                ProgressMainIsInd = false;
                MainText = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при проверке:" + ex.Message);
                RESTART = true;
            }

        }

        private void Update()
        {
            try
            {
                MainText = "Обновление...";
                ProgressMainMax = FileForUpdate.Count;
                int i = 0;
                foreach (UpdateFile item in FileForUpdate)
                {
                    item.StatusUpdate = StatusUpdate.UpdateNow;
                    item.newFilePath = LoadFile(item.file, UpdateFolder);
                  
                    i++;
                    ProgressMainValue = i;
                    item.StatusUpdate = StatusUpdate.Updated;
                }
            }
            catch (Exception ex)
            {
                RESTART = true;
                MessageBox.Show("Ошибка при обновлении: " + ex.Message);
            }
        }




        private bool Replace()
        {
            try
            {
                bool res = false;
                string curr_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (!Directory.Exists(DeleteFolder))
                {
                    Directory.CreateDirectory(DeleteFolder);
                }
                foreach (UpdateFile item in FileForUpdate)
                {
                    string PathFile = System.IO.Path.Combine(curr_dir, item.file.Name);
                    if (File.Exists(PathFile))
                    {
                        if (!Directory.Exists(System.IO.Path.GetDirectoryName(System.IO.Path.Combine(DeleteFolder, item.file.Name))))
                        {
                            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.Combine(DeleteFolder, item.file.Name)));
                        }

                        File.Move(PathFile, System.IO.Path.Combine(DeleteFolder, item.file.Name));
                    }
                    if (!Directory.Exists(System.IO.Path.GetDirectoryName(PathFile)))
                    {
                        Directory.CreateDirectory(System.IO.Path.GetDirectoryName(PathFile));
                    }
                    File.Move(item.newFilePath, PathFile);
                    res = true;
                }
                if (FileForUpdate.Count == 0)
                {
                    Directory.Delete(DeleteFolder, true);
                }

                return res;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при замене файлов: " + ex.Message);
                return false;
            }
        }
        private string LoadFile(FileAndMD5 file, string Dir_to)
        {
            try
            {
                if (!Directory.Exists(Dir_to))
                {
                    Directory.CreateDirectory(Dir_to);
                }
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(System.IO.Path.Combine(Dir_to, file.Name))))
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(System.IO.Path.Combine(Dir_to, file.Name)));
                }
                Stream st = File.Create(System.IO.Path.Combine(Dir_to, file.Name));
                int count = 1048576;
                int offset = 0;
                int i = 0;
                ProgressLv1Max = Math.Ceiling(Convert.ToDouble(file.Length / count));
                while (offset <= file.Length)
                {
                    var buf = wcf.LoadFileUpdate(file, offset, count);
                    st.Write(buf, 0, buf.Count());
                    i++;

                    offset += count;
                    ProgressLv1Value = i;
                }
                st.Close();
                return System.IO.Path.Combine(Dir_to, file.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении " + file.Name + " :" + ex.Message);
                return "";
            }
        }

        double ProgressMainValue
        {
            get
            {
                double value = 0;
                progressBarMain.Dispatcher.Invoke(new Action(() =>
                {
                    value = progressBarMain.Value;
                }));
                return value;
            }
            set
            {
                progressBarLevel1.Dispatcher.Invoke(new Action(() =>
                {
                    progressBarMain.Value = value;
                }));
            }
        }
        double ProgressMainMax
        {
            set
            {
                progressBarLevel1.Dispatcher.Invoke(new Action(() =>
                {
                    progressBarMain.Maximum = value;
                }));
            }
        }
        bool ProgressMainIsInd
        {
            set
            {
                progressBarLevel1.Dispatcher.Invoke(new Action(() =>
                {
                    progressBarMain.IsIndeterminate = value;
                }));
            }
        }

        double ProgressLv1Value
        {
            set
            {
                progressBarLevel1.Dispatcher.Invoke(new Action(() =>
                {
                    progressBarLevel1.Value = value;
                }));
            }
        }
        double ProgressLv1Max
        {
            set
            {
                progressBarLevel1.Dispatcher.Invoke(new Action(() =>
                {
                    progressBarLevel1.Maximum = value;
                }));
            }
        }
        bool ProgressLv1IsInd
        {
            get
            {
                bool value = false;
                progressBarLevel1.Dispatcher.Invoke(new Action(() =>
                {
                    value = progressBarLevel1.IsIndeterminate;
                }));
                return value;
            }
            set
            {
                progressBarLevel1.Dispatcher.Invoke(new Action(() =>
                {
                    progressBarLevel1.IsIndeterminate = value;
                }));
            }
        }

        string MainText
        {
            get
            {
                string value = "";
                textBlock1.Dispatcher.Invoke(() =>
                {
                    value = textBlock1.Text;
                });
                return value;
            }
            set
            {
                textBlock1.Dispatcher.Invoke(() =>
                {
                    textBlock1.Text = value;
                });
            }
        }

        private void LauncherForm_Loaded(object sender, RoutedEventArgs e)
        {
            var th = new Thread(Threadupdate) { IsBackground = true };
            th.Start();
            //DynamicDictionaryLoader.AddDictionary(textBox1);

        }

        public static bool IsNeedUpdate()
        {
            var version = LoginForm.wcf.GetVersion();
            string curr_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            foreach (var file in version.FileList)
            {
                string PathFile = System.IO.Path.Combine(curr_dir, file.Name);
                if (File.Exists(PathFile))
                {
                    if (ServiceLoaderMedpomData.Version.GetMd5Hash(PathFile) != file.MD5)
                    {
                        return true;
                    }
                }
                else
                {
                    return true;

                }
            }
            return false;
        }

        public bool RESTART = false;
        void Threadupdate()
        {
            var curr_exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ClearFolder();
            CheckUpdate();
            Update();
            var rep = Replace();
            var clear = TryClearFolder();
            if (rep && !clear)
            {
                var pr = new Process { StartInfo = { FileName = curr_exe } };
                pr.Start();
                RESTART = true;
            }
            Dispatcher?.Invoke(Close);
        }

        bool TryClearFolder()
        {
            try
            {
                if (Directory.Exists(DeleteFolder))
                {
                    Directory.Delete(DeleteFolder, true);
                }
                if (Directory.Exists(UpdateFolder))
                {
                    Directory.Delete(UpdateFolder, true);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        void ClearFolder()
        {
            try
            {
                if (Directory.Exists(DeleteFolder))
                {
                    Directory.Delete(DeleteFolder, true);
                }
                if (Directory.Exists(UpdateFolder))
                {
                    Directory.Delete(UpdateFolder, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при очистке директорий: " + ex.Message);
                return;
            }
        }
    }


    public enum StatusUpdate
    {
        New = 1,
        UpdateNow = 2,
        Updated = 3
    }

    public class UpdateFile : INotifyPropertyChanged
    {
        public UpdateFile(FileAndMD5 file, StatusUpdate StatusUpdate)
        {
            this.file = file;
            this.StatusUpdate = StatusUpdate;
        }
        public FileAndMD5 file { get; set; }
        public string newFilePath { get; set; }
        StatusUpdate statusUpdate;
        public StatusUpdate StatusUpdate
        {
            get
            {
                return statusUpdate;
            }
            set
            {
                statusUpdate = value;
                OnPropertyChanged("StatusUpdate");
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

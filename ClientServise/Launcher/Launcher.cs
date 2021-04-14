using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using System.IO;
using ServiceLoaderMedpomData;
using System.Threading;
using System.Diagnostics;

namespace ClientServise.Launcher
{
    public partial class Launcher : Form
    {
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

        public ObservableCollection<UpdateFile> GetList
        {
            get
            {
                return FileForUpdate;
            }
        }


        private void CheckUpdate()
        {
        try
            { 
            MainText = "Проверка обновлений";
            this.Invoke(new Action(() =>
            {
                FileForUpdate.Clear();
            }));

            var version = MainForm.MyWcfConnection.GetVersion();
            string curr_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            MainText = "Формирование списка обновлений";
            foreach (FileAndMD5 file in version.FileList)
            {
                string PathFile = System.IO.Path.Combine(curr_dir, file.Name);
                if (File.Exists(PathFile))
                {
                    if (ServiceLoaderMedpomData.Version.GetMd5Hash(PathFile) != file.MD5)
                    {
                        this.Invoke(new Action(() =>
                        {
                            FileForUpdate.Add(new UpdateFile(file, StatusUpdate.New));
                        }));

                    }
                }
                else
                {
                    this.Invoke(new Action(() =>
                    {
                        FileForUpdate.Add(new UpdateFile(file, StatusUpdate.New));
                    }));

                }
            }

            this.Invoke(new Action(() =>
                {
                    RefreshList();
                }));
            
            MainText = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateFile()
        {
            try
            {
                string curr_dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                MainText = "Обновление...";
                ProgressMainMax = FileForUpdate.Count;
                int i = 0;
                foreach (UpdateFile item in FileForUpdate)
                {
                    item.StatusUpdate = StatusUpdate.UpdateNow;
                    item.newFilePath = LoadFile(item.file, UpdateFolder);
                    if (item.file.Name.ToUpper() == "UPDATEINFO.TXT")
                    {
                        Properties.Settings.Default.ShowUpdate = true;
                        Properties.Settings.Default.Save();
                    }
                    i++;
                    ProgressMainValue = i;
                    item.StatusUpdate = StatusUpdate.Updated;
                    this.Invoke(new Action(() =>
                    {
                        RefreshList();
                    }));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении: " + ex.Message);
            }
        }

        private bool Replace()
        {
            try
            {
                bool res = false;
                MainText = "Замена файлов...";
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
                double t =  Convert.ToDouble(file.Length) /  Convert.ToDouble(count);
                int k = Convert.ToInt32(Math.Ceiling(t));
                ProgressLv1Max = Convert.ToInt32(Math.Ceiling(t));
                if (ProgressLv1Max == 0)
                    ProgressLv1Max = 1;
                              
                while (offset <= file.Length)
                {
                    var buf = MainForm.MyWcfConnection.LoadFileUpdate(file, offset, count);
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



        int ProgressMainValue
        {
            get
            {
                int value = 0;
                progressBarMain.Invoke(new Action(() =>
                {
                    value = progressBarMain.Value;
                }));
                return value;
            }
            set
            {
                progressBarLevel1.Invoke(new Action(() =>
                {
                    if (value > progressBarMain.Maximum)
                        progressBarMain.Value = progressBarMain.Maximum;
                    else
                        progressBarMain.Value = value;
                
                }));
            }
        }
        int ProgressMainMax
        {
            get
            {
                int value = 0;
                progressBarMain.Invoke(new Action(() =>
                {
                    value = progressBarMain.Maximum;
                }));
                return value;
            }
            set
            {
                progressBarLevel1.Invoke(new Action(() =>
                {
                    progressBarMain.Maximum = value;
                }));
            }
        }
       

        int ProgressLv1Value
        {
            get
            {
                int value = 0;
                progressBarLevel1.Invoke(new Action(() =>
                {                    
                    value = progressBarLevel1.Value;
                }));
                return value;
            }
            set
            {
                progressBarLevel1.Invoke(new Action(() =>
                {
                    if (value > progressBarLevel1.Maximum)
                        progressBarLevel1.Value = progressBarLevel1.Maximum;
                    else
                    progressBarLevel1.Value = value;
                }));
            }
        }
        int ProgressLv1Max
        {
            get
            {
                int value = 0;
                progressBarLevel1.Invoke(new Action(() =>
                {
                    value = progressBarLevel1.Maximum;
                }));
                return value;
            }
            set
            {
                progressBarLevel1.Invoke(new Action(() =>
                {
                    progressBarLevel1.Maximum = value;
                }));
            }
        }      

        string MainText
        {
            get
            {
                string value = "";
                textBlock1.Invoke(new Action(() =>
                {
                    value = textBlock1.Text;
                }));
                return value;
            }
            set
            {
                textBlock1.Invoke(new Action(() =>
                {
                    textBlock1.Text = value;
                }));
            }
        }

        private void Launcher_Load(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(Threadupdate));
            th.IsBackground = true;
            th.Start();
        }


        void RefreshList()
        {
            listView1.Items.Clear();
            foreach (UpdateFile item in FileForUpdate)
            {
                if (item.StatusUpdate == StatusUpdate.Updated)
                {


                    listView1.Items.Add(item.file.Name, 1);
                }
                else
                {
                    listView1.Items.Add(item.file.Name, 0);
                }

            }
           
        }
        public bool RESTART = false;
        void Threadupdate()
        {
            string curr_exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            ClearFolder();
            CheckUpdate();
            UpdateFile();
            if (Replace())
            {
                Process pr = new Process();
                pr.StartInfo.FileName = curr_exe;

                pr.Start();
                RESTART = true;

            }
            this.Invoke(new Action(() =>
            {
                this.Close();
            }));


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
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }



        public event PropertyChangedEventHandler PropertyChanged;
    }
}

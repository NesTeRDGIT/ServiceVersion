using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Runtime.Serialization;
namespace ServiceLoaderMedpomData
{
    /// <summary>
    /// Статус файла
    /// </summary>
    [Serializable]
    [DataContract]
    public enum JournalStatus
    {
        /// <summary>
        /// Архив битый сообщение не отправлено
        /// </summary>
        [EnumMember]
        ArchiveCrachNoSendErrorMessage = 0,
        /// <summary>
        /// Архив битый сообщение отправлено
        /// </summary>
        [EnumMember]
        ArchiveCrachSendErrorMessage = 1,
        /// <summary>
        /// Архив правильный
        /// </summary>
        [EnumMember]
        ArchiveValid =2 ,
        /// <summary>
        /// Имя архива не корректно сообщение не отправлено
        /// </summary>
        [EnumMember]
        ArchiveNameNotCorrectNoSendErrorMessage = 3,
        /// <summary>
        /// Имя архива не корректно сообщение отправлено
        /// </summary>
        [EnumMember]
        ArchiveNameNotCorrectSendErrorMessage = 4,
        /// <summary>
        /// Файл не читаеться сообщение не отправлено
        /// </summary>
        [EnumMember]
        FileCrachNoSendErrorMessage = 5,
        /// <summary>
        /// Файл не читаеться сообщение отправлено
        /// </summary>
       [EnumMember]
        FileCrachSendErrorMessage = 6,
        /// <summary>
        /// Файл корректный
        /// </summary>
        [EnumMember]
        FileValid = 7,
        /// <summary>
        /// Имя файла не корректно сообщение не отправлено
        /// </summary>
        [EnumMember]
        FileNameNotCorrectNoSendErrorMessage = 8,
        /// <summary>
        /// Имя файла не корректно сообщение отправлено
        /// </summary>
        [EnumMember]
        FileNameNotCorrectSendErrorMessage = 9
    }
    /// <summary>
    /// Элемент журнала. C уведомлением об изменении в элементах для BindingSource
    /// </summary>
    [Serializable]
    [DataContract]
    public class JournalItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Имя файла
        /// </summary>
          [DataMember]
        private string filename;
        /// <summary>
        /// Путь к файлу
        /// </summary>
          [DataMember]
        private string filepath;
        /// <summary>
        /// Статус
        /// </summary>
          [DataMember]
        private JournalStatus status;
        /// <summary>
        /// Комментарий
        /// </summary>
          [DataMember]
        private string comment;
        /// <summary>
        /// Дата поступеления
        /// </summary>
          [DataMember]
        private DateTime date;

        public event PropertyChangedEventHandler PropertyChanged; //Уведомления
        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }//уведомление

        public JournalItem()
        {
        }

        public JournalItem(string fn, string fp, JournalStatus st, string com, DateTime dt)
        {
            filename = fn;
            filepath = fp;
            status = st;
            comment = com;
            date = dt;
        }
        /// <summary>
        /// Имя файла
        /// </summary>
        public string FileName
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
                NotifyPropertyChanged("filename");
            }
        }
        /// <summary>
        /// Путь к файлу
        /// </summary>
        public string FilePath
        {
            get
            {
                return filepath;
            }
            set
            {

                filepath = value;
                NotifyPropertyChanged("filepath");
            }
        }
        /// <summary>
        /// Статус
        /// </summary>
        public JournalStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
                NotifyPropertyChanged("status");
            }
        }
        /// <summary>
        /// Комментарий
        /// </summary>
        public string Comment
        {
            get
            {
                return comment;
            }
            set
            {
                comment = value;
                NotifyPropertyChanged("comment");
            }
        }
        /// <summary>
        /// Дата
        /// </summary>
        public DateTime Date
        {
            get
            {
                return date;
            }
            set
            {
                date = value;
                NotifyPropertyChanged("date");
            }
        }
    }

    /// <summary>
    /// Журнал поступивших файлов
    /// </summary>
    [Serializable]
    [DataContract]
    public class JournalReception
    {
        /// <summary>
        /// Базовый список
        /// </summary>
        BindingList<JournalItem> list;
        /// <summary>
        /// Список в соответствии со значеним Fillter 
        /// </summary>
        BindingList<JournalItem> listFillter;
        /// <summary>
        /// Значение фильтра
        /// </summary>
        List<JournalStatus> Fillter;

        public JournalReception()
        {
            list = new BindingList<JournalItem>();
            listFillter = new BindingList<JournalItem>();
            Fillter = new List<JournalStatus>();
            Fillter.Add(JournalStatus.ArchiveCrachNoSendErrorMessage);
        Fillter.Add(JournalStatus.ArchiveCrachSendErrorMessage);
        Fillter.Add(JournalStatus.ArchiveValid);       
        Fillter.Add(JournalStatus.ArchiveNameNotCorrectNoSendErrorMessage);       
        Fillter.Add(JournalStatus.ArchiveNameNotCorrectSendErrorMessage);        
        Fillter.Add(JournalStatus.FileCrachNoSendErrorMessage);        
        Fillter.Add(JournalStatus.FileCrachSendErrorMessage);        
        Fillter.Add(JournalStatus.FileValid);       
        Fillter.Add(JournalStatus.FileNameNotCorrectNoSendErrorMessage);       
        Fillter.Add(JournalStatus.FileNameNotCorrectSendErrorMessage);
            list.ListChanged += new ListChangedEventHandler(list_ListChanged);

        }
        //Событие на изменение элемента в базовом списке. Для Переноса в список фильтра
        void list_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.ItemChanged && e.PropertyDescriptor.DisplayName == "Status")
                if (Fillter.Contains(list[e.NewIndex].Status))
                    listFillter.Insert(0, list[e.NewIndex]);
        }
        /// <summary>
        /// Добавить в начало
        /// </summary>
        /// <param name="item">Элемент</param>
        public void Insert(JournalItem item)
        {
            list.Insert(0, item);
            if (Fillter.Contains(item.Status))
                listFillter.Insert(0, item);


        }

        /// <summary>
        /// Добавить в начало
        /// </summary>
        /// <param name="filename">Имя файла</param>
        /// <param name="filepath">Путь к файлу</param>
        /// <param name="Status">Статус</param>
        /// <param name="Date">Дата</param>
        /// <param name="comment">Комментарий</param>
        /// <returns></returns>
        public JournalItem Insert(string filename, string filepath, JournalStatus Status, DateTime Date, string comment)
        {
            JournalItem item = new JournalItem();
            item.FileName = filename;
            item.FilePath = filepath;
            item.Status = Status;
            item.Date = Date;
            item.Comment = comment;
            list.Insert(0, item);
            if (Fillter.Contains(item.Status))
                listFillter.Insert(0, item);
            return item;
        }
        /// <summary>
        /// Удалить из списка
        /// </summary>
        /// <param name="item">Элемент</param>
        public void Remove(JournalItem item)
        {
            list.Remove(item);
            listFillter.Remove(item);
        }
        /// <summary>
        /// Доступ к элементам
        /// </summary>
        /// <param name="index">Индекс элемента</param>
        /// <returns>Элемент</returns>
        public JournalItem this[int index]
        {
            get
            {
                return list[index];
            }
            set
            {
                list[index] = value;
            }
        }
        /// <summary>
        /// Кол-во элементов в базовом списке
        /// </summary>
        public int Count
        {
            get
            {
                return list.Count;
            }
        }
        /// <summary>
        /// Получить базовый список
        /// </summary>
        /// <returns>Список</returns>
        public BindingList<JournalItem> Get()
        {
            return list;
        }
        /// <summary>
        /// Получить фильтрованый список
        /// </summary>
        /// <param name="_filter">Фильтр (Массив допустимых значений статуса)</param>
        /// <returns>Список фильтрованый</returns>
        public BindingList<JournalItem> Get(List<JournalStatus> _filter)
        {
            Fillter = _filter;
            listFillter.Clear();
            foreach (JournalItem item in list)
            {
                if (Fillter.Contains(item.Status))
                    listFillter.Add(item);
            }
            return listFillter;
        }
        public void Clear()
        {
            list.Clear();
            listFillter.Clear();
        }

        public void SaveToFile(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Stream st = new FileStream(path,FileMode.Create);
            bf.Serialize(st, this.list);
            st.Close();
        }
        public void LoadFromFile(string path)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Stream st = new FileStream(path, FileMode.Open);
            list =  (BindingList<JournalItem>)bf.Deserialize(st);
            if (list == null)
                list = new BindingList<JournalItem>();
            st.Close();
        }
    }
}

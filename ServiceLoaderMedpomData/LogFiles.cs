using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace ServiceLoaderMedpomData
{
    /// <summary>
    /// Лог файл
    /// </summary>
    [Serializable]
    [DataContract]
    public class LogFile
    {
        /// <summary>
        /// Поток файла
        /// </summary>
        [NonSerialized]
        [IgnoreDataMember]
        StreamWriter SW;
        [DataMember]
        string filename;

        public LogFile()
        {

        }
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="_filename">Путь к файлу</param>

        public LogFile(string _filename)
        {
            filename = _filename;
            SW = new StreamWriter(filename, false);
        }
        /// <summary>
        /// Записать с переходом на новую строку
        /// </summary>
        /// <param name="text">Текст для записи</param>
        public void WriteLn(string text)
        {
            SW?.WriteLine($"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] {text}");
        }
        /// <summary>
        /// Закрытие файла
        /// </summary>
        public void Close()
        {
            if (SW == null) return;
            SW.Close();            
            SW = null;
        }
        /// <summary>
        /// Открыть файл для добавления
        /// </summary>
        public void Append()
        {
            if (SW != null) return ;
            Stream st = File.Open(filename, FileMode.Append, FileAccess.Write);
            SW = new StreamWriter(st);
        }

        public void Reset()
        {
            if (SW != null) return;
            Stream st = File.Open(filename, FileMode.Create, FileAccess.Write);
            SW = new StreamWriter(st);
        }


        public string FileName => Path.GetFileName(filename);

        public string FilePath
        {
            get
            {
                return filename;
            }
            set
            {
                filename = value;
            }
        }
    }
}

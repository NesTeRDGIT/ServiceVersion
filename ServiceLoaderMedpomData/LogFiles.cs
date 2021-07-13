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
        private Encoding encoding = Encoding.GetEncoding("Windows-1251");
        /// <summary>
        /// Поток файла
        /// </summary>
        [NonSerialized] [IgnoreDataMember] private StreamWriter SW;
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
            SW = new StreamWriter(filename, false, encoding);
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
            lock (SW)
            {
                SW.Close();
                SW = null;
            }
        }
        /// <summary>
        /// Открыть файл для добавления
        /// </summary>
        public void Append()
        {
            if (SW != null) return ;
            Stream st = File.Open(filename, FileMode.Append, FileAccess.Write);
            SW = new StreamWriter(st, encoding);

        }

        public void Reset()
        {
            if (SW != null) return;
            Stream st = File.Open(filename, FileMode.Create, FileAccess.Write);
            SW = new StreamWriter(st, encoding);
        }


        public string FileName => Path.GetFileName(filename);

        public string FilePath
        {
            get => filename;
            set => filename = value;
        }
    }
}

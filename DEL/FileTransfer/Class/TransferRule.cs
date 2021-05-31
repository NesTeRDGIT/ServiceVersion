using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileTransfer.Class
{
    [Serializable]
    public class TransferRule
    {
        public bool onStart { get; set; }
        /// <summary>
        /// Директория источник
        /// </summary>
        public string PathSource { get; set; }
        /// <summary>
        /// Директория назначения
        /// </summary>
        public string PathDestination { get; set; }
        /// <summary>
        /// Период переноса
        /// </summary>
        public int TimeOut { get; set; }
        /// <summary>
        /// Правила(регулярные выражения)
        /// </summary>
        public List<string> Rule { get; set; }
        /// <summary>
        /// Пользователь
        /// </summary>
        public UserWin User { get; set; } = new UserWin();
    }

    [Serializable]
    public class UserWin
    {

        public string UserName { get; set; }
        public string Domain { get; set; }
        private string _Password;
        [XmlIgnore]
        public string Password
        {
            get { return _Password;}
            set
            {
                _Password = value;
                _ProtectPassword = !string.IsNullOrEmpty(value) ? ProtectStr.ProtectString(value) : "";
            }
        }

        string _ProtectPassword;

        public string ProtectPassword
        {
            get { return _ProtectPassword; }
            set
            {
                _ProtectPassword = value;
                _Password = !string.IsNullOrEmpty(value) ? ProtectStr.UnprotectString(_ProtectPassword) : "";
            }
        }


        public string FULL_USER => $"{UserName}{(string.IsNullOrEmpty(Domain) ? "" : $"@{Domain}")}";
    }
}

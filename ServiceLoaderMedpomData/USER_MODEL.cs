using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLoaderMedpomData
{
    public class METHOD
    {
        public static List<METHOD> Get(DataRow[] rows)
        {
            return rows.Select(Get).ToList();
        }
        public static METHOD Get(DataRow row)
        {
            try
            {
                var item = new METHOD
                {
                    COMENT = row["COMENT"].ToString(),
                    NAME = row["NAME"].ToString(),
                    ID = Convert.ToInt32(row["ID"])
                };
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения METHOD: {ex.Message}", ex);
            }
        }
        public int ID { get; set; }
        public string NAME { get; set; }
        public string COMENT { get; set; }
        public bool CHECKED { get; set; }
    }
    public class USERS : INotifyPropertyChanged
    {
        public static List<USERS> Get(DataRow[] rows)
        {
            return rows.Select(r => Get(r)).ToList();
        }

        public static USERS Get(DataRow row)
        {
            try
            {
                var item = new USERS
                {
                    ID = Convert.ToInt32(row["ID"]),
                    NAME = row["NAME"].ToString(),
                    PASS = row["PASS"].ToString()
                };
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения USERS: {ex.Message}", ex);
            }
        }

        public void CopyTo(USERS user)
        {
            user.ID = this.ID;
            user.NAME = this.NAME;
            user.PASS = this.PASS;
            user.ROLES = new List<int>();
            user.ROLES.AddRange(this.ROLES);
        }
        public int ID { get; set; }
        public string NAME { get; set; }
        public string PASS { get; set; }
        public List<int> ROLES { get; set; } = new List<int>();
        private bool _Modyfi;
        public bool Modyfi
        {
            get { return _Modyfi; }
            set { _Modyfi = value; OnPropertyChanged("Modyfi"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class ROLES
    {
        public static List<ROLES> Get(DataRow[] rows)
        {
            return rows.Select(Get).ToList();
        }
        public static ROLES Get(DataRow row)
        {
            try
            {
                var item = new ROLES
                {
                    ROLE_COMMENT = row["ROLE_COMMENT"].ToString(),
                    ROLE_NAME = row["ROLE_NAME"].ToString(),
                    ID = Convert.ToInt32(row["ID"])
                };
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения ROLES: {ex.Message}", ex);
            }
        }
        public string ROLE_NAME { get; set; }
        public string ROLE_COMMENT { get; set; }
        public int ID { get; set; }
        public List<int> METHOD { get; set; } = new List<int>();
        public bool IsModify { get; set; }

        public void RemoveMethod(int ID)
        {
            METHOD.Remove(ID);
            IsModify = true;
        }

        public void AddMethod(int ID)
        {
            METHOD.Add(ID);
            IsModify = true;
        }

    }

    public enum TypeEdit
    {
        New = 0,
        Update = 1,
        Delete = 2
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClientServiceWPF.Class
{
    public static class ProtectStr
    {
        static byte[] s_aditionalEntropy = { 10, 5, 8, 4, 9 };
        public static string ProtectString(string str)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(str);
                return Convert.ToBase64String(ProtectedData.Protect(data, s_aditionalEntropy, DataProtectionScope.CurrentUser));
            }
            catch (Exception)
            {
                return "";
            }
        }


        public static string UnprotectString(string str)
        {
            try
            {
                var data = Convert.FromBase64String(str);
                return Encoding.UTF8.GetString(ProtectedData.Unprotect(data, s_aditionalEntropy, DataProtectionScope.CurrentUser));
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}

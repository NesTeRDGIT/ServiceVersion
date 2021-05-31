using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileTransferContract.Class
{
    public static class FileMover
    {
        [DllImport("advapi32.DLL", SetLastError = true)]
        private static extern int LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);
        public static void MoveFiles(TransferRule tr, ILogger logger)
        {
            WindowsImpersonationContext context = null;
            if (!string.IsNullOrEmpty(tr.User.UserName))
            {
                AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
                var token = default(IntPtr);
                if (LogonUser(tr.User.UserName, tr.User.Domain, tr.User.Password, 2, 0, ref token) != 0)
                {
                    var identity = new WindowsIdentity(token);
                    context = identity.Impersonate();
                }
            }
            var files = Directory.GetFiles(tr.PathSource);
            foreach (var file in  files)
            {
                var fileName = Path.GetFileName(file);
                if (FileMatch(fileName, tr.Rule))
                {
                    var pathDist = Path.Combine(tr.PathDestination, fileName);
                    if (!File.Exists(pathDist))
                        File.Move(file, pathDist);
                    logger?.Add($"Перенос файла {fileName}", false);
                }
                   
            }
            context?.Undo();
        }

        static bool FileMatch(string FileName, IEnumerable<string> match)
        {
            return match.Any(pattern => Regex.IsMatch(FileName, pattern));
        }

    }
}

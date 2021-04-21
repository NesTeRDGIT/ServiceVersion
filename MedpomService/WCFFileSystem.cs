using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MedpomService
{
    public partial class WcfInterface
    {
        public string[] GetFolderLocal(string path)
        {
            try
            {
                return Directory.GetDirectories(path);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public string[] GetFilesLocal(string path, string pattern)
        {
            try
            {
                return Directory.GetFiles(path, pattern);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public string[] GetLocalDisk()
        {
            return Environment.GetLogicalDrives();
        }
    }
}

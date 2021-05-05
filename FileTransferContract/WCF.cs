using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using  FileTransferContract.Class;
namespace FileTransferContract
{
    public interface IWCFContract
    {
        List<LogItem> GetLOG(int Count=50);
        void AddRule(TransferRule rule);
        void RemoveRule(string[] GUID);
        void ReplaceRule(string GUID, TransferRule rule);
        List<ARule> GetRule();
        void StartRule(string[] GUID);
        void StopRule(string[] GUID);
    }
}

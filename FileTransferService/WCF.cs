using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransferContract;
using FileTransferContract.Class;

namespace FileTransferService
{
    public class WCF:IWCFContract
    {
        private ILogger logger;
        private IRuleManager ruleManager;
        public WCF(ILogger logger, IRuleManager ruleManager)
        {
            this.logger = logger;
            this.ruleManager = ruleManager;
        }

        public List<LogItem> GetLOG(int Count = 50)
        {
            return  logger?.GetTop(Count);
        }

        public void AddRule(TransferRule rule)
        {
            ruleManager?.AddRule(rule);
        }

        public void RemoveRule(string[] GUID)
        {
            ruleManager?.RemoveRule(GUID);
        }

        public void ReplaceRule(string GUID, TransferRule rule)
        {
            ruleManager?.ReplaceRule(GUID, rule);
        }

        public List<ARule> GetRule()
        {
           return  ruleManager?.GetRule();
        }

        public void StartRule(string[] GUID)
        {
            ruleManager?.StartRule(GUID);
        }

        public void StopRule(string[] GUID)
        {
            ruleManager?.StopRule(GUID);
        }

       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferContract.Class
{


    public interface ILogger
    {
        void Add(string text, bool isError);
        List<LogItem> GetTop(int Count=50);
    }

    public class LogItem
    {
        public string Text { get; set; }
        public bool IsError { get; set; }
        public DateTime TimeGenerated { get; set; }
    }

}

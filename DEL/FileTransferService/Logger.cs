using FileTransferContract.Class;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FileTransferService
{
   
    public class LoggerEventLog : ILogger
    {
        private string nameLog;

        public LoggerEventLog(string nameLog)
        {
            this.nameLog = nameLog;
        }
        public void Add(string text, bool isError)
        {
            try
            {
                if (!EventLog.SourceExists(nameLog))
                {
                    EventLog.CreateEventSource(nameLog, nameLog);
                }
                var log = EventLogEntryType.Information;
                if (isError)
                    log = EventLogEntryType.Error;
                var el = new EventLog { Source = nameLog };
                el.WriteEntry(text, log);
            }
            catch
            {
                // ignored
            }
        }

        public List<LogItem> GetTop(int Count = 50)
        {
            var rez = new List<LogItem>();

            if (EventLog.Exists(nameLog))
            {
                var EventLog1 = new EventLog {Source = nameLog};

                for (var i = 0; i < Count; i++)
                {
                    if (i > EventLog1.Entries.Count - 1)
                        continue;
                    var entry = EventLog1.Entries[EventLog1.Entries.Count - 1 - i];
                    var item = new LogItem {Text = entry.Message, TimeGenerated = entry.TimeGenerated};

                    switch (entry.EntryType)
                    {
                        case EventLogEntryType.Error:
                            item.IsError = true;
                            break;
                        default:
                            item.IsError = false;
                            break;
                    }

                    rez.Add(item);
                }
            }

            return rez;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedpomService
{
    public enum LogType
    {
        Error = 0,
        Information = 1,
        Warning = 2
    }
    public static class Logger
    {
        private  static string nameLog = "MedpomServiceLog";
        public static void AddLog(string log, LogType type)
        {
            try
            {
             
                if (!EventLog.SourceExists(nameLog))
                {
                    EventLog.CreateEventSource(nameLog, nameLog);
                }
                var el = new EventLog {Source = nameLog};
                el.WriteEntry(log, LogTypeToLogEntryType(type));
            }

            catch
            {
                // ignored
            }
        }

        private static EventLogEntryType LogTypeToLogEntryType(LogType lt)
        {
            switch (lt)
            {
                case LogType.Error: return EventLogEntryType.Error;
                case LogType.Information: return EventLogEntryType.Information;
                case LogType.Warning:return EventLogEntryType.Warning;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lt), lt, null);
            }
        }
    }
}

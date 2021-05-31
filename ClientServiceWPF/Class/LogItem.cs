namespace ClientServiceWPF.Class
{
    public enum LogType
    {
        Info = 0,
        Error = 1,
        Warning = 2
    }
    public class LogItem
    {
        public LogItem(LogType _Type, string _Message)
        {
            Type = _Type;
            Message = _Message;
        }
        public string Message { get; set; }
        public LogType Type { get; set; }
    }
}

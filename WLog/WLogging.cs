namespace Wattmate_Site.WLog
{
    public class WLogging
    {
        public static WLogSession Session { get; set; }

        public static void Initialize()
        {
            Session = new WLogSession();
        }

        public static void Log(string log)
        {
            Session.AddToLog(log);
        }

        public static List<string> GetLogs()
        {
            return Session.Logs;
        }
    }
}

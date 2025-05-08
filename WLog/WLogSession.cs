using System.Security.Cryptography.X509Certificates;

namespace Wattmate_Site.WLog
{
    public class WLogSession
    {
        public List<string> Logs { get; set; } = new();

        public WLogSession()
        {
            
        }

        public void AddToLog(string log)
        {
            Logs.Add(log);
            if(Logs.Count > 200)
            {
                Logs.RemoveAt(0);
            }
        }
    }
}

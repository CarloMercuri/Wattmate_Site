using System.Data;

namespace Wattmate_Site.WDatabase
{
    public class DatabaseQueryResponse
    {
        public bool Success { get; set; }
        public DateTime TimeStamp { get; set; }
        public DataTable Data { get; set; } = new DataTable();
        public string ResponseMessage { get; set; }
        public Exception Exception { get; set; }
        public int AffectedRows { get; set; }
    }
}

namespace Wattmate_Site.WDatabase.QueriesModels
{
    public abstract class DatabaseQueryResult
    {
        public DatabaseQueryResultCode Result { get; set; }
        public string Message { get; set; }
    }
}

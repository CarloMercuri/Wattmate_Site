namespace Wattmate_Site.WDatabase
{
    public static class WDatabaseProcessor
    {
        public static WDatabaseConnection GetDatabaseConnector()
        {
            return new WDatabaseConnection();
        }
    }
}

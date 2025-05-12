using Wattmate_Site.DataModels;

namespace Wattmate_Site.Controllers.ViewModels
{
    public class PricesViewModel
    {
        public List<ElectricityPriceUnit> Prices { get; set; } = new List<ElectricityPriceUnit>();
    }
}

namespace Wattmate_Site.DataModels
{
    public class PeakTroughPoint
    {
        public DateTime Time { get; set; }
        public float Temperature { get; set; }

        public PeakTroughPoint()
        {
                
        }

        public PeakTroughPoint(DateTime time, float temp)
        {
            this.Time = time;
            this.Temperature = temp;
        }
    }
}

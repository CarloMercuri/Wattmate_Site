using System.Diagnostics;

namespace Wattmate_Site.DataModels
{
    [DebuggerDisplay("{DebugDisplay}")]
    public class TelemetryWaveGroup
    {
        public PeakTroughPoint PointA { get; set; }
        public PeakTroughPoint PointB { get; set; }

        public string DebugDisplay => $"A: {PointA.Temperature} ({PointA.Time.ToString("HH:mm")})  -   B: {PointB.Temperature} ({PointB.Time.ToString("HH:mm")})";


    }
}

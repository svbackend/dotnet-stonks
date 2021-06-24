using System;

namespace Stonks.Shared.Models
{
    public class PolygonChartResponse
    {
        public string Ticker { get; set; }

        public long QueryCount { get; set; }

        public long ResultsCount { get; set; }

        public bool Adjusted { get; set; }

        public ChartItem[] Results { get; set; }

        public string Status { get; set; }

        public string RequestId { get; set; }

        public long Count { get; set; }
    }

    public class ChartItem
    {
        public long V { get; set; }

        public double Vw { get; set; }

        public double O { get; set; }

        public double C { get; set; }

        public double H { get; set; }

        public double L { get; set; }

        public long T { get; set; }

        public long N { get; set; }

        public static ChartItem CreateByChartOhlcItem(StockChartOhlcItem i)
        {
            return new()
            {
                V = i.V,
                Vw = i.Vw,
                O = i.O,
                C = i.C,
                H = i.H,
                L = i.L,
                T = ((DateTimeOffset) i.Timestamp).ToUnixTimeSeconds(),
                N = i.N
            };
        }
    }
}
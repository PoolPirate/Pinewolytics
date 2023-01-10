using System.Globalization;

namespace Pinewolytics.Models.DTOs.Optimism;

public class OptimismOPHolderMetricsDTO : IFlipsideObject<OptimismOPHolderMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required ulong HolderCount { get; init; }

    public required double MedianBalance { get; init; }
    public required double AverageBalance { get; init; }
    public required double MinimumBalance { get; init; }
    public required double MaximumBalance { get; init; }
    public required double Percentile20thBalance { get; init; }
    public required double Percentile80thBalance { get; init; }

    public static OptimismOPHolderMetricsDTO Parse(string[] rawValues)
    {
        return new OptimismOPHolderMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            HolderCount = ulong.Parse(rawValues[1]),
            MedianBalance = double.Parse(rawValues[2], NumberStyles.Float),
            AverageBalance = double.Parse(rawValues[3], NumberStyles.Float),
            MinimumBalance = double.Parse(rawValues[4], NumberStyles.Float),
            MaximumBalance = double.Parse(rawValues[5], NumberStyles.Float),
            Percentile20thBalance = double.Parse(rawValues[6], NumberStyles.Float),
            Percentile80thBalance = double.Parse(rawValues[7], NumberStyles.Float),
        };
    }
}

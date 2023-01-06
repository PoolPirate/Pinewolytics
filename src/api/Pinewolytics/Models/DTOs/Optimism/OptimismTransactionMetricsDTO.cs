using System.Globalization;

namespace Pinewolytics.Models.DTOs.Terra;

public class OptimismTransactionMetricsDTO : IFlipsideObject<OptimismTransactionMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required long TransactionCount { get; init; }
    public required long BlockCount { get; init; }

    public static OptimismTransactionMetricsDTO Parse(string[] rawValues)
    {
        return new OptimismTransactionMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            TransactionCount = long.Parse(rawValues[1]),
            BlockCount = long.Parse(rawValues[2])
        };
    }
}
using System.Globalization;

namespace Pinewolytics.Models.DTOs.Terra;

public class TerraTransactionMetricsDTO : IFlipsideObject<TerraTransactionMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }
    
    public required double MinimumFee { get; init; }
    public required double AverageFee { get; init; }
    public required double MedianFee { get; init; }
    public required double MaximumFee { get; init; }
    public required double TotalFee { get; init; }
    
    public required long TransactionCount { get; init; }
    public required long BlockCount { get; init; }

    public static TerraTransactionMetricsDTO Parse(string[] rawValues)
    {
        return new TerraTransactionMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            MinimumFee = double.Parse(rawValues[1], NumberStyles.Float),
            AverageFee = double.Parse(rawValues[2], NumberStyles.Float),
            MedianFee = double.Parse(rawValues[3], NumberStyles.Float),
            MaximumFee = double.Parse(rawValues[4], NumberStyles.Float),
            TotalFee = double.Parse(rawValues[5], NumberStyles.Float),

            TransactionCount = long.Parse(rawValues[6]),
            BlockCount = long.Parse(rawValues[7])
        };
    }
}

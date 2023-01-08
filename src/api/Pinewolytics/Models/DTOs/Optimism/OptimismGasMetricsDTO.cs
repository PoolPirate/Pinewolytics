using Pinewolytics.Models;
using System.Globalization;

namespace Pinewolytics.Models.DTOs.Optimism;

public class OptimismGasMetricsDTO : IFlipsideObject<OptimismGasMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required double TotalL2GasFee { get; init; }
    public required double TotalL1GasFee { get; init; }

    public static OptimismGasMetricsDTO Parse(string[] rawValues)
    {
        return new OptimismGasMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            TotalL2GasFee = double.Parse(rawValues[1], NumberStyles.Float),
            TotalL1GasFee = double.Parse(rawValues[2], NumberStyles.Float),
        };
    }
}

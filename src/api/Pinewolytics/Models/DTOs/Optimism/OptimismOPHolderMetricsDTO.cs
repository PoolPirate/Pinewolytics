namespace Pinewolytics.Models.DTOs.Optimism;

public class OptimismOPHolderMetricsDTO : IFlipsideObject<OptimismOPHolderMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required ulong HolderCount { get; init; }

    public static OptimismOPHolderMetricsDTO Parse(string[] rawValues)
    {
        return new OptimismOPHolderMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            HolderCount = ulong.Parse(rawValues[1])
        };
    }
}

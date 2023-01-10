namespace Pinewolytics.Models.DTOs.Optimism;

public class OptimismDAppUsageDTO : IFlipsideObject<OptimismDAppUsageDTO>
{
    public required string ProjectName { get; init; }
    public required ulong TxCount { get; init; }
    public required ulong AddressCount { get; init; }

    public static OptimismDAppUsageDTO Parse(string[] rawValues)
    {
        return new OptimismDAppUsageDTO()
        {
            ProjectName = rawValues[0],
            TxCount = ulong.Parse(rawValues[1]),
            AddressCount = ulong.Parse(rawValues[2])
        };
    }
}

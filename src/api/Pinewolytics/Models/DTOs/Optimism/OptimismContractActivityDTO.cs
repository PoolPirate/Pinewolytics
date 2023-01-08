namespace Pinewolytics.Models.DTOs.Optimism;

public class OptimismContractActivityDTO : IFlipsideObject<OptimismContractActivityDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required ulong DailyCalledContracts { get; init; }
    public required ulong WeeklyCalledContracts { get; init; }

    public required ulong DailyUsedContracts { get; init; }
    public required ulong WeeklyUsedContracts { get; init; }

    public static OptimismContractActivityDTO Parse(string[] rawValues)
    {
        return new OptimismContractActivityDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            DailyCalledContracts = ulong.Parse(rawValues[1]),
            WeeklyCalledContracts = ulong.Parse(rawValues[2]),
            DailyUsedContracts = ulong.Parse(rawValues[3]),
            WeeklyUsedContracts = ulong.Parse(rawValues[4]),
        };
    }
}

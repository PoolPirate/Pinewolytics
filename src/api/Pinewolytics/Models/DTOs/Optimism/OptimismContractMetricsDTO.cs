namespace Pinewolytics.Models.DTOs.Terra;

public class OptimismContractMetricsDTO : IFlipsideObject<OptimismContractMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }
    public required long NewContracts { get; init; }
    public required long TotalContracts { get; init; }

    public required long ActiveDevelopers { get; init; }
    public required long TotalDevelopers { get; init; }

    public static OptimismContractMetricsDTO Parse(string[] rawValues)
    {
        return new OptimismContractMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            NewContracts = long.Parse(rawValues[1]),
            TotalContracts = long.Parse(rawValues[2]),
            ActiveDevelopers = long.Parse(rawValues[3]),
            TotalDevelopers = long.Parse(rawValues[4]),
        };
    }
}

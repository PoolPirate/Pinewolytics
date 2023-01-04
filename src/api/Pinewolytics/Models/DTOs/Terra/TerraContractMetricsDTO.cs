namespace Pinewolytics.Models.DTOs.Terra;

public class TerraContractMetricsDTO : IFlipsideObject<TerraContractMetricsDTO>
{
    public required DateTimeOffset Timestamp { get; init; }
    public required long NewContracts { get; init; }
    public required long TotalContracts { get; init; }

    public static TerraContractMetricsDTO Parse(string[] rawValues)
    {
        return new TerraContractMetricsDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            NewContracts= long.Parse(rawValues[1]),
            TotalContracts= long.Parse(rawValues[2]),
        };
    }
}

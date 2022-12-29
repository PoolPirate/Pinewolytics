namespace Pinewolytics.Models.DTOs;

public class OsmosisLPJoinDTO : IFlipsideObject<OsmosisLPJoinDTO>
{
    public required DateTimeOffset BlockTimestamp { get; init; }
    public required string LiquidityProviderAddress { get; init; }
    public required double Amount { get; init; }

    public static OsmosisLPJoinDTO Parse(string[] rawValues)
    {
        return new OsmosisLPJoinDTO()
        {
            BlockTimestamp = DateTimeOffset.Parse(rawValues[0]),
            LiquidityProviderAddress = rawValues[1],
            Amount = double.Parse(rawValues[2])
        };
    }
}

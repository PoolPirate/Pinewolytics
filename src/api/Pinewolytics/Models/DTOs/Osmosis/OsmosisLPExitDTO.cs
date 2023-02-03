namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisLPExitDTO : IFlipsideObject<OsmosisLPExitDTO>
{
    public required DateTimeOffset BlockTimestamp { get; init; }
    public required string LiquidityProviderAddress { get; init; }
    public required double Amount { get; init; }

    public static OsmosisLPExitDTO Parse(string[] rawValues)
    {
        return new OsmosisLPExitDTO()
        {
            BlockTimestamp = DateTimeOffset.Parse(rawValues[0]),
            LiquidityProviderAddress = rawValues[1],
            Amount = double.Parse(rawValues[2])
        };
    }
}

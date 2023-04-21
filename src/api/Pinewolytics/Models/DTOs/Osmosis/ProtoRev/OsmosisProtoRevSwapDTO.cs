namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisProtoRevSwapDTO
{
    public required DenominatedAmountDTO Profit { get; init; }
    public required decimal ProfitUSD { get; init; }
}

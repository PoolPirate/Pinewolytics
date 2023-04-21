namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisTokenInfoDTO
{
    public required string Denom { get; init; }
    public required string Symbol { get; init; }
    public required decimal Price { get; init; }
    public required uint Exponent { get; init; }
}

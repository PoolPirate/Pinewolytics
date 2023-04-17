namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisProtoRevRouteStatisticsDTO
{
    public required DenominatedAmountDTO[] Profits { get; init; }
    public required ulong NumberOfTrades { get; init; }
    public required string[] Route { get; init; }
}

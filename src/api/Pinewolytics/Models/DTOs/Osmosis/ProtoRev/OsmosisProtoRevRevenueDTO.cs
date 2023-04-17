namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisProtoRevRevenueDTO : IFlipsideObject<OsmosisProtoRevRevenueDTO>
{
    public required DateTimeOffset Date { get; init; }
    public required string Currency { get; init; }
    public required string Symbol { get; init; }

    public required decimal TotalAmount { get; init; }
    public required decimal TotalAmountUSD { get; init; }

    public static OsmosisProtoRevRevenueDTO Parse(string[] rawValues)
    {
        return new OsmosisProtoRevRevenueDTO()
        {
            Date = DateTimeOffset.Parse(rawValues[0]),
            Currency = rawValues[1],
            Symbol = rawValues[2],
            TotalAmount = decimal.Parse(rawValues[3]),
            TotalAmountUSD = decimal.Parse(rawValues[4])
        };
    }
}

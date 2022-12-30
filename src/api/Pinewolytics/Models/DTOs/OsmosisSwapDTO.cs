namespace Pinewolytics.Models.DTOs;

public class OsmosisSwapDTO : IFlipsideObject<OsmosisSwapDTO>
{
    public required DateTimeOffset BlockTimestamp { get; init; }

    public required string Trader { get; init; }

    public required double FromAmount { get; init; }
    public required string FromCurrency { get; init; }

    public required double ToAmount { get; init; }
    public required string ToCurrency { get; init; }

    public static OsmosisSwapDTO Parse(string[] rawValues)
    {
        return new OsmosisSwapDTO()
        {
            BlockTimestamp = DateTimeOffset.Parse(rawValues[0]),
            Trader = rawValues[1],
            FromAmount = double.Parse(rawValues[2]),
            FromCurrency = rawValues[3],
            ToAmount = double.Parse(rawValues[4]),
            ToCurrency = rawValues[5]
        };
    }
}

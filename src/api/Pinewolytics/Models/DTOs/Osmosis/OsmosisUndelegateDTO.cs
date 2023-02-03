namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisUndelegateDTO : IFlipsideObject<OsmosisUndelegateDTO>
{
    public required DateTimeOffset BlockTimestamp { get; init; }
    public required string Address { get; init; }
    public required double Amount { get; init; }

    public static OsmosisUndelegateDTO Parse(string[] rawValues)
    {
        return new OsmosisUndelegateDTO()
        {
            BlockTimestamp = DateTimeOffset.Parse(rawValues[0]),
            Address = rawValues[1],
            Amount = double.Parse(rawValues[2])
        };
    }
}

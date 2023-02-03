namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisDelegateDTO : IFlipsideObject<OsmosisDelegateDTO>
{
    public required DateTimeOffset BlockTimestamp { get; init; }
    public required string Address { get; init; }
    public required double Amount { get; init; }

    public static OsmosisDelegateDTO Parse(string[] rawValues)
    {
        return new OsmosisDelegateDTO()
        {
            BlockTimestamp = DateTimeOffset.Parse(rawValues[0]),
            Address = rawValues[1],
            Amount = double.Parse(rawValues[2])
        };
    }
}

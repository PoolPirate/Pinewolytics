namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisTotalDelegationsDTO : IFlipsideObject<OsmosisTotalDelegationsDTO>
{
    public required DateTimeOffset Date { get; init; }
    public required double TotalDelegated { get; init; }
    public required double TotalUndelegated { get; init; }

    public static OsmosisTotalDelegationsDTO Parse(string[] rawValues)
    {
        return new OsmosisTotalDelegationsDTO()
        {
            Date = DateTimeOffset.Parse(rawValues[0]),
            TotalDelegated = double.Parse(rawValues[1]),
            TotalUndelegated = double.Parse(rawValues[2])
        };
    }
}

namespace Pinewolytics.Models.DTOs;

public class OsmosisTransferDTO : IFlipsideObject<OsmosisTransferDTO>
{
    public required DateTimeOffset BlockTimestamp { get; init; }
    public required decimal Amount { get; init; }

    public required string Receiver { get; init; }
    public required string Sender { get; init; }

    public static OsmosisTransferDTO Parse(string[] rawValues)
    {
        return new OsmosisTransferDTO()
        {
            BlockTimestamp = DateTimeOffset.Parse(rawValues[0]),
            Amount = decimal.Parse(rawValues[1]),
            Receiver = rawValues[2],
            Sender = rawValues[3],
        };
    }
}

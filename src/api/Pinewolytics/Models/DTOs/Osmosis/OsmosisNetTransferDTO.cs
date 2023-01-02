namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisNetTransferDTO : IFlipsideObject<OsmosisNetTransferDTO>
{
    public required string Sender { get; init; }
    public required string Receiver { get; init; }

    public required double Amount { get; init; }

    public static OsmosisNetTransferDTO Parse(string[] rawValues)
    {
        return new OsmosisNetTransferDTO()
        {
            Amount = double.Parse(rawValues[0]),
            Sender = rawValues[1],
            Receiver = rawValues[2]
        };
    }
}

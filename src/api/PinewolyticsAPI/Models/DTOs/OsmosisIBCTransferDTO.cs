namespace Pinewolytics.Models.DTOs;

public class OsmosisIBCTransferDTO : IFlipsideObject<OsmosisIBCTransferDTO>
{
    public required DateTimeOffset BlockTimestamp { get; init; }
    public required decimal Amount { get; init; }

    public required string Receiver { get; init; }
    public required string Sender { get; init; }

    public required IBCDirection TransferType { get; init; }

    public static OsmosisIBCTransferDTO Parse(string[] rawValues)
    {
        return new OsmosisIBCTransferDTO()
        {
            BlockTimestamp = DateTimeOffset.Parse(rawValues[0]),
            Amount = decimal.Parse(rawValues[1]),
            Receiver = rawValues[2],
            Sender = rawValues[3],
            TransferType = rawValues[4] == "IBC_TRANSFER_IN" ? IBCDirection.IBC_IN : IBCDirection.IBC_OUT,
        };
    }
}

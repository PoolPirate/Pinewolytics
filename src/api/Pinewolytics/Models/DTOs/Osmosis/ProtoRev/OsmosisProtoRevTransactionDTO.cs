using Pinewolytics.Models.DTOs.All;

namespace Pinewolytics.Models.DTOs.Osmosis.ProtoRev;

public class OsmosisProtoRevTransactionDTO : ITimestampedDTO
{
    public required string Hash { get; init; }
    public required ulong Height { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required int Index { get; init; }

    public required DenominatedAmountDTO[] Profits { get; init; }

    public DateTimeOffset GetTimestamp() 
        => Timestamp;
}

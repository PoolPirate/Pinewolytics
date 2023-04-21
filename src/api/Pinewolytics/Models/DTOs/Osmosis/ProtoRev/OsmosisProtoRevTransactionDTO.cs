using Pinewolytics.Models.DTOs.All;

namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisProtoRevTransactionDTO : ITimestampedDTO
{
    public required string Hash { get; init; }
    public required ulong Height { get; init; }
    public required int Index { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required OsmosisProtoRevSwapDTO[] Swaps { get; init; }

    public DateTimeOffset GetTimestamp()
    {
        return Timestamp;
    }
}

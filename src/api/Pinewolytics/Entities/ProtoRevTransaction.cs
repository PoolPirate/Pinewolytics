namespace Pinewolytics.Models.Entities;

public class ProtoRevTransaction
{
    public required Guid Id { get; init; }

    public required string Hash { get; init; }
    public required DateTimeOffset TimeStamp { get; init; }

    public required string TxFrom { get; init; }

    public virtual List<ProtoRevSwap> Swaps { get; init; } = null!; //Navigation Property
}

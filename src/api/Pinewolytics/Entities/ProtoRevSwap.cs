using System.Numerics;
using System.Text.Json.Serialization;

namespace Pinewolytics.Models.Entities;

public class ProtoRevSwap
{
    public required Guid Id { get; init; }
    public required string Currency { get; init; }
    public required BigInteger Profit { get; init; }
    public required decimal ProfitUSD { get; init; }

    public required Guid TransactionId { get; init; }
    public virtual ProtoRevTransaction Transaction { get; init; } = null!; //Navigation Property
}

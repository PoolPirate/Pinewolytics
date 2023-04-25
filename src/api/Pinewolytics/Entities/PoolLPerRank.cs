using System.Numerics;

namespace Pinewolytics.Entities;

public class PoolLPerRank
{
    public long Id { get; init; }
    public required Guid WalletRankingId { get; init; }

    public required int PoolId { get; init; }
    public required BigInteger GammBalance { get; init; }
    public required long Rank { get; init; }

    public virtual WalletRanking? WalletRanking { get; init; } //Navigation Property
}

namespace Pinewolytics.Entities;

public class WalletRanking
{
    public required Guid Id { get; init; }
    public required string Address { get; init; }
    public required long StakedRank { get; init; }
    public required decimal StakedAmount { get; init; }
    public required long BalanceRank { get; init; }
    public required decimal BalanceAmount { get; init; }

    public virtual List<PoolLPerRank>? LPerRanks { get; init; } //Navigation Property
}

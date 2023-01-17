namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisWalletPoolRankingDTO
{
    public ulong PoolId { get; init; }
    public double LPTokenBalance { get; init; }
    public long Rank { get; init; }
}

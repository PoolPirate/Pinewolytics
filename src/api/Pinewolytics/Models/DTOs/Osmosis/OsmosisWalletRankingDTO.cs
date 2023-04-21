using System.Globalization;
using System.Text.Json;

namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisWalletRankingDTO : IFlipsideObject<OsmosisWalletRankingDTO>
{
    public required string Address { get; init; }
    public required DateTimeOffset LastUpdatedAt { get; init; }
    public required decimal StakedAmount { get; init; }
    public required long StakedRank { get; init; }

    public required decimal BalanceAmount { get; set; }
    public required long BalanceRank { get; init; }

    public required OsmosisWalletPoolRankingDTO[] PoolRankings { get; init; }

    public static OsmosisWalletRankingDTO Parse(string[] rawValues)
    {
        ulong[] pids = JsonSerializer.Deserialize<ulong[]>(rawValues[6]) ?? throw new JsonException("Unexpected format");
        double[] lpTokenBalances = JsonSerializer.Deserialize<double[]>(rawValues[7]) ?? throw new JsonException("Unexpected format");
        long[] rankings = JsonSerializer.Deserialize<long[]>(rawValues[8]) ?? throw new JsonException("Unexpected format");

        decimal stakedAmount = decimal.Parse(rawValues[2], NumberStyles.Float);
        decimal balanceAmount = decimal.Parse(rawValues[4], NumberStyles.Float);

        return new OsmosisWalletRankingDTO()
        {
            Address = rawValues[0],
            LastUpdatedAt = DateTimeOffset.Parse(rawValues[1]),
            StakedAmount = Math.Max(0, stakedAmount),
            StakedRank = stakedAmount > 0 ? long.Parse(rawValues[3]) : -1,
            BalanceAmount = Math.Max(0, balanceAmount),
            BalanceRank = balanceAmount > 0 ? long.Parse(rawValues[5]) : -1,
            PoolRankings = pids.Zip(lpTokenBalances, rankings).Select(x => new OsmosisWalletPoolRankingDTO()
            {
                PoolId = Math.Max(0, x.First),
                LPTokenBalance = Math.Max(0, x.Second),
                Rank = x.Second < 0 ? -1 : x.Third,
            }).Where(x => x.Rank != -1).ToArray(),
        };
    }
}

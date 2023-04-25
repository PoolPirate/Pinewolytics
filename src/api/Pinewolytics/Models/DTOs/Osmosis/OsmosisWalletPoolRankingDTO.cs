using Pinewolytics.Utils;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisWalletPoolRankingDTO
{
    public int PoolId { get; init; }
    [JsonConverter(typeof(BigIntegerNumberConverter))]
    public BigInteger LPTokenBalance { get; init; }
    public long Rank { get; init; }
}

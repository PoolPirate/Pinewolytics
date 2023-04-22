using Pinewolytics.Models.DTOs.All;
using System.Text.Json;

namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisProtoRevTransactionDTO : ITimestampedDTO, IFlipsideObject<OsmosisProtoRevTransactionDTO>
{
    public required string Hash { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required string TxFrom { get; init; }
    public required OsmosisProtoRevSwapDTO[] Swaps { get; init; }

    public DateTimeOffset GetTimestamp()
    {
        return Timestamp;
    }

    public static OsmosisProtoRevTransactionDTO Parse(string[] rawValues)
    {
        string[] currencies = JsonSerializer.Deserialize<string[]>(rawValues[3]) ?? throw new JsonException("Unexpected format");
        ulong[] arbAmounts = JsonSerializer.Deserialize<ulong[]>(rawValues[4]) ?? throw new JsonException("Unexpected format");
        decimal[] arbAmountsUSD = JsonSerializer.Deserialize<decimal[]>(rawValues[5]) ?? throw new JsonException("Unexpected format");

        return new OsmosisProtoRevTransactionDTO()
        {
            Hash = rawValues[0],
            Timestamp = DateTimeOffset.Parse(rawValues[1]).UtcDateTime,
            TxFrom = rawValues[2],
            Swaps = Enumerable.Range(0, currencies.Length)
                .Select(x => new OsmosisProtoRevSwapDTO()
                {
                    Profit = new DenominatedAmountDTO()
                    {
                        Denom = currencies[x],
                        Amount = arbAmounts[x]
                    },
                    ProfitUSD = arbAmountsUSD[x]
                }).ToArray()
        };
    }
}

using System.Text.Json;

namespace Pinewolytics.Models.DTOs.Osmosis.ProtoRev;

public class OsmosisProtoRevAddressStatsDTO : IFlipsideObject<OsmosisProtoRevAddressStatsDTO>
{
    public required string Address { get; init; }
    public required OsmosisProtoRevSwapDTO[] AggregatedSwaps { get; init; }

    public static OsmosisProtoRevAddressStatsDTO Parse(string[] rawValues)
    {
        string[] currencies = JsonSerializer.Deserialize<string[]>(rawValues[1]) ?? throw new JsonException("Unexpected format");
        ulong[] arbAmounts = JsonSerializer.Deserialize<ulong[]>(rawValues[2]) ?? throw new JsonException("Unexpected format");
        decimal[] arbAmountsUSD = JsonSerializer.Deserialize<decimal[]>(rawValues[3]) ?? throw new JsonException("Unexpected format");

        return new OsmosisProtoRevAddressStatsDTO()
        {
            Address = rawValues[0],
            AggregatedSwaps = Enumerable.Range(0, currencies.Length)
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

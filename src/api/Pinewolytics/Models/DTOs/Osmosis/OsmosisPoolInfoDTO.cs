using System.Text.Json;

namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisPoolInfoDTO : IFlipsideObject<OsmosisPoolInfoDTO>
{
    public required int PoolId { get; init; }
    public required string[] Assets { get; init; }

    public static OsmosisPoolInfoDTO Parse(string[] rawValues)
    {
        return new OsmosisPoolInfoDTO()
        {
            PoolId = int.Parse(rawValues[0]),
            Assets = JsonSerializer.Deserialize<string[]>(rawValues[1]) ?? throw new Exception("Invalid JSON format")
        };
    }
}

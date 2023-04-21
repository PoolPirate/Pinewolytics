namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisEpochInfoDTO
{
    public required ulong CurrentEpoch { get; init; }
    public required ulong CurrentEpochStartHeight { get; init; }

    public required string Identifier { get; init; }
    public required DateTimeOffset StartTime { get; init; }
    public required DateTimeOffset CurrentEpochStartTime { get; init; }
    public required string Duration { get; init; }

    public OsmosisEpochInfoDTO()
    {
    }
}

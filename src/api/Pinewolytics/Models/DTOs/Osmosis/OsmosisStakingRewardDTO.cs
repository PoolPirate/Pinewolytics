namespace Pinewolytics.Models.DTOs.Osmosis;

public class OsmosisStakingRewardDTO
{
    public required DateTimeOffset Date { get; init; }
    public required string Address { get; init; }
    public required double Amount { get; init; }

    public static OsmosisStakingRewardDTO Parse(string[] rawValues)
    {
        return new OsmosisStakingRewardDTO()
        {
            Date = DateTimeOffset.Parse(rawValues[0]),
            Address = rawValues[1],
            Amount = double.Parse(rawValues[2])
        };
    }
}

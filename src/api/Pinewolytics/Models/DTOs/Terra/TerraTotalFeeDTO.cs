using System.Globalization;

namespace Pinewolytics.Models.DTOs.Terra;

public class TerraTotalFeeDTO : IFlipsideObject<TerraTotalFeeDTO>
{
    public required DateTimeOffset Timestamp { get; init; }

    public required double FeesSinceInception { get; init; }

    public static TerraTotalFeeDTO Parse(string[] rawValues)
    {
        return new TerraTotalFeeDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            FeesSinceInception = double.Parse(rawValues[1], NumberStyles.Float),
        };
    }
}

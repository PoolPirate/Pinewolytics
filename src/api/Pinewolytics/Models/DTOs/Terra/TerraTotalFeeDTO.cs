using System.Globalization;

namespace Pinewolytics.Models.DTOs.Terra;

public class TerraTotalFeeDTO : IFlipsideObject<TerraTotalFeeDTO>
{
    public required double FeesSinceInception { get; init; }

    public static TerraTotalFeeDTO Parse(string[] rawValues)
    {
        return new TerraTotalFeeDTO()
        {
            FeesSinceInception = double.Parse(rawValues[0], NumberStyles.Float),
        };
    }
}

namespace Pinewolytics.Models.DTOs.Terra;

public class TerraValidatorCountDTO : IFlipsideObject<TerraValidatorCountDTO>
{
    public required DateTimeOffset Timestamp { get; init; }
    public required int Count { get; init; }

    public static TerraValidatorCountDTO Parse(string[] rawValues)
    {
        return new TerraValidatorCountDTO()
        {
            Timestamp = DateTimeOffset.Parse(rawValues[0]),
            Count = int.Parse(rawValues[1])
        };
    }
}

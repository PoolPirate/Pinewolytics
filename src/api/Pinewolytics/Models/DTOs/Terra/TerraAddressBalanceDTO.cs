using System.Globalization;

namespace Pinewolytics.Models.DTOs.Terra;

public class TerraAddressBalanceDTO : IFlipsideObject<TerraAddressBalanceDTO>
{
    public required string Address { get; init; }
    public required double Balance { get; init; }

    public static TerraAddressBalanceDTO Parse(string[] rawValues)
    {
        return new TerraAddressBalanceDTO()
        {
            Address = rawValues[0],
            Balance = double.Parse(rawValues[1], NumberStyles.Float)
        };
    }
}

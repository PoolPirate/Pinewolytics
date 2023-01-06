using System.Globalization;

namespace Pinewolytics.Models.DTOs.Optimism;

public class OptimismAddressBalanceDTO : IFlipsideObject<OptimismAddressBalanceDTO>
{
    public required string Address { get; init; }
    public required double Balance { get; init; }

    public static OptimismAddressBalanceDTO Parse(string[] rawValues)
    {
        return new OptimismAddressBalanceDTO()
        {
            Address = rawValues[0],
            Balance = double.Parse(rawValues[1], NumberStyles.Float),
        };
    }
}

namespace Pinewolytics.Models.DTOs.All;

public class MarketDataDTO
{
    public required double Price { get; init; }
    public required double MarketCap { get; init; }

    public required double TotalSupply { get; init; }
    public required double CirculatingSupply { get; init; }
}

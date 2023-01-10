using Pinewolytics.Models.DTOs.All;

namespace Pinewolytics.Hubs.Optimism;

public interface IOptimismDataHubClient
{
    Task PeakBlockHeight(ulong blockHeight);
    Task MarketData(MarketDataDTO marketData);
    Task GasPrice(ulong l1GasPrice, ulong l2GasPrice);
}

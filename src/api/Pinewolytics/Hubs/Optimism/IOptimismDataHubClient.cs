namespace Pinewolytics.Hubs.Optimism;

public interface IOptimismDataHubClient
{
    Task PeakBlockHeight(ulong blockHeight);
    Task Price(double price);
    Task GasPrice(ulong l1GasPrice, ulong l2GasPrice);
}

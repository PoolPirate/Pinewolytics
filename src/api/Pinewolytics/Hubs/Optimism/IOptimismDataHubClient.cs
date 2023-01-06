namespace Pinewolytics.Hubs.Optimism;

public interface IOptimismDataHubClient
{
    Task PeakBlockHeight(ulong blockHeight);
    Task Price(double price);
}

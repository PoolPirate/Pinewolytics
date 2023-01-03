namespace Pinewolytics.Hubs.Optimism;

public interface IOptimismDataHubClient
{
    Task Price(double price);
}

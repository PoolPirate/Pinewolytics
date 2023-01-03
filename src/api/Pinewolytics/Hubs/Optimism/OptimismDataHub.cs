namespace Pinewolytics.Hubs.Optimism;

public class OptimismDataHub : DataHub<OptimismDataClient, IOptimismDataHubClient>
{
    public OptimismDataHub(OptimismDataClient client)
        : base(client)
    {
    }
}

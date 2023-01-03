using Common.Services;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Hubs.Optimism;

public class OptimismDataClient : BaseDataClient<OptimismDataHub, IOptimismDataHubClient>
{
    [Inject]
    private readonly CoinGeckoClient CoinGeckoClient = null!;

    [RealtimeValue(20 * SECONDS, nameof(IOptimismDataHubClient.Price))]
    private async Task<double> LoadPriceAsync()
    {
        return await CoinGeckoClient.GetOPPriceAsync();
    }
}

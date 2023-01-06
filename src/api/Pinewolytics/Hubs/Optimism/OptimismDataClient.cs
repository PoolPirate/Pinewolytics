using Common.Services;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Hubs.Optimism;

public class OptimismDataClient : BaseDataClient<OptimismDataHub, IOptimismDataHubClient>
{
    [Inject]
    private readonly CoinGeckoClient CoinGeckoClient = null!;

    [Inject]
    private readonly OptimismRpcClient OptimismRpcClient = null!;

    [RealtimeValue(20 * SECONDS, nameof(IOptimismDataHubClient.Price))]
    private async Task<double> LoadPriceAsync()
    {
        return await CoinGeckoClient.GetOPPriceAsync();
    }

    [RealtimeValue(SECONDS, nameof(IOptimismDataHubClient.PeakBlockHeight))]
    private async Task<double> LoadPeakBlockHeightAsync()
    {
        return await OptimismRpcClient.GetPeakBlockHeightAsync();
    }

    [RealtimeValue(10 * SECONDS, nameof(IOptimismDataHubClient.GasPrice))]
    private async Task<(ulong, ulong)> LoadGasPricesAsync()
    {
        return await OptimismRpcClient.GetGasPricesAsync();
    }
}

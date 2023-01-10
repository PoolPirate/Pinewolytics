using Common.Services;
using Pinewolytics.Models.DTOs.All;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Hubs.Optimism;

public class OptimismDataClient : BaseDataClient<OptimismDataHub, IOptimismDataHubClient>
{
    [Inject]
    private readonly CoinGeckoClient CoinGeckoClient = null!;

    [Inject]
    private readonly OptimismRpcClient OptimismRpcClient = null!;

    [RealtimeValue(20 * SECONDS, nameof(IOptimismDataHubClient.MarketData))]
    private async Task<MarketDataDTO> LoadPriceAsync()
    {
        return await CoinGeckoClient.GetOPMarketDataDTOAsync();
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

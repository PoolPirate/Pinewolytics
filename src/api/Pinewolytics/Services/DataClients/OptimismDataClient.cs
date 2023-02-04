using Common.Services;
using Pinewolytics.Models.DTOs.All;
using Pinewolytics.Models.DTOs.Optimism;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Services.DataClients;

public class OptimismDataClient : BaseDataClient
{
    [Inject]
    private readonly CoinGeckoClient CoinGeckoClient = null!;

    [Inject]
    private readonly OptimismRpcClient OptimismRpcClient = null!;

    [RealtimeValue("Optimism-MarketData", 20 * SECONDS)]
    private async Task<MarketDataDTO> LoadMarketDataAsync()
    {
        return await CoinGeckoClient.GetOPMarketDataDTOAsync();
    }

    [RealtimeValue("Optimism-Block-Height", SECONDS)]
    private async Task<double> LoadPeakBlockHeightAsync()
    {
        return await OptimismRpcClient.GetPeakBlockHeightAsync();
    }

    [RealtimeValue("Optimism-Gas-Prices", 10 * SECONDS)]
    private async Task<OptimismGasPriceDTO> LoadGasPricesAsync()
    {
        return await OptimismRpcClient.GetGasPricesAsync();
    }
}

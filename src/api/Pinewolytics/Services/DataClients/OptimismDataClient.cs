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
    private readonly OptimismRPCClient OptimismRpcClient = null!;

    [RealtimeValue("Optimism-MarketData", 20 * SECONDS)]
    public async Task<MarketDataDTO> LoadMarketDataAsync()
    {
        return await CoinGeckoClient.GetOPMarketDataDTOAsync();
    }

    [RealtimeValue("Optimism-Block-Height", SECONDS)]
    public async Task<double> LoadPeakBlockHeightAsync()
    {
        return await OptimismRpcClient.GetPeakBlockHeightAsync();
    }
}

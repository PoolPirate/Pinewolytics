using Common.Services;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Hubs.Luna;

public class LunaDataClient : BaseDataClient<LunaDataHub, ILunaDataHubClient>
{
    [Inject]
    private readonly LunaLCDClient LunaLCDClient = null!;
    [Inject]
    private readonly CoinGeckoClient CoinGeckoClient = null!;

    [RealtimeValue(2 * SECONDS, nameof(ILunaDataHubClient.PeakBlockHeight))]
    private async Task<ulong> LoadPeakBlockHeightAsync()
    {
        var (height, _) = await LunaLCDClient.GetLatestBlockInfoAsync();
        return height;
    }

    [RealtimeValue(2 * SECONDS, nameof(ILunaDataHubClient.PeakBlockTimestamp))]
    private async Task<DateTimeOffset> LoadPeakBlockTimestampAsync()
    {
        var (_, timestamp) = await LunaLCDClient.GetLatestBlockInfoAsync();
        return timestamp;
    }

    [RealtimeValue(10 * SECONDS, nameof(ILunaDataHubClient.Price))]
    private async Task<double> LoadPriceAsync()
    {
        return await CoinGeckoClient.GetLunaPriceAsync();
    }
}

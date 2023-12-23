using Common.Services;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Services.DataClients;

public class LunaDataClient : BaseDataClient
{
    [Inject]
    private readonly LunaLCDClient LunaLCDClient = null!;
    [Inject]
    private readonly CoinGeckoClient CoinGeckoClient = null!;

    //[RealtimeValue("Luna-Block-Height", 2 * SECONDS)]
    //public async Task<ulong> LoadPeakBlockHeightAsync()
    //{
    //    var (height, _) = await LunaLCDClient.GetLatestBlockInfoAsync();
    //    return height;
    //}

    //[RealtimeValue("Luna-Block-Timestamp", 2 * SECONDS)]
    //public async Task<DateTimeOffset> LoadPeakBlockTimestampAsync()
    //{
    //    var (_, timestamp) = await LunaLCDClient.GetLatestBlockInfoAsync();
    //    return timestamp;
    //}

    //[RealtimeValue("Luna-Price", 10 * SECONDS)]
    //public async Task<double> LoadPriceAsync()
    //{
    //    return await CoinGeckoClient.GetLunaPriceAsync();
    //}

    //[RealtimeValue("Luna-Total-Supply", 30 * SECONDS)]
    //public async Task<double> LoadTotalSupplyAsync()
    //{
    //    return await LunaLCDClient.GetTotalSupplyAsync();
    //}

    //[RealtimeValue("Luna-Circulating-Supply", 30 * SECONDS)]
    //public async Task<double> LoadCirculatingSupplyAsync()
    //{
    //    return await LunaLCDClient.GetCirculatingSupplyAsync();
    //}
}

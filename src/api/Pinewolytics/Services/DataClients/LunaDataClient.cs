using Common.Services;
using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Hubs;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Services.DataClients;

public class LunaDataClient : Singleton
{
    [Inject]
    private readonly IHubContext<LunaDataHub, ILunaClient> LunaDataHubContext;
    [Inject]
    private readonly LunaLCDClient LunaLCDClient;
    [Inject]
    private readonly CoinGeckoClient CoinGeckoClient;

    public ulong PeakBlockHeight { get; private set; }
    public double Price { get; private set; }

    protected override async ValueTask RunAsync()
    {
        _ = RunRefresherAsync(nameof(PeakBlockHeight), new PeriodicTimer(TimeSpan.FromSeconds(1.5)), RefreshPeakBlockHeightAsync);
        _ = RunRefresherAsync(nameof(Price), new PeriodicTimer(TimeSpan.FromSeconds(10)), RefreshPriceAsync);
    }

    private async Task RunRefresherAsync(string name, PeriodicTimer timer, Func<Task> callback)
    {
        while (true)
        {
            try
            {
                await callback.Invoke();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to run refresh for {name}", name);
            }

            await timer.WaitForNextTickAsync();
        }
    }

    private async Task RefreshPeakBlockHeightAsync()
    {
        ulong height = await LunaLCDClient.GetPeakBlockHeightAsync();

        if (height > PeakBlockHeight)
        {
            await LunaDataHubContext.Clients.All.UpdateBlockHeight(height);
        }

        PeakBlockHeight = height;        
    }

    private async Task RefreshPriceAsync()
    {
        double price = await CoinGeckoClient.GetLunaPriceAsync();

        if (price != Price)
        {
            await LunaDataHubContext.Clients.All.UpdatePrice(price);
        }

        Price = price;
    }
}

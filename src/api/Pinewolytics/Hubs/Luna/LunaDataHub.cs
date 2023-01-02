using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Services.DataClients;

namespace Pinewolytics.Hubs;

public class LunaDataHub : Hub<ILunaDataHubClient>
{
    private readonly LunaDataClient LunaDataClient;

    public LunaDataHub(LunaDataClient lunaDataClient)
    {
        LunaDataClient = lunaDataClient;
    }

    public override Task OnConnectedAsync()
    {
        Clients.Caller.UpdatePeakBlockInfo(LunaDataClient.PeakBlockHeight, LunaDataClient.PeakBlockTimestamp);
        Clients.Caller.UpdatePrice(LunaDataClient.Price);
        return base.OnConnectedAsync();
    }
}

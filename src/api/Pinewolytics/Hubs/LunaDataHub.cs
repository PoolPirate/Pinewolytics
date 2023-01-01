using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Services.DataClients;

namespace Pinewolytics.Hubs;

public class LunaDataHub : Hub<ILunaClient>
{
    private readonly LunaDataClient LunaDataClient;

    public LunaDataHub(LunaDataClient lunaDataClient)
    {
        LunaDataClient = lunaDataClient;
    }

    public override Task OnConnectedAsync()
    {
        Clients.Caller.UpdateBlockHeight(LunaDataClient.PeakBlockHeight);
        Clients.Caller.UpdatePrice(LunaDataClient.Price);
        return base.OnConnectedAsync();
    }
}

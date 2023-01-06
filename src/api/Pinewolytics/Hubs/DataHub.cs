using Microsoft.AspNetCore.SignalR;

namespace Pinewolytics.Hubs;

public class DataHub<TDataClient, TReceiver> : Hub
    where TDataClient : IDataClient
{
    private readonly TDataClient Client;

    public DataHub(TDataClient client)
    {
        Client = client;
    }

    public override async Task OnConnectedAsync() 
        => await Client.SendWelcomeToAsync(Context.ConnectionId);
}

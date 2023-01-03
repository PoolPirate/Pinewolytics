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
    {
        string[] propertyNames = Client.GetPropertyNames();

        foreach (string property in propertyNames)
        {
            object value = Client.GetPropertyValue(property);
            await Clients.Caller.SendAsync(property, value, Context.ConnectionAborted);
        }
    }
}

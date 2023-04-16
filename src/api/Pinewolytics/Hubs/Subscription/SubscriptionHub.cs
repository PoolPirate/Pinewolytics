using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Services;

namespace Pinewolytics.Hubs;

public class SubscriptionHub : Hub<ISubscriptionHubClient>
{
    private readonly SocketSubscriptionService QuerySubscriptionService;

    public SubscriptionHub(SocketSubscriptionService querySubscriptionService)
    {
        QuerySubscriptionService = querySubscriptionService;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        QuerySubscriptionService.ClearSubscriptions(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task Subscribe(string queryName)
    {
        await QuerySubscriptionService.SubscribeAsync(Context.ConnectionId, queryName, Context.ConnectionAborted);
    }
}

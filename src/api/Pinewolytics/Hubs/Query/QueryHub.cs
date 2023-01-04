using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Services;

namespace Pinewolytics.Hubs;

public class QueryHub : Hub<IQueryHubClient>
{
    private readonly QuerySubscriptionService QuerySubscriptionService;

    public QueryHub(QuerySubscriptionService querySubscriptionService)
    {
        QuerySubscriptionService = querySubscriptionService;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        QuerySubscriptionService.ClearSubscriptions(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public async Task GetAndSubscribe(string queryName)
    {
        await QuerySubscriptionService.GetAndSubscribeAsync(Context.ConnectionId, queryName);
    }
}

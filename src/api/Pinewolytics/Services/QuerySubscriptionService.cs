using Common.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Pinewolytics.Hubs;

namespace Pinewolytics.Services;

public class QuerySubscriptionService : Singleton
{
    private readonly object SubscriptionsLock = new object();
    private readonly Dictionary<string, List<string>> Subscriptions = new Dictionary<string, List<string>>();

    [Inject]
    private readonly IMemoryCache Cache = null!;
    [Inject]
    private readonly IHubContext<QueryHub, IQueryHubClient> QueryHubContext = null!;

    public async Task GetAndSubscribeAsync(string connectionId, string queryName)
    {
        lock (SubscriptionsLock)
        {
            if (Subscriptions.TryGetValue(connectionId, out var subscriptionList))
            {
                subscriptionList.Add(queryName);
            }
            else
            {
                Subscriptions.Add(connectionId, new List<string>() { queryName });
            }
        }

        if (Cache.TryGetValue(queryName, out object? value))
        {
            await QueryHubContext.Clients.Client(connectionId).SendQueryResult(queryName, value!);
        }
    }

    public void ClearSubscriptions(string connectionId)
    {
        lock (SubscriptionsLock)
        {
            Subscriptions.Remove(connectionId);
        }
    }

    public async Task BroadcastQueryUpdate(string queryName)
    {
        if (!Cache.TryGetValue(queryName, out object[]? value))
        {
            return;
        }

        IEnumerable<string> targetClients;

        lock (SubscriptionsLock)
        {
            targetClients = Subscriptions
                .Where(x => x.Value.Contains(queryName))
                .Select(x => x.Key);
        }

        await QueryHubContext.Clients.Clients(targetClients).SendQueryResult(queryName, value);
    }
}

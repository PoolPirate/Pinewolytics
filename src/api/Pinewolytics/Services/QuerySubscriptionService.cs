using Common.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Pinewolytics.Hubs;
using System.Text.Json;
using System.Threading;

namespace Pinewolytics.Services;

public class QuerySubscriptionService : Singleton
{
    private readonly object SubscriptionsLock = new object();
    private readonly Dictionary<string, List<string>> Subscriptions = new Dictionary<string, List<string>>();

    [Inject]
    private readonly IDistributedCache Cache = null!;
    [Inject]
    private readonly IHubContext<QueryHub, IQueryHubClient> QueryHubContext = null!;

    public async Task GetAndSubscribeAsync(string connectionId, string queryName, CancellationToken cancellationToken)
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

        var resultJson = await Cache.GetStringAsync(queryName, cancellationToken);

        if (resultJson is null)
        {
            return;
        }

        var result = JsonSerializer.Deserialize<object[]>(resultJson)!;
        await QueryHubContext.Clients.Client(connectionId).SendQueryResult(queryName, result);
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
        var resultJson = await Cache.GetStringAsync(queryName);

        if (resultJson is null)
        {
            return;
        }

        var result = JsonSerializer.Deserialize<object[]>(resultJson)!;

        IEnumerable<string> targetClients;

        lock (SubscriptionsLock)
        {
            targetClients = Subscriptions
                .Where(x => x.Value.Contains(queryName))
                .Select(x => x.Key);
        }

        await QueryHubContext.Clients.Clients(targetClients)
            .SendQueryResult(queryName, result);
    }
}

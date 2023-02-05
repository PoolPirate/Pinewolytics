using Common.Services;
using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Hubs;
using Pinewolytics.Services.DataClients;

namespace Pinewolytics.Services;

public class SocketSubscriptionService : Singleton
{
    private readonly object SubscriptionsLock = new object();
    private readonly Dictionary<string, List<string>> Subscriptions = new Dictionary<string, List<string>>();

    [Inject]
    private readonly IHubContext<SubscriptionHub, ISubscriptionHubClient> QueryHubContext = null!;

    [Inject]
    private readonly QueryCache Cache = null!;

    [Inject]
    private readonly OsmosisDataClient OsmosisDataClient = null!;
    [Inject]
    private readonly LunaDataClient LunaDataClient = null!;
    [Inject]
    private readonly OptimismDataClient OptimismDataClient = null!;

    public async Task GetAndSubscribeAsync(string connectionId, string name, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return;
        }

        lock (SubscriptionsLock)
        {
            if (Subscriptions.TryGetValue(connectionId, out var subscriptionList))
            {
                subscriptionList.Add(name);
            }
            else
            {
                Subscriptions.Add(connectionId, new List<string>() { name });
            }
        }

        var client = QueryHubContext.Clients.Client(connectionId);

        await OsmosisDataClient.SendPropertyToAsync(name, client);
        await LunaDataClient.SendPropertyToAsync(name, client);
        await OptimismDataClient.SendPropertyToAsync(name, client);

        var result = await Cache.GetFromCacheAsync(name);

        if (result is null)
        {
            return;
        }

        await QueryHubContext.Clients.Client(connectionId).SendQueryResult(name, result);
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
        var result = await Cache.GetFromCacheAsync(queryName);

        if (result is null)
        {
            return;
        }

        string[] targetClients;

        lock (SubscriptionsLock)
        {
            targetClients = Subscriptions
                .Where(x => x.Value.Contains(queryName))
                .Select(x => x.Key)
                .ToArray();
        }

        await QueryHubContext.Clients.Clients(targetClients)
            .SendQueryResult(queryName, result);
    }

    public async Task BroadcastRealtimeValueUpdate(string key, object value)
    {
        string[] targetClients;

        lock (SubscriptionsLock)
        {
            targetClients = Subscriptions
                .Where(x => x.Value.Contains(key))
                .Select(x => x.Key)
                .ToArray();
        }

        await QueryHubContext.Clients.Clients(targetClients)
            .SendQueryResult(key, value);
    }
}

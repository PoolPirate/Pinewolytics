using Common.Services;
using Microsoft.AspNetCore.SignalR;
using Pinewolytics.Hubs;

namespace Pinewolytics.Services;

public class SocketSubscriptionService : Singleton
{
    private readonly object SubscriptionsLock = new object();
    private readonly Dictionary<string, List<string>> Subscriptions = new Dictionary<string, List<string>>();

    [Inject]
    private readonly IHubContext<SubscriptionHub, ISubscriptionHubClient> QueryHubContext = null!;

    [Inject]
    private readonly QueryCache Cache = null!;

    public void Subscribe(string connectionId, string name, CancellationToken cancellationToken)
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
    }

    public void ClearSubscriptions(string connectionId)
    {
        lock (SubscriptionsLock)
        {
            Subscriptions.Remove(connectionId);
        }
    }

    public async Task BroadcastQueryUpdateAsync(string queryName)
    {
        object[]? result = await Cache.GetFromCacheAsync(queryName);

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

    public async Task BroadcastRealtimeValueUpdateAsync(string key, object value)
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
            .SendRealtimeValue(key, value);
    }

    public async Task BroadcastFeedExtensionAsync(string key, object value)
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
            .SendRealtimeFeedExtension(key, value);
    }
}

using Common.Services;
using Pinewolytics.Services.DataClients;
using System.Reflection;

namespace Pinewolytics.Services.StreamClients;

[InitializationPriority(-1)]
public abstract class BaseFeedClient : Singleton
{
    class Feed
    {
        public string Key { get; }
        public int CacheSize { get; }
        public IAsyncEnumerable<object> Enumerable { get; }

        public List<object> FeedCache { get; set; } = new List<object>();

        public Feed(string key, int cacheSize, IAsyncEnumerable<object> enumerable)
        {
            Key = key;
            CacheSize = cacheSize;
            Enumerable = enumerable;
        }

        public void Push(object entry)
        {
            FeedCache.Insert(0, entry);

            if (FeedCache.Count <= CacheSize)
            {
                return;
            }

            FeedCache.RemoveAt(FeedCache.Count - 1);
        }
    }

    protected const int SECONDS = 1000;
    protected const int MINUTES = 60 * SECONDS;
    protected const int HOURS = 60 * MINUTES;

    [Inject]
    private readonly SocketSubscriptionService SocketSubscriptionService = null!;
    private readonly Feed[] Feeds;

    public BaseFeedClient()
    {
        Feeds = GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Where(x => x.GetCustomAttribute<RealtimeFeed>() is not null)
        .Select(x =>
        {
            if (x.ReturnType.GenericTypeArguments.Length != 1 || x.ReturnType != typeof(IAsyncEnumerable<>).MakeGenericType(x.ReturnType.GetGenericArguments()[0]))
            {
                throw new InvalidOperationException($"RealtimeFeed {x.Name} must return a IAsyncEnumerable<T>");
            }

            var attribute = x.GetCustomAttribute<RealtimeFeed>()!;

            return new Feed(
                attribute.Key,
                attribute.CacheSize,
                (IAsyncEnumerable<object>)x.Invoke(this, null)!
            );
        })
        .ToArray();
    }

    protected override ValueTask InitializeAsync()
    {
        foreach(var feed in Feeds)
        {
           _ = Task.Run(async () =>
            {
                try
                {
                    await foreach (var entry in feed.Enumerable)
                    {
                        feed.Push(entry);
                        await SocketSubscriptionService.BroadcastFeedExtensionAsync(feed.Key, entry);
                    }

                    Logger.LogCritical("A feed exited without an exception: {key}", feed.Key);
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex, "A feed crashed with an exception: {key}", feed.Key);
                }
            });
        }

        return base.InitializeAsync();
    }

    public bool TryGetCachedFeed(string key, out object[]? value)
    {
        var feed = Feeds.SingleOrDefault(x => x.Key == key);

        if (feed is null)
        {
            value = null;
            return false;
        }

        value = feed.FeedCache.ToArray();
        return true;
    }
}

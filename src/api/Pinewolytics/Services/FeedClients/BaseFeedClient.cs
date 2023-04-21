using Common.Services;
using Pinewolytics.Models.DTOs.All;
using System.Reflection;

namespace Pinewolytics.Services.StreamClients;

[InitializationPriority(-1)]
public abstract class BaseFeedClient : Singleton
{
    class Feed
    {
        public string Key { get; }
        public int CacheSizeLimit { get; }
        public TimeSpan? MaxItemAge { get; }
        public IAsyncEnumerable<object> Enumerable { get; }

        public List<object> FeedCache { get; set; } = new List<object>();

        public Feed(string key, int cacheSize, TimeSpan? maxItemAge, IAsyncEnumerable<object> enumerable)
        {
            Key = key;
            CacheSizeLimit = cacheSize;
            MaxItemAge = maxItemAge;
            Enumerable = enumerable;
        }

        public void Push(object entry)
        {
            FeedCache.Insert(0, entry);

            if (CacheSizeLimit > 0 && FeedCache.Count <= CacheSizeLimit)
            {
                FeedCache.RemoveAt(FeedCache.Count - 1);
            }
            if (MaxItemAge is not null)
            {
                FeedCache.RemoveAll(x => ((ITimestampedDTO)x).GetTimestamp() < DateTimeOffset.UtcNow - MaxItemAge.Value);
            }
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

            return attribute.MaximumItemAge.HasValue && (x.ReturnType.GenericTypeArguments[0].GetInterface(typeof(ITimestampedDTO).Name) is null)
                ? throw new InvalidOperationException($"RealtimeFeed {x.Name} must return a IAsyncEnumerable<T> where T is ITimestampedDTO")
                : new Feed(
                attribute.Key,
                attribute.CacheSizeLimit,
                attribute.MaximumItemAge,
                (IAsyncEnumerable<object>)x.Invoke(this, null)!
            );
        })
        .ToArray();
    }

    protected override ValueTask InitializeAsync()
    {
        foreach (var feed in Feeds)
        {
            _ = Task.Run(async () =>
             {
                 try
                 {
                     await foreach (object entry in feed.Enumerable)
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

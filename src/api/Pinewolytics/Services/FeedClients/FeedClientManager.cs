using Common.Services;
using Pinewolytics.Services.StreamClients;
using System.Collections.Immutable;
using System.Reflection;

namespace Pinewolytics.Services.FeedClients;

public class FeedClientManager : Singleton
{
    private ImmutableArray<BaseFeedClient> DataClients;

    protected override ValueTask InitializeAsync()
    {
        var clientTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.BaseType == typeof(BaseFeedClient))
            .Where(x => !x.IsAbstract)
            .ToArray();

        DataClients = clientTypes.Select(x => (BaseFeedClient)Provider.GetRequiredService(x)).ToImmutableArray();

        return base.InitializeAsync();
    }

    public bool TryGetRealtimeValue(string key, out object[]? value)
    {
        foreach (var client in DataClients)
        {
            if (!client.TryGetCachedFeed(key, out value))
            {
                continue;
            }

            return true;
        }

        value = null;
        return false;
    }

}

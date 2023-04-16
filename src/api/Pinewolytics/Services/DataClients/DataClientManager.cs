using Common.Services;
using System.Collections.Immutable;
using System.Reflection;

namespace Pinewolytics.Services.DataClients;

public class DataClientManager : Singleton
{
    private ImmutableArray<BaseDataClient> DataClients;

    protected override ValueTask InitializeAsync()
    {
        var clientTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.BaseType == typeof(BaseDataClient))
            .Where(x => !x.IsAbstract)
            .ToArray();

        DataClients = clientTypes.Select(x => (BaseDataClient)Provider.GetRequiredService(x)).ToImmutableArray();

        return base.InitializeAsync();
    }

    public bool TryGetRealtimeValue(string key, out object? value)
    {
        foreach(var client in DataClients)
        {
            if (!client.TryGetProperty(key, out value))
            {
                continue;
            }

            return true;
        }

        value = null;
        return false;
    }
    
}

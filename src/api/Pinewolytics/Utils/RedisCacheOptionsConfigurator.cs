using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Options;
using Pinewolytics.Configuration;

namespace Pinewolytics.Utils;

public class RedisCacheOptionsConfigurator : IConfigureNamedOptions<RedisCacheOptions>
{
    private readonly DatabaseOptions DatabaseOptions;
    private readonly InstanceOptions InstanceOptions;

    public RedisCacheOptionsConfigurator(DatabaseOptions databaseOptions, InstanceOptions instanceOptions)
    {
        DatabaseOptions = databaseOptions;
        InstanceOptions = instanceOptions;
    }

    public void Configure(string? name, RedisCacheOptions options)
    {
        options.Configuration = DatabaseOptions.RedisConnectionString;
        options.InstanceName = InstanceOptions.Name;
    }

    public void Configure(RedisCacheOptions options)
    {
        options.Configuration = DatabaseOptions.RedisConnectionString;
        options.InstanceName = InstanceOptions.Name;
    }
}

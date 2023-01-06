using Common.Services;
using StackExchange.Redis;
using System.Text.Json;

namespace Pinewolytics.Services;

public class QueryCache : Singleton
{
    private const int CacheDatabase = 5;

    [Inject]
    private readonly IConnectionMultiplexer RedisConnection = null!;

    public async Task AddToCacheAsync(string queryName, object[] results)
    {
        var resultJson = JsonSerializer.Serialize(results, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await RedisConnection.GetDatabase(CacheDatabase).StringSetAsync(queryName, resultJson);
    }

    public async Task<string?> GetFromCacheRawAsync(string queryName) 
        => await RedisConnection.GetDatabase(CacheDatabase).StringGetAsync(queryName);

    public async Task<object[]?> GetFromCacheAsync(string queryName)
    {
        string json = await RedisConnection.GetDatabase(CacheDatabase).StringGetAsync(queryName);

        return json is null 
            ? null 
            : JsonSerializer.Deserialize<object[]>(json);
    }
}

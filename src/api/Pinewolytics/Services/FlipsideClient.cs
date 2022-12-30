using Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pinewolytics.Configuration;
using Pinewolytics.Models;
using Pinewolytics.Models.FlipsideAPI;

namespace Pinewolytics.Services;

public class FlipsideClient : Singleton
{
    [Inject]
    private readonly HttpClient Client;
    [Inject]
    private readonly ApiKeyOptions ApiKeyOptions;
    [Inject]
    private readonly IMemoryCache Cache;

    public async Task RunQueryAndCacheAsync(string sql, TimeSpan cacheDuration,
        CancellationToken cancellationToken = default)
    {
        var queueResult = await QueueQueryAsync(sql, cancellationToken);

        if (!queueResult.Cached)
        {
            await Task.Delay(200, cancellationToken);
        }

        object[][]? results = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);

        while (results is null)
        {
            await Task.Delay(200, cancellationToken);
            results = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);
        }

        Cache.Set(sql, results, cacheDuration);
    }

    public async Task<T[]> GetOrRunAsync<T>(string sql,
        CancellationToken cancellationToken = default)
        where T : IFlipsideObject<T>
    {
        return Cache.TryGetValue<object[][]>(sql, out object[][]? results)
            ? ParseFlipsideObjects<T>(results!)
            : await RunQueryAsync<T>(sql, cancellationToken);
    }

    private async Task<T[]> RunQueryAsync<T>(string sql,
        CancellationToken cancellationToken = default)
        where T : IFlipsideObject<T>
    {
        var queueResult = await QueueQueryAsync(sql, cancellationToken);

        if (!queueResult.Cached)
        {
            await Task.Delay(200, cancellationToken);
        }

        object[][]? results = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);

        while (results is null)
        {
            await Task.Delay(200, cancellationToken);
            results = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);
        }

        return ParseFlipsideObjects<T>(results);
    }

    private T[] ParseFlipsideObjects<T>(object[][] results)
        where T : IFlipsideObject<T>
    {
        return results.Select(result =>
        {
            if (result.Contains(null))
            {
                Logger.LogWarning("Query result contains null values!");
            }

            return T.Parse(result.Select(x => x?.ToString() ?? "").ToArray()!);
        }).ToArray();
    }

    private async Task<QueueQueryResult> QueueQueryAsync(string sql,
        CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(new Dictionary<string, object>()
        {
            ["sql"] = sql,
            ["ttl_minutes"] = 1, //Use own caching instead
            ["cache"] = false
        });

        var request = new HttpRequestMessage(HttpMethod.Post, "https://node-api.flipsidecrypto.com/queries")
        {
            Content = content
        };

        request.Headers.Add("x-api-key", ApiKeyOptions.FlipsideApiKey);

        var response = await Client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Flipside responded with {response.StatusCode}: {errorMessage}");
        }

        return (await response.Content.ReadFromJsonAsync<QueueQueryResult>(cancellationToken: cancellationToken))
            ?? throw new InvalidOperationException("Failed parsing result from flipside!");
    }

    private async Task<object[][]?> GetQueryResultsAsync(string token, int pageNumber = 1, int pageSize = 100000,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://node-api.flipsidecrypto.com/queries/{token}?pageNumber={pageNumber}&pageSize={pageSize}");

        request.Headers.Add("x-api-key", ApiKeyOptions.FlipsideApiKey);

        var response = await Client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"Flipside responded with {response.StatusCode}: {errorMessage}");
        }

        var result = await response.Content.ReadFromJsonAsync<QueryResultsResult>(cancellationToken: cancellationToken);

        return result?.Status != "finished"
            ? null
            : (result?.Results);
    }
}

using Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Pinewolytics.Configuration;
using Pinewolytics.Models;
using Pinewolytics.Models.FlipsideAPI;
using System.Reflection;

namespace Pinewolytics.Services;

public class FlipsideClient : Singleton
{
    [Inject]
    private readonly HttpClient Client = null!;
    [Inject]
    private readonly ApiKeyOptions ApiKeyOptions = null!;
    [Inject]
    private readonly IMemoryCache Cache = null!;

    public async Task RunQueryAndCacheAsync(string key, Type type, string sql,
        CancellationToken cancellationToken = default)
    {
        var queueResult = await QueueQueryAsync(sql, cancellationToken);

        if (!queueResult.Cached)
        {
            await Task.Delay(200, cancellationToken);
        }

        object[][]? rows = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);

        while (rows is null)
        {
            await Task.Delay(200, cancellationToken);
            rows = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);
        }

        object[] result = ParseFlipsideObjects(type, rows);
        Cache.Set(key, result.Length == 1 ? result[0] : result);
    }

    public async Task<T[]> GetOrRunAsync<T>(string sql,
        CancellationToken cancellationToken = default)
        where T : IFlipsideObject<T>
    {
        return Cache.TryGetValue(sql, out T[]? results)
            ? results!
            : await RunQueryAsync<T>(sql, cancellationToken);
    }

    public async Task<T[]> RunQueryAsync<T>(string sql,
        CancellationToken cancellationToken)
        where T : IFlipsideObject<T>
    {
        return (await RunQueryAsync(sql, typeof(T), cancellationToken)).Cast<T>().ToArray()!;
    }

    private async Task<object[]> RunQueryAsync(string sql, Type type,
        CancellationToken cancellationToken = default)
    {
        var queueResult = await QueueQueryAsync(sql, cancellationToken);

        if (!queueResult.Cached)
        {
            await Task.Delay(200, cancellationToken);
        }

        object[][]? rows = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);

        while (rows is null)
        {
            await Task.Delay(200, cancellationToken);
            rows = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);
        }

        var result = ParseFlipsideObjects(type, rows);
        return result;
    }

    private object[] ParseFlipsideObjects(Type type, object[][] rows)
    {
        return rows.Select(result =>
        {
            if (result.Contains(null))
            {
                Logger.LogWarning("Query result contains null values!");
            }

            var method = type.GetMethod(nameof(IFlipsideObject<object>.Parse), BindingFlags.Static | BindingFlags.Public);

            if (method is null)
            {
                throw new InvalidOperationException($"Missing method on Type {type.Name}");
            }
            //
            var parsed = method.Invoke(
                null,
                new[] { result.Select(x => x?.ToString() ?? "").ToArray() }
            )!;
            return parsed;
        }).ToArray();
    }

    private async Task<QueueQueryResult> QueueQueryAsync(string sql,
        CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(new Dictionary<string, object>()
        {
            ["sql"] = sql,
            ["ttl_minutes"] = 1, //Use own caching instead
            ["cache"] = true
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

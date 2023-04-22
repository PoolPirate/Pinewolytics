using Common.Services;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Configuration;
using Pinewolytics.Models;
using Pinewolytics.Models.FlipsideAPI;
using Polly;
using System.Reflection;
using System.Text.Json;

namespace Pinewolytics.Services;

public class FlipsideClient : Singleton
{
    [Inject]
    private readonly HttpClient Client = null!;
    [Inject]
    private readonly ApiKeyOptions ApiKeyOptions = null!;
    [Inject]
    private readonly QueryCache Cache = null!;

    private readonly IAsyncPolicy<QueryResultsResult> RetryPolicy =
        Policy<QueryResultsResult>
            .HandleResult(x => x.Status == QueryResultStatus.Running)
            .WaitAndRetryForeverAsync(x => TimeSpan.FromMilliseconds(200) * x);

    public async Task RunQueryAndCacheAsync(string key, Type type, string sql,
        CancellationToken cancellationToken = default)
    {
        var queueResult = await QueueQueryAsync(sql, cancellationToken);

        if (!queueResult.Cached)
        {
            await Task.Delay(200, cancellationToken);
        }

        object[][] rows = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);
        object[] result = ParseFlipsideObjects(type, rows);

        if (result.Length > 99000)
        {
            Logger.LogWarning("Query too close to result size limit: {queryName}", key);
        }

        await Cache.AddToCacheAsync(key, result);
    }

    public async Task<T[]> GetOrRunAsync<T>(string sql,
        CancellationToken cancellationToken = default)
        where T : IFlipsideObject<T>
    {
        string? resultJson = await Cache.GetFromCacheRawAsync(sql);

        if (resultJson is null)
        {
            return await RunQueryAsync<T>(sql, cancellationToken);
        }
        //
        return JsonSerializer.Deserialize<T[]>(resultJson)
            ?? throw new InvalidOperationException("Redis resonse json was invalid!");
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
        object[][] rows = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);
        object[] result = ParseFlipsideObjects(type, rows);
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
            object parsed = method.Invoke(
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

    private async Task<object[][]> GetQueryResultsAsync(string token,
        CancellationToken cancellationToken = default)
    {
        var result = await RetryPolicy.ExecuteAsync(() => GetFinishedQueryResultsAsync(token, cancellationToken));
        return result.Results;
    }

    private async Task<QueryResultsResult> GetFinishedQueryResultsAsync(string token, CancellationToken cancellationToken = default)
    {
        List<QueryResultsResult>? resultPages = null;

        for(int i = 1; i <= 10; i++)
        {
            var page = await GetFinishedQueryResultPageAsync(token, i, 100000, cancellationToken);

            if (page.Status != "finished")
            {
                return page;
            }

            if (page.Results.Length < 100_000)
            {
                if (resultPages is null)
                {
                    return page;
                }

                resultPages.Add(page);
                break;
            }

            resultPages ??= new List<QueryResultsResult>();
            resultPages.Add(page);
        }

        return new QueryResultsResult()
        {
            Results = resultPages!.SelectMany(x => x.Results).ToArray(),
            ColumnLabels = resultPages![0].ColumnLabels,
            ColumnTypes = resultPages![0].ColumnTypes,
            Status = resultPages![0].Status,
            Message = resultPages![0].Message,
            StartedAt = resultPages![0].StartedAt,
            EndedAt = resultPages![0].EndedAt,
        };
}

    private async Task<QueryResultsResult> GetFinishedQueryResultPageAsync(string token, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"https://node-api.flipsidecrypto.com/queries/{token}?pageNumber={pageNumber}&pageSize={pageSize}");

        request.Headers.Add("x-api-key", ApiKeyOptions.FlipsideApiKey);

        var response = await Client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Flipside responded with {errorMessage}", null, response.StatusCode);
        }

        return (await response.Content.ReadFromJsonAsync<QueryResultsResult>(cancellationToken: cancellationToken))
            ?? throw new InvalidOperationException("Unexpected json format");
    }
}

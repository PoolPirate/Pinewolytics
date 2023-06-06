using Common.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Pinewolytics.Configuration;
using Pinewolytics.Models;
using Pinewolytics.Models.FlipsideAPI;
using Pinewolytics.Models.FlipsideAPI.Requests;
using Pinewolytics.Models.FlipsideAPI.Results;
using Polly;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;

namespace Pinewolytics.Services;

public class FlipsideClient : Singleton
{
    private static readonly Uri APIUrl = new Uri("https://api-v2.flipsidecrypto.xyz/json-rpc");

    [Inject]
    private readonly HttpClient Client = null!;
    [Inject]
    private readonly ApiKeyOptions ApiKeyOptions = null!;
    [Inject]
    private readonly QueryCache Cache = null!;

    private readonly IAsyncPolicy<GetQueryRunResult> RetryPolicy =
        Policy<GetQueryRunResult>
            .HandleResult(x => x.QueryRun.State == QueryStatus.Ready || x.QueryRun.State == QueryStatus.Running || x.QueryRun.State == QueryStatus.Pending)
            .WaitAndRetryForeverAsync(x => TimeSpan.FromMilliseconds(200) * x);

    public async Task RunQueryAndCacheAsync(string key, Type type, string sql,
        CancellationToken cancellationToken = default)
    {
        var queueResult = await QueueQueryAsync(sql, cancellationToken);

        await Task.Delay(200, cancellationToken);

        object[][] rows = await GetQueryResultsAsync(queueResult.QueryRequest.QueryRunId, cancellationToken: cancellationToken);
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
        object[][] rows = await GetQueryResultsAsync(queueResult.QueryRequest.QueryRunId, cancellationToken: cancellationToken);
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

    private Task<CreateQueryRunResult> QueueQueryAsync(string sql,
        CancellationToken cancellationToken = default)
    {
        return SendRPCAsync<CreateQueryRunResult>(FlipsideRequest.CreateQueryRun(0, 0, sql), cancellationToken);
    }

    private async IAsyncEnumerable<object[]> GetBatchedQueryResultsAsync(string token,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await RetryPolicy.ExecuteAsync(() => GetQueryRunAsync(token, cancellationToken));
        var pageSize = 10000;

        for(int i = 0; i <= result.QueryRun.RowCount / pageSize; i++)
        {
            var page = await GetFinishedQueryResultPageAsync(
                token, 
                i + 1,
                pageSize,
                cancellationToken
            );

            foreach(var row in page.Rows)
            {
                yield return row;
            }
        }
    }

    private async Task<object[][]> GetQueryResultsAsync(string token, CancellationToken cancellationToken)
    {
        var result = await RetryPolicy.ExecuteAsync(() => GetQueryRunAsync(token, cancellationToken));

        await Task.Delay(100, cancellationToken);

        if (!result.QueryRun.RowCount.HasValue)
        {
            throw new InvalidOperationException("Result set size unknown");
        }

        var rows = new object[result.QueryRun.RowCount.Value][];
        var pageSize = 10000;

        for (int i = 0; i <= result.QueryRun.RowCount / pageSize; i++)
        {
            var page = await GetFinishedQueryResultPageAsync(
                token,
                i + 1,
                pageSize,
                cancellationToken
            );

            page.Rows.CopyTo(rows.AsSpan(pageSize * i, Math.Min(pageSize, page.Rows.Length)));
        }

        return rows;
    }

    private async Task<GetQueryRunResult> GetQueryRunAsync(string token, CancellationToken cancellationToken)
    {
        var getQueryRunResult = await SendRPCAsync<GetQueryRunResult>(FlipsideRequest.GetQueryRun(token), cancellationToken);
        return getQueryRunResult;
    }

    private async Task<GetQueryRunResultsResult> GetFinishedQueryResultPageAsync(string token, int pageNumber, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var result = await SendRPCAsync<GetQueryRunResultsResult>(
            FlipsideRequest.GetQueryRunResults(token, pageNumber, pageSize),
            cancellationToken
        );

        return result;
    }

    private async Task<TResult> SendRPCAsync<TResult>(FlipsideRequest payload, CancellationToken cancellationToken)
        where TResult : class, IFlipsideRequestResult
    {
        var content = JsonContent.Create(payload, options: new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
        var request = new HttpRequestMessage(HttpMethod.Post, APIUrl)
        {
            Content = content,
        };

        request.Headers.Add("x-api-key", ApiKeyOptions.FlipsideApiKey);

        var response = await Client.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string errorMessage = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Flipside responded with {errorMessage}", null, response.StatusCode);
        }

        var result = await response.Content.ReadFromJsonAsync<FlipsideResult<TResult>>(cancellationToken: cancellationToken);

        if (result is null)
        {
            throw new HttpRequestException($"Flipside responded with null");
        }
        if (result.Error is not null)
        {
            throw new HttpRequestException($"Flipside responded with error: {JsonSerializer.Serialize(result.Error)}");
        }

        //

        return result.Result is null
            ? throw new HttpRequestException($"Flipside responded with null")
            : result.Result;
    }
}

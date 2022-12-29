using Common.Services;
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

    public async Task<T[]> RunQueryAsync<T>(string sql, int ttlMinutes = 10, bool allowCache = true,
        CancellationToken cancellationToken = default)
        where T : IFlipsideObject<T>
    {
        var queueResult = await QueueQueryAsync(sql, ttlMinutes, allowCache, cancellationToken);

        if (!queueResult.Cached)
        {
            await Task.Delay(200, cancellationToken);
        }

        object[][]? results = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);

        while (results == null)
        {
            await Task.Delay(200, cancellationToken);
            results = await GetQueryResultsAsync(queueResult.Token, cancellationToken: cancellationToken);
        }

        return results.Select(result => {
            if (result.Contains(null))
            {
                Logger.LogWarning("Query result contains null values!");
            }

            return T.Parse(result.Select(x => x?.ToString() ?? "").ToArray()!);
        }).ToArray();
    }

    private async Task<QueueQueryResult> QueueQueryAsync(string sql, int ttlMinutes = 10, bool allowCache = true,
        CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(new Dictionary<string, object>()
        {
            ["sql"] = sql,
            ["ttl_minutes"] = ttlMinutes,
            ["cache"] = allowCache
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

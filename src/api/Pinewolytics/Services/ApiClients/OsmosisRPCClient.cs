using Common.Services;
using Pinewolytics.Utils;
using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading;

namespace Pinewolytics.Services.ApiClients;

public class OsmosisRPCClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://rpc-osmosis.imperator.co/", UriKind.Absolute);

    [Inject]
    private readonly HttpClient Client = null!;

    record JsonRpcResult<T>(T Result) where T : class;
    record TransactionsResult(Transaction[] Txs);
    public record Transaction(string Hash, ulong Height, int Index);
    public async Task<Transaction[]> GetProtoRevTXsAsync(ulong minHeight, int limit, CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, $"tx_search" +
            $"?query=\"tx.height>%3D{minHeight}%20AND%20coinbase.minter%3D%27osmo17qdmjdumw4xawam4g46gtwzle5rd4zwyfqvvza%27\"" +
            $"&order_by=\"asc\"" +
            $"&per_page={limit}" +
            $"&prove=false");
        var response = await Client.GetAsync(route, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonRpcResult<TransactionsResult>>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);
        return result!.Result.Txs;
    }

    record NodeStatusResult(SyncInfo SyncInfo);
    record SyncInfo(ulong LatestBlockHeight);
    public async Task<ulong> GetPeakBlockHeightAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, $"status");
        var response = await Client.GetAsync(route, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonRpcResult<NodeStatusResult>>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);
        return result!.Result.SyncInfo.LatestBlockHeight;
    }
}

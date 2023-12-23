using Common.Services;
using Pinewolytics.Models.DTOs;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinewolytics.Services.ApiClients;

public class OsmosisRPCClient : Singleton
{
    private static readonly string ProtoRevModuleAddress = "osmo17qdmjdumw4xawam4g46gtwzle5rd4zwyfqvvza";
    private static readonly string ProtoRevModuleAddressBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(ProtoRevModuleAddress));

    private readonly Uri ApiEndpoint = new Uri("https://rpc-osmosis.imperator.co/", UriKind.Absolute);

    [Inject]
    private readonly HttpClient Client = null!;

    record JsonRpcResult<T>(T Result) where T : class;
    record TransactionsResult(Transaction[] Txs);
    record Transaction(string Hash, ulong Height, int Index, TxResult TxResult);
    record TxResult(List<TxEvent> Events);
    record TxEvent(string Type, TxEventAttribute[] Attributes);
    record TxEventAttribute(string Key, string Value);
    public async Task<(OsmosisProtoRevTransactionDTO Tx, ulong Height, int Index)[]> GetProtoRevTXsAsync(ulong minHeight, int limit, CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, $"tx_search" +
            $"?query=\"tx.height>%3D{minHeight}%20AND%20coinbase.minter%3D%27{ProtoRevModuleAddress}%27\"" +
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

        return await Task.WhenAll(result!.Result.Txs.Select(async tx =>
        {
            var backRuns = tx.TxResult.Events
                .Where(x => x.Type == "protorev_backrun")
                .ToArray();

            var swaps = backRuns.Select(backrun =>
            {
                string arbDenom = backrun.Attributes
                    .Single(x => x.Key == "arb_denom")
                    .Value;

                ulong profit = ulong.Parse(backrun.Attributes
                    .Single(x => x.Key == "profit")
                    .Value);

                return new OsmosisProtoRevSwapDTO()
                {
                    Profit = new DenominatedAmountDTO()
                    {
                        Denom = arbDenom,
                        Amount = profit,
                    },
                    ProfitUSD = 0 //ToDo: Add price calculation  

                };
            })
            .ToArray();

            var txFrom = tx.TxResult.Events
                .Where(x => x.Type == "message")
                .SelectMany(x => x.Attributes.Where(y => y.Key == "sender"))
                .First().Value;

            return (new OsmosisProtoRevTransactionDTO()
            {
                Hash = tx.Hash,
                TxFrom = txFrom,
                Timestamp = await GetBlockTimestampAsync(tx.Height, cancellationToken),
                Swaps = swaps
            }, tx.Height, tx.Index);
        }));
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

    record BlockInfo(Block Block);
    record Block(BlockHeader Header);
    record BlockHeader(DateTimeOffset Time);
    public async Task<DateTimeOffset> GetBlockTimestampAsync(ulong height, CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, $"block?height={height}");
        var response = await Client.GetAsync(route, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonRpcResult<BlockInfo>>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.Result.Block.Header.Time;
    }
}

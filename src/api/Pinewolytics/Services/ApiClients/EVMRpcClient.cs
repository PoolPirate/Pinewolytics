using Common.Services;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Pinewolytics.Utils.JsonConverters;
using System.Globalization;

namespace Pinewolytics.Services.ApiClients;

public abstract class EVMRpcClient : Singleton
{
    [Inject]
    private readonly HttpClient Client = null!;

    protected abstract Uri GetAPIUrl();

    record PeakBlockHeightResult(string Result);
    public async Task<ulong> GetPeakBlockHeightAsync(CancellationToken cancellationToken = default)
    {
        var result = await SendRpcCallAsync <PeakBlockHeightResult>("eth_blockNumber", Array.Empty<object>(), cancellationToken);
        return ulong.Parse(result.Result[2..], NumberStyles.HexNumber);
    }

    private async Task<T> SendRpcCallAsync<T>(string method, object[] parameters, CancellationToken cancellationToken)
    {
        var response = await Client.PostAsJsonAsync<EVMRpcParameters>(GetAPIUrl(), new EVMRpcParameters()
        {
            Method = method,
            Params = parameters
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)) 
            ?? throw new InvalidOperationException("RPC returned null");
    }

    class EVMRpcParameters
    {
        public int Id { get; } = 1;
        public string JsonRPC { get; } = "2.0";

        public required string Method { get; init; }
        public required object[] Params { get; init; }
    }
}

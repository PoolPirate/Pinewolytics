using Common.Services;
using System.Globalization;

namespace Pinewolytics.Services.ApiClients;

public abstract class EVMRpcClient : Singleton
{
    [Inject]
    private readonly HttpClient Client = null!;

    protected abstract Uri GetAPIUrl();

    public async Task<ulong> GetPeakBlockHeightAsync(CancellationToken cancellationToken = default)
    {
        var result = await SendRpcCallAsync <string>("eth_blockNumber", Array.Empty<object>(), cancellationToken);
        return ulong.Parse(result[2..], NumberStyles.HexNumber);
    }

    protected async Task<T> SendRpcCallAsync<T>(string method, object[] parameters, CancellationToken cancellationToken)
    {
        var response = await Client.PostAsJsonAsync(GetAPIUrl(), new EVMRpcParameters()
        {
            Method = method,
            Params = parameters
        }, cancellationToken);

        response.EnsureSuccessStatusCode();

        var evmResult = await response.Content.ReadFromJsonAsync<EVMResult<T>>(cancellationToken: cancellationToken);

        return evmResult is null || evmResult.Result is null
        ? throw new InvalidOperationException("RPC returned null")
        : evmResult.Result;
    }

    class EVMRpcParameters
    {
        public int Id { get; } = 1;
        public string JsonRPC { get; } = "2.0";

        public required string Method { get; init; }
        public required object[] Params { get; init; }
    }

    class EVMResult<T>
    {
        public required T? Result { get; init; }
    }
}

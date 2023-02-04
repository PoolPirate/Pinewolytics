using Common.Services;
using Pinewolytics.Configuration;
using Pinewolytics.Models.DTOs.Optimism;
using System.Globalization;

namespace Pinewolytics.Services.ApiClients;

public class OptimismRpcClient : EVMRpcClient
{
    [Inject]
    private readonly ApiKeyOptions ApiKeyOptions = null!;

    protected override Uri GetAPIUrl() 
        => new Uri(ApiKeyOptions.OptimismRPCProviderUrl, UriKind.Absolute);

    record GasPricesResultRaw(string L1GasPrice, string L2GasPrice);
    public async Task<OptimismGasPriceDTO> GetGasPricesAsync(CancellationToken cancellationToken = default)
    {
        var result = await SendRpcCallAsync<GasPricesResultRaw>("rollup_gasPrices", Array.Empty<object>(), cancellationToken);
        return new OptimismGasPriceDTO(
            ulong.Parse(result.L1GasPrice[2..], NumberStyles.HexNumber),
            ulong.Parse(result.L2GasPrice[2..], NumberStyles.HexNumber)
        );
    }
}

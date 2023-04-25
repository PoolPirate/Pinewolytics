using Common.Services;
using Pinewolytics.Models.DTOs;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinewolytics.Services.ApiClients;

public class OsmosisLCDClient : Singleton
{
    private static readonly string ProtoRevModuleAddress = "osmo17qdmjdumw4xawam4g46gtwzle5rd4zwyfqvvza";

    private readonly Uri ApiEndpoint = new Uri("https://lcd-osmosis.imperator.co", UriKind.Absolute);

    [Inject]
    private readonly HttpClient Client = null!;

    record ICNSResult(ICNSName Data);
    record ICNSName(string Name);
    public async Task<string?> GetICNSNameFromAddressAsync(string address, CancellationToken cancellationToken)
    {
        object payloadObject = new
        {
            primary_name = new
            {
                address
            }
        };
        string payloadJson = JsonSerializer.Serialize(payloadObject);
        string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(payloadJson));
        var route = new Uri(ApiEndpoint, $"cosmwasm/wasm/v1/contract/osmo1xk0s8xgktn9x5vwcgtjdxqzadg88fgn33p8u9cnpdxwemvxscvast52cdd/smart/{payload}?encoding=UTF-8");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ICNSResult>(cancellationToken: cancellationToken);
        string name = result!.Data!.Name;
        return string.IsNullOrWhiteSpace(name)
            ? null
            : name;
    }

    record BalanceResult(DenominatedAmountDTO Balance);
    public async Task<decimal> GetCurrentOSMOBalanceAsync(string address, CancellationToken cancellationToken)
    {
        var route = new Uri(ApiEndpoint, $"cosmos/bank/v1beta1/balances/{address}/by_denom?denom=uosmo");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<BalanceResult>(cancellationToken: cancellationToken);
        return (decimal)((double)result!.Balance!.Amount / Math.Pow(10, 6));
    }

    record AmountResult(DenominatedAmountDTO Amount);
    public async Task<decimal> GetTotalOSMOSupplyAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, "cosmos/bank/v1beta1/supply/uosmo");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AmountResult>(cancellationToken: cancellationToken);
        return (decimal)((double)result!.Amount.Amount / Math.Pow(10, 6));
    }

    record EpochResult(OsmosisEpochInfoDTO[] Epochs);
    public async Task<OsmosisEpochInfoDTO[]> GetEpochInfosAsync(CancellationToken cancellationToken)
    {
        var route = new Uri(ApiEndpoint, "osmosis/epochs/v1beta1/epochs");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<EpochResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);
        return result!.Epochs;
    }

    record TotalDelegationsResult(double TotalDelegations);
    public async Task<double> GetTotalSuperfluidDelegationsAsync(CancellationToken cancellationToken)
    {
        var route = new Uri(ApiEndpoint, "osmosis/superfluid/v1beta1/all_superfluid_delegations");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TotalDelegationsResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);
        return result!.TotalDelegations / Math.Pow(10, 6);
    }

    record ProtoRevProfitsResult(DenominatedAmountDTO[] Profits);
    public async Task<DenominatedAmountDTO[]> GetTotalProtoRevProfitsAsync(CancellationToken cancellationToken)
    {
        var route = new Uri(ApiEndpoint, "/osmosis/v14/protorev/all_profits");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ProtoRevProfitsResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.Profits;
    }

    record ProtoRevTradeCountResult(long NumberOfTrades);
    public async Task<long> GetTotalProtoRevTradeCountAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, "/osmosis/v14/protorev/number_of_trades");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ProtoRevTradeCountResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.NumberOfTrades;
    }

    record ProtoRevDeveloperAccountResult(string DeveloperAccount);
    public async Task<string> GetProtoRevDeveloperAddressAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, "/osmosis/v14/protorev/developer_account");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ProtoRevDeveloperAccountResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.DeveloperAccount;
    }

    record ProtoRevAdminAccountResult(string AdminAccount);
    public async Task<string> GetProtoRevAdminAddressAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, "/osmosis/v14/protorev/admin_account");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ProtoRevAdminAccountResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.AdminAccount;
    }

    record ProtoRevEnabledResult(bool Enabled);
    public async Task<bool> GetProtoRevIsEnabledAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, "/osmosis/v14/protorev/enabled");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ProtoRevEnabledResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.Enabled;
    }

    record ProtoRevRouteStatisticsResult(OsmosisProtoRevRouteStatisticsDTO[] Statistics);
    public async Task<OsmosisProtoRevRouteStatisticsDTO[]> GetProtoRevAllRouteStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, "/osmosis/v14/protorev/all_route_statistics");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ProtoRevRouteStatisticsResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.Statistics;
    }

    record ProtoRevBalanceResult(DenominatedAmountDTO[] Balances);
    public async Task<DenominatedAmountDTO[]> GetProtoRevModuleBalanceAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, $"cosmos/bank/v1beta1/balances/{ProtoRevModuleAddress}");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<ProtoRevBalanceResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!.Balances;
    }

    record PoolInfoResult(PoolInfo Pool);
    record PoolInfo(PoolAsset[]? PoolAssets, DenominatedAmountDTO[]? PoolLiquidity);
    record PoolAsset(DenominatedAmountDTO Token);
    public async Task<OsmosisPoolInfoDTO> GetPoolInfoAsync(int poolId, CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, $"osmosis/gamm/v1beta1/pools/{poolId}");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PoolInfoResult>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        var denoms = (result!.Pool.PoolAssets?.Select(x => x.Token) 
            ?? result.Pool.PoolLiquidity ?? throw new InvalidOperationException("Missing assets data"))
                .Select(x => x.Denom)
                .Distinct()
                .ToArray();

        return new OsmosisPoolInfoDTO()
        {
            PoolId = poolId,
            Assets = denoms
        };
    }
}


using Common.Services;
using Microsoft.AspNetCore.Routing;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Utils;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pinewolytics.Services.ApiClients;

public class OsmosisLCDClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://lcd-osmosis.imperator.co", UriKind.Absolute);

    [Inject]
    private readonly HttpClient Client = null!;

    record ICNSResult(ICNSName  Data);
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

    record BalanceResult(DenominatedBalance Balance);
    record DenominatedBalance(string Denom, double Amount);
    public async Task<double> GetCurrentOSMOBalanceAsync(string address, CancellationToken cancellationToken)
    {
        var route = new Uri(ApiEndpoint, $"cosmos/bank/v1beta1/balances/{address}/by_denom?denom=uosmo");
        var response = await Client.GetAsync(route, cancellationToken); 
        
        response.EnsureSuccessStatusCode();
    
        var result = await response.Content.ReadFromJsonAsync<BalanceResult>(cancellationToken: cancellationToken);
        return result!.Balance!.Amount / Math.Pow(10, 6);
    }

    record AmountResult(DenominatedAmount Amount);
    record DenominatedAmount(string Denom, double Amount);
    public async Task<double> GetTotalOSMOSupplyAsync(CancellationToken cancellationToken)
    {
        var route = new Uri(ApiEndpoint, "cosmos/bank/v1beta1/supply/uosmo");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AmountResult>(cancellationToken: cancellationToken);
        return result!.Amount.Amount / Math.Pow(10, 6);
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
        return result!.TotalDelegations;
    }
}

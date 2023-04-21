using Common.Services;
using Pinewolytics.Models.DTOs;
using System.Text.Json.Nodes;

namespace Pinewolytics.Services.ApiClients;

public class LunaLCDClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://phoenix-lcd.terra.dev", UriKind.Absolute);

    [Inject]
    private readonly HttpClient Client = null!;

    record AmountResult(DenominatedAmountDTO Amount);
    public async Task<ulong> GetTotalSupplyAsync()
    {
        var response = await Client.GetAsync(new Uri(ApiEndpoint, "cosmos/bank/v1beta1/supply/by_denom?denom=uluna"));
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AmountResult>();

        return result switch
        {
            not null => (ulong)result.Amount.Amount,
            _ => throw new Exception("Unexpected json format")
        };
    }

    public async Task<ulong> GetCirculatingSupplyAsync()
    {
        var response = await Client.GetAsync(new Uri("https://phoenix-api.terra.dev/balance/circulating-supply"));
        response.EnsureSuccessStatusCode();

        long supply = await response.Content.ReadFromJsonAsync<long>();

        return supply < 0 
            ? 0 
            : (ulong)supply;
    }

    public async Task<(ulong Height, DateTimeOffset Timestamp)> GetLatestBlockInfoAsync()
    {
        var response = await Client.GetAsync(new Uri(ApiEndpoint, "blocks/latest"));
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonObject>();

        if (result is null)
        {
            throw new Exception("Unexpected json format");
        }
        if (!result.TryGetPropertyValue("block", out var blockNode))
        {
            throw new Exception("Unexpected json format");
        }
        if (!blockNode!.AsObject().TryGetPropertyValue("header", out var headerNode))
        {
            throw new Exception("Unexpected json format");
        }
        if (!headerNode!.AsObject().TryGetPropertyValue("height", out var heightValue))
        {
            throw new Exception("Unexpected json format");
        }
        if (!headerNode!.AsObject().TryGetPropertyValue("time", out var timeValue))
        {
            throw new Exception("Unexpected json format");
        }
        //

        return
        (
            Height: ulong.Parse(heightValue!.AsValue().GetValue<string>()),
            Timestamp: timeValue!.AsValue().GetValue<DateTimeOffset>()
        );
    }
}

using Common.Services;
using System.Text.Json.Nodes;

namespace Pinewolytics.Services.ApiClients;

public class LunaLCDClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://phoenix-lcd.terra.dev", UriKind.Absolute);


    [Inject]
    private readonly HttpClient Client = null!;

    public async Task<ulong> GetTotalSupplyAsync()
    {
        var response = await Client.GetAsync(new Uri(ApiEndpoint, "cosmos/bank/v1beta1/supply"));
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonObject>();

        if (result is null)
        {
            throw new Exception("Unexpected json format");
        }
        if (!result.TryGetPropertyValue("supply", out var supplyArray))
        {
            throw new Exception("Unexpected json format");
        }

        var lunaSupplyNode = supplyArray!
            .AsArray()
            .SingleOrDefault(x => x!.AsObject()
                                   .TryGetPropertyValue("denom", out var denom)
                                   && denom is not null && denom.AsValue().GetValue<string>() == "uluna");

        lunaSupplyNode!.AsObject().TryGetPropertyValue("amount", out var amountValue);
        return ulong.Parse(amountValue!.AsValue().GetValue<string>());
    }

    public async Task<ulong> GetCirculatingSupplyAsync()
    {
        var response = await Client.GetAsync(new Uri("https://phoenix-api.terra.dev/balance/circulating-supply"));
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ulong>();
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

using Common.Services;
using System.Text.Json.Nodes;

namespace Pinewolytics.Services.ApiClients;

public class CoinGeckoClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://api.coingecko.com/api/v3/", UriKind.Absolute);

    [Inject]
    private readonly HttpClient Client = null!;

    public Task<double> GetLunaPriceAsync()
    {
        return GetPriceAsync("terra-luna-2");
    }

    public Task<double> GetOPPriceAsync()
    {
        return GetPriceAsync("optimism");
    }

    private async Task<double> GetPriceAsync(string currency)
    {
        var url = new Uri(ApiEndpoint, $"simple/price?ids={currency}&vs_currencies=usd");
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonObject>();

        if (result is null)
        {
            throw new Exception("Unexpected json format");
        }
        if (!result.TryGetPropertyValue(currency, out var priceNode))
        {
            throw new Exception("Unexpected json format");
        }
        if (!priceNode!.AsObject().TryGetPropertyValue("usd", out var priceValue))
        {
            throw new Exception("Unexpected json format");
        }
        //
        return priceValue!.AsValue().GetValue<double>();
    }
}

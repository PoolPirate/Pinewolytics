using Common.Services;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace Pinewolytics.Services.ApiClients;

public class CoinGeckoClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://api.coingecko.com/api/v3/", UriKind.Absolute);
    private Uri LunaPrice() => new Uri(ApiEndpoint, "simple/price?ids=terra-luna-2&vs_currencies=usd");

    [Inject]
    private readonly HttpClient Client;

    public async Task<double> GetLunaPriceAsync()
    {
        var response = await Client.GetAsync(LunaPrice());
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonObject>();

        if (result is null)
        {
            throw new Exception("Unexpected json format");
        }
        if (!result.TryGetPropertyValue("terra-luna-2", out var priceNode))
        {
            throw new Exception("Unexpected json format");
        }
        if(! priceNode!.AsObject().TryGetPropertyValue("usd", out var priceValue))
        {
            throw new Exception("Unexpected json format");
        }
        //
        return priceValue!.AsValue().GetValue<double>();
    }
}

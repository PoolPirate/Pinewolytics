using Common.Services;
using Pinewolytics.Models.DTOs.All;
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

    public Task<MarketDataDTO> GetOPMarketDataDTOAsync()
    {
        return GetMarketDataAsync("optimism");
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

    private async Task<MarketDataDTO> GetMarketDataAsync(string currency)
    {
        var url = new Uri(ApiEndpoint, $"coins/{currency}?localization=false&tickers=false&market_data=true&community_data=false&developer_data=false&sparkline=false");
        var response = await Client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonObject>();

        if (result is null) { throw new Exception("Unexpected json format"); }

        if (!result.TryGetPropertyValue("market_data", out var marketDataNode) || marketDataNode is null) { throw new Exception("Unexpected json format"); }
        if (!marketDataNode.AsObject().TryGetPropertyValue("current_price", out var currentPriceNode) || currentPriceNode is null) { throw new Exception("Unexpected json format"); }
        if (!currentPriceNode.AsObject().TryGetPropertyValue("usd", out var priceUsdNode) || priceUsdNode is null) { throw new Exception("Unexpected json format"); }
        double usdPrice = priceUsdNode.AsValue().GetValue<double>();

        if (!marketDataNode.AsObject().TryGetPropertyValue("market_cap", out var marketCapNode) || marketCapNode is null) { throw new Exception("Unexpected json format"); }
        if (!marketCapNode.AsObject().TryGetPropertyValue("usd", out var mcapUsdNode) || mcapUsdNode is null) { throw new Exception("Unexpected json format"); }
        double usdMcap = mcapUsdNode.AsValue().GetValue<double>();

        if (!marketDataNode.AsObject().TryGetPropertyValue("total_supply", out var totalSupplyNode) || totalSupplyNode is null) { throw new Exception("Unexpected json format"); }
        double totalSupply = totalSupplyNode.AsValue().GetValue<double>();

        if (!marketDataNode.AsObject().TryGetPropertyValue("circulating_supply", out var circulatingSupplyNode) || circulatingSupplyNode is null) { throw new Exception("Unexpected json format"); }
        double circulatingSupply = circulatingSupplyNode.AsValue().GetValue<double>();

        return new MarketDataDTO()
        {
            Price = usdPrice,
            MarketCap = usdMcap,
            TotalSupply = totalSupply,
            CirculatingSupply = circulatingSupply
        };
    }
}

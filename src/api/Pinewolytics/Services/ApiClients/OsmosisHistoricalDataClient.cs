using Common.Services;
using Pinewolytics.Models.DTOs.Osmosis;
using Pinewolytics.Utils;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Pinewolytics.Services.ApiClients;

public class OsmosisHistoricalDataClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://api-osmosis.imperator.co", UriKind.Absolute);

    [Inject]
    private readonly HttpClient Client = null!;

    public async Task<OsmosisTokenInfoDTO[]> GetAllTokenInfosAsync(CancellationToken cancellationToken = default)
    {
        var route = new Uri(ApiEndpoint, "/tokens/v2/all");
        var response = await Client.GetAsync(route, cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OsmosisTokenInfoDTO[]>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        }, cancellationToken: cancellationToken);

        return result!;
    }
}

using Common.Services;
using Newtonsoft.Json;

namespace Pinewolytics.Services.ApiClients;

public class LunaLCDClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://phoenix-lcd.terra.dev", UriKind.Absolute);
    private Uri LatestBlockEndpoint() => new Uri(ApiEndpoint, "blocks/latest");

    [Inject]
    private readonly HttpClient Client;

    public async Task<ulong> GetPeakBlockHeightAsync()
    {
        var response = await Client.GetAsync(LatestBlockEndpoint());
        response.EnsureSuccessStatusCode();

        dynamic result = JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
        return result.block.header.height;
    }
}

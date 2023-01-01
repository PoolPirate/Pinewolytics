using Common.Services;
using Newtonsoft.Json;
using System.Text.Json.Nodes;

namespace Pinewolytics.Services.ApiClients;

public class LunaLCDClient : Singleton
{
    private readonly Uri ApiEndpoint = new Uri("https://phoenix-lcd.terra.dev", UriKind.Absolute);
    private Uri LatestBlockEndpoint() => new Uri(ApiEndpoint, "blocks/latest");

    [Inject]
    private readonly HttpClient Client;

    public async Task<(ulong Height, DateTimeOffset Timestamp)> GetLatestBlockInfoAsync()
    {
        var response = await Client.GetAsync(LatestBlockEndpoint());
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
            Height: ulong.Parse( heightValue!.AsValue().GetValue<string>()),
            Timestamp: timeValue!.AsValue().GetValue<DateTimeOffset>()
        );
    }
}

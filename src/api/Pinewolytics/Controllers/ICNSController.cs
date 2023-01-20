using Microsoft.AspNetCore.Mvc;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Controllers;

[Route("Api/ICNS")]
[ApiController]
public class ICNSController : ControllerBase
{
    private readonly OsmosisLCDClient OsmosisLCDClient;

    public ICNSController(OsmosisLCDClient osmosisLCDClient)
    {
        OsmosisLCDClient = osmosisLCDClient;
    }

    [HttpGet("Reverse/{address}")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "address" })]
    public async Task<IActionResult> LookupICNSReverseAsync([FromRoute] string address, 
        CancellationToken cancellationToken)
    {
        string? name = await OsmosisLCDClient.GetICNSNameFromAddressAsync(address, cancellationToken);
        Console.WriteLine(address);
        return name is null 
            ? NotFound() 
            : Ok(name);
    }
}

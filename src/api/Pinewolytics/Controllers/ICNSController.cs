using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Services.ApiClients;

namespace Pinewolytics.Controllers;

[Route("Api/ICNS")]
[ApiController]
public class ICNSController : ControllerBase
{
    private readonly OsmosisLCDClient OsmosisLCDClient;
    private readonly PinewolyticsContext DbContext;

    public ICNSController(OsmosisLCDClient osmosisLCDClient, PinewolyticsContext dbContext)
    {
        OsmosisLCDClient = osmosisLCDClient;
        DbContext = dbContext;
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

    [HttpGet("Resolve/{name}")]
    [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, VaryByQueryKeys = new[] { "name" })]
    public async Task<IActionResult> ResolveICNSNameAsyc([FromRoute] string name, CancellationToken cancellationToken)
    {
        if (name.EndsWith(".osmo"))
        {
            name = name[..^5];
        }

        var icnsName = await DbContext.ICNSNames.FirstOrDefaultAsync(x => x.Name == name, cancellationToken: cancellationToken);

        return icnsName is null 
            ? NotFound() 
            : Ok(icnsName);
    }
}

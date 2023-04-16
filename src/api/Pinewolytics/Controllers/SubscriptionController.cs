using Microsoft.AspNetCore.Mvc;
using Pinewolytics.Services;
using Pinewolytics.Services.DataClients;

namespace Pinewolytics.Controllers;
[Route("api/subscriptions")]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly QueryCache QueryCache = null!;
    private readonly DataClientManager DataClientManager = null!;

    public SubscriptionController(QueryCache queryCache, DataClientManager dataClientManager)
    {
        QueryCache = queryCache;
        DataClientManager = dataClientManager;
    }

    public record SubscriptionValueDTO(object Value);

    [HttpGet("RealtimeValue/{key}")]
    [ResponseCache(Duration = 90, Location = ResponseCacheLocation.Client)]
    public ActionResult<SubscriptionValueDTO> GetRealtimeValue([FromRoute] string key)
    {
        return !DataClientManager.TryGetRealtimeValue(key, out var value) 
            ? NotFound() 
            : Ok(new SubscriptionValueDTO(value!));
    }

    [HttpGet("Query/{key}")]
    [ResponseCache(Duration = 90, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult<SubscriptionValueDTO>> GetQueryValueAsync([FromRoute] string key)
    {
        var value = await QueryCache.GetFromCacheAsync(key);

        return value is null 
            ? NotFound() 
            : Ok(new SubscriptionValueDTO(value));
    }
}

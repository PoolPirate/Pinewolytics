using Microsoft.AspNetCore.Mvc;
using Pinewolytics.Services;
using Pinewolytics.Services.DataClients;
using Pinewolytics.Services.FeedClients;

namespace Pinewolytics.Controllers;
[Route("api/subscriptions")]
[ApiController]
public class SubscriptionController : ControllerBase
{
    private readonly QueryCache QueryCache = null!;
    private readonly DataClientManager DataClientManager = null!;
    private readonly FeedClientManager FeedClientManager = null!;

    public SubscriptionController(QueryCache queryCache, DataClientManager dataClientManager, FeedClientManager feedClientManager)
    {
        QueryCache = queryCache;
        DataClientManager = dataClientManager;
        FeedClientManager = feedClientManager;
    }

    public record SubscriptionValueDTO(object Value);

    [HttpGet("RealtimeValue/{key}")]
    [ResponseCache(Duration = 90, Location = ResponseCacheLocation.Client)]
    public ActionResult<SubscriptionValueDTO> GetRealtimeValue([FromRoute] string key)
    {
        return !DataClientManager.TryGetRealtimeValue(key, out object? value)
            ? NotFound()
            : Ok(new SubscriptionValueDTO(value!));
    }

    [HttpGet("Query/{key}")]
    [ResponseCache(Duration = 90, Location = ResponseCacheLocation.Client)]
    public async Task<ActionResult<SubscriptionValueDTO>> GetQueryValueAsync([FromRoute] string key)
    {
        object[]? value = await QueryCache.GetFromCacheAsync(key);

        return value is null
            ? NotFound()
            : Ok(new SubscriptionValueDTO(value));
    }

    [HttpGet("RealtimeFeed/{key}")]
    [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
    public ActionResult<SubscriptionValueDTO> GetRealtimeFeedValue([FromRoute] string key)
    {
        return !FeedClientManager.TryGetRealtimeValue(key, out object[]? value)
            ? NotFound()
            : Ok(new SubscriptionValueDTO(value!));
    }
}

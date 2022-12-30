using Common.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;

namespace Pinewolytics.Services;

public class QueryRunner : Singleton
{
    private readonly TimeSpan OverlapCacheDuration = TimeSpan.FromSeconds(60);

    [Inject]
    private readonly FlipsideClient Flipside;

    public async Task RunAndCacheQueryAsync(string name)
    {
        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();

        var scheduledQuery = await dbContext.ScheduledQueries
            .Where(x => x.Name == name)
            .SingleOrDefaultAsync();

        if (scheduledQuery is null)
        {
            Logger.LogWarning("Deleting query schedule for {name}", name);
            RecurringJob.RemoveIfExists(name);
            return;
        }

        Logger.LogDebug("Executing {name} query to refresh cache", name);

        await Flipside.RunQueryAndCacheAsync(scheduledQuery.Query, scheduledQuery.Interval + OverlapCacheDuration);
    }
}

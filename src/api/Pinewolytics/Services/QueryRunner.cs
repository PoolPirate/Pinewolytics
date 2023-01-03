using Common.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Models.Entities;
using System.Reflection;

namespace Pinewolytics.Services;

public class QueryRunner : Singleton
{
    private readonly TimeSpan OverlapCacheDuration = TimeSpan.FromSeconds(60);

    [Inject]
    private readonly FlipsideClient Flipside = null!;
    [Inject]
    private readonly QuerySubscriptionService QuerySusbcriptionService = null!;

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

        await RunAndCacheQueryAsync(scheduledQuery);
    }

    public async Task RunAndCacheQueryAsync(ScheduledQuery scheduledQuery)
    {
        Logger.LogDebug("Executing {name} query to refresh cache", scheduledQuery.Name);

        var type = Assembly.GetExecutingAssembly()
            .GetTypes()
            .SingleOrDefault(x => x.Name == scheduledQuery.TypeName)!;

        await Flipside.RunQueryAndCacheAsync(
            scheduledQuery.Name,
            type,
            scheduledQuery.Query,
            scheduledQuery.Interval + OverlapCacheDuration);

        await QuerySusbcriptionService.BroadcastQueryUpdate(scheduledQuery.Name);
    }
}

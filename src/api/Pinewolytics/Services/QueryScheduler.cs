using Common.Services;
using Hangfire;
using Pinewolytics.Configuration;
using Pinewolytics.Database;
using Pinewolytics.Models.Entities;
using Pinewolytics.Utils;

namespace Pinewolytics.Services;

public class QueryScheduler : Singleton
{
    private BackgroundJobServer BackgroundProcessor = null!;

    [Inject]
    private readonly QueryRunner QueryRunner = null!;

    [Inject]
    private readonly InstanceOptions InstanceOptions = null!;

    [Inject]
    private readonly QueryCache Cache = null!;

    protected override async ValueTask InitializeAsync()
    {
        using var scope = Provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();

        var scheduledQueries = dbContext.ScheduledQueries.ToArray();

        Logger.LogInformation("Scheduling {count} queries for automatic caching", scheduledQueries.Length);
        Parallel.ForEach(scheduledQueries,
            (scheduledQuery, cancellationToken) => ScheduleQuery(scheduledQuery));

        if (InstanceOptions.RequireFullSync)
        {
            Logger.LogInformation("Refreshing all queries");
            await Parallel.ForEachAsync(scheduledQueries,
                async (scheduledQuery, cancellationToken) => await QueryRunner.RunAndCacheQueryAsync(scheduledQuery));
        }
        else if (InstanceOptions.RequirePartialSync)
        {
            Logger.LogInformation("Refreshing necessary queries");
            await Parallel.ForEachAsync(scheduledQueries,
            async (scheduledQuery, cancellationToken) =>
            {
                if (await Cache.IsCachedAsync(scheduledQuery.Name))
                {
                    return;
                }

                await QueryRunner.RunAndCacheQueryAsync(scheduledQuery);
            });
        }

        BackgroundProcessor = new BackgroundJobServer(new BackgroundJobServerOptions()
        {
            ServerName = InstanceOptions.Name,
        });
    }

    private void ScheduleQuery(ScheduledQuery scheduledQuery)
    {
        Logger.LogDebug("Scheduling query {name} every {interval}", scheduledQuery.Name, scheduledQuery.Interval);

        RecurringJob.AddOrUpdate<QueryRunner>(
            scheduledQuery.Name,
            runner => runner.RunAndCacheQueryAsync(scheduledQuery.Name),
            CronUtils.ConvertFromPeriodRecurrence(scheduledQuery.Interval));
    }
}
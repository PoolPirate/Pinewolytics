using Common.Services;
using Hangfire;
using Hangfire.Server;
using Hangfire.Storage;
using Pinewolytics.Database;
using Pinewolytics.Models.Entities;

namespace Pinewolytics.Services;

public class QueryScheduler : Singleton
{
    private BackgroundJobServer BackgroundProcessor;

    [Inject]
    private readonly QueryRunner QueryRunner;

    protected override async ValueTask InitializeAsync()
    {
        using var scope = Provider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();

        var scheduledQueries = dbContext.ScheduledQueries.ToArray();

        Logger.LogInformation("Scheduling {count} queries for automatic caching", scheduledQueries.Length);

        foreach (var scheduledQuery in scheduledQueries)
        {
            ScheduleQuery(scheduledQuery);
            await QueryRunner.RunAndCacheQueryAsync(scheduledQuery);
        }

        BackgroundProcessor = new BackgroundJobServer(new  BackgroundJobServerOptions()
        {
            ServerName = "Processor",
            
        });
    }

    private void ScheduleQuery(ScheduledQuery scheduledQuery)
    {
        Logger.LogDebug("Scheduling query {name} every {interval}", scheduledQuery.Name, scheduledQuery.Interval);

        RecurringJob.AddOrUpdate<QueryRunner>(
            scheduledQuery.Name,
            runner => runner.RunAndCacheQueryAsync(scheduledQuery.Name),
            ConvertFromPeriodRecurrence(scheduledQuery.Interval));
    }

    private static string ConvertFromPeriodRecurrence(TimeSpan periodRecurrence)
    {
        if (periodRecurrence.Seconds > 0 || periodRecurrence.Milliseconds > 0)
        {
            throw new ArgumentException("Interval cannot contain seconds nor milliseconds.");
        }
        if (periodRecurrence.Hours >= 1)
        {
            return $"{periodRecurrence.Minutes} */{periodRecurrence.Hours} * * *";
        }
        if (periodRecurrence.Minutes > 1)
        {
            return $"*/{periodRecurrence.Minutes} * * * *";
        }
        //
        return $"*/1 * * * *";
    }
}
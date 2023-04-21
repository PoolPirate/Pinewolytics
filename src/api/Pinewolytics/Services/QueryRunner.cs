using Common.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Models.Entities;
using System.Reflection;

namespace Pinewolytics.Services;

public class QueryRunner : Singleton
{
    [Inject]
    private readonly FlipsideClient Flipside = null!;
    [Inject]
    private readonly SocketSubscriptionService QuerySusbcriptionService = null!;

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

        string[] typeParts = scheduledQuery.TypeName.Split('<');
        string baseTypeName = typeParts[0];

        var typeArgs = new List<Type>();

        if (typeParts.Length == 2)
        {
            string[] paramTypeNames = typeParts[1].Split(",");
            paramTypeNames[^1] = paramTypeNames[^1][..^1];

            typeArgs.AddRange(paramTypeNames.Select(x => GetTypeParamByName(x)));
        }

        var type = GetTypeByName(typeArgs.Count == 0 ? baseTypeName : $"{baseTypeName}`{typeArgs.Count}");

        if (typeArgs.Count > 0)
        {
            type = type.MakeGenericType(typeArgs.ToArray());
        }

        await Flipside.RunQueryAndCacheAsync(
            scheduledQuery.Name,
            type,
            scheduledQuery.Query);

        await QuerySusbcriptionService.BroadcastQueryUpdateAsync(scheduledQuery.Name);
    }

    private static Type GetTypeByName(string name)
    {
        return Assembly.GetExecutingAssembly()
                .GetTypes()
                .SingleOrDefault(x => x.Name == name) ?? throw new InvalidOperationException($"Unable to find type {name}");
    }

    private static Type GetTypeParamByName(string name)
    {
        return Assembly.GetAssembly(typeof(int))!
                .GetTypes()
                .SingleOrDefault(x => x.Name == name) ?? throw new InvalidOperationException($"Unable to find type {name}");
    }
}

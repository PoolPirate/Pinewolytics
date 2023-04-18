using Common.Services;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;

namespace Pinewolytics.Services;

public class FlipsideCloner : Singleton
{
    private readonly PeriodicTimer RefreshTimer;

    [Inject]
    private readonly QueryClient QueryClient = null!;

    public FlipsideCloner()
    {
        RefreshTimer = new PeriodicTimer(TimeSpan.FromHours(2));
    }

    //protected override async ValueTask RunAsync()
    //{
    //    while (true)
    //    {
    //        try
    //        {
    //            await CloneICNSTagsAsync();
    //        }
    //        catch (Exception ex)
    //        {
    //            Logger.LogCritical(ex, "There was an error cloning ICNS Tags");
    //        }

    //        await RefreshTimer.WaitForNextTickAsync();
    //    }
    //}

    private async Task CloneICNSTagsAsync()
    {
        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();
        using var transaction = await dbContext.Database.BeginTransactionAsync();

        await dbContext.ICNSNames.ExecuteDeleteAsync();

        var names = await QueryClient.ListICNSNamesAsync();
        dbContext.ICNSNames.AddRange(names);

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}

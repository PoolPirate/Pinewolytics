using Common.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Utils;

namespace Pinewolytics.Services;

public class FlipsideExtractor : Singleton
{
    //NEVER CHANGE THESE MINDLESSLY!
    private const string ICNSCloneTask = "CLONE_ICNS_NAMES";
    private const string ProtoRevTransactionExtractTask = "EXTRACT_PROTOREV_TX";

    [Inject]
    private readonly QueryClient QueryClient = null!;

    public FlipsideExtractor()
    {
    }

    protected override ValueTask InitializeAsync()
    {
        RecurringJob.AddOrUpdate<FlipsideExtractor>(
            ICNSCloneTask,
            x => x.ExtractICNSTagsAsync(),
            CronUtils.ConvertFromPeriodRecurrence(TimeSpan.FromHours(4)));

        RecurringJob.AddOrUpdate<FlipsideExtractor>(
            ProtoRevTransactionExtractTask,
            x => x.ExtractAllAddressProtoRevTransactionsAsync(),
            CronUtils.ConvertFromPeriodRecurrence(TimeSpan.FromHours(18)));

        return base.InitializeAsync();
    }

    public async Task ExtractICNSTagsAsync()
    {
        var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();
        var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await dbContext.ICNSNames.ExecuteDeleteAsync();

            var names = await QueryClient.ListICNSNamesAsync();
            dbContext.ICNSNames.AddRange(names);

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            scope.Dispose();
            await transaction.DisposeAsync();
            GC.Collect();
        }
    }

    public async Task ExtractAllAddressProtoRevTransactionsAsync()
    {
        var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();
        var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await dbContext.ProtoRevSwaps.ExecuteDeleteAsync();
            await dbContext.ProtoRevTransactions.ExecuteDeleteAsync();

            var txs = await QueryClient.ListProtoRevTransactionsAsync();
            dbContext.ProtoRevTransactions.AddRange(txs); //Recursively finds Swaps

            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
        finally
        {
            scope.Dispose();
            await transaction.DisposeAsync();
            GC.Collect();
        }
    }
}

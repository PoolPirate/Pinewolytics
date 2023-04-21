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
        using var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();
        using var transaction = await dbContext.Database.BeginTransactionAsync();

        await dbContext.ICNSNames.ExecuteDeleteAsync();

        var names = await QueryClient.ListICNSNamesAsync();
        dbContext.ICNSNames.AddRange(names);

        await dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task ExtractAllAddressProtoRevTransactionsAsync()
    {

    }
}

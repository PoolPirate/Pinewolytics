using Common.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Pinewolytics.Database;
using Pinewolytics.Entities;
using Pinewolytics.Utils;

namespace Pinewolytics.Services;

public class FlipsideExtractor : Singleton
{
    //NEVER CHANGE THESE MINDLESSLY!
    private const string ICNSCloneTask = "CLONE_ICNS_NAMES";
    private const string ProtoRevTransactionExtractTask = "EXTRACT_PROTOREV_TX";
    private const string WalletRankings = "EXTRACT_WALLET_RANKINGS";

    [Inject]
    private readonly QueryClient QueryClient = null!;

    protected override ValueTask InitializeAsync()
    {
        RecurringJob.AddOrUpdate<FlipsideExtractor>(
            ICNSCloneTask,
            x => x.ExtractICNSTagsAsync(),
            CronUtils.ConvertFromPeriodRecurrence(TimeSpan.FromHours(23)));

        RecurringJob.AddOrUpdate<FlipsideExtractor>(
            ProtoRevTransactionExtractTask,
            x => x.ExtractAllAddressProtoRevTransactionsAsync(),
            CronUtils.ConvertFromPeriodRecurrence(TimeSpan.FromHours(23)));

        RecurringJob.AddOrUpdate<FlipsideExtractor>(
            WalletRankings,
            x => x.ExtractAllWalletRankingsAsync(),
            CronUtils.ConvertFromPeriodRecurrence(TimeSpan.FromHours(23)));

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

    public async Task ExtractAllWalletRankingsAsync()
    {
        var rankingDTOs = await QueryClient.ListWalletRankingAsync();
        var rankings = rankingDTOs.Select(x => {
            var id = Guid.NewGuid();
            return new WalletRanking()
            {
                Id = id,
                Address = x.Address,
                StakedRank = x.StakedRank,
                StakedAmount = x.StakedAmount,
                BalanceRank = x.BalanceRank,
                BalanceAmount = x.BalanceAmount,
                LPerRanks = x.PoolRankings.Select(x => new PoolLPerRank()
                {
                    WalletRankingId = id,
                    PoolId = x.PoolId,
                    GammBalance = x.LPTokenBalance,
                    Rank = x.Rank
                }).ToList()
            };
        }).ToArray();

        var scope = Provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PinewolyticsContext>();
        var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            await dbContext.WalletRankings.ExecuteDeleteAsync();
            await dbContext.PoolLPerRanks.ExecuteDeleteAsync();

            var updateTimestamp = await dbContext.UpdateTimestamps
                .Where(x => x.Key == UpdateTimestamp.WalletRankingsKey)
                .SingleOrDefaultAsync();

            if (updateTimestamp is null)
            {
                updateTimestamp = new UpdateTimestamp()
                {
                    Key = UpdateTimestamp.WalletRankingsKey,
                    Timestamp = rankingDTOs.Max(x => x.LastUpdatedAt.UtcDateTime)
                };

                dbContext.UpdateTimestamps.Add(updateTimestamp);
            } else
            {
                updateTimestamp.Timestamp = rankingDTOs.Max(x => x.LastUpdatedAt);
            }

            await dbContext.SaveChangesAsync();
            dbContext.ChangeTracker.Clear();

            foreach (var chunk in rankings.Chunk(25000))
            {
                dbContext.WalletRankings.AddRange(chunk);
                await dbContext.SaveChangesAsync();
                dbContext.ChangeTracker.Clear();
            }

             //Recursively finds Swaps

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

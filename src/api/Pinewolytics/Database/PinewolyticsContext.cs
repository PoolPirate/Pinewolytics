using Microsoft.EntityFrameworkCore;
using Pinewolytics.Entities;
using Pinewolytics.Models.Entities;

namespace Pinewolytics.Database;

public class PinewolyticsContext : DbContext
{
    public DbSet<UpdateTimestamp> UpdateTimestamps { get; set; }

    public DbSet<ScheduledQuery> ScheduledQueries { get; set; }
    
    public DbSet<ICNSName> ICNSNames { get; set; }

    public DbSet<ProtoRevTransaction> ProtoRevTransactions { get; set; }
    public DbSet<ProtoRevSwap> ProtoRevSwaps { get; set; }

    public DbSet<WalletRanking> WalletRankings { get; set; }
    public DbSet<PoolLPerRank> PoolLPerRanks { get; set; }

    public PinewolyticsContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UpdateTimestamp>(b =>
        {
            b.Property(x => x.Key);
            b.HasKey(x => x.Key);

            b.Property(x => x.Timestamp);

            b.ToTable("UpdateTimestamps");
        });

        modelBuilder.Entity<ScheduledQuery>(b =>
        {
            b.Property(x => x.Name);
            b.HasKey(x => x.Name);

            b.Property(x => x.Query)
            .IsRequired();

            b.Property(x => x.TypeName)
            .IsRequired();

            b.Property(x => x.Interval)
            .IsRequired();

            b.ToTable("ScheduledQueries");
        });

        modelBuilder.Entity<ICNSName>(b =>
        {
            b.Property(x => x.Name);
            b.HasKey(x => x.Name);

            b.Property(x => x.OSMOAddress);
            b.HasIndex(x => x.OSMOAddress);

            b.ToTable("ICNSNames");
        });

        modelBuilder.Entity<ProtoRevTransaction>(b =>
        {
            b.Property(x => x.Id);
            b.HasKey(x => x.Id);

            b.Property(x => x.Hash);
            b.HasIndex(x => x.Hash)
            .IsUnique();
            b.Property(x => x.TimeStamp);

            b.Property(x => x.TxFrom);
            b.HasIndex(x => x.TxFrom);

            b.HasMany(x => x.Swaps)
            .WithOne(x => x.Transaction)
            .HasForeignKey(x => x.TransactionId);

            b.ToTable("ProtoRevTransactions");
        });

        modelBuilder.Entity<ProtoRevSwap>(b =>
        {
            b.Property(x => x.Id);
            b.HasKey(x => x.Id);

            b.Property(x => x.Currency);
            b.Property(x => x.Profit);
            b.Property(x => x.ProfitUSD);

            b.ToTable("ProtoRevSwaps");
        });

        modelBuilder.Entity<WalletRanking>(b =>
        {
            b.Property(x => x.Id);
            b.HasKey(x => x.Id);

            b.Property(x => x.Address);
            b.HasIndex(x => x.Address)
            .IsUnique();

            b.Property(x => x.StakedRank);
            b.Property(x => x.StakedAmount);

            b.Property(x => x.BalanceRank);
            b.Property(x => x.BalanceAmount);

            b.HasMany(x => x.LPerRanks)
            .WithOne(x => x.WalletRanking)
            .HasForeignKey(x => x.WalletRankingId);

            b.ToTable("WalletRankings");
        });

        modelBuilder.Entity<PoolLPerRank>(b =>
        {
            b.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityAlwaysColumn();
            b.HasKey(x => x.Id);

            b.Property(x => x.PoolId);
            b.Property(x => x.Rank);
            b.Property(x => x.GammBalance);

            b.ToTable("PoolLPerRanks");
        });
    }
}

using Microsoft.EntityFrameworkCore;
using Pinewolytics.Models.Entities;

namespace Pinewolytics.Database;

public class PinewolyticsContext : DbContext
{
    public DbSet<ScheduledQuery> ScheduledQueries { get; set; }
    
    public DbSet<ICNSName> ICNSNames { get; set; }

    public DbSet<ProtoRevTransaction> ProtoRevTransactions { get; set; }
    public DbSet<ProtoRevSwap> ProtoRevSwaps { get; set; }

    public PinewolyticsContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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
    }
}

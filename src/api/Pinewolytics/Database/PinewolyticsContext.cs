using Microsoft.EntityFrameworkCore;
using Pinewolytics.Models.Entities;

namespace Pinewolytics.Database;

public class PinewolyticsContext : DbContext
{
    public DbSet<ScheduledQuery> ScheduledQueries { get; set; }

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

            b.Property(x => x.Interval)
            .IsRequired();

            b.ToTable("ScheduledQueries");
        });
    }
}

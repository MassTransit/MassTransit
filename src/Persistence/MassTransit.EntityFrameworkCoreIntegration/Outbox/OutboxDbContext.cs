using MassTransit.Transports.Outbox.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.EntityFrameworkCoreIntegration.Outbox
{
    public class OutboxDbContext : DbContext
    {
        public OutboxDbContext(DbContextOptions<OutboxDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mt");
            modelBuilder.ApplyConfiguration(new OutboxLockConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
            modelBuilder.ApplyConfiguration(new OutboxSweeperConfiguration());
        }

        public DbSet<OnRampLock> MassTransitOutboxLocks { get; set; }
        public DbSet<OnRampMessage> MassTransitOutboxMessages { get; set; }
        public DbSet<OnRampSweeper> MassTransitOutboxSweepers { get; set; }
    }
}

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

        public DbSet<OutboxLock> MassTransitOutboxLocks { get; set; }
        public DbSet<OutboxMessage> MassTransitOutboxMessages { get; set; }
        public DbSet<OutboxSweeper> MassTransitOutboxSweepers { get; set; }
    }
}

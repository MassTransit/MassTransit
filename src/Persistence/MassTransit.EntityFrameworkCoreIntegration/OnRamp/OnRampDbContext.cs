using MassTransit.Transports.OnRamp.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassTransit.EntityFrameworkCoreIntegration.OnRamp
{
    public class OnRampDbContext : DbContext
    {
        public OnRampDbContext(DbContextOptions<OnRampDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("mt");
            modelBuilder.ApplyConfiguration(new OnRampLockConfiguration());
            modelBuilder.ApplyConfiguration(new OnRampMessageConfiguration());
            modelBuilder.ApplyConfiguration(new OnRampSweeperConfiguration());
        }

        public DbSet<OnRampLock> MassTransitOnRampLocks { get; set; }
        public DbSet<OnRampMessage> MassTransitOnRampMessages { get; set; }
        public DbSet<OnRampSweeper> MassTransitOnRampSweepers { get; set; }
    }
}

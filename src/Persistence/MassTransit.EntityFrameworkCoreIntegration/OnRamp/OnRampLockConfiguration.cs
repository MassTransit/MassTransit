using MassTransit.Transports.OnRamp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.EntityFrameworkCoreIntegration.OnRamp
{
    public class OnRampLockConfiguration : IEntityTypeConfiguration<OnRampLock>
    {
        public void Configure(EntityTypeBuilder<OnRampLock> builder)
        {
            builder.ToTable("Locks", "mt");

            builder.HasKey(o => new { o.OnRampName, o.LockName });
        }
    }
}

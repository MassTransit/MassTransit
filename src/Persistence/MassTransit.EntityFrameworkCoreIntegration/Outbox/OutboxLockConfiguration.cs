using MassTransit.Transports.Outbox.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.EntityFrameworkCoreIntegration.Outbox
{
    public class OutboxLockConfiguration : IEntityTypeConfiguration<OnRampLock>
    {
        public void Configure(EntityTypeBuilder<OnRampLock> builder)
        {
            builder.ToTable("Locks", "mt");

            builder.HasKey(o => new { o.OnRampName, o.LockName });
        }
    }
}

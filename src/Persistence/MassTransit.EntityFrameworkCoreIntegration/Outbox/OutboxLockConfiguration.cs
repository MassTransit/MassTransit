using MassTransit.Transports.Outbox.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.EntityFrameworkCoreIntegration.Outbox
{
    public class OutboxLockConfiguration : IEntityTypeConfiguration<OutboxLock>
    {
        public void Configure(EntityTypeBuilder<OutboxLock> builder)
        {
            builder.ToTable("Locks", "mt");

            builder.HasKey(o => new { o.OutboxName, o.LockName });
        }
    }
}

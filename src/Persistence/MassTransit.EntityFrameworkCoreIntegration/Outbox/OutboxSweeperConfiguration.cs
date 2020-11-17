using MassTransit.Transports.Outbox.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace MassTransit.EntityFrameworkCoreIntegration.Outbox
{
    public class OutboxSweeperConfiguration : IEntityTypeConfiguration<OutboxSweeper>
    {
        public void Configure(EntityTypeBuilder<OutboxSweeper> builder)
        {
            builder.ToTable("Sweepers", "mt");

            builder.HasKey(o => new { o.OutboxName, o.InstanceId });

            builder.Property(x => x.LastCheckinTime)
                .HasConversion(
                    v => v.Ticks,
                    v => new DateTime(v, DateTimeKind.Utc));

            builder.Property(x => x.CheckinInterval)
                .HasConversion(
                    v => Convert.ToInt64(v.TotalMilliseconds),
                    v => TimeSpan.FromMilliseconds(v));
        }
    }
}

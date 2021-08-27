using MassTransit.Transports.OnRamp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace MassTransit.EntityFrameworkCoreIntegration.OnRamp
{
    public class OnRampSweeperConfiguration : IEntityTypeConfiguration<OnRampSweeper>
    {
        public void Configure(EntityTypeBuilder<OnRampSweeper> builder)
        {
            builder.ToTable("Sweepers", "mt");

            builder.HasKey(o => new { o.OnRampName, o.InstanceId });

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

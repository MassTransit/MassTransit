using MassTransit.Transports.OnRamp;
using MassTransit.Transports.OnRamp.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.EntityFrameworkCoreIntegration.OnRamp
{
    public class OnRampMessageConfiguration : IEntityTypeConfiguration<OnRampMessage>
    {
        public void Configure(EntityTypeBuilder<OnRampMessage> builder)
        {
            builder.ToTable("Messages", "mt");

            builder.HasKey(o => new { o.OnRampName, o.Id });

            builder.Property(x => x.SerializedMessage)
                .IsRequired()
                .HasConversion(new JsonValueConverter<OnRampSerializedMessage>())
                .Metadata.SetValueComparer(new JsonValueComparer<OnRampSerializedMessage>());

            builder.Property(x => x.Added)
                .HasConversion(
                    v => v.Ticks,
                    v => new System.DateTime(v, System.DateTimeKind.Utc));
        }
    }
}

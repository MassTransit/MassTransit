using MassTransit.Serialization;
using MassTransit.Transports.Outbox;
using MassTransit.Transports.Outbox.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MassTransit.EntityFrameworkCoreIntegration.Outbox
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OnRampMessage>
    {
        public void Configure(EntityTypeBuilder<OnRampMessage> builder)
        {
            builder.ToTable("Messages", "mt");

            builder.HasKey(o => new { o.OnRampName, o.Id });

            builder.Property(x => x.SerializedMessage)
                .IsRequired()
                .HasConversion(new JsonValueConverter<JsonSerializedMessage>())
                .Metadata.SetValueComparer(new JsonValueComparer<JsonSerializedMessage>());

            builder.Property(x => x.Added)
                .HasConversion(
                    v => v.Ticks,
                    v => new System.DateTime(v, System.DateTimeKind.Utc));
        }
    }
}

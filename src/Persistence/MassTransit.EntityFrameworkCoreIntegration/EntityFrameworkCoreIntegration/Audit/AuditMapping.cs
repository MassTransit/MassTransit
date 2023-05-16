namespace MassTransit.EntityFrameworkCoreIntegration.Audit
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;


    public class AuditMapping :
        IEntityTypeConfiguration<AuditRecord>
    {
        readonly string _tableName;
        readonly string _schemaName;

        public AuditMapping(string tableName, string schemaName = null)
        {
            _tableName = tableName;
            _schemaName = schemaName;
        }

        public void Configure(EntityTypeBuilder<AuditRecord> builder)
        {
            if (string.IsNullOrWhiteSpace(_schemaName))
            {
                builder.ToTable(_tableName);
            }
            else
            {
                builder.ToTable(_tableName, _schemaName);
            }

            builder.HasKey(x => x.AuditRecordId);
            builder.Property(x => x.AuditRecordId)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.ContextType);
            builder.Property(x => x.MessageId);
            builder.Property(x => x.InitiatorId);
            builder.Property(x => x.ConversationId);
            builder.Property(x => x.CorrelationId);
            builder.Property(x => x.RequestId);
            builder.Property(x => x.SentTime);
            builder.Property(x => x.SourceAddress);
            builder.Property(x => x.DestinationAddress);
            builder.Property(x => x.ResponseAddress);
            builder.Property(x => x.FaultAddress);
            builder.Property(x => x.InputAddress);
            builder.Property(x => x.MessageType);

            builder.Property(x => x.Headers)
                .HasConversion(new JsonValueConverter<Dictionary<string, string>>());
            builder.Property(x => x.Custom)
                .HasConversion(new JsonValueConverter<Dictionary<string, string>>());
            builder.Property(x => x.Message)
                .HasConversion(new JsonValueConverter<object>());
        }
    }
}

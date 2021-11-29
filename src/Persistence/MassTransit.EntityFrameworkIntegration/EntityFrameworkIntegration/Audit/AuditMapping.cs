namespace MassTransit.EntityFrameworkIntegration.Audit
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;


    public class AuditMapping : EntityTypeConfiguration<AuditRecord>
    {
        public AuditMapping(string tableName)
        {
            ToTable(tableName);

            HasKey(x => x.AuditRecordId)
                .Property(x => x.AuditRecordId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(x => x.ContextType);
            Property(x => x.MessageId);
            Property(x => x.InitiatorId);
            Property(x => x.ConversationId);
            Property(x => x.CorrelationId);
            Property(x => x.RequestId);
            Property(x => x.SentTime);
            Property(x => x.SourceAddress);
            Property(x => x.DestinationAddress);
            Property(x => x.ResponseAddress);
            Property(x => x.FaultAddress);
            Property(x => x.InputAddress);

            Property(x => x.MessageType);
            Property(x => x._headers).HasColumnName("Headers");
            Property(x => x._custom).HasColumnName("Custom");
            Property(x => x._message).HasColumnName("Message");
        }
    }
}

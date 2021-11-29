namespace MassTransit.EntityFrameworkIntegration.Audit
{
    using System.Data.Entity;
    using System.Threading.Tasks;
    using MassTransit.Audit;


    public class EntityFrameworkAuditStore : IMessageAuditStore
    {
        readonly string _auditTableName;
        readonly string _nameOrConnectionString;

        public EntityFrameworkAuditStore(string nameOrConnectionString, string auditTableName)
        {
            _nameOrConnectionString = nameOrConnectionString;
            _auditTableName = auditTableName;
        }

        public DbContext AuditContext => new AuditDbContext(_nameOrConnectionString, _auditTableName);

        async Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            using (var dbContext = AuditContext)
            {
                var auditRecord = AuditRecord.Create(message, TypeCache<T>.ShortName, metadata);

                dbContext.Set<AuditRecord>().Add(auditRecord);

                await dbContext.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}

namespace MassTransit.EntityFrameworkCoreIntegration.Audit
{
    using System.Threading.Tasks;
    using MassTransit.Audit;
    using Microsoft.EntityFrameworkCore;


    public class EntityFrameworkAuditStore :
        IMessageAuditStore
    {
        readonly string _auditTableName;
        readonly string _auditTableSchema;
        readonly DbContextOptions _contextOptions;

        public EntityFrameworkAuditStore(DbContextOptions contextOptions, string auditTableName, string auditTableSchema = null)
        {
            _contextOptions = contextOptions;
            _auditTableName = auditTableName;
            _auditTableSchema = auditTableSchema;
        }

        public DbContext AuditContext => new AuditDbContext(_contextOptions, _auditTableName, _auditTableSchema);

        async Task IMessageAuditStore.StoreMessage<T>(T message, MessageAuditMetadata metadata)
        {
            await using var dbContext = AuditContext;
            var auditRecord = AuditRecord.Create(message, TypeCache<T>.ShortName, metadata);

            await dbContext.Set<AuditRecord>().AddAsync(auditRecord).ConfigureAwait(false);

            await dbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}

namespace MassTransit.EntityFrameworkCoreIntegration.Audit
{
    using Microsoft.EntityFrameworkCore;


    public class AuditDbContext :
        DbContext
    {
        readonly string _auditTableName;
        readonly string _auditTableSchema;

        protected AuditDbContext(string auditTableName, string auditTableSchema = null)
        {
            _auditTableName = auditTableName;
            _auditTableSchema = auditTableSchema;
        }

        public AuditDbContext(DbContextOptions options, string auditTableName, string auditTableSchema = null)
            : base(options)
        {
            _auditTableName = auditTableName;
            _auditTableSchema = auditTableSchema;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuditMapping(_auditTableName, _auditTableSchema));
        }
    }
}

namespace MassTransit.EntityFrameworkCoreIntegration.Audit
{
    using Microsoft.EntityFrameworkCore;


    public class AuditDbContext :
        DbContext
    {
        readonly string _auditTableName;

        protected AuditDbContext(string auditTableName)
        {
            _auditTableName = auditTableName;
        }

        public AuditDbContext(DbContextOptions options, string auditTableName)
            : base(options)
        {
            _auditTableName = auditTableName;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AuditMapping(_auditTableName));
        }
    }
}

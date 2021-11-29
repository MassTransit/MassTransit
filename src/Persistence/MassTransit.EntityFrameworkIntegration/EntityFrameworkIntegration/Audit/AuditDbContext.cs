namespace MassTransit.EntityFrameworkIntegration.Audit
{
    using System.Data.Entity;


    public class AuditDbContext : DbContext
    {
        readonly string _auditTableName;

        public AuditDbContext(string nameOrConnectionString, string auditTableName)
            : base(nameOrConnectionString)
        {
            _auditTableName = auditTableName;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AuditMapping(_auditTableName));
        }
    }
}

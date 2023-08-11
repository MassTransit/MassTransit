namespace MassTransit.EntityFrameworkCoreIntegration.Tests.ReliableMessaging
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;


    public class ReliableDbContext :
        SagaDbContext
    {
        public ReliableDbContext(DbContextOptions<ReliableDbContext> options)
            : base(options)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new ReliableStateMap(); }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddTransactionalOutboxEntities();
        }
    }
}

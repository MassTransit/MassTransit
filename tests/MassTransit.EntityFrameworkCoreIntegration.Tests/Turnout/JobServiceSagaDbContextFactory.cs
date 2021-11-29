namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Turnout
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Shared;


    public class JobServiceSagaDbContextFactory :
        IDesignTimeDbContextFactory<JobServiceSagaDbContext>
    {
        public JobServiceSagaDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<JobServiceSagaDbContext> optionsBuilder = new SqlServerTestDbParameters()
                .GetDbContextOptions<JobServiceSagaDbContext>();

            return new JobServiceSagaDbContext(optionsBuilder.Options);
        }

        public JobServiceSagaDbContext CreateDbContext(DbContextOptionsBuilder<JobServiceSagaDbContext> optionsBuilder)
        {
            return new JobServiceSagaDbContext(optionsBuilder.Options);
        }
    }
}

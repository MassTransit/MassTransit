namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Turnout
{
    using JobService;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Shared;


    public class JobServiceSagaDbContextFactory :
        IDesignTimeDbContextFactory<JobServiceSagaDbContext>
    {
        public JobServiceSagaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new SqlServerTestDbParameters()
                .GetDbContextOptions<JobServiceSagaDbContext>();

            return new JobServiceSagaDbContext(optionsBuilder.Options);
        }

        public JobServiceSagaDbContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            return new JobServiceSagaDbContext(optionsBuilder.Options);
        }
    }
}

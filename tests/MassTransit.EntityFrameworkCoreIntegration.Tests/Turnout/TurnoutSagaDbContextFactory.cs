namespace MassTransit.EntityFrameworkCoreIntegration.Tests.Turnout
{
    using EntityFrameworkCoreIntegration.Turnout;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Shared;


    public class TurnoutSagaDbContextFactory :
        IDesignTimeDbContextFactory<TurnoutSagaDbContext>
    {
        public TurnoutSagaDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new SqlServerTestDbParameters()
                .GetDbContextOptions(typeof(TurnoutSagaDbContext));

            return new TurnoutSagaDbContext(optionsBuilder.Options);
        }

        public TurnoutSagaDbContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
        {
            return new TurnoutSagaDbContext(optionsBuilder.Options);
        }
    }
}

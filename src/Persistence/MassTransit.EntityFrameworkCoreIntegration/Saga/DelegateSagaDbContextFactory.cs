namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public class DelegateSagaDbContextFactory<TSaga> :
        ISagaDbContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly Func<DbContext> _dbContextFactory;

        public DelegateSagaDbContextFactory(Func<DbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public DbContext Create()
        {
            return _dbContextFactory();
        }

        public DbContext CreateScoped<T>(ConsumeContext<T> context)
            where T : class
        {
            return _dbContextFactory();
        }

        public async Task ReleaseAsync(DbContext dbContext)
        {
            await dbContext.DisposeAsync();
        }
    }
}

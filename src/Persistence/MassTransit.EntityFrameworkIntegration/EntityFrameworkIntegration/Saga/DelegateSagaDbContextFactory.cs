namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Data.Entity;


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

        public void Release(DbContext dbContext)
        {
            dbContext.Dispose();
        }
    }
}

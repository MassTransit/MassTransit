namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System.Threading.Tasks;
    using EntityFrameworkCoreIntegration;
    using Microsoft.EntityFrameworkCore;


    public class ContainerSagaDbContextFactory<TContext, TSaga> :
        ISagaDbContextFactory<TSaga>
        where TContext : DbContext
        where TSaga : class, ISaga
    {
        readonly TContext _dbContext;

        public ContainerSagaDbContextFactory(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbContext Create()
        {
            return _dbContext;
        }

        public DbContext CreateScoped<T>(ConsumeContext<T> context)
            where T : class
        {
            return _dbContext;
        }

        public ValueTask ReleaseAsync(DbContext dbContext)
        {
            return default;
        }
    }
}

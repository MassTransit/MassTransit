namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public class DbContextSagaRepositoryContextFactory<TContext, TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TContext : DbContext
        where TSaga : class, ISaga
    {
        readonly Func<TContext> _dbContextFactory;

        public DbContextSagaRepositoryContextFactory(Func<TContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId = default)
            where T : class
        {
            var dbContext = _dbContextFactory();

            return Task.FromResult<SagaRepositoryContext<TSaga, T>>(new DbContextSagaRepositoryContext<TContext, TSaga, T>(dbContext));
        }

        public Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            var dbContext = _dbContextFactory();

            return Task.FromResult<SagaRepositoryContext<TSaga>>(new DbContextSagaRepositoryContext<TContext, TSaga>(dbContext));
        }

        public void Probe(ProbeContext context)
        {
            var dbContext = _dbContextFactory();
            try
            {
                context.Add("persistence", "entity-framework");
                context.Add("entities", dbContext.Model.GetEntityTypes().Select(type => type.Name).ToArray());
            }
            finally
            {
                dbContext.Dispose();
            }
        }
    }
}

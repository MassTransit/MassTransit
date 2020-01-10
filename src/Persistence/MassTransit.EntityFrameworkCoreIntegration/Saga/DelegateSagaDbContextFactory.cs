using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Saga;
using Microsoft.EntityFrameworkCore;


namespace MassTransit.EntityFrameworkCoreIntegration.Saga
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;


    public class DelegateSagaDbContextFactory :
        ISagaDbContextFactory
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


    public class DbContextSagaRepositoryContext<TContext, TSaga, TMessage> :
        SagaRepositoryContext<TSaga, TMessage>,
        IDisposable
        where TContext : DbContext
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly TContext _dbContext;

        public DbContextSagaRepositoryContext(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            throw new NotImplementedException();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            throw new NotImplementedException();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public Task<SagaRepositoryQueryContext<TSaga, TMessage>> Query(ISagaQuery<TSaga> query)
        {
            throw new NotImplementedException();
        }

        public Task Faulted(Exception exception)
        {
            throw new NotImplementedException();
        }
    }

    public class DbContextSagaRepositoryContext<TContext, TSaga> :
        SagaRepositoryContext<TSaga>,
        IDisposable
        where TContext : DbContext
        where TSaga : class, ISaga
    {
        readonly TContext _dbContext;

        public DbContextSagaRepositoryContext(TContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query)
        {
            throw new NotImplementedException();
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            throw new NotImplementedException();
        }

        public Task Faulted(Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}

namespace MassTransit.EntityFrameworkIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using Middleware;


    public class DbContextSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>,
        IDisposable
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly DbContext _dbContext;
        readonly ISagaConsumeContextFactory<DbContext, TSaga> _factory;
        readonly SemaphoreSlim _inUse = new SemaphoreSlim(1);
        readonly ISagaRepositoryLockStrategy<TSaga> _lockStrategy;

        public DbContextSagaRepositoryContext(DbContext dbContext, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DbContext, TSaga> factory, ISagaRepositoryLockStrategy<TSaga> lockStrategy)
            : base(consumeContext, dbContext)
        {
            _dbContext = dbContext;
            _consumeContext = consumeContext;
            _factory = factory;
            _lockStrategy = lockStrategy;
        }

        public void Dispose()
        {
            _inUse.Dispose();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_dbContext, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            var entity = _dbContext.Set<TSaga>().Add(instance);
            try
            {
                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_dbContext, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Because we will still be using the same dbContext, we need to reset the entry we just tried to pre-insert (likely a duplicate), so
                // on the next save changes (which is the update), it will pass.
                // see here for details: https://www.davideguida.com/how-to-reset-the-entities-state-on-a-entity-framework-db-context/
                _dbContext.Entry(entity).State = EntityState.Detached;

                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);

                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _lockStrategy.Load(_dbContext, correlationId, CancellationToken).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_dbContext, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public async Task Save(SagaConsumeContext<TSaga> context)
        {
            await _inUse.WaitAsync(context.CancellationToken).ConfigureAwait(false);
            try
            {
                _dbContext.Set<TSaga>().Add(context.Saga);

                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            await _inUse.WaitAsync(context.CancellationToken).ConfigureAwait(false);
            try
            {
                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            await _inUse.WaitAsync(context.CancellationToken).ConfigureAwait(false);
            try
            {
                _dbContext.Set<TSaga>().Remove(context.Saga);

                await _dbContext.SaveChangesAsync(CancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _inUse.Release();
            }
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            var entity = _dbContext.ChangeTracker.Entries<TSaga>().FirstOrDefault(x => x.Entity.CorrelationId == context.Saga.CorrelationId);
            if (entity != null)
                entity.State = EntityState.Unchanged;

            return Task.CompletedTask;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_dbContext, consumeContext, instance, mode);
        }
    }


    public class DbContextSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DbContext _dbContext;

        public DbContextSagaRepositoryContext(DbContext dbContext, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _dbContext = dbContext;
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            IList<Guid> results = await _dbContext.Set<TSaga>()
                .AsNoTracking()
                .Where(query.FilterExpression)
                .Select(x => x.CorrelationId)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new DefaultSagaRepositoryQueryContext<TSaga>(this, results);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _dbContext.Set<TSaga>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.CorrelationId == correlationId);
        }
    }
}

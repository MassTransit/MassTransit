namespace MassTransit.EntityFrameworkCoreIntegration.Saga.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Context;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;


    public class DbContextSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly DbContext _dbContext;
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<DbContext, TSaga> _factory;
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

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_dbContext, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            EntityEntry<TSaga> entry = await _dbContext.Set<TSaga>().AddAsync(instance, CancellationToken).ConfigureAwait(false);
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
                entry.State = EntityState.Detached;

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

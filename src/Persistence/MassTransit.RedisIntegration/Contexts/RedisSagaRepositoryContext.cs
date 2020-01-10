namespace MassTransit.RedisIntegration.Contexts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using GreenPipes.Util;
    using Saga;


    public class RedisSagaRepositoryContext<TSaga, T> :
        SagaRepositoryContext<TSaga, T>,
        IAsyncDisposable
        where TSaga : class, IVersionedSaga
        where T : class
    {
        readonly DatabaseContext<TSaga> _context;
        readonly ConsumeContext<T> _consumeContext;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public RedisSagaRepositoryContext(DatabaseContext<TSaga> context, ConsumeContext<T> consumeContext,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public DatabaseContext<TSaga> Context => _context;

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            return _context.DisposeAsync(cancellationToken);
        }

        public Task<SagaConsumeContext<TSaga, T>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_context,_consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, T>> Insert(TSaga instance)
        {
            try
            {
                await _context.Insert<T>(instance).ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, T>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _consumeContext.LogInsertFault<TSaga, T>(ex, instance.CorrelationId);

                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, T>> Load(Guid correlationId)
        {
            var instance = await _context.Load<T>(correlationId).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task<SagaRepositoryQueryContext<TSaga, T>> Query(ISagaQuery<TSaga> query)
        {
            throw new NotImplementedByDesignException("Redis saga repository does not support queries");
        }

        public Task Faulted(Exception exception)
        {
            return TaskUtil.Completed;
        }
    }


    public class RedisSagaRepositoryContext<TSaga> :
        SagaRepositoryContext<TSaga>,
        IAsyncDisposable
        where TSaga : class, IVersionedSaga
    {
        readonly DatabaseContext<TSaga> _context;

        public RedisSagaRepositoryContext(DatabaseContext<TSaga> context)
        {
            _context = context;
        }

        public DatabaseContext<TSaga> Context => _context;

        public Task DisposeAsync(CancellationToken cancellationToken)
        {
            return _context.DisposeAsync(cancellationToken);
        }

        public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query)
        {
            throw new NotImplementedByDesignException("Redis saga repository does not support queries");
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.Load<TSaga>(correlationId);
        }

        public Task Faulted(Exception exception)
        {
            return TaskUtil.Completed;
        }
    }
}

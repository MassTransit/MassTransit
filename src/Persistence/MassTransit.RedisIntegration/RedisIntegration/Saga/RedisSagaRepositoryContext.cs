namespace MassTransit.RedisIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using Middleware;


    public class RedisSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>,
        IAsyncDisposable
        where TSaga : class, ISagaVersion
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly DatabaseContext<TSaga> _context;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public RedisSagaRepositoryContext(DatabaseContext<TSaga> context, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
            : base(consumeContext)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                await _context.Insert(instance).ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);

                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _context.Load(correlationId).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return _context.Add(context);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            return _context.Update(context);
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _context.Delete(context);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_context, consumeContext, instance, mode);
        }
    }


    public class RedisSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>,
        IAsyncDisposable
        where TSaga : class, ISagaVersion
    {
        readonly DatabaseContext<TSaga> _context;

        public RedisSagaRepositoryContext(DatabaseContext<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }

        public Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            throw new NotImplementedByDesignException("Redis saga repository does not support queries");
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.Load(correlationId);
        }
    }
}

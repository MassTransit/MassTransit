namespace MassTransit.RedisIntegration.Saga
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using Middleware;
    using Serialization;


    public class RedisSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>,
        IAsyncDisposable
        where TSaga : class, ISagaVersion
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _context;
        readonly DatabaseContext<TSaga> _dbContext;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public RedisSagaRepositoryContext(DatabaseContext<TSaga> dbContext, ConsumeContext<TMessage> context,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
            : base(context)
        {
            _dbContext = dbContext;
            _context = context;
            _factory = factory;
        }

        public ValueTask DisposeAsync()
        {
            return _dbContext.DisposeAsync();
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_dbContext, _context, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                await _dbContext.Insert(_context.SerializerContext, instance).ConfigureAwait(false);

                _context.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_dbContext, _context, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _context.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);

                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _dbContext.Load(_context.SerializerContext, correlationId).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_dbContext, _context, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return _dbContext.Add(context);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            return _dbContext.Update(context);
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _dbContext.Delete(context);
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
            return _factory.CreateSagaConsumeContext(_dbContext, consumeContext, instance, mode);
        }
    }


    public class RedisSagaRepositoryContext<TSaga> :
        BasePipeContext,
        LoadSagaRepositoryContext<TSaga>,
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

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.Load(SystemTextJsonMessageSerializer.Instance, correlationId);
        }
    }
}

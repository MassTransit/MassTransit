namespace MassTransit.DapperIntegration.Context
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Context;
    using Saga;


    public class DapperSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly DatabaseContext<TSaga> _context;
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> _factory;

        public DapperSagaRepositoryContext(DatabaseContext<TSaga> context, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<DatabaseContext<TSaga>, TSaga> factory)
            : base(consumeContext, context)
        {
            _context = context;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_context, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            await _context.InsertAsync(instance, CancellationToken).ConfigureAwait(false);

            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _context.LoadAsync<TSaga>(correlationId, CancellationToken).ConfigureAwait(false);
            if (instance == default)
                return default;

            return await _factory.CreateSagaConsumeContext(_context, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }
    }


    public class DapperSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly DatabaseContext<TSaga> _context;

        public DapperSagaRepositoryContext(DatabaseContext<TSaga> context, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _context = context;
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default)
        {
            var instances = await _context.QueryAsync(query.FilterExpression, cancellationToken).ConfigureAwait(false);

            return new LoadedSagaRepositoryQueryContext<TSaga>(this, instances.ToList());
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _context.LoadAsync<TSaga>(correlationId, CancellationToken);
        }
    }
}

namespace MassTransit.MartenIntegration.Saga.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using Marten;
    using MassTransit.Context;
    using MassTransit.Saga;


    public class MartenSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly IDocumentSession _session;
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<IDocumentSession, TSaga> _factory;

        public MartenSagaRepositoryContext(IDocumentSession session, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<IDocumentSession, TSaga> factory)
            : base(consumeContext)
        {
            _session = session;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public MartenSagaRepositoryContext(ConsumeContext<TMessage> context, params object[] payloads)
            : base(context, payloads)
        {
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_session, consumeContext, instance, mode);
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_session, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                _session.Store(instance);
                await _session.SaveChangesAsync(CancellationToken).ConfigureAwait(false);

                this.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_session, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);
            }

            return default;
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _session.LoadAsync<TSaga>(correlationId).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_session, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }
    }


    public class MartenSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly IQuerySession _session;

        public MartenSagaRepositoryContext(IQuerySession session, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _session = session;
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _session.LoadAsync<TSaga>(correlationId);
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken = default)
        {
            IReadOnlyList<TSaga> instances = await _session.Query<TSaga>()
                .Where(query.FilterExpression)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return new LoadedSagaRepositoryQueryContext<TSaga>(this, instances.ToList());
        }
    }
}

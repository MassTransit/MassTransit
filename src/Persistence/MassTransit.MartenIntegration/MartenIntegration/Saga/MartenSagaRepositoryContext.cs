namespace MassTransit.MartenIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using Marten;
    using MassTransit.Saga;
    using Middleware;


    public class MartenSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<IDocumentSession, TSaga> _factory;
        readonly IDocumentSession _session;

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

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            _session.Store(context.Saga);

            return _session.SaveChangesAsync(CancellationToken);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            return _session.SaveChangesAsync(CancellationToken);
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            _session.Delete(context.Saga);

            return _session.SaveChangesAsync(CancellationToken);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            _session.Eject(context.Saga);

            return Task.CompletedTask;
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

            return new LoadedSagaRepositoryQueryContext<TSaga>(this, instances);
        }
    }
}

namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Context;
    using Logging;
    using MassTransit.Saga;
    using Middleware;
    using NHibernate;
    using NHibernate.Exceptions;


    public class NHibernateSagaRepositoryContext<TSaga, TMessage> :
        ConsumeContextScope<TMessage>,
        SagaRepositoryContext<TSaga, TMessage>
        where TSaga : class, ISaga
        where TMessage : class
    {
        readonly ConsumeContext<TMessage> _consumeContext;
        readonly ISagaConsumeContextFactory<ISession, TSaga> _factory;
        readonly ISession _session;

        public NHibernateSagaRepositoryContext(ISession session, ConsumeContext<TMessage> consumeContext,
            ISagaConsumeContextFactory<ISession, TSaga> factory)
            : base(consumeContext)
        {
            _session = session;
            _consumeContext = consumeContext;
            _factory = factory;
        }

        public Task<SagaConsumeContext<TSaga, TMessage>> Add(TSaga instance)
        {
            return _factory.CreateSagaConsumeContext(_session, _consumeContext, instance, SagaConsumeContextMode.Add);
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Insert(TSaga instance)
        {
            try
            {
                await _session.SaveAsync(instance).ConfigureAwait(false);
                await _session.FlushAsync().ConfigureAwait(false);

                _consumeContext.LogInsert<TSaga, TMessage>(instance.CorrelationId);

                return await _factory.CreateSagaConsumeContext(_session, _consumeContext, instance, SagaConsumeContextMode.Insert).ConfigureAwait(false);
            }
            catch (GenericADOException ex)
            {
                _consumeContext.LogInsertFault<TSaga, TMessage>(ex, instance.CorrelationId);

                return default;
            }
        }

        public async Task<SagaConsumeContext<TSaga, TMessage>> Load(Guid correlationId)
        {
            var instance = await _session.GetAsync<TSaga>(correlationId, LockMode.Upgrade).ConfigureAwait(false);
            if (instance == null)
                return default;

            return await _factory.CreateSagaConsumeContext(_session, _consumeContext, instance, SagaConsumeContextMode.Load).ConfigureAwait(false);
        }

        public Task Save(SagaConsumeContext<TSaga> context)
        {
            return _session.SaveAsync(context.Saga, CancellationToken);
        }

        public Task Update(SagaConsumeContext<TSaga> context)
        {
            return _session.SaveAsync(context.Saga, CancellationToken);
        }

        public Task Delete(SagaConsumeContext<TSaga> context)
        {
            return _session.DeleteAsync(context.Saga, CancellationToken);
        }

        public Task Discard(SagaConsumeContext<TSaga> context)
        {
            return Task.CompletedTask;
        }

        public Task Undo(SagaConsumeContext<TSaga> context)
        {
            _session.Evict(context.Saga);

            return Task.CompletedTask;
        }

        public Task<SagaConsumeContext<TSaga, T>> CreateSagaConsumeContext<T>(ConsumeContext<T> consumeContext, TSaga instance, SagaConsumeContextMode mode)
            where T : class
        {
            return _factory.CreateSagaConsumeContext(_session, consumeContext, instance, mode);
        }
    }


    public class NHibernateSagaRepositoryContext<TSaga> :
        BasePipeContext,
        SagaRepositoryContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISession _session;

        public NHibernateSagaRepositoryContext(ISession session, CancellationToken cancellationToken)
            : base(cancellationToken)
        {
            _session = session;
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _session.GetAsync<TSaga>(correlationId, LockMode.None);
        }

        public async Task<SagaRepositoryQueryContext<TSaga>> Query(ISagaQuery<TSaga> query, CancellationToken cancellationToken)
        {
            IList<TSaga> instances = await _session.QueryOver<TSaga>()
                .Where(query.FilterExpression)
                .ListAsync<TSaga>(cancellationToken)
                .ConfigureAwait(false);

            return new LoadedSagaRepositoryQueryContext<TSaga>(this, instances);
        }
    }
}

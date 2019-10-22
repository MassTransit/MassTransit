namespace MassTransit.MartenIntegration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using Marten;
    using Saga;


    public class MartenSagaRepository<TSaga> : ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>,
        IRetrieveSagaFromRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly IDocumentStore _store;

        public MartenSagaRepository(IDocumentStore store)
        {
            _store = store;
        }

        public async Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            using (var session = _store.QuerySession())
            {
                return await session.Query<TSaga>()
                    .Where(query.FilterExpression)
                    .Select(x => x.CorrelationId)
                    .ToListAsync().ConfigureAwait(false);
            }
        }

        public TSaga GetSaga(Guid correlationId)
        {
            using (var session = _store.QuerySession())
                return session.Load<TSaga>(correlationId);
        }

        public async Task<TSaga> GetSagaAsync(Guid correlationId)
        {
            using (var session = _store.QuerySession())
                return await session.LoadAsync<TSaga>(correlationId).ConfigureAwait(false);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            scope.Set(new {Persistence = "marten"});
        }

        async Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            if (!context.CorrelationId.HasValue)
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            var sagaId = context.CorrelationId.Value;

            using (var session = _store.DirtyTrackedSession())
            {
                if (policy.PreInsertInstance(context, out var instance))
                    await PreInsertSagaInstance(session, context, instance).ConfigureAwait(false);

                if (instance == null)
                    instance = session.Load<TSaga>(sagaId);

                if (instance == null)
                {
                    var missingSagaPipe = new MissingPipe<T>(session, next);
                    await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                }
                else
                {
                    await SendToInstance(context, policy, instance, next, session).ConfigureAwait(false);
                }
            }
        }

        public async Task SendQuery<T>(SagaQueryConsumeContext<TSaga, T> context, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            using (var session = _store.DirtyTrackedSession())
            {
                try
                {
                    IEnumerable<TSaga> instances = await session.Query<TSaga>()
                        .Where(context.Query.FilterExpression)
                        .ToListAsync().ConfigureAwait(false);

                    if (!instances.Any())
                    {
                        var missingSagaPipe = new MissingPipe<T>(session, next);
                        await policy.Missing(context, missingSagaPipe).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (var instance in instances)
                            await SendToInstance(context, policy, instance, next, session).ConfigureAwait(false);
                    }
                }
                catch (SagaException sex)
                {
                    context.LogFault(sex);

                    throw;
                }
                catch (Exception ex)
                {
                    context.LogFault(ex);

                    throw new SagaException(ex.Message, typeof(TSaga), typeof(T), Guid.Empty, ex);
                }
            }
        }

        async Task PreInsertSagaInstance<T>(IDocumentSession session, ConsumeContext<T> context, TSaga instance)
            where T : class
        {
            try
            {
                session.Store(instance);
                await session.SaveChangesAsync(context.CancellationToken).ConfigureAwait(false);

                context.LogInsert(this, instance.CorrelationId);
            }
            catch (Exception ex)
            {
                context.LogInsertFault(this, ex, instance.CorrelationId);
            }
        }

        async Task SendToInstance<T>(ConsumeContext<T> context,
            ISagaPolicy<TSaga, T> policy, TSaga instance,
            IPipe<SagaConsumeContext<TSaga, T>> next, IDocumentSession session)
            where T : class
        {
            var sagaConsumeContext = new MartenSagaConsumeContext<TSaga, T>(session, context, instance);

            try
            {
                sagaConsumeContext.LogUsed();

                await policy.Existing(sagaConsumeContext, next).ConfigureAwait(false);

                if (!sagaConsumeContext.IsCompleted)
                    await session.SaveChangesAsync().ConfigureAwait(false);
            }
            catch (SagaException sex)
            {
                sagaConsumeContext.LogFault(sex);

                throw;
            }
            catch (Exception ex)
            {
                sagaConsumeContext.LogFault(ex);

                throw new SagaException(ex.Message, typeof(TSaga), typeof(T), instance.CorrelationId, ex);
            }
        }


        /// <summary>
        ///     Once the message pipe has processed the saga instance, add it to the saga repository
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        class MissingPipe<TMessage> :
            IPipe<SagaConsumeContext<TSaga, TMessage>>
            where TMessage : class
        {
            readonly IPipe<SagaConsumeContext<TSaga, TMessage>> _next;
            readonly IDocumentSession _session;

            public MissingPipe(IDocumentSession session, IPipe<SagaConsumeContext<TSaga, TMessage>> next)
            {
                _session = session;
                _next = next;
            }

            void IProbeSite.Probe(ProbeContext context)
            {
                _next.Probe(context);
            }

            public async Task Send(SagaConsumeContext<TSaga, TMessage> context)
            {
                SagaConsumeContext<TSaga, TMessage> proxy = new MartenSagaConsumeContext<TSaga, TMessage>(_session,context, context.Saga);

                proxy.LogAdded();

                await _next.Send(proxy).ConfigureAwait(false);

                if (!proxy.IsCompleted)
                {
                    _session.Store(context.Saga);
                    await _session.SaveChangesAsync().ConfigureAwait(false);
                }
            }
        }
    }
}

namespace MassTransit.MartenIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Marten;
    using MassTransit.Saga;


    public class MartenSagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IDocumentStore _documentStore;
        readonly ISagaConsumeContextFactory<IDocumentSession, TSaga> _factory;

        public MartenSagaRepositoryContextFactory(IDocumentStore documentStore, ISagaConsumeContextFactory<IDocumentSession, TSaga> factory)
        {
            _documentStore = documentStore;
            _factory = factory;
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            using var session = _documentStore.DirtyTrackedSession();

            var sagaRepositoryContext = new MartenSagaRepositoryContext<TSaga, T>(session, context, _factory);

            await next.Send(sagaRepositoryContext).ConfigureAwait(false);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            using var session = _documentStore.DirtyTrackedSession();

            var repositoryContext = new MartenSagaRepositoryContext<TSaga, T>(session, context, _factory);

            IReadOnlyList<TSaga> instances = await session.Query<TSaga>()
                .Where(query.FilterExpression)
                .ToListAsync().ConfigureAwait(false);


            var queryContext = new LoadedSagaRepositoryQueryContext<TSaga, T>(repositoryContext, instances);

            await next.Send(queryContext).ConfigureAwait(false);
        }

        public async Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            var session = _documentStore.OpenSession();
            try
            {
                var repositoryContext = new MartenSagaRepositoryContext<TSaga>(session, cancellationToken);

                return await asyncMethod(repositoryContext).ConfigureAwait(false);
            }
            finally
            {
                session.Dispose();
            }
        }

        public void Probe(ProbeContext context)
        {
            context.Add("persistence", "marten");
        }
    }
}

namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    /// <summary>
    /// Supports the InMemorySagaRepository
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class InMemorySagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga> _factory;
        readonly IndexedSagaDictionary<TSaga> _sagas;

        public InMemorySagaRepositoryContextFactory(IndexedSagaDictionary<TSaga> sagas, ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga> factory)
        {
            _sagas = sagas;
            _factory = factory;
        }

        public void Probe(ProbeContext context)
        {
            context.Add("count", _sagas.Count);
            context.Add("persistence", "memory");
        }

        public async Task Send<T>(ConsumeContext<T> context, IPipe<SagaRepositoryContext<TSaga, T>> next)
            where T : class
        {
            await _sagas.MarkInUse(context.CancellationToken).ConfigureAwait(false);

            using var repositoryContext = new InMemorySagaRepositoryContext<TSaga, T>(_sagas, _factory, context);

            await next.Send(repositoryContext).ConfigureAwait(false);
        }

        public async Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, IPipe<SagaRepositoryQueryContext<TSaga, T>> next)
            where T : class
        {
            await _sagas.MarkInUse(context.CancellationToken).ConfigureAwait(false);

            using var repositoryContext = new InMemorySagaRepositoryContext<TSaga, T>(_sagas, _factory, context);

            List<Guid> matchingInstances = _sagas.Where(query).Select(x => x.Instance.CorrelationId).ToList();

            var queryContext = new DefaultSagaRepositoryQueryContext<TSaga, T>(repositoryContext, matchingInstances);

            await next.Send(queryContext).ConfigureAwait(false);
        }

        public Task<T> Execute<T>(Func<SagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken)
            where T : class
        {
            var repositoryContext = new InMemorySagaRepositoryContext<TSaga>(_sagas, cancellationToken);

            return asyncMethod(repositoryContext);
        }
    }
}

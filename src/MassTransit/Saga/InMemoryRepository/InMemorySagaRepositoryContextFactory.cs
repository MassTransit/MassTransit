namespace MassTransit.Saga.InMemoryRepository
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;


    /// <summary>
    /// Supports the InMemorySagaRepository
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class InMemorySagaRepositoryContextFactory<TSaga> :
        ISagaRepositoryContextFactory<TSaga>
        where TSaga : class, ISaga
    {
        readonly IndexedSagaDictionary<TSaga> _sagas;
        readonly ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga> _factory;

        public InMemorySagaRepositoryContextFactory(IndexedSagaDictionary<TSaga> sagas, ISagaConsumeContextFactory<IndexedSagaDictionary<TSaga>, TSaga> factory)
        {
            _sagas = sagas;
            _factory = factory;
        }

        /// <summary>
        /// Locks the saga repository and creates a repository context. The returned context
        /// must be disposed, or the saga repository will remain locked. The returned context unlocks the repository
        /// when a saga instance is either loaded or created.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public async Task<SagaRepositoryContext<TSaga, T>> CreateContext<T>(ConsumeContext<T> context, Guid? correlationId = default)
            where T : class
        {
            await _sagas.MarkInUse(context.CancellationToken).ConfigureAwait(false);

            return new InMemorySagaRepositoryContext<TSaga, T>(_sagas, _factory, context);
        }

        /// <summary>
        /// Locks the saga repository and creates a repository context. The returned context
        /// must be disposed, or the saga repository will remain locked. The returned context unlocks the repository
        /// when a saga instance is either loaded or created.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <param name="correlationId"></param>
        /// <returns></returns>
        public Task<SagaRepositoryContext<TSaga>> CreateContext(CancellationToken cancellationToken = default, Guid? correlationId = default)
        {
            return Task.FromResult<SagaRepositoryContext<TSaga>>(new InMemorySagaRepositoryContext<TSaga>(_sagas));
        }

        public void Probe(ProbeContext context)
        {
            context.Add("count", _sagas.Count);
            context.Add("persistence", "memory");
        }
    }
}

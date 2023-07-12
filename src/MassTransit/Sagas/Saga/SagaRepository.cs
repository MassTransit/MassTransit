namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Middleware;


    /// <summary>
    /// The modern saga repository, which can be used with any storage engine. Leverages the new interfaces for consume and query context.
    /// </summary>
    /// <typeparam name="TSaga"></typeparam>
    public class SagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ILoadSagaRepository<TSaga> _loadSagaRepository;
        readonly QuerySagaRepository<TSaga> _querySagaRepository;
        readonly ISagaRepositoryContextFactory<TSaga> _repositoryContextFactory;

        public SagaRepository(ISagaRepositoryContextFactory<TSaga> repositoryContextFactory,
            IQuerySagaRepositoryContextFactory<TSaga> queryRepositoryContextFactory = null,
            ILoadSagaRepositoryContextFactory<TSaga> loadSagaRepositoryContextFactory = null)
        {
            _repositoryContextFactory = repositoryContextFactory;
            _querySagaRepository = new QuerySagaRepository<TSaga>(queryRepositoryContextFactory ?? NotImplementedSagaRepositoryContextFactory.Instance);
            _loadSagaRepository = new LoadSagaRepository<TSaga>(loadSagaRepositoryContextFactory ?? NotImplementedSagaRepositoryContextFactory.Instance);
        }

        public Task<TSaga> Load(Guid correlationId)
        {
            return _loadSagaRepository.Load(correlationId);
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return _querySagaRepository.Find(query);
        }

        public void Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");

            _repositoryContextFactory.Probe(scope);
            _querySagaRepository.Probe(scope);
            _loadSagaRepository.Probe(scope);
        }

        public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            var correlationId = context.CorrelationId ??
                throw new SagaException("The CorrelationId was not specified", typeof(TSaga), typeof(T));

            return _repositoryContextFactory.Send(context, new SendSagaPipe<TSaga, T>(policy, next, correlationId));
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repositoryContextFactory.SendQuery(context, query, new SendQuerySagaPipe<TSaga, T>(policy, next));
        }


        class NotImplementedSagaRepositoryContextFactory :
            ILoadSagaRepositoryContextFactory<TSaga>,
            IQuerySagaRepositoryContextFactory<TSaga>
        {
            public static readonly NotImplementedSagaRepositoryContextFactory Instance = new NotImplementedSagaRepositoryContextFactory();

            static readonly string QueryErrorMessage =
                $"Query-based saga correlation is not available when using current saga repository implementation: {TypeCache<TSaga>.ShortName}";

            static readonly string LoadErrorMessage =
                $"Load-based saga correlation is not available when using current saga repository implementation: {TypeCache<TSaga>.ShortName}";

            NotImplementedSagaRepositoryContextFactory()
            {
            }

            public Task<T> Execute<T>(Func<LoadSagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
                where T : class
            {
                throw new NotSupportedException(LoadErrorMessage);
            }

            public void Probe(ProbeContext context)
            {
            }

            public Task<T> Execute<T>(Func<QuerySagaRepositoryContext<TSaga>, Task<T>> asyncMethod, CancellationToken cancellationToken = default)
                where T : class
            {
                throw new NotSupportedException(QueryErrorMessage);
            }
        }
    }
}

namespace MassTransit
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Saga;


    public class InMemorySagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _repository;
        readonly ISagaRepositoryContextFactory<TSaga> _repositoryContextFactory;
        readonly IndexedSagaDictionary<TSaga> _sagas;

        public InMemorySagaRepository()
        {
            _sagas = new IndexedSagaDictionary<TSaga>();

            var factory = new InMemorySagaConsumeContextFactory<TSaga>();

            _repositoryContextFactory = new InMemorySagaRepositoryContextFactory<TSaga>(_sagas, factory);

            _repository = new SagaRepository<TSaga>(_repositoryContextFactory);
        }

        public SagaInstance<TSaga> this[Guid id] => _sagas[id];

        public int Count => _sagas.Count;

        public Task<TSaga> Load(Guid correlationId)
        {
            return _repositoryContextFactory.Execute(context => context.Load(correlationId));
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return _repositoryContextFactory.Execute<IEnumerable<Guid>>(async context => await context.Query(query).ConfigureAwait(false));
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            var scope = context.CreateScope("sagaRepository");
            scope.Set(new
            {
                _sagas.Count,
                Persistence = "memory"
            });
        }

        Task ISagaRepository<TSaga>.Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.Send(context, policy, next);
        }

        Task ISagaRepository<TSaga>.SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
        {
            return _repository.SendQuery(context, query, policy, next);
        }
    }
}

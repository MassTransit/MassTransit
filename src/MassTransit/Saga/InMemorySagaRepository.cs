namespace MassTransit.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using GreenPipes;
    using InMemoryRepository;


    public class InMemorySagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly ISagaRepository<TSaga> _repository;
        readonly IndexedSagaDictionary<TSaga> _sagas;

        public InMemorySagaRepository()
        {
            _sagas = new IndexedSagaDictionary<TSaga>();

            var factory = new InMemorySagaConsumeContextFactory<TSaga>();

            var repositoryContextFactory = new InMemorySagaRepositoryContextFactory<TSaga>(_sagas, factory);

            _repository = new SagaRepository<TSaga>(repositoryContextFactory);
        }

        public SagaInstance<TSaga> this[Guid id] => _sagas[id];

        public int Count => _sagas.Count;

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return Task.FromResult(_sagas.Where(query).Select(x => x.Instance.CorrelationId));
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

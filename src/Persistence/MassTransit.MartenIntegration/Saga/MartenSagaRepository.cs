namespace MassTransit.MartenIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Context;
    using GreenPipes;
    using Marten;
    using MassTransit.Saga;


    public class MartenSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        ILoadSagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaRepository<TSaga> _repository;

        public MartenSagaRepository(IDocumentStore documentStore)
        {
            var consumeContextFactory = new MartenSagaConsumeContextFactory<TSaga>();

            ISagaRepositoryContextFactory<TSaga> repositoryContextFactory = new MartenSagaRepositoryContextFactory<TSaga>(documentStore, consumeContextFactory);
            _repository = new SagaRepository<TSaga>(repositoryContextFactory);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _repository.Probe(context);
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

        Task<TSaga> ILoadSagaRepository<TSaga>.Load(Guid correlationId)
        {
            return _repository.Load(correlationId);
        }

        Task<IEnumerable<Guid>> IQuerySagaRepository<TSaga>.Find(ISagaQuery<TSaga> query)
        {
            return _repository.Find(query);
        }
    }
}

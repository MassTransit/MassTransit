namespace MassTransit.NHibernateIntegration.Saga
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Saga;
    using NHibernate;


    public class NHibernateSagaRepository<TSaga> :
        ISagaRepository<TSaga>,
        IQuerySagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        readonly SagaRepository<TSaga> _repository;

        public NHibernateSagaRepository(ISessionFactory sessionFactory)
        {
            var consumeContextFactory = new NHibernateSagaConsumeContextFactory<TSaga>();

            var repositoryContextFactory = new NHibernateSagaRepositoryContextFactory<TSaga>(sessionFactory, consumeContextFactory);

            _repository = new SagaRepository<TSaga>(repositoryContextFactory);
        }

        public Task<IEnumerable<Guid>> Find(ISagaQuery<TSaga> query)
        {
            return _repository.Find(query);
        }

        void IProbeSite.Probe(ProbeContext context)
        {
            _repository.Probe(context);
        }

        public Task Send<T>(ConsumeContext<T> context, ISagaPolicy<TSaga, T> policy, IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.Send(context, policy, next);
        }

        public Task SendQuery<T>(ConsumeContext<T> context, ISagaQuery<TSaga> query, ISagaPolicy<TSaga, T> policy,
            IPipe<SagaConsumeContext<TSaga, T>> next)
            where T : class
        {
            return _repository.SendQuery(context, query, policy, next);
        }
    }
}

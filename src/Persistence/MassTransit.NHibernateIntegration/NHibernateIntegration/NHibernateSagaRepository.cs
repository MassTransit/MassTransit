namespace MassTransit.NHibernateIntegration
{
    using MassTransit.Saga;
    using NHibernate;
    using Saga;


    public static class NHibernateSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public static ISagaRepository<TSaga> Create(ISessionFactory sessionFactory)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<ISession, TSaga>();

            var repositoryContextFactory = new NHibernateSagaRepositoryContextFactory<TSaga>(sessionFactory, consumeContextFactory);

            return new SagaRepository<TSaga>(repositoryContextFactory);
        }
    }
}

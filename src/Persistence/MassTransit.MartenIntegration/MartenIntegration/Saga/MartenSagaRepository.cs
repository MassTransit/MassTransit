namespace MassTransit.MartenIntegration.Saga
{
    using Marten;
    using MassTransit.Saga;


    public static class MartenSagaRepository<TSaga>
        where TSaga : class, ISaga
    {
        public static ISagaRepository<TSaga> Create(IDocumentStore documentStore)
        {
            var consumeContextFactory = new SagaConsumeContextFactory<IDocumentSession, TSaga>();

            ISagaRepositoryContextFactory<TSaga> repositoryContextFactory = new MartenSagaRepositoryContextFactory<TSaga>(documentStore, consumeContextFactory);
            return new SagaRepository<TSaga>(repositoryContextFactory);
        }
    }
}

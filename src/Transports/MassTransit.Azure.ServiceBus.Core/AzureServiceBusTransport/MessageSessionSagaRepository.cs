namespace MassTransit.AzureServiceBusTransport
{
    using Saga;


    public static class MessageSessionSagaRepository
    {
        public static ISagaRepository<T> Create<T>()
            where T : class, ISaga
        {
            var consumeContextFactory = new SagaConsumeContextFactory<MessageSessionContext, T>();

            var repositoryFactory = new MessageSessionSagaRepositoryContextFactory<T>(consumeContextFactory);

            return new SagaRepository<T>(repositoryFactory);
        }
    }
}

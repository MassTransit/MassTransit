namespace MassTransit.Persistence.Tests.IntegrationTests.Connectors
{
    using Configuration;


    public interface TestConnector
    {
        Task Setup();
        Task Teardown();

        void Connect(ICustomJobSagaRepositoryConfigurator conf);

        void Connect<TSaga>(ICustomRepositoryConfigurator<TSaga> conf)
            where TSaga : class, ISaga;

        IMessageDataRepository CreateMessageDataRepository(TimeProvider timeProvider);

        Task<List<TSaga>> GetSagas<TSaga>()
            where TSaga : class, ISaga;
    }
}

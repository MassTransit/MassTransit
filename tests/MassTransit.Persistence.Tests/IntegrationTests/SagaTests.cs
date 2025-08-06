namespace MassTransit.Persistence.Tests.IntegrationTests
{
    using Connectors;
    using NUnit.Framework;
    using TestFramework;


    public abstract class SagaTests<TConnector> : InMemoryTestFixture
        where TConnector : TestConnector, new()
    {
        protected static readonly Guid SagaId = Guid.Parse("d747db39-0d64-49b5-85f4-2a796ba82130");
        protected readonly TConnector Connector;

        protected readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(2);

        protected SagaTests()
        {
            Connector = new TConnector();
        }

        [SetUp]
        public Task Initialize()
        {
            return Connector.Setup();
        }

        [TearDown]
        public Task Teardown()
        {
            return Connector.Teardown();
        }

        protected Task<List<TSaga>> GetSagas<TSaga>()
            where TSaga : class, ISaga
        {
            return Connector.GetSagas<TSaga>();
        }
    }
}

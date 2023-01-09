namespace MassTransit.Azure.Cosmos.Tests
{
    using System.Threading.Tasks;
    using ContainerTests;
    using NUnit.Framework;
    using Saga.Data;


    [SetUpFixture]
    public class AzureCosmosTestSetUpFixture
    {
        [OneTimeSetUp]
        public async Task Before_any()
        {
            await SagaRepository<SimpleSaga>.Instance.Initialize();
            await SagaRepository<TestInstance>.Instance.Initialize();
        }
    }
}

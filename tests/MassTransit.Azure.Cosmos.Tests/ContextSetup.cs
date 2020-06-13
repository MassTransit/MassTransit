namespace MassTransit.Azure.Cosmos.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga.Data;


    [SetUpFixture]
    public class ContextSetup
    {
        [OneTimeSetUp]
        public async Task Before_any()
        {
            await SagaRepository<SimpleSaga>.Instance.Initialize();
            await SagaRepository<ContainerTests.TestInstance>.Instance.Initialize();
        }
    }
}

namespace MassTransit.DocumentDbIntegration.Tests
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
            await SagaRepository.Instance.Initialize();
        }
    }
}

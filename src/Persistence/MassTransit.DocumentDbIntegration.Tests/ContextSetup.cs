namespace MassTransit.DocumentDbIntegration.Tests
{
    using System.Threading.Tasks;
    using Log4NetIntegration.Logging;
    using NUnit.Framework;
    using Saga;


    [SetUpFixture]
    public class ContextSetup
    {
        [OneTimeSetUp]
        public async Task Before_any()
        {
            Log4NetLogger.Use("test.log4net.xml");

            await SagaRepository.Instance.Initialize();
        }
    }
}

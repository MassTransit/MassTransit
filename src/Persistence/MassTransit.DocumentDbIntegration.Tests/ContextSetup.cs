namespace MassTransit.DocumentDbIntegration.Tests
{
    using System.Threading.Tasks;
    using MassTransit.Tests;
    using NUnit.Framework;
    using Saga;
    using Serilog;


    [SetUpFixture]
    public class ContextSetup
    {
        ILogger _baseLogger;

        [OneTimeSetUp]
        public async Task Before_any()
        {
            _baseLogger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log\\MassTransit.Tests.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Logging.Logger.UseLoggerFactory(new SerilogLoggerFactory(_baseLogger));
            await SagaRepository.Instance.Initialize();
        }

        [OneTimeTearDown]
        public void After_all()
        {
            Logging.Logger.Shutdown();
        }
    }
}

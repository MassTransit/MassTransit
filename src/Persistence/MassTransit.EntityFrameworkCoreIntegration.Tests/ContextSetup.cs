namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    using MassTransit.Tests;
    using NUnit.Framework;
    using Serilog;


    [SetUpFixture]
    public class ContextSetup
    {
        ILogger _baseLogger;

        [OneTimeSetUp]
        public void Before_any()
        {
            _baseLogger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("log\\MassTransit.Tests.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Logging.Logger.UseLoggerFactory(new SerilogLoggerFactory(_baseLogger));
        }

        [OneTimeTearDown]
        public void After_all()
        {
            Logging.Logger.Shutdown();
        }
    }
}

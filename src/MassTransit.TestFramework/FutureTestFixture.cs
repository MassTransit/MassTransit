namespace MassTransit.TestFramework
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using Testing;


    public class FutureTestFixture
    {
        readonly IFutureTestFixtureConfigurator _testFixtureConfigurator;

        protected ServiceProvider Provider;
        protected ITestHarness TestHarness;

        public FutureTestFixture(IFutureTestFixtureConfigurator testFixtureConfigurator)
        {
            _testFixtureConfigurator = testFixtureConfigurator;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var collection = new ServiceCollection()
                .AddMassTransitTestHarness(cfg =>
                {
                    _testFixtureConfigurator.ConfigureFutureSagaRepository(cfg);

                    cfg.SetKebabCaseEndpointNameFormatter();

                    ConfigureMassTransit(cfg);
                });

            _testFixtureConfigurator.ConfigureServices(collection);

            ConfigureServices(collection);

            Provider = collection.BuildServiceProvider(true);

            ConfigureLogging();

            await _testFixtureConfigurator.OneTimeSetup(Provider);

            TestHarness = Provider.GetTestHarness();
            TestHarness.TestTimeout = TimeSpan.FromSeconds(5);

            await TestHarness.Start();
        }

        protected virtual void ConfigureMassTransit(IBusRegistrationConfigurator configurator)
        {
        }

        protected virtual void ConfigureServices(IServiceCollection collection)
        {
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            try
            {
                await _testFixtureConfigurator.OneTimeTearDown(Provider);
            }
            finally
            {
                await Provider.DisposeAsync();
            }
        }

        void ConfigureLogging()
        {
            var loggerFactory = Provider.GetRequiredService<ILoggerFactory>();

            LogContext.ConfigureCurrentLogContext(loggerFactory);
        }
    }
}

namespace MassTransit.TestComponents
{
    using System;
    using System.Threading.Tasks;
    using Context;
    using ExtensionsDependencyInjectionIntegration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    public class FutureTestFixture
    {
        readonly IFutureTestFixtureConfigurator _testFixtureConfigurator;

        protected ServiceProvider Provider;
        protected InMemoryTestHarness TestHarness;

        public FutureTestFixture(IFutureTestFixtureConfigurator testFixtureConfigurator)
        {
            _testFixtureConfigurator = testFixtureConfigurator;
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var collection = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(_ => BusTestFixture.LoggerFactory)
                .AddSingleton(typeof(ILogger<>), typeof(Logger<>))
                .AddMassTransitInMemoryTestHarness(cfg =>
                {
                    _testFixtureConfigurator.ConfigureFutureSagaRepository(cfg);

                    cfg.SetKebabCaseEndpointNameFormatter();

                    ConfigureMassTransit(cfg);
                })
                .AddGenericRequestClient();

            _testFixtureConfigurator.ConfigureServices(collection);

            ConfigureServices(collection);

            Provider = collection.BuildServiceProvider(true);

            ConfigureLogging();

            await _testFixtureConfigurator.OneTimeSetup(Provider);

            TestHarness = Provider.GetRequiredService<InMemoryTestHarness>();
            TestHarness.TestTimeout = TimeSpan.FromSeconds(5);
            TestHarness.OnConfigureInMemoryBus += configurator =>
            {
                ConfigureInMemoryBus(configurator);
            };

            await TestHarness.Start();
        }

        protected virtual void ConfigureMassTransit(IServiceCollectionBusConfigurator configurator)
        {
        }

        protected virtual void ConfigureServices(IServiceCollection collection)
        {
        }

        protected virtual void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            try
            {
                await TestHarness.Stop();

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

namespace MassTransit.Transports.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Logging;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Testing;
    using Transports;


    /// <summary>
    /// Tests a transport capability that should be available on all transports.
    /// </summary>
    [TestFixture(typeof(ActiveMqTestHarnessFactory))]
    [TestFixture(typeof(ArtemisTestHarnessFactory))]
    [TestFixture(typeof(AmazonSqsTestHarnessFactory))]
    [TestFixture(typeof(AzureServiceBusTestHarnessFactory))]
    [TestFixture(typeof(GrpcTestHarnessFactory))]
    [TestFixture(typeof(InMemoryTestHarnessFactory))]
    [TestFixture(typeof(RabbitMqTestHarnessFactory))]
    public abstract class TransportTest
    {
        static readonly bool _enableLog = !bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var isBuildServer) || !isBuildServer;
        static readonly TestOutputLoggerFactory LoggerFactory;

        static TransportTest()
        {
            LoggerFactory = new TestOutputLoggerFactory(_enableLog);
        }

        protected BusTestHarness Harness;
        TestExecutionContext _fixtureContext;
        readonly ITestHarnessFactory _harnessFactory;

        protected TransportTest(Type harnessType)
        {
            _harnessFactory = (ITestHarnessFactory)Activator.CreateInstance(harnessType);
        }

        /// <summary>
        /// Override this method to setup the test harness features, such as sagas, consumers, etc.
        /// </summary>
        /// <returns></returns>
        protected abstract Task Arrange();

        [OneTimeSetUp]
        public async Task TransportTestSetUp()
        {
            Harness = await _harnessFactory.CreateTestHarness();

            if (_enableLog)
            {
                Harness.OnConfigureBus += cfg =>
                {
                    LogContext.ConfigureCurrentLogContext(LoggerFactory);

                    LoggerFactory.Current = default;
                };
            }

            await Harness.Clean();

            await Arrange();

            _fixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = _fixtureContext;


            await StartTestHarness();

            await Task.Delay(200);
        }

        async Task StartTestHarness()
        {
            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            await Harness.Start(source.Token);
        }

        [OneTimeTearDown]
        public async Task TransportTestsTearDown()
        {
            LoggerFactory.Current = _fixtureContext;

            await StopTestHarness();

            Harness.Dispose();
        }

        async Task StopTestHarness()
        {
            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            await Harness.Stop();
        }
    }
}

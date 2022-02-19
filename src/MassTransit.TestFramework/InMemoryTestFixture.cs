namespace MassTransit.TestFramework
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Testing;


    public class InMemoryTestFixture :
        BusTestFixture
    {
        TestExecutionContext _fixtureContext;

        public InMemoryTestFixture()
            : this(new InMemoryTestHarness())
        {
        }

        public InMemoryTestFixture(InMemoryTestHarness harness)
            : base(harness)
        {
            InMemoryTestHarness = harness;
            InMemoryTestHarness.OnConfigureInMemoryBus += ConfigureInMemoryBus;
            InMemoryTestHarness.OnConfigureInMemoryReceiveEndpoint += ConfigureInMemoryReceiveEndpoint;
        }

        protected InMemoryTestHarness InMemoryTestHarness { get; }

        protected string InputQueueName => InMemoryTestHarness.InputQueueName;

        protected Uri BaseAddress => InMemoryTestHarness.BaseAddress;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => InMemoryTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => InMemoryTestHarness.BusSendEndpoint;

        protected Uri BusAddress => InMemoryTestHarness.BusAddress;

        protected Uri InputQueueAddress => InMemoryTestHarness.InputQueueAddress;

        [SetUp]
        public Task SetupInMemoryTest()
        {
            return Task.CompletedTask;
        }

        [TearDown]
        public Task TearDownInMemoryTest()
        {
            return Task.CompletedTask;
        }

        protected IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return InMemoryTestHarness.CreateRequestClient<TRequest>();
        }

        protected IRequestClient<TRequest> CreateRequestClient<TRequest>(Uri destinationAddress)
            where TRequest : class
        {
            return InMemoryTestHarness.CreateRequestClient<TRequest>(destinationAddress);
        }

        protected Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>()
            where TRequest : class
        {
            return InMemoryTestHarness.ConnectRequestClient<TRequest>();
        }

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            _fixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = _fixtureContext;

            return InMemoryTestHarness.Start();
        }

        protected Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return InMemoryTestHarness.GetSendEndpoint(address);
        }

        [OneTimeTearDown]
        public async Task TearDownInMemoryTestFixture()
        {
            LoggerFactory.Current = _fixtureContext;

            await InMemoryTestHarness.Stop().ConfigureAwait(false);

            InMemoryTestHarness.Dispose();
        }

        protected virtual void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
        }
    }
}

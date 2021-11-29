namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using TestFramework;
    using Testing;


    public class GrpcTestFixture :
        BusTestFixture
    {
        protected TestExecutionContext FixtureContext;

        public GrpcTestFixture()
            : this(new GrpcTestHarness())
        {
        }

        public GrpcTestFixture(GrpcTestHarness harness)
            : base(harness)
        {
            GrpcTestHarness = harness;
            GrpcTestHarness.OnConfigureGrpcBus += ConfigureGrpcBus;
            GrpcTestHarness.OnConfigureGrpcReceiveEndpoint += ConfigureGrpcReceiveEndpoint;
        }

        protected GrpcTestHarness GrpcTestHarness { get; }

        protected string InputQueueName => GrpcTestHarness.InputQueueName;

        protected Uri BaseAddress => GrpcTestHarness.BaseAddress;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => GrpcTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => GrpcTestHarness.BusSendEndpoint;

        protected Uri BusAddress => GrpcTestHarness.BusAddress;

        protected Uri InputQueueAddress => GrpcTestHarness.InputQueueAddress;

        [SetUp]
        public Task SetupGrpcTest()
        {
            return Task.CompletedTask;
        }

        [TearDown]
        public Task TearDownGrpcTest()
        {
            return Task.CompletedTask;
        }

        protected virtual IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return GrpcTestHarness.CreateRequestClient<TRequest>();
        }

        protected virtual IRequestClient<TRequest> CreateRequestClient<TRequest>(Uri destinationAddress)
            where TRequest : class
        {
            return GrpcTestHarness.CreateRequestClient<TRequest>(destinationAddress);
        }

        protected virtual Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>()
            where TRequest : class
        {
            return GrpcTestHarness.ConnectRequestClient<TRequest>();
        }

        [OneTimeSetUp]
        public Task SetupGrpcTestFixture()
        {
            FixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = FixtureContext;

            return GrpcTestHarness.Start();
        }

        protected Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
            return GrpcTestHarness.GetSendEndpoint(address);
        }

        [OneTimeTearDown]
        public async Task TearDownGrpcTestFixture()
        {
            LoggerFactory.Current = FixtureContext;

            await GrpcTestHarness.Stop().ConfigureAwait(false);

            GrpcTestHarness.Dispose();
        }

        protected virtual void ConfigureGrpcBus(IGrpcBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureGrpcReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
        }
    }
}

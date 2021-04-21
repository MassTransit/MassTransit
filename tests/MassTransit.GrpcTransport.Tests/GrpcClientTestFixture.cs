namespace MassTransit.GrpcTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Testing;


    public class GrpcClientTestFixture :
        GrpcTestFixture
    {
        public GrpcClientTestFixture()
            : base(new GrpcTestHarness(new Uri("http://127.0.0.1:29796")))
        {
            GrpcClientTestHarness = new GrpcTestHarness(new Uri("http://127.0.0.1:29797"));
            GrpcClientTestHarness.OnConfigureGrpcBus += ConfigureGrpcClientBus;
            GrpcClientTestHarness.OnConfigureGrpcHost += ConfigureGrpcClientHost;
            GrpcClientTestHarness.OnConfigureGrpcReceiveEndpoint += ConfigureGrpcClientReceiveEndpoint;
        }

        protected GrpcTestHarness GrpcClientTestHarness { get; }

        protected string ClientInputQueueName => GrpcClientTestHarness.InputQueueName;

        protected Uri ClientBaseAddress => GrpcClientTestHarness.BaseAddress;

        protected ISendEndpoint ClientInputQueueSendEndpoint => GrpcClientTestHarness.InputQueueSendEndpoint;

        protected ISendEndpoint ClientBusSendEndpoint => GrpcClientTestHarness.BusSendEndpoint;

        protected Uri ClientBusAddress => GrpcClientTestHarness.BusAddress;

        protected Uri ClientInputQueueAddress => GrpcClientTestHarness.InputQueueAddress;

        [OneTimeSetUp]
        public async Task SetupGrpcClientTestFixture()
        {
            LoggerFactory.Current = FixtureContext;

            GrpcClientTestHarness.TestTimeout = TestTimeout;
            GrpcClientTestHarness.TestInactivityTimeout = TestInactivityTimeout;

            await GrpcClientTestHarness.Start().ConfigureAwait(false);
        }

        [OneTimeTearDown]
        public async Task TearDownGrpcClientTestFixture()
        {
            LoggerFactory.Current = FixtureContext;

            await GrpcClientTestHarness.Stop().ConfigureAwait(false);

            GrpcClientTestHarness.Dispose();
        }

        protected override IRequestClient<TRequest> CreateRequestClient<TRequest>()
            where TRequest : class
        {
            return GrpcClientTestHarness.CreateRequestClient<TRequest>();
        }

        protected override IRequestClient<TRequest> CreateRequestClient<TRequest>(Uri destinationAddress)
            where TRequest : class
        {
            return GrpcTestHarness.CreateRequestClient<TRequest>(destinationAddress);
        }

        protected override Task<IRequestClient<TRequest>> ConnectRequestClient<TRequest>()
            where TRequest : class
        {
            return GrpcClientTestHarness.ConnectRequestClient<TRequest>();
        }

        protected virtual void ConfigureGrpcClientHost(IGrpcHostConfigurator configurator)
        {
            //configurator.AddServer(BaseAddress);
        }

        protected virtual void ConfigureGrpcClientBus(IGrpcBusFactoryConfigurator configurator)
        {
            ConfigureBusDiagnostics(configurator);
        }

        protected virtual void ConfigureGrpcClientReceiveEndpoint(IGrpcReceiveEndpointConfigurator configurator)
        {
        }
    }
}

namespace MassTransit.Azure.ServiceBus.Core.Tests
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    public abstract class AzureServiceBusTestFixture :
        BusTestFixture
    {
        protected AzureServiceBusTestHarness AzureServiceBusTestHarness { get; }

        public AzureServiceBusTestFixture(string inputQueueName = null, Uri serviceUri = null, ServiceBusTokenProviderSettings settings = null)
            : this(new AzureServiceBusTestHarness(
                serviceUri ?? AzureServiceBusEndpointUriCreator.Create(Configuration.ServiceNamespace, "MassTransit.Azure.ServiceBus.Core.Tests"),
                settings?.KeyName ?? ((ServiceBusTokenProviderSettings)new TestAzureServiceBusAccountSettings()).KeyName,
                settings?.SharedAccessKey ?? ((ServiceBusTokenProviderSettings)new TestAzureServiceBusAccountSettings()).SharedAccessKey,
                inputQueueName))
        {
        }

        protected AzureServiceBusTestFixture(AzureServiceBusTestHarness harness)
            : base(harness)
        {
            AzureServiceBusTestHarness = harness;

            AzureServiceBusTestHarness.OnConfigureServiceBusBus += ConfigureServiceBusBus;
            AzureServiceBusTestHarness.OnConfigureServiceBusReceiveEndpoint += ConfigureServiceBusReceiveEndpoint;
        }

        protected string InputQueueName => AzureServiceBusTestHarness.InputQueueName;

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => AzureServiceBusTestHarness.InputQueueSendEndpoint;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => AzureServiceBusTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => AzureServiceBusTestHarness.Sent;

        protected Uri BusAddress => AzureServiceBusTestHarness.BusAddress;

        protected Uri HostAddress => AzureServiceBusTestHarness.HostAddress;

        protected Uri InputQueueAddress => AzureServiceBusTestHarness.InputQueueAddress;

        [OneTimeSetUp]
        public async Task SetupAzureServiceBusTestFixture()
        {
            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(20));

            await AzureServiceBusTestHarness.Start(source.Token);
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return AzureServiceBusTestHarness.Stop();
        }

        protected virtual void ConfigureServiceBusBus(IServiceBusBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureServiceBusReceiveEndpoint(IServiceBusReceiveEndpointConfigurator configurator)
        {
        }
    }
}

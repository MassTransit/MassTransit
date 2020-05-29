namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using RabbitMQ.Client;
    using RabbitMqTransport.Testing;
    using TestFramework;
    using Transports;


    public class RabbitMqTestFixture :
        BusTestFixture
    {
        public RabbitMqTestFixture(Uri logicalHostAddress = null, string inputQueueName = null)
            : this(new RabbitMqTestHarness(inputQueueName), logicalHostAddress)
        {
        }

        public RabbitMqTestFixture(RabbitMqTestHarness harness, Uri logicalHostAddress = null)
            : base(harness)
        {
            RabbitMqTestHarness = harness;

            if (logicalHostAddress != null)
            {
                RabbitMqTestHarness.NodeHostName = RabbitMqTestHarness.HostAddress.Host;
                RabbitMqTestHarness.HostAddress = logicalHostAddress;
            }

            RabbitMqTestHarness.OnConfigureRabbitMqHost += ConfigureRabbitMqHost;
            RabbitMqTestHarness.OnConfigureRabbitMqBus += ConfigureRabbitMqBus;
            RabbitMqTestHarness.OnConfigureRabbitMqReceiveEndpoint += ConfigureRabbitMqReceiveEndpoint;
            RabbitMqTestHarness.OnCleanupVirtualHost += OnCleanupVirtualHost;
        }

        protected RabbitMqTestHarness RabbitMqTestHarness { get; }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => RabbitMqTestHarness.InputQueueSendEndpoint;

        protected Uri InputQueueAddress => RabbitMqTestHarness.InputQueueAddress;

        protected Uri HostAddress => RabbitMqTestHarness.HostAddress;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => RabbitMqTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => RabbitMqTestHarness.Sent;

        protected Uri BusAddress => RabbitMqTestHarness.BusAddress;

        protected IMessageNameFormatter NameFormatter => RabbitMqTestHarness.NameFormatter;

        protected RabbitMqHostSettings GetHostSettings()
        {
            return RabbitMqTestHarness.GetHostSettings();
        }

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return RabbitMqTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return RabbitMqTestHarness.Stop();
        }

        protected virtual void ConfigureRabbitMqHost(IRabbitMqHostConfigurator configurator)
        {
        }

        protected virtual void ConfigureRabbitMqBus(IRabbitMqBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureRabbitMqReceiveEndpoint(IRabbitMqReceiveEndpointConfigurator configurator)
        {
        }

        protected virtual void OnCleanupVirtualHost(IModel model)
        {
        }
    }
}

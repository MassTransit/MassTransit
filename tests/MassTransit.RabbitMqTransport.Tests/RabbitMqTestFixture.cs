namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using RabbitMQ.Client;
    using TestFramework;
    using Transports;


    public class RabbitMqTestFixture :
        BusTestFixture
    {
        TestExecutionContext _fixtureContext;

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
        public async Task SetupRabbitMqTestFixture()
        {
            await CleanupVirtualHost().ConfigureAwait(false);

            _fixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = _fixtureContext;

            await RabbitMqTestHarness.Start().ConfigureAwait(false);

            await Task.Delay(200);
        }

        [OneTimeTearDown]
        public async Task TearDownRabbitMqTestFixture()
        {
            LoggerFactory.Current = _fixtureContext;

            await RabbitMqTestHarness.Stop().ConfigureAwait(false);

            RabbitMqTestHarness.Dispose();
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

        async Task CleanupVirtualHost()
        {
            try
            {
                var cleanVirtualHostEntirely = !bool.TryParse(Environment.GetEnvironmentVariable("CI"), out var isBuildServer) || !isBuildServer;
                if (cleanVirtualHostEntirely)
                {
                    await RabbitMqTestHarness.Clean().ConfigureAwait(false);

                    RabbitMqTestHarness.CleanVirtualHost = false;
                }
            }
            catch (Exception exception)
            {
                await TestContext.Error.WriteLineAsync(exception.Message);
            }
        }

        protected virtual void OnCleanupVirtualHost(IModel model)
        {
        }
    }
}

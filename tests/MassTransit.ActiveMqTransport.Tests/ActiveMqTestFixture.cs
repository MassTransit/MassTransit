namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using TestFramework;
    using Testing;


    public class ActiveMqTestFixture :
        BusTestFixture
    {
        TestExecutionContext _fixtureContext;

        public ActiveMqTestFixture(string inputQueueName = null)
            : this(new ActiveMqTestHarness(inputQueueName))
        {
        }

        public ActiveMqTestFixture(ActiveMqTestHarness harness)
            : base(harness)
        {
            ActiveMqTestHarness = harness;

            ActiveMqTestHarness.OnConfigureActiveMqHost += ConfigureActiveMqHost;
            ActiveMqTestHarness.OnConfigureActiveMqBus += ConfigureActiveMqBus;
            ActiveMqTestHarness.OnConfigureActiveMqReceiveEndpoint += ConfigureActiveMqReceiveEndpoint;
        }

        protected ActiveMqTestHarness ActiveMqTestHarness { get; }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => ActiveMqTestHarness.InputQueueSendEndpoint;

        protected Uri InputQueueAddress => ActiveMqTestHarness.InputQueueAddress;

        protected Uri HostAddress => ActiveMqTestHarness.HostAddress;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => ActiveMqTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => ActiveMqTestHarness.Sent;

        protected Uri BusAddress => ActiveMqTestHarness.BusAddress;

        [OneTimeSetUp]
        public async Task SetupInMemoryTestFixture()
        {
            _fixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = _fixtureContext;

            await ActiveMqTestHarness.Start();
        }

        [OneTimeTearDown]
        public async Task TearDownInMemoryTestFixture()
        {
            LoggerFactory.Current = _fixtureContext;

            await ActiveMqTestHarness.Stop();

            ActiveMqTestHarness.Dispose();
        }

        protected virtual void ConfigureActiveMqHost(IActiveMqHostConfigurator configurator)
        {
        }

        protected virtual void ConfigureActiveMqBus(IActiveMqBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureActiveMqReceiveEndpoint(IActiveMqReceiveEndpointConfigurator configurator)
        {
        }
    }
}

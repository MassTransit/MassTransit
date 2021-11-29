namespace MassTransit.ActiveMqTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    public class ActiveMqTestFixture :
        BusTestFixture
    {
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
        public Task SetupInMemoryTestFixture()
        {
            return ActiveMqTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return ActiveMqTestHarness.Stop();
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

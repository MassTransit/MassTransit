namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    public class AmazonSqsTestFixture :
        BusTestFixture
    {
        public AmazonSqsTestFixture()
            : this(new AmazonSqsTestHarness())
        {
        }

        public AmazonSqsTestFixture(AmazonSqsTestHarness harness)
            : base(harness)
        {
            AmazonSqsTestHarness = harness;

            AmazonSqsTestHarness.OnConfigureAmazonSqsHost += ConfigureAmazonSqsHost;
            AmazonSqsTestHarness.OnConfigureAmazonSqsBus += ConfigureAmazonSqsBus;
            AmazonSqsTestHarness.OnConfigureAmazonSqsReceiveEndpoint += ConfigureAmazonSqsReceiveEndpoint;
        }

        protected AmazonSqsTestHarness AmazonSqsTestHarness { get; }

        /// <summary>
        /// The sending endpoint for the InputQueue
        /// </summary>
        protected ISendEndpoint InputQueueSendEndpoint => AmazonSqsTestHarness.InputQueueSendEndpoint;

        protected Uri InputQueueAddress => AmazonSqsTestHarness.InputQueueAddress;

        protected Uri HostAddress => AmazonSqsTestHarness.HostAddress;

        /// <summary>
        /// The sending endpoint for the Bus
        /// </summary>
        protected ISendEndpoint BusSendEndpoint => AmazonSqsTestHarness.BusSendEndpoint;

        protected ISentMessageList Sent => AmazonSqsTestHarness.Sent;

        protected Uri BusAddress => AmazonSqsTestHarness.BusAddress;

        [OneTimeSetUp]
        public Task SetupInMemoryTestFixture()
        {
            return AmazonSqsTestHarness.Start();
        }

        [OneTimeTearDown]
        public Task TearDownInMemoryTestFixture()
        {
            return AmazonSqsTestHarness.Stop();
        }

        protected virtual void ConfigureAmazonSqsHost(IAmazonSqsHostConfigurator configurator)
        {
        }

        protected virtual void ConfigureAmazonSqsBus(IAmazonSqsBusFactoryConfigurator configurator)
        {
        }

        protected virtual void ConfigureAmazonSqsReceiveEndpoint(IAmazonSqsReceiveEndpointConfigurator configurator)
        {
        }
    }
}

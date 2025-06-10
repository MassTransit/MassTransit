namespace MassTransit.AmazonSqsTransport.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using TestFramework;
    using Testing;


    public abstract class AmazonSqsTestFixture :
        BusTestFixture
    {
        TestExecutionContext _fixtureContext;

        protected AmazonSqsTestFixture()
            : this(new AmazonSqsTestHarness())
        {
        }

        protected AmazonSqsTestFixture(AmazonSqsTestHarness harness)
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
        public async Task SetupInMemoryTestFixture()
        {
            _fixtureContext = TestExecutionContext.CurrentContext;

            LoggerFactory.Current = _fixtureContext;


            await AmazonSqsTestHarness.Start();
        }

        [OneTimeTearDown]
        public async Task TearDownInMemoryTestFixture()
        {
            LoggerFactory.Current = _fixtureContext;

            await AmazonSqsTestHarness.Stop();

            AmazonSqsTestHarness.Dispose();
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

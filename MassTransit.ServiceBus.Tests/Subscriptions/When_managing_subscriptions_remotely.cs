namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;

    [TestFixture]
    public class When_managing_subscriptions_remotely :
        Specification
    {
        private SubscriptionClient sc;
        private IServiceBus _mockBus;
        private ISubscriptionCache _mockCache;
        private IEndpoint _mockEndpoint;

        protected override void Before_each()
        {
            _mockBus = DynamicMock<IServiceBus>();
            _mockCache = DynamicMock<ISubscriptionCache>();
            _mockEndpoint = DynamicMock<IEndpoint>();

            sc = new SubscriptionClient(_mockBus, _mockCache, _mockEndpoint);
        }

        protected override void After_each()
        {
            _mockEndpoint = null;
            _mockCache = null;
            _mockBus = null;
            sc = null;
        }

        [Test]
        public void start()
        {
            sc.Start();
        }

        [Test]
        public void stop()
        {
            
        }

        //consumes
    }
}
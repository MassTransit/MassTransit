namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_managing_subscriptions_remotely :
        Specification
    {
        private SubscriptionClient sc;
        private IServiceBus _mockBus;
        private ISubscriptionCache _mockCache;
        private IEndpoint _mockEndpoint;
        private IEndpoint _mockBusEndpoint;

        protected override void Before_each()
        {
            _mockBus = DynamicMock<IServiceBus>();
            _mockCache = DynamicMock<ISubscriptionCache>();
            _mockEndpoint = DynamicMock<IEndpoint>();
            _mockBusEndpoint = DynamicMock<IEndpoint>();

            sc = new SubscriptionClient(_mockBus, _mockCache, _mockEndpoint);
        }

        protected override void After_each()
        {
            _mockBusEndpoint = null;
            _mockEndpoint = null;
            _mockCache = null;
            _mockBus = null;
            sc = null;
        }

        [Test]
        public void start()
        {
            Uri uri = new Uri("msmq://localhost/test");
            using(Record())
            {
                _mockBus.Subscribe(sc);
                Expect.Call(_mockBus.Endpoint).Return(_mockBusEndpoint);
                Expect.Call(_mockBusEndpoint.Uri).Return(uri);
                _mockEndpoint.Send(new CacheUpdateRequest(uri));
                LastCall.IgnoreArguments();
            }
            using (Playback())
            {
                sc.Start();
            }
        }

        [Test]
        public void stop()
        {
            //Assert.Fail("start back up here");
        }

        //consumes
    }
}
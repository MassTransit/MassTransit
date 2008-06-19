namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using Exceptions;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;
    using c = MassTransit.ServiceBus.Subscriptions.ClientHandlers;

    [TestFixture]
    public class When_managing_subscriptions_remotely :
        Specification
    {
        private SubscriptionClient sc;
        private IServiceBus _mockBus;
        private ISubscriptionCache _mockCache;
        private IEndpoint _mockEndpoint;
        private IEndpoint _mockBusEndpoint;
        private readonly Uri uri = new Uri("msmq://localhost/test");

        protected override void Before_each()
        {
            _mockBus = DynamicMock<IServiceBus>();
            _mockCache = DynamicMock<ISubscriptionCache>();
            _mockEndpoint = DynamicMock<IEndpoint>();
            _mockBusEndpoint = DynamicMock<IEndpoint>();

            SetupResult.For(_mockBus.Endpoint).Return(_mockBusEndpoint);
            SetupResult.For(_mockBusEndpoint.Uri).Return(uri);
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
            using(Record())
            {
                _mockBus.AddComponent<c.AddSubscriptionHandler>();
                _mockBus.AddComponent<c.RemoveSubscriptionHandler>();
                _mockBus.AddComponent<c.CacheUpdateHandler>();

                Expect.Call(delegate { _mockEndpoint.Send(new CacheUpdateRequest(uri)); }).IgnoreArguments();
            }
            using (Playback())
            {
                sc.Start();
            }
        }


        [Test]
        [ExpectedException(typeof(EndpointException))]
        public void Test_Endpoint_Detection()
        {
            using (Record())
            {
                
            }
            using (Playback())
            {
                SubscriptionClient sc2 = new SubscriptionClient(_mockBus, _mockCache, _mockBusEndpoint);
                sc2.Start();
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
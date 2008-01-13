namespace MassTransit.ServiceBus.Tests.Subscriptions
{
    using System;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using Rhino.Mocks;

    [TestFixture]
    public class When_Working_With_Subscriptions
    {
        private MockRepository mocks = new MockRepository();
        private IServiceBus bus;
        private IEndpoint wellKnownEndpoint;
        string mockPath = "msmq://localhost/bob";

        [SetUp]
        public void SetUp()
        {
            bus = mocks.CreateMock<IServiceBus>();
            wellKnownEndpoint = mocks.CreateMock<IEndpoint>();
        }

        [TearDown]
        public void TearDown()
        {
            bus = null;
            mocks = null;
            wellKnownEndpoint = null;
        }
       

        [Test]
        [Ignore("This test is bogus based on new data")]
        public void Add_Subscription_without_a_bus()
        {
            IEndpoint mockEndpoint = mocks.CreateMock<IEndpoint>();
            LocalSubscriptionCache cache = new LocalSubscriptionCache();
            using(mocks.Record())
            {
                Expect.Call(mockEndpoint.Uri).Return(new Uri(mockPath));
            }
            using(mocks.Playback())
            {
                cache.Add(typeof(PingMessage).FullName, new Uri(mockPath));
            }
            
        }
    }
}
namespace MassTransit.ServiceBus.Tests.IntegrationTests
{
    using System;
    using System.Threading;
    using MassTransit.ServiceBus.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    [TestFixture]
    public class MsmqStorage_Idempotent_Tests
    {
        [Test]
        [Explicit("needs items in the queue to work - dru")]
        public void Add_Should_Be_Idempotent()
        {
            IMessageQueueEndpoint ep = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");
            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage store = new MsmqSubscriptionStorage(ep, cache);
            Thread.Sleep(10000);
            Assert.That(store.List().Count, Is.EqualTo(2));
        }

        [Test]
        public void Idempontent_Add()
        {
            IMessageQueueEndpoint ep = new MessageQueueEndpoint("msmq://localhost/test_subscriptions");
            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage store = new MsmqSubscriptionStorage(ep, cache);
            store.Add(typeof(PingMessage).FullName, new Uri("msmq://localhost/test_servicebus"));
            store.Add(typeof(PingMessage).FullName, new Uri("msmq://localhost/test_servicebus"));
            Assert.That(store.List().Count, Is.EqualTo(1));
        }
    }
}
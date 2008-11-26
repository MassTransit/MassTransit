namespace MassTransit.Tests.Subscriptions
{
    using System;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Tests.Messages;
    using Tests.Subscriptions;
    using Tests.TestConsumers;

    [TestFixture]
    public class When_using_the_subscription_service : 
        SubscriptionManagerContext
    {
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(5);

        [Test]
        public void It_should_startup_properly()
        {
            Assert.That(SubscriptionCache.List(typeof(PingMessage).FullName).Count, Is.EqualTo(0));
        }

        [Test]
        public void A_subscription_should_end_up_on_the_service()
        {
            MonitorSubscriptionCache<PingMessage> monitor = new MonitorSubscriptionCache<PingMessage>(SubscriptionCache);

            LocalBus.Subscribe<TestMessageConsumer<PingMessage>>();

            monitor.ShouldHaveBeenAdded(_timeout);
        }

        [Test]
        public void A_subscription_should_be_removed_from_the_service()
        {
            MonitorSubscriptionCache<PingMessage> monitor = new MonitorSubscriptionCache<PingMessage>(SubscriptionCache);

            TestMessageConsumer<PingMessage> consumer = new TestMessageConsumer<PingMessage>();
            LocalBus.Subscribe(consumer);

            monitor.ShouldHaveBeenAdded(_timeout);

            LocalBus.Unsubscribe(consumer);

            monitor.ShouldHaveBeenRemoved(_timeout);
        }
    }
}
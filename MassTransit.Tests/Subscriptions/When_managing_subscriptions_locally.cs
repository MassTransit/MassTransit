namespace MassTransit.Tests.Subscriptions
{
    using System;
    using MassTransit.Subscriptions;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    
    using Tests.Messages;

    [TestFixture]
    public class When_managing_subscriptions_locally :
        Specification
    {
        private ISubscriptionCache cache;
        private Uri sendTo = new Uri("msmq://localhost/test");

        protected override void Before_each()
        {
            cache = new LocalSubscriptionCache();
        }

        protected override void After_each()
        {
            cache = null;
        }

        [Test]
        public void Adding_a_subscription_should_fire_event()
        {
            bool wasFired = false;
            cache.OnAddSubscription += delegate(object sender, SubscriptionEventArgs e)
                                           {
                                               wasFired = true;
                                               Assert.That(e.Subscription.EndpointUri, Is.EqualTo(sendTo));
                                               Assert.That(e.Subscription.MessageName,
                                                           Is.EqualTo(typeof(PingMessage).FullName));
                                           };

            cache.Add(new Subscription(typeof(PingMessage).FullName, sendTo));

            Assert.That(cache.List().Count, Is.EqualTo(1));
            Assert.That(wasFired, Is.True);
        }

        [Test]
        public void Event_should_not_fire_if_subscription_exists()
        {
            bool wasFired = false;
            cache.Add(new Subscription(typeof(PingMessage).FullName, sendTo));
            cache.OnAddSubscription += delegate(object sender, SubscriptionEventArgs e)
                                           {
                                               wasFired = true;
                                               Assert.That(e.Subscription.EndpointUri, Is.EqualTo(sendTo));
                                               Assert.That(e.Subscription.MessageName,
                                                           Is.EqualTo(typeof(PingMessage).FullName));
                                           };

            cache.Add(new Subscription(typeof(PingMessage).FullName, sendTo));

            Assert.That(cache.List().Count, Is.EqualTo(1));
            Assert.That(wasFired, Is.False);
        }

        [Test]
        public void Listing_Subscriptions_By_Name()
        {
            cache.Add(new Subscription(typeof(PingMessage).FullName, new Uri("msmq://localhost/test")));
            cache.Add(new Subscription(typeof(PongMessage).FullName, new Uri("msmq://localhost/test")));

            Assert.That(cache.List(typeof(PingMessage).FullName).Count, Is.EqualTo(1));
            Assert.That(cache.List(typeof(PongMessage).FullName).Count, Is.EqualTo(1));
        }

        [Test]
        public void Remove_With_Event()
        {
            bool wasFired = false;
            cache.Add(new Subscription(typeof(PingMessage).FullName, sendTo));
            Assert.That(cache.List().Count, Is.EqualTo(1));

            cache.OnRemoveSubscription += delegate(object sender, SubscriptionEventArgs e)
                                              {
                                                  wasFired = true;
                                                  Assert.That(e.Subscription.EndpointUri, Is.EqualTo(sendTo));
                                                  Assert.That(e.Subscription.MessageName,
                                                              Is.EqualTo(typeof(PingMessage).FullName));
                                              };
            cache.Remove(new Subscription(typeof(PingMessage).FullName, sendTo));

            Assert.That(wasFired, Is.True);
        }
    }
}
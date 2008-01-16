using System;
using MassTransit.ServiceBus.Subscriptions;
using MassTransit.ServiceBus.Subscriptions.Messages;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    [TestFixture]
    public class As_Local_Subscription_Storage
    {
        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void Listing_Subscriptions_By_Name()
        {
            LocalSubscriptionCache cache = new LocalSubscriptionCache();
            cache.Add(typeof (PingMessage).FullName, new Uri("msmq://localhost/test"));
            cache.Add(typeof (PongMessage).FullName, new Uri("msmq://localhost/test"));

            Assert.That(cache.List(typeof (PingMessage).FullName).Count, Is.EqualTo(1));
            Assert.That(cache.List(typeof (PongMessage).FullName).Count, Is.EqualTo(1));
        }

        [Test]
        public void Adding_Subscription()
        {
            LocalSubscriptionCache cache = new LocalSubscriptionCache();
            cache.Add(typeof (PingMessage).FullName, new Uri("msmq://localhost/test"));

            Assert.That(cache.List().Count, Is.EqualTo(1));
        }

        [Test]
        public void Add_With_Event()
        {
            Uri sendTo = new Uri("msmq://localhost/test");
            bool wasFired = false;
            LocalSubscriptionCache cache = new LocalSubscriptionCache();
            cache.SubscriptionChanged += delegate(object sender, SubscriptionChangedEventArgs e)
                                             {
                                                 wasFired = true;
                                                 Assert.That(e.Change.Subscription.Address, Is.EqualTo(sendTo));
                                                 Assert.That(e.Change.ChangeType, Is.EqualTo(SubscriptionChangeType.Add));
                                                 Assert.That(e.Change.Subscription.MessageName,
                                                             Is.EqualTo(typeof (PingMessage).FullName));
                                             };
            cache.Add(typeof (PingMessage).FullName, sendTo);

            Assert.That(cache.List().Count, Is.EqualTo(1));
            Assert.That(wasFired, Is.True);
        }


        [Test]
        public void Removing_Subscription()
        {
            LocalSubscriptionCache cache = new LocalSubscriptionCache();
            cache.Add(typeof (PingMessage).FullName, new Uri("msmq://localhost/test"));

            Assert.That(cache.List().Count, Is.EqualTo(1));

            cache.Remove(typeof (PingMessage).FullName, new Uri("msmq://localhost/test"));
            Assert.That(cache.List().Count, Is.EqualTo(0));
        }

        [Test]
        public void Remove_With_Event()
        {
            Uri sendTo = new Uri("msmq://localhost/test");
            bool wasFired = false;
            LocalSubscriptionCache cache = new LocalSubscriptionCache();
            cache.Add(typeof (PingMessage).FullName, sendTo);
            Assert.That(cache.List().Count, Is.EqualTo(1));

            cache.SubscriptionChanged += delegate(object sender, SubscriptionChangedEventArgs e)
                                             {
                                                 wasFired = true;
                                                 Assert.That(e.Change.Subscription.Address, Is.EqualTo(sendTo));
                                                 Assert.That(e.Change.ChangeType,
                                                             Is.EqualTo(SubscriptionChangeType.Remove));
                                                 Assert.That(e.Change.Subscription.MessageName,
                                                             Is.EqualTo(typeof (PingMessage).FullName));
                                             };
            cache.Remove(typeof (PingMessage).FullName, sendTo);

            Assert.That(wasFired, Is.True);
        }
    }
}
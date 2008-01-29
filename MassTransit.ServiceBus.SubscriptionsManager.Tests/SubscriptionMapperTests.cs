namespace MassTransit.ServiceBus.SubscriptionsManager.Tests
{
    using System;
    using System.Collections.Generic;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;
    using Subscriptions;

    [TestFixture]
    public class SubscriptionMapperTests
    {
        [Test]
        public void From_Change_To_Persistant()
        {
            SubscriptionChange change = new SubscriptionChange(typeof(CacheUpdateResponse).FullName, new Uri("msmq://localhost/bob"), SubscriptionChangeType.Remove );
            StoredSubscription stored = SubscriptionMapper.MapFrom(change);

            Assert.That(stored.Address, Is.EqualTo(new Uri("msmq://localhost/bob")));
            Assert.That(stored.Id, Is.EqualTo(0));
            Assert.That(stored.IsActive, Is.True);
            Assert.That(stored.Message, Is.EqualTo(typeof(CacheUpdateResponse).FullName));
        }

        [Test]
        public void From_Persistant_To_Change()
        {
            StoredSubscription stored = new StoredSubscription("msmq://localhost/bob", typeof(CacheUpdateResponse).FullName);
            Subscription sub = SubscriptionMapper.MapFrom(stored);
            

            Assert.That(sub.Address, Is.EqualTo(new Uri("msmq://localhost/bob")));
            Assert.That(sub.MessageName, Is.EqualTo(typeof(CacheUpdateResponse).FullName));
        }

        [Test]
        public void Multiple_StoredSubscription_To_Many_Subscriptions()
        {
            List<StoredSubscription> subs = new List<StoredSubscription>();
            StoredSubscription stored = new StoredSubscription("msmq://localhost/bob", typeof(CacheUpdateResponse).FullName);
            subs.Add(stored);

            IList<Subscription> result = SubscriptionMapper.MapFrom(subs);
            Assert.That(result.Count, Is.EqualTo(1));
        }
    }
}
using System.Messaging;
using MassTransit.ServiceBus.Subscriptions;
using MassTransit.ServiceBus.Subscriptions.Messages;
using NUnit.Framework;

namespace MassTransit.ServiceBus.Tests.IntegrationTests
{
    [TestFixture]
    [Explicit]
    public class As_Msmq_Storage
    {
        private IMessageQueueEndpoint storageEndpoint;
        private IMessageQueueEndpoint subscriberEndpoint;
        private IMessageQueueEndpoint listenEndpoint;

        [SetUp]
        public void SetUp()
        {
            storageEndpoint = new MessageQueueEndpoint("msmq://localhost/storage");
            subscriberEndpoint = new MessageQueueEndpoint("msmq://localhost/subscriber");
            listenEndpoint = new MessageQueueEndpoint("msmq://localhost/listen");

            ServiceBusSetupFixture.ValidateAndPurgeQueue(storageEndpoint.QueuePath);
            ServiceBusSetupFixture.ValidateAndPurgeQueue(subscriberEndpoint.QueuePath);
            ServiceBusSetupFixture.ValidateAndPurgeQueue(listenEndpoint.QueuePath);
        }

        [TearDown]
        public void TearDown()
        {
            storageEndpoint = null;
            subscriberEndpoint = null;
            listenEndpoint = null;
        }

        [Test]
        public void I_Should_Work_When_Adding()
        {
            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(storageEndpoint, cache);

            storage.Add(typeof (PingMessage).FullName, subscriberEndpoint.Uri);

            SubscriptionChange msg = new SubscriptionChange("", null, SubscriptionChangeType.Add);

            ServiceBusSetupFixture.VerifyMessageInQueue(storageEndpoint.QueuePath, msg);
        }

        [Test]
        [ExpectedException(typeof (MessageQueueException))]
        public void I_Should_Puke_If_My_Storage_Queue_Doesnt_Exist()
        {
            IMessageQueueEndpoint nonExistentStorageEndpoint =
                new MessageQueueEndpoint(@"msmq://localhost/some_queue_that_doesnt_exist");

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(nonExistentStorageEndpoint, cache);
        }

        [Test]
        [Ignore("Chris make me pass gas.")]
        public void I_Should_Work_When_Removing()
        {
            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(storageEndpoint, cache);

            storage.Add(typeof (PingMessage).FullName, subscriberEndpoint.Uri);

            Assert.AreEqual(1, storage.List().Count);
            storage.Remove(typeof (PingMessage).FullName, subscriberEndpoint.Uri);
            Assert.AreEqual(0, storage.List().Count);

            ServiceBusSetupFixture.VerifyQueueIsEmpty(storageEndpoint.QueuePath);
        }
    }
}
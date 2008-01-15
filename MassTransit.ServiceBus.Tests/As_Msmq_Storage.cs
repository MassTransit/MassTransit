namespace MassTransit.ServiceBus.Tests
{
    using System.Messaging;
    using System.Threading;
    using MassTransit.ServiceBus.Subscriptions;
    using MassTransit.ServiceBus.Subscriptions.Messages;
    using NUnit.Framework;

    [TestFixture]
    public class As_Msmq_Storage :
         ServiceBusSetupFixture
    {
        //TODO: Load this up with 18,000 different tests

        [Test]
        public void I_Should_Work_When_Adding()
        {
            //TODO: This is nasty
            IMessageQueueEndpoint storageEndpoint = base._serviceBusEndPoint;
            IMessageQueueEndpoint subscriberEndpoint = base._remoteServiceBusEndPoint;

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(storageEndpoint, cache);

            storage.Add(typeof(PingMessage).FullName, subscriberEndpoint.Uri);

            SubscriptionChange msg = new SubscriptionChange("", null, SubscriptionChangeType.Add);

            VerifyMessageInQueue(storageEndpoint.QueueName, msg);
        }

        [Test]
        [ExpectedException(typeof(MessageQueueException))]
        public void I_Should_Puke_If_My_Storage_Queue_Doesnt_Exist()
        {
            IMessageQueueEndpoint nonExistentStorageEndpoint = new MessageQueueEndpoint(@"msmq://localhost/some_queue_that_doesnt_exist");

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(nonExistentStorageEndpoint, cache);
        }

        [Test]
        public void I_Should_Work_When_Removing()
        {
            IMessageQueueEndpoint storageEndpoint = base._serviceBusEndPoint;
            IMessageQueueEndpoint listenEndpoint = base._testEndPoint;
            IMessageQueueEndpoint subscriberEndpoint = base._remoteServiceBusEndPoint;

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(storageEndpoint, cache);

            storage.Add(typeof(PingMessage).FullName, subscriberEndpoint.Uri);

            Assert.AreEqual(1, storage.List().Count);
            storage.Remove(typeof(PingMessage).FullName, subscriberEndpoint.Uri);
            Assert.AreEqual(0, storage.List().Count);

            VerifyQueueIsEmpty(storageEndpoint.QueueName);
        }

    }
}
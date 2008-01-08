namespace MassTransit.ServiceBus.Tests
{
    using System.Messaging;
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
            IEndpoint storageEndpoint = base._serviceBusEndPoint;
            IEndpoint listenEndpoint = base._testEndPoint;
            IEndpoint subscriberEndpoint = base._remoteServiceBusEndPoint;

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(storageEndpoint, listenEndpoint, cache);

            storage.Add(typeof(PingMessage), subscriberEndpoint);

            SubscriptionMessage msg = new SubscriptionMessage(null, null, SubscriptionMessage.SubscriptionChangeType.Add);

            VerifyMessageInQueue(storageEndpoint.Address, msg);
        }

        [Test]
        [ExpectedException(typeof(MessageQueueException))]
        public void I_Should_Puke_If_My_Storage_Queue_Doesnt_Exist()
        {
            IEndpoint nonExistentStorageEndpoint = new MessageQueueEndpoint(@".\private$\some_queue_that_doesnt_exist");
            IEndpoint listenEndpoint = base._testEndPoint;

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(nonExistentStorageEndpoint, listenEndpoint, cache);
        }

        [Test]
        [ExpectedException(typeof(MessageQueueException))]
        public void I_Should_Puke_If_My_Listen_Queue_Doesnt_Exist()
        {
            IEndpoint storageEndpoint = base._serviceBusEndPoint;
            IEndpoint nonExistentListenEndpoint = new MessageQueueEndpoint(@".\private$\some_queue_that_doesnt_exist");

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(storageEndpoint, nonExistentListenEndpoint, cache);
        }

        [Test]
        public void I_Should_Work_When_Removing()
        {
            //TODO: This is nasty
            IEndpoint storageEndpoint = base._serviceBusEndPoint;
            IEndpoint listenEndpoint = base._testEndPoint;
            IEndpoint subscriberEndpoint = base._remoteServiceBusEndPoint;

            ISubscriptionStorage cache = new LocalSubscriptionCache();
            MsmqSubscriptionStorage storage = new MsmqSubscriptionStorage(storageEndpoint, listenEndpoint, cache);

            storage.Add(typeof(PingMessage), subscriberEndpoint);

            SubscriptionMessage msg = new SubscriptionMessage(null, null, SubscriptionMessage.SubscriptionChangeType.Add);

            storage.Remove(typeof(PingMessage), subscriberEndpoint);
            
            VerifyQueueIsEmpty(storageEndpoint.Address);
        }

    }
}
using System;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using MassTransit.ServiceBus.Subscriptions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    public abstract class ServiceBusSetupFixture
    {
        protected IServiceBus _serviceBus;
        protected IServiceBus _remoteServiceBus;

        protected MessageQueueEndpoint _serviceBusEndPoint;
        protected MessageQueueEndpoint _remoteServiceBusEndPoint;
        protected MessageQueueEndpoint _testEndPoint;
        protected MessageQueueEndpoint _subscriptionEndPoint;

        protected ISubscriptionStorage _subscriptionCache;
        protected ISubscriptionStorage _subscriptionStorage;

        protected ISubscriptionStorage _remoteSubscriptionCache;
        protected ISubscriptionStorage _remoteSubscriptionStorage;

        [SetUp]
        public virtual void Before_Each_Test_In_The_Fixture()
        {
            _serviceBusEndPoint = @".\private$\test_servicebus";
            _remoteServiceBusEndPoint = @".\private$\test_remoteservicebus";
            _testEndPoint = @".\private$\test_endpoint";
            _subscriptionEndPoint = @".\private$\test_subscriptions";

            ValidateAndPurgeQueue(_serviceBusEndPoint);
            ValidateAndPurgeQueue(_remoteServiceBusEndPoint);
            ValidateAndPurgeQueue(_testEndPoint);
            ValidateAndPurgeQueue(_subscriptionEndPoint);

            _subscriptionCache = new SubscriptionCache();

            _subscriptionStorage = new MsmqSubscriptionStorage(_subscriptionEndPoint.Transport.Address, _serviceBusEndPoint,
                                                               _subscriptionCache);

            ServiceBus bus = new ServiceBus(_serviceBusEndPoint, _testEndPoint);
            bus.SubscriptionStorage = _subscriptionStorage;

            _serviceBus = bus;

            _remoteSubscriptionCache = new SubscriptionCache();

            _remoteSubscriptionStorage = new MsmqSubscriptionStorage(_subscriptionEndPoint.Transport.Address, _remoteServiceBusEndPoint,
                                                               _remoteSubscriptionCache);

            bus = new ServiceBus(_remoteServiceBusEndPoint, _testEndPoint, _serviceBusEndPoint);
            bus.SubscriptionStorage = _remoteSubscriptionStorage;

            _remoteServiceBus = bus;
        }

        [TearDown]
        public virtual void After_Each_Test_In_The_Fixture()
        {
            //TeardownQueue(_serviceBusEndPoint);
            //TeardownQueue(_testEndPoint);
            //TeardownQueue(_subscriptionEndPoint);
            //_serviceBus.Dispose();
        }

        protected static void TeardownQueue(string point)
        {
            if(MessageQueue.Exists(point))
                MessageQueue.Delete(point);
        }

        protected static void ValidateAndPurgeQueue(string queuePath)
        {
            try
            {
                MessageQueue.Create(queuePath);
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.QueueExists)
                    throw;
            }

            MessageQueue queue = new MessageQueue(queuePath, QueueAccessMode.ReceiveAndAdmin);
            queue.Purge();
        }

        protected static void VerifyMessageInQueue<T>(string queuePath, T message)
        {
            using (MessageQueue mq = new MessageQueue(queuePath, QueueAccessMode.Receive))
            {
                Message msg = mq.Receive(TimeSpan.FromSeconds(30));

                IMessage[] messages = new BinaryFormatter().Deserialize(msg.BodyStream) as IMessage[];

                Assert.That(messages, Is.Not.Null);
                if (messages != null)
                {
                    Assert.That(messages.Length, Is.EqualTo(1));

                    Assert.That(messages[0].GetType(), Is.EqualTo(typeof (T)));
                }
            }
        }
    }
}
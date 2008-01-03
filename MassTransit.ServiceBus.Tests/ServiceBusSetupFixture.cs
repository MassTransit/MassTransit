using System;
using System.Messaging;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using MassTransit.ServiceBus.Subscriptions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    public abstract class ServiceBusSetupFixture
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected IServiceBus _serviceBus;
        protected IServiceBus _remoteServiceBus;

        protected MessageQueueEndpoint _serviceBusEndPoint;
        protected MessageQueueEndpoint _remoteServiceBusEndPoint;
        protected MessageQueueEndpoint _testEndPoint;

        protected string _serviceBusQueueName = @".\private$\test_servicebus";
        protected string _remoteServiceBusQueueName = @".\private$\test_remoteservicebus";
        protected string _testEndPointQueueName = @".\private$\test_endpoint";
        protected string _subscriptionQueueName = @".\private$\test_subscriptions";
        protected string _poisonQueueName = @".\private$\test_servicebus_poison";

        [SetUp]
        public virtual void Before_Each_Test_In_The_Fixture()
        {
            _log.Debug("Starting Test");

            ValidateAndPurgeQueue(_serviceBusQueueName);
            ValidateAndPurgeQueue(_remoteServiceBusQueueName);
            ValidateAndPurgeQueue(_testEndPointQueueName);
            ValidateAndPurgeQueue(_subscriptionQueueName);
            ValidateAndPurgeQueue(_poisonQueueName);

            _serviceBusEndPoint = @".\private$\test_servicebus";
            _remoteServiceBusEndPoint = @".\private$\test_remoteservicebus";
            _testEndPoint = @".\private$\test_endpoint";

            ISubscriptionStorage _subscriptionCache;
            ISubscriptionStorage _subscriptionStorage;

            ISubscriptionStorage _remoteSubscriptionCache;
            ISubscriptionStorage _remoteSubscriptionStorage;

            _subscriptionCache = new SubscriptionCache();
            _subscriptionStorage =
                new MsmqSubscriptionStorage(_subscriptionQueueName, _serviceBusEndPoint, _subscriptionCache);

            _serviceBus = new ServiceBus(_serviceBusEndPoint, _subscriptionStorage);


            
            _remoteSubscriptionCache = new SubscriptionCache();
            _remoteSubscriptionStorage =
                new MsmqSubscriptionStorage(_subscriptionQueueName, _remoteServiceBusEndPoint, _remoteSubscriptionCache);
            _remoteServiceBus = new ServiceBus(_remoteServiceBusEndPoint, _remoteSubscriptionStorage);
        }

        [TearDown]
        public virtual void After_Each_Test_In_The_Fixture()
        {
            _log.Debug("Ending Test");

            _serviceBus.Dispose();
            _remoteServiceBus.Dispose();

            _serviceBusEndPoint.Dispose();
            _remoteServiceBusEndPoint.Dispose();

            _testEndPoint.Dispose();


            TeardownQueue(_serviceBusQueueName);
            TeardownQueue(_remoteServiceBusQueueName);
            TeardownQueue(_testEndPointQueueName);
            TeardownQueue(_subscriptionQueueName);
            TeardownQueue(_poisonQueueName);
        }

        protected static void TeardownQueue(string point)
        {
            if (MessageQueue.Exists(point))
                MessageQueue.Delete(point);
        }

        protected static void ValidateAndPurgeQueue(string queuePath)
        {
            ValidateAndPurgeQueue(queuePath, false);
        }

        protected static void ValidateAndPurgeQueue(string queuePath, bool isTransactional)
        {
            try
            {
                MessageQueue.Create(queuePath, isTransactional);
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.QueueExists)
                    _log.Error("Error creating queue: ", ex);
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
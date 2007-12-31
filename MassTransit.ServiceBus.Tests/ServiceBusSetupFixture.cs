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

    	protected string _subscriptionQueueName;
    	protected string _poisonQueueName;

        protected ISubscriptionStorage _subscriptionCache;
        protected ISubscriptionStorage _subscriptionStorage;

        protected ISubscriptionStorage _remoteSubscriptionCache;
        protected ISubscriptionStorage _remoteSubscriptionStorage;

        [SetUp]
        public virtual void Before_Each_Test_In_The_Fixture()
        {
        	_log.Debug("Starting Test");

            _serviceBusEndPoint = @".\private$\test_servicebus";
            _remoteServiceBusEndPoint = @".\private$\test_remoteservicebus";
            _testEndPoint = @".\private$\test_endpoint";
			_subscriptionQueueName = @".\private$\test_subscriptions";
			_poisonQueueName = @".\private$\test_poison";

            ValidateAndPurgeQueue(_serviceBusEndPoint);
            ValidateAndPurgeQueue(_remoteServiceBusEndPoint);
            ValidateAndPurgeQueue(_testEndPoint);
			ValidateAndPurgeQueue(_subscriptionQueueName);
			ValidateAndPurgeQueue(_poisonQueueName);

			ServiceBus bus = new ServiceBus(_serviceBusEndPoint, _testEndPoint);

			_subscriptionCache = new SubscriptionCache();
			_subscriptionStorage = new MsmqSubscriptionStorage(_subscriptionQueueName, _serviceBusEndPoint, _subscriptionCache);
			bus.SubscriptionStorage = _subscriptionStorage;

            _serviceBus = bus;

            bus = new ServiceBus(_remoteServiceBusEndPoint);

			_remoteSubscriptionCache = new SubscriptionCache();
			_remoteSubscriptionStorage = new MsmqSubscriptionStorage(_subscriptionQueueName, _remoteServiceBusEndPoint, _remoteSubscriptionCache);
			bus.SubscriptionStorage = _remoteSubscriptionStorage;

            _remoteServiceBus = bus;
        }

        [TearDown]
        public virtual void After_Each_Test_In_The_Fixture()
        {
			_log.Debug("Ending Test");

			//TeardownQueue(_serviceBusEndPoint);
            //TeardownQueue(_testEndPoint);
            //_serviceBus.Dispose();
        }

        protected static void TeardownQueue(string point)
        {
            if(MessageQueue.Exists(point))
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
                MessageQueue.Create(queuePath,isTransactional);
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
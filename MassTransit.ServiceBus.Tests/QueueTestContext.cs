using System;
using System.Diagnostics;
using System.Messaging;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using MassTransit.ServiceBus.Subscriptions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace MassTransit.ServiceBus.Tests
{
    public class QueueTestContext :
        IDisposable
    {
        protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ServiceBus _remoteServiceBus;
        private MessageQueueEndpoint _remoteServiceBusEndPoint = @"msmq://localhost/test_remoteservicebus";

        private IServiceBus _serviceBus;
        private MessageQueueEndpoint _serviceBusEndPoint = @"msmq://localhost/test_servicebus";
        private MessageQueueEndpoint _subscriptionEndpoint = @"msmq://localhost/test_subscriptions";

        public QueueTestContext()
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            _log.InfoFormat("QueueTestContext Created for {0}", methodBase.Name);

            MessageQueue.EnableConnectionCache = false;

            ValidateAndPurgeQueue(_serviceBusEndPoint.QueueName);
            ValidateAndPurgeQueue(_remoteServiceBusEndPoint.QueueName);
            ValidateAndPurgeQueue(_subscriptionEndpoint.QueueName);

            IServiceBus ignore = ServiceBus;
        }

        public IServiceBus ServiceBus
        {
            get
            {
                if (_serviceBus == null)
                {
                    ISubscriptionStorage subscriptionStorage = CreateSubscriptionStorage();

                    _serviceBus = new ServiceBus(ServiceBusEndPoint, subscriptionStorage);
                }

                return _serviceBus;
            }
        }

        public IServiceBus RemoteServiceBus
        {
            get
            {
                if (_remoteServiceBus == null)
                {
                    ISubscriptionStorage subscriptionStorage = CreateSubscriptionStorage();

                    _remoteServiceBus = new ServiceBus(RemoteServiceBusEndPoint, subscriptionStorage);
                }

                return _remoteServiceBus;
            }
        }

        public IMessageQueueEndpoint RemoteServiceBusEndPoint
        {
            get { return _remoteServiceBusEndPoint; }
        }


        public IMessageQueueEndpoint ServiceBusEndPoint
        {
            get { return _serviceBusEndPoint; }
        }

        public IMessageQueueEndpoint SubscriptionEndpoint
        {
            get { return _subscriptionEndpoint; }
        }

        #region IDisposable Members

        public void Dispose()
        {
            _log.Info("QueueTestContext Disposing");

            if (_remoteServiceBus != null)
                _remoteServiceBus.Dispose();

            if (_serviceBus != null)
                _serviceBus.Dispose();
        }

        #endregion

        private ISubscriptionStorage CreateSubscriptionStorage()
        {
            ISubscriptionStorage subscriptionCache;
            ISubscriptionStorage subscriptionStorage;

            subscriptionCache = new LocalSubscriptionCache();
            subscriptionStorage =
                new MsmqSubscriptionStorage(SubscriptionEndpoint, subscriptionCache);

            return subscriptionStorage;
        }

        public static void VerifyMessageInQueue<T>(string queuePath, T message)
        {
            using (MessageQueue mq = new MessageQueue(queuePath, QueueAccessMode.Receive))
            {
                Message msg = mq.Receive(TimeSpan.FromSeconds(3));

                IMessage[] messages = new BinaryFormatter().Deserialize(msg.BodyStream) as IMessage[];

                Assert.That(messages, Is.Not.Null);
                if (messages != null)
                {
                    Assert.That(messages.Length, Is.EqualTo(1));

                    Assert.That(messages[0].GetType(), Is.EqualTo(typeof (T)));
                }
            }
        }

        private static void ValidateAndPurgeQueue(string queuePath)
        {
            ValidateAndPurgeQueue(queuePath, false);
        }

        private static void ValidateAndPurgeQueue(string queuePath, bool isTransactional)
        {
            try
            {
                MessageQueue.Create(queuePath, isTransactional);
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode != MessageQueueErrorCode.QueueExists)
                {
                }
            }

            MessageQueue queue = new MessageQueue(queuePath, QueueAccessMode.ReceiveAndAdmin);
            queue.Purge();
        }
    }
}
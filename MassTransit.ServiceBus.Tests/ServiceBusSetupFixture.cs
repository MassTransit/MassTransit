using System;
using System.Diagnostics;
using System.Messaging;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using log4net;
using MassTransit.ServiceBus.Subscriptions;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using System.IO;

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
        protected MessageQueueEndpoint _storageEndPoint;

        protected string _serviceBusQueueName = @".\private$\test_servicebus";
        protected string _remoteServiceBusQueueName = @".\private$\test_remoteservicebus";
        protected string _testEndPointQueueName = @".\private$\test_endpoint";
        protected string _poisonQueueName = @".\private$\test_servicebus_poison";

        [SetUp]
        public virtual void Before_Each_Test_In_The_Fixture()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(@".\log4net.config"));
            //TODO: Is this necessary still?
        	MessageQueue.EnableConnectionCache = false;

            ValidateAndPurgeQueue(_serviceBusQueueName);
            ValidateAndPurgeQueue(_remoteServiceBusQueueName);
            ValidateAndPurgeQueue(_testEndPointQueueName);
            ValidateAndPurgeQueue(_poisonQueueName);

            _serviceBusEndPoint = @".\private$\test_servicebus";
            _remoteServiceBusEndPoint = @".\private$\test_remoteservicebus";
            _testEndPoint = @".\private$\test_endpoint";

            ISubscriptionStorage _subscriptionCache = new LocalSubscriptionCache();

            _serviceBus = new ServiceBus(_serviceBusEndPoint, _subscriptionCache);
            _remoteServiceBus = new ServiceBus(_remoteServiceBusEndPoint, _subscriptionCache);
        }

        [TearDown]
        public virtual void After_Each_Test_In_The_Fixture()
        {
            _log.Debug("Ending Test");

            _serviceBus.Dispose();
            
            _remoteServiceBus.Dispose();


            // TODO: Are these screwing up asyncs?
            //_serviceBusEndPoint.Dispose();
            //_remoteServiceBusEndPoint.Dispose();
            //_testEndPoint.Dispose();


            //TeardownQueue(_serviceBusQueueName);
            //TeardownQueue(_remoteServiceBusQueueName);
            //TeardownQueue(_testEndPointQueueName);
            //TeardownQueue(_subscriptionQueueName);
            //TeardownQueue(_poisonQueueName);
        }

        protected static void TeardownQueue(string queuePath)
        {
            if (MessageQueue.Exists(queuePath))
                MessageQueue.Delete(queuePath);
        }

        public static void ValidateAndPurgeQueue(string queuePath)
        {
            ValidateAndPurgeQueue(queuePath, false);
        }

        public static void ValidateAndPurgeQueue(string queuePath, bool isTransactional)
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

        public static void VerifyMessageInQueue<T>(string queuePath, T message)
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

        public static void VerifyQueueIsEmpty(string queuePath)
        {
            PerformanceCounterCategory myCat = new PerformanceCounterCategory("MSMQ Queue");

            PerformanceCounter cntr = new PerformanceCounter();

            cntr.CategoryName = "MSMQ Queue";

            cntr.CounterName = "Messages in Queue";



            foreach (string inst in myCat.GetInstanceNames())
            {
                if(inst.Equals(queuePath, StringComparison.InvariantCultureIgnoreCase))
                {
                    cntr.InstanceName = inst;
                    Assert.AreEqual(0.0f, cntr.NextValue());
                }
            } 
        }
    }
}
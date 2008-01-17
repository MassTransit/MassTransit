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

        protected MessageQueueEndpoint _serviceBusEndPoint = @"msmq://localhost/test_servicebus";
        protected MessageQueueEndpoint _remoteServiceBusEndPoint = @"msmq://localhost/test_remoteservicebus";
        protected MessageQueueEndpoint _testEndPoint = @"msmq://localhost/test_endpoint";
        protected MessageQueueEndpoint _poisonEndpoint = @"msmq://localhost/test_servicebus_poison";

        protected string _serviceBusQueueName = @"msmq://localhost/test_servicebus";
        protected string _remoteServiceBusQueueName = @"msmq://localhost/test_remoteservicebus";
        protected string _testEndPointQueueName = @"msmq://localhost/test_endpoint";
        protected string _poisonQueueName = @"msmq://localhost/test_servicebus_poison";

        [SetUp]
        public virtual void Before_Each_Test_In_The_Fixture()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(@".\log4net.config"));

            //TODO: Is this necessary still?
        	MessageQueue.EnableConnectionCache = false;

            ValidateAndPurgeQueue(_serviceBusEndPoint.QueueName);
            ValidateAndPurgeQueue(_remoteServiceBusEndPoint.QueueName);
            ValidateAndPurgeQueue(_testEndPoint.QueueName);
            ValidateAndPurgeQueue(_poisonEndpoint.QueueName);

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
                MessageQueue.Create(GetQueueName(queuePath), isTransactional);
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
                    Assert.AreEqual(0.0f, cntr.NextValue(), "the queue should have been empty and wasn't");
                }
            } 
        }

        public static string GetQueueName(string name)
        {
            string result = name;
            if (result.Contains("FormatName:DIRECT=OS:"))
                result = result.Replace("FormatName:DIRECT=OS:", "");
            if (result.Contains("localhost"))
                result = result.Replace("localhost", ".");
            if (result.Contains(Environment.MachineName.ToLowerInvariant()))
                result = result.Replace(Environment.MachineName.ToLowerInvariant(), ".");

            return result;
        }
    }
}
// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Transports.Msmq.Tests
{
    using System;
    using System.Diagnostics;
    using System.Messaging;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Transactions;
    using log4net;
    using NUnit.Framework;
    using NUnit.Framework.SyntaxHelpers;

    public class QueueTestContext :
        IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly MsmqEndpoint _remoteServiceBusEndPoint = new MsmqEndpoint(@"msmq://localhost/test_remoteservicebus");
        private readonly MsmqEndpoint _serviceBusEndPoint = new MsmqEndpoint(@"msmq://localhost/test_servicebus");
        private readonly MsmqEndpoint _subscriptionEndpoint = new MsmqEndpoint(@"msmq://localhost/test_subscriptions");

        private ServiceBus _remoteServiceBus;
        private ServiceBus _serviceBus;

    	public QueueTestContext(IObjectBuilder objectBuilder) : this(objectBuilder, "localhost")
        {
        }
        public QueueTestContext(IObjectBuilder objectBuilder, string remoteMachineName)
        {
            _remoteServiceBusEndPoint = new MsmqEndpoint("msmq://" + remoteMachineName + "/test_remoteservicebus");
            _serviceBusEndPoint = new MsmqEndpoint("msmq://" + remoteMachineName + "/test_servicebus");

            Initialize(objectBuilder);
        }


        public IServiceBus ServiceBus
        {
            get
            {
                return _serviceBus;
            }
        }

        public IServiceBus RemoteServiceBus
        {
            get
            {
                return _remoteServiceBus;
            }
        }


    	public IEndpoint RemoteServiceBusEndPoint
        {
            get { return _remoteServiceBusEndPoint; }
        }


        public IEndpoint ServiceBusEndPoint
        {
            get { return _serviceBusEndPoint; }
        }

        public IEndpoint SubscriptionEndpoint
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

        private void Initialize(IObjectBuilder objectBuilder)
        {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();

            _log.InfoFormat("QueueTestContext Created for {0}", methodBase.Name);

            MessageQueue.EnableConnectionCache = false;

            ValidateAndPurgeQueue(_serviceBusEndPoint.QueuePath);
            ValidateAndPurgeQueue(_remoteServiceBusEndPoint.QueuePath);
            ValidateAndPurgeQueue(_subscriptionEndpoint.QueuePath);




//            _serviceBus = MassTransit.ServiceBus.Build()
//                .SupportingTransport<MsmqEndpoint>()
//                .ListeningOn(_serviceBusEndPoint.Uri)
//                .UsingObjectBuilder(objectBuilder);
//
//            _remoteServiceBus = MassTransit.ServiceBus.Build()
//                .SupportingTransport<MsmqEndpoint>()
//                .ListeningOn(_remoteServiceBusEndPoint.Uri)
//                .UsingObjectBuilder(objectBuilder);
        }

        public static void VerifyMessageInQueue<T>(string queuePath, T messageItem)
        {
            using (MessageQueue mq = new MessageQueue(GetQueueName(queuePath), QueueAccessMode.Receive))
            {
                Message msg = mq.Receive(TimeSpan.FromSeconds(3));

                object message = new BinaryFormatter().Deserialize(msg.BodyStream);

                Assert.That(message, Is.Not.Null);
                if (message != null)
                {
                    Assert.That(message.GetType(), Is.EqualTo(typeof (T)));
                }
            }
        }

        public static void VerifyMessageInQueue<T>(MsmqEndpoint ep, T messageItem)
        {
            using (MessageQueue mq = new MessageQueue(ep.QueuePath, QueueAccessMode.Receive))
            {
                Message msg = mq.Receive(TimeSpan.FromSeconds(3));

                object message = new BinaryFormatter().Deserialize(msg.BodyStream);

                Assert.That(message, Is.Not.Null);
                if (message != null)
                {
                    Assert.That(message.GetType(), Is.EqualTo(typeof(T)));
                }
            }
        }
        public static void VerifyMessageInTransactionalQueue<T>(MsmqEndpoint ep, T messageItem)
        {
            using(TransactionScope trx = new TransactionScope())
            {
                VerifyMessageInQueue(ep, messageItem);
                trx.Complete();
            }
        }

        public static void VerifyMessageNotInTransactionalQueue(MsmqEndpoint ep)
        {
            using(TransactionScope trx = new TransactionScope())
            {
                VerifyMessageNotInQueue(ep);
                trx.Complete();
            }
        }
        public static void VerifyMessageNotInQueue(MsmqEndpoint ep)
        {
            using (MessageQueue mq = new MessageQueue(ep.QueuePath, QueueAccessMode.Receive))
            {
                try
                {
                    Message msg = mq.Receive(TimeSpan.FromSeconds(0.1));
                    Assert.IsNull(msg);
                }
                catch (MessageQueueException ex)
                {
                    Assert.AreEqual("Timeout for the requested operation has expired.", ex.Message);
                }
            }
        }

        public static void ValidateAndPurgeQueue(string queuePath)
        {
            ValidateAndPurgeQueue(queuePath, false);
        }

        public static void ValidateAndPurgeQueue(string queuePath, bool isTransactional)
        {
            try
            {
                MessageQueue queue = new MessageQueue(GetQueueName(queuePath), QueueAccessMode.ReceiveAndAdmin);
                queue.Purge();
            }
            catch (MessageQueueException ex)
            {
                if (ex.MessageQueueErrorCode == MessageQueueErrorCode.QueueNotFound)
                {
                    MessageQueue.Create(GetQueueName(queuePath), isTransactional);
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
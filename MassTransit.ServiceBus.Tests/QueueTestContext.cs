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

		private readonly string _remoteServiceBusQueueName = @".\private$\test_remoteservicebus";
		private readonly string _serviceBusQueueName = @".\private$\test_servicebus";
		private readonly string _subscriptionQueueName = @".\private$\test_subscriptions";

		private ServiceBus _remoteServiceBus;
		private MessageQueueEndpoint _remoteServiceBusEndPoint;

		private IServiceBus _serviceBus;
		private MessageQueueEndpoint _serviceBusEndPoint;

		private MessageQueueEndpoint _subscriptionEndpoint;

		public QueueTestContext()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(1);
			MethodBase methodBase = stackFrame.GetMethod();

			_log.InfoFormat("QueueTestContext Created for {0}", methodBase.Name);

			MessageQueue.EnableConnectionCache = false;

			IServiceBus ignore = ServiceBus;
		}

		public IServiceBus ServiceBus
		{
			get
			{
				if (_serviceBus == null)
				{
					ISubscriptionStorage subscriptionStorage = CreateSubscriptionStorage(ServiceBusEndPoint);

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
					ISubscriptionStorage subscriptionStorage = CreateSubscriptionStorage(ServiceBusEndPoint);

					_remoteServiceBus = new ServiceBus(RemoteServiceBusEndPoint, subscriptionStorage);
				}

				return _remoteServiceBus;
			}
		}

		public IEndpoint RemoteServiceBusEndPoint
		{
			get
			{
				if (_remoteServiceBusEndPoint == null)
				{
					ValidateAndPurgeQueue(_remoteServiceBusQueueName);

					_remoteServiceBusEndPoint = new MessageQueueEndpoint(_remoteServiceBusQueueName);
				}

				return _remoteServiceBusEndPoint;
			}
		}


		public IEndpoint ServiceBusEndPoint
		{
			get
			{
				if (_serviceBusEndPoint == null)
				{
					ValidateAndPurgeQueue(_serviceBusQueueName);

					_serviceBusEndPoint = new MessageQueueEndpoint(_serviceBusQueueName);
				}

				return _serviceBusEndPoint;
			}
		}

		public IEndpoint SubscriptionEndpoint
		{
			get
			{
				if (_subscriptionEndpoint == null)
				{
					ValidateAndPurgeQueue(_subscriptionQueueName);

					_subscriptionEndpoint = new MessageQueueEndpoint(_subscriptionQueueName);
				}

				return _subscriptionEndpoint;
			}
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

		private ISubscriptionStorage CreateSubscriptionStorage(IEndpoint endpoint)
		{
			ISubscriptionStorage subscriptionCache;
			ISubscriptionStorage subscriptionStorage;

			subscriptionCache = new LocalSubscriptionCache();
			subscriptionStorage =
				new MsmqSubscriptionStorage(SubscriptionEndpoint, endpoint, subscriptionCache);

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
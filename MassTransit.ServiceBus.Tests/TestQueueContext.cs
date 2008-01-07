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
	public class TestQueueContext : IDisposable
	{
		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly string _serviceBusQueueName = @".\private$\test_servicebus";
		private MessageQueueEndpoint _serviceBusEndPoint;
		private IServiceBus _serviceBus;
		private ServiceBus _remoteServiceBus;
		private readonly string _remoteServiceBusQueueName = @".\private$\test_remoteservicebus";
		private MessageQueueEndpoint _remoteServiceBusEndPoint;

		private string _subscriptionQueueName = @".\private$\test_subscriptions";

		public TestQueueContext()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(1);
			MethodBase methodBase = stackFrame.GetMethod();

			_log.InfoFormat("TestQueueContext Created for {0}", methodBase.Name);

			MessageQueue.EnableConnectionCache = false;

			// we need to create this first so we can actually receive and send messages
            //TODO: Why not just do this?
			IServiceBus ignore = this.ServiceBus;
		}

		public IServiceBus ServiceBus
		{
			get
			{
				if (_serviceBus == null)
				{
					ServiceBus bus = new ServiceBus(ServiceBusEndPoint);

					bus.SubscriptionStorage = CreateSubscriptionStorage(_subscriptionQueueName, bus.Endpoint);

					_serviceBus = bus;
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
					ServiceBus bus = new ServiceBus(RemoteServiceBusEndPoint);

					bus.SubscriptionStorage = CreateSubscriptionStorage(_subscriptionQueueName, bus.Endpoint);

					_remoteServiceBus = bus;
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

					_remoteServiceBusEndPoint = MessageQueueEndpointFactory.Instance.Resolve(_remoteServiceBusQueueName);
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

					_serviceBusEndPoint = MessageQueueEndpointFactory.Instance.Resolve(_serviceBusQueueName);
				}

				return _serviceBusEndPoint;
			}
		}

		private static ISubscriptionStorage CreateSubscriptionStorage(string queuePath, IEndpoint endpoint)
		{
			ValidateAndPurgeQueue(queuePath);

			ISubscriptionStorage subscriptionCache;
			ISubscriptionStorage subscriptionStorage;

			subscriptionCache = new SubscriptionCache();
			subscriptionStorage =
				new MsmqSubscriptionStorage(queuePath, endpoint, subscriptionCache);

			return subscriptionStorage;
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

		public void Dispose()
		{
			_log.Info("TestQueueContext Disposing");

			if (_remoteServiceBus != null)
				_remoteServiceBus.Dispose();

			if (_serviceBus != null)
				_serviceBus.Dispose();
		}
	}
}
namespace MassTransit.ServiceBus.MSMQ.Tests
{
	using System;
	using System.Diagnostics;
	using System.Messaging;
	using System.Reflection;
	using System.Runtime.Serialization.Formatters.Binary;
	using log4net;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using Subscriptions;

	public class QueueTestContext :
		IDisposable
	{
		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly MsmqEndpoint _remoteServiceBusEndPoint = @"msmq://localhost/test_remoteservicebus";

		private readonly MsmqEndpoint _serviceBusEndPoint = @"msmq://localhost/test_servicebus";
		private readonly MsmqEndpoint _subscriptionEndpoint = @"msmq://localhost/test_subscriptions";
		private ServiceBus _remoteServiceBus;
		private IServiceBus _serviceBus;

		public QueueTestContext(string remoteMachineName)
		{
			_remoteServiceBusEndPoint = "msmq://" + remoteMachineName + "/test_remoteservicebus";
			_serviceBusEndPoint = "msmq://" + remoteMachineName + "/test_servicebus";

			Initialize();
		}

		public QueueTestContext()
		{
			Initialize();
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

		public IMsmqEndpoint RemoteServiceBusEndPoint
		{
			get { return _remoteServiceBusEndPoint; }
		}


		public IMsmqEndpoint ServiceBusEndPoint
		{
			get { return _serviceBusEndPoint; }
		}

		public IMsmqEndpoint SubscriptionEndpoint
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

		private void Initialize()
		{
			StackTrace stackTrace = new StackTrace();
			StackFrame stackFrame = stackTrace.GetFrame(1);
			MethodBase methodBase = stackFrame.GetMethod();

			_log.InfoFormat("QueueTestContext Created for {0}", methodBase.Name);

			MessageQueue.EnableConnectionCache = false;

			ValidateAndPurgeQueue(_serviceBusEndPoint.QueuePath);
			ValidateAndPurgeQueue(_remoteServiceBusEndPoint.QueuePath);
			ValidateAndPurgeQueue(_subscriptionEndpoint.QueuePath);

			IServiceBus ignore = ServiceBus;
		}

		private ISubscriptionStorage CreateSubscriptionStorage()
		{
			ISubscriptionStorage subscriptionCache;

			subscriptionCache = new LocalSubscriptionCache();

			return subscriptionCache;
		}

		public static void VerifyMessageInQueue<T>(string queuePath, T message)
		{
			using (MessageQueue mq = new MessageQueue(GetQueueName(queuePath), QueueAccessMode.Receive))
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
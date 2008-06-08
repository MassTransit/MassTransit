namespace MassTransit.DistributedSubscriptionCache.Tests
{
	using System;
	using System.Diagnostics;
	using System.Messaging;
	using System.Reflection;
	using System.Runtime.Serialization.Formatters.Binary;
	using log4net;
	using MassTransit.ServiceBus.MSMQ;
	using NUnit.Framework;
	using NUnit.Framework.SyntaxHelpers;
	using ServiceBus;

	public class DistributedQueueContext : IDisposable
	{
		protected static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private readonly DistributedSubscriptionCache _cache = new DistributedSubscriptionCache();
		private readonly DistributedSubscriptionCache _remoteCache = new DistributedSubscriptionCache();
		private readonly MsmqEndpoint _remoteServiceBusEndPoint = @"msmq://localhost/test_remoteservicebus";
		private readonly MsmqEndpoint _serviceBusEndPoint = @"msmq://localhost/test_servicebus";
		private ServiceBus _remoteServiceBus;
		private IServiceBus _serviceBus;

		public DistributedQueueContext(string remoteMachineName)
		{
			_remoteServiceBusEndPoint = "msmq://" + remoteMachineName + "/test_remoteservicebus";
			_serviceBusEndPoint = "msmq://" + remoteMachineName + "/test_servicebus";

			Initialize();
		}

		public DistributedQueueContext()
		{
			Initialize();
		}

		public IServiceBus ServiceBus
		{
			get { return _serviceBus; }
		}

		public IServiceBus RemoteServiceBus
		{
			get { return _remoteServiceBus; }
		}

		public IMsmqEndpoint RemoteServiceBusEndPoint
		{
			get { return _remoteServiceBusEndPoint; }
		}

		public IMsmqEndpoint ServiceBusEndPoint
		{
			get { return _serviceBusEndPoint; }
		}

		#region IDisposable Members

		public void Dispose()
		{
			_log.Info("DistributedQueueContext Disposing");

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

			_log.InfoFormat("DistributedQueueContext Created for {0}", methodBase.Name);

			MessageQueue.EnableConnectionCache = false;

			ValidateAndPurgeQueue(_serviceBusEndPoint.QueuePath);
			ValidateAndPurgeQueue(_remoteServiceBusEndPoint.QueuePath);

			_serviceBus = new ServiceBus(ServiceBusEndPoint, null, _cache);
			_remoteServiceBus = new ServiceBus(RemoteServiceBusEndPoint, null, _remoteCache);
		}

		public static void VerifyMessageInQueue<T>(string queuePath, T messageExample)
		{
			using (MessageQueue mq = new MessageQueue(GetQueueName(queuePath), QueueAccessMode.Receive))
			{
				Message msg = mq.Receive(TimeSpan.FromSeconds(3));

				object message = new BinaryFormatter().Deserialize(msg.BodyStream);

				Assert.That(message, Is.Not.Null);
				Assert.That(message.GetType(), Is.EqualTo(typeof (T)));
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
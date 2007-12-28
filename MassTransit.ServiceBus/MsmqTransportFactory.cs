using System;
using System.Collections.Generic;
using System.Messaging;

namespace MassTransit.ServiceBus
{
	public class MsmqTransportFactory
	{
		private static readonly MsmqTransportFactory _instance;
		private readonly Dictionary<string, MsmqTransport> _transportCache;

		static MsmqTransportFactory()
		{
			_instance = new MsmqTransportFactory();
		}

		protected MsmqTransportFactory()
		{
			_transportCache = new Dictionary<string, MsmqTransport>();
		}

		public ITransport Resolve(string queuePath)
		{
			string queueName = NormalizeQueueName(queuePath);

			lock (_instance)
			{
				if (_transportCache.ContainsKey(queueName))
					return _transportCache[queueName];

				MsmqTransport transport = new MsmqTransport(queueName);

				_transportCache.Add(queueName, transport);

				return transport;
			}
		}

		private static string NormalizeQueueName(string queuePath)
		{
			using (MessageQueue queue = new MessageQueue(queuePath))
			{
				string machineName = queue.MachineName;
				if (machineName == "." || string.Compare(machineName, "localhost", true) == 0)
				{
					queue.MachineName = Environment.MachineName;
				}

				return queue.Path;
			}
		}

		public static MsmqTransportFactory Instance
		{
			get { return _instance; }
		}
	}
}
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
			string key;
			string queueName = NormalizeQueueName(queuePath, out key);

			lock (_instance)
			{
				if (_transportCache.ContainsKey(key))
					return _transportCache[key];

				MsmqTransport transport = new MsmqTransport(queueName);

				_transportCache.Add(key, transport);

				return transport;
			}
		}

		private static string NormalizeQueueName(string queuePath, out string key)
		{
			using (MessageQueue queue = new MessageQueue(queuePath))
			{
				string machineName = queue.MachineName;
				if (machineName == "." || string.Compare(machineName, "localhost", true) == 0)
				{
					queue.MachineName = Environment.MachineName;
				}

				key = queue.Path.Replace("FORMATNAME:DIRECT=OS:", "");

				return queue.Path;
			}
		}

		public static MsmqTransportFactory Instance
		{
			get { return _instance; }
		}
	}
}
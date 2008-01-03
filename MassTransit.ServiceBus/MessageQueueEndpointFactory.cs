using System;
using System.Collections.Generic;
using System.Messaging;

namespace MassTransit.ServiceBus
{
	public partial class MessageQueueEndpoint
	{
		public class MessageQueueEndpointFactory
		{
			private static readonly MessageQueueEndpointFactory _instance;
			private readonly Dictionary<string, MessageQueueEndpoint> _transportCache;

			static MessageQueueEndpointFactory()
			{
				_instance = new MessageQueueEndpointFactory();
			}

			protected MessageQueueEndpointFactory()
			{
				_transportCache = new Dictionary<string, MessageQueueEndpoint>();
			}

			public MessageQueueEndpoint Resolve(string queuePath)
			{
				string key;
				string queueName = NormalizeQueueName(queuePath, out key);

				lock (_instance)
				{
					if (_transportCache.ContainsKey(key))
						return _transportCache[key];

					MessageQueueEndpoint transport = new MessageQueueEndpoint(queueName);

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

			public static MessageQueueEndpointFactory Instance
			{
				get { return _instance; }
			}
		}

	}
}
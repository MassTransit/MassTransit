/// Copyright 2007-2008 The Apache Software Foundation.
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
/// this file except in compliance with the License. You may obtain a copy of the 
/// License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software distributed 
/// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
/// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
/// specific language governing permissions and limitations under the License.
namespace MassTransit.ServiceBus.Internal
{
	using System;
	using System.Collections.Generic;
	using Subscriptions;

	/// <summary>
	/// Manages and dispatches messages to correlated message consumers
	/// </summary>
	public class MessageTypeDispatcher :
		IMessageTypeDispatcher
	{
		private static readonly Type _consumes = typeof (Consumes<>.All);
		private static readonly Type _consumesSelected = typeof (Consumes<>.Selected);
		private readonly IObjectBuilder _builder;
		private readonly IServiceBus _bus;
		private readonly ISubscriptionCache _cache;
		private readonly Dictionary<Type, IMessageDispatcher> _messageDispatchers = new Dictionary<Type, IMessageDispatcher>();
		private readonly object _messageLock = new object();
		private readonly Dictionary<Type, Type> _messageTypeToKeyType = new Dictionary<Type, Type>();

		public MessageTypeDispatcher()
		{
		}

		public MessageTypeDispatcher(IServiceBus bus, ISubscriptionCache cache, IObjectBuilder builder)
		{
			_bus = bus;
			_cache = cache;
			_builder = builder;
		}

		public bool Accept(object message)
		{
			Type messageType = message.GetType();

			IMessageDispatcher dispatcher;

			bool found;
			lock (_messageDispatchers)
				found = _messageDispatchers.TryGetValue(messageType, out dispatcher);

			if (found)
				return dispatcher.Accept(message);

			return false;
		}

		public void Consume(object message)
		{
			Type messageType = message.GetType();

			IMessageDispatcher dispatcher;

			bool found;
			lock (_messageDispatchers)
				found = _messageDispatchers.TryGetValue(messageType, out dispatcher);

			if (found)
				dispatcher.Consume(message);
		}

		public void Dispose()
		{
		}

		public void Attach<T>(Consumes<T>.All consumer) where T : class
		{
			Produces<T> dispatcher = GetMessageProducer<T>();

			dispatcher.Attach(consumer);
		}

		public void Detach<T>(Consumes<T>.All consumer) where T : class
		{
			Produces<T> dispatcher = GetMessageProducer<T>();

			dispatcher.Detach(consumer);
		}

		private Produces<T> GetMessageProducer<T>() where T : class
		{
			Type messageType = typeof (T);

			lock (_messageLock)
			{
				IMessageDispatcher consumer;
				if (_messageDispatchers.TryGetValue(messageType, out consumer))
				{
					return (IMessageDispatcher<T>) consumer;
				}

				Type dispatcherType = typeof (MessageDispatcher<>).MakeGenericType(messageType);

				consumer = (IMessageDispatcher) Activator.CreateInstance(dispatcherType, _bus, _cache, _builder);

				_messageDispatchers.Add(messageType, consumer);

				return (Produces<T>) consumer;
			}
		}
	}
}
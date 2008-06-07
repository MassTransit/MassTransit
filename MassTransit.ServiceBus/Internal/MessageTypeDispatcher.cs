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
		IMessageDispatcher
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

		public void Subscribe<T>(T component) where T : class
		{
			Type componentType = typeof (T);

			SubscriptionTypeInfo info = SubscriptionTypeInfo.Resolve(componentType, _builder);

			info.Subscribe(this, component);
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			Type componentType = typeof(T);

			SubscriptionTypeInfo info = SubscriptionTypeInfo.Resolve(componentType, _builder);

			info.Unsubscribe(this, component);
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			if (_builder == null)
				throw new ArgumentException("No object builder interface is available");

			Type componentType = typeof (TComponent);

			SubscriptionTypeInfo info = SubscriptionTypeInfo.Resolve(componentType, _builder);

			info.AddComponent(this);
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			if (_builder == null)
				throw new ArgumentException("No object builder interface is available");

			Type componentType = typeof (TComponent);

			SubscriptionTypeInfo info = SubscriptionTypeInfo.Resolve(componentType, _builder);

			info.RemoveComponent(this);
		}

		public bool Active
		{
			get { return true; }
		}
		
		public void Dispose()
		{
		}

//		private IMessageDispatcher GetCorrelatedDispatcher(Type[] genericArguments)
//		{
//			lock (_correlatedLock)
//			{
//				if (_correlatedDispatchers.ContainsKey(genericArguments[1]))
//					return _correlatedDispatchers[genericArguments[1]];
//
//				Type dispatcherType = typeof (CorrelationIdDispatcher<,>).MakeGenericType(genericArguments);
//
//				IMessageDispatcher dispatcher = (IMessageDispatcher) Activator.CreateInstance(dispatcherType, _bus, _cache, _builder);
//
//				if (!_messageTypeToKeyType.ContainsKey(genericArguments[0]))
//					_messageTypeToKeyType.Add(genericArguments[0], genericArguments[1]);
//
//				_correlatedDispatchers.Add(genericArguments[1], dispatcher);
//
//				return dispatcher;
//			}
//		}

		public IMessageDispatcher<T> GetMessageProducer<T>() where T : class
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

				return (IMessageDispatcher<T>) consumer;
			}
		}
	}
}
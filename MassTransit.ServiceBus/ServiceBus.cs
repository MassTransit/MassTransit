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
namespace MassTransit.ServiceBus
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Internal;
	using log4net;
	using Subscriptions;
	using Threading;
	using Util;

	/// <summary>
	/// A service bus is used to attach message handlers (services) to endpoints, as well as 
	/// communicate with other service bus instances in a distributed application
	/// </summary>
	public class ServiceBus :
		IServiceBus
	{
		private static readonly ILog _log;

		private readonly ManagedThread _receiveThread;
		private readonly ManagedThreadPool<object> _asyncDispatcher;
		private readonly EndpointResolver _endpointResolver = new EndpointResolver();
		private readonly IEndpoint _endpointToListenOn;
		private readonly IMessageDispatcher _messageDispatcher;
		private readonly ISubscriptionCache _subscriptionCache;
		private IEndpoint _poisonEndpoint;

		static ServiceBus()
		{
			try
			{
				_log = LogManager.GetLogger(typeof (ServiceBus));
			}
			catch (Exception ex)
			{
				throw new Exception("log4net isn't referenced", ex);
			}
		}

		/// <summary>
		/// Uses an in-memory subscription manager and the default object builder
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn) : this(endpointToListenOn, new LocalSubscriptionCache())
		{
		}

		/// <summary>
		/// Uses the default object builder
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn, ISubscriptionCache subscriptionCache) : this(endpointToListenOn, subscriptionCache, new ActivatorObjectBuilder())
		{
		}

		/// <summary>
		/// Uses the specified subscription cache
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn, ISubscriptionCache subscriptionCache, IObjectBuilder objectBuilder)
		{
			Check.Parameter(endpointToListenOn).WithMessage("endpointToListenOn").IsNotNull();
			Check.Parameter(subscriptionCache).WithMessage("subscriptionCache").IsNotNull();

			_endpointToListenOn = endpointToListenOn;
			_subscriptionCache = subscriptionCache;

			_messageDispatcher = new MessageDispatcher(this, subscriptionCache, objectBuilder);

			_receiveThread = new ReceiveThread(this, endpointToListenOn);

			_asyncDispatcher = new ManagedThreadPool<object>(
				delegate(object message) { _messageDispatcher.Consume(message); });
		}

		public ISubscriptionCache SubscriptionCache
		{
			get { return _subscriptionCache; }
		}

		private static readonly Type _correlatedBy = typeof (CorrelatedBy<>);

		public void Dispose()
		{
			_receiveThread.Dispose();
			_asyncDispatcher.Dispose();
			_subscriptionCache.Dispose();
			_messageDispatcher.Dispose();
			_endpointToListenOn.Dispose();
		}

		/// <summary>
		/// Publishes a message to all subscribed consumers for the message type
		/// </summary>
		/// <typeparam name="T">The type of the message</typeparam>
		/// <param name="message">The messages to be published</param>
		public void Publish<T>(T message) where T : class
		{
			IList<Subscription> subs = null;

			Type messageType = typeof (T);
			foreach (Type interfaceType in messageType.GetInterfaces())
			{
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == _correlatedBy)
				{
					Type[] arguments = interfaceType.GetGenericArguments();

					Type invokeType = _correlatedBy.MakeGenericType(arguments);

					object value = invokeType.InvokeMember("CorrelationId", BindingFlags.GetProperty, null, message, null);

					string correlationId = value.ToString();

					subs = _subscriptionCache.List(messageType.FullName, correlationId);
					break;
				}
			}

			if (subs == null)
				subs = _subscriptionCache.List(messageType.FullName);

			if (subs.Count == 0)
				_log.WarnFormat("There are now subscriptions for the message type {0} for the bus listening on {1}", messageType.FullName, _endpointToListenOn.Uri);

			foreach (Subscription subscription in subs)
			{
				IEndpoint endpoint = _endpointResolver.Resolve(subscription.EndpointUri);
				endpoint.Send(message);
			}
		}

		/// <summary>
		/// The endpoint associated with this instance
		/// </summary>
		public IEndpoint Endpoint
		{
			get { return _endpointToListenOn; }
		}

		/// <summary>
		/// The poison endpoint associated with this instance where exception messages are sent
		/// </summary>
		public IEndpoint PoisonEndpoint
		{
			get { return _poisonEndpoint; }
			set { _poisonEndpoint = value; }
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public void Subscribe<T>(Action<IMessageContext<T>> callback) where T : class
		{
			Subscribe(callback, null);
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		/// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
		public void Subscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class
		{
			Subscribe(new GenericComponent<T>(callback, condition, this));
		}

		public void Subscribe<T>(T component) where T : class
		{
			_messageDispatcher.Subscribe(component);
			StartListening();
		}

		public void Unsubscribe<T>(Action<IMessageContext<T>> callback) where T : class
		{
			Unsubscribe(callback, null);
		}

		public void Unsubscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class
		{
			Unsubscribe(new GenericComponent<T>(callback, condition, this));
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			_messageDispatcher.Unsubscribe(component);
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			_messageDispatcher.AddComponent<TComponent>();
			StartListening();
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			_messageDispatcher.RemoveComponent<TComponent>();
		}

		private void StartListening()
		{
			_receiveThread.Start();
		}

		public void Dispatch(object message)
		{
			Dispatch(message, DispatchMode.Synchronous);
		}

		public void Dispatch(object message, DispatchMode mode)
		{
			if (mode == DispatchMode.Synchronous)
				_messageDispatcher.Consume(message);
			else
			{
				_asyncDispatcher.Enqueue(message);
			}
		}

		public bool Accept(object obj)
		{
			return _messageDispatcher.Accept(obj);
		}
	}
}
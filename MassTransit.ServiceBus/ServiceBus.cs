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
		IServiceBus,
		IEnvelopeConsumer
	{
		private static readonly ILog _log;

		private readonly AsyncReplyDispatcher _asyncReplyDispatcher = new AsyncReplyDispatcher();
		private readonly IMessageDispatcher _dispatcher;

		private readonly EndpointResolver _endpointResolver = new EndpointResolver();
		private readonly IEndpoint _endpointToListenOn;
		private readonly ISubscriptionCache _subscriptionCache;
		private readonly ManagedThreadPool<object> _asyncDispatcher;
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
		/// Uses an in-memory subscription manager
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn) : this(endpointToListenOn, new LocalSubscriptionCache())
		{

		}

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

			_dispatcher = new MessageDispatcher(this, subscriptionCache, objectBuilder);

			_asyncDispatcher = new ManagedThreadPool<object>(
				delegate(object message) { _dispatcher.Consume(message); });
		}

		public ISubscriptionCache SubscriptionCache
		{
			get { return _subscriptionCache; }
		}

		#region IEnvelopeConsumer Members

		/// <summary>
		/// Called when a message is available from the endpoint. If the consumer returns true, the message
		/// will be removed from the endpoint and delivered to the consumer
		/// </summary>
		/// <param name="envelope">The message envelope available</param>
		/// <returns>True is the consumer will handle the message, false if it should be ignored</returns>
		public bool IsInterested(IEnvelope envelope)
		{
			bool result;
			try
			{
				lock (_asyncReplyDispatcher)
				{
					result = _asyncReplyDispatcher.Exists(envelope.CorrelationId);
				}

				if (result == false)
				{
					foreach (IMessage message in envelope.Messages)
					{
						result = _dispatcher.Accept(message);
						if (result)
							break;
					}
				}
			}
			catch (Exception ex)
			{
				if (_log.IsErrorEnabled)
					_log.Error("Exception in ServiceBus.IsHandled: ", ex);

				throw;
			}

			return result;
		}

		/// <summary>
		/// Delivers the message envelope to the consumer
		/// </summary>
		/// <param name="envelope">The message envelope</param>
		public void Deliver(IEnvelope envelope)
		{
			if (_log.IsDebugEnabled)
				_log.DebugFormat("Envelope {0} Received By {1}", envelope.Id, GetHashCode());

			bool delivered;
			lock (_asyncReplyDispatcher)
			{
				delivered = _asyncReplyDispatcher.Complete(envelope);
			}

			if (delivered == false)
			{
				foreach (IMessage message in envelope.Messages)
				{
					Dispatch(message);
				}
			}
		}

		#endregion

		#region IServiceBus Members

		public void Dispose()
		{
			_subscriptionCache.Dispose();

			_asyncDispatcher.Dispose();

			_dispatcher.Dispose();

			_endpointToListenOn.Dispose();
		}

		/// <summary>
		/// Publishes a message to all subscribed consumers for the message type
		/// </summary>
		/// <typeparam name="T">The type of the message</typeparam>
		/// <param name="messages">The messages to be published</param>
		public void Publish<T>(params T[] messages) where T : class, IMessage
		{
			IList<Subscription> subs = _subscriptionCache.List(typeof (T).FullName);

			if (subs.Count == 0)
				_log.WarnFormat("There are now subscriptions for the message type {0} for the bus listening on {1}", typeof (T).FullName, _endpointToListenOn.Uri);

			foreach (Subscription subscription in subs)
			{
				IEndpoint endpoint = _endpointResolver.Resolve(subscription.EndpointUri);
				Send(endpoint, messages);
			}
		}

		/// <summary>
		/// Sends a list of messages to the specified destination
		/// </summary>
		/// <param name="destinationEndpoint">The destination for the message</param>
		/// <param name="messages">The list of messages</param>
		public void Send<T>(IEndpoint destinationEndpoint, params T[] messages) where T : class, IMessage
		{
			foreach (T msg in messages)
			{
				IEnvelope envelope = new Envelope(_endpointToListenOn, msg);

				destinationEndpoint.Sender.Send(envelope);
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
		public void Subscribe<T>(Action<IMessageContext<T>> callback) where T : class, IMessage
		{
			Subscribe(callback, null);
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		/// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
		public void Subscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class, IMessage
		{
			Subscribe(new GenericComponent<T>(callback, condition, this));
		}

		public void Subscribe<T>(T component) where T : class
		{
			_dispatcher.Subscribe(component);
//            _subscriptionCache.Add(new Subscription(typeof (T).FullName, Endpoint.Uri));

			StartListening();
		}

		public void Unsubscribe<T>(Action<IMessageContext<T>> callback) where T : class, IMessage
		{
			Unsubscribe(callback, null);
		}

		public void Unsubscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class, IMessage
		{
			Unsubscribe(new GenericComponent<T>(callback, condition, this));
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			_dispatcher.Unsubscribe(component);
//		    _subscriptionCache.Remove(new Subscription(typeof (T).FullName, Endpoint.Uri));
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			_dispatcher.AddComponent<TComponent>();
			//TODO: subscription client
			StartListening();
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			//TODO: subscription client
			_dispatcher.RemoveComponent<TComponent>();
		}

		/// <summary>
		/// Submits a request message to the default destination for the message type
		/// </summary>
		/// <typeparam name="T">The type of message</typeparam>
		/// <param name="destinationEndpoint">The destination for the message</param>
		/// <param name="messages">The messages to be sent</param>
		/// <returns>An IAsyncResult that can be used to wait for the response</returns>
		public IServiceBusAsyncResult Request<T>(IEndpoint destinationEndpoint, params T[] messages) where T : class, IMessage
		{
			return Request<T>(destinationEndpoint, null, null, messages);
		}

		public IServiceBusAsyncResult Request<T>(IEndpoint destinationEndpoint, AsyncCallback callback, object state,
		                                         params T[] messages) where T : class, IMessage
		{
			StartListening();

			IEnvelope envelope = new Envelope(_endpointToListenOn, messages);

			lock (_asyncReplyDispatcher)
			{
				destinationEndpoint.Sender.Send(envelope);

				return _asyncReplyDispatcher.Track(envelope.Id, callback, state);
			}
		}

		#endregion

		private void StartListening()
		{
			_endpointToListenOn.Receiver.Subscribe(this);
		}

		public void Dispatch(object message)
		{
			Dispatch(message, DispatchMode.Synchronous);
		}

		public void Dispatch(object message, DispatchMode mode)
		{
			if (mode == DispatchMode.Synchronous)
				_dispatcher.Consume(message);
			else
			{
				_asyncDispatcher.Enqueue(message);
			}
		}
	}

	/// <summary>
	/// The method used to dispatch the message to the service bus
	/// </summary>
	public enum DispatchMode
	{
		/// <summary>
		/// Dispatch the message in a synchronous fashion (default)
		/// </summary>
		Synchronous,

		/// <summary>
		/// Dipatch the message using an asynchronous handler
		/// </summary>
		Asynchronous,
	}
}
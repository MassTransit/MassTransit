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

		private readonly Dictionary<Type, IMessageConsumer> _consumers =
			new Dictionary<Type, IMessageConsumer>();

		private readonly object _consumersLock = new object();
		private readonly EndpointResolver _endpointResolver = new EndpointResolver();
		private readonly IEndpoint _endpointToListenOn;
		private IEndpoint _poisonEndpoint;
		private readonly ISubscriptionCache _subscriptionCache;
		private readonly IMessageDispatcher _dispatcher;

		static ServiceBus()
        {
            try
            {
                _log = LogManager.GetLogger(typeof (ServiceBus));
            }
            catch(Exception ex)
            {
                throw new Exception("log4net isn't referenced", ex);
            }
        }

        /// <summary>
        /// Uses an in-memory subscription manager
        /// </summary>
		public ServiceBus(IEndpoint endpointToListenOn) : this(endpointToListenOn, new LocalSubscriptionCache()) {  }

        /// <summary>
        /// Uses the specified subscription cache
        /// </summary>
        public ServiceBus(IEndpoint endpointToListenOn, ISubscriptionCache subscriptionCache)
        {
			Check.Parameter(endpointToListenOn).WithMessage("endpointToListenOn").IsNotNull();
            Check.Parameter(subscriptionCache).WithMessage("subscriptionCache").IsNotNull();

			_endpointToListenOn = endpointToListenOn;
            _subscriptionCache = subscriptionCache;

        	_dispatcher = new MessageDispatcher(new ActivatorObjectBuilder());
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
					result = IsTheBusInterested(envelope);
				}

				if(result == false)
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
                //TODO: How can we best combine these?
				DeliverMessagesToConsumers(envelope);

				foreach (IMessage message in envelope.Messages)
				{
					_dispatcher.Consume(message);
				}
			}
		}

		#endregion

		#region IServiceBus Members

		public void Dispose()
		{
			_subscriptionCache.Dispose();

			_consumers.Clear();

			_dispatcher.Dispose();

			_endpointToListenOn.Dispose();
		}

		/// <summary>
		/// Publishes a message to all subscribed consumers for the message type
		/// </summary>
		/// <typeparam name="T">The type of the message</typeparam>
		/// <param name="messages">The messages to be published</param>
		public void Publish<T>(params T[] messages) where T : IMessage
		{
			foreach (Subscription subscription in _subscriptionCache.List(typeof (T).FullName))
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
		public void Send<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage
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
		public void Subscribe<T>(Action<IMessageContext<T>> callback) where T : IMessage
		{
			Subscribe(callback, null);
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		/// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
		public void Subscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : IMessage
		{
			lock (_consumersLock)
			{
				if (!_consumers.ContainsKey(typeof (T)))
				{
					_consumers[typeof (T)] = new MessageConsumer<T>();
					_subscriptionCache.Add(new Subscription(typeof (T).FullName, Endpoint.Uri));
				}

				((IMessageConsumer<T>) _consumers[typeof (T)]).Subscribe(callback, condition);
			}

            //Subscribe(new GenericComponent<T>(callback, this));
			StartListening();
		}

		public void Subscribe<T>(T component) where T : class
		{
			_dispatcher.Subscribe(component);

			StartListening();
		}

		public void Unsubscribe<T>(Action<IMessageContext<T>> callback) where T : IMessage
		{
			Unsubscribe(callback, null);
		}

		public void Unsubscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : IMessage
		{
			lock (_consumersLock)
			{
				if (_consumers.ContainsKey(typeof (T)))
				{
					IMessageConsumer<T> consumer = ((IMessageConsumer<T>) _consumers[typeof (T)]);

					consumer.Unsubscribe(callback, condition);
					if (consumer.Count == 0)
					{
						_consumers.Remove(typeof (T));
						_subscriptionCache.Remove(new Subscription(typeof (T).FullName, Endpoint.Uri));
					}
				}
			}
		}

		public void Unsubscribe<T>(T component) where T : class
		{
			_dispatcher.Unsubscribe(component);
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			_dispatcher.AddComponent<TComponent>();

			StartListening();
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			_dispatcher.RemoveComponent<TComponent>();
		}

		/// <summary>
		/// Submits a request message to the default destination for the message type
		/// </summary>
		/// <typeparam name="T">The type of message</typeparam>
		/// <param name="destinationEndpoint">The destination for the message</param>
		/// <param name="messages">The messages to be sent</param>
		/// <returns>An IAsyncResult that can be used to wait for the response</returns>
		public IServiceBusAsyncResult Request<T>(IEndpoint destinationEndpoint, params T[] messages) where T : IMessage
		{
			return Request<T>(destinationEndpoint, null, null, messages);
		}

		public IServiceBusAsyncResult Request<T>(IEndpoint destinationEndpoint, AsyncCallback callback, object state,
		                                         params T[] messages) where T : IMessage
		{
			StartListening();

			IEnvelope envelope = new Envelope(_endpointToListenOn, messages as IMessage[]);

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

		private bool IsTheBusInterested(IEnvelope envelope)
		{
			bool result = false;

			foreach (IMessage message in envelope.Messages)
			{
				Type messageType = message.GetType();

				if (_consumers.ContainsKey(messageType))
				{
					IMessageConsumer receivingConsumer = _consumers[messageType];

					if (receivingConsumer.IsHandled(message))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		private void DeliverMessagesToConsumers(IEnvelope envelope)
		{
			foreach (IMessage message in envelope.Messages)
			{
				Type messageType = message.GetType();

				if (_log.IsDebugEnabled)
					_log.DebugFormat("Message Received: {0}", messageType);

				if (_consumers.ContainsKey(messageType))
				{
					try
					{
						_consumers[messageType].Deliver(this, envelope, message);
					}
					catch (Exception ex)
					{
						if (_log.IsErrorEnabled)
							_log.Error("Exception from Deliver: ", ex);
					}
				}
			}
		}
	}
}
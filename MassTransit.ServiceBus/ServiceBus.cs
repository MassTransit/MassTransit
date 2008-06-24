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
		IServiceBus
	{
		private static readonly ILog _log;
	    private static readonly ILog _ironLog;

		private readonly ManagedThreadPool<object> _asyncDispatcher;
		private readonly IEndpointResolver _endpointResolver;
		private readonly IEndpoint _endpointToListenOn;
		private readonly MessageTypeDispatcher _messageDispatcher;
		private readonly IObjectBuilder _objectBuilder;
		private readonly ManagedThread _receiveThread;
		private readonly ISubscriptionCache _subscriptionCache;
		private readonly SubscriptionCoordinator _subscriptionCoordinator;
		private IEndpoint _poisonEndpoint;

		static ServiceBus()
		{
			try
			{
				_log = LogManager.GetLogger(typeof (ServiceBus));
			    _ironLog = LogManager.GetLogger("MassTransit.Iron");
			}
			catch (Exception ex)
			{
				throw new Exception("log4net isn't referenced", ex);
			}
		}

		/// <summary>
		/// Uses an in-memory subscription manager and the default object builder
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder) : this(endpointToListenOn, objectBuilder, new LocalSubscriptionCache())
		{
		}

		public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder, ISubscriptionCache subscriptionCache)
			: this(endpointToListenOn, objectBuilder, subscriptionCache, new EndpointResolver())
		{
		}

		/// <summary>
		/// Uses the specified subscription cache
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder, ISubscriptionCache subscriptionCache, IEndpointResolver endpointResolver)
		{
			Check.Parameter(endpointToListenOn).WithMessage("endpointToListenOn").IsNotNull();
			Check.Parameter(subscriptionCache).WithMessage("subscriptionCache").IsNotNull();

			_endpointToListenOn = endpointToListenOn;
			_subscriptionCache = subscriptionCache;
			_objectBuilder = objectBuilder;
			_endpointResolver = endpointResolver;

			//TODO: Move into IObjectBuilder?
			_messageDispatcher = new MessageTypeDispatcher(this);
			_subscriptionCoordinator = new SubscriptionCoordinator(_messageDispatcher, this, _subscriptionCache, _objectBuilder);
			_receiveThread = new ReceiveThread(this, endpointToListenOn);
            _asyncDispatcher = new ManagedThreadPool<object>(IronDispatcher);
		}

		public ISubscriptionCache SubscriptionCache
		{
			get { return _subscriptionCache; }
		}

		public void Dispose()
		{
			_receiveThread.Dispose();
			_asyncDispatcher.Dispose();
			_subscriptionCoordinator.Dispose();
			_subscriptionCache.Dispose();
			_messageDispatcher.Dispose();
			_endpointToListenOn.Dispose();

			if (_poisonEndpoint != null)
				_poisonEndpoint.Dispose();
		}

		/// <summary>
		/// Publishes a message to all subscribed consumers for the message type
		/// </summary>
		/// <typeparam name="T">The type of the message</typeparam>
		/// <param name="message">The messages to be published</param>
		public void Publish<T>(T message) where T : class
		{
			IPublicationTypeInfo info = _subscriptionCoordinator.Resolve<T>();

			IList<Subscription> subs = info.GetConsumers(message);

			if (_log.IsWarnEnabled && subs.Count == 0)
				_log.WarnFormat("There are no subscriptions for the message type {0} for the bus listening on {1}", typeof (T).FullName, _endpointToListenOn.Uri);

			List<Uri> done = new List<Uri>();

			foreach (Subscription subscription in subs)
			{
				if (done.Contains(subscription.EndpointUri))
					continue;

				IEndpoint endpoint = _endpointResolver.Resolve(subscription.EndpointUri);
				endpoint.Send(message);

				done.Add(subscription.EndpointUri);
			}
		}

		public RequestBuilder Request()
		{
			return new RequestBuilder(this);
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

		public int MinThreadCount
		{
			get { return _asyncDispatcher.MinThreads; }
			set { _asyncDispatcher.MinThreads = value; }
		}

		public int MaxThreadCount
		{
			get { return _asyncDispatcher.MaxThreads; }
			set { _asyncDispatcher.MaxThreads = value; }
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
			ISubscriptionTypeInfo info = _subscriptionCoordinator.Resolve(component);
			info.Subscribe(component);

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
			ISubscriptionTypeInfo info = _subscriptionCoordinator.Resolve(component);
			info.Unsubscribe(component);
		}

		public void AddComponent<TComponent>() where TComponent : class
		{
			_objectBuilder.Register<TComponent>();

			ISubscriptionTypeInfo info = _subscriptionCoordinator.Resolve<TComponent>();
			info.AddComponent();

			StartListening();
		}

		public void RemoveComponent<TComponent>() where TComponent : class
		{
			ISubscriptionTypeInfo info = _subscriptionCoordinator.Resolve<TComponent>();
			info.RemoveComponent();
		}

		private void StartListening()
		{
            _log.DebugFormat("ServiceBus is listening on {0}", _endpointToListenOn.Uri);
			_receiveThread.Start();
		}

		public void Dispatch(object message)
		{
			Dispatch(message, DispatchMode.Synchronous);
		}

		public void Dispatch(object message, DispatchMode mode)
		{
			if (mode == DispatchMode.Synchronous)
				IronDispatcher(message);
			else
				_asyncDispatcher.Enqueue(message);
		}

		public bool Accept(object obj)
		{
			return _messageDispatcher.Accept(obj);
		}

        private void IronDispatcher(object message)
        {
            try
            {
                _messageDispatcher.Consume(message);    
            }
            catch(Exception ex)
            {
            	IPublicationTypeInfo info = _subscriptionCoordinator.Resolve(message.GetType());

            	info.PublishFault(this, ex, message);

                _ironLog.Error("An error was caught in the ServiceBus.IronDispatcher", ex);
            }
            
        }
	}
}
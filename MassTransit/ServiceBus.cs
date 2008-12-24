// Copyright 2007-2008 The Apache Software Foundation.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit
{
	using System;
	using System.Collections.Generic;
	using System.Transactions;
	using Configuration;
	using Exceptions;
	using Internal;
	using log4net;
	using Pipeline;
	using Pipeline.Configuration;
	using Subscriptions;
	using Threading;
	using Util;

	/// <summary>
	/// A service bus is used to attach message handlers (services) to endpoints, as well as 
	/// communicate with other service bus instances in a distributed application
	/// </summary>
	public partial class ServiceBus :
		IServiceBus
	{
		private static readonly ILog _log;
		private static readonly IServiceBus _nullServiceBus;

		private readonly DispatcherContext _dispatcherContext;
		private readonly IEndpointFactory _endpointFactory;
		private readonly IObjectBuilder _objectBuilder;
		private ResourceThreadPool<IEndpoint, object> _asyncDispatcher;
		private bool _disposed;
		private IEndpoint _endpointToListenOn;
		private MessagePipeline _inbound;
		private ISubscriptionEvent _inboundSubscriptionEvent;
		private MessagePipeline _outbound;
		private ISubscriptionCache _subscriptionCache;
		private ITypeInfoCache _typeInfoCache;
		private TimeSpan _receiveTimeout;

		static ServiceBus()
		{
			try
			{
				_log = LogManager.GetLogger(typeof (ServiceBus));

				_nullServiceBus = new NullServiceBus();
			}
			catch (Exception ex)
			{
				throw new ConfigurationException("log4net isn't referenced", ex);
			}
		}

		/// <summary>
		/// Uses an in-memory subscription manager and the default object builder
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder) : this(endpointToListenOn, objectBuilder, new LocalSubscriptionCache())
		{
		}

		public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder,
		                  ISubscriptionCache subscriptionCache)
			: this(endpointToListenOn, objectBuilder, subscriptionCache, EndpointFactoryConfigurator.New(x => x.SetObjectBuilder(objectBuilder)), new TypeInfoCache())
		{
		}

		/// <summary>
		/// Uses the specified subscription cache
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn,
		                  IObjectBuilder objectBuilder,
		                  ISubscriptionCache subscriptionCache,
		                  IEndpointFactory endpointFactory,
		                  ITypeInfoCache typeInfoCache)
		{
			Check.Parameter(endpointToListenOn).WithMessage("endpointToListenOn").IsNotNull();
			Check.Parameter(subscriptionCache).WithMessage("subscriptionCache").IsNotNull();

			_endpointToListenOn = endpointToListenOn;
			_subscriptionCache = subscriptionCache;
			_objectBuilder = objectBuilder;
			_endpointFactory = endpointFactory;

			_typeInfoCache = typeInfoCache;

			_inboundSubscriptionEvent = new SubscriptionCacheEventConnector(_subscriptionCache, _endpointToListenOn);

			_inbound = MessagePipelineConfigurator.CreateDefault(_objectBuilder, _inboundSubscriptionEvent);

			_outbound = MessagePipelineConfigurator.CreateDefault(_objectBuilder, null);

			_dispatcherContext = new DispatcherContext(_objectBuilder, this, _subscriptionCache);

			var resourceLimit = 1;
			var minThreads = 1;
			var maxThreads = Environment.ProcessorCount*8;

			_asyncDispatcher = new ResourceThreadPool<IEndpoint, object>(endpointToListenOn,
			                                                             EndpointReader,
			                                                             resourceLimit,
			                                                             minThreads,
			                                                             maxThreads);
            
            PoisonEndpoint = new PoisonEndpointDecorator(new NullEndpoint());
		}

		public ISubscriptionCache SubscriptionCache
		{
			get { return _subscriptionCache; }
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

		public int ReadThreadCount
		{
			get { return _asyncDispatcher.ResourceLimit; }
			set { _asyncDispatcher.ResourceLimit = value; }
		}

		public static IServiceBus Null
		{
			get { return _nullServiceBus; }
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Publishes a message to all subscribed consumers for the message type
		/// </summary>
		/// <typeparam name="T">The type of the message</typeparam>
		/// <param name="message">The messages to be published</param>
		public void Publish<T>(T message) where T : class
		{
			IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo<T>();

			IList<Subscription> subs = info.GetConsumers(_dispatcherContext, message);

			if (_log.IsWarnEnabled && subs.Count == 0)
				_log.WarnFormat("There are no subscriptions for the message type {0} for the bus listening on {1}", typeof (T).FullName, _endpointToListenOn.Uri);

			List<Uri> done = new List<Uri>();

			foreach (Subscription subscription in subs)
			{
				if (done.Contains(subscription.EndpointUri))
					continue;

				IEndpoint endpoint = _endpointFactory.GetEndpoint(subscription.EndpointUri);
				endpoint.Send(message, info.TimeToLive);

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
        public IEndpoint PoisonEndpoint { get; set; }

		public TimeSpan ReceiveTimeout
		{
			get { return _receiveTimeout; }
			set { _receiveTimeout = value; }
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public Func<bool> Subscribe<T>(Action<T> callback) where T : class
		{
			return Subscribe(callback, null);
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		/// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
		public Func<bool> Subscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
		{
			var result = _inbound.Subscribe(new GenericComponent<T>(callback, condition, this));

			_asyncDispatcher.WakeUp();
			return result;
		}

		public Func<bool> Subscribe<T>(T consumer) where T : class
		{
			var result = _inbound.Subscribe(consumer);

			_asyncDispatcher.WakeUp();

			return (result);
		}

		public void Unsubscribe<T>(Action<T> callback) where T : class
		{
			// TODO silentry return for now but this is going away
		}

		public void Unsubscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
		{
			// TODO silentry return for now but this is going away
		}

		public void Unsubscribe<T>(T consumer) where T : class
		{
			// TODO silentry return for now but this is going away
		}

		public Func<bool> Subscribe<TComponent>() where TComponent : class
		{
			Func<bool> result = _inbound.Subscribe<TComponent>();

			_asyncDispatcher.WakeUp();

			return (result);
		}

		public Func<bool> Subscribe(Type consumerType)
		{
			throw new NotSupportedException("This needs fixed");
		}

		public void Unsubscribe(Type consumerType)
		{
			// TODO silentry return for now but this is going away
		}

		public void Unsubscribe<TComponent>() where TComponent : class
		{
			// TODO silentry return for now but this is going away
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_asyncDispatcher.Dispose();
				_asyncDispatcher = null;

				_typeInfoCache.Dispose();
				_typeInfoCache = null;

				_subscriptionCache.Dispose();
				_subscriptionCache = null;

				_inbound.Dispose();
				_inbound = null;

				_outbound.Dispose();
				_outbound = null;

				_endpointToListenOn.Dispose();
				_endpointToListenOn = null;

				if (PoisonEndpoint != null)
				{
					PoisonEndpoint.Dispose();
					PoisonEndpoint = null;
				}
			}
			_disposed = true;
		}

		public void Dispatch(object message)
		{
            if (message == null)
                return;

            try
            {
                _inbound.Dispatch(message);
            }
            catch (Exception ex)
            {
                //retry
                SpecialLoggers.Iron.Error("An error was caught in the ServiceBus.IronDispatcher", ex);

                IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo(message.GetType());
                info.PublishFault(this, ex, message);


                PoisonEndpoint.Send(message, TimeSpan.Zero);
            }
		}

		private void EndpointReader(IEndpoint resource)
		{
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					bool released = false;
					try
					{
						resource.Receive(_receiveTimeout, (message, acceptor) =>
							{
								try
								{
									return _inbound.Dispatch(message, accepted =>
										{
											bool result = acceptor(message);

											_asyncDispatcher.ReleaseResource(1);
											released = true;

											return result;
										});
								}
								catch (Exception ex)
								{
									//retry
									SpecialLoggers.Iron.Error("An error was caught in the ServiceBus.IronDispatcher", ex);

									IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo(message.GetType());
									info.PublishFault(this, ex, message);

									PoisonEndpoint.Send(message, TimeSpan.Zero);
								}

								return false;
							});
					}
					finally
					{
						if (!released)
							_asyncDispatcher.ReleaseResource(1);
					}

					scope.Complete();
				}
			}
			catch (Exception ex)
			{
				_log.Error(string.Format("An exception occurred receiving a message from {0}", _endpointToListenOn.Uri), ex);
				throw;
			}
		}

		public void Start()
		{
			_asyncDispatcher.WakeUp();
		}
	}

	public class SubscriptionCacheEventConnector :
		ISubscriptionEvent
	{
		private readonly ISubscriptionCache _cache;
		private readonly IEndpoint _endpoint;

		public SubscriptionCacheEventConnector(ISubscriptionCache cache, IEndpoint endpoint)
		{
			_cache = cache;
			_endpoint = endpoint;
		}

		public Func<bool> SubscribedTo(Type messageType)
		{
			Subscription subscription = new Subscription(messageType, _endpoint.Uri);

			_cache.Add(subscription);

			return () =>
				{
					_cache.Remove(subscription);
					return true;
				};
		}

		public Func<bool> SubscribedTo(Type messageType, string correlationId)
		{
			Subscription subscription = new Subscription(messageType, correlationId, _endpoint.Uri);

			_cache.Add(subscription);

			return () =>
				{
					_cache.Remove(subscription);
					return true;
				};
		}

		public void UnsubscribedFrom(Type messageType)
		{
			Subscription subscription = new Subscription(messageType, _endpoint.Uri);

			_cache.Remove(subscription);
		}

		public void UnsubscribedFrom(Type messageType, string correlationId)
		{
			Subscription subscription = new Subscription(messageType, correlationId, _endpoint.Uri);

			_cache.Remove(subscription);
		}
	}
}
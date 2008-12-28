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
	using System.Runtime.Serialization;
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

		private readonly DispatcherContext _dispatcherContext;
		private readonly IEndpointFactory _endpointFactory;
		private readonly IObjectBuilder _objectBuilder;
		private bool _disposed;
		private IEndpoint _endpointToListenOn;
		private MessagePipeline _inbound;
		private ISubscriptionEvent _inboundSubscriptionEvent;
		private MessagePipeline _outbound;
		private TimeSpan _receiveTimeout = TimeSpan.FromSeconds(3);
		private ISubscriptionCache _subscriptionCache;
		private ITypeInfoCache _typeInfoCache;
		private volatile bool _started;
		private ResourceLock<IEndpoint> _readerLock;
		private DynamicThreadPool _threadPool;
        //threading config stuff?
        private TimeSpan _threadTimeout;
        private int _readThreadLimit = 1;
        private int _minThreads = 1;
		private int _maxThreads = Environment.ProcessorCount*4;
		

		static ServiceBus()
		{
			try
			{
				_log = LogManager.GetLogger(typeof (ServiceBus));

				Null = new NullServiceBus();
			}
			catch (Exception ex)
			{
				throw new ConfigurationException("log4net isn't referenced", ex);
			}
		}

		/// <summary>
		/// Uses an in-memory subscription manager and the default object builder
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder)
			: this(endpointToListenOn, objectBuilder, new LocalSubscriptionCache())
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

			_threadTimeout = TimeSpan.FromSeconds(60);

			_inboundSubscriptionEvent = new SubscriptionCacheEventConnector(_subscriptionCache, _endpointToListenOn);

			_inbound = MessagePipelineConfigurator.CreateDefault(_objectBuilder, _inboundSubscriptionEvent);

			_outbound = MessagePipelineConfigurator.CreateDefault(_objectBuilder, null);

			_dispatcherContext = new DispatcherContext(_objectBuilder, this, _subscriptionCache);

			PoisonEndpoint = new PoisonEndpointDecorator(new NullEndpoint());
		}

		public ISubscriptionCache SubscriptionCache
		{
			get { return _subscriptionCache; }
		}

		public int MinThreadCount
		{
			get { return _minThreads; }
			set
			{
				_minThreads = value;
				if (_threadPool != null)
					_threadPool.MinThreads = value;
			}
		}

		public int MaxThreadCount
		{
			get { return _maxThreads; }
			set
			{
				_maxThreads = value;
				if (_threadPool != null)
					_threadPool.MaxThreads = value;
			}
		}

		public int ReadThreadCount
		{
			get { return _readThreadLimit; }
			set
			{
				if (_threadPool != null)
					throw new ConfigurationException("The read thread count cannot be changed once the bus is in motion. Beep! Beep!");

				_readThreadLimit = value;
			}
		}

		public TimeSpan ReceiveTimeout
		{
			get { return _receiveTimeout; }
			set { _receiveTimeout = value; }
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

			return result;
		}

		public Func<bool> Subscribe<T>(T consumer) where T : class
		{
			var result = _inbound.Subscribe(consumer);

			return (result);
		}

		public Func<bool> Subscribe<TComponent>() where TComponent : class
		{
			Func<bool> result = _inbound.Subscribe<TComponent>();

			return (result);
		}

		public Func<bool> Subscribe(Type consumerType)
		{
			throw new NotSupportedException("This needs fixed");
		}

		[Obsolete]
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

		public void Start()
		{
			if (_started)
				return;

			_readerLock = new ResourceLock<IEndpoint>(_endpointToListenOn, _readThreadLimit);
			_threadPool = new DynamicThreadPool(ReceiveFromEndpoint, _minThreads, _maxThreads);
			_threadPool.Start();
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_readerLock.Shutdown();

				if (_threadPool != null)
				{
					_threadPool.Stop();
					_threadPool.Dispose();
					_threadPool = null;
				}

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

		private bool ReceiveFromEndpoint()
		{
			bool performedWork = false;

			try
			{
				using (var resourceLock = _readerLock.AcquireLock(ThreadTimeout))
				{
					foreach (IMessageSelector selector in resourceLock.Resource.SelectiveReceive(ReceiveTimeout))
					{
						performedWork = true;

						try
						{
							object message = selector.DeserializeMessage();
							if (message == null)
								continue;

							using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
							{
								bool atLeastOneConsumerFailed = DispatchMessageToConsumers(resourceLock, selector, message);
								if (atLeastOneConsumerFailed)
								{
									PoisonEndpoint.Send(message);
								}

								scope.Complete();
							}
						}
						catch (SerializationException sex)
						{
							selector.MoveMessageTo(PoisonEndpoint);

							throw new MessageException(typeof (object), "An error occurred deserializing a message", sex);
						}
						catch (Exception ex)
						{
							_log.Error(string.Format("An exception occurred receiving a message from '{0}'", _endpointToListenOn.Uri), ex);
						}
					}
				}
			}
			catch (ResourceLockException ex)
			{
				// this is not a big deal, just means we couldn't get the resource
				// which means we're probably tired and ready to exit
			}
			catch (Exception ex)
			{
				// this could be a bigger deal, but we'll probably just log it.
				_log.Error("ReceiveFromEndpoint got an exception", ex);
			}

			return performedWork;
		}

		private TimeSpan ThreadTimeout
		{
			get { return _threadTimeout; }
		}

		private bool DispatchMessageToConsumers(IResourceLockScope<IEndpoint> resourceLock, IMessageSelector selector, object message)
		{
			bool atLeastOneConsumerFailed = false;

			foreach (Consumes<object>.All consumer in _inbound.Enumerate(message))
			{
				try
				{
					if (!selector.AcceptMessage())
						break;

					resourceLock.Release();

					consumer.Consume(message);
				}
				catch (Exception ex)
				{
					_log.Error(string.Format("'{0}' threw an exception consuming message '{1}'",
						consumer.GetType().FullName, message.GetType().FullName), ex);

					atLeastOneConsumerFailed = true;

					IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo(message.GetType());
					info.PublishFault(this, ex, message);
				}
			}

			return atLeastOneConsumerFailed;
		}
	}
}
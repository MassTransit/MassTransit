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
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Transactions;
	using Exceptions;
	using Internal;
	using log4net;
	using Magnum.ObjectExtensions;
	using Magnum.Reflection;
	using Magnum.InterfaceExtensions;
	using Pipeline;
	using Pipeline.Configuration;
	using Threading;

	/// <summary>
	/// A service bus is used to attach message handlers (services) to endpoints, as well as 
	/// communicate with other service bus instances in a distributed application
	/// </summary>
	public class ServiceBus :
		IControlBus
	{
		private static readonly ILog _log;

		private readonly TimeSpan _threadTimeout = TimeSpan.FromSeconds(60);
		private int _concurrentReceiveThreads = 1;
		private bool _disposed;
		private ResourceLock<IEndpoint> _receiveThreadLock;
		private IServiceContainer _serviceContainer;
		private volatile bool _started;
		private DynamicThreadPool _threadPool;
		private UnregisterAction _unsubscribeEventDispatchers = () => true;

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
		/// Creates an instance of the ServiceBus, which implements IServiceBus. This is normally
		/// not called and should be created using the ServiceBusConfigurator to ensure proper defaults 
		/// and operation.
		/// </summary>
		public ServiceBus(IEndpoint endpointToListenOn,
		                  IObjectBuilder objectBuilder,
		                  IEndpointFactory endpointFactory)
		{

			ReceiveTimeout = TimeSpan.FromSeconds(3);
			endpointToListenOn.MustNotBeNull("endpointToListenOn", "This parameter cannot be null");
			objectBuilder.MustNotBeNull("objectBuilder", "This parameter cannot be null");
			endpointFactory.MustNotBeNull("endpointFactory", "This parameter cannot be null");

			Endpoint = endpointToListenOn;
			ObjectBuilder = objectBuilder;
			EndpointFactory = endpointFactory;

			_serviceContainer = new ServiceContainer(this);

			OutboundPipeline = MessagePipelineConfigurator.CreateDefault(ObjectBuilder);

			InboundPipeline = MessagePipelineConfigurator.CreateDefault(ObjectBuilder);
			InboundPipeline.Configure(x =>
				{
					_unsubscribeEventDispatchers += x.Register(new InboundOutboundSubscriptionBinder(OutboundPipeline, Endpoint));
				});

			_threadPool = new DynamicThreadPool(ReceiveFromEndpoint, 1, Environment.ProcessorCount*4);

			PoisonEndpoint = new PoisonEndpointDecorator(new NullEndpoint());

			ControlBus = this;
		}

		public static IServiceBus Null { get; private set; }
		public IEndpointFactory EndpointFactory { get; private set; }
		public IObjectBuilder ObjectBuilder { get; private set; }
		public TimeSpan ReceiveTimeout { get; set; }

		public int MinimumConsumerThreads
		{
			get { return _threadPool.MinThreads; }
			set { _threadPool.MinThreads = value; }
		}

		public int MaximumConsumerThreads
		{
			get { return _threadPool.MaxThreads; }
			set { _threadPool.MaxThreads = value; }
		}

		public int ConcurrentReceiveThreads
		{
			get { return _concurrentReceiveThreads; }
			set
			{
				if (_started)
					throw new ConfigurationException("The read thread count cannot be changed once the bus is in motion. Beep! Beep!");

				_concurrentReceiveThreads = value;
			}
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
		public void Publish<T>(T message) 
			where T : class
		{
			var context = OutboundMessage.Headers;

			context.SetSourceAddress(Endpoint.Uri);
			context.SetMessageType(typeof (T));

			int publishedCount = 0;
			foreach (var consumer in OutboundPipeline.Enumerate(message))
			{
				try
				{
					consumer(message);
					publishedCount++;
				}
				catch (Exception ex)
				{
					_log.Error(string.Format("'{0}' threw an exception publishing message '{1}'",
						consumer.GetType().FullName, message.GetType().FullName), ex);
				}
			}

			context.Reset();

			if (_log.IsWarnEnabled && publishedCount == 0)
				_log.WarnFormat("There are no subscriptions for the message type {0} for the bus listening on {1}", typeof (T).FullName, Endpoint.Uri);
		}

		//		endpoint.Send(message, info.TimeToLive);

		public TService GetService<TService>()
		{
			return _serviceContainer.GetService<TService>();
		}

		public IMessagePipeline OutboundPipeline { get; private set; }

		public IMessagePipeline InboundPipeline { get; private set; }

		/// <summary>
		/// The endpoint associated with this instance
		/// </summary>
		public IEndpoint Endpoint { get; private set; }

		/// <summary>
		/// The poison endpoint associated with this instance where exception messages are sent
		/// </summary>
		public IEndpoint PoisonEndpoint { get; set; }

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		public UnsubscribeAction Subscribe<T>(Action<T> callback) where T : class
		{
			return Subscribe(callback, null);
		}

		/// <summary>
		/// Adds a message handler to the service bus for handling a specific type of message
		/// </summary>
		/// <typeparam name="T">The message type to handle, often inferred from the callback specified</typeparam>
		/// <param name="callback">The callback to invoke when messages of the specified type arrive on the service bus</param>
		/// <param name="condition">A condition predicate to filter which messages are handled by the callback</param>
		public UnsubscribeAction Subscribe<T>(Action<T> callback, Predicate<T> condition) where T : class
		{
			var result = InboundPipeline.Subscribe(callback, condition);

			return result;
		}

		public UnsubscribeAction Subscribe<T>(T consumer) where T : class
		{
			var result = InboundPipeline.Subscribe(consumer);

			return (result);
		}

		public UnsubscribeAction Subscribe<TComponent>() where TComponent : class
		{
			UnsubscribeAction result = InboundPipeline.Subscribe<TComponent>();

			return (result);
		}

		public UnsubscribeAction Subscribe(Type consumerType)
		{
		   var method = typeof (ServiceBus).GetMethod("SometimesGenericsSuck", BindingFlags.NonPublic | BindingFlags.Instance);
		   var genericMethod = method.MakeGenericMethod(consumerType);
		   return (UnsubscribeAction) genericMethod.Invoke(this, null);
		}

		//Just here to support Subscribe(Type)
		private UnsubscribeAction SometimesGenericsSuck<TComponent>() where TComponent : class
		{
			return Subscribe<TComponent>();
		}

	    public void Start()
		{
			if (_started)
				return;

			_receiveThreadLock = new ResourceLock<IEndpoint>(Endpoint, ConcurrentReceiveThreads);
			_threadPool.Start();

			_serviceContainer.Start();

			_started = true;
		}

		public void AddService(Type serviceType, IBusService service)
		{
			_serviceContainer.AddService(serviceType, service);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;
			if (disposing)
			{
				_receiveThreadLock.Shutdown();

				if (_threadPool != null)
				{
					_threadPool.Stop();
					_threadPool.Dispose();
					_threadPool = null;
				}

				if (_serviceContainer != null)
				{
					_serviceContainer.Stop();
					_serviceContainer.Dispose();
					_serviceContainer = null;
				}

				if (ControlBus != this)
					ControlBus.Dispose();

				_unsubscribeEventDispatchers();

				InboundPipeline.Dispose();
				InboundPipeline = null;

				OutboundPipeline.Dispose();
				OutboundPipeline = null;

				Endpoint = null;

				if (PoisonEndpoint != null)
				{
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
				using (var resourceLock = _receiveThreadLock.AcquireLock(_threadTimeout))
				{
					foreach (IMessageSelector selector in resourceLock.Resource.SelectiveReceive(ReceiveTimeout))
					{
						performedWork = true;

						try
						{
							object message = selector.DeserializeMessage();
							if (message == null)
								continue;

							InboundMessageHeaders.SetCurrent(context =>
								{
									context.ReceivedOn(this);
									context.ReceivedAs(message);
								});

							//using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required))
							//{
								bool atLeastOneConsumerFailed = DispatchMessageToConsumers(resourceLock, selector, message);
								if (atLeastOneConsumerFailed)
								{
									MoveMessageToPoisonEndpoint(selector);
								}

							//	scope.Complete();
							//}
						}
						catch (SerializationException sex)
						{
							MoveMessageToPoisonEndpoint(selector);
							throw new MessageException(typeof (object), "An error occurred deserializing a message", sex);
						}
						catch(SagaException sax)
						{
							MoveMessageToPoisonEndpoint(selector);
							throw new MessageException(typeof(object), "There was a problem with a saga message", sax);
						}
						catch (Exception ex)
						{
							_log.Error(string.Format("An exception occurred receiving a message from '{0}'", Endpoint.Uri), ex);
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

		private void MoveMessageToPoisonEndpoint(IMessageSelector selector)
		{
			selector.AcceptMessage();

			try
			{
				selector.MoveMessageTo(PoisonEndpoint);
			}
			catch (Exception ex)
			{
				_log.Error("Failed to move message to poison endpoint", ex);
			}
		}

		private bool DispatchMessageToConsumers(IResourceLockScope<IEndpoint> resourceLock, IMessageSelector selector, object message)
		{
			bool atLeastOneConsumerFailed = false;

			foreach (var consumer in InboundPipeline.Enumerate(message))
			{
				try
				{
					if (!selector.AcceptMessage())
						break;

					resourceLock.Release();

					consumer(message);
				}
				catch (Exception ex)
				{
					_log.Error(string.Format("'{0}' threw an exception consuming message '{1}'",
						consumer.GetType().FullName, message.GetType().FullName), ex);

					atLeastOneConsumerFailed = true;

					CreateAndPublishFault(message, ex);
				}
			}

			return atLeastOneConsumerFailed;
		}

		private void CreateAndPublishFault(object message, Exception ex)
		{
			if (message.Implements(typeof (CorrelatedBy<>)))
				this.Call("PublishFault", ClassFactory.New(typeof (Fault<,>), message, ex));
			else
				this.Call("PublishFault", ClassFactory.New(typeof (Fault<>), message, ex));
		}

		private void PublishFault<T>(T message) where T : class 
		{
			CurrentMessage.GenerateFault(message);
		}

		public IServiceBus ControlBus { get; set; }
	}
}
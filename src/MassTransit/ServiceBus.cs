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
	using System.Reflection;
	using Exceptions;
	using Internal;
	using log4net;
	using Magnum.DateTimeExtensions;
	using Magnum.InterfaceExtensions;
	using Magnum.ObjectExtensions;
	using Magnum.Pipeline;
	using Magnum.Pipeline.Segments;
	using Magnum.Reflection;
	using Pipeline;
	using Pipeline.Configuration;

	/// <summary>
	/// A service bus is used to attach message handlers (services) to endpoints, as well as 
	/// communicate with other service bus instances in a distributed application
	/// </summary>
	public class ServiceBus :
		IControlBus
	{
		private static readonly ILog _log;

		private readonly TimeSpan _threadTimeout = 10.Seconds();
		private int _consumerThreadLimit = Environment.ProcessorCount*4;
		private volatile bool _disposed;
		private int _receiveThreadLimit = 1;
		private TimeSpan _receiveTimeout = 3.Seconds();
		private IServiceContainer _serviceContainer;
		private volatile bool _started;
		private UnregisterAction _unsubscribeEventDispatchers = () => true;
		private Pipe _eventAggregator;
		private ConsumerPool _consumerPool;

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

			_eventAggregator = PipeSegment.New();

			_serviceContainer = new ServiceContainer(this);

			OutboundPipeline = MessagePipelineConfigurator.CreateDefault(ObjectBuilder);

			InboundPipeline = MessagePipelineConfigurator.CreateDefault(ObjectBuilder);
			InboundPipeline.Configure(x => { _unsubscribeEventDispatchers += x.Register(new InboundOutboundSubscriptionBinder(OutboundPipeline, Endpoint)); });

			PoisonEndpoint = new PoisonEndpointDecorator(new NullEndpoint());

			ControlBus = this;
		}

		public static IServiceBus Null { get; private set; }
		public IEndpointFactory EndpointFactory { get; private set; }
		public IObjectBuilder ObjectBuilder { get; private set; }

		public TimeSpan ReceiveTimeout
		{
			get { return _receiveTimeout; }
			set
			{
				if (_started)
					throw new ConfigurationException("The receive timeout cannot be changed once the bus is in motion. Beep! Beep!");

				_receiveTimeout = value;
			}
		}

		public int MaximumConsumerThreads
		{
			get { return _consumerThreadLimit; }
			set
			{
				if (_started)
					throw new ConfigurationException("The consumer thread limit cannot be changed once the bus is in motion. Beep! Beep!");

				_consumerThreadLimit = value;
			}
		}

		public int ConcurrentReceiveThreads
		{
			get { return _receiveThreadLimit; }
			set
			{
				if (_started)
					throw new ConfigurationException("The receive thread limit cannot be changed once the bus is in motion. Beep! Beep!");

				_receiveThreadLimit = value;
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
			IOutboundMessageContext context = OutboundMessage.Context;

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

			if (publishedCount == 0)
			{
				context.NotifyNoSubscribers(message);
			}

			context.Reset();
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
			UnsubscribeAction result = InboundPipeline.Subscribe(callback, condition);

			return result;
		}

		public UnsubscribeAction Subscribe<T>(T consumer) where T : class
		{
			UnsubscribeAction result = InboundPipeline.Subscribe(consumer);

			return (result);
		}

		public UnsubscribeAction Subscribe<TComponent>() where TComponent : class
		{
			UnsubscribeAction result = InboundPipeline.Subscribe<TComponent>();

			return (result);
		}

		public UnsubscribeAction Subscribe(Type consumerType)
		{
			// TODO this.Call("Subscribe", new[] {consumerType}, new[] {});

			MethodInfo method = typeof (ServiceBus).GetMethod("SometimesGenericsSuck", BindingFlags.NonPublic | BindingFlags.Instance);
			MethodInfo genericMethod = method.MakeGenericMethod(consumerType);
			return (UnsubscribeAction) genericMethod.Invoke(this, null);
		}

		public IServiceBus ControlBus { get; set; }

		//Just here to support Subscribe(Type)

		public void Start()
		{
			if (_started)
				return;

			_consumerPool = new ThreadPoolConsumerPool(this, ObjectBuilder, _eventAggregator, _receiveTimeout)
				{
					MaximumConsumerCount = MaximumConsumerThreads,
				};
			_consumerPool.Start();

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
				if (_consumerPool != null)
				{
					_consumerPool.Stop();
					_consumerPool.Dispose();
					_consumerPool = null;
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

				_eventAggregator = null;

				Endpoint = null;

				if (PoisonEndpoint != null)
				{
					PoisonEndpoint = null;
				}
			}
			_disposed = true;
		}

		private UnsubscribeAction SometimesGenericsSuck<TComponent>() where TComponent : class
		{
			return Subscribe<TComponent>();
		}

		private void ReceiveFromEndpoint()
		{
			try
			{
				Endpoint.Receive(message =>
					{
						InboundMessageHeaders.SetCurrent(context =>
							{
								context.ReceivedOn(this);
								context.SetObjectBuilder(ObjectBuilder);
								context.ReceivedAs(message);
							});

						IEnumerable<Action<object>> consumers = InboundPipeline.Enumerate(message);

						IEnumerator<Action<object>> consumerEnumerator = consumers.GetEnumerator();
						if (!consumerEnumerator.MoveNext())
						{
							consumerEnumerator.Dispose();
							return null;
						}

						return msg =>
							{
								try
								{
									bool atLeastOneConsumerFailed = false;

									Exception lastException = null;

									do
									{
										try
										{
											consumerEnumerator.Current(msg);
										}
										catch (Exception ex)
										{
											_log.Error(string.Format("'{0}' threw an exception consuming message '{1}'",
												consumerEnumerator.Current.GetType().FullName,
												msg.GetType().FullName), ex);

											atLeastOneConsumerFailed = true;
											lastException = ex;

											CreateAndPublishFault(msg, ex);
										}
									} while (consumerEnumerator.MoveNext());

									if (atLeastOneConsumerFailed)
									{
										throw new MessageException(msg.GetType(),
											"At least one consumer threw an exception",
											lastException);
									}
								}
								finally
								{
									consumerEnumerator.Dispose();
								}
							};
					}, ReceiveTimeout);
			}
			catch (ObjectDisposedException ex)
			{
				_log.Error("The endpoint has been disposed", ex);
				throw;
			}
			catch (Exception ex)
			{
				_log.Error(string.Format("An exception occurred receiving a message from '{0}'", Endpoint.Uri), ex);
			}
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
	}
}
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
	using System.Diagnostics;
	using System.Reflection;
	using Events;
	using Exceptions;
	using Internal;
	using log4net;
	using Magnum.DateTimeExtensions;
	using Magnum.ObjectExtensions;
	using Magnum.Pipeline;
	using Magnum.Pipeline.Segments;
	using Monitoring;
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
		private ServiceBusInstancePerformanceCounters _counters;
		private ISubscriptionScope _eventAggregatorScope;

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
			_eventAggregatorScope = _eventAggregator.NewSubscriptionScope();

			_serviceContainer = new ServiceContainer(this);

			OutboundPipeline = MessagePipelineConfigurator.CreateDefault(ObjectBuilder, this);

			InboundPipeline = MessagePipelineConfigurator.CreateDefault(ObjectBuilder, this);
			InboundPipeline.Configure(x => { _unsubscribeEventDispatchers += x.Register(new InboundOutboundSubscriptionBinder(OutboundPipeline, Endpoint)); });

			PoisonEndpoint = new PoisonEndpointDecorator(new NullEndpoint());

			ControlBus = this;

			InitializePerformanceCounters();
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
			Stopwatch publishDuration = Stopwatch.StartNew();

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

			publishDuration.Stop();

			if (publishedCount == 0)
			{
				context.NotifyNoSubscribers(message);
			}

			_eventAggregator.Send(new MessagePublished
				{
					MessageType = typeof (T),
					ConsumerCount = publishedCount,
					Duration = publishDuration.Elapsed,
				});

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

		public UnsubscribeAction SubscribeConsumer<T>(Func<T,Action<T>> getConsumerAction)
			where T : class
		{
			return InboundPipeline.Subscribe<T>(getConsumerAction);
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

				if (_eventAggregatorScope != null)
				{
					_eventAggregatorScope.Dispose();
					_eventAggregatorScope = null;
				}

				_eventAggregator = null;

				Endpoint = null;

				if (_counters != null)
				{
					_counters.Dispose();
					_counters = null;
				}

				if (PoisonEndpoint != null)
				{
					PoisonEndpoint = null;
				}
			}
			_disposed = true;
		}

		// ReSharper disable UnusedMember.Local
		private UnsubscribeAction SometimesGenericsSuck<TComponent>() where TComponent : class
			// ReSharper restore UnusedMember.Local
		{
			return Subscribe<TComponent>();
		}

		private void InitializePerformanceCounters()
		{
			try
			{
				var instanceName = string.Format("{0}_{1}_{2}", Endpoint.Address.Path, Endpoint.Uri.Scheme, Endpoint.Uri.Host);

				_counters = new ServiceBusInstancePerformanceCounters(instanceName);

				_eventAggregatorScope.Subscribe<MessageReceived>(message =>
					{
						_counters.ReceiveCount.Increment();
						_counters.ReceiveRate.Increment();
						_counters.ReceiveDuration.IncrementBy((long) message.ReceiveDuration.TotalMilliseconds);
						_counters.ReceiveDurationBase.Increment();
						_counters.ConsumerDuration.IncrementBy((long) message.ConsumeDuration.TotalMilliseconds);
						_counters.ConsumerDurationBase.Increment();
					});

				_eventAggregatorScope.Subscribe<MessagePublished>(message =>
					{
						_counters.PublishCount.Increment();
						_counters.PublishRate.Increment();
						_counters.PublishDuration.IncrementBy((long) message.Duration.TotalMilliseconds);
						_counters.PublishDurationBase.Increment();

						_counters.SentCount.IncrementBy(message.ConsumerCount);
						_counters.SendRate.IncrementBy(message.ConsumerCount);
					});

				_eventAggregatorScope.Subscribe<ThreadPoolEvent>(message =>
					{
						_counters.ReceiveThreadCount.Set(message.ReceiverCount);
						_counters.ConsumerThreadCount.Set(message.ConsumerCount);
					});
			}
			catch (Exception ex)
			{
				_log.Warn("The performance counters could not be created", ex);
			}
		}
	}
}
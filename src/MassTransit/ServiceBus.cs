// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
	using Events;
	using Exceptions;
	using log4net;
	using Magnum;
	using Magnum.Extensions;
	using Magnum.Pipeline;
	using Magnum.Pipeline.Segments;
	using MessageHeaders;
	using Monitoring;
	using Pipeline;
	using Pipeline.Configuration;
	using Util;

	/// <summary>
	/// A service bus is used to attach message handlers (services) to endpoints, as well as 
	/// communicate with other service bus instances in a distributed application
	/// </summary>
	[DebuggerDisplay("{DebugDisplay}")]
	public class ServiceBus :
		IControlBus
	{
		static readonly ILog _log;

		ConsumerPool _consumerPool;
		int _consumerThreadLimit = Environment.ProcessorCount*4;
		ServiceBusInstancePerformanceCounters _counters;
		volatile bool _disposed;
		Pipe _eventAggregator;
		ISubscriptionScope _eventAggregatorScope;
		int _receiveThreadLimit = 1;
		TimeSpan _receiveTimeout = 3.Seconds();
		IServiceContainer _serviceContainer;
		volatile bool _started;
		UnregisterAction _unsubscribeEventDispatchers = () => true;

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
		                  IEndpointCache endpointCache)
		{
			ReceiveTimeout = TimeSpan.FromSeconds(3);
			Guard.AgainstNull(endpointToListenOn, "endpointToListenOn", "This parameter cannot be null");
			Guard.AgainstNull(endpointCache, "endpointFactory", "This parameter cannot be null");

			Endpoint = endpointToListenOn;
			EndpointCache = endpointCache;

			_eventAggregator = PipeSegment.New();
			_eventAggregatorScope = _eventAggregator.NewSubscriptionScope();

			_serviceContainer = new ServiceContainer(this);

			OutboundPipeline = MessagePipelineConfigurator.CreateDefault(this);

			InboundPipeline = MessagePipelineConfigurator.CreateDefault(this);
			InboundPipeline.Configure(
				x => { _unsubscribeEventDispatchers += x.Register(new InboundOutboundSubscriptionBinder(OutboundPipeline, Endpoint)); });

			ControlBus = this;

			InitializePerformanceCounters();
		}

		public static IServiceBus Null { get; private set; }

		[UsedImplicitly]
		protected string DebugDisplay
		{
			get { return string.Format("{0}: ", Endpoint.Address); }
		}

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
					throw new ConfigurationException(
						"The consumer thread limit cannot be changed once the bus is in motion. Beep! Beep!");

				_consumerThreadLimit = value;
			}
		}

		public int ConcurrentReceiveThreads
		{
			get { return _receiveThreadLimit; }
			set
			{
				if (_started)
					throw new ConfigurationException(
						"The receive thread limit cannot be changed once the bus is in motion. Beep! Beep!");

				_receiveThreadLimit = value;
			}
		}

		public IEndpointCache EndpointCache { get; private set; }

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

		public UnsubscribeAction Configure(Func<IPipelineConfigurator, UnsubscribeAction> configure)
		{
			return InboundPipeline.Configure(configure);
		}

		public IServiceBus ControlBus { get; set; }

		public IEndpoint GetEndpoint(Uri address)
		{
			return EndpointCache.GetEndpoint(address);
		}

		public void Start()
		{
			if (_started)
				return;

			_consumerPool = new ThreadPoolConsumerPool(this, _eventAggregator, _receiveTimeout)
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
			}
			_disposed = true;
		}

		void InitializePerformanceCounters()
		{
			try
			{
				string instanceName = string.Format("{0}_{1}_{2}", Endpoint.Address.Path, Endpoint.Uri.Scheme, Endpoint.Uri.Host);

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
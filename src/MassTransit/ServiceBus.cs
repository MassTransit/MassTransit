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
    using Context;
    using Diagnostics;
    using Diagnostics.Introspection;
    using Events;
    using Exceptions;
    using log4net;
    using Magnum;
    using Magnum.Extensions;
    using Monitoring;
    using Pipeline;
    using Pipeline.Configuration;
    using Stact;
    using Threading;
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
        UntypedChannel _eventChannel;
        ChannelConnection _performanceCounterConnection;
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

            _eventChannel = new ChannelAdapter();

            _serviceContainer = new ServiceContainer(this);

            OutboundPipeline = new OutboundPipelineConfigurator(this).Pipeline;
            InboundPipeline = InboundPipelineConfigurator.CreateDefault(this);

            ControlBus = this;

            InitializePerformanceCounters();
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

        [UsedImplicitly]
        protected string DebugDisplay
        {
            get { return string.Format("{0}: ", Endpoint.Address); }
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
        /// <param name="contextCallback">The callback to perform operations on the context</param>
        public void Publish<T>(T message, Action<IPublishContext<T>> contextCallback)
            where T : class
        {
            var context = ContextStorage.CreatePublishContext(message);
            context.SetSourceAddress(Endpoint.Address.Uri);

            contextCallback(context);

            int publishedCount = 0;
            foreach (var consumer in OutboundPipeline.Enumerate(context))
            {
                try
                {
                    consumer(context);
                    publishedCount++;
                }
                catch (Exception ex)
                {
                    _log.Error(string.Format("'{0}' threw an exception publishing message '{1}'",
                        consumer.GetType().FullName, message.GetType().FullName), ex);
                }
            }

            context.Complete();

            if (publishedCount == 0)
            {
                context.NotifyNoSubscribers();
            }

            _eventChannel.Send(new MessagePublished
                {
                    MessageType = typeof (T),
                    ConsumerCount = publishedCount,
                    Duration = context.Duration,
                });
        }

        public IOutboundMessagePipeline OutboundPipeline { get; private set; }

        public IInboundMessagePipeline InboundPipeline { get; private set; }

        /// <summary>
        /// The endpoint associated with this instance
        /// </summary>
        public IEndpoint Endpoint { get; private set; }

        public UnsubscribeAction Configure(Func<IInboundPipelineConfigurator, UnsubscribeAction> configure)
        {
            return InboundPipeline.Configure(configure);
        }

        public IServiceBus ControlBus { get; set; }

        public UntypedChannel EventChannel
        {
            get
            {
                return _eventChannel;
            }
        }

        public IEndpoint GetEndpoint(Uri address)
        {
            return EndpointCache.GetEndpoint(address);
        }

        public void Start()
        {
            if (_started)
                return;

            try
            {
                _consumerPool = new ThreadPoolConsumerPool(this, _eventChannel, _receiveTimeout)
                    {
                        MaximumConsumerCount = MaximumConsumerThreads,
                    };
                _consumerPool.Start();

                _serviceContainer.Start();
            }
            catch (Exception)
            {
                if(_consumerPool != null)
                    _consumerPool.Dispose();

                throw;
            }

            _started = true;
        }

        public void AddService(BusServiceLayer layer, IBusService service)
        {
            _serviceContainer.AddService(layer, service);
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

                if (_performanceCounterConnection != null)
                {
                    _performanceCounterConnection.Dispose();
                    _performanceCounterConnection = null;
                }

                _eventChannel = null;

                Endpoint = null;

                if (_counters != null)
                {
                    _counters.Dispose();
                    _counters = null;
                }

                EndpointCache.Dispose();
            }
            _disposed = true;
        }

        void InitializePerformanceCounters()
        {
            try
            {
                string instanceName = string.Format("{0}", Endpoint.Address.Uri);

                _counters = new ServiceBusInstancePerformanceCounters(instanceName);

                _performanceCounterConnection = _eventChannel.Connect(x =>
                    {
                        x.AddConsumerOf<MessageReceived>()
                            .UsingConsumer(message =>
                                {
                                    _counters.ReceiveCount.Increment();
                                    _counters.ReceiveRate.Increment();
                                    _counters.ReceiveDuration.IncrementBy((long) message.ReceiveDuration.TotalMilliseconds);
                                    _counters.ReceiveDurationBase.Increment();
                                    _counters.ConsumerDuration.IncrementBy((long) message.ConsumeDuration.TotalMilliseconds);
                                    _counters.ConsumerDurationBase.Increment();
                                });

                        x.AddConsumerOf<MessagePublished>()
                            .UsingConsumer(message =>
                                {
                                    _counters.PublishCount.Increment();
                                    _counters.PublishRate.Increment();
                                    _counters.PublishDuration.IncrementBy((long) message.Duration.TotalMilliseconds);
                                    _counters.PublishDurationBase.Increment();

                                    _counters.SentCount.IncrementBy(message.ConsumerCount);
                                    _counters.SendRate.IncrementBy(message.ConsumerCount);
                                });

                        x.AddConsumerOf<ThreadPoolEvent>()
                            .UsingConsumer(message =>
                                {
                                    _counters.ReceiveThreadCount.Set(message.ReceiverCount);
                                    _counters.ConsumerThreadCount.Set(message.ConsumerCount);
                                });
                    });
            }
            catch (Exception ex)
            {
                _log.Warn("The performance counters could not be created, try running the program in the Administrator role. Just once.", ex);
            }
        }

        public void Diagnose(DiagnosticsProbe probe)
        {
            DiagnosticsInfo.WriteCommonItems(probe);

            //network key
            //probe.Add("mt.network_key", builder.Settings.Network);

            probe.Add("mt.receive_from", Endpoint.Address);
            probe.Add("mt.control_bus", ControlBus.Endpoint.Address);
            probe.Add("mt.max_consumer_threads", MaximumConsumerThreads);
            probe.Add("mt.concurrent_receive_threads", ConcurrentReceiveThreads);
            probe.Add("mt._receive_timeout", ReceiveTimeout);
            
            EndpointCache.Diagnose(probe);
            //serializer(s)
            //transport(s)
            _serviceContainer.Diagnose(probe);

            OutboundPipeline.View(pipe => probe.Add("zz.mt.outbound_pipeline", pipe));
            InboundPipeline.View(pipe => probe.Add("zz.mt.inbound_pipeline", pipe));
        }
    }
}

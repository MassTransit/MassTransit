// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using Monitoring;
    using Pipeline;
    using Pipeline.Pipes;


    /// <summary>
    /// A service bus is used to attach message handlers (services) to endpoints, as well as
    /// communicate with other service bus instances in a distributed application
    /// </summary>
    [DebuggerDisplay("{DebugDisplay}")]
    public class ServiceBus
    {
        static readonly ILog _log;

        ServiceBusInstancePerformanceCounters _counters;
        volatile bool _disposed;
        volatile bool _started;

        static ServiceBus()
        {
            try
            {
                _log = Logger.Get(typeof(ServiceBus));
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
            bool enablePerformanceCounters)
        {

            Endpoint = endpointToListenOn;
  //          EndpointCache = endpointCache;

            ConsumePipe = new ConsumePipe();

            if (enablePerformanceCounters)
                InitializePerformanceCounters();
        }

        public TimeSpan ShutdownTimeout { get; set; }

        protected string DebugDisplay
        {
            get { return string.Format("{0}: ", Endpoint.Address); }
        }

//        public IEndpointCache EndpointCache { get; private set; }

        /// <summary>
        /// Publishes a message to all subscribed consumers for the message type
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The messages to be published</param>
        /// <param name="contextCallback">The callback to perform operations on the context</param>
//        public void Publish<T>(T message, Action<IPublishContext<T>> contextCallback)
//            where T : class
//        {
//            if (message == null)
//                throw new ArgumentNullException("message");
//            if (contextCallback == null)
//                throw new ArgumentNullException("contextCallback");
//
//            Context.PublishContext<T> context = ContextStorage.CreatePublishContext(message);
//            context.SetSourceAddress(Endpoint.Address.Uri);
//
//            contextCallback(context);
//
//            IList<Exception> exceptions = new List<Exception>();
//
//            int publishedCount = 0;
//            foreach (var consumer in OutboundPipeline.Enumerate(context))
//            {
//                try
//                {
//                    consumer(context);
//                    publishedCount++;
//                }
//                catch (Exception ex)
//                {
//                    _log.Error(string.Format("'{0}' threw an exception publishing message '{1}'",
//                        consumer.GetType().FullName, message.GetType().FullName), ex);
//
//                    exceptions.Add(ex);
//                }
//            }
//
//            context.Complete();
//
//            if (publishedCount == 0)
//            {
//                context.NotifyNoSubscribers();
//            }
//
//            if (exceptions.Count > 0)
//                throw new PublishException(typeof(T), exceptions);
//        }

        public IConsumePipe ConsumePipe { get; private set; }

        /// <summary>
        /// The endpoint associated with this instance
        /// </summary>
        public IEndpoint Endpoint { get; private set; }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Start()
        {
            if (_started)
                return;


            // TODO start endpoints

            _started = true;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                Endpoint = null;

                if (_counters != null)
                {
                    _counters.Dispose();
                    _counters = null;
                }

//                EndpointCache.Dispose();
            }
            _disposed = true;
        }

        void InitializePerformanceCounters()
        {
            try
            {
                string instanceName = string.Format("{0}_{1}{2}",
                    Endpoint.Address.Scheme, Endpoint.Address.Host, Endpoint.Address.AbsolutePath.Replace("/", "_"));

                _counters = new ServiceBusInstancePerformanceCounters(instanceName);

//                _performanceCounterConnection = _eventChannel.Connect(x =>
//                    {
//                        x.AddConsumerOf<MessageReceived>()
//                            .UsingConsumer(message =>
//                                {
//                                    _counters.ReceiveCount.Increment();
//                                    _counters.ReceiveRate.Increment();
//                                    _counters.ReceiveDuration.IncrementBy(
//                                        (long)message.ReceiveDuration.TotalMilliseconds);
//                                    _counters.ReceiveDurationBase.Increment();
//                                    _counters.ConsumerDuration.IncrementBy(
//                                        (long)message.ConsumeDuration.TotalMilliseconds);
//                                    _counters.ConsumerDurationBase.Increment();
//                                });
//
//                        x.AddConsumerOf<MessagePublished>()
//                            .UsingConsumer(message =>
//                                {
//                                    _counters.PublishCount.Increment();
//                                    _counters.PublishRate.Increment();
//                                    _counters.PublishDuration.IncrementBy((long)message.Duration.TotalMilliseconds);
//                                    _counters.PublishDurationBase.Increment();
//
//                                    _counters.SentCount.IncrementBy(message.ConsumerCount);
//                                    _counters.SendRate.IncrementBy(message.ConsumerCount);
//                                });
//
//                        x.AddConsumerOf<ThreadPoolEvent>()
//                            .UsingConsumer(message =>
//                                {
//                                    _counters.ReceiveThreadCount.Set(message.ReceiverCount);
//                                    _counters.ConsumerThreadCount.Set(message.ConsumerCount);
//                                });
//                    });
            }
            catch (Exception ex)
            {
                _log.Warn(
                    "The performance counters could not be created, try running the program in the Administrator role. Just once.",
                    ex);
            }
        }
    }
}
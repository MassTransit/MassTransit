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
        private static readonly IServiceBus _nullServiceBus;

        private readonly ResourceThreadPool<IEndpoint, object> _asyncDispatcher;
        private readonly DispatcherContext _dispatcherContext;
        private readonly IEndpointResolver _endpointResolver;
        private readonly IEndpoint _endpointToListenOn;
        private readonly MessageTypeDispatcher _messageDispatcher;
        private readonly IObjectBuilder _objectBuilder;
        private readonly ISubscriptionCache _subscriptionCache;
        private readonly TypeInfoCache _typeInfoCache;
        private IEndpoint _poisonEndpoint = new PoisonEndpointDecorator(new NullEndpoint());

        static ServiceBus()
        {
            try
            {
                _log = LogManager.GetLogger(typeof (ServiceBus));

                _nullServiceBus = new NullServiceBus();
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

        public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder,
            ISubscriptionCache subscriptionCache)
            : this(endpointToListenOn, objectBuilder, subscriptionCache, new EndpointResolver())
        {
        }

        /// <summary>
        /// Uses the specified subscription cache
        /// </summary>
        public ServiceBus(IEndpoint endpointToListenOn, IObjectBuilder objectBuilder, 
            ISubscriptionCache subscriptionCache, IEndpointResolver endpointResolver)
        {
            Check.Parameter(endpointToListenOn).WithMessage("endpointToListenOn").IsNotNull();
            Check.Parameter(subscriptionCache).WithMessage("subscriptionCache").IsNotNull();

            _endpointToListenOn = endpointToListenOn;
            _subscriptionCache = subscriptionCache;
            _objectBuilder = objectBuilder;
            _endpointResolver = endpointResolver;

            //TODO: Move into IObjectBuilder?
            _messageDispatcher = new MessageTypeDispatcher();
            _typeInfoCache = new TypeInfoCache();

            _dispatcherContext = new DispatcherContext(_objectBuilder, this, _messageDispatcher, _subscriptionCache, _typeInfoCache);

            _asyncDispatcher = new ResourceThreadPool<IEndpoint, object>(endpointToListenOn, 
                EndpointReader,
				EndpointDispatcher, 
                Environment.ProcessorCount * 2, 
                1, 
                Environment.ProcessorCount * 8);
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

        public static IServiceBus Null
        {
            get { return _nullServiceBus; }
        }

        public void Dispose()
        {
            _asyncDispatcher.Dispose();
            _typeInfoCache.Dispose();
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
            IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo<T>();

            IList<Subscription> subs = info.GetConsumers(_dispatcherContext, message);

            if (_log.IsWarnEnabled && subs.Count == 0)
                _log.WarnFormat("There are no subscriptions for the message type {0} for the bus listening on {1}", typeof (T).FullName, _endpointToListenOn.Uri);

            List<Uri> done = new List<Uri>();

            foreach (Subscription subscription in subs)
            {
                if (done.Contains(subscription.EndpointUri))
                    continue;

                IEndpoint endpoint = _endpointResolver.Resolve(subscription.EndpointUri);
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
            ISubscriptionTypeInfo info = _typeInfoCache.GetSubscriptionTypeInfo<T>();
            info.Subscribe(_dispatcherContext, component);

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
            ISubscriptionTypeInfo info = _typeInfoCache.GetSubscriptionTypeInfo<T>();
            info.Unsubscribe(_dispatcherContext, component);
        }

        public void AddComponent<TComponent>() where TComponent : class
        {
            _objectBuilder.Register<TComponent>();

            ISubscriptionTypeInfo info = _typeInfoCache.GetSubscriptionTypeInfo<TComponent>();
            info.AddComponent(_dispatcherContext);

            StartListening();
        }

        public void RemoveComponent<TComponent>() where TComponent : class
        {
            ISubscriptionTypeInfo info = _typeInfoCache.GetSubscriptionTypeInfo<TComponent>();
            info.RemoveComponent(_dispatcherContext);
        }

        private void StartListening()
        {
            // TODO NUKE
        }

        public void Dispatch(object message)
        {
            IronDispatcher(message);
        }

        public bool Accept(object obj)
        {
            return _messageDispatcher.Accept(obj);
        }

        private object EndpointReader(IEndpoint resource)
        {
			try
			{
				TimeSpan timeout = TimeSpan.FromSeconds(3);

				object message = resource.Receive(timeout, Accept);

				return message;
			}
			catch(Exception ex)
			{
				_log.Error(string.Format("An exception occurred receiving a message from {0}", _endpointToListenOn.Uri), ex);
				throw;
			}
        }

		private void EndpointDispatcher(object message)
		{
			if (message == null)
				return;

			try
			{
				_messageDispatcher.Consume(message);
			}
			catch (Exception ex)
			{
				IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo(message.GetType());

				info.PublishFault(this, ex, message);

				PoisonEndpoint.Send(message, TimeSpan.Zero);

				SpecialLoggers.Iron.Error("An error was caught in the ServiceBus.IronDispatcher", ex);

				throw;
			}
		}


        private void IronDispatcher(object message)
        {
            if (message == null)
                return;

            try
            {
                _messageDispatcher.Consume(message);
            }
            catch (Exception ex)
            {
                IPublicationTypeInfo info = _typeInfoCache.GetPublicationTypeInfo(message.GetType());

                info.PublishFault(this, ex, message);

                PoisonEndpoint.Send(message, TimeSpan.Zero);

                SpecialLoggers.Iron.Error("An error was caught in the ServiceBus.IronDispatcher", ex);
            }
        }

        public static ServiceBusBuilder Build()
        {
            return new ServiceBusBuilder();
        }

        private class NullServiceBus : IServiceBus
        {
            public void Dispose()
            {
            }

            public IEndpoint Endpoint
            {
                get { return null; }
            }

            public IEndpoint PoisonEndpoint
            {
                get { return null; }
            }

            public void Subscribe<T>(Action<IMessageContext<T>> callback) where T : class
            {
            }

            public void Subscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class
            {
            }

            public void Subscribe<T>(T component) where T : class
            {
            }

            public void Unsubscribe<T>(Action<IMessageContext<T>> callback) where T : class
            {
            }

            public void Unsubscribe<T>(Action<IMessageContext<T>> callback, Predicate<T> condition) where T : class
            {
            }

            public void Unsubscribe<T>(T component) where T : class
            {
            }

            public void AddComponent<TComponent>() where TComponent : class
            {
            }

            public void RemoveComponent<TComponent>() where TComponent : class
            {
            }

            public void Publish<T>(T message) where T : class
            {
            }

            public RequestBuilder Request()
            {
                throw new NotImplementedException();
            }
        }
    }
}
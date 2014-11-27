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
    using Configuration;
    using EndpointConfigurators;
    using Internals.Extensions;
    using Logging;
    using Magnum.Extensions;
    using Policies;
    using SubscriptionConfigurators;
    using SubscriptionConnectors;
    using Util;


    public static class ConsumerSubscriptionExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(ConsumerSubscriptionExtensions));

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(this SubscriptionBusServiceConfigurator configurator,
            IConsumerFactory<TConsumer> consumerFactory, IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(consumerFactory, retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator;
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator,
            IConsumerFactory<TConsumer> consumerFactory, IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(consumerFactory, retryPolicy ?? Retry.None);

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }

        /// <summary>
        /// Subscribes a consumer with a default constructor to the endpoint
        /// </summary>
        /// <typeparam name="TConsumer">The consumer type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator,
            IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer,new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var consumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(consumerFactory, retryPolicy ?? Retry.None);

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }

        public static ConnectHandle SubscribeConsumer<TConsumer>(this IBus bus, IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class, IConsumer
        {
            return ConsumerConnectorCache<TConsumer>.Connector.Connect(bus.ConsumePipe, consumerFactory, Retry.None);
        }


        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(this SubscriptionBusServiceConfigurator configurator,
            IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using default constructor)", TypeMetadataCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory,
                retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator;
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> Consumer<TConsumer>(this SubscriptionBusServiceConfigurator configurator,
            Func<TConsumer> consumerFactory, IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

            var consumerConfigurator = new ConsumerSubscriptionConfiguratorImpl<TConsumer>(delegateConsumerFactory,
                retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator;
        }

        public static ConsumerSubscriptionConfigurator Consumer(this SubscriptionBusServiceConfigurator configurator, Type consumerType,
            Func<Type, object> consumerFactory, IRetryPolicy retryPolicy = null)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType.GetTypeName());

            var consumerConfigurator = (SubscriptionBuilderConfigurator)Activator.CreateInstance(
                typeof(UntypedConsumerSubscriptionConfigurator<>).MakeGenericType(consumerType), consumerFactory, retryPolicy ?? Retry.None);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(consumerConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return consumerConfigurator as ConsumerSubscriptionConfigurator;
        }

        public static ConnectHandle SubscribeConsumer<TConsumer>(this IServiceBus bus, IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using default constructor)", TypeMetadataCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            ConsumerConnector connector = ConsumerConnectorCache<TConsumer>.Connector;

            return connector.Connect(bus.ConsumePipe, delegateConsumerFactory, retryPolicy ?? Retry.None);
        }

        public static ConnectHandle SubscribeConsumer<TConsumer>(this IServiceBus bus, Func<TConsumer> consumerFactory,
            IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", typeof(TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

            ConsumerConnector connector = ConsumerConnectorCache<TConsumer>.Connector;

            return connector.Connect(bus.ConsumePipe, delegateConsumerFactory, retryPolicy ?? Retry.None);
        }

        public static ConnectHandle SubscribeConsumer<TConsumer>(this IServiceBus bus, IConsumerFactory<TConsumer> consumerFactory,
            IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", typeof(TConsumer));

            ConsumerConnector connector = ConsumerConnectorCache<TConsumer>.Connector;

            return connector.Connect(bus.ConsumePipe, consumerFactory, retryPolicy ?? Retry.None);
        }

        public static ConnectHandle SubscribeConsumer(this IServiceBus bus, Type consumerType, Func<Type, object> objectFactory,
            IRetryPolicy retryPolicy = null)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType);

            return ConsumerConnectorCache.Connect(bus.ConsumePipe, consumerType, objectFactory, retryPolicy);
        }
    }
}
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
    using Logging;
    using SubscriptionConfigurators;
    using SubscriptionConnectors;


    public static class InterceptingConsumerSubscriptionExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(ConsumerSubscriptionExtensions));

        public static ConsumerSubscriptionConfigurator<TConsumer> InterceptingConsumer<TConsumer>(
            this SubscriptionBusServiceConfigurator configurator,
            IConsumerFactory<TConsumer> consumerFactory, ConsumerFactoryInterceptor<TConsumer> interceptor)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Subscribing Intercepting Consumer: {0} (using supplied consumer factory)",
                    typeof(TConsumer));
            }

            var interceptingConsumerFactory = new InterceptingConsumerFactory<TConsumer>(consumerFactory, interceptor);

            return configurator.Consumer(interceptingConsumerFactory);
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> InterceptingConsumer<TConsumer>(
            this SubscriptionBusServiceConfigurator configurator, ConsumerFactoryInterceptor<TConsumer> interceptor)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
            {
                _log.DebugFormat("Subscribing Intercepting Consumer: {0} (using default consumer factory)",
                    typeof(TConsumer));
            }

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            var interceptingConsumerFactory = new InterceptingConsumerFactory<TConsumer>(delegateConsumerFactory,
                interceptor);

            return configurator.Consumer(interceptingConsumerFactory);
        }

        public static ConsumerSubscriptionConfigurator<TConsumer> InterceptingConsumer<TConsumer>(
            this SubscriptionBusServiceConfigurator configurator, Func<TConsumer> consumerFactory,
            ConsumerFactoryInterceptor<TConsumer> interceptor)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", typeof(TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

            var interceptingConsumerFactory = new InterceptingConsumerFactory<TConsumer>(delegateConsumerFactory,
                interceptor);

            return configurator.Consumer(interceptingConsumerFactory);
        }

        public static UnsubscribeAction SubscribeInterceptingConsumer<TConsumer>(this IServiceBus bus,
            ConsumerFactoryInterceptor<TConsumer> interceptor)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using default consumer factory)", typeof(TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            var interceptingConsumerFactory = new InterceptingConsumerFactory<TConsumer>(delegateConsumerFactory,
                interceptor);

            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TConsumer>();

            throw new NotImplementedException();
        }

        public static UnsubscribeAction SubscribeInterceptingConsumer<TConsumer>(this IServiceBus bus,
            Func<TConsumer> consumerFactory, ConsumerFactoryInterceptor<TConsumer> interceptor)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", typeof(TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

            var interceptingConsumerFactory = new InterceptingConsumerFactory<TConsumer>(delegateConsumerFactory,
                interceptor);
            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TConsumer>();

            throw new NotImplementedException();
        }

        public static UnsubscribeAction SubscribeInterceptingConsumer<TConsumer>(this IServiceBus bus,
            IConsumerFactory<TConsumer> consumerFactory, ConsumerFactoryInterceptor<TConsumer> interceptor)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", typeof(TConsumer));

            var interceptingConsumerFactory = new InterceptingConsumerFactory<TConsumer>(consumerFactory,
                interceptor);
            ConsumerConnector connector = ConsumerConnectorCache.GetConsumerConnector<TConsumer>();

            throw new NotImplementedException();
        }
    }
}
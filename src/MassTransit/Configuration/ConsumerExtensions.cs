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
    using EndpointConfigurators;
    using Internals.Extensions;
    using Logging;
    using Pipeline;
    using Policies;
    using SubscriptionConfigurators;
    using Util;


    public static class ConsumerExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(ConsumerExtensions));

        /// <summary>
        /// Connect a consumer to the receiving endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactory"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static IConsumerConfigurator<TConsumer> Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator,
            IConsumerFactory<TConsumer> consumerFactory, IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, retryPolicy ?? Retry.None);

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }

        /// <summary>
        /// Connect a consumer to the bus instance's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="bus"></param>
        /// <param name="consumerFactory"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IBus bus, IConsumerFactory<TConsumer> consumerFactory,
            IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (consumerFactory == null)
                throw new ArgumentNullException("consumerFactory");

            return bus.ConsumePipe.ConnectConsumer(consumerFactory, retryPolicy);
        }

        /// <summary>
        /// Subscribes a consumer with a default constructor to the endpoint
        /// </summary>
        /// <typeparam name="TConsumer">The consumer type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static IConsumerConfigurator<TConsumer> Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator,
            IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using default constructor)", TypeMetadataCache<TConsumer>.ShortName);

            var consumerFactory = new DefaultConstructorConsumerFactory<TConsumer>();

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, retryPolicy ?? Retry.None);

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }

        /// <summary>
        /// Subscribe a consumer with a default constructor to the bus's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="bus"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IBus bus, IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer, new()
        {
            if (bus == null)
                throw new ArgumentNullException("bus");

            return bus.ConsumePipe.ConnectConsumer<TConsumer>(retryPolicy);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static IConsumerConfigurator<TConsumer> Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator,
            Func<TConsumer> consumerFactoryMethod, IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactoryMethod);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(delegateConsumerFactory,
                retryPolicy ?? Retry.None);

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }

        /// <summary>
        /// Subscribe a consumer with a consumer factor method to the bus's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="bus"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IBus bus, Func<TConsumer> consumerFactoryMethod,
            IRetryPolicy retryPolicy = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", typeof(TConsumer));

            return bus.ConsumePipe.ConnectConsumer(consumerFactoryMethod, retryPolicy);
        }

        /// <summary>
        /// Connect a consumer with a consumer type and object factory method for the consumer (used by containers mostly)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="consumerType"></param>
        /// <param name="consumerFactory"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static IConsumerConfigurator Consumer(this IReceiveEndpointConfigurator configurator, Type consumerType,
            Func<Type, object> consumerFactory, IRetryPolicy retryPolicy = null)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType.GetTypeName());

            var consumerConfigurator = (IReceiveEndpointBuilderConfigurator)Activator.CreateInstance(
                typeof(UntypedConsumerConfigurator<>).MakeGenericType(consumerType), consumerFactory, retryPolicy ?? Retry.None);

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator as IConsumerConfigurator;
        }

        /// <summary>
        /// Connect a consumer with a consumer type and object factory method for the consumer
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="consumerType"></param>
        /// <param name="objectFactory"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer(this IBus bus, Type consumerType, Func<Type, object> objectFactory,
            IRetryPolicy retryPolicy = null)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (consumerType == null)
                throw new ArgumentNullException("consumerType");
            if (objectFactory == null)
                throw new ArgumentNullException("objectFactory");
            if (!consumerType.HasInterface<IConsumer>())
                throw new ArgumentException("The consumer type must implement an IConsumer interface");

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType);

            return bus.ConsumePipe.ConnectConsumer(consumerType, objectFactory, retryPolicy);
        }
    }
}
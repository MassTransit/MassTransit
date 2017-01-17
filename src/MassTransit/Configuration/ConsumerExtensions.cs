// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using ConsumeConfigurators;
    using ConsumeConnectors;
    using ConsumerSpecifications;
    using GreenPipes;
    using Internals.Extensions;
    using Logging;
    using Pipeline;
    using Pipeline.ConsumerFactories;
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
        /// <param name="configure">Optional, configure the consumer</param>
        /// <returns></returns>
        public static void Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator, IConsumerFactory<TConsumer> consumerFactory,
            Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using supplied consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            configure?.Invoke(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Connect a consumer to the bus instance's default endpoint
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connector"></param>
        /// <param name="consumerFactory"></param>
        /// <param name="pipeSpecifications"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<T>(this IConsumePipeConnector connector, IConsumerFactory<T> consumerFactory,
            params IPipeSpecification<ConsumerConsumeContext<T>>[] pipeSpecifications)
            where T : class, IConsumer
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (consumerFactory == null)
                throw new ArgumentNullException(nameof(consumerFactory));

            IConsumerSpecification<T> specification = ConsumerConnectorCache<T>.Connector.CreateConsumerSpecification<T>();
            foreach (IPipeSpecification<ConsumerConsumeContext<T>> pipeSpecification in pipeSpecifications)
            {
                specification.AddPipeSpecification(pipeSpecification);
            }
            return ConsumerConnectorCache<T>.Connector.ConnectConsumer(connector, consumerFactory, specification);
        }

        /// <summary>
        /// Subscribes a consumer with a default constructor to the endpoint
        /// </summary>
        /// <typeparam name="TConsumer">The consumer type</typeparam>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator, Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using default constructor)", TypeMetadataCache<TConsumer>.ShortName);

            var consumerFactory = new DefaultConstructorConsumerFactory<TConsumer>();

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            configure?.Invoke(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Subscribe a consumer with a default constructor to the bus's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="connector"></param>
        /// <param name="pipeSpecifications"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IConsumePipeConnector connector,
            params IPipeSpecification<ConsumerConsumeContext<TConsumer>>[] pipeSpecifications)
            where TConsumer : class, IConsumer, new()
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));

            return ConnectConsumer(connector, new DefaultConstructorConsumerFactory<TConsumer>(), pipeSpecifications);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer>(this IReceiveEndpointConfigurator configurator, Func<TConsumer> consumerFactoryMethod,
            Action<IConsumerConfigurator<TConsumer>> configure = null)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactoryMethod);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(delegateConsumerFactory, configurator);

            configure?.Invoke(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Subscribe a consumer with a consumer factor method to the bus's default endpoint
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <param name="connector"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <param name="pipeSpecifications"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer<TConsumer>(this IConsumePipeConnector connector, Func<TConsumer> consumerFactoryMethod,
            params IPipeSpecification<ConsumerConsumeContext<TConsumer>>[] pipeSpecifications)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", typeof(TConsumer));

            var consumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactoryMethod);

            return ConnectConsumer(connector, consumerFactory, pipeSpecifications);
        }

        /// <summary>
        /// Connect a consumer with a consumer type and object factory method for the consumer (used by containers mostly)
        /// </summary>
        /// <param name="configurator"></param>
        /// <param name="consumerType"></param>
        /// <param name="consumerFactory"></param>
        /// <returns></returns>
        public static void Consumer(this IReceiveEndpointConfigurator configurator, Type consumerType, Func<Type, object> consumerFactory)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType.GetTypeName());

            var consumerConfigurator = (IReceiveEndpointSpecification)Activator.CreateInstance(
                typeof(UntypedConsumerConfigurator<>).MakeGenericType(consumerType), consumerFactory);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        /// <summary>
        /// Connect a consumer with a consumer type and object factory method for the consumer
        /// </summary>
        /// <param name="connector"></param>
        /// <param name="consumerType"></param>
        /// <param name="objectFactory"></param>
        /// <returns></returns>
        public static ConnectHandle ConnectConsumer(this IConsumePipeConnector connector, Type consumerType, Func<Type, object> objectFactory)
        {
            if (connector == null)
                throw new ArgumentNullException(nameof(connector));
            if (consumerType == null)
                throw new ArgumentNullException(nameof(consumerType));
            if (objectFactory == null)
                throw new ArgumentNullException(nameof(objectFactory));
            if (!consumerType.HasInterface<IConsumer>())
                throw new ArgumentException("The consumer type must implement an IConsumer interface");

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (by type, using object consumer factory)", consumerType);

            return ConsumerConnectorCache.Connect(connector, consumerType, objectFactory);
        }
    }
}
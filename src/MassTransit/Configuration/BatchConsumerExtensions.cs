// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Logging;
    using Pipeline.ConsumerFactories;
    using Util;


    public static class BatchConsumerExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(BatchConsumerExtensions));

        /// <summary>
        /// Connect a batch consumer
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Batch<TMessage>(this IReceiveEndpointConfigurator configurator, Action<IBatchConfigurator<TMessage>> configure)
            where TMessage : class
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Configuring batch: {0}", TypeMetadataCache<TMessage>.ShortName);

            var batchConfigurator = new BatchConfigurator<TMessage>(configurator);

            configure?.Invoke(batchConfigurator);
        }

        /// <summary>
        /// Connect a consumer with a consumer factory method
        /// </summary>
        /// <typeparam name="TConsumer"></typeparam>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="consumerFactoryMethod"></param>
        /// <returns></returns>
        public static void Consumer<TConsumer, TMessage>(this IBatchConfigurator<TMessage> configurator, Func<TConsumer> consumerFactoryMethod)
            where TConsumer : class, IConsumer<Batch<TMessage>>
            where TMessage : class
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer: {0} (using delegate consumer factory)", TypeMetadataCache<TConsumer>.ShortName);

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactoryMethod);

            configurator.Consumer(delegateConsumerFactory);
        }
    }
}
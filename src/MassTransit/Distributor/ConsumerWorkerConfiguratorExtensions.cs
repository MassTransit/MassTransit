// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Distributor.Configuration;
    using Distributor.WorkerConfigurators;
    using Logging;
    using Magnum.Reflection;
    using Util;

    public static class ConsumerWorkerConfiguratorExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(ConsumerSubscriptionExtensions));

        public static WorkerConsumerConfigurator<TConsumer> Consumer<TConsumer>(
            [NotNull] this WorkerConfigurator configurator,
            [NotNull] IConsumerFactory<TConsumer> consumerFactory)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer Worker: {0} (using supplied consumer factory)", typeof(TConsumer));

            return CreateWorkerConsumerConfigurator(configurator, consumerFactory);
        }

        public static WorkerConsumerConfigurator<TConsumer> Consumer<TConsumer>(
            [NotNull] this WorkerConfigurator configurator)
            where TConsumer : class, IConsumer, new()
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer Worker: {0} (using default consumer factory)", typeof(TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(() => new TConsumer());

            return CreateWorkerConsumerConfigurator(configurator, delegateConsumerFactory);
        }

        public static WorkerConsumerConfigurator<TConsumer> Consumer<TConsumer>(
            [NotNull] this WorkerConfigurator configurator, [NotNull] Func<TConsumer> consumerFactory)
            where TConsumer : class, IConsumer
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer Worker: {0} (using delegate consumer factory)", typeof(TConsumer));

            var delegateConsumerFactory = new DelegateConsumerFactory<TConsumer>(consumerFactory);

            return CreateWorkerConsumerConfigurator(configurator, delegateConsumerFactory);
        }

        public static WorkerConsumerConfigurator Consumer(
            [NotNull] this WorkerConfigurator configurator,
            [NotNull] Type consumerType,
            [NotNull] Func<Type, object> consumerFactory)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer Worker: {0} (by type, using object consumer factory)",
                    consumerType);

            object consumerConfigurator =
                FastActivator.Create(typeof(UntypedWorkerConsumerConfigurator<>),
                    new[] {consumerType}, new object[] {consumerFactory});

            configurator.AddConfigurator((WorkerBuilderConfigurator)consumerConfigurator);

            return (WorkerConsumerConfigurator)consumerConfigurator;
        }

        static WorkerConsumerConfigurator<TConsumer> CreateWorkerConsumerConfigurator<TConsumer>(
            WorkerConfigurator configurator,
            IConsumerFactory<TConsumer> consumerFactory) where TConsumer : class, IConsumer
        {
            var consumerConfigurator = new WorkerConsumerConfiguratorImpl<TConsumer>(consumerFactory);

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }
    }
}
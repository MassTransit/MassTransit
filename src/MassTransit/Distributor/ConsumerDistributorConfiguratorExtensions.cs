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
    using Distributor;
    using Distributor.DistributorConfigurators;
    using Logging;
    using Magnum.Reflection;
    using Util;

    public static class ConsumerDistributorConfiguratorExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(ConsumerDistributorConfiguratorExtensions));

        public static ConsumerDistributorConfigurator<TConsumer> Consumer<TConsumer>(
            this DistributorBusServiceConfigurator configurator)
            where TConsumer : class
        {
            var consumerConfigurator = new ConsumerDistributorConfiguratorImpl<TConsumer>();

            configurator.AddConfigurator(consumerConfigurator);

            return consumerConfigurator;
        }

        public static ConsumerDistributorConfigurator Consumer(
            [NotNull] this DistributorBusServiceConfigurator configurator,
            [NotNull] Type consumerType,
            [NotNull] Func<Type, object> consumerFactory)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Consumer Worker: {0} (by type, using object consumer factory)",
                    consumerType);

            object consumerConfigurator =
                FastActivator.Create(typeof(UntypedConsumerDistributorConfigurator<>),
                    new[] { consumerType }, new object[] { consumerFactory });

            configurator.AddConfigurator((DistributorBuilderConfigurator)consumerConfigurator);

            return (ConsumerDistributorConfigurator)consumerConfigurator;
        }
    }
}
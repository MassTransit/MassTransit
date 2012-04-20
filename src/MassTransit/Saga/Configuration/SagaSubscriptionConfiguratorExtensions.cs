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
    using Logging;
    using Magnum;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using Saga.SubscriptionConnectors;
    using SubscriptionConfigurators;

    public static class SagaSubscriptionConfiguratorExtensions
    {
        static readonly ILog _log = Logger.Get(typeof (SagaSubscriptionConfiguratorExtensions));

        /// <summary>
        /// Configure a saga subscription
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="sagaRepository"></param>
        /// <returns></returns>
        public static SagaSubscriptionConfigurator<TSaga> Saga<TSaga>(
            this SubscriptionBusServiceConfigurator configurator, ISagaRepository<TSaga> sagaRepository)
            where TSaga : class, ISaga
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", typeof(TSaga));

            var sagaConfigurator = new SagaSubscriptionConfiguratorImpl<TSaga>(sagaRepository);

            var busServiceConfigurator = new SubscriptionBusServiceBuilderConfiguratorImpl(sagaConfigurator);

            configurator.AddConfigurator(busServiceConfigurator);

            return sagaConfigurator;
        }

        /// <summary>
        /// Connects the saga to the service bus
        /// </summary>
        /// <typeparam name="TSaga">The consumer type</typeparam>
        /// <param name="bus"></param>
        /// <param name="sagaRepository"></param>
        public static UnsubscribeAction SubscribeSaga<TSaga>(this IServiceBus bus, ISagaRepository<TSaga> sagaRepository)
            where TSaga : class, ISaga
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", typeof(TSaga));

            Guard.AgainstNull(sagaRepository, "sagaRepository", "A saga repository must be specified");

            var connector = new SagaConnector<TSaga>(sagaRepository);

            return bus.Configure(x => connector.Connect(x));
        }
    }
}
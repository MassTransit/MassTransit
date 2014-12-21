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
    using Logging;
    using Policies;
    using Saga;
    using Saga.SubscriptionConfigurators;
    using Saga.SubscriptionConnectors;


    public static class SagaExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(SagaExtensions));

        /// <summary>
        /// Configure a saga subscription
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="sagaRepository"></param>
        /// <returns></returns>
        public static ISagaConfigurator<TSaga> Saga<TSaga>(this IReceiveEndpointConfigurator configurator,
            ISagaRepository<TSaga> sagaRepository, IRetryPolicy retryPolicy = null)
            where TSaga : class, ISaga
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", typeof(TSaga));

            var sagaConfigurator = new SagaConfigurator<TSaga>(sagaRepository, retryPolicy);

            configurator.AddConfigurator(sagaConfigurator);

            return sagaConfigurator;
        }

        /// <summary>
        /// Connects the saga to the service bus
        /// </summary>
        /// <typeparam name="TSaga">The consumer type</typeparam>
        /// <param name="bus"></param>
        /// <param name="sagaRepository"></param>
        public static ConnectHandle ConnectSaga<TSaga>(this IBus bus, ISagaRepository<TSaga> sagaRepository, IRetryPolicy retryPolicy = null)
            where TSaga : class, ISaga
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (sagaRepository == null)
                throw new ArgumentNullException("sagaRepository");

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", typeof(TSaga));

            return SagaConnectorCache<TSaga>.Connector.Connect(bus.ConsumePipe, sagaRepository, retryPolicy ?? Retry.None);
        }
    }
}
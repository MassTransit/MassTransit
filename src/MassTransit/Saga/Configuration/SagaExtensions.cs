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
    using Internals.Extensions;
    using Logging;
    using Policies;
    using Saga;
    using Saga.Connectors;
    using Saga.SubscriptionConfigurators;
    using Util;


    public static class SagaExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(SagaExtensions));

        /// <summary>
        /// Configure a saga subscription
        /// </summary>
        /// <typeparam name="TSaga"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="sagaRepository"></param>
        /// <param name="retryPolicy"></param>
        /// <returns></returns>
        public static ISagaConfigurator<TSaga> Saga<TSaga>(this IReceiveEndpointConfigurator configurator,
            ISagaRepository<TSaga> sagaRepository, IRetryPolicy retryPolicy = null)
            where TSaga : class, ISaga
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", TypeMetadataCache<TSaga>.ShortName);

            var sagaConfigurator = new SagaConfigurator<TSaga>(sagaRepository, retryPolicy);

            configurator.AddEndpointSpecification(sagaConfigurator);

            return sagaConfigurator;
        }

        /// <summary>
        /// Connects the saga to the bus
        /// </summary>
        /// <typeparam name="TSaga">The saga type</typeparam>
        /// <param name="bus">The bus to which the saga is to be connected</param>
        /// <param name="sagaRepository">The saga repository</param>
        public static ConnectHandle ConnectSaga<TSaga>(this IBus bus, ISagaRepository<TSaga> sagaRepository, IRetryPolicy retryPolicy = null)
            where TSaga : class, ISaga
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (sagaRepository == null)
                throw new ArgumentNullException("sagaRepository");

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", TypeMetadataCache<TSaga>.ShortName);

            return SagaConnectorCache<TSaga>.Connector.Connect(bus, sagaRepository, retryPolicy ?? Retry.None);
        }
    }
}
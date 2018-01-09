// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using GreenPipes;
    using Logging;
    using Pipeline;
    using Saga;
    using Saga.Connectors;
    using SagaConfigurators;
    using Util;


    public static class SagaExtensions
    {
        static readonly ILog _log = Logger.Get(typeof(SagaExtensions));

        /// <summary>
        /// Configure a saga subscription
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configurator"></param>
        /// <param name="sagaRepository"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, ISagaRepository<T> sagaRepository,
            Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", TypeMetadataCache<T>.ShortName);

            var sagaConfigurator = new SagaConfigurator<T>(sagaRepository, configurator);

            configure?.Invoke(sagaConfigurator);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        /// <summary>
        /// Connects the saga to the bus
        /// </summary>
        /// <typeparam name="T">The saga type</typeparam>
        /// <param name="bus">The bus to which the saga is to be connected</param>
        /// <param name="sagaRepository">The saga repository</param>
        /// <param name="pipeSpecifications"></param>
        public static ConnectHandle ConnectSaga<T>(this IConsumePipeConnector bus, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
            where T : class, ISaga
        {
            if (bus == null)
                throw new ArgumentNullException(nameof(bus));
            if (sagaRepository == null)
                throw new ArgumentNullException(nameof(sagaRepository));

            if (_log.IsDebugEnabled)
                _log.DebugFormat("Subscribing Saga: {0}", TypeMetadataCache<T>.ShortName);

            ISagaSpecification<T> specification = SagaConnectorCache<T>.Connector.CreateSagaSpecification<T>();
            foreach (IPipeSpecification<SagaConsumeContext<T>> pipeSpecification in pipeSpecifications)
            {
                specification.AddPipeSpecification(pipeSpecification);
            }

            return SagaConnectorCache<T>.Connector.ConnectSaga(bus, sagaRepository, specification);
        }
    }
}
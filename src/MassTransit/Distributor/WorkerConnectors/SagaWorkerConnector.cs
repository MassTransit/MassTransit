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
namespace MassTransit.Distributor.WorkerConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using MassTransit.Pipeline;
    using Saga;
    using Subscriptions;
    using Util;

    public interface SagaWorkerConnector
    {
        Type MessageType { get; }

        UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IWorker worker);
    }

    public class SagaWorkerConnector<T> :
        WorkerConnector
        where T : class, ISaga
    {
        readonly object[] _args;
        readonly IEnumerable<SagaWorkerConnector> _connectors;
        readonly ReferenceFactory _referenceFactory;
        readonly ISagaRepository<T> _sagaRepository;

        public SagaWorkerConnector(ReferenceFactory referenceFactory, ISagaRepository<T> sagaRepository)
        {
            _referenceFactory = referenceFactory;
            _sagaRepository = sagaRepository;

            _args = new object[] { _sagaRepository };

            try
            {
                Type[] interfaces = typeof(T).GetInterfaces();

                if (!interfaces.Contains(typeof(ISaga)))
                    throw new ConfigurationException("The type specified is not a saga");

                _connectors = Initiates()
                        .Concat(Orchestrates())
                        .Concat(Observes())
                        .Distinct((x, y) => x.MessageType == y.MessageType)
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException("Failed to create the saga connector for " + typeof(T).FullName, ex);
            }
        }

        public ISubscriptionReference Connect(IInboundPipelineConfigurator configurator, IWorker worker)
        {
            throw new NotImplementedException();
            //            return _referenceFactory(_connectors.Select(x => x.Connect(configurator, worker))
//                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x()));
        }

        IEnumerable<SagaWorkerConnector> Initiates()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
                .Select(x => new { InterfaceType = x, MessageType = x.GetGenericArguments()[0] })
                .Where(x => x.MessageType.IsValueType == false)
                .Select(x => CreateInitiatedByConnector(x.MessageType));
        }

        IEnumerable<SagaWorkerConnector> Orchestrates()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Orchestrates<>))
                .Select(x => new { InterfaceType = x, MessageType = x.GetGenericArguments()[0] })
                .Where(x => x.MessageType.IsValueType == false)
                .Select(x => CreateOrchestratesConnector(x.MessageType));
        }

        IEnumerable<SagaWorkerConnector> Observes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Observes<,>))
                .Select(x => new { InterfaceType = x, MessageType = x.GetGenericArguments()[0] })
                .Where(x => x.MessageType.IsValueType == false)
                .Select(x => CreateObservesConnector(x.MessageType));
        }

        SagaWorkerConnector CreateInitiatedByConnector(Type messageType)
        {
            return (SagaWorkerConnector)
                   FastActivator.Create(typeof(InitiatedBySagaWorkerConnector<,>),
                       new[] { typeof(T), messageType },
                       _args);
        }

        SagaWorkerConnector CreateOrchestratesConnector(Type messageType)
        {
            return (SagaWorkerConnector)
                   FastActivator.Create(typeof(OrchestratesSagaWorkerConnector<,>),
                       new[] { typeof(T), messageType },
                       _args);
        }

        SagaWorkerConnector CreateObservesConnector(Type messageType)
        {
            return (SagaWorkerConnector)
                   FastActivator.Create(typeof(ObservesSagaWorkerConnector<,>),
                       new[] { typeof(T), messageType },
                       _args);
        }
    }
}
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
namespace MassTransit.Distributor.DistributorConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using Magnum.Extensions;
    using Magnum.Reflection;
    using MassTransit.Pipeline;
    using Saga;
    using Subscriptions;
    using Util;

    public interface SagaDistributorConnector
    {
        Type MessageType { get; }

        UnsubscribeAction Connect(IInboundPipelineConfigurator configurator, IDistributor distributor);
    }

    public class SagaDistributorConnector<T> :
        DistributorConnector
        where T : class, ISaga
    {
        readonly object[] _args;
        readonly IEnumerable<SagaDistributorConnector> _connectors;
        readonly ReferenceFactory _referenceFactory;
        readonly ISagaRepository<T> _sagaRepository;
        readonly IWorkerSelectorFactory _workerSelectorFactory;

        public SagaDistributorConnector(ReferenceFactory referenceFactory, IWorkerSelectorFactory workerSelectorFactory,
            ISagaRepository<T> sagaRepository)
        {
            _referenceFactory = referenceFactory;
            _workerSelectorFactory = workerSelectorFactory;
            _sagaRepository = sagaRepository;

            _args = new object[] {_workerSelectorFactory, _sagaRepository};

            try
            {
                Type[] interfaces = typeof(T).GetInterfaces();

                if (!interfaces.Contains(typeof(ISaga)))
                    throw new ConfigurationException("The type specified is not a saga");

                _connectors = StateMachineEvents()
                    .Concat(Initiates()
                        .Concat(Orchestrates())
                        .Concat(Observes())
                        .Distinct((x, y) => x.MessageType == y.MessageType))
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException("Failed to create the saga connector for " + typeof(T).FullName, ex);
            }
        }

        public ISubscriptionReference Connect(IInboundPipelineConfigurator configurator, IDistributor distributor)
        {
            throw new NotImplementedException();
            //            return _referenceFactory(_connectors.Select(x => x.Connect(configurator, distributor))
//                .Aggregate<UnsubscribeAction, UnsubscribeAction>(() => true, (seed, x) => () => seed() && x()));
        }

        IEnumerable<SagaDistributorConnector> Initiates()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(InitiatedBy<>))
                .Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
                .Where(x => x.MessageType.IsValueType == false)
                .Select(x => CreateConnector(x.MessageType));
        }

        IEnumerable<SagaDistributorConnector> Orchestrates()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Orchestrates<>))
                .Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
                .Where(x => x.MessageType.IsValueType == false)
                .Select(x => CreateConnector(x.MessageType));
        }

        IEnumerable<SagaDistributorConnector> Observes()
        {
            return typeof(T).GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(Observes<,>))
                .Select(x => new {InterfaceType = x, MessageType = x.GetGenericArguments()[0]})
                .Where(x => x.MessageType.IsValueType == false)
                .Select(x => CreateObservesConnector(x.MessageType));
        }

        IEnumerable<SagaDistributorConnector> StateMachineEvents()
        {
            if (typeof(T).Implements(typeof(SagaStateMachine<>)))
            {
                var factory =
                    (IEnumerable<SagaDistributorConnector>)
                    FastActivator.Create(typeof(SagaStateMachineDistributorConnectorFactory<>),
                        new[] {typeof(T)},
                        _args);

                return factory;
            }

            return Enumerable.Empty<SagaDistributorConnector>();
        }

        SagaDistributorConnector CreateConnector(Type messageType)
        {
            return (SagaDistributorConnector)
                   FastActivator.Create(typeof(CorrelatedSagaDistributorConnector<,>),
                       new[] {typeof(T), messageType},
                       _args);
        }

        SagaDistributorConnector CreateObservesConnector(Type messageType)
        {
            return (SagaDistributorConnector)
                   FastActivator.Create(typeof(ObservesSagaDistributorConnector<,>),
                       new[] {typeof(T), messageType},
                       _args);
        }

        public ISubscriptionReference Connect(IInboundMessagePipe pipe, IDistributor distributor)
        {
            throw new NotImplementedException();
        }
    }
}
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
namespace Automatonymous.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using GreenPipes;
    using MassTransit;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.Configuration;
    using MassTransit.Saga.Connectors;
    using MassTransit.Util;


    public class StateMachineConnector<TInstance> :
        ISagaConnector
        where TInstance : class, ISaga, SagaStateMachineInstance
    {
        readonly List<ISagaMessageConnector<TInstance>> _connectors;
        readonly SagaStateMachine<TInstance> _stateMachine;

        public StateMachineConnector(SagaStateMachine<TInstance> stateMachine)
        {
            _stateMachine = stateMachine;

            try
            {
                _connectors = StateMachineEvents().ToList();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Failed to create the state machine connector for {TypeMetadataCache<TInstance>.ShortName}", ex);
            }
        }

        public ISagaSpecification<T> CreateSagaSpecification<T>() where T : class, ISaga
        {
            List<ISagaMessageSpecification<T>> messageSpecifications =
                _connectors.Select(x => x.CreateSagaMessageSpecification())
                    .Cast<ISagaMessageSpecification<T>>()
                    .ToList();

            return new SagaSpecification<T>(messageSpecifications);
        }

        public ConnectHandle ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository, ISagaSpecification<T> specification)
            where T : class, ISaga
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (var connector in _connectors.Cast<ISagaMessageConnector<T>>())
                {
                    var handle = connector.ConnectSaga(consumePipe, sagaRepository, specification);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (var handle in handles)
                    handle.Dispose();
                throw;
            }
        }

        IEnumerable<ISagaMessageConnector<TInstance>> StateMachineEvents()
        {
            EventCorrelation[] correlations = _stateMachine.Correlations.ToArray();

            StateMachineConfigurationResult.CompileResults(correlations.SelectMany(x => x.Validate()).ToArray());

            foreach (var correlation in correlations)
            {
                if (correlation.DataType.GetTypeInfo().IsValueType)
                    continue;

                var genericType = typeof(StateMachineInterfaceType<,>).MakeGenericType(typeof(TInstance), correlation.DataType);

                var interfaceType = (IStateMachineInterfaceType)Activator.CreateInstance(genericType,
                    _stateMachine, correlation);

                yield return interfaceType.GetConnector<TInstance>();
            }
        }
    }
}
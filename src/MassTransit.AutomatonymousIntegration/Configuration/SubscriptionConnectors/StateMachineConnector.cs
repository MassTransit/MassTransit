// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using MassTransit;
    using MassTransit.PipeConfigurators;
    using MassTransit.Pipeline;
    using MassTransit.Saga;
    using MassTransit.Saga.Connectors;
    using MassTransit.Util;


    public class StateMachineConnector<TInstance> :
        ISagaConnector
        where TInstance : class, ISaga, SagaStateMachineInstance
    {
        readonly IEnumerable<ISagaMessageConnector> _connectors;
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

        public ConnectHandle ConnectSaga<T>(IConsumePipeConnector consumePipe, ISagaRepository<T> sagaRepository,
            params IPipeSpecification<SagaConsumeContext<T>>[] pipeSpecifications)
            where T : class, ISaga
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (ISagaMessageConnector connector in _connectors)
                {
                    ConnectHandle handle = connector.ConnectSaga(consumePipe, sagaRepository, pipeSpecifications);

                    handles.Add(handle);
                }

                return new MultipleConnectHandle(handles);
            }
            catch (Exception)
            {
                foreach (ConnectHandle handle in handles)
                    handle.Dispose();
                throw;
            }
        }

        IEnumerable<ISagaMessageConnector> StateMachineEvents()
        {
            foreach (var correlation in _stateMachine.Correlations)
            {
                if (correlation.DataType.IsValueType)
                    continue;

                Type genericType = typeof(StateMachineInterfaceType<,>).MakeGenericType(typeof(TInstance), correlation.DataType);

                var interfaceType = (IStateMachineInterfaceType)Activator.CreateInstance(genericType,
                    _stateMachine, correlation);

                yield return interfaceType.GetConnector();
            }
        }
    }
}
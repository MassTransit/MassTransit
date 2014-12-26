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
namespace Automatonymous.SubscriptionConnectors
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MassTransit;
    using MassTransit.Internals.Extensions;
    using MassTransit.PipeConfigurators;
    using MassTransit.Pipeline;
    using MassTransit.Policies;
    using MassTransit.Saga;
    using MassTransit.Saga.SubscriptionConnectors;
    using MassTransit.Util;


    public class StateMachineConnector<TInstance> :
        SagaConnector
        where TInstance : class, ISaga, SagaStateMachineInstance
    {
        readonly IEnumerable<SagaMessageConnector> _connectors;
        readonly StateMachineSagaRepository<TInstance> _repository;
        readonly StateMachine<TInstance> _stateMachine;

        public StateMachineConnector(StateMachine<TInstance> stateMachine, StateMachineSagaRepository<TInstance> repository)
        {
            _stateMachine = stateMachine;
            _repository = repository;

            try
            {
                _connectors = StateMachineEvents().ToList();
            }
            catch (Exception ex)
            {
                throw new ConfigurationException(
                    "Failed to create the state machine connector for " + typeof(TInstance).FullName, ex);
            }
        }

        public IEnumerable<SagaMessageConnector> Connectors
        {
            get { return _connectors; }
        }

        public ConnectHandle Connect<TInstance>(IConsumePipe consumePipe, ISagaRepository<TInstance> sagaRepository,
            IRetryPolicy retryPolicy,
            params IPipeBuilderConfigurator<SagaConsumeContext<TInstance>>[] pipeBuilderConfigurators) where TInstance : class, ISaga
        {
            var handles = new List<ConnectHandle>();
            try
            {
                foreach (SagaMessageConnector connector in _connectors)
                {
                    ConnectHandle handle = connector.Connect(consumePipe, sagaRepository, retryPolicy, pipeBuilderConfigurators);

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

        IEnumerable<SagaMessageConnector> StateMachineEvents()
        {
            StateMachinePolicyFactory<TInstance> policyFactory = new AutomatonymousStateMachinePolicyFactory<TInstance>(_stateMachine);

            foreach (Event @event in _stateMachine.Events)
            {
                Event stateMachineEvent = @event;

                Type eventType = stateMachineEvent.GetType();
                if (!eventType.HasInterface(typeof(Event<>)))
                    continue;

                Type messageType = eventType.GetClosingArguments(typeof(Event<>)).SingleOrDefault();
                if (messageType == null || messageType.IsValueType)
                    continue;

                IEnumerable<State<TInstance>> states = _stateMachine.States
                    .Where(state => _stateMachine.NextEvents(state).Contains(stateMachineEvent))
                    .Select(x => _stateMachine.GetState(x.Name));

                Type genericType = typeof(StateMachineInterfaceType<,>).MakeGenericType(typeof(TInstance), messageType);

                var interfaceType = (IStateMachineInterfaceType<TInstance>)Activator.CreateInstance(genericType,
                    _stateMachine, _repository, policyFactory, states, stateMachineEvent);


//                        public StateMachineInterfaceType(StateMachine<TInstance> machine, StateMachineSagaRepository<TInstance> repository,
//            StateMachinePolicyFactory<TInstance> policyFactory, IEnumerable<State<TInstance>> states, Event<TData> @event)

                yield return interfaceType.GetConnector();
            }
        }
    }
}
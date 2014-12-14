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
    using Magnum.Reflection;
    using MassTransit;
    using MassTransit.PipeConfigurators;
    using MassTransit.Pipeline;
    using MassTransit.Policies;
    using MassTransit.Saga;
    using MassTransit.SubscriptionConnectors;


    public interface StateMachineConnector
    {
        ConnectHandle Connect<TInstance>(IConsumePipe consumePipe, ISagaRepository<TInstance> sagaRepository, IRetryPolicy retryPolicy,
            params IPipeBuilderConfigurator<SagaConsumeContext<TInstance>>[] pipeBuilderConfigurators)
            where TInstance : class, ISaga, SagaStateMachineInstance;
    }


    public class StateMachineConnector<T> :
        StateMachineConnector
        where T : class, SagaStateMachineInstance
    {
        readonly IEnumerable<StateMachineSubscriptionConnector> _connectors;
        readonly StateMachineSagaRepository<T> _repository;
        readonly StateMachine<T> _stateMachine;

        public StateMachineConnector(StateMachine<T> stateMachine,
            StateMachineSagaRepository<T> repository)
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
                    "Failed to create the state machine connector for " + typeof(T).FullName, ex);
            }
        }

        public IEnumerable<StateMachineSubscriptionConnector> Connectors
        {
            get { return _connectors; }
        }

        public ConnectHandle Connect<TInstance>(IConsumePipe consumePipe, ISagaRepository<TInstance> sagaRepository, IRetryPolicy retryPolicy,
            params IPipeBuilderConfigurator<SagaConsumeContext<TInstance>>[] pipeBuilderConfigurators)
            where TInstance : class, ISaga, SagaStateMachineInstance
        {
            return new MultipleConnectHandle(_connectors.Select(x => x.Connect(consumePipe, sagaRepository, retryPolicy, pipeBuilderConfigurators)));
        }

        IEnumerable<StateMachineSubscriptionConnector> StateMachineEvents()
        {
            var policyFactory = new AutomatonymousStateMachinePolicyFactory<T>(_stateMachine);

            foreach (Event @event in _stateMachine.Events)
            {
                Type eventType = @event.GetType();

                Type dataEventInterfaceType = eventType.GetInterfaces()
                    .Where(x => x.IsGenericType)
                    .Where(x => x.GetGenericTypeDefinition() == typeof(Event<>))
                    .SingleOrDefault();
                if (dataEventInterfaceType == null)
                    continue;

                Type messageType = dataEventInterfaceType.GetGenericArguments()[0];
                if (messageType.IsValueType)
                    continue;

                IEnumerable<State> states =
                    _stateMachine.States.Where(state => _stateMachine.NextEvents(state).Contains(@event));

                var factory =
                    (StateMachineEventConnectorFactory)
                        FastActivator.Create(typeof(StateMachineEventConnectorFactory<,>),
                            new[] {typeof(T), messageType},
                            new object[] {_stateMachine, _repository, policyFactory, @event, states});

                foreach (StateMachineSubscriptionConnector connector in factory.Create())
                    yield return connector;
            }
        }
    }
}
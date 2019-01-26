// Copyright 2007-2019 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace Automatonymous.Registration
{
    using System;
    using System.Collections.Generic;
    using MassTransit;
    using MassTransit.Registration;
    using MassTransit.Saga;
    using SagaConfigurators;


    public class SagaStateMachineRegistration<TInstance> :
        ISagaRegistration
        where TInstance : class, SagaStateMachineInstance
    {
        readonly List<Action<ISagaConfigurator<TInstance>>> _configureActions;

        public SagaStateMachineRegistration()
        {
            _configureActions = new List<Action<ISagaConfigurator<TInstance>>>();
        }

        public void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (configure is Action<ISagaConfigurator<TInstance>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            ISagaStateMachineFactory stateMachineFactory = configurationServiceProvider.GetService<ISagaStateMachineFactory>();

            SagaStateMachine<TInstance> stateMachine = stateMachineFactory.CreateStateMachine<TInstance>();

            IStateMachineActivityFactory activityFactory = configurationServiceProvider.GetService<IStateMachineActivityFactory>();

            void AddStateMachineActivityFactory(ConsumeContext context)
            {
                context.GetOrAddPayload(() => activityFactory);
            }

            var repositoryFactory = configurationServiceProvider.GetService<ISagaRepositoryFactory>();

            ISagaRepository<TInstance> repository = repositoryFactory.CreateSagaRepository<TInstance>(AddStateMachineActivityFactory);

            var stateMachineConfigurator = new StateMachineSagaConfigurator<TInstance>(stateMachine, repository, configurator);

            foreach (Action<ISagaConfigurator<TInstance>> action in _configureActions)
                action(stateMachineConfigurator);

            configurator.AddEndpointSpecification(stateMachineConfigurator);
        }
    }
}

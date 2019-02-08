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
namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using Definition;
    using Saga;
    using SagaConfigurators;


    /// <summary>
    /// A saga registration represents a single saga, which will use the container for the scope provider, as well as
    /// to resolve the saga repository.
    /// </summary>
    /// <typeparam name="TSaga">The saga type</typeparam>
    public class SagaRegistration<TSaga> :
        ISagaRegistration
        where TSaga : class, ISaga
    {
        readonly List<Action<ISagaConfigurator<TSaga>>> _configureActions;
        ISagaDefinition<TSaga> _definition;

        public SagaRegistration()
        {
            _configureActions = new List<Action<ISagaConfigurator<TSaga>>>();
        }

        void ISagaRegistration.AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
        {
            if (configure is Action<ISagaConfigurator<TSaga>> action)
                _configureActions.Add(action);
        }

        void ISagaRegistration.Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var repositoryFactory = configurationServiceProvider.GetRequiredService<ISagaRepositoryFactory>();
            ISagaRepository<TSaga> sagaRepository = repositoryFactory.CreateSagaRepository<TSaga>();
            var sagaConfigurator = new SagaConfigurator<TSaga>(sagaRepository, configurator);

            GetSagaDefinition(configurationServiceProvider)
                .Configure(configurator, sagaConfigurator);

            foreach (Action<ISagaConfigurator<TSaga>> action in _configureActions)
                action(sagaConfigurator);

            configurator.AddEndpointSpecification(sagaConfigurator);
        }

        ISagaDefinition ISagaRegistration.GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetSagaDefinition(provider);
        }

        ISagaDefinition<TSaga> GetSagaDefinition(IConfigurationServiceProvider provider)
        {
            return _definition ?? (_definition = provider.GetService<ISagaDefinition<TSaga>>() ?? new DefaultSagaDefinition<TSaga>());
        }
    }
}

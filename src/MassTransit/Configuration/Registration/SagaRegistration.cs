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
    using Saga;
    using SagaConfigurators;


    public class SagaRegistration<TSaga> :
        ISagaRegistration
        where TSaga : class, ISaga
    {
        readonly List<Action<ISagaConfigurator<TSaga>>> _configureActions;

        public SagaRegistration()
        {
            _configureActions = new List<Action<ISagaConfigurator<TSaga>>>();
        }

        public void AddConfigureAction<T>(Action<ISagaConfigurator<T>> configure)
            where T : class, ISaga
        {
            if (configure is Action<ISagaConfigurator<TSaga>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var repositoryFactory = configurationServiceProvider.GetService<ISagaRepositoryFactory>();

            ISagaRepository<TSaga> sagaRepository = repositoryFactory.CreateSagaRepository<TSaga>();

            var consumerConfigurator = new SagaConfigurator<TSaga>(sagaRepository, configurator);

            foreach (var action in _configureActions)
            {
                action(consumerConfigurator);
            }

            configurator.AddEndpointSpecification(consumerConfigurator);
        }
    }
}

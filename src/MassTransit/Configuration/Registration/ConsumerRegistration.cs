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
    using ConsumeConfigurators;
    using Scoping;


    public class ConsumerRegistration<TConsumer> :
        IConsumerRegistration
        where TConsumer : class, IConsumer
    {
        readonly List<Action<IConsumerConfigurator<TConsumer>>> _configureActions;

        public ConsumerRegistration()
        {
            _configureActions = new List<Action<IConsumerConfigurator<TConsumer>>>();
        }

        public void AddConfigureAction<T>(Action<IConsumerConfigurator<T>> configure)
            where T : class, IConsumer
        {
            if (configure is Action<IConsumerConfigurator<TConsumer>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var scopeProvider = configurationServiceProvider.GetService<IConsumerScopeProvider>();

            var consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            foreach (var action in _configureActions)
            {
                action(consumerConfigurator);
            }

            configurator.AddEndpointSpecification(consumerConfigurator);
        }
    }
}

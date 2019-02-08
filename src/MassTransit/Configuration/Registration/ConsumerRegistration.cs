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
    using Definition;
    using Scoping;


    /// <summary>
    /// A consumer registration represents a single consumer, which will be resolved from the container using the scope
    /// provider. The consumer definition, if present, is loaded from the container and used to configure the consumer
    /// within the receive endpoint.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    public class ConsumerRegistration<TConsumer> :
        IConsumerRegistration
        where TConsumer : class, IConsumer
    {
        readonly List<Action<IConsumerConfigurator<TConsumer>>> _configureActions;
        IConsumerDefinition<TConsumer> _definition;

        public ConsumerRegistration()
        {
            _configureActions = new List<Action<IConsumerConfigurator<TConsumer>>>();
        }

        void IConsumerRegistration.AddConfigureAction<T>(Action<IConsumerConfigurator<T>> configure)
        {
            if (configure is Action<IConsumerConfigurator<TConsumer>> action)
                _configureActions.Add(action);
        }

        void IConsumerRegistration.Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var scopeProvider = configurationServiceProvider.GetRequiredService<IConsumerScopeProvider>();
            var consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);
            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            GetConsumerDefinition(configurationServiceProvider)
                .Configure(configurator, consumerConfigurator);

            foreach (Action<IConsumerConfigurator<TConsumer>> action in _configureActions)
                action(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        IConsumerDefinition IConsumerRegistration.GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetConsumerDefinition(provider);
        }

        IConsumerDefinition<TConsumer> GetConsumerDefinition(IConfigurationServiceProvider provider)
        {
            return _definition ?? (_definition = provider.GetService<IConsumerDefinition<TConsumer>>() ?? new DefaultConsumerDefinition<TConsumer>());
        }
    }
}

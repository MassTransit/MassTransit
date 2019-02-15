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
    using Definition;


    public class ConsumerRegistrationConfigurator<TConsumer> :
        IConsumerRegistrationConfigurator<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IConsumerRegistration _registration;
        readonly IContainerRegistrar _registrar;

        public ConsumerRegistrationConfigurator(IRegistrationConfigurator configurator, IConsumerRegistration registration, IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registration = registration;
            _registrar = registrar;
        }

        public void Endpoint(Action<IConsumerEndpointRegistrationConfigurator<TConsumer>> configure)
        {
            var configurator = new ConsumerEndpointRegistrationConfigurator<TConsumer>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ConsumerEndpointDefinition<TConsumer>, TConsumer>(configurator.Settings);

            _registrar.RegisterConsumerDefinition<EndpointConsumerDefinition<TConsumer>, TConsumer>();
        }
    }
}
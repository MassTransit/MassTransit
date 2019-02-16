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
    using Saga;


    public class SagaRegistrationConfigurator<TSaga> :
        ISagaRegistrationConfigurator<TSaga>
        where TSaga : class, ISaga
    {
        readonly IRegistrationConfigurator _configurator;
        readonly ISagaRegistration _registration;
        readonly IContainerRegistrar _registrar;

        public SagaRegistrationConfigurator(IRegistrationConfigurator configurator, ISagaRegistration registration, IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registration = registration;
            _registrar = registrar;
        }

        public void Endpoint(Action<ISagaEndpointRegistrationConfigurator<TSaga>> configure)
        {
            var configurator = new SagaEndpointRegistrationConfigurator<TSaga>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<SagaEndpointDefinition<TSaga>, TSaga>(configurator.Settings);

            _registrar.RegisterSagaDefinition<EndpointSagaDefinition<TSaga>, TSaga>();
        }
    }
}

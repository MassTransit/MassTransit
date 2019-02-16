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
    using Courier;
    using Definition;


    public class ExecuteActivityRegistrationConfigurator<TActivity, TArguments> :
        IExecuteActivityRegistrationConfigurator<TActivity, TArguments>
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IExecuteActivityRegistration _registration;
        readonly IContainerRegistrar _registrar;

        public ExecuteActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IExecuteActivityRegistration registration,
            IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registration = registration;
            _registrar = registrar;
        }

        public void Endpoint(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configure)
        {
            var configurator = new ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, ExecuteActivity<TArguments>>(configurator.Settings);

            _registrar.RegisterExecuteActivityDefinition<EndpointExecuteActivityDefinition<TActivity, TArguments>, TActivity, TArguments>();
        }
    }
}

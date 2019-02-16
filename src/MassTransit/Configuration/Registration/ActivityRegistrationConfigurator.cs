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


    public class ActivityRegistrationConfigurator<TActivity, TArguments, TLog> :
        IActivityRegistrationConfigurator<TActivity, TArguments, TLog>
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IActivityRegistration _registration;
        readonly IContainerRegistrar _registrar;

        public ActivityRegistrationConfigurator(IRegistrationConfigurator configurator, IActivityRegistration registration,
            IContainerRegistrar registrar)
        {
            _configurator = configurator;
            _registration = registration;
            _registrar = registrar;
        }

        public void Endpoints(Action<IExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>> configureExecute,
            Action<ICompensateActivityEndpointRegistrationConfigurator<TActivity, TLog>> configureCompensate)
        {
            var configurator = new ExecuteActivityEndpointRegistrationConfigurator<TActivity, TArguments>();

            configureExecute?.Invoke(configurator);

            _configurator.AddEndpoint<ExecuteActivityEndpointDefinition<TActivity, TArguments>, ExecuteActivity<TArguments>>(configurator.Settings);

            var compensateConfigurator = new CompensateActivityEndpointRegistrationConfigurator<TActivity, TLog>();

            configureCompensate?.Invoke(compensateConfigurator);

            _configurator.AddEndpoint<CompensateActivityEndpointDefinition<TActivity, TLog>, CompensateActivity<TLog>>(compensateConfigurator.Settings);

            _registrar.RegisterActivityDefinition<EndpointActivityDefinition<TActivity, TArguments, TLog>, TActivity, TArguments, TLog>();
        }
    }
}

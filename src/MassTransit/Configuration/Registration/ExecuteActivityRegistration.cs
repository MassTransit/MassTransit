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
    using Courier;
    using PipeConfigurators;
    using Scoping;


    public class ExecuteActivityRegistration<TActivity, TArguments> :
        IExecuteActivityRegistration
        where TActivity : class, ExecuteActivity<TArguments>
        where TArguments : class
    {
        readonly List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>> _configureActions;

        public ExecuteActivityRegistration()
        {
            _configureActions = new List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>>();
        }

        void IExecuteActivityRegistration.AddConfigureAction<T, TArgs>(Action<IExecuteActivityConfigurator<T, TArgs>> configure)
        {
            if (configure is Action<IExecuteActivityConfigurator<TActivity, TArguments>> action)
                _configureActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var executeActivityScopeProvider = configurationServiceProvider.GetService<IExecuteActivityScopeProvider<TActivity, TArguments>>();

            var executeActivityFactory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(executeActivityFactory);

            foreach (var action in _configureActions)
                action(specification);

            configurator.AddEndpointSpecification(specification);
        }
    }
}
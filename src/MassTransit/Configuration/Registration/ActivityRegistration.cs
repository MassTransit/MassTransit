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


    public class ActivityRegistration<TActivity, TArguments, TLog> :
        IActivityRegistration
        where TActivity : class, Activity<TArguments, TLog>
        where TArguments : class
        where TLog : class
    {
        readonly List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>> _executeActions;
        readonly List<Action<ICompensateActivityConfigurator<TActivity, TLog>>> _compensateActions;

        public ActivityRegistration()
        {
            _executeActions = new List<Action<IExecuteActivityConfigurator<TActivity, TArguments>>>();
            _compensateActions = new List<Action<ICompensateActivityConfigurator<TActivity, TLog>>>();
        }

        public void AddConfigureAction<T, TA>(Action<IExecuteActivityConfigurator<T, TA>> configure)
            where T : class, ExecuteActivity<TA>
            where TA : class
        {
            if (configure is Action<IExecuteActivityConfigurator<TActivity, TArguments>> action)
                _executeActions.Add(action);
        }

        public void AddConfigureAction<T, TL>(Action<ICompensateActivityConfigurator<T, TL>> configure)
            where T : class, CompensateActivity<TL>
            where TL : class
        {
            if (configure is Action<ICompensateActivityConfigurator<TActivity, TLog>> action)
                _compensateActions.Add(action);
        }

        public void Configure(IReceiveEndpointConfigurator executeEndpointConfigurator, IReceiveEndpointConfigurator compensateEndpointConfigurator,
            IConfigurationServiceProvider scopeProvider)
        {
            ConfigureCompensate(compensateEndpointConfigurator, scopeProvider);

            ConfigureExecute(executeEndpointConfigurator, scopeProvider, compensateEndpointConfigurator.InputAddress);
        }

        void ConfigureCompensate(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var activityScopeProvider = configurationServiceProvider.GetService<ICompensateActivityScopeProvider<TActivity, TLog>>();

            var activityFactory = new ScopeCompensateActivityFactory<TActivity, TLog>(activityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(activityFactory);

            foreach (var action in _compensateActions)
                action(specification);

            configurator.AddEndpointSpecification(specification);
        }

        void ConfigureExecute(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider, Uri compensateAddress)
        {
            var activityScopeProvider = configurationServiceProvider.GetService<IExecuteActivityScopeProvider<TActivity, TArguments>>();

            var activityFactory = new ScopeExecuteActivityFactory<TActivity, TArguments>(activityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(activityFactory, compensateAddress);

            foreach (var action in _executeActions)
                action(specification);

            configurator.AddEndpointSpecification(specification);
        }
    }
}
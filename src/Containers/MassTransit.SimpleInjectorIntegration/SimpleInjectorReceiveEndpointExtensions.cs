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
namespace MassTransit
{
    using System;
    using ConsumeConfigurators;
    using Courier;
    using PipeConfigurators;
    using SimpleInjector;
    using SimpleInjectorIntegration.ScopeProviders;
    using Saga;
    using Scoping;


    public static class SimpleInjectorReceiveEndpointExtensions
    {
        public static void Consumer<T>(this IReceiveEndpointConfigurator configurator, Container container, Action<IConsumerConfigurator<T>> configure = null)
            where T : class, IConsumer
        {
            var consumerFactory = new ScopeConsumerFactory<T>(new SimpleInjectorConsumerScopeProvider(container));

            configurator.Consumer(consumerFactory, configure);
        }

        public static void Saga<T>(this IReceiveEndpointConfigurator configurator, Container container, Action<ISagaConfigurator<T>> configure = null)
            where T : class, ISaga
        {
            var repository = container.GetInstance<ISagaRepository<T>>();

            var scopeProvider = new SimpleInjectorSagaScopeProvider<T>(container);

            var sagaRepository = new ScopeSagaRepository<T>(repository, scopeProvider);

            configurator.Saga(sagaRepository, configure);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Uri compensateAddress,
            Container container,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new SimpleInjectorExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory, compensateAddress);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void ExecuteActivityHost<TActivity, TArguments>(this IReceiveEndpointConfigurator configurator, Container container,
            Action<IExecuteActivityConfigurator<TActivity, TArguments>> configure = null)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var executeActivityScopeProvider = new SimpleInjectorExecuteActivityScopeProvider<TActivity, TArguments>(container);

            var factory = new ScopeExecuteActivityFactory<TActivity, TArguments>(executeActivityScopeProvider);

            var specification = new ExecuteActivityHostSpecification<TActivity, TArguments>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }

        public static void CompensateActivityHost<TActivity, TLog>(this IReceiveEndpointConfigurator configurator, Container container,
            Action<ICompensateActivityConfigurator<TActivity, TLog>> configure = null)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var compensateActivityScopeProvider = new SimpleInjectorCompensateActivityScopeProvider<TActivity, TLog>(container);

            var factory = new ScopeCompensateActivityFactory<TActivity, TLog>(compensateActivityScopeProvider);

            var specification = new CompensateActivityHostSpecification<TActivity, TLog>(factory);

            configure?.Invoke(specification);

            configurator.AddEndpointSpecification(specification);
        }
    }
}

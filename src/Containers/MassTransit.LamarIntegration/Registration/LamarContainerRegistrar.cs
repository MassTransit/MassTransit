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
namespace MassTransit.LamarIntegration.Registration
{
    using System;
    using Courier;
    using Definition;
    using Lamar;
    using MassTransit.Registration;
    using Microsoft.Extensions.DependencyInjection;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class LamarContainerRegistrar :
        IContainerRegistrar
    {
        readonly ServiceRegistry _registry;

        public LamarContainerRegistrar(ServiceRegistry registry)
        {
            _registry = registry;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _registry.ForConcreteType<T>();
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _registry.For<IConsumerDefinition<TConsumer>>()
                .Use<TDefinition>();
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _registry.For<ISagaDefinition<TSaga>>()
                .Use<TDefinition>();
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            _registry.ForConcreteType<TActivity>();

            _registry.For<IExecuteActivityScopeProvider<TActivity, TArguments>>()
                .Use(CreateExecuteActivityScopeProvider<TActivity, TArguments>);
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, Activity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _registry.For<IActivityDefinition<TActivity, TArguments, TLog>>()
                .Use<TDefinition>();
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            _registry.For<IExecuteActivityDefinition<TActivity, TArguments>>()
                .Use<TDefinition>();
        }

        public void RegisterEndpointDefinition<TDefinition, T>(IEndpointSettings<IEndpointDefinition<T>> settings)
            where TDefinition : class, IEndpointDefinition<T>
            where T : class
        {
            _registry.For<IEndpointDefinition<T>>().Use<TDefinition>();

            if (settings != null)
                _registry.ForSingletonOf<IEndpointSettings<IEndpointDefinition<T>>>().Use(settings);
        }

        public void RegisterRequestClient<T>(RequestTimeout timeout = default)
            where T : class
        {
            _registry.For<IRequestClient<T>>().Use(context =>
            {
                var clientFactory = context.GetInstance<IClientFactory>();

                ConsumeContext consumeContext = context.TryGetInstance<ConsumeContext>();
                return (consumeContext != null)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, timeout)
                    : clientFactory.CreateRequestClient<T>(timeout);
            }).Scoped();
        }

        public void RegisterRequestClient<T>(Uri destinationAddress, RequestTimeout timeout = default)
            where T : class
        {
            _registry.For<IRequestClient<T>>().Use(context =>
            {
                var clientFactory = context.GetInstance<IClientFactory>();

                ConsumeContext consumeContext = context.TryGetInstance<ConsumeContext>();
                return (consumeContext != null)
                    ? clientFactory.CreateRequestClient<T>(consumeContext, destinationAddress, timeout)
                    : clientFactory.CreateRequestClient<T>(destinationAddress, timeout);
            }).Scoped();
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            _registry.ForConcreteType<TActivity>();

            _registry.For<ICompensateActivityScopeProvider<TActivity, TLog>>()
                .Use(CreateCompensateActivityScopeProvider<TActivity, TLog>);
        }

        IExecuteActivityScopeProvider<TActivity, TArguments> CreateExecuteActivityScopeProvider<TActivity, TArguments>(IServiceContext context)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            return new LamarExecuteActivityScopeProvider<TActivity, TArguments>(context.GetRequiredService<IContainer>());
        }

        ICompensateActivityScopeProvider<TActivity, TLog> CreateCompensateActivityScopeProvider<TActivity, TLog>(IServiceContext context)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            return new LamarCompensateActivityScopeProvider<TActivity, TLog>(context.GetRequiredService<IContainer>());
        }
    }
}

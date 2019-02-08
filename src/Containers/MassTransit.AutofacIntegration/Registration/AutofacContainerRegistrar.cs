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
namespace MassTransit.AutofacIntegration.Registration
{
    using Autofac;
    using Courier;
    using Definition;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class AutofacContainerRegistrar :
        IContainerRegistrar
    {
        readonly ContainerBuilder _builder;

        public AutofacContainerRegistrar(ContainerBuilder builder)
        {
            _builder = builder;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _builder.RegisterType<T>();
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _builder.RegisterType<TDefinition>()
                .As<IConsumerDefinition<TConsumer>>();
        }

        public void RegisterSaga<T>()
            where T : class, ISaga
        {
        }

        public void RegisterSagaDefinition<TDefinition, TSaga>()
            where TDefinition : class, ISagaDefinition<TSaga>
            where TSaga : class, ISaga
        {
            _builder.RegisterType<TDefinition>()
                .As<ISagaDefinition<TSaga>>();
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            _builder.RegisterType<TActivity>();

            _builder.Register(CreateExecuteActivityScopeProvider<TActivity, TArguments>)
                .As<IExecuteActivityScopeProvider<TActivity, TArguments>>();
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, Activity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _builder.RegisterType<TDefinition>()
                .As<IActivityDefinition<TActivity, TArguments, TLog>>();
        }

        public void RegisterExecuteActivityDefinition<TDefinition, TActivity, TArguments>()
            where TDefinition : class, IExecuteActivityDefinition<TActivity, TArguments>
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            _builder.RegisterType<TDefinition>()
                .As<IExecuteActivityDefinition<TActivity, TArguments>>();
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            _builder.RegisterType<TActivity>();

            _builder.Register(CreateCompensateActivityScopeProvider<TActivity, TLog>)
                .As<ICompensateActivityScopeProvider<TActivity, TLog>>();
        }

        IExecuteActivityScopeProvider<TActivity, TArguments> CreateExecuteActivityScopeProvider<TActivity, TArguments>(IComponentContext context)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacExecuteActivityScopeProvider<TActivity, TArguments>(lifetimeScopeProvider, "message");
        }

        ICompensateActivityScopeProvider<TActivity, TLog> CreateCompensateActivityScopeProvider<TActivity, TLog>(IComponentContext context)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            var lifetimeScopeProvider = new SingleLifetimeScopeProvider(context.Resolve<ILifetimeScope>());

            return new AutofacCompensateActivityScopeProvider<TActivity, TLog>(lifetimeScopeProvider, "message");
        }
    }
}

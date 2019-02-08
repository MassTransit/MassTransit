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
namespace MassTransit.StructureMapIntegration.Registration
{
    using Courier;
    using Definition;
    using StructureMap;
    using MassTransit.Registration;
    using Saga;
    using ScopeProviders;
    using Scoping;


    public class StructureMapContainerRegistrar :
        IContainerRegistrar
    {
        readonly ConfigurationExpression _expression;

        public StructureMapContainerRegistrar(ConfigurationExpression expression)
        {
            _expression = expression;
        }

        public void RegisterConsumer<T>()
            where T : class, IConsumer
        {
            _expression.ForConcreteType<T>();
        }

        public void RegisterConsumerDefinition<TDefinition, TConsumer>()
            where TDefinition : class, IConsumerDefinition<TConsumer>
            where TConsumer : class, IConsumer
        {
            _expression.For<IConsumerDefinition<TConsumer>>()
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
            _expression.For<ISagaDefinition<TSaga>>()
                .Use<TDefinition>();
        }

        public void RegisterExecuteActivity<TActivity, TArguments>()
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            _expression.ForConcreteType<TActivity>();

            _expression.For<IExecuteActivityScopeProvider<TActivity, TArguments>>()
                .Use(context => CreateExecuteActivityScopeProvider<TActivity, TArguments>(context));
        }

        public void RegisterActivityDefinition<TDefinition, TActivity, TArguments, TLog>()
            where TDefinition : class, IActivityDefinition<TActivity, TArguments, TLog>
            where TActivity : class, Activity<TArguments, TLog>
            where TArguments : class
            where TLog : class
        {
            _expression.For<IActivityDefinition<TActivity, TArguments, TLog>>()
                .Use<TDefinition>();
        }

        public void RegisterCompensateActivity<TActivity, TLog>()
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            _expression.ForConcreteType<TActivity>();

            _expression.For<ICompensateActivityScopeProvider<TActivity, TLog>>()
                .Use(context => CreateCompensateActivityScopeProvider<TActivity, TLog>(context));
        }

        IExecuteActivityScopeProvider<TActivity, TArguments> CreateExecuteActivityScopeProvider<TActivity, TArguments>(IContext context)
            where TActivity : class, ExecuteActivity<TArguments>
            where TArguments : class
        {
            return new StructureMapExecuteActivityScopeProvider<TActivity, TArguments>(context.GetInstance<IContainer>());
        }

        ICompensateActivityScopeProvider<TActivity, TLog> CreateCompensateActivityScopeProvider<TActivity, TLog>(IContext context)
            where TActivity : class, CompensateActivity<TLog>
            where TLog : class
        {
            return new StructureMapCompensateActivityScopeProvider<TActivity, TLog>(context.GetInstance<IContainer>());
        }
    }
}

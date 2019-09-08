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
    using Automatonymous;
    using Automatonymous.Registration;
    using StructureMap;


    public class StructureMapSagaStateMachineRegistrar :
        ISagaStateMachineRegistrar
    {
        readonly ConfigurationExpression _expression;

        public StructureMapSagaStateMachineRegistrar(ConfigurationExpression expression)
        {
            _expression = expression;
        }

        public void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _expression.For<ISagaStateMachineFactory>().Use<StructureMapSagaStateMachineFactory>().Singleton();
            _expression.For<IStateMachineActivityFactory>().Use<StructureMapStateMachineActivityFactory>().Singleton();

            _expression.For<TStateMachine>().Singleton();
            _expression.For<SagaStateMachine<TInstance>>().Use(provider => provider.GetInstance<TStateMachine>()).Singleton();
        }
    }
}

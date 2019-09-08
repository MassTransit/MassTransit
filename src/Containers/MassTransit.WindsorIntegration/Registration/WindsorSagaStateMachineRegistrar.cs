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
namespace MassTransit.AutomatonymousWindsorIntegration.Registration
{
    using Automatonymous;
    using Automatonymous.Registration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;


    public class WindsorSagaStateMachineRegistrar :
        ISagaStateMachineRegistrar
    {
        readonly IWindsorContainer _container;

        public WindsorSagaStateMachineRegistrar(IWindsorContainer container)
        {
            _container = container;
        }

        public void RegisterStateMachineSaga<TStateMachine, TInstance>()
            where TStateMachine : class, SagaStateMachine<TInstance>
            where TInstance : class, SagaStateMachineInstance
        {
            _container.Register(
                Component.For<ISagaStateMachineFactory>().ImplementedBy<WindsorSagaStateMachineFactory>().LifestyleSingleton(),
                Component.For<IStateMachineActivityFactory>().ImplementedBy<WindsorStateMachineActivityFactory>().LifestyleSingleton(),
                Component.For<TStateMachine>().LifestyleSingleton(),
                Component.For<SagaStateMachine<TInstance>>().UsingFactoryMethod(provider => provider.Resolve<TStateMachine>()).LifestyleSingleton()
            );
        }
    }
}

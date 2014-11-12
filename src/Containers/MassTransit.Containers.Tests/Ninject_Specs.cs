// Copyright 2007-2012 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Containers.Tests
{
    using Magnum.TestFramework;
    using Ninject;
    using Ninject.Extensions.NamedScope;
    using Saga;
    using Scenarios;
    using SubscriptionConfigurators;


    [Scenario]
    public class Ninject_Consumer :
        When_registering_a_consumer
    {
        readonly IKernel _container;

        public Ninject_Consumer()
        {
            _container = new StandardKernel();
            _container.Bind<SimpleConsumer>()
                      .ToSelf().InCallScope();
            _container.Bind<ISimpleConsumerDependency>()
                      .To<SimpleConsumerDependency>().InCallScope();
            _container.Bind<AnotherMessageConsumer>()
                      .To<AnotherMessageConsumerImpl>().InCallScope();
        }

        [Finally]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void SubscribeLocalBus(SubscriptionBusServiceConfigurator subscriptionBusServiceConfigurator)
        {
            subscriptionBusServiceConfigurator.LoadFrom(_container);
        }
    }


    [Scenario]
    public class Ninject_Saga :
        When_registering_a_saga
    {
        readonly IKernel _container;

        public Ninject_Saga()
        {
            _container = new StandardKernel();
            _container.Bind<SimpleSaga>()
                      .ToSelf();
            _container.Bind(typeof(ISagaRepository<>))
                      .To(typeof(InMemorySagaRepository<>))
                      .InSingletonScope();
        }

        [Finally]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void SubscribeLocalBus(SubscriptionBusServiceConfigurator subscriptionBusServiceConfigurator)
        {
            // we have to do this explicitly, since the metadata is not exposed by Ninject
            subscriptionBusServiceConfigurator.Saga<SimpleSaga>(_container);
        }
    }
}
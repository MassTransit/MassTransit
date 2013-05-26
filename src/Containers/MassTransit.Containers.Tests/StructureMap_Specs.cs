// Copyright 2007-2011 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Saga;
    using Scenarios;
    using StructureMap;
    using SubscriptionConfigurators;

    [Scenario]
    public class StructureMap_Consumer :
        When_registering_a_consumer
    {
        readonly IContainer _container;

        public StructureMap_Consumer()
        {
            _container = new Container(x =>
                {
                    x.For<SimpleConsumer>()
                        .Use<SimpleConsumer>();
                    x.For<ISimpleConsumerDependency>()
                     .Use<SimpleConsumerDependency>();
                    x.For<AnotherMessageConsumer>()
                        .Use<AnotherMessageConsumerImpl>();
                });
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
    public class StructureMap_Saga :
        When_registering_a_saga
    {
        readonly IContainer _container;

        public StructureMap_Saga()
        {
            _container = new Container(x =>
                {
                    x.For(typeof (ISagaRepository<>))
                        .Singleton()
                        .Use(typeof (InMemorySagaRepository<>));

                    x.ForConcreteType<SimpleSaga>();
                });
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
}
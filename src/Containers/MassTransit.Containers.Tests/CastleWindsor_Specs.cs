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
    using System;
    using System.Linq;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Magnum.TestFramework;
    using Saga;
    using Scenarios;
    using SubscriptionConfigurators;
    using Testing;
    using Util;

    [Scenario]
    public class Castle_Consumer :
        When_registering_a_consumer
    {
        readonly IWindsorContainer _container;

        public Castle_Consumer()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<SimpleConsumer>());
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
    public class Castle_Saga :
        When_registering_a_saga
    {
        readonly IWindsorContainer _container;

        public Castle_Saga()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<SimpleSaga>(),
                Component.For(typeof (ISagaRepository<>))
                    .ImplementedBy(typeof (InMemorySagaRepository<>))
                    .LifeStyle.Singleton);
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
    public class Test_Bus_Subscriptions_For_Consumers_In_Dummy_Saga_Using_Castle_As_IoC :
        Given_a_service_bus_instance
    {
        readonly IWindsorContainer _container;

        public Test_Bus_Subscriptions_For_Consumers_In_Dummy_Saga_Using_Castle_As_IoC()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<DummySaga>(),
                Component.For(typeof(ISagaRepository<>))
                    .ImplementedBy(typeof(InMemorySagaRepository<>))
                    .LifeStyle.Singleton);
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

        [When]
        public void Registering_a_dummy_saga()
        {
        }

        [Then]
        public void Should_have_a_subscription_for_the_first_saga_message()
        {
            LocalBus.HasSubscription<FirstSagaMessage>().Count()
                .ShouldEqual(1, "No subscription for the FirstSagaMessage was found.");
        }

        [Then]
        public void Should_have_a_subscription_for_the_second_saga_message()
        {
            LocalBus.HasSubscription<SecondSagaMessage>().Count()
                .ShouldEqual(1, "No subscription for the SecondSagaMessage was found.");
        }

        [UsedImplicitly]
        class DummySaga : ISaga, InitiatedBy<FirstSagaMessage>, Orchestrates<SecondSagaMessage>
        {
            public Guid CorrelationId
            {
                get { return Guid.Empty; }
            }

            public IServiceBus Bus { get; set; }

            public void Consume(FirstSagaMessage message)
            {
            }

            public void Consume(SecondSagaMessage message)
            {
            }
        }
    }
}
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
    using System;
    using System.Linq;
    using System.Threading;
    using BusConfigurators;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using EndpointConfigurators;
    using Magnum.Extensions;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using Shouldly;
    using SubscriptionConfigurators;
    using TestFramework;
    using Testing;
    using Util;
    using WindsorIntegration;


    
    public class Castle_Consumer :
        When_registering_a_consumer
    {
        readonly IWindsorContainer _container;

        public Castle_Consumer()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<SimpleConsumer>()
                         .LifestyleTransient(),
                Component.For<ISimpleConsumerDependency>()
                         .ImplementedBy<SimpleConsumerDependency>()
                         .LifestyleTransient(),
                Component.For<AnotherMessageConsumer>()
                         .ImplementedBy<AnotherMessageConsumerImpl>()
                         .LifestyleTransient());
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    
    public class Castle_Saga :
        When_registering_a_saga
    {
        readonly IWindsorContainer _container;

        public Castle_Saga()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<SimpleSaga>(),
                Component.For(typeof(ISagaRepository<>))
                         .ImplementedBy(typeof(InMemorySagaRepository<>))
                         .LifeStyle.Singleton);
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    
    public class Test_Bus_Subscriptions_For_Consumers_In_Dummy_Saga_Using_Castle_As_IoC :
        Given_a_service_bus_instance
    {
        readonly IWindsorContainer _container;

        public Test_Bus_Subscriptions_For_Consumers_In_Dummy_Saga_Using_Castle_As_IoC()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<SimpleSaga>(),
                Component.For(typeof(ISagaRepository<>))
                         .ImplementedBy(typeof(InMemorySagaRepository<>))
                         .LifeStyle.Singleton);
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }

        [SetUp]
        public void Registering_a_dummy_saga()
        {
        }

        [Test]
        public void Should_have_a_subscription_for_the_first_saga_message()
        {
//            LocalBus.HasSubscription<FirstSagaMessage>().Count()
//                    .ShouldBe(1, "No subscription for the FirstSagaMessage was found.");
        }

        [Test]
        public void Should_have_a_subscription_for_the_second_saga_message()
        {
//            LocalBus.HasSubscription<SecondSagaMessage>().Count()
//                    .ShouldBe(1, "No subscription for the SecondSagaMessage was found.");
        }
    }


    
    public class MessageScope_usage :
        InMemoryTestFixture
    {
        readonly IWindsorContainer _container;

        public MessageScope_usage()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<CheckScopeConsumer>()
                         .LifestyleScoped<MessageScope>(),
                Component.For<IDepedency>()
                         .ImplementedBy<Depedency>()
                         .LifestyleScoped<MessageScope>());
        }

        [Test]
        public async void Should_receive_a_message_in_scope()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            CheckScopeConsumer.Last.Name.ShouldBe(name);
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        protected override void ConfigureBus(IInMemoryServiceBusFactoryConfigurator configurator)
        {
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.EnableMessageScope();

            configurator.LoadFrom(_container);
        }



        public class CheckScopeConsumer :
            Consumes<SimpleMessageInterface>.All
        {
            static SimpleMessageInterface _last;
            static ManualResetEvent _received = new ManualResetEvent(false);
            readonly IDepedency _depedency;

            public CheckScopeConsumer(IDepedency depedency)
            {
                _depedency = depedency;

                depedency.ShouldNotBe(null);
            }

            public static SimpleMessageInterface Last
            {
                get
                {
                    if (_received.WaitOne(8.Seconds()))
                        return _last;

                    throw new TimeoutException("Timeout waiting for message to be consumed");
                }
            }

            public void Consume(SimpleMessageInterface message)
            {
                _last = message;
                _received.Set();
            }
        }


        public class Depedency :
            IDepedency
        {
        }


        public interface IDepedency
        {
        }
    }
}
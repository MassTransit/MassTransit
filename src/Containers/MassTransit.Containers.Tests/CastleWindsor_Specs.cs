// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using System.Threading.Tasks;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Internals.Extensions;
    using NUnit.Framework;
    using Saga;
    using Scenarios;
    using TestFramework;
    using WindsorIntegration;


    public class Castle_Consumer :
        When_registering_a_consumer
    {
        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    public class Castle_Saga :
        When_registering_a_saga
    {
        [OneTimeTearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }

        protected override ISagaRepository<T> GetSagaRepository<T>()
        {
            return _container.Resolve<ISagaRepository<T>>();
        }
    }


    public class Test_Bus_Subscriptions_For_Consumers_In_Dummy_Saga_Using_Castle_As_IoC :
        Given_a_service_bus_instance
    {
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

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        [SetUp]
        public void Registering_a_dummy_saga()
        {
        }

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

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.LoadFrom(_container);
        }
    }


    public class Using_message_scope_with_two_consumers :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_receive_a_message_in_scope()
        {
            const string name = "Joe";

            await InputQueueSendEndpoint.Send(new SimpleMessageClass(name));

            var result = await Depedency.Completed.WithCancellation(TestCancellationToken);

            Console.WriteLine(result);
        }

        [TearDown]
        public void Close_container()
        {
            _container.Dispose();
        }

        readonly IWindsorContainer _container;

        public Using_message_scope_with_two_consumers()
        {
            _container = new WindsorContainer();
            _container.Register(
                Component.For<FirstConsumer>(),
                Component.For<SecondConsumer>(),
                Component.For<IScopedDependency>()
                    .ImplementedBy<Depedency>()
                    .LifestyleScoped<MessageScope>());
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.UseMessageScope();

            configurator.Consumer(new WindsorConsumerFactory<FirstConsumer>(_container.Kernel));
            configurator.Consumer(new WindsorConsumerFactory<SecondConsumer>(_container.Kernel));
        }


        public class FirstConsumer :
            IConsumer<SimpleMessageInterface>
        {
            readonly IScopedDependency _depedency;

            public FirstConsumer(IScopedDependency depedency)
            {
                _depedency = depedency;
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                _depedency.CompleteFirst();
            }
        }


        public class SecondConsumer :
            IConsumer<SimpleMessageInterface>
        {
            readonly IScopedDependency _depedency;

            public SecondConsumer(IScopedDependency depedency)
            {
                _depedency = depedency;
            }

            public async Task Consume(ConsumeContext<SimpleMessageInterface> context)
            {
                _depedency.CompleteSecond();
            }
        }


        public class Depedency :
            IScopedDependency
        {
            static TaskCompletionSource<string> _completed;
            TaskCompletionSource<string> _first;
            TaskCompletionSource<string> _second;

            static Depedency()
            {
                _completed = new TaskCompletionSource<string>();
            }

            public Depedency()
            {
                _first = new TaskCompletionSource<string>();
                _second = new TaskCompletionSource<string>();
            }

            public static Task<string> Completed => _completed.Task;

            public void CompleteFirst()
            {
                _first.TrySetResult("First");

                if (_second.Task.Status == TaskStatus.RanToCompletion)
                    _completed.TrySetResult(_second.Task.Result);
            }

            public void CompleteSecond()
            {
                _second.TrySetResult("Second");

                if (_first.Task.Status == TaskStatus.RanToCompletion)
                    _completed.TrySetResult(_first.Task.Result);
            }
        }


        public interface IScopedDependency
        {
            void CompleteFirst();
            void CompleteSecond();
        }
    }
}
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
namespace MassTransit.Tests.Distributor
{
    using System;
    using System.Linq;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.StateMachine;
    using Magnum.TestFramework;
    using MassTransit.Distributor.Messages;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class Using_the_distributor_with_a_state_machine_saga :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_deliver_a_published_message()
        {
            string businessId = Guid.NewGuid().ToString("N");

            Guid sagaId = NewId.NextGuid();

            Publish(new A {BusinessId = businessId, CorrelationId = sagaId});
            MySaga.FutureA.IsAvailable(800.Seconds()).ShouldBeTrue("Initiating message not received");

            Publish(new B { CorrelationId = sagaId });
            MySaga.FutureB.IsAvailable(800.Seconds()).ShouldBeTrue("Orchestrating message not received");

            Publish(new C { BusinessId = businessId });
            MySaga.FutureC.IsAvailable(800.Seconds()).ShouldBeTrue("Observed message not received");
        }

        [Test]
        public void Should_have_the_subscription_for_the_distributed_initiating()
        {
            LocalBus.HasSubscription<Distributed<A>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_distributed_observed()
        {
            LocalBus.HasSubscription<Distributed<C>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_distributed_orchestrating()
        {
            LocalBus.HasSubscription<Distributed<B>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_initiating()
        {
            LocalBus.HasSubscription<A>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_observed()
        {
            LocalBus.HasSubscription<C>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_orchestrating()
        {
            LocalBus.HasSubscription<B>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_ping_worker()
        {
            RemoteBus.HasSubscription<PingWorker>().Any()
                .ShouldBeTrue("PingWorker subscription was not found.");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability_A()
        {
            LocalBus.HasSubscription<WorkerAvailable<A>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability_B()
        {
            LocalBus.HasSubscription<WorkerAvailable<B>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability_C()
        {
            LocalBus.HasSubscription<WorkerAvailable<C>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        void Publish<T>(T message)
            where T : class
        {
            LocalBus.HasSubscription<T>().Any()
                .ShouldBeTrue("Message subscription was not found");
            LocalBus.HasSubscription<WorkerAvailable<T>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
            RemoteBus.HasSubscription<Distributed<T>>().Any()
                .ShouldBeTrue("Message subscription was not found");

            LocalBus.Endpoint.Send(message, context => { context.SendResponseTo(LocalBus); });
        }

        InMemorySagaRepository<MySaga> _sagaRepository;

        public Using_the_distributor_with_a_state_machine_saga()
        {
            _sagaRepository = new InMemorySagaRepository<MySaga>();
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.UseJsonSerializer();

            configurator.Distributor(x => x.Saga(_sagaRepository));
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.UseJsonSerializer();

            configurator.Worker(x => x.Saga(_sagaRepository));
        }

        class MySaga :
            SagaStateMachine<MySaga>,
            ISaga
        {
            public static FutureMessage<A> FutureA = new FutureMessage<A>();
            public static FutureMessage<B> FutureB = new FutureMessage<B>();
            public static FutureMessage<C> FutureC = new FutureMessage<C>();

            static MySaga()
            {
                Define(() =>
                    {
                        Correlate(FiredC).By((saga, message) => saga.BusinessId == message.BusinessId);

                        Initially(
                            When(FiredA)
                                .Then((saga, message) => saga.BusinessId = message.BusinessId)
                                .Then((saga, message) => FutureA.Set(message))
                                .TransitionTo(Running));

                        During(Running,
                            When(FiredB)
                                .Then((saga, message) => FutureB.Set(message)),
                            When(FiredC)
                                .Then((saga, message) => FutureC.Set(message))
                                .TransitionTo(Completed));
                    });
            }

            public MySaga(Guid correlationId)
                : this()
            {
                CorrelationId = correlationId;
            }

            public MySaga()
            {
            }

            public static Event<A> FiredA { get; set; }
            public static Event<B> FiredB { get; set; }
            public static Event<C> FiredC { get; set; }

            public static State Initial { get; set; }
            public static State Running { get; set; }
            public static State Completed { get; set; }

            public string BusinessId { get; set; }
            public Guid CorrelationId { get; set; }
            public IServiceBus Bus { get; set; }
        }

        class A :
            CorrelatedBy<Guid>
        {
            public string BusinessId { get; set; }
            public Guid CorrelationId { get; set; }
        }

        class B :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class C
        {
            public string BusinessId { get; set; }
        }
    }
}
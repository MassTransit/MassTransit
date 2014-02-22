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
    using System.Linq;
    using BusConfigurators;
    using Magnum.Extensions;
    using Magnum.TestFramework;
    using MassTransit.Distributor.Messages;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class Using_a_distributor_consumer :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_have_the_subscription_for_the_all_consumer()
        {
            LocalBus.HasSubscription<A>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_context_consumer()
        {
            LocalBus.HasSubscription<C>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_selected_consumer()
        {
            LocalBus.HasSubscription<B>().Any()
                .ShouldBeTrue("Message subscription was not found");
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
            LocalBus.HasSubscription<WorkerAvailable<A>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability_C()
        {
            LocalBus.HasSubscription<WorkerAvailable<A>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Distributor(x => x.Consumer<MyDistributorConsumer>());
        }

        class MyDistributorConsumer :
            Consumes<A>.All,
            Consumes<B>.All,
            Consumes<C>.Context
        {
            public static FutureMessage<A> FutureA = new FutureMessage<A>();
            public static FutureMessage<B> FutureB = new FutureMessage<B>();
            public static FutureMessage<C> FutureC = new FutureMessage<C>();

            public void Consume(A message)
            {
                FutureA.Set(message);
            }

            public void Consume(B message)
            {
                FutureB.Set(message);
            }

            public void Consume(IConsumeContext<C> context)
            {
                FutureC.Set(context.Message);
            }
        }

        class A
        {
        }

        class B
        {
        }

        class C
        {
        }
    }

    [TestFixture]
    public class Using_a_distributor_consumer_and_worker :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_deliver_a_published_message()
        {
            Publish<A>();
            Publish<C>();
            Publish<B>();

            MyDistributorConsumer.FutureA.IsAvailable(16.Seconds()).ShouldBeTrue("Missing regular consumer");
            MyDistributorConsumer.FutureC.IsAvailable(16.Seconds()).ShouldBeTrue("Missing context consumer");
            MyDistributorConsumer.FutureB.IsAvailable(16.Seconds()).ShouldBeTrue("Missing selective consumer");
        }

        void Publish<T>()
            where T : class, new()
        {
            LocalBus.HasSubscription<T>().Any()
                .ShouldBeTrue("Message subscription was not found");
            LocalBus.HasSubscription<WorkerAvailable<T>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
            RemoteBus.HasSubscription<Distributed<T>>().Any()
                .ShouldBeTrue("Message subscription was not found");

            var message = new T();
            LocalBus.Endpoint.Send(message, context => { context.SendResponseTo(LocalBus); });
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Distributor(x => x.Consumer<MyDistributorConsumer>());
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Worker(x => x.Consumer<MyDistributorConsumer>());
        }

        class MyDistributorConsumer :
            Consumes<A>.All,
            Consumes<B>.All,
            Consumes<C>.Context
        {
            public static FutureMessage<A> FutureA = new FutureMessage<A>();
            public static FutureMessage<B> FutureB = new FutureMessage<B>();
            public static FutureMessage<C> FutureC = new FutureMessage<C>();

            public void Consume(A message)
            {
                FutureA.Set(message);
            }

            public void Consume(B message)
            {
                FutureB.Set(message);
            }

            public void Consume(IConsumeContext<C> context)
            {
                FutureC.Set(context.Message);
            }
        }

        class A
        {
        }

        class B
        {
        }

        class C
        {
        }
    }

    [TestFixture]
    public class Using_a_worker_consumer :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_have_the_subscription_for_the_all_consumer()
        {
            LocalBus.HasSubscription<Distributed<A>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_context_consumer()
        {
            LocalBus.HasSubscription<Distributed<C>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_ping_worker()
        {
            RemoteBus.HasSubscription<PingWorker>().Any()
                .ShouldBeTrue("PingWorker subscription was not found.");
        }

        [Test]
        public void Should_have_the_subscription_for_the_selected_consumer()
        {
            LocalBus.HasSubscription<Distributed<B>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Worker(x => x.Consumer<MyDistributorConsumer>());
        }

        class MyDistributorConsumer :
            Consumes<A>.All,
            Consumes<B>.All,
            Consumes<C>.Context
        {
            public static FutureMessage<A> FutureA = new FutureMessage<A>();
            public static FutureMessage<B> FutureB = new FutureMessage<B>();
            public static FutureMessage<C> FutureC = new FutureMessage<C>();

            public void Consume(A message)
            {
                FutureA.Set(message);
            }

            public void Consume(B message)
            {
                FutureB.Set(message);
            }

            public void Consume(IConsumeContext<C> context)
            {
                FutureC.Set(context.Message);
            }
        }

        class A
        {
        }

        class B
        {
        }

        class C
        {
        }
    }
}
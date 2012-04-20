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
    using Magnum.TestFramework;
    using MassTransit.Distributor;
    using MassTransit.Distributor.DistributorConfigurators;
    using MassTransit.Distributor.Messages;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TextFixtures;

    [TestFixture]
    public class Using_a_distributor_handler :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_have_the_subscription_for_the_actual_message()
        {
            LocalBus.HasSubscription<A>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_worker_availability()
        {
            LocalBus.HasSubscription<WorkerAvailable<A>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Distributor(x => x.Handler<A>());
        }

        class A
        {
        }
    }

    [TestFixture]
    public class Using_a_worker_handler :
        LoopbackLocalAndRemoteTestFixture
    {
        [Test]
        public void Should_have_the_subscription_for_the_distributed_message()
        {
            RemoteBus.HasSubscription<Distributed<A>>().Any()
                .ShouldBeTrue("Message subscription was not found");
        }

        [Test]
        public void Should_have_the_subscription_for_the_ping_worker()
        {
            RemoteBus.HasSubscription<PingWorker>().Any()
                .ShouldBeTrue("PingWorker subscription was not found.");
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Worker(x => x.Handler<A>(message => { }));
        }

        class A
        {
        }
    }

    [TestFixture]
    public class Using_a_distributor_and_worker_handler :
        LoopbackLocalAndRemoteTestFixture
    {
        FutureMessage<A> _futureA = new FutureMessage<A>();

        [Test]
        public void Should_deliver_a_published_message()
        {
            var message = Publish<A>();

            _futureA.IsAvailable(8.Seconds()).ShouldBeTrue();

            _futureA.Message.CorrelationId.ShouldEqual(message.CorrelationId);
        }

        T Publish<T>()
            where T : class, new()
        {
            LocalBus.HasSubscription<T>().Any()
                .ShouldBeTrue("Message subscription was not found");
            LocalBus.HasSubscription<WorkerAvailable<T>>().Any()
                .ShouldBeTrue("Worker available subscription was not found.");
            RemoteBus.HasSubscription<Distributed<T>>().Any()
                .ShouldBeTrue("Message subscription was not found");

            T message = new T();
            LocalBus.Endpoint.Send(message, context =>
                {
                    context.SendResponseTo(LocalBus);
                });

            return message;
        }

        protected override void ConfigureLocalBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureLocalBus(configurator);

            configurator.Distributor(x => x.Handler<A>());
        }

        protected override void ConfigureRemoteBus(ServiceBusConfigurator configurator)
        {
            base.ConfigureRemoteBus(configurator);

            configurator.Worker(x => x.Handler<A>(message => { _futureA.Set(message); }));
        }

        class A
        {
            public A()
            {
                CorrelationId = Guid.NewGuid();
            }

            public Guid CorrelationId { get; private set; }
        }
    }
}
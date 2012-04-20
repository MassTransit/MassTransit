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
    using MassTransit.Distributor;
    using MassTransit.Saga;
    using NUnit.Framework;

    [TestFixture]
    public class NewConfiguration_Specs
    {
        [Test]
        public void Configuring_the_distributor_side()
        {
            IServiceBus bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("loopback://localhost/queue");

                    x.Distributor(d =>
                    {
                        d.Handler<A>()
                            .UseWorkerSelector(() => new LeastBusyWorkerSelectorFactory())
                            .UseWorkerSelector<LeastBusyWorkerSelectorFactory>();

                        d.Consumer<MyConsumer>();

                        d.Saga<MySaga>();
                    });

                });
        }

        [Test]
        public void Configuring_the_worker_side()
        {
            IServiceBus bus = ServiceBusFactory.New(x =>
                {
                    x.ReceiveFrom("loopback://localhost/queue");

                    x.Worker(w =>
                    {
                        w.Handler<A>(message => { });
                        w.Handler<A>((context, message) => { });

                        w.Consumer<MyConsumer>();
                        w.Consumer(() => new MyConsumer());
                        w.Consumer(typeof(MyConsumer), Activator.CreateInstance);

                        w.Saga(new InMemorySagaRepository<MySaga>());
                    });

                });
        }

        class MySaga :
            ISaga,
            InitiatedBy<A>,
            Orchestrates<B>
        {
            public MySaga(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public MySaga()
            {
            }

            public void Consume(A message)
            {
            }

            public Guid CorrelationId { get; set; }

            public IServiceBus Bus { get; set; }

            public void Consume(B message)
            {
            }
        }

        class MyConsumer :
            Consumes<A>.All
        {
            public void Consume(A message)
            {
            }
        }

        class A : CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }

        class B : CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
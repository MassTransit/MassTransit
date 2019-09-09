// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.Tests.AutomatonymousIntegration
{
    using System;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Automatonymous;
    using GreenPipes;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Partitioning_a_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_initiate_the_saga()
        {
            var timer = Stopwatch.StartNew();

            var ids = new Guid[Limit];
            for (var i = 0; i < Limit; i++)
            {
                var correlationId = NewId.NextGuid();
                await Bus.Publish(new CreateSaga {CorrelationId = correlationId});
                ids[i] = correlationId;
            }

            for (var i = 0; i < Limit; i++)
            {
                Guid? guid = await _repository.ShouldContainSaga(ids[i], TestTimeout);
                Assert.IsTrue(guid.HasValue);
            }

            timer.Stop();

            Console.WriteLine("Total time: {0}", timer.Elapsed);
        }

        InMemorySagaRepository<Instance> _repository;

        const int Limit = 100;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(new TestStateMachine(), _repository, cfg =>
            {
                cfg.Message<CreateSaga>(m => m.UsePartitioner(4, p => p.Message.CorrelationId));
                cfg.SagaMessage<CreateSaga>(m => m.Message(mx => mx.UseExecute(context =>
                {
                })));
            });
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(Created)
                        .Then(context =>
                        {
                        })
                        .TransitionTo(Waiting));
            }

            public State Waiting { get; private set; }
            public Event<CreateSaga> Created { get; private set; }
        }


        class CreateSaga :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
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
namespace MassTransit.Tests.AutomatonymousIntegration
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Triggering_a_change_event_from_a_transition :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_a_double_state()
        {
            Guid sagaId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new Start
            {
                CorrelationId = sagaId
            });

            Guid? saga = await _repository.ShouldContainSaga(x => x.CorrelationId == sagaId && Equals(x.CurrentState, _machine.RunningFaster), TestTimeout);
            Assert.IsTrue(saga.HasValue);

            Assert.AreEqual(1, _repository[saga.Value].Instance.OnEnter);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public Triggering_a_change_event_from_a_transition()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public int Counter { get; set; }
            public int OnEnter { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .Then(context =>
                        {
                            Console.WriteLine("Started:Then");
                            context.Instance.Counter = 1;
                        })
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());

                WhenEnter(Running, x => x.Then(context =>
                {
                    Console.WriteLine("Running.Enter:Then");
                    context.Instance.OnEnter = context.Instance.Counter;
                }).TransitionTo(RunningFaster));
            }

            public State Running { get; private set; }
            public State RunningFaster { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}
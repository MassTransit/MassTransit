// Copyright 2007-2014 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.AutomatonymousTests
{
    using System;
    using Automatonymous;
    using NUnit.Framework;
    using Saga;
    using TestFramework;


    [TestFixture]
    public class When_a_remove_expression_is_specified :
        InMemoryTestFixture
    {
        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public When_a_remove_expression_is_specified()
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
            public IServiceBus Bus { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                State(() => Running);
                Event(() => Started);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public State Running { get; private set; }
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


        [Test]
        public async void Should_handle_the_initial_state()
        {
            Guid sagaId = Guid.NewGuid();

            await Bus.Publish(new Start
            {
                CorrelationId = sagaId
            });


            Guid? saga =
                await _repository.ShouldContainSaga(x => x.CorrelationId == sagaId && x.CurrentState == _machine.Running, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async void Should_remove_the_saga_once_completed()
        {
            Guid sagaId = Guid.NewGuid();

            await Bus.Publish(new Start
            {
                CorrelationId = sagaId
            });

            Guid? saga = await _repository.ShouldContainSaga(sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Stop
            {
                CorrelationId = sagaId
            });

            saga = await _repository.ShouldNotContainSaga(sagaId, TestTimeout);
            Assert.IsFalse(saga.HasValue);
        }
    }
}
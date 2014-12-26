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
    using System.Threading.Tasks;
    using Automatonymous;
    using NUnit.Framework;
    using Saga;
    using TestFramework;


    [TestFixture]
    public class When_combining_events_into_a_single_event :
        InMemoryTestFixture
    {
        InMemorySagaRepository<Instance> _repository;

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;


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

            public CompositeEventStatus CompositeStatus { get; set; }
            public State CurrentState { get; set; }

            public Guid CorrelationId { get; private set; }
        }


        class TestStateMachine :
            AutomatonymousStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);
                State(() => Waiting);
                State(() => WaitingForSecond);

                Event(() => Start);
                Event(() => First);
                Event(() => Second);
                Event(() => Third, x => x.CompositeStatus, First, Second);

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(First)
                        .TransitionTo(WaitingForSecond));

                During(WaitingForSecond,
                    When(Third)
                        .Publish(context => new CompleteMessage(context.Instance.CorrelationId))
                        .Finalize());
            }

            public State Waiting { get; private set; }
            public State WaitingForSecond { get; private set; }

            public Event<StartMessage> Start { get; private set; }

            public Event<FirstMessage> First { get; private set; }
            public Event<SecondMessage> Second { get; private set; }
            public Event Third { get; private set; }
        }


        class StartMessage :
            CorrelatedBy<Guid>
        {
            public StartMessage()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Guid CorrelationId { get; set; }
        }


        class FirstMessage :
            CorrelatedBy<Guid>
        {
            public FirstMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class SecondMessage :
            CorrelatedBy<Guid>
        {
            public SecondMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class CompleteMessage :
            CorrelatedBy<Guid>
        {
            public CompleteMessage(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        [Test]
        public async void Should_have_called_combined_event()
        {
            var message = new StartMessage();

            Task<ConsumeContext<CompleteMessage>> received = SubscribeHandler<CompleteMessage>(x => x.Message.CorrelationId == message.CorrelationId);

            await Bus.Publish(message);
            Guid? saga = await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId
                && x.CurrentState == _machine.Waiting, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new FirstMessage(message.CorrelationId));
            saga = await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId
                && x.CurrentState == _machine.WaitingForSecond, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new SecondMessage(message.CorrelationId));

            await received;
        }
    }
}
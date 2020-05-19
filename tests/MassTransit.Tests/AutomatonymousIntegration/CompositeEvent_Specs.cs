// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
    using Automatonymous.Contexts;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_combining_events_into_a_single_event :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            var message = new StartMessage();

            Task<ConsumeContext<CompleteMessage>> received = ConnectPublishHandler<CompleteMessage>(x => x.Message.CorrelationId == message.CorrelationId);

            await Bus.Publish(message);

            Guid? saga = await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId
                && GetCurrentState(x).Result == _machine.Waiting, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new FirstMessage(message.CorrelationId));
            saga = await _repository.ShouldContainSaga(x => x.CorrelationId == message.CorrelationId
                && GetCurrentState(x).Result == _machine.WaitingForSecond, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new SecondMessage(message.CorrelationId));

            await received;
        }

        InMemorySagaRepository<Instance> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        TestStateMachine _machine;

        async Task<State> GetCurrentState(Instance state)
        {
            var context = new StateMachineInstanceContext<Instance>(state);

            return await _machine.GetState(context.Instance);
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

            public int CompositeStatus { get; set; }
            public int CurrentState { get; set; }

            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Second, x => x.CorrelateById(m => m.Message.CorrelationId));

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(First)
                        .TransitionTo(WaitingForSecond));

                CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

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


        class SecondMessage
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
    }
}
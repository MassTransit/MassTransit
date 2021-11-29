namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
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

            Task<ConsumeContext<CompleteMessage>> received = await ConnectPublishHandler<CompleteMessage>(x => x.Message.CorrelationId == message.CorrelationId);

            await Bus.Publish(message);

            Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, x => x.Waiting, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new FirstMessage(message.CorrelationId));
            saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, x => x.WaitingForSecond, TestTimeout);
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

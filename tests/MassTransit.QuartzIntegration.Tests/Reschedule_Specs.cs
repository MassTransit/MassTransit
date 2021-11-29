namespace MassTransit.QuartzIntegration.Tests
{
    namespace Reschedule_Specs
    {
        using System;
        using System.Threading.Tasks;
        using NUnit.Framework;
        using Saga;
        using Testing;


        [TestFixture]
        public class Rescheduling_a_message_from_a_state_machine :
            QuartzInMemoryTestFixture
        {
            [Test]
            public async Task Should_reschedule_the_message_with_a_new_token_id()
            {
                var correlationId = Guid.NewGuid();

                var startCommand = new StartCommand(correlationId);

                await InputQueueSendEndpoint.Send(startCommand);

                ConsumeContext<MessageRescheduled> rescheduledEvent = await _rescheduled;

                var sagaInstance = _repository[correlationId].Instance;

                Assert.NotNull(rescheduledEvent.Message.NewScheduleTokenId);
                Assert.AreEqual(sagaInstance.CorrelationId, rescheduledEvent.Message.CorrelationId);
                Assert.AreEqual(sagaInstance.ScheduleId, rescheduledEvent.Message.NewScheduleTokenId);

                await InputQueueSendEndpoint.Send(new StopCommand(correlationId));

                Guid? saga = await _repository.ShouldNotContainSaga(correlationId, TestTimeout);

                Assert.IsNull(saga);
            }

            InMemorySagaRepository<TestState> _repository;
            TestStateMachine _machine;
            Task<ConsumeContext<MessageRescheduled>> _rescheduled;

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                base.ConfigureInMemoryReceiveEndpoint(configurator);

                _repository = new InMemorySagaRepository<TestState>();
                _machine = new TestStateMachine();

                configurator.StateMachineSaga(_machine, _repository);

                _rescheduled = Handled<MessageRescheduled>(configurator);
            }
        }


        class Check
        {
            public Check(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class MessageRescheduled
        {
            public MessageRescheduled(Guid correlationId, Guid? newScheduleTokenId)
            {
                CorrelationId = correlationId;
                NewScheduleTokenId = newScheduleTokenId;
            }

            public Guid CorrelationId { get; set; }
            public Guid? NewScheduleTokenId { get; set; }
        }


        class StopCommand
        {
            public StopCommand(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class StartCommand
        {
            public StartCommand(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine : MassTransitStateMachine<TestState>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);
                Event(() => StartCommand, x =>
                {
                    x.CorrelateBy((s, m) => s.CorrelationId == m.Message.CorrelationId);
                    x.SelectId(m => m.Message.CorrelationId);
                });
                Event(() => StopCommand, x => x.CorrelateBy((s, m) => s.CorrelationId == m.Message.CorrelationId));

                Schedule(() => ScheduledMessage, x => x.ScheduleId,
                    x =>
                    {
                        x.Delay = TimeSpan.FromSeconds(2);
                        x.Received = e => e.CorrelateById(context => context.Message.CorrelationId);
                    });

                Initially(
                    When(StartCommand)
                        .Then(x => x.Instance.CorrelationId = x.Data.CorrelationId)
                        .TransitionTo(Active));

                WhenEnter(Active, binder => binder
                    .Schedule(ScheduledMessage, x => new Check(x.Instance.CorrelationId)));

                During(Active,
                    When(ScheduledMessage.Received)
                        .Schedule(ScheduledMessage, x => new Check(x.Instance.CorrelationId))
                        .Publish(x => new MessageRescheduled(x.Instance.CorrelationId, x.Instance.ScheduleId)),
                    When(StopCommand)
                        .Unschedule(ScheduledMessage)
                        .Finalize());
                SetCompletedWhenFinalized();
            }

            public Event<StartCommand> StartCommand { get; private set; }
            public Event<StopCommand> StopCommand { get; private set; }
            public Schedule<TestState, Check> ScheduledMessage { get; private set; }
            public State Active { get; private set; }
        }


        class TestState : SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public Guid? ScheduleId { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}

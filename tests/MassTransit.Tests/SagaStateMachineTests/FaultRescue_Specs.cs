namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_a_fault_handling_middleware_component_exists_on_the_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_be_received_as_a_fault_message()
        {
            var message = new Start();

            await InputQueueSendEndpoint.Send(message);

            Guid? saga = await _repository.ShouldContainSagaInState(message.CorrelationId, _machine, _machine.FailedToStart, TestTimeout);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository, x =>
            {
                IPipe<ExceptionSagaConsumeContext<Instance>> rescuePipe = Pipe.New<ExceptionSagaConsumeContext<Instance>>(p =>
                {
                    p.UseExecuteAsync(async context =>
                    {
                        //    await _machine.RaiseEvent(context.Saga, e => e.InlineStartFaulted, new StartFault(context.Saga.CorrelationId));
                    });
                });
                x.UseRescue(rescuePipe);
            });
        }

        TestStateMachine _machine;
        InMemorySagaRepository<Instance> _repository;


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
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => StartFaulted, x => x.CorrelateById(context => context.Message.Message.CorrelationId));

                Initially(
                    When(Started)
                        .TransitionTo(WaitingToStart)
                        .Then(context =>
                        {
                            throw new NotSupportedException("This is expected, but nonetheless exceptional");
                        })
                        .TransitionTo(Running));

                During(WaitingToStart,
                    When(StartFaulted)
                        .TransitionTo(FailedToStart),
                    When(InlineStartFaulted)
                        .TransitionTo(FailedToStart));
            }

            public State WaitingToStart { get; private set; }
            public State FailedToStart { get; private set; }
            public State Running { get; private set; }

            public Event<Start> Started { get; private set; }
            public Event<Fault<Start>> StartFaulted { get; private set; }
            public Event<StartFault> InlineStartFaulted { get; private set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Start()
            {
                CorrelationId = NewId.NextGuid();
            }

            public Start(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class StartFault :
            CorrelatedBy<Guid>
        {
            public StartFault()
            {
                CorrelationId = NewId.NextGuid();
            }

            public StartFault(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }
    }
}

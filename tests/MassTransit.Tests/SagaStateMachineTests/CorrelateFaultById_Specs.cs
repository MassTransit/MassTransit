namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class When_a_state_machine_fault_event_is_correlated_by_id
    {
        [Test]
        public async Task Should_not_require_explicit_configuration()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSagaStateMachine<TestStateMachine, Instance>();
                }).BuildServiceProvider(true);


            var harness = provider.GetTestHarness();

            await harness.Start();
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
                Initially(
                    When(CorrelatedEvent)
                        .TransitionTo(Running),
                    When(CorrelatedEventFault)
                        .TransitionTo(NotRunning),
                    When(UncorrelatedEvent)
                        .TransitionTo(Running),
                    When(UncorrelatedEventFault)
                        .TransitionTo(NotRunning)
                    );
            }

            public State Running { get; }
            public State NotRunning { get; }

            public Event<CorrelatedEvent> CorrelatedEvent { get; }
            public Event<Fault<CorrelatedEvent>> CorrelatedEventFault { get; }

            public Event<UncorrelatedEvent> UncorrelatedEvent { get; }
            public Event<Fault<UncorrelatedEvent>> UncorrelatedEventFault { get; }
        }


        class CorrelatedEvent :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class UncorrelatedEvent
        {
            public Guid CorrelationId { get; set; }
        }
    }
}

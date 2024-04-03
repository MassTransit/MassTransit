namespace MassTransit.Tests.SagaStateMachineTests
{
    using System.Threading.Tasks;
    using BaseStateMachineTestSubjects;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_base_state_machine
    {
        [Test]
        public async Task Should_initialize_all_states_and_events()
        {
            await using var provider = new ServiceCollection()
                .AddMassTransitTestHarness(x =>
                {
                    x.AddSagaStateMachine<HappyGoLuckyStateMachine, HappyGoLuckyState>();
                })
                .BuildServiceProvider(true);

            var harness = await provider.StartTestHarness();

            var id = NewId.NextGuid();

            await harness.Bus.Publish(new HappyEvent(id));

            Assert.That(await harness.Consumed.Any<HappyEvent>());

            await harness.Bus.Publish(new EndItAllEvent(id));

            Assert.That(await harness.Consumed.Any<EndItAllEvent>());
        }
    }


    namespace BaseStateMachineTestSubjects
    {
        using System;


        public class HappyEvent
        {
            public HappyEvent(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class GoLuckyEvent
        {
            public GoLuckyEvent(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class EndItAllEvent
        {
            public EndItAllEvent(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            public Guid CorrelationId { get; set; }
        }


        public class CommonStateMachine<T> :
            MassTransitStateMachine<T>
            where T : class, SagaStateMachineInstance
        {
            //
            // ReSharper disable UnassignedGetOnlyAutoProperty
            public State Happy { get; }
            public State GoLucky { get; }

            public Event<HappyEvent> OnHappy { get; }
            public Event<GoLuckyEvent> OnGoLucky { get; }
        }


        public class HappyGoLuckyState :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class HappyGoLuckyStateMachine :
            CommonStateMachine<HappyGoLuckyState>
        {
            public HappyGoLuckyStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Initially(
                    When(OnHappy)
                        .TransitionTo(Happy),
                    When(OnGoLucky)
                        .TransitionTo(GoLucky));

                During(Happy, GoLucky,
                    When(OnEndItAll)
                        .TransitionTo(Finished));

                SetCompletedWhenFinalized();
            }

            public State Finished { get; }

            public Event<EndItAllEvent> OnEndItAll { get; }
        }
    }
}

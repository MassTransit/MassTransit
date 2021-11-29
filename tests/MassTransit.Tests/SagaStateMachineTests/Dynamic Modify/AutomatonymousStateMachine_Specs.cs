namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Linq;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Using_a_simple_state_machine
    {
        [Test]
        public void Should_initialize_inherited_final_state_property()
        {
            var stateMachine = CreateStateMachine();

            Assert.That(stateMachine.Final, Is.Not.Null);
        }

        [Test]
        public void Should_initialize_inherited_initial_state_property()
        {
            var stateMachine = CreateStateMachine();

            Assert.That(stateMachine.Initial, Is.Not.Null);
        }

        [Test]
        public void Should_register_all_events()
        {
            var stateMachine = CreateStateMachine();

            var events = stateMachine.Events.ToList();

            Assert.That(events, Has.Count.EqualTo(2));
        }

        [Test]
        public void Should_register_all_states()
        {
            var stateMachine = CreateStateMachine();

            var states = stateMachine.States.ToList();

            Assert.That(states, Has.Count.EqualTo(3));
        }

        [Test]
        public void Should_register_declared_state()
        {
            var stateMachine = CreateStateMachine();

            Assert.That(stateMachine.States, Contains.Item(ThisIsAState));
        }

        [Test]
        public void Should_register_generic_event()
        {
            var stateMachine = CreateStateMachine();

            Assert.That(stateMachine.Events, Contains.Item(ThisIsAnEventConsumingData));
        }

        [Test]
        public void Should_register_inherited_final_state_property()
        {
            var stateMachine = CreateStateMachine();

            Assert.That(stateMachine.States, Contains.Item(stateMachine.Final));
        }

        [Test]
        public void Should_register_inherited_initial_state_property()
        {
            var stateMachine = CreateStateMachine();

            Assert.That(stateMachine.States, Contains.Item(stateMachine.Initial));
        }

        [Test]
        public void Should_register_simple_event()
        {
            var stateMachine = CreateStateMachine();

            Assert.That(stateMachine.Events, Contains.Item(ThisIsASimpleEvent));
        }

        State ThisIsAState;
        Event ThisIsASimpleEvent;
        Event<EventData> ThisIsAnEventConsumingData;


        class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
        }


        class EventData
        {
        }


        private StateMachine<Instance> CreateStateMachine() =>
            MassTransitStateMachine<Instance>.New(builder => builder
                .State("ThisIsAState", out ThisIsAState)
                .Event("ThisIsASimpleEvent", out ThisIsASimpleEvent)
                .Event("ThisIsAnEventConsumingData", out ThisIsAnEventConsumingData)
            );
    }
}

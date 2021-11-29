namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Linq;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_simple_state_machine
    {
        [Test]
        public void Should_initialize_declared_state_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.ThisIsAState, Is.Not.Null);
        }

        [Test]
        public void Should_initialize_generic_event_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.ThisIsAnEventConsumingData, Is.Not.Null);
        }

        [Test]
        public void Should_initialize_inherited_final_state_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.Final, Is.Not.Null);
        }

        [Test]
        public void Should_initialize_inherited_initial_state_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.Initial, Is.Not.Null);
        }

        [Test]
        public void Should_initialize_simple_event_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.ThisIsASimpleEvent, Is.Not.Null);
        }

        [Test]
        public void Should_register_all_event_properties()
        {
            var stateMachine = new TestStateMachine();

            var events = stateMachine.Events.ToList();

            Assert.That(events, Has.Count.EqualTo(2));
        }

        [Test]
        public void Should_register_all_state_properties()
        {
            var stateMachine = new TestStateMachine();

            var states = stateMachine.States.ToList();

            Assert.That(states, Has.Count.EqualTo(3));
        }

        [Test]
        public void Should_register_declared_state_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.States, Contains.Item(stateMachine.ThisIsAState));
        }

        [Test]
        public void Should_register_generic_event_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.Events, Contains.Item(stateMachine.ThisIsAnEventConsumingData));
        }

        [Test]
        public void Should_register_inherited_final_state_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.States, Contains.Item(stateMachine.Final));
        }

        [Test]
        public void Should_register_inherited_initial_state_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.States, Contains.Item(stateMachine.Initial));
        }

        [Test]
        public void Should_register_simple_event_property()
        {
            var stateMachine = new TestStateMachine();

            Assert.That(stateMachine.Events, Contains.Item(stateMachine.ThisIsASimpleEvent));
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
        }


        class EventData
        {
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            // ReSharper disable UnassignedGetOnlyAutoProperty
            public State ThisIsAState { get; }
            public Event ThisIsASimpleEvent { get; }
            public Event<EventData> ThisIsAnEventConsumingData { get; }
        }
    }
}

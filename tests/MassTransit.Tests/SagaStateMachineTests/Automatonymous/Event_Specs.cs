namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;
    using SagaStateMachine;


    [TestFixture]
    public class When_an_event_is_declared
    {
        [Test]
        public void It_should_capture_a_simple_event_name()
        {
            Assert.That(_machine.Hello.Name, Is.EqualTo("Hello"));
        }

        [Test]
        public void It_should_capture_the_data_event_name()
        {
            Assert.That(_machine.EventA.Name, Is.EqualTo("EventA"));
        }

        [Test]
        public void It_should_create_the_proper_event_type_for_data_events()
        {
            Assert.That(_machine.EventA, Is.InstanceOf<MessageEvent<A>>());
        }

        [Test]
        public void It_should_create_the_proper_event_type_for_simple_events()
        {
            Assert.That(_machine.Hello, Is.InstanceOf<TriggerEvent>());
        }

        TestStateMachine _machine;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
        }


        class A
        {
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public Event Hello { get; private set; }
            public Event<A> EventA { get; private set; }
        }
    }
}

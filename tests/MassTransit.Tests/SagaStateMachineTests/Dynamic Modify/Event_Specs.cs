namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;
    using SagaStateMachine;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_an_event_is_declared
    {
        [Test]
        public void It_should_capture_a_simple_event_name()
        {
            Assert.AreEqual("Hello", Hello.Name);
        }

        [Test]
        public void It_should_capture_the_data_event_name()
        {
            Assert.AreEqual("EventA", EventA.Name);
        }

        [Test]
        public void It_should_create_the_proper_event_type_for_data_events()
        {
            Assert.IsInstanceOf<MessageEvent<A>>(EventA);
        }

        [Test]
        public void It_should_create_the_proper_event_type_for_simple_events()
        {
            Assert.IsInstanceOf<TriggerEvent>(Hello);
        }

        Event Hello;
        Event<A> EventA;
        StateMachine<Instance> _machine;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .Event("Hello", out Hello)
                    .Event("EventA", out EventA)
                );
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

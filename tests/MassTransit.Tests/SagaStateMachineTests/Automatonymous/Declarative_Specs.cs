namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class When_an_instance_has_multiple_states
    {
        [Test]
        public void Should_handle_both_states()
        {
            Assert.AreEqual(_top.Greeted, _instance.Top);
            Assert.AreEqual(_bottom.Ignored, _instance.Bottom);
        }

        MyState _instance;
        TopInstanceStateMachine _top;
        BottomInstanceStateMachine _bottom;

        [OneTimeSetUp]
        public void Specifying_an_event_activity_with_data()
        {
            _instance = new MyState();

            _top = new TopInstanceStateMachine();
            _bottom = new BottomInstanceStateMachine();

            _top.RaiseEvent(_instance, _top.Initialized, new Init
            {
                Value = "Hello"
            }).Wait();

            _bottom.RaiseEvent(_instance, _bottom.Initialized, new Init
            {
                Value = "Goodbye"
            }).Wait();
        }


        class MyState :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State Top { get; set; }
            public State Bottom { get; set; }
        }


        class Init
        {
            public string Value { get; set; }
        }


        class TopInstanceStateMachine :
            MassTransitStateMachine<MyState>
        {
            public TopInstanceStateMachine()
            {
                InstanceState(x => x.Top);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Greeted));
            }

            public State Greeted { get; private set; }

            public Event<Init> Initialized { get; private set; }
        }


        class BottomInstanceStateMachine :
            MassTransitStateMachine<MyState>
        {
            public BottomInstanceStateMachine()
            {
                InstanceState(x => x.Bottom);

                State(() => Ignored);

                Event(() => Initialized);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Ignored));
            }

            public State Ignored { get; private set; }

            public Event<Init> Initialized { get; private set; }
        }
    }
}

namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_an_instance_has_multiple_states
    {
        [Test]
        public void Should_handle_both_states()
        {
            Assert.AreEqual(TopGreeted, _instance.Top);
            Assert.AreEqual(BottomIgnored, _instance.Bottom);
        }

        State TopGreeted;
        Event<Init> TopInitialized;
        State BottomIgnored;
        Event<Init> BottomInitialized;

        MyState _instance;
        StateMachine<MyState> _top;
        StateMachine<MyState> _bottom;

        [OneTimeSetUp]
        public void Specifying_an_event_activity_with_data()
        {
            _instance = new MyState();

            _top = MassTransitStateMachine<MyState>
                .New(builder => builder
                    .State("Greeted", out TopGreeted)
                    .Event("Initialized", out TopInitialized)
                    .InstanceState(b => b.Top)
                    .During(builder.Initial)
                        .When(TopInitialized, b => b.TransitionTo(TopGreeted))
                );
            _bottom = MassTransitStateMachine<MyState>
                .New(builder => builder
                    .State("Ignored", out BottomIgnored)
                    .Event("Initialized", out BottomInitialized)
                    .InstanceState(b => b.Bottom)
                    .During(builder.Initial)
                        .When(BottomInitialized, b => b.TransitionTo(BottomIgnored))
                );

            _top.RaiseEvent(_instance, TopInitialized, new Init
            {
                Value = "Hello"
            }).Wait();

            _bottom.RaiseEvent(_instance, BottomInitialized, new Init
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
    }
}

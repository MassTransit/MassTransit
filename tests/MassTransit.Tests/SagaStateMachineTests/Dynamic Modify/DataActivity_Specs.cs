namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_specifying_an_event_activity_with_data
    {
        [Test]
        public void Should_have_the_proper_value()
        {
            Assert.That(_instance.Value, Is.EqualTo("Hello"));
        }

        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.That(_instance.CurrentState, Is.EqualTo(Running));
        }

        State Running;
        Event<Init> Initialized;

        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity_with_data()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .During(builder.Initial)
                    .When(Initialized, b => b
                        .Then(context => context.Instance.Value = context.Data.Value)
                        .TransitionTo(Running)
                    )
                );

            _machine.RaiseEvent(_instance, Initialized, new Init { Value = "Hello" }).Wait();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public string Value { get; set; }
            public int OtherValue { get; set; }
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class Init
        {
            public string Value { get; set; }
        }
    }
}

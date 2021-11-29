namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Anytime_events
    {
        [Test]
        public async Task Should_be_called_regardless_of_state()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, Init);
            await _machine.RaiseEvent(instance, Hello);

            Assert.IsTrue(instance.HelloCalled);
            Assert.AreEqual(_machine.Final, instance.CurrentState);
        }

        [Test]
        public async Task Should_have_value_of_event_data()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, Init);
            await _machine.RaiseEvent(instance, EventA, new A
            {
                Value = "Test"
            });

            Assert.AreEqual("Test", instance.AValue);
            Assert.AreEqual(_machine.Final, instance.CurrentState);
        }

        [Test]
        public void Should_not_be_handled_on_initial()
        {
            var instance = new Instance();

            Assert.That(async () => await _machine.RaiseEvent(instance, Hello), Throws.TypeOf<UnhandledEventException>());

            Assert.IsFalse(instance.HelloCalled);
            Assert.AreEqual(_machine.Initial, instance.CurrentState);
        }

        State Ready;
        Event Init;
        Event Hello;
        Event<A> EventA;

        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Ready", out Ready)
                    .Event("Init", out Init)
                    .Event("Hello", out Hello)
                    .Event("EventA", out EventA)
                    .Initially()
                        .When(Init, b => b.TransitionTo(Ready))
                    .DuringAny()
                        .When(Hello, b => b
                            .Then(context => context.Instance.HelloCalled = true)
                            .Finalize()
                        )
                        .When(EventA, b => b
                            .Then(context => context.Instance.AValue = context.Data.Value)
                            .Finalize()
                        )
                );
        }

        class A
        {
            public string Value { get; set; }
        }

        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public bool HelloCalled { get; set; }
            public string AValue { get; set; }
            public State CurrentState { get; set; }
        }
    }
}

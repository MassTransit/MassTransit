namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Anytime_events
    {
        [Test]
        public async Task Should_be_called_regardless_of_state()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Init);
            await _machine.RaiseEvent(instance, x => x.Hello);

            Assert.IsTrue(instance.HelloCalled);
            Assert.AreEqual(_machine.Final, instance.CurrentState);
        }

        [Test]
        public async Task Should_have_value_of_event_data()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Init);
            await _machine.RaiseEvent(instance, x => x.EventA, new A
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

            Assert.That(async () => await _machine.RaiseEvent(instance, x => x.Hello), Throws.TypeOf<UnhandledEventException>());

            Assert.IsFalse(instance.HelloCalled);
            Assert.AreEqual(_machine.Initial, instance.CurrentState);
        }

        TestStateMachine _machine;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
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


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Init)
                        .TransitionTo(Ready));

                DuringAny(
                    When(Hello)
                        .Then(context => context.Instance.HelloCalled = true)
                        .Finalize(),
                    When(EventA)
                        .Then(context => context.Instance.AValue = context.Data.Value)
                        .Finalize());
            }

            public State Ready { get; private set; }

            public Event Init { get; private set; }
            public Event Hello { get; private set; }
            public Event<A> EventA { get; private set; }
        }
    }
}

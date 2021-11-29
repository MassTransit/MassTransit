namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Raising_an_unhandled_event_in_a_state
    {
        [Test]
        public async Task Should_throw_an_exception_when_event_is_not_allowed_in_current_state()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Start);

            Assert.That(async () => await _machine.RaiseEvent(instance, x => x.Start), Throws.TypeOf<UnhandledEventException>());
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


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Start)
                        .TransitionTo(Running));
            }

            public Event Start { get; private set; }

            public State Running { get; private set; }
        }
    }


    [TestFixture]
    public class Raising_an_ignored_event_that_is_not_filtered
    {
        [Test]
        public async Task Should_throw_an_exception_when_event_is_not_allowed_in_current_state()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Start);

            Assert.That(async () => await _machine.RaiseEvent(instance, x => x.Charge, new A { Volts = 12 }),
                Throws.TypeOf<UnhandledEventException>());
        }

        TestStateMachine _machine;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public int Volts { get; set; }
            public Guid CorrelationId { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Start)
                        .TransitionTo(Running));

                During(Running,
                    Ignore(Start),
                    Ignore(Charge, x => x.Data.Volts == 9));
            }

            public Event Start { get; private set; }
            public Event<A> Charge { get; private set; }

            public State Running { get; private set; }
        }


        class A
        {
            public int Volts { get; set; }
        }
    }


    [TestFixture]
    public class Raising_an_ignored_event
    {
        [Test]
        public async Task Should_also_ignore_yet_process_invalid_events()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Start);

            await _machine.RaiseEvent(instance, x => x.Charge, new A { Volts = 12 });

            Assert.AreEqual(0, instance.Volts);
        }

        [Test]
        public async Task Should_have_the_next_event_even_though_ignored()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Start);

            Assert.AreEqual(_machine.Running, await _machine.GetState(instance));

            var nextEvents = await _machine.NextEvents(instance);

            Assert.IsTrue(nextEvents.Any(x => x.Name.Equals("Charge")));
        }

        [Test]
        public async Task Should_silently_ignore_the_invalid_event()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Start);

            await _machine.RaiseEvent(instance, x => x.Start);
        }

        TestStateMachine _machine;


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public int Volts { get; set; }
            public Guid CorrelationId { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = new TestStateMachine();
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Start)
                        .TransitionTo(Running));

                During(Running,
                    Ignore(Start),
                    Ignore(Charge));
            }

            public Event Start { get; private set; }
            public Event<A> Charge { get; private set; }

            public State Running { get; private set; }
        }


        class A
        {
            public int Volts { get; set; }
        }
    }


    [TestFixture]
    public class Raising_an_unhandled_event_when_the_state_machine_ignores_all_unhandled_events
    {
        [Test]
        public async Task Should_silenty_ignore_the_invalid_event()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => x.Start);

            await _machine.RaiseEvent(instance, x => x.Start);
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


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Event(() => Start);

                State(() => Running);

                OnUnhandledEvent(x => x.Ignore());

                Initially(
                    When(Start)
                        .TransitionTo(Running));
            }

            public Event Start { get; private set; }

            public State Running { get; private set; }
        }
    }
}

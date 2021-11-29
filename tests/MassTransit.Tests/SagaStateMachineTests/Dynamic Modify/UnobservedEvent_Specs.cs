namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Automatonymous;
    using Introspection;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Raising_an_unhandled_event_in_a_state
    {
        [Test]
        public async Task Should_throw_an_exception_when_event_is_not_allowed_in_current_state()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, x => Start);

            Assert.That(async () => await _machine.RaiseEvent(instance, x => Start), Throws.TypeOf<UnhandledEventException>());
        }

        StateMachine<Instance> _machine;
        Event Start;


        class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out State Running)
                    .Event("Start", out Start)
                    .Initially()
                    .When(Start, b => b.TransitionTo(Running))
                );
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class Raising_an_ignored_event_that_is_not_filtered
    {
        [Test]
        public async Task Should_throw_an_exception_when_event_is_not_allowed_in_current_state()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, Start);

            Assert.That(async () => await _machine.RaiseEvent(instance, Charge, new A { Volts = 12 }),
                Throws.TypeOf<UnhandledEventException>());
        }

        Event Start;
        Event<A> Charge;
        StateMachine<Instance> _machine;


        class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public int Volts { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out State Running)
                    .Event("Start", out Start)
                    .Event("Charge", out Charge)
                    .Initially()
                    .When(Start, b => b.TransitionTo(Running))
                    .During(Running)
                    .Ignore(Start)
                    .Ignore(Charge, x => x.Data.Volts == 9)
                );
        }


        class A
        {
            public int Volts { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class Raising_an_ignored_event
    {
        [Test]
        public async Task Should_also_ignore_yet_process_invalid_events()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, Start);

            await _machine.RaiseEvent(instance, Charge, new A { Volts = 12 });

            Assert.AreEqual(0, instance.Volts);
        }

        [Test]
        public async Task Should_have_the_next_event_even_though_ignored()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, Start);

            Assert.AreEqual(Running, await _machine.GetState(instance));

            var nextEvents = await _machine.NextEvents(instance);

            Assert.IsTrue(nextEvents.Any(x => x.Name.Equals("Charge")));
        }

        [Test]
        public async Task Should_silently_ignore_the_invalid_event()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, Start);

            await _machine.RaiseEvent(instance, Start);
        }

        State Running;
        Event Start;
        Event<A> Charge;
        StateMachine<Instance> _machine;


        class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public int Volts { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Start", out Start)
                    .Event("Charge", out Charge)
                    .Initially()
                    .When(Start, b => b.TransitionTo(Running))
                    .During(Running)
                    .Ignore(Start)
                    .Ignore(Charge)
                );
        }


        class A
        {
            public int Volts { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class Raising_an_unhandled_event_when_the_state_machine_ignores_all_unhandled_events
    {
        [Test]
        public async Task Should_silenty_ignore_the_invalid_event()
        {
            var instance = new Instance();

            await _machine.RaiseEvent(instance, Start);

            await _machine.RaiseEvent(instance, Start);
        }

        Event Start;
        StateMachine<Instance> _machine;


        class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
        }


        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .Event("Start", out Start)
                    .State("Running", out State Running)
                    .OnUnhandledEvent(x => x.Ignore())
                    .Initially()
                    .When(Start, b => b.TransitionTo(Running))
                );
        }
    }
}

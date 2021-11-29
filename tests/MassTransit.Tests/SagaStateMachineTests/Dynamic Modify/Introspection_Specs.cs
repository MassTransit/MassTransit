namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Introspection_Specs
    {
        [Test]
        public void The_machine_should_expose_all_events()
        {
            List<Event> events = _machine.Events.ToList();

            Assert.AreEqual(4, events.Count);
            Assert.Contains(Ignored, events);
            Assert.Contains(Handshake, events);
            Assert.Contains(Hello, events);
            Assert.Contains(YelledAt, events);
        }

        [Test]
        public void The_machine_should_expose_all_states()
        {
            Assert.AreEqual(5, _machine.States.Count());
            Assert.Contains(_machine.Initial, _machine.States.ToList());
            Assert.Contains(_machine.Final, _machine.States.ToList());
            Assert.Contains(Greeted, _machine.States.ToList());
            Assert.Contains(Loved, _machine.States.ToList());
            Assert.Contains(Pissed, _machine.States.ToList());
        }

        [Test]
        public void The_machine_should_report_its_instance_type()
        {
            Assert.AreEqual(typeof(Instance), _machine.InstanceType);
        }

        [Test]
        public async Task The_next_events_should_be_known()
        {
            List<Event> events = (await _machine.NextEvents(_instance)).ToList();
            Assert.AreEqual(3, events.Count);
        }

        Event<B> Ignored;
        Event<A> Handshake;
        Event Hello;
        Event YelledAt;
        State Greeted;
        State Pissed;
        State Loved;
        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .Event("Ignored", out Ignored)
                    .Event("Handshake", out Handshake)
                    .Event("Hello", out Hello)
                    .Event("YelledAt", out YelledAt)
                    .State("Greeted", out Greeted)
                    .State("Pissed", out Pissed)
                    .State("Loved", out Loved)
                    .Initially()
                    .When(Hello, b => b.TransitionTo(Greeted))
                    .During(Greeted)
                    .When(Handshake, b => b.TransitionTo(Loved))
                    .When(Ignored, b => b.TransitionTo(Pissed))
                    .WhenEnter(Greeted, b => b.Then(context =>
                    {
                    }))
                    .DuringAny()
                    .When(YelledAt, b => b.TransitionTo(builder.Final))
                );

            _machine.RaiseEvent(_instance, Hello);
        }


        class A
        {
        }


        class B
        {
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                Initially(
                    When(Hello)
                        .TransitionTo(Greeted));

                During(Greeted,
                    When(Handshake)
                        .TransitionTo(Loved),
                    When(Ignored)
                        .TransitionTo(Pissed));

                WhenEnter(Greeted, x => x.Then(context =>
                {
                }));

                DuringAny(When(YelledAt).TransitionTo(Final));
            }

            public State Greeted { get; set; }
            public State Pissed { get; set; }
            public State Loved { get; set; }

            public Event Hello { get; private set; }
            public Event YelledAt { get; private set; }
            public Event<A> Handshake { get; private set; }
            public Event<B> Ignored { get; private set; }
        }
    }
}

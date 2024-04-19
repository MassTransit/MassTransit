namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Introspection_Specs
    {
        [Test]
        public void The_machine_shoud_report_its_instance_type()
        {
            Assert.That(((StateMachine<Instance>)_machine).InstanceType, Is.EqualTo(typeof(Instance)));
        }

        [Test]
        public void The_machine_should_expose_all_events()
        {
            List<Event> events = _machine.Events.ToList();

            Assert.That(events, Has.Count.EqualTo(4));
            Assert.That(events, Does.Contain(_machine.Ignored));
            Assert.That(events, Does.Contain(_machine.Handshake));
            Assert.That(events, Does.Contain(_machine.Hello));
            Assert.That(events, Does.Contain(_machine.YelledAt));
        }

        [Test]
        public void The_machine_should_expose_all_states()
        {
            Assert.Multiple(() =>
            {
                Assert.That(((StateMachine)_machine).States.Count(), Is.EqualTo(5));
                Assert.That(_machine.States.ToList(), Does.Contain(_machine.Initial));
            });
            Assert.That(_machine.States.ToList(), Does.Contain(_machine.Final));
            Assert.That(_machine.States.ToList(), Does.Contain(_machine.Greeted));
            Assert.That(_machine.States.ToList(), Does.Contain(_machine.Loved));
            Assert.That(_machine.States.ToList(), Does.Contain(_machine.Pissed));
        }

        [Test]
        public async Task The_next_events_should_be_known()
        {
            List<Event> events = (await _machine.NextEvents(_instance)).ToList();
            Assert.That(events, Has.Count.EqualTo(3));
        }

        Instance _instance;
        TestStateMachine _machine;

        [OneTimeSetUp]
        public void A_state_is_declared()
        {
            _instance = new Instance();
            _machine = new TestStateMachine();

            _machine.RaiseEvent(_instance, _machine.Hello);
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

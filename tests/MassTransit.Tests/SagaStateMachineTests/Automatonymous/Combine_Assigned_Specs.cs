namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_combining_events_into_a_single_event_assigned_to_state
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.Called, Is.True);
                Assert.That(_machine.NextEvents(_instance.CurrentState), Is.Empty);
            });
        }

        [Test]
        public async Task Should_have_correct_events()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            Assert.Multiple(() =>
            {
                Assert.That(_machine.NextEvents(_machine.Initial).Count(), Is.EqualTo(1));
                Assert.That(_machine.NextEvents(_machine.Waiting).Count(), Is.EqualTo(3));
                Assert.That(_machine.NextEvents(_machine.Final).Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.That(_instance.Called, Is.False);

            Event[] events = _machine.NextEvents(_instance.CurrentState).ToArray();

            Assert.Multiple(() =>
            {
                Assert.That(events, Has.Length.EqualTo(3));
                Assert.That(events.Count(e => !_machine.IsCompositeEvent(e)), Is.EqualTo(2));
            });
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.Called, Is.False);
                Assert.That(_machine.NextEvents(_instance.CurrentState).Count(), Is.EqualTo(3));
            });
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance :
            SagaStateMachineInstance
        {
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(Third)
                        .Then(context => context.Instance.Called = true)
                        .Finalize());
            }

            public State Waiting { get; private set; }

            public Event Start { get; private set; }

            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }
    }


    [TestFixture]
    public class When_combining_events_with_an_int_for_state_assigned_to_state
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.That(_instance.Called, Is.False);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.Called, Is.True);

                Assert.That(_instance.CurrentState, Is.EqualTo(2));
                Assert.That(_machine.NextEvents(_machine.GetState("Final")), Is.Empty);
            });
        }

        [Test]
        public async Task Should_have_correct_events()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            Assert.Multiple(() =>
            {
                Assert.That(_machine.NextEvents(_machine.Initial).Count(), Is.EqualTo(1));
                Assert.That(_machine.NextEvents(_machine.GetState("Initial")).Count(), Is.EqualTo(1));
                Assert.That(_machine.NextEvents(_machine.Waiting).Count(), Is.EqualTo(3));
                Assert.That(_machine.NextEvents(_machine.GetState("Waiting")).Count(), Is.EqualTo(3));
                Assert.That(_machine.NextEvents(_machine.Final).Count(), Is.EqualTo(0));
                Assert.That(_machine.NextEvents(_machine.GetState("Final")).Count(), Is.EqualTo(0));
            });
        }

        [Test]
        public async Task Should_have_initial_state_with_zero()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.CurrentState, Is.EqualTo(3));
                Assert.That(_machine.NextEvents(_machine.GetState("Final")), Is.Empty);
            });
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.Called, Is.False);
                Assert.That(_machine.NextEvents(_machine.GetState("Final")), Is.Empty);
            });
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.Multiple(() =>
            {
                Assert.That(_instance.Called, Is.False);
                Assert.That(_machine.NextEvents(_machine.GetState("Final")), Is.Empty);
            });
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance : SagaStateMachineInstance
        {
            public int CompositeStatus { get; set; }
            public bool Called { get; set; }
            public int CurrentState { get; set; }

            public bool CalledFirst { get; set; }
            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                InstanceState(x => x.CurrentState);

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(First)
                        .Then(context => context.Instance.CalledFirst = true),
                    When(Third)
                        .Then(context => context.Instance.Called = context.Instance.CalledFirst)
                        .Finalize());
            }

            public State Waiting { get; private set; }

            public Event Start { get; private set; }

            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }
    }
}

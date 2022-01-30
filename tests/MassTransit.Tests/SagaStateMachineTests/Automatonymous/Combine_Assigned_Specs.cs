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

            Assert.IsTrue(_instance.Called);
            Assert.IsEmpty(_machine.NextEvents(_instance.CurrentState));
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);

            var events = _machine.NextEvents(_instance.CurrentState);

            Assert.AreEqual(3, events.Count());
            Assert.AreEqual(2, events.Count(e => !_machine.IsCompositeEvent(e)));
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
            Assert.AreEqual(3, _machine.NextEvents(_instance.CurrentState).Count());
        }

        [Test]
        public async Task Should_have_correct_events()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, _machine.NextEvents(_machine.Initial).Count());
                Assert.AreEqual(3, _machine.NextEvents(_machine.Waiting).Count());
                Assert.AreEqual(0, _machine.NextEvents(_machine.Final).Count());
            });
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public CompositeEventStatus CompositeStatus { get; set; }
            public bool Called { get; set; }
            public State CurrentState { get; set; }
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

            Assert.IsFalse(_instance.Called);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsTrue(_instance.Called);

            Assert.AreEqual(2, _instance.CurrentState);
            Assert.IsEmpty(_machine.NextEvents(_machine.GetState("Final")));
        }

        [Test]
        public async Task Should_have_initial_state_with_zero()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.AreEqual(3, _instance.CurrentState);
            Assert.IsEmpty(_machine.NextEvents(_machine.GetState("Final")));
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);
            Assert.IsEmpty(_machine.NextEvents(_machine.GetState("Final")));
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
            Assert.IsEmpty(_machine.NextEvents(_machine.GetState("Final")));
        }

        [Test]
        public async Task Should_have_correct_events()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, _machine.NextEvents(_machine.Initial).Count());
                Assert.AreEqual(1, _machine.NextEvents(_machine.GetState("Initial")).Count());
                Assert.AreEqual(3, _machine.NextEvents(_machine.Waiting).Count());
                Assert.AreEqual(3, _machine.NextEvents(_machine.GetState("Waiting")).Count());
                Assert.AreEqual(0, _machine.NextEvents(_machine.Final).Count());
                Assert.AreEqual(0, _machine.NextEvents(_machine.GetState("Final")).Count());
            });
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance : SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public int CompositeStatus { get; set; }
            public bool Called { get; set; }
            public int CurrentState { get; set; }

            public bool CalledFirst { get; set; }
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

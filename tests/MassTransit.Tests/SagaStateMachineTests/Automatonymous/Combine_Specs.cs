namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_combining_events_into_a_single_event
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
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
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
    public class When_combining_events_with_an_int_for_state
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
        }

        [Test]
        public async Task Should_have_initial_state_with_zero()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            Assert.AreEqual(3, _instance.CurrentState);
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);

            Assert.IsFalse(_instance.Called);
        }

        [Test]
        public async Task Should_not_call_for_one_other_event()
        {
            _machine = new TestStateMachine();
            _instance = new Instance();
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsFalse(_instance.Called);
        }

        TestStateMachine _machine;
        Instance _instance;


        class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public int CompositeStatus { get; set; }
            public bool Called { get; set; }
            public int CurrentState { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

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
    public class When_multiple_events_trigger_a_composite_event
    {
        [Test]
        public async Task Should_have_called_combined_event()
        {
            var machine = new TestStateMachine(CompositeEventOptions.None);
            var instance = new Instance();

            await machine.RaiseEvent(instance, machine.Start);

            await machine.RaiseEvent(instance, machine.First);
            await machine.RaiseEvent(instance, machine.Second);

            Assert.That(instance.TriggerCount, Is.EqualTo(1));
        }

        [Test]
        public async Task Should_not_call_for_one_event()
        {
            var machine = new TestStateMachine(CompositeEventOptions.None);
            var instance = new Instance();
            await machine.RaiseEvent(instance, machine.Start);

            await machine.RaiseEvent(instance, machine.First);

            Assert.That(instance.TriggerCount, Is.EqualTo(0));
        }

        [Test]
        public async Task Should_call_twice_for_duplicate_events()
        {
            var machine = new TestStateMachine(CompositeEventOptions.None);
            var instance = new Instance();

            await machine.RaiseEvent(instance, machine.Start);

            await machine.RaiseEvent(instance, machine.First);
            await machine.RaiseEvent(instance, machine.Second);
            await machine.RaiseEvent(instance, machine.Second);

            Assert.That(instance.TriggerCount, Is.EqualTo(2));
        }

        [Test]
        public async Task Should_call_once_for_duplicate_events()
        {
            var machine = new TestStateMachine(CompositeEventOptions.RaiseOnce);
            var instance = new Instance();

            await machine.RaiseEvent(instance, machine.Start);

            await machine.RaiseEvent(instance, machine.First);
            await machine.RaiseEvent(instance, machine.Second);
            await machine.RaiseEvent(instance, machine.Second);

            Assert.That(instance.TriggerCount, Is.EqualTo(1));
        }


        class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public int CurrentState { get; set; }
            public int CompositeStatus { get; set; }
            public int TriggerCount { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine(CompositeEventOptions options)
            {
                InstanceState(x => x.CurrentState, Waiting);

                CompositeEvent(() => Third, x => x.CompositeStatus, options, First, Second);

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(Third)
                        .Then(context => context.Instance.TriggerCount++));
            }

            // ReSharper disable UnassignedGetOnlyAutoProperty
            // ReSharper disable MemberCanBePrivate.Local
            public State Waiting { get; }

            public Event Start { get; }

            public Event First { get; }
            public Event Second { get; }
            public Event Third { get; }
        }
    }
}

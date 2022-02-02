namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class When_combining_events_into_a_single_event_into_a_single_event
    {
        TestStateMachine _machine;
        Instance _instance;

        [Test]
        public async Task Should_have_called_combined_event_when_compositeevent_defined_before()
        {
            _machine = new TestStateMachine(nameof(Should_have_called_combined_event_when_compositeevent_defined_before));
            _instance = new Instance(Guid.NewGuid());
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsTrue(_instance.Called);
        }

        [Test]
        public async Task Should_have_called_combined_event_when_compositeevent_defined_during()
        {
            _machine = new TestStateMachine(nameof(Should_have_called_combined_event_when_compositeevent_defined_during));
            _instance = new Instance(Guid.NewGuid());
            await _machine.RaiseEvent(_instance, _machine.Start);

            await _machine.RaiseEvent(_instance, _machine.First);
            await _machine.RaiseEvent(_instance, _machine.Second);

            Assert.IsTrue(_instance.Called);
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine(string testName)
            {
                InstanceState(x => x.CurrentState);

                Event(() => Second);
                if (testName == nameof(Should_have_called_combined_event_when_compositeevent_defined_before))
                    CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                Initially(
                    When(Start)
                        .TransitionTo(Waiting));

                During(Waiting,
                    When(First)
                        .TransitionTo(WaitingForSecond));

                if (testName == nameof(Should_have_called_combined_event_when_compositeevent_defined_during))
                    CompositeEvent(() => Third, x => x.CompositeStatus, First, Second);

                During(WaitingForSecond,
                    When(Third)
                        .Then(context => context.Instance.Called = true)
                        .Finalize());
            }

            public State Waiting { get; private set; }
            public State WaitingForSecond { get; private set; }

            public Event Start { get; private set; }
            public Event First { get; private set; }
            public Event Second { get; private set; }
            public Event Third { get; private set; }
        }

        class Instance :
            SagaStateMachineInstance
        {
            bool? _called;

            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public int CompositeStatus { get; set; }
            public int CurrentState { get; set; }

            public Guid CorrelationId { get; set; }

            public bool? Called
            {
                get { return _called; }
                set { _called = value; }
            }
        }
    }
}

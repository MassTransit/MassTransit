namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class When_specifying_an_event_activity
    {
        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Running));
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized)
                .Wait();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    [TestFixture]
    public class When_specifying_an_event_activity_using_initially
    {
        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Running));
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized);
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                Initially(
                    When(Initialized)
                        .TransitionTo(Running));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    [TestFixture]
    public class When_specifying_an_event_activity_using_finally
    {
        [Test]
        public void Should_have_called_the_finally_activity()
        {
            Assert.That(_instance.Value, Is.EqualTo(InstanceStateMachine.Finalized));
        }

        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Final));
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized)
                .Wait();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public string Value { get; set; }
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public const string Finalized = "Finalized";

            public InstanceStateMachine()
            {
                Initially(
                    When(Initialized)
                        .Finalize());

                Finally(x => x.Then(context => context.Saga.Value = Finalized));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
        }
    }


    [TestFixture]
    public class When_hooking_the_initial_enter_state_event
    {
        [Test]
        public void Should_call_the_activity()
        {
            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Final));
        }

        [Test]
        public void Should_have_trigger_the_final_before_enter_event()
        {
            Assert.That(_instance.FinalState, Is.EqualTo(_machine.Running));
        }

        [Test]
        public void Should_have_triggered_the_after_leave_event()
        {
            Assert.That(_instance.LeftState, Is.EqualTo(_machine.Initial));
        }

        [Test]
        public void Should_have_triggered_the_before_enter_event()
        {
            Assert.That(_instance.EnteredState, Is.EqualTo(_machine.Initializing));
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, _machine.Initialized)
                .Wait();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public State EnteredState { get; set; }
            public State LeftState { get; set; }
            public State FinalState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                During(Initializing,
                    When(Initialized)
                        .TransitionTo(Running));

                DuringAny(
                    When(Initial.Enter)
                        .TransitionTo(Initializing),
                    When(Initial.AfterLeave)
                        .Then(context => context.Instance.LeftState = context.Data),
                    When(Initializing.BeforeEnter)
                        .Then(context => context.Instance.EnteredState = context.Data),
                    When(Running.Enter)
                        .Finalize(),
                    When(Final.BeforeEnter)
                        .Then(context => context.Instance.FinalState = context.Instance.CurrentState));
            }

            public State Running { get; private set; }
            public State Initializing { get; private set; }

            public Event Initialized { get; private set; }
        }
    }
}

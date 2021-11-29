namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_specifying_an_event_activity
    {
        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(Running, _instance.CurrentState);
        }

        State Running;
        Event Initialized;

        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                );

            _machine.RaiseEvent(_instance, Initialized)
                .Wait();
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class When_specifying_an_event_activity_using_initially
    {
        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(Running, _instance.CurrentState);
        }

        State Running;
        Event Initialized;

        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                );

            _machine.RaiseEvent(_instance, Initialized);
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
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


    [TestFixture(Category = "Dynamic Modify")]
    public class When_specifying_an_event_activity_using_finally
    {
        [Test]
        public void Should_have_called_the_finally_activity()
        {
            Assert.AreEqual(Finalized, _instance.Value);
        }

        [Test]
        public void Should_transition_to_the_proper_state()
        {
            Assert.AreEqual(_machine.Final, _instance.CurrentState);
        }

        const string Finalized = "Finalized";

        State Running;
        Event Initialized;

        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            const string Finalized = "Finalized";

            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Initialized, b => b.Finalize())
                    .Finally(b => b.Then(context => context.Instance.Value = Finalized))
                );

            _machine.RaiseEvent(_instance, Initialized)
                .Wait();
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public string Value { get; set; }
            public State CurrentState { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class When_hooking_the_initial_enter_state_event
    {
        [Test]
        public void Should_call_the_activity()
        {
            Assert.AreEqual(_machine.Final, _instance.CurrentState);
        }

        [Test]
        public void Should_have_trigger_the_final_before_enter_event()
        {
            Assert.AreEqual(Running, _instance.FinalState);
        }

        [Test]
        public void Should_have_triggered_the_after_leave_event()
        {
            Assert.AreEqual(_machine.Initial, _instance.LeftState);
        }

        [Test]
        public void Should_have_triggered_the_before_enter_event()
        {
            Assert.AreEqual(Initializing, _instance.EnteredState);
        }

        State Running;
        State Initializing;
        Event Initialized;

        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .State("Initializing", out Initializing)
                    .Event("Initialized", out Initialized)
                    .InstanceState(b => b.CurrentState)
                    .During(Initializing)
                        .When(Initialized, b => b.TransitionTo(Running))
                    .DuringAny()
                        .When(builder.Initial.Enter, b => b.TransitionTo(Initializing))
                        .When(builder.Initial.AfterLeave, b => b.Then(context => context.Instance.LeftState = context.Data))
                        .When(Initializing.BeforeEnter, b => b.Then(context => context.Instance.EnteredState = context.Data))
                        .When(Running.Enter, b => b.Finalize())
                        .When(builder.Final.BeforeEnter, b => b.Then(context => context.Instance.FinalState = context.Instance.CurrentState))
                );

            _machine.RaiseEvent(_instance, Initialized)
                .Wait();
        }

        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public State EnteredState { get; set; }
            public State LeftState { get; set; }
            public State FinalState { get; set; }
        }
    }
}

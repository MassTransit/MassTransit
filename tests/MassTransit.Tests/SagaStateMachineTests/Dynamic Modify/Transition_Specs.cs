namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using Automatonymous;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Explicitly_transitioning_to_a_state
    {
        [Test]
        public void Should_call_the_enter_event()
        {
            Assert.IsTrue(_instance.EnterCalled);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_second_moved_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(2, _observer.Events.Count);
        }

        State Running;
        Instance _instance;
        StateMachine<Instance> _machine;
        StateChangeObserver<Instance> _observer;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Event Initialized)
                    .Event("Finish", out Event Finish)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                    .During(Running)
                        .When(Finish, b => b.Finalize())
                    .WhenEnter(Running, x => x.Then(context => context.Instance.EnterCalled = true))
                );
            _observer = new StateChangeObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            {
                _machine.TransitionToState(_instance, Running)
                    .Wait();
            }
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public bool EnterCalled { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class Transitioning_to_a_state_from_a_state
    {
        [Test]
        public void Should_call_the_enter_event()
        {
            Assert.IsTrue(_instance.EnterCalled);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_invoked_final_entered()
        {
            Assert.IsTrue(_instance.FinalEntered);
        }

        [Test]
        public void Should_have_second_moved_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_have_third_moved_to_final()
        {
            Assert.AreEqual(Running, _observer.Events[2].Previous);
            Assert.AreEqual(_machine.Final, _observer.Events[2].Current);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(3, _observer.Events.Count);
        }

        State Running;
        Event Initialized;
        Event Finish;
        Instance _instance;
        StateMachine<Instance> _machine;
        StateChangeObserver<Instance> _observer;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .Event("Finish", out Finish)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                    .During(Running)
                        .When(Finish, b => b.Finalize())
                    .BeforeEnter(builder.Final, x => x.Then(context => context.Instance.FinalEntered = true))
                    .WhenEnter(Running, x => x.Then(context => context.Instance.EnterCalled = true))
                );
            _observer = new StateChangeObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            {
                _machine.RaiseEvent(_instance, Initialized)
                    .Wait();
                _machine.RaiseEvent(_instance, Finish);
            }
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
            public bool EnterCalled { get; set; }

            public bool FinalEntered { get; set; }
        }
    }
}

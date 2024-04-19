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
            Assert.That(_instance.EnterCalled, Is.True);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[0].Previous, Is.EqualTo(null));
                Assert.That(_observer.Events[0].Current, Is.EqualTo(_machine.Initial));
            });
        }

        [Test]
        public void Should_have_second_moved_to_running()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[1].Previous, Is.EqualTo(_machine.Initial));
                Assert.That(_observer.Events[1].Current, Is.EqualTo(Running));
            });
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(2));
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
            public State CurrentState { get; set; }
            public bool EnterCalled { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class Transitioning_to_a_state_from_a_state
    {
        [Test]
        public void Should_call_the_enter_event()
        {
            Assert.That(_instance.EnterCalled, Is.True);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[0].Previous, Is.EqualTo(null));
                Assert.That(_observer.Events[0].Current, Is.EqualTo(_machine.Initial));
            });
        }

        [Test]
        public void Should_have_invoked_final_entered()
        {
            Assert.That(_instance.FinalEntered, Is.True);
        }

        [Test]
        public void Should_have_second_moved_to_running()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[1].Previous, Is.EqualTo(_machine.Initial));
                Assert.That(_observer.Events[1].Current, Is.EqualTo(Running));
            });
        }

        [Test]
        public void Should_have_third_moved_to_final()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[2].Previous, Is.EqualTo(Running));
                Assert.That(_observer.Events[2].Current, Is.EqualTo(_machine.Final));
            });
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(3));
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
            public State CurrentState { get; set; }
            public bool EnterCalled { get; set; }

            public bool FinalEntered { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}

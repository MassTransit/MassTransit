namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;


    [TestFixture]
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
                Assert.That(_observer.Events[1].Current, Is.EqualTo(_machine.Running));
            });
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(2));
        }

        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            {
                _machine.TransitionToState(_instance, _machine.Running)
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


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());

                WhenEnter(Running, x => x.Then(context => context.Instance.EnterCalled = true));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }
    }


    [TestFixture]
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
                Assert.That(_observer.Events[1].Current, Is.EqualTo(_machine.Running));
            });
        }

        [Test]
        public void Should_have_third_moved_to_final()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[2].Previous, Is.EqualTo(_machine.Running));
                Assert.That(_observer.Events[2].Current, Is.EqualTo(_machine.Final));
            });
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(3));
        }

        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            {
                _machine.RaiseEvent(_instance, x => x.Initialized)
                    .Wait();
                _machine.RaiseEvent(_instance, x => x.Finish);
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


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());

                BeforeEnter(Final, x => x.Then(context => context.Instance.FinalEntered = true));
                WhenEnter(Running, x => x.Then(context => context.Instance.EnterCalled = true));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }
    }
}

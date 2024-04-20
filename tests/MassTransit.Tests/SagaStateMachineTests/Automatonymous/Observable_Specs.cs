namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class Observing_state_machine_instance_state_changes
    {
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
        public void Should_have_second_switched_to_running()
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
                _machine.RaiseEvent(_instance, x => x.Initialized).Wait();
                _machine.RaiseEvent(_instance, x => x.Finish).Wait();
            }
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
                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());
            }

            public State Running { get; private set; }
            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }
    }


    [TestFixture]
    public class Observing_events_with_substates
    {
        [Test]
        public void Should_have_all_events()
        {
            Assert.That(_eventObserver.Events, Has.Count.EqualTo(2));
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
        public void Should_have_fourth_switched_to_finished()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[3].Previous, Is.EqualTo(_machine.Resting));
                Assert.That(_observer.Events[3].Current, Is.EqualTo(_machine.Final));
            });
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[1].Previous, Is.EqualTo(_machine.Initial));
                Assert.That(_observer.Events[1].Current, Is.EqualTo(_machine.Running));
            });
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[2].Previous, Is.EqualTo(_machine.Running));
                Assert.That(_observer.Events[2].Current, Is.EqualTo(_machine.Resting));
            });
        }

        [Test]
        public void Should_have_transition_1()
        {
            Assert.That(_eventObserver.Events[0].Event.Name, Is.EqualTo("Initialized"));
        }

        [Test]
        public void Should_have_transition_2()
        {
            Assert.That(_eventObserver.Events[1].Event.Name, Is.EqualTo("LegCramped"));
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(4));
        }

        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;
        EventRaisedObserver<Instance> _eventObserver;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();
            _eventObserver = new EventRaisedObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            using (IDisposable beforeEnterSub = _machine.ConnectEventObserver(_machine.Initialized, _eventObserver))
            using (IDisposable afterLeaveSub = _machine.ConnectEventObserver(_machine.LegCramped, _eventObserver))
            {
                _machine.RaiseEvent(_instance, x => x.Initialized).Wait();
                _machine.RaiseEvent(_instance, x => x.LegCramped).Wait();
                _machine.RaiseEvent(_instance, x => x.Finish).Wait();
            }
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
                SubState(() => Resting, Running);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(LegCramped)
                        .TransitionTo(Resting),
                    When(Finish)
                        .Finalize());

                WhenEnter(Running, x => x.Then(context =>
                {
                }));
                WhenLeave(Running, x => x.Then(context =>
                {
                }));
                BeforeEnter(Running, x => x.Then(context =>
                {
                }));
                AfterLeave(Running, x => x.Then(context =>
                {
                }));
            }

            public State Running { get; private set; }
            public State Resting { get; private set; }
            public Event Initialized { get; private set; }
            public Event LegCramped { get; private set; }
            public Event Finish { get; private set; }
        }
    }


    [TestFixture]
    public class Observing_events_with_substates_part_deux
    {
        [Test]
        public void Should_have_all_events()
        {
            Assert.That(_eventObserver.Events, Has.Count.EqualTo(2));
        }

        [Test]
        public void Should_have_fifth_switched_to_finished()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[4].Previous, Is.EqualTo(_machine.Running));
                Assert.That(_observer.Events[4].Current, Is.EqualTo(_machine.Final));
            });
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
        public void Should_have_fourth_switched_to_finished()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[3].Previous, Is.EqualTo(_machine.Resting));
                Assert.That(_observer.Events[3].Current, Is.EqualTo(_machine.Running));
            });
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[1].Previous, Is.EqualTo(_machine.Initial));
                Assert.That(_observer.Events[1].Current, Is.EqualTo(_machine.Running));
            });
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[2].Previous, Is.EqualTo(_machine.Running));
                Assert.That(_observer.Events[2].Current, Is.EqualTo(_machine.Resting));
            });
        }

        [Test]
        public void Should_have_transition_1()
        {
            Assert.That(_eventObserver.Events[0].Event.Name, Is.EqualTo("Running.BeforeEnter"));
        }

        [Test]
        public void Should_have_transition_2()
        {
            Assert.That(_eventObserver.Events[1].Event.Name, Is.EqualTo("Running.AfterLeave"));
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(5));
        }

        Instance _instance;
        InstanceStateMachine _machine;
        StateChangeObserver<Instance> _observer;
        EventRaisedObserver<Instance> _eventObserver;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new StateChangeObserver<Instance>();
            _eventObserver = new EventRaisedObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            using (IDisposable beforeEnterSub = _machine.ConnectEventObserver(_machine.Running.BeforeEnter, _eventObserver))
            using (IDisposable afterLeaveSub = _machine.ConnectEventObserver(_machine.Running.AfterLeave, _eventObserver))
            {
                _machine.RaiseEvent(_instance, x => x.Initialized).Wait();
                _machine.RaiseEvent(_instance, x => x.LegCramped).Wait();
                _machine.RaiseEvent(_instance, x => x.Recovered).Wait();
                _machine.RaiseEvent(_instance, x => x.Finish).Wait();
            }
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
                SubState(() => Resting, Running);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(LegCramped)
                        .TransitionTo(Resting),
                    When(Finish)
                        .Finalize());

                During(Resting,
                    When(Recovered)
                        .TransitionTo(Running));

                WhenEnter(Running, x => x.Then(context =>
                {
                }));
                WhenLeave(Running, x => x.Then(context =>
                {
                }));
                BeforeEnter(Running, x => x.Then(context =>
                {
                }));
                AfterLeave(Running, x => x.Then(context =>
                {
                }));
            }

            public State Running { get; private set; }
            public State Resting { get; private set; }
            public Event Initialized { get; private set; }
            public Event LegCramped { get; private set; }
            public Event Recovered { get; private set; }
            public Event Finish { get; private set; }
        }
    }
}

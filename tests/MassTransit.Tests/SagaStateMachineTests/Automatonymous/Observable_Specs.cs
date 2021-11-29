namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using Introspection;
    using NUnit.Framework;


    [TestFixture]
    public class Observing_state_machine_instance_state_changes
    {
        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(3, _observer.Events.Count);
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
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
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
            Assert.AreEqual(2, _eventObserver.Events.Count);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_fourth_switched_to_finished()
        {
            Assert.AreEqual(_machine.Resting, _observer.Events[3].Previous);
            Assert.AreEqual(_machine.Final, _observer.Events[3].Current);
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.AreEqual(_machine.Running, _observer.Events[2].Previous);
            Assert.AreEqual(_machine.Resting, _observer.Events[2].Current);
        }

        [Test]
        public void Should_have_transition_1()
        {
            Assert.AreEqual("Initialized", _eventObserver.Events[0].Event.Name);
        }

        [Test]
        public void Should_have_transition_2()
        {
            Assert.AreEqual("LegCramped", _eventObserver.Events[1].Event.Name);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(4, _observer.Events.Count);
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
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
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
            Assert.AreEqual(2, _eventObserver.Events.Count);
        }

        [Test]
        public void Should_have_fifth_switched_to_finished()
        {
            Assert.AreEqual(_machine.Running, _observer.Events[4].Previous);
            Assert.AreEqual(_machine.Final, _observer.Events[4].Current);
        }

        [Test]
        public void Should_have_first_moved_to_initial()
        {
            Assert.AreEqual(null, _observer.Events[0].Previous);
            Assert.AreEqual(_machine.Initial, _observer.Events[0].Current);
        }

        [Test]
        public void Should_have_fourth_switched_to_finished()
        {
            Assert.AreEqual(_machine.Resting, _observer.Events[3].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[3].Current);
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.AreEqual(_machine.Initial, _observer.Events[1].Previous);
            Assert.AreEqual(_machine.Running, _observer.Events[1].Current);
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.AreEqual(_machine.Running, _observer.Events[2].Previous);
            Assert.AreEqual(_machine.Resting, _observer.Events[2].Current);
        }

        [Test]
        public void Should_have_transition_1()
        {
            Assert.AreEqual("Running.BeforeEnter", _eventObserver.Events[0].Event.Name);
        }

        [Test]
        public void Should_have_transition_2()
        {
            Assert.AreEqual("Running.AfterLeave", _eventObserver.Events[1].Event.Name);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(5, _observer.Events.Count);
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
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
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

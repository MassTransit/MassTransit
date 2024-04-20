namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using Automatonymous;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
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
                Assert.That(_observer.Events[1].Current, Is.EqualTo(Running));
            });
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(3));
        }

        State Running;
        Instance _instance;
        StateMachine<Instance> _machine;
        StateChangeObserver<Instance> _observer;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            Event Initialized = null;
            Event Finish = null;

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
                );
            _observer = new StateChangeObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            {
                _machine.RaiseEvent(_instance, Initialized).Wait();
                _machine.RaiseEvent(_instance, Finish).Wait();
            }
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }


    [TestFixture(Category = "Dynamic Modify")]
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
                Assert.That(_observer.Events[3].Previous, Is.EqualTo(Resting));
                Assert.That(_observer.Events[3].Current, Is.EqualTo(_machine.Final));
            });
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[1].Previous, Is.EqualTo(_machine.Initial));
                Assert.That(_observer.Events[1].Current, Is.EqualTo(Running));
            });
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[2].Previous, Is.EqualTo(Running));
                Assert.That(_observer.Events[2].Current, Is.EqualTo(Resting));
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

        State<Instance> Resting;
        State<Instance> Running;
        Instance _instance;
        StateMachine<Instance> _machine;
        StateChangeObserver<Instance> _observer;
        EventRaisedObserver<Instance> _eventObserver;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            Event Initialized = null;
            Event LegCramped = null;
            Event Finish = null;

            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .Event("LegCramped", out LegCramped)
                    .Event("Finish", out Finish)
                    .SubState("Resting", Running, out Resting)
                    .During(builder.Initial)
                    .When(Initialized, b => b.TransitionTo(Running))
                    .During(Running)
                    .When(LegCramped, b => b.TransitionTo(Resting))
                    .When(Finish, b => b.Finalize())
                    .WhenEnter(Running, b => b.Then(context =>
                    {
                    }))
                    .WhenLeave(Running, b => b.Then(context =>
                    {
                    }))
                    .BeforeEnter(Running, b => b.Then(context =>
                    {
                    }))
                    .AfterLeave(Running, b => b.Then(context =>
                    {
                    }))
                );
            _observer = new StateChangeObserver<Instance>();
            _eventObserver = new EventRaisedObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            using (IDisposable beforeEnterSub = _machine.ConnectEventObserver(Initialized, _eventObserver))
            using (IDisposable afterLeaveSub = _machine.ConnectEventObserver(LegCramped, _eventObserver))
            {
                _machine.RaiseEvent(_instance, Initialized).Wait();
                _machine.RaiseEvent(_instance, LegCramped).Wait();
                _machine.RaiseEvent(_instance, Finish).Wait();
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


    [TestFixture(Category = "Dynamic Modify")]
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
                Assert.That(_observer.Events[4].Previous, Is.EqualTo(Running));
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
                Assert.That(_observer.Events[3].Previous, Is.EqualTo(Resting));
                Assert.That(_observer.Events[3].Current, Is.EqualTo(Running));
            });
        }

        [Test]
        public void Should_have_second_switched_to_running()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[1].Previous, Is.EqualTo(_machine.Initial));
                Assert.That(_observer.Events[1].Current, Is.EqualTo(Running));
            });
        }

        [Test]
        public void Should_have_third_switched_to_resting()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_observer.Events[2].Previous, Is.EqualTo(Running));
                Assert.That(_observer.Events[2].Current, Is.EqualTo(Resting));
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

        State<Instance> Resting;
        State<Instance> Running;
        Instance _instance;
        StateMachine<Instance> _machine;
        StateChangeObserver<Instance> _observer;
        EventRaisedObserver<Instance> _eventObserver;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            Event Initialized = null;
            Event LegCramped = null;
            Event Finish = null;
            Event Recovered = null;

            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .Event("LegCramped", out LegCramped)
                    .Event("Finish", out Finish)
                    .Event("Recovered", out Recovered)
                    .SubState("Resting", Running, out Resting)
                    .During(builder.Initial)
                    .When(Initialized, b => b.TransitionTo(Running))
                    .During(Running)
                    .When(LegCramped, b => b.TransitionTo(Resting))
                    .When(Finish, b => b.Finalize())
                    .During(Resting)
                    .When(Recovered, b => b.TransitionTo(Running))
                    .WhenEnter(Running, b => b.Then(context =>
                    {
                    }))
                    .WhenLeave(Running, b => b.Then(context =>
                    {
                    }))
                    .BeforeEnter(Running, b => b.Then(context =>
                    {
                    }))
                    .AfterLeave(Running, b => b.Then(context =>
                    {
                    }))
                );
            _observer = new StateChangeObserver<Instance>();
            _eventObserver = new EventRaisedObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectStateObserver(_observer))
            using (IDisposable beforeEnterSub = _machine.ConnectEventObserver(Running.BeforeEnter, _eventObserver))
            using (IDisposable afterLeaveSub = _machine.ConnectEventObserver(Running.AfterLeave, _eventObserver))
            {
                _machine.RaiseEvent(_instance, Initialized).Wait();
                _machine.RaiseEvent(_instance, LegCramped).Wait();
                _machine.RaiseEvent(_instance, Recovered).Wait();
                _machine.RaiseEvent(_instance, Finish).Wait();
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

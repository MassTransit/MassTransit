namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture]
    public class Using_a_condition_in_a_state_machine
    {
        [Test]
        public async Task Should_allow_if_condition_to_be_evaluated()
        {
            await _machine.RaiseEvent(_instance, _machine.ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.CurrentState, Is.Not.EqualTo(_machine.ShouldNotBeHere));
        }

        [Test]
        public async Task Should_allow_the_condition_to_be_used()
        {
            await _machine.RaiseEvent(_instance, _machine.Started, new Start {InitializeOnly = true});

            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Initialized));
        }

        [Test]
        public async Task Should_evaluate_else_statement_when_if_condition__is_false()
        {
            await _machine.RaiseEvent(_instance, _machine.ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.ShouldBeCalled, Is.True);
        }

        [Test]
        public async Task Should_work()
        {
            await _machine.RaiseEvent(_instance, _machine.Started, new Start());

            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Running));
        }


        [SetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
        }

        Instance _instance;
        InstanceStateMachine _machine;


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public bool InitializeOnly { get; set; }
            public State CurrentState { get; set; }

            public bool ShouldBeCalled { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Started)
                        .Then(context => context.Instance.InitializeOnly = context.Data.InitializeOnly)
                        .If(context => context.Data.InitializeOnly, x => x.Then(context => Console.WriteLine("Initializing Only!")))
                        .TransitionTo(Initialized));

                During(Initial,
                    When(ExplicitFilterStarted, context => true)
                        .IfElse(context => false,
                            binder => binder
                                .Then(context => Console.WriteLine("Should not be here!"))
                                .TransitionTo(ShouldNotBeHere),
                            binder => binder
                                .Then(context => context.Instance.ShouldBeCalled = true)
                                .Then(context => Console.WriteLine("Initializing Only!"))));

                During(Running,
                    When(Finish)
                        .Finalize());

                WhenEnter(Initialized, x => x.If(context => !context.Instance.InitializeOnly, b => b.TransitionTo(Running)));
            }

            public State Running { get; private set; }
            public State Initialized { get; private set; }
            public State ShouldNotBeHere { get; private set; }

            public Event<Start> Started { get; private set; }
            public Event<StartedExplicitFilter> ExplicitFilterStarted { get; private set; }

            public Event Finish { get; private set; }
        }


        class Start
        {
            public bool InitializeOnly { get; set; }
        }


        class StartedExplicitFilter
        {
        }
    }


    [TestFixture]
    public class Using_an_async_condition_in_a_state_machine
    {
        [Test]
        public async Task Should_allow_if_condition_to_be_evaluated()
        {
            await _machine.RaiseEvent(_instance, _machine.ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.CurrentState, Is.Not.EqualTo(_machine.ShouldNotBeHere));
        }

        [Test]
        public async Task Should_allow_the_condition_to_be_used()
        {
            await _machine.RaiseEvent(_instance, _machine.Started, new Start {InitializeOnly = true});

            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Initialized));
        }

        [Test]
        public async Task Should_evaluate_else_statement_when_if_condition__is_false()
        {
            await _machine.RaiseEvent(_instance, _machine.ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.ShouldBeCalled, Is.True);
        }

        [Test]
        public async Task Should_work()
        {
            await _machine.RaiseEvent(_instance, _machine.Started, new Start());

            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Running));
        }

        [SetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
        }

        Instance _instance;
        InstanceStateMachine _machine;


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public bool InitializeOnly { get; set; }
            public State CurrentState { get; set; }
            public bool ShouldBeCalled { get; set; }
        }


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                During(Initial,
                    When(Started)
                        .Then(context => context.Instance.InitializeOnly = context.Data.InitializeOnly)
                        .IfAsync(context => Task.FromResult(context.Data.InitializeOnly),
                            x => x.Then(context => Console.WriteLine("Initializing Only!")))
                        .TransitionTo(Initialized));

                During(Initial,
                    When(ExplicitFilterStarted, context => true)
                        .IfElseAsync(context => Task.FromResult(false),
                            binder => binder
                                .Then(context => Console.WriteLine("Should not be here!"))
                                .TransitionTo(ShouldNotBeHere),
                            binder => binder
                                .Then(context => context.Instance.ShouldBeCalled = true)
                                .Then(context => Console.WriteLine("Initializing Only!"))));

                During(Running,
                    When(Finish)
                        .Finalize());

                WhenEnter(Initialized,
                    x => x.IfAsync(context => Task.FromResult(!context.Instance.InitializeOnly), b => b.TransitionTo(Running)));
            }

            public State Running { get; private set; }
            public State Initialized { get; private set; }
            public State ShouldNotBeHere { get; private set; }

            public Event<Start> Started { get; private set; }
            public Event<StartedExplicitFilter> ExplicitFilterStarted { get; private set; }

            public Event Finish { get; private set; }
        }


        class Start
        {
            public bool InitializeOnly { get; set; }
        }


        class StartedExplicitFilter
        {
        }
    }
}

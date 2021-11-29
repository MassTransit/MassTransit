namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class Using_a_condition_in_a_state_machine
    {
        [Test]
        public async Task Should_allow_if_condition_to_be_evaluated()
        {
            await _machine.RaiseEvent(_instance, ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.CurrentState, Is.Not.EqualTo(ShouldNotBeHere));
        }

        [Test]
        public async Task Should_allow_the_condition_to_be_used()
        {
            await _machine.RaiseEvent(_instance, Started, new Start {InitializeOnly = true});

            Assert.That(_instance.CurrentState, Is.EqualTo(Initialized));
        }

        [Test]
        public async Task Should_evaluate_else_statement_when_if_condition__is_false()
        {
            await _machine.RaiseEvent(_instance, ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.ShouldBeCalled, Is.True);
        }

        [Test]
        public async Task Should_work()
        {
            await _machine.RaiseEvent(_instance, Started, new Start());

            Assert.That(_instance.CurrentState, Is.EqualTo(Running));
        }


        [SetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .State("Initialized", out Initialized)
                    .State("ShouldNotBeHere", out ShouldNotBeHere)
                    .Event("Started", out Started)
                    .Event("ExplicitFilterStarted", out ExplicitFilterStarted)
                    .Event("Finish", out Finish)
                    .During(builder.Initial)
                        .When(Started, b => b
                            .Then(context => context.Instance.InitializeOnly = context.Data.InitializeOnly)
                            .If(context => context.Data.InitializeOnly, x => x.Then(context => Console.WriteLine("Initializing Only!")))
                            .TransitionTo(Initialized)
                        )
                    .During(builder.Initial)
                        .When(ExplicitFilterStarted, context => true, b => b
                            .IfElse(context => false,
                                binder => binder
                                    .Then(context => Console.WriteLine("Should not be here!"))
                                    .TransitionTo(ShouldNotBeHere),
                                binder => binder
                                    .Then(context => context.Instance.ShouldBeCalled = true)
                                    .Then(context => Console.WriteLine("Initializing Only!")))
                        )
                    .During(Running)
                        .When(Finish, b => b.Finalize())
                    .WhenEnter(Initialized, b => b.If(context => !context.Instance.InitializeOnly, b => b.TransitionTo(Running)))
                );
        }

        State Running;
        State Initialized;
        State ShouldNotBeHere;
        Event<Start> Started;
        Event<StartedExplicitFilter> ExplicitFilterStarted;
        Event Finish;

        Instance _instance;
        StateMachine<Instance> _machine;

        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public bool InitializeOnly { get; set; }
            public State CurrentState { get; set; }

            public bool ShouldBeCalled { get; set; }
        }

        class Start
        {
            public bool InitializeOnly { get; set; }
        }

        class StartedExplicitFilter { }
    }


    [TestFixture(Category = "Dynamic Modify")]
    public class Using_an_async_condition_in_a_state_machine
    {
        [Test]
        public async Task Should_allow_if_condition_to_be_evaluated()
        {
            await _machine.RaiseEvent(_instance, ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.CurrentState, Is.Not.EqualTo(ShouldNotBeHere));
        }

        [Test]
        public async Task Should_allow_the_condition_to_be_used()
        {
            await _machine.RaiseEvent(_instance, Started, new Start {InitializeOnly = true});

            Assert.That(_instance.CurrentState, Is.EqualTo(Initialized));
        }

        [Test]
        public async Task Should_evaluate_else_statement_when_if_condition__is_false()
        {
            await _machine.RaiseEvent(_instance, ExplicitFilterStarted, new StartedExplicitFilter());

            Assert.That(_instance.ShouldBeCalled, Is.True);
        }

        [Test]
        public async Task Should_work()
        {
            await _machine.RaiseEvent(_instance, Started, new Start());

            Assert.That(_instance.CurrentState, Is.EqualTo(Running));
        }

        State Running;
        State Initialized;
        State ShouldNotBeHere;
        Event<Start> Started;
        Event<StartedExplicitFilter> ExplicitFilterStarted;
        Event Finish;

        Instance _instance;
        StateMachine<Instance> _machine;

        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public bool InitializeOnly { get; set; }
            public State CurrentState { get; set; }
            public bool ShouldBeCalled { get; set; }
        }

        [SetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .State("Initialized", out Initialized)
                    .State("ShouldNotBeHere", out ShouldNotBeHere)
                    .Event("Started", out Started)
                    .Event("ExplicitFilterStarted", out ExplicitFilterStarted)
                    .Event("Finish", out Finish)
                    .During(builder.Initial)
                        .When(Started, b => b
                            .Then(context => context.Instance.InitializeOnly = context.Data.InitializeOnly)
                            .IfAsync(context => Task.FromResult(context.Data.InitializeOnly),
                                x => x.Then(context => Console.WriteLine("Initializing Only!")))
                            .TransitionTo(Initialized)
                        )
                    .During(builder.Initial)
                        .When(ExplicitFilterStarted, context => true, b => b
                            .IfElseAsync(context => Task.FromResult(false),
                                binder => binder
                                    .Then(context => Console.WriteLine("Should not be here!"))
                                    .TransitionTo(ShouldNotBeHere),
                                binder => binder
                                    .Then(context => context.Instance.ShouldBeCalled = true)
                                    .Then(context => Console.WriteLine("Initializing Only!")))
                        )
                    .During(Running)
                        .When(Finish, b => b.Finalize())
                    .WhenEnter(Initialized, b => b.IfAsync(context => Task.FromResult(!context.Instance.InitializeOnly), b => b.TransitionTo(Running)))
                );
        }

        class Start
        {
            public bool InitializeOnly { get; set; }
        }

        class StartedExplicitFilter { }
    }
}

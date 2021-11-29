namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_any_state_transition_occurs
    {
        [Test]
        public void Should_be_running()
        {
            Assert.AreEqual(Running, _instance.CurrentState);
        }

        [Test]
        public void Should_have_entered_running()
        {
            Assert.AreEqual(Running, _instance.LastEntered);
        }

        [Test]
        public void Should_have_left_initial()
        {
            Assert.AreEqual(_machine.Initial, _instance.LastLeft);
        }

        State Running;
        Event Initialized;
        Event Finish;

        Instance _instance;
        StateMachine<Instance> _machine;

        [OneTimeSetUp]
        public void Setup()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .State("Running", out Running)
                    .Event("Initialized", out Initialized)
                    .Event("Finish", out Finish)
                    .InstanceState(b => b.CurrentState)
                    .During(builder.Initial)
                        .When(Initialized, b => b.TransitionTo(Running))
                    .During(Running)
                        .When(Finish, b => b.Finalize())
                    .BeforeEnterAny(b => b.Then(context => context.Instance.LastEntered = context.Data))
                    .AfterLeaveAny(b => b.Then(context => context.Instance.LastLeft = context.Data))
                );

            _machine.RaiseEvent(_instance, Initialized)
                .Wait();
        }


        class Instance :
SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }

            public State LastEntered { get; set; }
            public State LastLeft { get; set; }
        }
    }
}

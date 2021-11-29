namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class When_any_state_transition_occurs
    {
        [Test]
        public void Should_be_running()
        {
            Assert.AreEqual(_machine.Running, _instance.CurrentState);
        }

        [Test]
        public void Should_have_entered_running()
        {
            Assert.AreEqual(_machine.Running, _instance.LastEntered);
        }

        [Test]
        public void Should_have_left_initial()
        {
            Assert.AreEqual(_machine.Initial, _instance.LastLeft);
        }

        Instance _instance;
        InstanceStateMachine _machine;

        [OneTimeSetUp]
        public void Setup()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();

            _machine.RaiseEvent(_instance, x => x.Initialized)
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


        class InstanceStateMachine :
            MassTransitStateMachine<Instance>
        {
            public InstanceStateMachine()
            {
                InstanceState(instance => instance.CurrentState);

                During(Initial,
                    When(Initialized)
                        .TransitionTo(Running));

                During(Running,
                    When(Finish)
                        .Finalize());

                BeforeEnterAny(x => x.Then(context => context.Instance.LastEntered = context.Data));
                AfterLeaveAny(x => x.Then(context => context.Instance.LastLeft = context.Data));
            }

            public State Running { get; private set; }

            public Event Initialized { get; private set; }
            public Event Finish { get; private set; }
        }
    }
}

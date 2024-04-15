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
            Assert.That(_instance.CurrentState, Is.EqualTo(_machine.Running));
        }

        [Test]
        public void Should_have_entered_running()
        {
            Assert.That(_instance.LastEntered, Is.EqualTo(_machine.Running));
        }

        [Test]
        public void Should_have_left_initial()
        {
            Assert.That(_instance.LastLeft, Is.EqualTo(_machine.Initial));
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
            public State CurrentState { get; set; }

            public State LastEntered { get; set; }
            public State LastLeft { get; set; }
            public Guid CorrelationId { get; set; }
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

namespace MassTransit.Tests.SagaStateMachineTests.Automatonymous
{
    using System;
    using NUnit.Framework;


    [TestFixture]
    public class When_an_event_is_raised_on_an_instance
    {
        [Test]
        public void Should_have_raised_the_initialized_event()
        {
            Assert.AreEqual(_machine.Initialized, _observer.Events[0].Event);
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.AreEqual(1, _observer.Events.Count);
        }

        Instance _instance;
        InstanceStateMachine _machine;
        EventRaisedObserver<Instance> _observer;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = new InstanceStateMachine();
            _observer = new EventRaisedObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectEventObserver(_machine.Initialized, _observer))
                _machine.RaiseEvent(_instance, x => x.Initialized).Wait();
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
            }

            public State Running { get; private set; }
            public Event Initialized { get; private set; }
        }
    }
}

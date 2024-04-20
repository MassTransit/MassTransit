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
            Assert.That(_observer.Events[0].Event, Is.EqualTo(_machine.Initialized));
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(1));
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
            }

            public State Running { get; private set; }
            public Event Initialized { get; private set; }
        }
    }
}

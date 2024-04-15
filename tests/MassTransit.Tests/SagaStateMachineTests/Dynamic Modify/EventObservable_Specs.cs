namespace MassTransit.Tests.SagaStateMachineTests.Dynamic_Modify
{
    using System;
    using Automatonymous;
    using NUnit.Framework;


    [TestFixture(Category = "Dynamic Modify")]
    public class When_an_event_is_raised_on_an_instance
    {
        [Test]
        public void Should_have_raised_the_initialized_event()
        {
            Assert.That(_observer.Events[0].Event, Is.EqualTo(Initialized));
        }

        [Test]
        public void Should_raise_the_event()
        {
            Assert.That(_observer.Events, Has.Count.EqualTo(1));
        }

        State Running;
        Event Initialized;
        Instance _instance;
        StateMachine<Instance> _machine;
        EventRaisedObserver<Instance> _observer;

        [OneTimeSetUp]
        public void Specifying_an_event_activity()
        {
            _instance = new Instance();
            _machine = MassTransitStateMachine<Instance>
                .New(builder => builder
                    .Event("Initialized", out Initialized)
                    .State("Running", out Running)
                    .During(builder.Initial)
                    .When(Initialized, b => b.TransitionTo(Running))
                );
            _observer = new EventRaisedObserver<Instance>();

            using (IDisposable subscription = _machine.ConnectEventObserver(Initialized, _observer))
                _machine.RaiseEvent(_instance, Initialized).Wait();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }
    }
}

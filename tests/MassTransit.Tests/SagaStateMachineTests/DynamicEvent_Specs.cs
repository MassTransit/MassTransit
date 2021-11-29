namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    public class Specifying_dynamic_events_in_a_state_machine :
        InMemoryTestFixture
    {
        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        static Specifying_dynamic_events_in_a_state_machine()
        {
            GlobalTopology.Send.UseCorrelationId<Start>(x => x.ServiceId);
            GlobalTopology.Send.UseCorrelationId<Stop>(x => x.ServiceId);
        }

        public Specifying_dynamic_events_in_a_state_machine()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
        }

        [Test]
        public async Task Should_handle_a_double_state()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { ServiceId = sagaId });

            Guid? saga = await _repository.ShouldContainSaga(sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Stop { ServiceId = sagaId });

            saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Final, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async Task Should_handle_the_initial_state()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { ServiceId = sagaId });

            Guid? saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Running, TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }


        class Instance :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        sealed class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event<Start> started = Event<Start>(nameof(Start));
                Event<Stop> stopped = Event<Stop>(nameof(Stop));

                Initially(
                    When(started)
                        .TransitionTo(Running));

                During(Running,
                    When(stopped)
                        .Finalize());
            }

            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public State Running { get; private set; }
        }


        class Start
        {
            public Guid ServiceId { get; set; }
        }


        class Stop
        {
            public Guid ServiceId { get; set; }
        }
    }
}

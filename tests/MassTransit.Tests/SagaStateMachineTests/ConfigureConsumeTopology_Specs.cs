namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Specifying_no_topology :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_not_bind_the_event_handler()
        {
            var sagaId = NewId.NextGuid();

            await Bus.Publish(new Start {CorrelationId = sagaId});

            Guid? saga = await _repository.ShouldContainSaga(sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            var handler = await ConnectPublishHandler<Suspend>();

            await Bus.Publish(new Suspend {CorrelationId = sagaId});

            await handler;

            saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Running, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Stop() {CorrelationId = sagaId});

            saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Final, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public Specifying_no_topology()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
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

                Event(() => Started);
                Event(() => Suspended, x => x.ConfigureConsumeTopology = false);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Suspended)
                        .TransitionTo(Sus),
                    When(Stopped)
                        .Finalize());
            }

            // ReSharper disable UnassignedGetOnlyAutoProperty
            public State Running { get; }
            public State Sus { get; }

            public Event<Start> Started { get; }
            public Event<Suspend> Suspended { get; }
            public Event<Stop> Stopped { get; }
        }


        class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        class Suspend :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }
    }
}

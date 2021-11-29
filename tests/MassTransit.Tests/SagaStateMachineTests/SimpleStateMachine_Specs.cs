namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_a_simple_state_machine :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_a_double_state()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { CorrelationId = sagaId });

            Guid? saga = await _repository.ShouldContainSaga(sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Stop { CorrelationId = sagaId });

            saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Final, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async Task Should_handle_the_initial_state()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { CorrelationId = sagaId });

            Guid? saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Running, TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public Using_a_simple_state_machine()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
        }


        class Instance :
            SagaStateMachineInstance
        {
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());
            } // ReSharper disable UnassignedGetOnlyAutoProperty
            public State Running { get; }
            public Event<Start> Started { get; }
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
    }


    public class Using_a_simple_state_machine_with_topology_correlation_id :
        InMemoryTestFixture
    {
        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public Using_a_simple_state_machine_with_topology_correlation_id()
        {
            _machine = new TestStateMachine();
            _repository = new InMemorySagaRepository<Instance>();
        }

        [Test]
        public async Task Should_handle_a_double_state()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { CorrelationId = sagaId });

            Guid? saga = await _repository.ShouldContainSaga(sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Stop { CorrelationId = sagaId });

            saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Final, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async Task Should_handle_the_initial_state()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { CorrelationId = sagaId });

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
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());
            } // ReSharper disable UnassignedGetOnlyAutoProperty
            public State Running { get; }
            public Event<Start> Started { get; }
            public Event<Stop> Stopped { get; }
        }


        class Start
        {
            public Guid CorrelationId { get; set; }
        }


        class Stop
        {
            public Guid CorrelationId { get; set; }
        }
    }


    public class Using_a_simple_state_machine_with_custom_correlation_id :
        InMemoryTestFixture
    {
        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        static Using_a_simple_state_machine_with_custom_correlation_id()
        {
            GlobalTopology.Send.UseCorrelationId<Start>(x => x.ServiceId);
            GlobalTopology.Send.UseCorrelationId<Stop>(x => x.ServiceId);
        }

        public Using_a_simple_state_machine_with_custom_correlation_id()
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
            public Instance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected Instance()
            {
            }

            public State CurrentState { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started);
                Event(() => Stopped);

                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());
            }

            public State Running { get; }
            public Event<Start> Started { get; }
            public Event<Stop> Stopped { get; } // ReSharper disable UnassignedGetOnlyAutoProperty
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

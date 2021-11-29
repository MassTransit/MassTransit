namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class When_a_remove_expression_is_specified :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_handle_the_initial_state()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { CorrelationId = sagaId });

            Guid? saga = await _repository.ShouldContainSagaInState(sagaId, _machine, x => x.Running, TestTimeout);

            Assert.IsTrue(saga.HasValue);
        }

        [Test]
        public async Task Should_remove_the_saga_once_completed()
        {
            var sagaId = Guid.NewGuid();

            await Bus.Publish(new Start { CorrelationId = sagaId });

            Guid? saga = await _repository.ShouldContainSaga(sagaId, TestTimeout);
            Assert.IsTrue(saga.HasValue);

            await Bus.Publish(new Stop { CorrelationId = sagaId });

            saga = await _repository.ShouldNotContainSaga(sagaId, TestTimeout);
            Assert.IsFalse(saga.HasValue);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public When_a_remove_expression_is_specified()
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
                Initially(
                    When(Started)
                        .TransitionTo(Running));

                During(Running,
                    When(Stopped)
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<Stop> Stopped { get; private set; }
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


    [TestFixture]
    public class When_a_saga_goes_straight_to_finalized :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_remove_the_saga_once_completed()
        {
            var sagaId = Guid.NewGuid();

            Response<Answer> response = await Bus.Request<Ask, Answer>(InputQueueAddress, new Ask { CorrelationId = sagaId }, TestCancellationToken);

            await Task.Delay(50);

            Guid? saga = await _repository.ShouldNotContainSaga(sagaId, TestTimeout);
            Assert.IsFalse(saga.HasValue);

            Assert.AreEqual(sagaId, response.CorrelationId);
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.StateMachineSaga(_machine, _repository);
        }

        readonly TestStateMachine _machine;
        readonly InMemorySagaRepository<Instance> _repository;

        public When_a_saga_goes_straight_to_finalized()
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
                Event(() => Asked, x => x.CorrelateById(context => context.Message.CorrelationId));

                Initially(
                    When(Asked)
                        .Respond(context => new Answer { CorrelationId = context.Data.CorrelationId })
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public Event<Ask> Asked { get; private set; }
        }


        class Ask
        {
            public Guid CorrelationId { get; set; }
        }


        class Answer
        {
            public Guid CorrelationId { get; set; }
        }
    }
}

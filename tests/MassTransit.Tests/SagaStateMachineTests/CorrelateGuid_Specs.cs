namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Correlation_a_state_machine_by_guid :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_map_to_the_instance()
        {
            var id = NewId.NextGuid();

            await Bus.Publish<BeginTransaction>(new { TransactionId = id });

            Guid? saga = await _repository.ShouldContainSagaInState(state => state.TransactionId == id, _machine, x => x.Active, TestTimeout);

            Assert.IsTrue(saga.HasValue);

            await Bus.Publish<CommitTransaction>(new { TransactionId = id });

            saga = await _repository.ShouldContainSagaInState(state => state.TransactionId == id, _machine, x => x.Final, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        InMemorySagaRepository<TransactionState> _repository;
        TransactionStateMachine _machine;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            base.ConfigureInMemoryReceiveEndpoint(configurator);

            _repository = new InMemorySagaRepository<TransactionState>();
            _machine = new TransactionStateMachine();

            configurator.StateMachineSaga(_machine, _repository, x =>
            {
                x.Message<BeginTransaction>(m => m.UsePartitioner(4, p => p.Message.TransactionId));
            });
        }


        class TransactionState :
            SagaStateMachineInstance
        {
            public State CurrentState { get; set; }
            public Guid TransactionId { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class TransactionStateMachine :
            MassTransitStateMachine<TransactionState>
        {
            public TransactionStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Begin, x =>
                {
                    x.CorrelateById(state => state.TransactionId, context => context.Message.TransactionId);
                    x.SelectId(context => context.Message.TransactionId);
                    x.InsertOnInitial = true;
                    x.SetSagaFactory(context => new TransactionState
                    {
                        CorrelationId = context.Message.TransactionId,
                        TransactionId = context.Message.TransactionId
                    });
                });

                Event(() => Commit, x =>
                {
                    x.CorrelateById(state => state.TransactionId, context => context.Message.TransactionId);
                    x.SelectId(context => context.Message.TransactionId);
                });


                Initially(
                    When(Begin)
                        .TransitionTo(Active));

                During(Active,
                    When(Commit)
                        .Finalize());
            }

            public State Active { get; private set; }
            public Event<BeginTransaction> Begin { get; private set; }
            public Event<CommitTransaction> Commit { get; private set; }
        }


        public interface BeginTransaction
        {
            Guid TransactionId { get; }
        }


        public interface CommitTransaction
        {
            Guid TransactionId { get; }
        }
    }
}

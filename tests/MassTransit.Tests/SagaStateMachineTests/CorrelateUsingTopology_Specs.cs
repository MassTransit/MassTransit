namespace MassTransit.Tests.SagaStateMachineTests
{
    using System;
    using System.Threading.Tasks;
    using CorrelationEvents;
    using MassTransit.Saga;
    using MassTransit.Testing;
    using NUnit.Framework;
    using TestFramework;


    namespace CorrelationEvents
    {
        public interface BeginTransaction
        {
            Guid TransactionId { get; }
        }


        public interface CommitTransaction
        {
            Guid TransactionId { get; }
        }


        static class Events
        {
            public static void ConfigureCorrelation()
            {
                GlobalTopology.Send.UseCorrelationId<BeginTransaction>(x => x.TransactionId);
                GlobalTopology.Send.UseCorrelationId<CommitTransaction>(x => x.TransactionId);
            }
        }
    }


    [TestFixture]
    public class Using_topology_for_event_correlation :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_properly_map_to_the_instance()
        {
            var id = NewId.NextGuid();

            await Bus.Publish<BeginTransaction>(new { TransactionId = id });

            Guid? saga = await _repository.ShouldContainSagaInState(id, _machine, x => x.Active, TestTimeout);

            Assert.IsTrue(saga.HasValue);

            await Bus.Publish<CommitTransaction>(new { TransactionId = id });

            saga = await _repository.ShouldContainSagaInState(id, _machine, x => x.Final, TestTimeout);
            Assert.IsTrue(saga.HasValue);
        }

        static Using_topology_for_event_correlation()
        {
            Events.ConfigureCorrelation();
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
            public Guid CorrelationId { get; set; }
        }


        class TransactionStateMachine :
            MassTransitStateMachine<TransactionState>
        {
            public TransactionStateMachine()
            {
                InstanceState(x => x.CurrentState);

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
    }
}

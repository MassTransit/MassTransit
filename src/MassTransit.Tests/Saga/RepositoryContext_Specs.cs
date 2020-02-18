namespace MassTransit.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using MassTransit.Saga;
    using MassTransit.Saga.InMemoryRepository;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    public class Using_the_universal_saga_repository :
        InMemoryTestFixture
    {
        TestStateMachine _machine;
        ISagaRepository<Instance> _repository;

        [Test]
        public async Task Should_reach_the_saga()
        {
            var createCompleted = ConnectPublishHandler<CreateCompleted>();
            var destroyCompleted = ConnectPublishHandler<DestroyCompleted>();

            var values = new {InVar.CorrelationId};
            await InputQueueSendEndpoint.Send<Create>(values);

            var createContext = await createCompleted;

            await InputQueueSendEndpoint.Send<Destroy>(values);

            var destroyContext = await destroyCompleted;
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new TestStateMachine();
            _repository = CreateInMemorySagaRepository<Instance>();

            configurator.StateMachineSaga(_machine, _repository);
        }

        ISagaRepository<T> CreateInMemorySagaRepository<T>()
            where T : class, ISaga
        {
            var dictionary = new IndexedSagaDictionary<T>();

            ISagaConsumeContextFactory<IndexedSagaDictionary<T>, T> factory = new InMemorySagaConsumeContextFactory<T>();

            ISagaRepositoryContextFactory<T> repositoryContextFactory = new InMemorySagaRepositoryContextFactory<T>(dictionary, factory);

            return new SagaRepository<T>(repositoryContextFactory);
        }


        public interface Create :
            CorrelatedBy<Guid>
        {
        }


        public interface CreateCompleted :
            CorrelatedBy<Guid>
        {
        }


        public interface Destroy :
            CorrelatedBy<Guid>
        {
        }


        public interface DestroyCompleted :
            CorrelatedBy<Guid>
        {
        }


        public class Instance :
            SagaStateMachineInstance
        {
            public Guid CorrelationId { get; set; }
            public State CurrentState { get; set; }
        }


        class TestStateMachine :
            MassTransitStateMachine<Instance>
        {
            public TestStateMachine()
            {
                InstanceState(x => x.CurrentState);
                SetCompletedWhenFinalized();

                Initially(
                    When(Created)
                        .PublishAsync(x => x.Init<CreateCompleted>(x.Instance))
                        .TransitionTo(Active));

                During(Active,
                    When(Destroyed)
                        .PublishAsync(x => x.Init<DestroyCompleted>(x.Instance))
                        .Finalize());
            }

            public State Active { get; private set; }
            public Event<Create> Created { get; private set; }
            public Event<Destroy> Destroyed { get; private set; }
        }
    }
}

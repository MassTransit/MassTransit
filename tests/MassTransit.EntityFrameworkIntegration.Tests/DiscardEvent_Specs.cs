namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using TestFramework;


    [TestFixture]
    public class StateMachineTest :
        InMemoryTestFixture
    {
        [Test]
        public async Task Test_Discarded_Missing_Saga_Instance_Is_Not_Persisted()
        {
            // arrange
            var sagaId = Guid.NewGuid();

            // act
            await Bus.Publish<TriggerSecond>(new
            {
                CorrelationId = sagaId,
                TestName = "Test_Discarded_Missing_Saga_Instance_Is_Not_Persisted"
            });

            var wasDiscarded = await _discarded.Task;

            Assert.IsTrue(wasDiscarded);

            using (var dbContext = _sagaDbContextFactory.Create())
            {
                var result = dbContext.Set<SimpleState>().FirstOrDefault(x => x.CorrelationId == sagaId);
                // THE PROBLEM : the missing instance is not discarded and is persisted to the repository
                // This test fails
                Assert.IsNull(result);
            }
        }

        [Test]
        [Explicit]
        public async Task Test_Event_Persists_Saga_By_Default()
        {
            // arrange
            var sagaId = Guid.NewGuid();

            // act
            await Bus.Publish<TriggerFirst>(new
            {
                CorrelationId = sagaId,
                TestName = "Test_Event_Persists_Saga_By_Default"
            });

            await Task.Delay(10 * 1000).ConfigureAwait(false);

            // assert
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                var result = dbContext.Set<SimpleState>().FirstOrDefault(x => x.CorrelationId == sagaId);
                Assert.IsNotNull(result);
            }
        }

        ISagaDbContextFactory<SimpleState> _sagaDbContextFactory;

        SimpleStateMachine _simpleStateMachine;
        Lazy<ISagaRepository<SimpleState>> _simpleStateRepository;
        TaskCompletionSource<bool> _discarded;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _discarded = GetTask<bool>();

            _simpleStateMachine = new SimpleStateMachine(x =>
            {
                _discarded.TrySetResult(true);
            });

            _sagaDbContextFactory = new DelegateSagaDbContextFactory<SimpleState>(() =>
                new SimpleStateSagaDbContext(LocalDbConnectionStringProvider.GetLocalDbConnectionString()));

            _simpleStateRepository = new Lazy<ISagaRepository<SimpleState>>(() =>
                EntityFrameworkSagaRepository<SimpleState>.CreatePessimistic(_sagaDbContextFactory));


            configurator.StateMachineSaga(_simpleStateMachine, _simpleStateRepository.Value);

            base.ConfigureInMemoryReceiveEndpoint(configurator);
        }
    }


    public class TriggerFirst : CorrelatedBy<Guid>
    {
        public string TestName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class TriggerSecond : CorrelatedBy<Guid>
    {
        public string TestName { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class SimpleState : SagaStateMachineInstance
    {
        public string CurrentState { get; set; }
        public string TestName { get; set; }
        public DateTime PublishDate { get; set; }
        public Guid CorrelationId { get; set; }
    }


    public class SimpleStateSagaDbContext : SagaDbContext
    {
        public SimpleStateSagaDbContext(string getLocalDbConnectionString)
            : base(getLocalDbConnectionString)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new SimpleStateMap(); }
        }


        class SimpleStateMap :
            SagaClassMap<SimpleState>
        {
            protected override void Configure(EntityTypeConfiguration<SimpleState> entity, DbModelBuilder modelBuilder)
            {
                entity.Property(x => x.CurrentState)
                    .HasMaxLength(64);

                entity.Property(x => x.CorrelationId);
                entity.Property(x => x.TestName);
                entity.Property(x => x.PublishDate);
            }
        }
    }


    public sealed class SimpleStateMachine :
        MassTransitStateMachine<SimpleState>
    {
        public SimpleStateMachine(Action<string> executionMessage)
        {
            Event(() => FirstTrigger);
            Event(() => SecondTrigger,
                x => x.OnMissingInstance(e =>
                {
                    executionMessage("Missing Instance!!!");
                    return e.Discard();
                }));

            InstanceState(i => i.CurrentState);

            Initially(
                When(FirstTrigger)
                    .Then(DoFirstThing)
                    .TransitionTo(InProgress));

            DuringAny(
                When(SecondTrigger)
                    .Then(DoSecondThing)
                    .TransitionTo(Pending));
        }

        public Event<TriggerFirst> FirstTrigger { get; set; }
        public Event<TriggerSecond> SecondTrigger { get; set; }

        public State InProgress { get; set; }
        public State Pending { get; set; }

        void DoFirstThing(BehaviorContext<SimpleState, TriggerFirst> obj)
        {
            obj.Instance.TestName = obj.Data.TestName;
            obj.Instance.PublishDate = DateTime.Now;
        }

        void DoSecondThing(BehaviorContext<SimpleState, TriggerSecond> obj)
        {
            obj.Instance.TestName = obj.Data.TestName;
            obj.Instance.PublishDate = DateTime.Now;
        }
    }
}

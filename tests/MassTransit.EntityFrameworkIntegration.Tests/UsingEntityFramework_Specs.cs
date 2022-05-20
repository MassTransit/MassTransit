namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.ModelConfiguration;
    using System.Linq;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("EntityFramework")]
    public class When_using_EntityFramework :
        InMemoryTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_handle_the_big_load()
        {
            var tasks = new List<Task>();

            var sagaIds = new Guid[200];
            for (var i = 0; i < 200; i++)
            {
                var correlationId = Guid.NewGuid();

                tasks.Add(InputQueueSendEndpoint.Send(new GirlfriendYelling { CorrelationId = correlationId }));

                sagaIds[i] = correlationId;
            }

            await Task.WhenAll(tasks);

            for (var i = 0; i < 200; i++)
            {
                Guid? sagaId = await _repository.Value.ShouldContainSaga(sagaIds[i], TestTimeout);
                Assert.IsTrue(sagaId.HasValue);
            }
        }

        [Test]
        public async Task Should_have_removed_the_state_machine()
        {
            var correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling { CorrelationId = correlationId });

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);
            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new SodOff { CorrelationId = correlationId });

            sagaId = await _repository.Value.ShouldNotContainSaga(correlationId, TestTimeout);
            Assert.IsFalse(sagaId.HasValue);
        }

        [Test]
        public async Task Should_have_the_state_machine()
        {
            var correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling { CorrelationId = correlationId });

            Guid? sagaId = await _repository.Value.ShouldContainSaga(correlationId, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new GotHitByACar { CorrelationId = correlationId });

            sagaId = await _repository.Value.ShouldContainSagaInState(correlationId, _machine, _machine.Dead, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            var instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
        }

        SuperShopper _machine;
        readonly ISagaDbContextFactory<ShoppingChore> _sagaDbContextFactory;
        readonly Lazy<ISagaRepository<ShoppingChore>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new SuperShopper();

            configurator.UseRetry(x =>
            {
                x.Handle<DbUpdateException>();
                x.Immediate(5);
            });

            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_EntityFramework()
        {
            _sagaDbContextFactory = new DelegateSagaDbContextFactory<ShoppingChore>(
                () => new ShoppingChoreSagaDbContext(LocalDbConnectionStringProvider.GetLocalDbConnectionString()));

            _repository = new Lazy<ISagaRepository<ShoppingChore>>(() => EntityFrameworkSagaRepository<ShoppingChore>.CreatePessimistic(_sagaDbContextFactory));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
        }

        async Task<ShoppingChore> GetSaga(Guid id)
        {
            using (var dbContext = _sagaDbContextFactory.Create())
            {
                var sagaInstance = dbContext.Set<ShoppingChore>().SingleOrDefault(x => x.CorrelationId == id);
                return sagaInstance;
            }
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ConcurrentMessageLimit = 16;
        }
    }


    class ShoppingChoreSagaDbContext : SagaDbContext
    {
        public ShoppingChoreSagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        protected override IEnumerable<ISagaClassMap> Configurations
        {
            get { yield return new EntityFrameworkShoppingChoreMap(); }
        }


        class EntityFrameworkShoppingChoreMap :
            SagaClassMap<ShoppingChore>
        {
            protected override void Configure(EntityTypeConfiguration<ShoppingChore> entity, DbModelBuilder modelBuilder)
            {
                entity.Property(x => x.CurrentState);
                entity.Property(x => x.Everything);

                entity.Property(x => x.Screwed);
            }
        }
    }
}

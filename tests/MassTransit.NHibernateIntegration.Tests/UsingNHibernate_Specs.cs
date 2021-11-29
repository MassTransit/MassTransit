namespace MassTransit.NHibernateIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using NHibernate;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class When_using_NHibernateRepository :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_removed_the_state_machine()
        {
            var correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling { CorrelationId = correlationId });

            Guid? sagaId = await _repository.ShouldContainSaga(correlationId, TestTimeout);
            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new SodOff { CorrelationId = correlationId });

            sagaId = await _repository.ShouldNotContainSaga(correlationId, TestTimeout);
            Assert.IsFalse(sagaId.HasValue);
        }

        [Test]
        public async Task Should_have_the_state_machine()
        {
            var correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling { CorrelationId = correlationId });

            Guid? sagaId = await _repository.ShouldContainSaga(correlationId, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new GotHitByACar { CorrelationId = correlationId });

            sagaId = await _repository.ShouldContainSagaInState(correlationId, _machine, x => x.Dead, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            var instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
        }

        SuperShopper _machine;
        ISessionFactory _sessionFactory;
        ISagaRepository<ShoppingChore> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new SuperShopper();

            _sessionFactory = new SQLiteSessionFactoryProvider(typeof(ShoppingChoreMap))
                .GetSessionFactory();
            _repository = NHibernateSagaRepository<ShoppingChore>.Create(_sessionFactory);

            configurator.StateMachineSaga(_machine, _repository);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _sessionFactory.Dispose();
        }

        Task<ShoppingChore> GetSaga(Guid id)
        {
            using (var session = _sessionFactory.OpenSession())
            {
                var result = session.QueryOver<ShoppingChore>()
                    .Where(x => x.CorrelationId == id)
                    .SingleOrDefault<ShoppingChore>();

                return Task.FromResult(result);
            }
        }


        class ShoppingChoreMap :
            SagaClassMapping<ShoppingChore>
        {
            public ShoppingChoreMap()
            {
                Lazy(false);
                Table("ShoppingChore");

                Property(x => x.CurrentState);
                Property(x => x.Everything);
                Property(x => x.Screwed);
            }
        }
    }
}

namespace MassTransit.NHibernateIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Automatonymous;
    using MassTransit.Saga;
    using NHibernate;
    using NHibernateIntegration;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    public class When_using_NHibernateRepository :
        InMemoryTestFixture
    {
        SuperShopper _machine;
        ISessionFactory _sessionFactory;
        ISagaRepository<ShoppingChore> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new SuperShopper();

            _sessionFactory = new SQLiteSessionFactoryProvider(typeof(ShoppingChoreMap))
                .GetSessionFactory();
            _repository = new NHibernateSagaRepository<ShoppingChore>(_sessionFactory);

            configurator.StateMachineSaga(_machine, _repository);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _sessionFactory.Dispose();
        }

        async Task RaiseEvent<T>(Guid id, Event<T> @event, T data)
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var instance = session.Get<ShoppingChore>(id, LockMode.Upgrade);
                if (instance == null)
                    instance = new ShoppingChore(id);

                await _machine.RaiseEvent(instance, @event, data);

                session.SaveOrUpdate(instance);

                transaction.Commit();
            }
        }

        Task<ShoppingChore> GetSaga(Guid id)
        {
            using (ISession session = _sessionFactory.OpenSession())
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


        [Test]
        public async Task Should_have_removed_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling {CorrelationId = correlationId});

            Guid? sagaId = await _repository.ShouldContainSaga(correlationId, TestTimeout);
            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new SodOff {CorrelationId = correlationId});

            sagaId = await _repository.ShouldNotContainSaga(correlationId, TestTimeout);
            Assert.IsFalse(sagaId.HasValue);
        }

        [Test]
        public async Task Should_have_the_state_machine()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling {CorrelationId = correlationId});

            Guid? sagaId = await _repository.ShouldContainSaga(correlationId, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            await InputQueueSendEndpoint.Send(new GotHitByACar {CorrelationId = correlationId});

            sagaId = await _repository.ShouldContainSaga(x => x.CorrelationId == correlationId && x.CurrentState == _machine.Dead.Name, TestTimeout);

            Assert.IsTrue(sagaId.HasValue);

            ShoppingChore instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
        }
    }
}

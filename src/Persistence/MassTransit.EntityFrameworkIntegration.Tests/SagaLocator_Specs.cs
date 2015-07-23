namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using MassTransit.Saga;
    using MassTransit.Tests.Saga.Messages;
    using NUnit.Framework;
    using Saga;
    using Shouldly;
    using TestFramework;
    using Mehdime.Entity;


    [TestFixture, Category("Integration")]
    public class Locating_an_existing_ef_saga :
        InMemoryTestFixture
    {
        [Test]
        public async void A_correlated_message_should_find_the_correct_saga()
        {
            Guid sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async void An_initiating_message_should_start_the_saga()
        {
            Guid sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        readonly IDbContextScopeFactory _dbContextScopeFactory;
        readonly Lazy<ISagaRepository<SimpleSagaEntity>> _sagaRepository;

        public Locating_an_existing_ef_saga()
        {
            _dbContextScopeFactory = new DbContextScopeFactory(new SagaDbContextFactoryProvider());
            _sagaRepository = new Lazy<ISagaRepository<SimpleSagaEntity>>(() => new EntityFrameworkSagaRepository<SimpleSagaEntity>(_dbContextScopeFactory));
        }

        [TestFixtureSetUp]
        public void Setup()
        {
        }

        [TestFixtureTearDown]
        public void Teardown()
        {
        }

        protected override void ConfigureInputQueueEndpoint(IReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }

    }
}

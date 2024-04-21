namespace MassTransit.EntityFrameworkIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Tests;
    using MassTransit.Tests.Saga;
    using MassTransit.Tests.Saga.Messages;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class Locating_an_existing_ef_saga :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId, Is.Not.Null);

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            Assert.That(foundId, Is.Not.Null);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId, Is.Not.Null);
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public Locating_an_existing_ef_saga()
        {
            var sagaDbContextFactory = new DelegateSagaDbContextFactory<SimpleSaga>(() =>
                new SimpleSagaDbContext(LocalDbConnectionStringProvider.GetLocalDbConnectionString()));

            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => EntityFrameworkSagaRepository<SimpleSaga>.CreatePessimistic(sagaDbContextFactory));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}

namespace MassTransit.Azure.Table.Tests.Saga
{
    using System;
    using System.Threading.Tasks;
    using AzureTable.Saga;
    using NUnit.Framework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSaga :
        AzureTableInMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(found, Is.Not.Null);

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(sagaId, x => x != null && x.Moved, TestTimeout);
            Assert.That(found, Is.Not.Null);

            var retrieveRepository = _sagaRepository.Value as ILoadSagaRepository<SimpleSaga>;
            var retrieved = await retrieveRepository.Load(sagaId);
            Assert.That(retrieved, Is.Not.Null);
            Assert.That(retrieved.Moved, Is.True);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(found, Is.Not.Null);
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public LocatingAnExistingSaga()
        {
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => AzureTableSagaRepository<SimpleSaga>.Create(() => TestCloudTable));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}

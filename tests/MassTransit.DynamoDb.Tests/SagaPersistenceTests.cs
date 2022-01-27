namespace MassTransit.DynamoDb.Tests
{
    using System;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using NUnit.Framework;
    using Saga;
    using Shouldly;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSaga : InMemoryTestFixture
    {
        readonly ISagaRepository<SimpleSaga> _sagaRepository;

        public LocatingAnExistingSaga()
        {
            var dynamoDb = new AmazonDynamoDBClient(new AmazonDynamoDBConfig {ServiceURL = "http://localhost:8000"});
            _sagaRepository = DynamoDbSagaRepository<SimpleSaga>.Create(() => new DynamoDBContext(dynamoDb), "Mass", TimeSpan.FromDays(1));
        }

        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();

            var nextMessage = new CompleteSimpleSaga {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.ShouldContainSaga(sagaId, x => x != null && x.Moved, TestTimeout);
            found.ShouldNotBeNull();

            var retrieveRepository = _sagaRepository as ILoadSagaRepository<SimpleSaga>;
            var retrieved = await retrieveRepository.Load(sagaId);
            retrieved.ShouldNotBeNull();
            retrieved.Moved.ShouldBeTrue();
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository);
        }
    }
}

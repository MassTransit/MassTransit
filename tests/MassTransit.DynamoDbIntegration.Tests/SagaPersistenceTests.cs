namespace MassTransit.DynamoDb.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.DataModel;
    using Amazon.DynamoDBv2.Model;
    using DynamoDbIntegration;
    using DynamoDbIntegration.Saga;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSaga : InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

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

        [SetUp]
        public async Task CreateTableIfNotExists()
        {
            try
            {
                await DynamoDbClient.DescribeTableAsync(nameof(LocatingAnExistingSaga), CancellationToken.None);
                await ClearTable();
            }
            catch (ResourceNotFoundException)
            {
                await DynamoDbClient.CreateTableAsync(new CreateTableRequest
                {
                    BillingMode = BillingMode.PAY_PER_REQUEST,
                    KeySchema = new List<KeySchemaElement>
                    {
                        new KeySchemaElement("PK", KeyType.HASH),
                        new KeySchemaElement("SK", KeyType.RANGE)
                    },
                    TableName = nameof(LocatingAnExistingSaga),
                    AttributeDefinitions = new List<AttributeDefinition>
                    {
                        new AttributeDefinition("PK", ScalarAttributeType.S),
                        new AttributeDefinition("SK", ScalarAttributeType.S)
                    }
                });
            }
        }

        [TearDown]
        public async Task ClearTable()
        {
            var response = await DynamoDbClient.ScanAsync(nameof(LocatingAnExistingSaga), new List<string>
            {
                "PK",
                "SK"
            }, CancellationToken.None);

            foreach (Dictionary<string, AttributeValue> item in response.Items)
            {
                await DynamoDbClient.DeleteItemAsync(nameof(LocatingAnExistingSaga),
                    new Dictionary<string, AttributeValue>
                    {
                        { "PK", new AttributeValue(item["PK"].S) },
                        { "SK", new AttributeValue(item["SK"].S) }
                    });
            }
        }

        readonly ISagaRepository<SimpleSaga> _sagaRepository;

        public LocatingAnExistingSaga()
        {
            DynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig { ServiceURL = "http://localhost:4566" });
            DynamoDbContext = new DynamoDBContext(DynamoDbClient);
            _sagaRepository = DynamoDbSagaRepository<SimpleSaga>.Create(() => DynamoDbContext, nameof(LocatingAnExistingSaga));
        }

        public DynamoDBContext DynamoDbContext { get; }

        public AmazonDynamoDBClient DynamoDbClient { get; }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository);
        }
    }
}

namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using AzureCosmos;
    using AzureCosmos.Saga;
    using Microsoft.Azure.Cosmos;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Category("Cosmos")]
    public class When_using_Cosmos :
        InMemoryTestFixture
    {
        [Test]
        public async Task Should_have_removed_the_state_machine()
        {
            var correlationId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling { CorrelationId = correlationId });

            var saga = await GetSagaRetry(correlationId, TestTimeout);
            Assert.IsNotNull(saga);

            await InputQueueSendEndpoint.Send(new SodOff { CorrelationId = correlationId });

            saga = await GetNoSagaRetry(correlationId, TestTimeout);
            Assert.IsNull(saga);
        }

        [Test]
        public async Task Should_have_the_state_machine()
        {
            var correlationId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new GirlfriendYelling { CorrelationId = correlationId });

            var saga = await GetSagaRetry(correlationId, TestTimeout);

            Assert.IsNotNull(saga);

            await InputQueueSendEndpoint.Send(new GotHitByACar { CorrelationId = correlationId });

            saga = await GetSagaRetry(correlationId, TestTimeout, x => x.CurrentState == _machine.Dead.Name);

            Assert.IsNotNull(saga);
            Assert.IsTrue(saga.CurrentState == _machine.Dead.Name);

            var instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.Screwed);
        }

        SuperShopper _machine;
        Database _database;
        Container _container;
        readonly CosmosClient _cosmosClient;
        readonly string _databaseName;
        readonly string _collectionName;
        readonly Lazy<ISagaRepository<ShoppingChore>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new SuperShopper();

            configurator.UseRetry(x =>
            {
                x.Immediate(5);
            });
            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_Cosmos()
        {
            _databaseName = "shoppingChoreSagas";
            _collectionName = "sagas";
            _cosmosClient = new CosmosClient(Configuration.EndpointUri, Configuration.Key,
                new CosmosClientOptions
                {
                    Serializer = new CosmosJsonDotNetSerializer(JsonSerializerSettingsExtensions.GetSagaRenameSettings<ShoppingChore>())
                });

            _repository = new Lazy<ISagaRepository<ShoppingChore>>(() =>
                CosmosSagaRepository<ShoppingChore>.Create(_cosmosClient, _databaseName, _collectionName));
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dbResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName).ConfigureAwait(false);
            _database = dbResponse.Database;
            var cResponse = await _database
                .CreateContainerIfNotExistsAsync(_collectionName, "/id")
                .ConfigureAwait(false);
            _container = cResponse.Container;
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _container.DeleteContainerAsync().ConfigureAwait(false);
            await _database.DeleteAsync().ConfigureAwait(false);
        }

        async Task<ShoppingChore> GetSaga(Guid id)
        {
            try
            {
                ItemResponse<ShoppingChore> document = await _container.ReadItemAsync<ShoppingChore>(id.ToString(), new PartitionKey(id.ToString()));
                return document.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        async Task<ShoppingChore> GetNoSagaRetry(Guid id, TimeSpan timeout)
        {
            var giveUpAt = DateTime.Now + timeout;
            ShoppingChore saga = null;

            while (DateTime.Now < giveUpAt)
            {
                try
                {
                    ItemResponse<ShoppingChore> document = await _container.ReadItemAsync<ShoppingChore>(id.ToString(), new PartitionKey(id.ToString()));

                    saga = document.Resource;

                    await Task.Delay(10).ConfigureAwait(false);
                }
                catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    saga = null;
                    break;
                }
            }

            return saga;
        }

        async Task<ShoppingChore> GetSagaRetry(Guid id, TimeSpan timeout, Func<ShoppingChore, bool> filterExpression = null)
        {
            var giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                try
                {
                    ItemResponse<ShoppingChore> document = await _container.ReadItemAsync<ShoppingChore>(id.ToString(), new PartitionKey(id.ToString()));
                    var saga = document.Resource;

                    if (filterExpression?.Invoke(saga) == false)
                        continue;
                    return saga;
                }
                catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await Task.Delay(10).ConfigureAwait(false);
                }
            }

            return null;
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.ConcurrentMessageLimit = 16;
        }
    }
}

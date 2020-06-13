namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Azure.Cosmos.Saga;
    using MassTransit.Saga;
    using Microsoft.Azure.Cosmos;
    using NUnit.Framework;
    using TestFramework;


    [TestFixture]
    [Category("Cosmos")]
    public class When_using_CosmosConcurrencyNoRetry :
        InMemoryTestFixture
    {
        ChoirStateMachine _machine;
        private Database _database;
        private Container _container;
        readonly CosmosClient _cosmosClient;
        readonly string _databaseName;
        readonly string _collectionName;
        readonly Lazy<ISagaRepository<ChoirStateOptimistic>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new ChoirStateMachine();

            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_CosmosConcurrencyNoRetry()
        {
            _databaseName = "choirSagas";
            _collectionName = "sagas";
            _cosmosClient = new CosmosClient(Configuration.EndpointUri, Configuration.Key, new CosmosClientOptions { Serializer = new CosmosJsonDotNetSerializer(JsonSerializerSettingsExtensions.GetSagaRenameSettings<ChoirStateOptimistic>()) });

            _repository = new Lazy<ISagaRepository<ChoirStateOptimistic>>(() => CosmosSagaRepository<ChoirStateOptimistic>.Create(_cosmosClient,
                _databaseName));

            TestTimeout = TimeSpan.FromMinutes(3);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var dbResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseName).ConfigureAwait(false);
            _database = dbResponse.Database;
            var cResponse = await _database
                .CreateContainerAsync(_collectionName, "/id")
                .ConfigureAwait(false);
            _container = cResponse.Container;
        }

        [OneTimeTearDown]
        public async Task Teardown()
        {
            await _container.DeleteContainerAsync().ConfigureAwait(false);
            await _database.DeleteAsync().ConfigureAwait(false);
        }

        async Task<ChoirStateOptimistic> GetSaga(Guid id)
        {
            try
            {
                var document = await _container.ReadItemAsync<ChoirStateOptimistic>(id.ToString(), new PartitionKey(id.ToString()));
                return document.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        async Task<ChoirStateOptimistic> GetSagaRetry(Guid id, TimeSpan timeout, Func<ChoirStateOptimistic, bool> filterExpression = null)
        {
            DateTime giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                try
                {
                    var document = await _container.ReadItemAsync<ChoirStateOptimistic>(id.ToString(), new PartitionKey(id.ToString()));

                    var saga = document.Resource;

                    if (filterExpression?.Invoke(saga) == false)
                        continue;
                    return saga;
                }
                catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await Task.Delay(20).ConfigureAwait(false);
                }
            }

            return null;
        }

        [Test]
        public async Task Some_should_not_be_in_final_state_all()
        {
            var tasks = new List<Task>();

            Guid[] sagaIds = new Guid[20];
            for (int i = 0; i < 20; i++)
            {
                Guid correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new RehersalBegins {CorrelationId = correlationId});

                sagaIds[i] = correlationId;
            }

            for (int i = 0; i < 20; i++)
            {
                var saga = await GetSagaRetry(sagaIds[i], TestTimeout);
                Assert.IsNotNull(saga);
            }

            for (int i = 0; i < 20; i++)
            {
                tasks.Add(InputQueueSendEndpoint.Send(new Bass
                {
                    CorrelationId = sagaIds[i],
                    Name = "John"
                }));
                tasks.Add(InputQueueSendEndpoint.Send(new Baritone
                {
                    CorrelationId = sagaIds[i],
                    Name = "Mark"
                }));
                tasks.Add(InputQueueSendEndpoint.Send(new Tenor
                {
                    CorrelationId = sagaIds[i],
                    Name = "Anthony"
                }));
                tasks.Add(InputQueueSendEndpoint.Send(new Countertenor
                {
                    CorrelationId = sagaIds[i],
                    Name = "Tom"
                }));
            }

            await Task.WhenAll(tasks);
            await Task.Delay(2000);
            tasks.Clear();

            var someNotInFinalState = false;

            foreach (var sid in sagaIds)
            {
                ChoirStateOptimistic instance = await GetSaga(sid);

                someNotInFinalState = !instance.CurrentState.Equals("Harmony");
                if (someNotInFinalState)
                    break;
            }

            Assert.IsTrue(someNotInFinalState);
        }

        [Test]
        public async Task Should_not_be_in_final_state()
        {
            Guid correlationId = Guid.NewGuid();

            await InputQueueSendEndpoint.Send(new RehersalBegins {CorrelationId = correlationId});

            var saga = await GetSagaRetry(correlationId, TestTimeout);

            Assert.IsNotNull(saga);

            await Task.WhenAll(
                InputQueueSendEndpoint.Send(new Bass
                {
                    CorrelationId = correlationId,
                    Name = "John"
                }),
                InputQueueSendEndpoint.Send(new Baritone
                {
                    CorrelationId = correlationId,
                    Name = "Mark"
                }),
                InputQueueSendEndpoint.Send(new Tenor
                {
                    CorrelationId = correlationId,
                    Name = "Anthony"
                }),
                InputQueueSendEndpoint.Send(new Countertenor
                {
                    CorrelationId = correlationId,
                    Name = "Tom"
                })
            );

            // Because concurrency exception's happened without retry middleware configured, we aren't in our final state/
            ChoirStateOptimistic instance = await GetSaga(correlationId);

            Assert.IsTrue(!instance.CurrentState.Equals("Harmony"));
        }

        protected override void ConfigureInMemoryBus(IInMemoryBusFactoryConfigurator configurator)
        {
            base.ConfigureInMemoryBus(configurator);

            configurator.TransportConcurrencyLimit = 16;
        }
    }
}

namespace MassTransit.Azure.Cosmos.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;
    using AzureCosmos;
    using AzureCosmos.Saga;
    using Microsoft.Azure.Cosmos;
    using NUnit.Framework;
    using TestFramework;


    // Both of these tests will pass in Debug, because the TestTimeout is 50 minutes, which is enough time for all concurrency to settle. Marked as explicit so they don't run on Appveyor
    [TestFixture]
    [Category("Cosmos")]
    public class When_using_CosmosConcurrencyOptimistic :
        InMemoryTestFixture
    {
        [Test]
        [Explicit]
        public async Task Should_capture_all_events_many_sagas()
        {
            var tasks = new List<Task>();

            var sagaIds = new Guid[20];
            for (var i = 0; i < 20; i++)
            {
                var correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

                sagaIds[i] = correlationId;
            }

            for (var i = 0; i < 20; i++)
            {
                var saga = await GetSagaRetry(sagaIds[i], TestTimeout);
                Assert.IsNotNull(saga);
            }

            for (var i = 0; i < 20; i++)
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
            await Task.Delay(100000);
            tasks.Clear();


            foreach (var sid in sagaIds)
            {
                var instance = await GetSagaRetry(sid, TestTimeout, x => x.CurrentState == _machine.Harmony.Name);

                Assert.IsNotNull(instance);
                Assert.IsTrue(instance.CurrentState.Equals("Harmony"));
            }
        }

        [Test]
        [Explicit]
        public async Task Should_capture_all_events_single_saga()
        {
            var correlationId = NewId.NextGuid();

            await InputQueueSendEndpoint.Send(new RehersalBegins { CorrelationId = correlationId });

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

            saga = await GetSagaRetry(correlationId, TestTimeout, x => x.CurrentState == _machine.Harmony.Name);

            Assert.IsNotNull(saga);
            Assert.IsTrue(saga.CurrentState == _machine.Harmony.Name);

            var instance = await GetSaga(correlationId);

            Assert.IsTrue(instance.CurrentState.Equals("Harmony"));
        }

        ChoirStateMachine _machine;
        Database _database;
        Container _container;
        readonly CosmosClient _cosmosClient;
        readonly string _databaseName;
        readonly string _collectionName;
        readonly Lazy<ISagaRepository<ChoirStateOptimistic>> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _machine = new ChoirStateMachine();

            configurator.UseRetry(x =>
            {
                x.Handle<CosmosConcurrencyException>();
                x.Interval(5, 300);
            });
            configurator.StateMachineSaga(_machine, _repository.Value);
        }

        public When_using_CosmosConcurrencyOptimistic()
        {
            _databaseName = "choirSagas";
            _collectionName = "sagas";
            _cosmosClient = new CosmosClient(Configuration.EndpointUri, Configuration.Key,
                new CosmosClientOptions
                {
                    Serializer = new CosmosJsonDotNetSerializer(JsonSerializerSettingsExtensions.GetSagaRenameSettings<ChoirStateOptimistic>())
                });

            _repository = new Lazy<ISagaRepository<ChoirStateOptimistic>>(() =>
                CosmosSagaRepository<ChoirStateOptimistic>.Create(_cosmosClient, _databaseName, _collectionName));
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

        async Task<ChoirStateOptimistic> GetSaga(Guid id)
        {
            try
            {
                ItemResponse<ChoirStateOptimistic> document =
                    await _container.ReadItemAsync<ChoirStateOptimistic>(id.ToString(), new PartitionKey(id.ToString()));
                return document.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        async Task<ChoirStateOptimistic> GetSagaRetry(Guid id, TimeSpan timeout, Func<ChoirStateOptimistic, bool> filterExpression = null)
        {
            var giveUpAt = DateTime.Now + timeout;

            while (DateTime.Now < giveUpAt)
            {
                try
                {
                    ItemResponse<ChoirStateOptimistic> document =
                        await _container.ReadItemAsync<ChoirStateOptimistic>(id.ToString(), new PartitionKey(id.ToString()));

                    var saga = document.Resource;

                    if (filterExpression?.Invoke(saga) == false)
                        continue;
                    return saga;
                }
                catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    await Task.Delay(400).ConfigureAwait(false);
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

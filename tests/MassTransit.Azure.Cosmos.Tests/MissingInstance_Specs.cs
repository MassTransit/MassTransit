namespace MassTransit.Azure.Cosmos.Tests
{
    namespace MissingTests
    {
        using System;
        using System.Threading.Tasks;
        using AzureCosmos;
        using AzureCosmos.Saga;
        using Internals;
        using Microsoft.Azure.Cosmos;
        using Microsoft.Extensions.DependencyInjection;
        using NUnit.Framework;
        using TestFramework;
        using Testing;


        class InstanceNotFound
        {
            public InstanceNotFound(string serviceName)
            {
                ServiceName = serviceName;
            }

            public string ServiceName { get; set; }
        }


        class Status
        {
            public Status(string statusDescription, string serviceName)
            {
                StatusDescription = statusDescription;
                ServiceName = serviceName;
            }

            public string ServiceName { get; set; }
            public string StatusDescription { get; set; }
        }


        public class CheckStatus
        {
            public CheckStatus(string serviceName)
            {
                ServiceName = serviceName;
            }

            public CheckStatus()
            {
            }

            public string ServiceName { get; set; }
        }


        public class Start
        {
            public Start(string serviceName, Guid serviceId)
            {
                ServiceName = serviceName;
                ServiceId = serviceId;
            }

            public string ServiceName { get; set; }
            public Guid ServiceId { get; set; }
        }


        class StartupComplete
        {
            public Guid ServiceId { get; set; }
            public string ServiceName { get; set; }
        }


        public class MissingInstance :
            SagaStateMachineInstance
        {
            public MissingInstance(Guid correlationId)
            {
                CorrelationId = correlationId;
            }

            protected MissingInstance()
            {
            }

            public string CurrentState { get; set; }
            public string ServiceName { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class MissingInstanceStateMachine :
            MassTransitStateMachine<MissingInstance>
        {
            public MissingInstanceStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Started, x => x
                    .CorrelateBy(instance => instance.ServiceName, context => context.Message.ServiceName)
                    .SelectId(context => context.Message.ServiceId));

                Event(() => CheckStatus, x =>
                {
                    x.CorrelateBy(instance => instance.ServiceName, context => context.Message.ServiceName);

                    x.OnMissingInstance(m =>
                    {
                        return m.ExecuteAsync(context => context.RespondAsync(new InstanceNotFound(context.Message.ServiceName)));
                    });
                });

                Initially(
                    When(Started)
                        .Then(context => context.Instance.ServiceName = context.Data.ServiceName)
                        .Respond(context => new StartupComplete
                        {
                            ServiceId = context.Instance.CorrelationId,
                            ServiceName = context.Instance.ServiceName
                        })
                        .Then(context => Console.WriteLine("Started: {0} - {1}", context.Instance.CorrelationId, context.Instance.ServiceName))
                        .TransitionTo(Running));

                During(Running,
                    When(CheckStatus)
                        .Then(context => Console.WriteLine("Status check!"))
                        .Respond(context => new Status("Running", context.Instance.ServiceName)));
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<CheckStatus> CheckStatus { get; private set; }
        }


        [TestFixture]
        public class Testing_with_the_container_and_query_linq
        {
            [Test]
            public async Task Should_work_as_expected()
            {
                await using var provider = new ServiceCollection()
                    .AddMassTransitTestHarness(x =>
                    {
                        x.AddSagaStateMachine<MissingInstanceStateMachine, MissingInstance>()
                            .CosmosRepository(r =>
                            {
                                r.AccountEndpoint = Configuration.AccountEndpoint;
                                r.AuthKeyOrResourceToken = Configuration.AccountKey;

                                r.DatabaseId = "sagaTest";
                                r.CollectionId = "TestInstance";
                            });

                        x.AddConfigureEndpointsCallback((name, configurator) =>
                        {
                            configurator.UseInMemoryOutbox();
                        });
                    })
                    .BuildServiceProvider(true);

                var harness = provider.GetTestHarness();

                await harness.Start();

                var correlationId = NewId.NextGuid();

                IRequestClient<Start> startClient = harness.GetRequestClient<Start>();

                await startClient.GetResponse<StartupComplete>(new Start("A", correlationId));

                IRequestClient<CheckStatus> statusClient = harness.GetRequestClient<CheckStatus>();

                Response<Status, InstanceNotFound> response = await statusClient.GetResponse<Status, InstanceNotFound>(new CheckStatus("A"));

                Assert.Multiple(() =>
                {
                    Assert.That(response.Is(out MassTransit.Response<Status> status), Is.True);

                    Assert.That(status.Message.ServiceName, Is.EqualTo("A"));
                });
            }

            readonly string _databaseName;
            readonly string _collectionName;
            readonly CosmosClient _cosmosClient;
            Database _database;
            Container _container;

            public Testing_with_the_container_and_query_linq()
            {
                _databaseName = "sagaTest";
                _collectionName = "TestInstance";
                _cosmosClient = new CosmosClient(Configuration.AccountEndpoint, Configuration.AccountKey,
                    new CosmosClientOptions
                    {
                        Serializer = new SystemTextJsonCosmosSerializer(AzureCosmosSerializerExtensions.GetSerializerOptions<MissingInstance>())
                    });
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
        }


        [TestFixture]
        public class When_matching_to_an_instance :
            InMemoryTestFixture
        {
            [Test]
            public async Task Should_match_an_existing_instance()
            {
                IRequestClient<Start> startClient = Bus.CreateRequestClient<Start>(InputQueueAddress, TestTimeout);

                await startClient.GetResponse<StartupComplete>(new Start("A", NewId.NextGuid()));

                IRequestClient<CheckStatus> statusClient = Bus.CreateRequestClient<CheckStatus>(InputQueueAddress, TestTimeout);

                (Task<MassTransit.Response<Status>> status, Task<MassTransit.Response<InstanceNotFound>> notFound) =
                    await statusClient.GetResponse<Status, InstanceNotFound>(new CheckStatus("A"), TestCancellationToken);

                Assert.That(status.IsCompletedSuccessfully(), Is.True);

                MassTransit.Response<Status> result = await status;

                Assert.That(result.Message.ServiceName, Is.EqualTo("A"));

                Assert.That(async () => await notFound, Throws.TypeOf<TaskCanceledException>());
            }

            [Test]
            public async Task Should_publish_the_event_of_the_missing_instance()
            {
                IRequestClient<CheckStatus> requestClient = Bus.CreateRequestClient<CheckStatus>(InputQueueAddress, TestTimeout);

                (Task<MassTransit.Response<Status>> status, Task<MassTransit.Response<InstanceNotFound>> notFound) =
                    await requestClient.GetResponse<Status, InstanceNotFound>(new CheckStatus("Z"), TestCancellationToken);

                Assert.That(notFound.IsCompletedSuccessfully(), Is.True);

                MassTransit.Response<InstanceNotFound> result = await notFound;

                Assert.That(result.Message.ServiceName, Is.EqualTo("Z"));

                Assert.That(async () => await status, Throws.TypeOf<TaskCanceledException>());
            }

            Database _database;
            Container _container;
            readonly CosmosClient _cosmosClient;
            readonly string _databaseName;
            readonly string _collectionName;
            readonly Lazy<ISagaRepository<MissingInstance>> _repository;

            public When_matching_to_an_instance()
            {
                _databaseName = "masstransitunittests";
                _collectionName = "sagas";
                _cosmosClient = new CosmosClient(Configuration.AccountEndpoint, Configuration.AccountKey,
                    new CosmosClientOptions
                    {
                        Serializer = new NewtonsoftJsonCosmosSerializer(AzureCosmosSerializerExtensions.GetSagaRenameSettings<MissingInstance>())
                    });

                _repository = new Lazy<ISagaRepository<MissingInstance>>(() =>
                    CosmosSagaRepository<MissingInstance>.Create(_cosmosClient, _databaseName, _collectionName));
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

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                _machine = new MissingInstanceStateMachine();

                configurator.UseMessageRetry(r => r.Intervals(1000, 2000));
                configurator.UseInMemoryOutbox();
                configurator.StateMachineSaga(_machine, _repository.Value);
            }

            MissingInstanceStateMachine _machine;
        }
    }
}

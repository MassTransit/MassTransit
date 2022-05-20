namespace MassTransit.DynamoDb.Tests
{
    namespace ContainerTests
    {
        using System;
        using System.Collections.Generic;
        using System.Threading;
        using System.Threading.Tasks;
        using Amazon.DynamoDBv2;
        using Amazon.DynamoDBv2.DataModel;
        using Amazon.DynamoDBv2.Model;
        using Microsoft.Extensions.DependencyInjection;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Sagas.ChoirConcurrency;
        using Testing;


        public class When_testing_concurrency_with_the_choir :
            InMemoryTestFixture
        {
            readonly IServiceProvider _provider;

            public When_testing_concurrency_with_the_choir()
            {
                _provider = new ServiceCollection()
                    .AddMassTransit(ConfigureRegistration)
                    .BuildServiceProvider();
            }

            public AmazonDynamoDBClient DynamoDbClient { get; set; }

            [Test]
            public async Task Should_work_as_expected()
            {
                var correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new RehearsalBegins { CorrelationId = correlationId });

                var machine = _provider.GetRequiredService<ChoirStateMachine>();
                var repository = _provider.GetRequiredService<ISagaRepository<ChoirState>>();

                Guid? sagaId = await repository.ShouldContainSaga(correlationId, x => x.CurrentState.Equals(machine.Warmup.Name), TestTimeout);
                Assert.That(sagaId.HasValue, Is.True);

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

                sagaId = await repository.ShouldContainSaga(correlationId, x => x.CurrentState.Equals(machine.Harmony.Name), TestTimeout);
                Assert.That(sagaId.HasValue, Is.True);
            }

            [SetUp]
            public async Task CreateTableIfNotExists()
            {
                try
                {
                    await DynamoDbClient.DescribeTableAsync(nameof(ChoirState), CancellationToken.None);
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
                        TableName = nameof(ChoirState),
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
                var response = await DynamoDbClient.ScanAsync(nameof(ChoirState), new List<string>
                {
                    "PK",
                    "SK"
                }, CancellationToken.None);

                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    await DynamoDbClient.DeleteItemAsync(nameof(ChoirState),
                        new Dictionary<string, AttributeValue>
                        {
                            { "PK", new AttributeValue(item["PK"].S) },
                            { "SK", new AttributeValue(item["SK"].S) }
                        });
                }
            }

            void ConfigureRegistration(IBusRegistrationConfigurator configurator)
            {
                DynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig { ServiceURL = "http://localhost:4566" });

                configurator.AddSagaStateMachine<ChoirStateMachine, ChoirState>()
                    .DynamoDbRepository(r =>
                    {
                        r.ContextFactory(provider => new DynamoDBContext(DynamoDbClient));

                        r.TableName = nameof(ChoirState);
                    });

                configurator.AddBus(provider => BusControl);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.UseMessageRetry(r => r.Intervals(500, 1000, 2000, 2000, 2000));
                configurator.UseInMemoryOutbox();
                configurator.ConfigureSaga<ChoirState>(_provider.GetRequiredService<IBusRegistrationContext>());
            }
        }
    }
}

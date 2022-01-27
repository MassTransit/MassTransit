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
        using Internals;
        using Microsoft.Extensions.DependencyInjection;
        using NUnit.Framework;
        using TestFramework;
        using TestFramework.Sagas;


        public class Using_optimistic_concurrency :
            InMemoryTestFixture
        {
            readonly IServiceProvider _provider;

            public Using_optimistic_concurrency()
            {
                _provider = new ServiceCollection()
                    .AddMassTransit(ConfigureRegistration)
                    .AddScoped<PublishTestStartedActivity>().BuildServiceProvider();
            }

            public AmazonDynamoDBClient DynamoDbClient { get; set; }

            [Test]
            public async Task Should_work_as_expected()
            {
                Task<ConsumeContext<TestStarted>> started = await ConnectPublishHandler<TestStarted>();
                Task<ConsumeContext<TestUpdated>> updated = await ConnectPublishHandler<TestUpdated>();

                var correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new StartTest
                {
                    CorrelationId = correlationId,
                    TestKey = "Unique"
                });

                await started.OrCanceled(InactivityToken);

                await InputQueueSendEndpoint.Send(new UpdateTest
                {
                    TestId = correlationId,
                    TestKey = "Unique"
                });

                await updated.OrCanceled(InactivityToken);
            }

            [SetUp]
            public async Task CreateTableIfNotExists()
            {
                try
                {
                    await DynamoDbClient.DescribeTableAsync(nameof(TestInstance), CancellationToken.None);
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
                        TableName = nameof(TestInstance),
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
                var response = await DynamoDbClient.ScanAsync(nameof(TestInstance), new List<string>
                {
                    "PK",
                    "SK"
                }, CancellationToken.None);

                foreach (Dictionary<string, AttributeValue> item in response.Items)
                {
                    await DynamoDbClient.DeleteItemAsync(nameof(TestInstance),
                        new Dictionary<string, AttributeValue>
                        {
                            { "PK", new AttributeValue(item["PK"].S) },
                            { "SK", new AttributeValue(item["SK"].S) }
                        });
                }
            }

            protected void ConfigureRegistration(IBusRegistrationConfigurator configurator)
            {
                DynamoDbClient = new AmazonDynamoDBClient(new AmazonDynamoDBConfig { ServiceURL = "http://localhost:4566" });

                configurator.AddSagaStateMachine<TestStateMachineSaga, TestInstance>()
                    .DynamoDbRepository(r =>
                    {
                        r.ContextFactory(provider => new DynamoDBContext(DynamoDbClient));

                        r.TableName = nameof(TestInstance);
                    });

                configurator.AddBus(provider => BusControl);
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.UseInMemoryOutbox();
                configurator.ConfigureSaga<TestInstance>(_provider.GetRequiredService<IBusRegistrationContext>());
            }
        }


        public class TestInstance :
            SagaStateMachineInstance,
            ISagaVersion
        {
            public string CurrentState { get; set; }
            public string Key { get; set; }
            public int Version { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class TestStateMachineSaga :
            MassTransitStateMachine<TestInstance>
        {
            public TestStateMachineSaga()
            {
                InstanceState(x => x.CurrentState);

                Event(() => Updated, x => x.CorrelateById(m => m.Message.TestId));

                Initially(
                    When(Started)
                        .Then(context => context.Instance.Key = context.Data.TestKey)
                        .Activity(x => x.OfInstanceType<PublishTestStartedActivity>())
                        .TransitionTo(Active));

                During(Active,
                    When(Updated)
                        .Publish(context => new TestUpdated
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            TestKey = context.Instance.Key
                        })
                        .TransitionTo(Done)
                        .Finalize());

                SetCompletedWhenFinalized();
            }

            public State Active { get; private set; }
            public State Done { get; private set; }

            public Event<StartTest> Started { get; private set; }
            public Event<UpdateTest> Updated { get; private set; }
        }


        public class UpdateTest
        {
            public Guid TestId { get; set; }
            public string TestKey { get; set; }
        }


        public class PublishTestStartedActivity :
            IStateMachineActivity<TestInstance>
        {
            readonly ConsumeContext _context;

            public PublishTestStartedActivity(ConsumeContext context)
            {
                _context = context;
            }

            public void Probe(ProbeContext context)
            {
                context.CreateScope("publisher");
            }

            public void Accept(StateMachineVisitor visitor)
            {
                visitor.Visit(this);
            }

            public async Task Execute(BehaviorContext<TestInstance> context, IBehavior<TestInstance> next)
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public async Task Execute<T>(BehaviorContext<TestInstance, T> context, IBehavior<TestInstance, T> next)
                where T : class
            {
                await _context.Publish(new TestStarted
                {
                    CorrelationId = context.Instance.CorrelationId,
                    TestKey = context.Instance.Key
                }).ConfigureAwait(false);

                await next.Execute(context).ConfigureAwait(false);
            }

            public Task Faulted<TException>(BehaviorExceptionContext<TestInstance, TException> context, IBehavior<TestInstance> next)
                where TException : Exception
            {
                return next.Faulted(context);
            }

            public Task Faulted<T, TException>(BehaviorExceptionContext<TestInstance, T, TException> context, IBehavior<TestInstance, T> next)
                where TException : Exception
                where T : class
            {
                return next.Faulted(context);
            }
        }
    }
}

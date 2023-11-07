namespace MassTransit.CassandraDb.Tests
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.TestFramework;
    using MassTransit.TestFramework.Sagas.ChoirConcurrency;
    using MassTransit.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;


    namespace ContainerTests
    {
        using System.Linq;
        using Cassandra;
        using Cassandra.Data.Linq;
        using Cassandra.Mapping;
        using CassandraDbIntegration.Saga;
        using Configuration;


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

            public ISession Session { get; set; }

            [Test]
            public async Task Should_work_as_expected()
            {
                var correlationId = NewId.NextGuid();

                await InputQueueSendEndpoint.Send(new RehearsalBegins { CorrelationId = correlationId });

                var machine = _provider.GetRequiredService<ChoirStateMachine>();
                var repository = _provider.GetRequiredService<ISagaRepository<ChoirState>>();

                Guid? sagaId = await repository.ShouldContainSaga(correlationId, TestTimeout);
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
                Session.CreateKeyspaceIfNotExists("test");
                Session.ChangeKeyspace("test");
                var mapping = MappingConfiguration.Global.Get<CassandraDbSaga>();
                if (mapping == null)
                {
                    mapping = new Map<CassandraDbSaga>()
                        .PartitionKey(u => u.CorrelationId)
                        .TableName(nameof(ChoirState));
                    MappingConfiguration.Global.Define(mapping);
                }
                var config = new MappingConfiguration().Define(mapping);

                try
                {
                    await ClearTable();
                }
                catch (Exception e)
                {
                    var table = new Table<CassandraDbSaga>(Session, config, nameof(ChoirState));
                    await table.CreateAsync();
                }
            }

            [TearDown]
            public async Task ClearTable()
            {
                await Session.ExecuteAsync(new SimpleStatement($"TRUNCATE {nameof(ChoirState)}"));
            }

            void ConfigureRegistration(IBusRegistrationConfigurator configurator)
            {
                var clusterBuilder = Cluster.Builder()
                    .AddContactPoints("localhost".Split(","))
                    .WithPort(9042)
                    .WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalOne))
                    .WithCredentials("cassandra", "cassandra");

                var cluster = clusterBuilder.Build();
                Session = cluster.Connect();

                configurator.AddSagaStateMachine<ChoirStateMachine, ChoirState>()
                    .CassandraDbRepository(r =>
                    {
                        r.ContextFactory(provider => Session);
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

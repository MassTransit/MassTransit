namespace MassTransit.EntityFrameworkCoreIntegration.Tests
{
    namespace ReadOnlyTests
    {
        using System;
        using System.Collections.Generic;
        using System.Threading.Tasks;
        using Microsoft.EntityFrameworkCore;
        using Microsoft.EntityFrameworkCore.Design;
        using Microsoft.EntityFrameworkCore.Metadata.Builders;
        using NUnit.Framework;
        using Shared;


        public class ReadOnlySagaDbContext :
            SagaDbContext
        {
            public ReadOnlySagaDbContext(DbContextOptions options)
                : base(options)
            {
            }

            protected override IEnumerable<ISagaClassMap> Configurations
            {
                get { yield return new ReadOnlySagaMap(); }
            }
        }


        public class ReadOnlySagaDbContextFactory :
            IDesignTimeDbContextFactory<ReadOnlySagaDbContext>
        {
            public ReadOnlySagaDbContext CreateDbContext(string[] args)
            {
                DbContextOptionsBuilder<ReadOnlySagaDbContext> optionsBuilder = new SqlServerTestDbParameters()
                    .GetDbContextOptions<ReadOnlySagaDbContext>();

                return new ReadOnlySagaDbContext(optionsBuilder.Options);
            }

            public ReadOnlySagaDbContext CreateDbContext(DbContextOptionsBuilder optionsBuilder)
            {
                return new ReadOnlySagaDbContext(optionsBuilder.Options);
            }
        }


        public class ReadOnlySagaMap :
            SagaClassMap<ReadOnlyInstance>
        {
            protected override void Configure(EntityTypeBuilder<ReadOnlyInstance> entity, ModelBuilder model)
            {
                entity.Property(x => x.CurrentState);
                entity.Property(x => x.StatusText);
            }
        }


        [TestFixture(typeof(SqlServerTestDbParameters))]
        [TestFixture(typeof(PostgresTestDbParameters))]
        public class When_a_readonly_event_is_consumed<T> :
            EntityFrameworkTestFixture<T, ReadOnlySagaDbContext>
            where T : ITestDbParameters, new()
        {
            [Test]
            public async Task Should_not_update_the_saga_repository()
            {
                var serviceId = NewId.NextGuid();

                IRequestClient<Start> startClient = Bus.CreateRequestClient<Start>(InputQueueAddress, TestTimeout);

                await startClient.GetResponse<StartupComplete>(new Start { CorrelationId = serviceId }, TestCancellationToken);

                IRequestClient<CheckStatus> requestClient = Bus.CreateRequestClient<CheckStatus>(InputQueueAddress, TestTimeout);

                Response<Status> status = await requestClient.GetResponse<Status>(new CheckStatus { CorrelationId = serviceId }, TestCancellationToken);

                Assert.That(status.Message.StatusText, Is.EqualTo("Started"));

                status = await requestClient.GetResponse<Status>(new CheckStatus { CorrelationId = serviceId }, TestCancellationToken);

                Assert.That(status.Message.StatusText, Is.EqualTo("Started"));
            }

            protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
            {
                configurator.UseInMemoryOutbox();

                _machine = new ReadOnlyStateMachine();

                configurator.StateMachineSaga(_machine, CreateSagaRepository());
            }

            ISagaRepository<ReadOnlyInstance> CreateSagaRepository()
            {
                return EntityFrameworkSagaRepository<ReadOnlyInstance>
                    .CreatePessimistic(() => new ReadOnlySagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder), RawSqlLockStatements);
            }

            ReadOnlyStateMachine _machine;

            [OneTimeSetUp]
            public async Task Arrange()
            {
                await using var context = new ReadOnlySagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder);

                await context.Database.EnsureDeletedAsync();
                await context.Database.EnsureCreatedAsync();
            }

            [OneTimeTearDown]
            public async Task TearDown()
            {
                await using var context = new ReadOnlySagaDbContextFactory().CreateDbContext(DbContextOptionsBuilder);

                await context.Database.EnsureDeletedAsync();
            }
        }


        public class ReadOnlyInstance :
            SagaStateMachineInstance
        {
            public string CurrentState { get; set; }
            public string StatusText { get; set; }
            public Guid CorrelationId { get; set; }
        }


        class ReadOnlyStateMachine :
            MassTransitStateMachine<ReadOnlyInstance>
        {
            public ReadOnlyStateMachine()
            {
                InstanceState(x => x.CurrentState);

                Event(() => StatusCheckRequested, x =>
                {
                    x.ReadOnly = true;
                });

                Initially(
                    When(Started)
                        .Then(context => context.Instance.StatusText = "Started")
                        .Respond(context => new StartupComplete { CorrelationId = context.Instance.CorrelationId })
                        .TransitionTo(Running)
                );

                During(Running,
                    When(StatusCheckRequested)
                        .Respond(context => new Status
                        {
                            CorrelationId = context.Instance.CorrelationId,
                            StatusText = context.Instance.StatusText
                        })
                        .Then(context => context.Instance.StatusText = "Running")
                );
            }

            public State Running { get; private set; }
            public Event<Start> Started { get; private set; }
            public Event<CheckStatus> StatusCheckRequested { get; private set; }
        }


        public class Status :
            CorrelatedBy<Guid>
        {
            public string StatusText { get; set; }
            public Guid CorrelationId { get; set; }
        }


        public class CheckStatus :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        public class Start :
            CorrelatedBy<Guid>
        {
            public Guid CorrelationId { get; set; }
        }


        public class StartupComplete
        {
            public Guid CorrelationId { get; set; }
        }
    }
}

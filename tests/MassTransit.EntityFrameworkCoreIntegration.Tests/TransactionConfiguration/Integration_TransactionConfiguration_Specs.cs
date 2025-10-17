namespace MassTransit.EntityFrameworkCoreIntegration.Tests.TransactionConfiguration
{
    using System;
    using System.Threading.Tasks;
    using MassTransit.Tests.Saga;
    using MassTransit.Tests.Saga.Messages;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore;
    using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
    using NUnit.Framework;
    using Shared;
    using SimpleSaga.DataAccess;
    using Testing;


    /// <summary>
    /// Integration tests verifying optimistic concurrency with transactions disabled
    /// </summary>
    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    [Category("Integration")]
    public class Optimistic_saga_with_transactions_disabled<T> :
        EntityFrameworkTestFixture<T, SimpleSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task Should_work_with_optimistic_repository_and_transactions_disabled()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId, Is.Not.Null, "Saga should be created successfully with optimistic repository and no transactions");

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            Assert.That(foundId, Is.Not.Null, "Saga should be completed successfully with optimistic repository and no transactions");
        }

        [Test]
        public async Task Should_handle_multiple_saga_instances_without_transactions()
        {
            var saga1Id = NewId.NextGuid();
            var saga2Id = NewId.NextGuid();

            // Start two sagas without transactions
            await InputQueueSendEndpoint.Send(new InitiateSimpleSaga(saga1Id));
            await InputQueueSendEndpoint.Send(new InitiateSimpleSaga(saga2Id));

            Guid? found1Id = await _sagaRepository.Value.ShouldContainSaga(saga1Id, TestTimeout);
            Guid? found2Id = await _sagaRepository.Value.ShouldContainSaga(saga2Id, TestTimeout);

            Assert.That(found1Id, Is.Not.Null, "First saga should be created");
            Assert.That(found2Id, Is.Not.Null, "Second saga should be created");

            // Complete both sagas
            await InputQueueSendEndpoint.Send(new CompleteSimpleSaga { CorrelationId = saga1Id });
            await InputQueueSendEndpoint.Send(new CompleteSimpleSaga { CorrelationId = saga2Id });

            found1Id = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == saga1Id && x.Completed, TestTimeout);
            found2Id = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == saga2Id && x.Completed, TestTimeout);

            Assert.That(found1Id, Is.Not.Null, "First saga should be completed");
            Assert.That(found2Id, Is.Not.Null, "Second saga should be completed");
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public Optimistic_saga_with_transactions_disabled()
        {
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() =>
                EntityFrameworkSagaRepository<SimpleSaga>.CreateOptimistic(
                    () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                    null, false));
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var context = new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }


    /// <summary>
    /// Integration tests verifying optimistic concurrency with transactions enabled
    /// </summary>
    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    [Category("Integration")]
    public class Optimistic_saga_with_transactions_enabled<T> :
        EntityFrameworkTestFixture<T, SimpleSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task Should_work_with_optimistic_repository_and_transactions_enabled()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId, Is.Not.Null, "Saga should be created successfully with optimistic repository and transactions");

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            Assert.That(foundId, Is.Not.Null, "Saga should be completed successfully with optimistic repository and transactions");
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public Optimistic_saga_with_transactions_enabled()
        {
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() =>
                EntityFrameworkSagaRepository<SimpleSaga>.CreateOptimistic(
                    () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                    null, true));
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var context = new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }


    /// <summary>
    /// Integration tests verifying pessimistic concurrency with transactions enabled
    /// </summary>
    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    [Category("Integration")]
    public class Pessimistic_saga_with_transactions_enabled<T> :
        EntityFrameworkTestFixture<T, SimpleSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task Should_work_with_pessimistic_repository_and_transactions_enabled()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId, Is.Not.Null, "Saga should be created successfully with pessimistic repository and transactions");

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            Assert.That(foundId, Is.Not.Null, "Saga should be completed successfully with pessimistic repository and transactions");
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public Pessimistic_saga_with_transactions_enabled()
        {
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() =>
                EntityFrameworkSagaRepository<SimpleSaga>.CreatePessimistic(
                    () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                    RawSqlLockStatements, null));
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var context = new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}

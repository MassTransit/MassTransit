namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SimpleSaga
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using MassTransit.Saga;
    using MassTransit.Tests.Saga;
    using MassTransit.Tests.Saga.Messages;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Saga;
    using Shared;
    using Shouldly;
    using Testing;


    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class Locating_an_existing_ef_saga<T> :
        EntityFrameworkTestFixture<T, SimpleSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var nextMessage = new CompleteSimpleSaga {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;

        public Locating_an_existing_ef_saga()
        {
            // add new migration by calling
            // dotnet ef migrations add --context "SagaDbContext``2" Init  -v
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() =>
                EntityFrameworkSagaRepository<SimpleSaga>.CreatePessimistic(
                    () => new SimpleSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                    RawSqlLockStatements));
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

namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SagaWithDependency
{
    using System;
    using System.Threading.Tasks;
    using DataAccess;
    using MassTransit.Tests.Saga.Messages;
    using Messages;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Shared;
    using Shouldly;
    using Testing;


    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class Using_custom_include_in_repository<T> :
        EntityFrameworkTestFixture<T, SagaWithDependencyContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task A_correlated_message_should_update_inner_saga_dependency()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var propertyValue = "expected saga property value";
            var updateInnerProperty = new UpdateSagaDependency(sagaId, propertyValue);

            await InputQueueSendEndpoint.Send(updateInnerProperty);

            foundId = await _sagaRepository.Value.ShouldContainSaga(
                x => x.CorrelationId == sagaId && x.Completed && x.Dependency.SagaInnerDependency.Name == propertyValue, TestTimeout);

            foundId.HasValue.ShouldBe(true);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            Console.WriteLine(sagaId);
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message).ConfigureAwait(false);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout).ConfigureAwait(false);

            foundId.HasValue.ShouldBe(true);
        }

        readonly Lazy<ISagaRepository<SagaWithDependency>> _sagaRepository;

        public Using_custom_include_in_repository()
        {
            // // add new migration by calling
            // // dotnet ef migrations add --context "SagaDbContext``2" Init  -v
            _sagaRepository = new Lazy<ISagaRepository<SagaWithDependency>>(() =>
                EntityFrameworkSagaRepository<SagaWithDependency>.CreatePessimistic(
                    () => new SagaWithDependencyContextFactory().CreateDbContext(DbContextOptionsBuilder),
                    RawSqlLockStatements,
                    queryable => queryable.Include(it => it.Dependency).ThenInclude(dependency => dependency.SagaInnerDependency)));
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var context = new SagaWithDependencyContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new SagaWithDependencyContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}

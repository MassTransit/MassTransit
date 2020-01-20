namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using DataAccess;
    using Events;
    using MassTransit.Saga;
    using Microsoft.EntityFrameworkCore;
    using NUnit.Framework;
    using Saga;
    using Shared;
    using Shouldly;
    using Testing;


    [TestFixture(typeof(SqlServerTestDbParameters))]
    [TestFixture(typeof(SqlServerResiliencyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    public class SlowConcurrentSaga_Specs<T> :
        EntityFrameworkTestFixture<T, SlowConcurrentSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task Two_Initiating_Messages_Deadlock_Results_In_One_Instance()
        {
            var sagaId = NewId.NextGuid();
            var message = new Begin {CorrelationId = sagaId};

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var slowMessage = new IncrementCounterSlowly {CorrelationId = sagaId};
            await Task.WhenAll(
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)),
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)));

            _sagaTestHarness.Consumed.Select<IncrementCounterSlowly>().Take(2).ToList();

            // I might be getting superstitions but it looks like sometimes the test harness can report consumed before
            // the transaction is properly committed.
            await Task.Delay(1000);

            await _sagaRepository.Value.ShouldContainSaga(
                s => s.CorrelationId == sagaId && s.Counter == 2 && s.CurrentState == "DidIncrement",
                this.TestTimeout);
        }

        readonly Lazy<ISagaRepository<SlowConcurrentSaga>> _sagaRepository;
        readonly SagaTestHarness<SlowConcurrentSaga> _sagaTestHarness;

        public SlowConcurrentSaga_Specs()
        {
            // rowlock statements that don't work to cause a deadlock.
            var notWorkingRowLockStatements = new SqlLockStatementProvider("dbo", "SELECT * FROM \"{1}\" WHERE \"CorrelationId\" = @p0");

            // add new migration by calling
            // dotnet ef migrations add --context "SagaDbContext``2" Init  -v
            _sagaRepository = new Lazy<ISagaRepository<SlowConcurrentSaga>>(() =>
                EntityFrameworkSagaRepository<SlowConcurrentSaga>.CreatePessimistic(
                    () => new SlowConcurrentSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                    notWorkingRowLockStatements));

            _sagaTestHarness = BusTestHarness.StateMachineSaga(new SlowConcurrentSagaStateMachine(), _sagaRepository.Value);
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var context = new SlowConcurrentSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.MigrateAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new SlowConcurrentSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrencyLimit = 16;
        }
    }
}

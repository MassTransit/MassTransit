namespace MassTransit.EntityFrameworkCoreIntegration.Tests.DeadlockSaga
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
    [TestFixture(typeof(SqlServerResiliancyTestDbParameters))]
    [TestFixture(typeof(PostgresTestDbParameters))]
    [Category("Integration")]
    public class DeadlockSaga_Specs<T> :
        EntityFrameworkTestFixture<T, DeadlockSagaDbContext>
        where T : ITestDbParameters, new()
    {
        [Test]
        public async Task Two_Initiating_Messages_Deadlock_Results_In_One_Instance()
        {
            var sagaId = NewId.NextGuid();
            var message = new Begin { CorrelationId = sagaId };

            await this.InputQueueSendEndpoint.Send(message);

            Guid? foundId = await this._sagaRepository.Value.ShouldContainSaga(message.CorrelationId, this.TestTimeout);

            foundId.HasValue.ShouldBe(true);

            var slowMessage = new IncrementCounterSlowly { CorrelationId = sagaId };
            await Task.WhenAll(
                Task.Run(() => this.InputQueueSendEndpoint.Send(slowMessage)),
                Task.Run(() => this.InputQueueSendEndpoint.Send(slowMessage)));

            this._sagaTestHarness.Consumed.Select<IncrementCounterSlowly>().Take(2).ToList();

            // I might be getting superstitions but it looks like sometimes the test harness can report consumed before
            // the transaction is properly committed.
            await Task.Delay(1000);

            await this._sagaRepository.Value.ShouldContainSaga(
                s => s.CorrelationId == sagaId && s.Counter == 2 && s.CurrentState == "DidIncrement",
                this.TestTimeout);
        }

        readonly Lazy<ISagaRepository<DeadlockSaga>> _sagaRepository;
        readonly SagaTestHarness<DeadlockSaga> _sagaTestHarness;

        public DeadlockSaga_Specs()
        {
            // rowlock statements that don't work so we can cause a deadlock.
            var unworkingRowLockStatements = new RawSqlLockStatements("dbo", "SELECT * FROM \"{1}\" WHERE \"CorrelationId\" = @p0");

            // add new migration by calling
            // dotnet ef migrations add --context "SagaDbContext``2" Init  -v
            this._sagaRepository = new Lazy<ISagaRepository<DeadlockSaga>>(() =>
                EntityFrameworkSagaRepository<DeadlockSaga>.CreatePessimistic(
                    () => new DeadlockSagaContextFactory().CreateDbContext(this.DbContextOptionsBuilder),
                    unworkingRowLockStatements));

            this._sagaTestHarness = this.BusTestHarness.StateMachineSaga(new DeadlockSagaStateMachine(), this._sagaRepository.Value);
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            using (var context = new DeadlockSagaContextFactory().CreateDbContext(this.DbContextOptionsBuilder))
            {
                await context.Database.MigrateAsync();
            }
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            using (var context = new DeadlockSagaContextFactory().CreateDbContext(this.DbContextOptionsBuilder))
            {
                await context.Database.EnsureDeletedAsync();
            }
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrencyLimit = 16;
        }
    }
}

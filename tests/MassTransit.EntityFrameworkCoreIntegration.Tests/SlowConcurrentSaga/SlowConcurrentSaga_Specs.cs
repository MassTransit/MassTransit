namespace MassTransit.EntityFrameworkCoreIntegration.Tests.SlowConcurrentSaga
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DataAccess;
    using Events;
    using NUnit.Framework;
    using Shared;
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
            var message = new Begin { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId, Is.Not.Null);

            var slowMessage = new IncrementCounterSlowly { CorrelationId = sagaId };
            await Task.WhenAll(
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)),
                Task.Run(() => InputQueueSendEndpoint.Send(slowMessage)));

            _sagaTestHarness.Consumed.Select<IncrementCounterSlowly>().Take(2).ToList();

            await InactivityTask;

            await _sagaRepository.Value.ShouldContainSagaInState(s => s.CorrelationId == sagaId && s.Counter == 2, _machine, _machine.DidIncrement,
                TestTimeout);
        }

        readonly Lazy<ISagaRepository<SlowConcurrentSaga>> _sagaRepository;
        readonly ISagaStateMachineTestHarness<SlowConcurrentSagaStateMachine, SlowConcurrentSaga> _sagaTestHarness;
        readonly SlowConcurrentSagaStateMachine _machine;

        public SlowConcurrentSaga_Specs()
        {
            TestInactivityTimeout = TimeSpan.FromSeconds(3);

            // rowlock statements that don't work to cause a deadlock.
            var notWorkingRowLockStatements = new SqlLockStatementProvider("dbo", new NoLockStatementFormatter());

            // add new migration by calling
            // dotnet ef migrations add --context "SagaDbContext``2" Init  -v
            _sagaRepository = new Lazy<ISagaRepository<SlowConcurrentSaga>>(() =>
                EntityFrameworkSagaRepository<SlowConcurrentSaga>.CreatePessimistic(
                    () => new SlowConcurrentSagaContextFactory().CreateDbContext(DbContextOptionsBuilder),
                    notWorkingRowLockStatements));

            _machine = new SlowConcurrentSagaStateMachine();
            _sagaTestHarness = BusTestHarness.StateMachineSaga(_machine, _sagaRepository.Value);
        }

        [OneTimeSetUp]
        public async Task SetUp()
        {
            await using var context = new SlowConcurrentSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task TearDown()
        {
            await using var context = new SlowConcurrentSagaContextFactory().CreateDbContext(DbContextOptionsBuilder);

            await context.Database.EnsureDeletedAsync();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.ConcurrentMessageLimit = 16;
        }


        class NoLockStatementFormatter :
            ILockStatementFormatter
        {
            public void Create(StringBuilder sb, string schema, string table)
            {
                sb.AppendFormat("SELECT * FROM \"{0}\" WHERE ", table);
            }

            public void AppendColumn(StringBuilder sb, int index, string columnName)
            {
                if (index == 0)
                    sb.AppendFormat("\"{0}\" = @p0", columnName);
                else
                    sb.AppendFormat(" AND \"{0}\" = @p{1}", columnName, index);
            }

            public void Complete(StringBuilder sb)
            {
            }

            public void CreateOutboxStatement(StringBuilder sb, string schema, string table, string columnName)
            {
                throw new NotImplementedException();
            }
        }
    }
}

namespace MassTransit.Persistence.Tests.IntegrationTests.StateMachineSagas
{
    using Common;
    using Connectors;
    using Integration.Saga;
    using NUnit.Framework;
    using Testing;


    [Category("Integration")]
    [TestFixture(typeof(OptimisticSqlServerConnector))]
    [TestFixture(typeof(OptimisticPostgresConnector))]
    [TestFixture(typeof(OptimisticMySqlConnector))]
    [TestFixture(typeof(PessimisticSqlServerConnector))]
    [TestFixture(typeof(PessimisticPostgresConnector))]
    [TestFixture(typeof(PessimisticMySqlConnector))]
    public class StateMachineSagaTests<TConnector> : SagaTests<TConnector>
        where TConnector : BehaviorSaga, TestConnector, new()
    {
        [Test]
        public async Task CreateMessage_creates_saga()
        {
            List<TConnector>? sagas = await GetSagas<TConnector>();
            Assert.That(sagas, Is.Empty);

            await InputQueueSendEndpoint.Send<CreateSaga>(new
            {
                CorrelationId = SagaId,
                Name = "my saga"
            });
            await BusTestHarness.Consumed.Any<CreateSaga>();
            await Task.Delay(50);

            Guid? found = await _repository.ShouldContainSaga(SagaId, DefaultTimeout);
            Assert.That(found, Is.EqualTo(SagaId));

            sagas = await GetSagas<TConnector>();
            Assert.That(sagas, Is.Not.Empty);
            Assert.That(sagas[0].Name, Is.EqualTo("my saga"));
        }

        [Test]
        public async Task DeleteMessage_deletes_saga()
        {
            List<TConnector>? sagas = await GetSagas<TConnector>();
            Assert.That(sagas, Is.Empty);

            await InputQueueSendEndpoint.Send<CreateSaga>(new
            {
                CorrelationId = SagaId,
                Name = "my saga"
            });
            await BusTestHarness.Consumed.Any<CreateSaga>();
            await Task.Delay(50);

            Guid? found = await _repository.ShouldContainSaga(SagaId, DefaultTimeout);
            Assert.That(found, Is.EqualTo(SagaId));

            await InputQueueSendEndpoint.Send<DeleteSagaByName>(new { Name = "my saga" });
            await BusTestHarness.Consumed.Any<DeleteSagaByName>();
            await Task.Delay(50);

            sagas = await GetSagas<TConnector>();
            Assert.That(sagas, Is.Empty);
        }

        [Test]
        public async Task UpdateMessage_updates_saga()
        {
            List<TConnector>? sagas = await GetSagas<TConnector>();
            Assert.That(sagas, Is.Empty);

            await InputQueueSendEndpoint.Send<CreateSaga>(new
            {
                CorrelationId = SagaId,
                Name = "my saga 0"
            });
            await BusTestHarness.Consumed.Any<CreateSaga>();
            await Task.Delay(50);

            await InputQueueSendEndpoint.Send<UpdateSaga>(new
            {
                CorrelationId = SagaId,
                Name = "my saga 1"
            });
            await BusTestHarness.Consumed.Any<UpdateSaga>();
            await Task.Delay(50);

            Guid? found = await _repository.ShouldContainSaga(SagaId, DefaultTimeout);
            Assert.That(found, Is.EqualTo(SagaId));

            sagas = await GetSagas<TConnector>();
            Assert.That(sagas, Is.Not.Empty);
            Assert.That(sagas[0].Name, Is.EqualTo("my saga 1"));
        }

        readonly SagaStateMachine<TConnector> _stateMachine;
        ISagaRepository<TConnector> _repository;

        public StateMachineSagaTests()
        {
            _stateMachine = new SagaStateMachine<TConnector>();
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _repository = CustomSagaRepository<TConnector>.Create(conf =>
                Connector.Connect(conf)
            );

            configurator.StateMachineSaga(_stateMachine, _repository);
            base.ConfigureInMemoryReceiveEndpoint(configurator);
        }
    }
}

namespace MassTransit.DapperIntegration.Tests.IntegrationTests.StateMachines
{
    using System.Threading.Tasks;
    using Common;
    using ConsumerSagas;
    using NUnit.Framework;
    using Saga;
    using SqlBuilders;
    using Testing;


    public class BehaviorSagaTests : DapperVersionedSagaTests
    {
        readonly VersionedSagaStateMachine _stateMachine;
        ISagaRepository<VersionedBehaviorSaga> _repository;

        public BehaviorSagaTests() => _stateMachine = new VersionedSagaStateMachine();

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _repository = DapperSagaRepository<VersionedBehaviorSaga>.Create(conf =>
            {
                conf.ConnectionString = ConnectionString;
                conf.TableName = "VersionedSagas";
                conf.ContextFactoryProvider = _ => (c, t) => new SagaDatabaseContext<VersionedBehaviorSaga>(c, t, new SqlServerSagaFormatter<VersionedBehaviorSaga>("VersionedSagas"));
            });

            configurator.StateMachineSaga(_stateMachine, _repository);
            base.ConfigureInMemoryReceiveEndpoint(configurator);
        }

        [Test]
        public async Task CreateMessage_creates_saga()
        {
            await InputQueueSendEndpoint.Send<CreateSaga>(new { CorrelationId = SagaId, Name = "my saga" });
            await BusTestHarness.Consumed.Any<CreateSaga>();

            var found = await _repository.ShouldContainSaga(SagaId, DefaultTimeout);
            Assert.That(found, Is.EqualTo(SagaId));

            var sagas = await GetSagas<VersionedConsumerSaga>();
            Assert.That(sagas, Is.Not.Empty);
            Assert.That(sagas[0].Name, Is.EqualTo("my saga"));
            Assert.That(sagas[0].Version, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateMessage_updates_saga()
        {
            await InputQueueSendEndpoint.Send<CreateSaga>(new { CorrelationId = SagaId, Name = "my saga 0" });
            await BusTestHarness.Consumed.Any<CreateSaga>();

            await InputQueueSendEndpoint.Send<UpdateSaga>(new { CorrelationId = SagaId, Name = "my saga 1" });
            await BusTestHarness.Consumed.Any<UpdateSaga>();

            var found = await _repository.ShouldContainSaga(SagaId, DefaultTimeout);
            Assert.That(found, Is.EqualTo(SagaId));

            var sagas = await GetSagas<VersionedConsumerSaga>();
            Assert.That(sagas, Is.Not.Empty);
            Assert.That(sagas[0].Name, Is.EqualTo("my saga 1"));
            Assert.That(sagas[0].Version, Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteMessage_deletes_saga()
        {
            await InputQueueSendEndpoint.Send<CreateSaga>(new { CorrelationId = SagaId, Name = "my saga" });
            await BusTestHarness.Consumed.Any<CreateSaga>();

            var found = await _repository.ShouldContainSaga(SagaId, DefaultTimeout);
            Assert.That(found, Is.EqualTo(SagaId));

            await InputQueueSendEndpoint.Send<DeleteSagaByName>(new { Name = "my saga" });
            await BusTestHarness.Consumed.Any<DeleteSagaByName>();

            var sagas = await GetSagas<VersionedConsumerSaga>();
            Assert.That(sagas, Is.Empty);
        }
    }
}

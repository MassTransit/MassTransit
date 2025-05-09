namespace MassTransit.DapperIntegration.Tests.IntegrationTests.ConsumerSagas
{
    using System.Threading.Tasks;
    using Common;
    using NUnit.Framework;
    using Saga;
    using SqlBuilders;
    using Testing;


    public class ConsumerSagaTests : DapperVersionedSagaTests
    {
        ISagaRepository<VersionedConsumerSaga> _repository;

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            _repository = DapperSagaRepository<VersionedConsumerSaga>.Create(conf =>
            {
                conf.ConnectionString = ConnectionString;
                conf.TableName = "VersionedSagas";
                conf.ContextFactoryProvider = _ => (c, t) => new SagaDatabaseContext<VersionedConsumerSaga>(c, t, new SqlServerSagaFormatter<VersionedConsumerSaga>("VersionedSagas"));
            });

            configurator.Saga(_repository);
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
            Assert.That(sagas.Count, Is.EqualTo(1));
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
            Assert.That(sagas.Count, Is.EqualTo(1));
            Assert.That(sagas[0].Name, Is.EqualTo("my saga 1"));
            Assert.That(sagas[0].Version, Is.EqualTo(2));
        }
    }
}

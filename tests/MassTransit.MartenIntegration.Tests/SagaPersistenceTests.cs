namespace MassTransit.MartenIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Marten;
    using NUnit.Framework;
    using Saga;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSaga :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(found, Is.EqualTo(sagaId));

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);
            Assert.That(found, Is.EqualTo(sagaId));
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);
            Assert.That(found, Is.EqualTo(sagaId));
        }

        [Test]
        public async Task An_observed_message_should_find_and_update_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId) { Name = "MySimpleSaga" };

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(found, Is.EqualTo(sagaId));

            var nextMessage = new ObservableSagaMessage { Name = "MySimpleSaga" };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Observed, TestTimeout);
            Assert.That(found, Is.EqualTo(sagaId));
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;
        readonly DocumentStore _store;

        [OneTimeSetUp]
        public async Task OneTime()
        {
            await _store.Storage.ApplyAllConfiguredChangesToDatabaseAsync();
        }

        public LocatingAnExistingSaga()
        {
            var connectionString = "server=localhost;port=5432;database=MartenTest;user id=postgres;password=Password12!;";

            _store = DocumentStore.For(x =>
            {
                x.Connection(connectionString);
                x.CreateDatabasesForTenants(c =>
                {
                    c.MaintenanceDatabase("server=localhost;port=5432;database=postgres;user id=postgres;password=Password12!;");
                    c.ForTenant()
                        .CheckAgainstPgDatabase()
                        .WithOwner("postgres")
                        .WithEncoding("UTF-8")
                        .ConnectionLimit(-1);
                });
            });
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => MartenSagaRepository<SimpleSaga>.Create(_store));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}

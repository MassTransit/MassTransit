namespace MassTransit.DapperIntegration.Tests
{
    using System;
    using System.Threading.Tasks;
    using Dapper;
    using MassTransit.Tests;
    using MassTransit.Tests.Saga.Messages;
    using Microsoft.Data.SqlClient;
    using NUnit.Framework;
    using TestFramework;
    using Testing;


    [TestFixture]
    [Category("Integration")]
    public class DapperSagaRepositoryTests :
        InMemoryTestFixture
    {
        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            foundId = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Completed, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);
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

        [OneTimeSetUp]
        public async Task Setup()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = @"
                if not exists (select * from sysobjects where name='SimpleSagas' and xtype='U')
                CREATE TABLE SimpleSagas (
                    CorrelationId uniqueidentifier NOT NULL,
                    CONSTRAINT PK_SimpleSagas_CorrelationId PRIMARY KEY CLUSTERED (CorrelationId),
                    Name nvarchar(max),
                    Completed bit,
                    Initiated bit,
                    Observed bit,
                    CorrelateBySomething nvarchar(max)
                );
            ";
                connection.Execute(sql);
            }
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;
        readonly string _connectionString;

        public DapperSagaRepositoryTests()
        {
            _connectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => DapperSagaRepository<SimpleSaga>.Create(_connectionString));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}

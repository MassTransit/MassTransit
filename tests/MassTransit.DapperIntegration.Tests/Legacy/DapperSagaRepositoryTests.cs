namespace MassTransit.DapperIntegration.Tests.Legacy
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

            var foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

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

            var foundId = await _sagaRepository.Value.ShouldContainSaga(message.CorrelationId, TestTimeout);

            Assert.That(foundId.HasValue, Is.True);
        }
            
        [Test]
        public async Task An_observed_message_should_find_and_update_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId) { Name = "MySimpleSaga" };

            await InputQueueSendEndpoint.Send(message);
            
            var found = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId, TestTimeout);

            Assert.That(found, Is.EqualTo(sagaId));

            var nextMessage = new ObservableSagaMessage { Name = "MySimpleSaga" };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.Value.ShouldContainSaga(x => x.CorrelationId == sagaId && x.Observed, TestTimeout);
            Assert.That(found, Is.EqualTo(sagaId));
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            await using var connection = new SqlConnection(_connectionString);
            var sql = @"IF NOT EXISTS (SELECT * FROM SYSOBJECTS WHERE Name='SimpleSagas' AND xtype='U')
CREATE TABLE SimpleSagas (
    CorrelationId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(MAX),
    Version INT,
    Completed BIT,
    Initiated BIT,
    Observed BIT,
    CorrelateBySomething NVARCHAR(MAX),

    CONSTRAINT PK_SimpleSagas_CorrelationId PRIMARY KEY CLUSTERED (CorrelationId)
);";

            await connection.ExecuteAsync(sql);
        }

        readonly Lazy<ISagaRepository<SimpleSaga>> _sagaRepository;
        readonly string _connectionString;

        public DapperSagaRepositoryTests()
        {
            _connectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();
            _sagaRepository = new Lazy<ISagaRepository<SimpleSaga>>(() => DapperSagaRepository<SimpleSaga>.Create(opt =>
            {
                opt.ConnectionString = _connectionString;
            }));
        }

        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository.Value);
        }
    }
}

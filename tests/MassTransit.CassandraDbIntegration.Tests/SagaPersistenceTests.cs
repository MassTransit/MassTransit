namespace MassTransit.CassandraDb.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Cassandra;
    using Cassandra.Data.Linq;
    using Cassandra.Mapping;
    using CassandraDbIntegration.Saga;
    using MassTransit.TestFramework;
    using MassTransit.Testing;
    using NUnit.Framework;
    using Shouldly;
    using TestFramework.Sagas.ChoirConcurrency;


    [TestFixture]
    [Category("Integration")]
    public class LocatingAnExistingSaga : InMemoryTestFixture
    {
        readonly ISagaRepository<SimpleSaga> _sagaRepository;
        public ISession Session { get; set; }

        public LocatingAnExistingSaga()
        {
            var clusterBuilder = Cluster.Builder()
                .AddContactPoints("localhost".Split(","))
                .WithPort(9042)
                .WithQueryOptions(new QueryOptions().SetConsistencyLevel(ConsistencyLevel.LocalOne))
                .WithCredentials("cassandra", "cassandra");

            var cluster = clusterBuilder.Build();
            Session = cluster.Connect();

            _sagaRepository = CassandraDbSagaRepository<SimpleSaga>.Create(() => Session);
        }


        [Test]
        public async Task A_correlated_message_should_find_the_correct_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();

            var nextMessage = new CompleteSimpleSaga { CorrelationId = sagaId };

            await InputQueueSendEndpoint.Send(nextMessage);

            found = await _sagaRepository.ShouldContainSaga(sagaId, x => x != null && x.Moved, TestTimeout);
            found.ShouldNotBeNull();

            var retrieveRepository = _sagaRepository as ILoadSagaRepository<SimpleSaga>;
            var retrieved = await retrieveRepository.Load(sagaId);
            retrieved.ShouldNotBeNull();
            retrieved.Moved.ShouldBeTrue();
        }

        [Test]
        public async Task An_initiating_message_should_start_the_saga()
        {
            var sagaId = NewId.NextGuid();
            var message = new InitiateSimpleSaga(sagaId);

            await InputQueueSendEndpoint.Send(message);

            Guid? found = await _sagaRepository.ShouldContainSaga(message.CorrelationId, TestTimeout);

            found.ShouldNotBeNull();
        }

        [SetUp]
        public async Task CreateTableIfNotExists()
        {
            Session.CreateKeyspaceIfNotExists("test");
            Session.ChangeKeyspace("test");
            var mapping = MappingConfiguration.Global.Get<CassandraDbSaga>();
            if (mapping == null)
            {
                mapping = new Map<CassandraDbSaga>()
                    .PartitionKey(u => u.CorrelationId)
                    .TableName(nameof(ChoirState));
                MappingConfiguration.Global.Define(mapping);
            }
            var config = new MappingConfiguration().Define(mapping);

            try
            {
                await ClearTable();
            }
            catch (Exception e)
            {
                var table = new Table<CassandraDbSaga>(Session, config, nameof(ChoirState));
                await table.CreateAsync();
            }
        }

        [TearDown]
        public async Task ClearTable()
        {
            await Session.ExecuteAsync(new SimpleStatement($"TRUNCATE {nameof(ChoirState)}"));
        }



        protected override void ConfigureInMemoryReceiveEndpoint(IInMemoryReceiveEndpointConfigurator configurator)
        {
            configurator.Saga(_sagaRepository);
        }
    }
}

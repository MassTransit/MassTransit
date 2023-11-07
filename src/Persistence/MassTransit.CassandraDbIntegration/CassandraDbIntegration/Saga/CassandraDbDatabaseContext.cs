namespace MassTransit.CassandraDbIntegration.Saga
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Cassandra;
    using Cassandra.Data.Linq;
    using Cassandra.Mapping;
    using Exceptions;
    using Serialization;


    public class CassandraDbDatabaseContext<TSaga> :
        DatabaseContext<TSaga>
        where TSaga : class, ISagaVersion
    {
        readonly ISession _database;
        readonly CassandraDbSagaRepositoryOptions<TSaga> _options;

        public CassandraDbDatabaseContext(ISession database, CassandraDbSagaRepositoryOptions<TSaga> options)
        {
            _database = database;
            _options = options;
        }

        public Task Add(SagaConsumeContext<TSaga> context)
        {
            return Create(context.Saga);
        }

        public Task Insert(TSaga instance)
        {
            return Create(instance);
        }

        public async Task<TSaga> Load(Guid correlationId)
        {
            try
            {
                var value = await _database
                    .GetTable<CassandraDbSaga>()
                    .FirstOrDefault(x => x.CorrelationId == _options.FormatSagaKey(correlationId))
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                return value == null
                    ? null
                    : JsonSerializer.Deserialize<TSaga>(value.Properties, SystemTextJsonMessageSerializer.Options);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task Update(SagaConsumeContext<TSaga> context)
        {
            var instance = context.Saga;
            ++instance.Version;

            var updateQuery = _database.GetTable<CassandraDbSaga>()
                .Where(x => x.CorrelationId == _options.FormatSagaKey(instance.CorrelationId))
                .Select(x => new CassandraDbSaga
                {
                    Properties = JsonSerializer.Serialize(instance, SystemTextJsonMessageSerializer.Options),
                    VersionNumber = instance.Version
                })
                .UpdateIf(x => x.VersionNumber < instance.Version);

            AppliedInfo<CassandraDbSaga> result;
            try
            {
                result = await updateQuery.ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new CassandraDbSagaConcurrencyException("Saga version conflict", typeof(TSaga), instance.CorrelationId, exception);
            }

            if (!result.Applied) throw new CassandraDbSagaConcurrencyException("Saga version conflict", typeof(TSaga), instance.CorrelationId);
        }

        public async Task Delete(SagaConsumeContext<TSaga> context)
        {
            await _database.GetTable<CassandraDbSaga>()
                .Where(x => x.CorrelationId == _options.FormatSagaKey(context.CorrelationId.Value))
                .Delete()
                .ExecuteAsync()
                .ConfigureAwait(false);
        }

        async Task Create(TSaga instance)
        {
            var addSaga = GetCassandraDbSaga(instance);
            try
            {
                await _database.GetTable<CassandraDbSaga>().Insert(addSaga).ExecuteAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw new CassandraDbSagaConcurrencyException("Saga insert failed", typeof(TSaga), instance.CorrelationId, exception);
            }
        }

        CassandraDbSaga GetCassandraDbSaga(TSaga instance)
        {
            return new CassandraDbSaga
            {
                CorrelationId = _options.FormatSagaKey(instance.CorrelationId),
                VersionNumber = instance.Version,
                Properties = JsonSerializer.Serialize(instance, SystemTextJsonMessageSerializer.Options),
            };
        }

        public void Dispose()
        {
            _database?.Dispose();
        }

    }
}

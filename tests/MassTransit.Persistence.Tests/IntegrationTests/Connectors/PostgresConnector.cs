namespace MassTransit.Persistence.Tests.IntegrationTests.Connectors
{
    using System.Data;
    using Configuration;
    using Integration.SqlBuilders;
    using MessageData;
    using Npgsql;
    using PostgreSql.Configuration;
    using StateMachineSagas;


    public class OptimisticPostgresConnector : PostgresConnector,
        TestConnector
    {
        public uint XMin { get; set; }

        public void Connect<TSaga>(ICustomRepositoryConfigurator<TSaga> conf)
            where TSaga : class, ISaga
        {
            conf.UsingPostgres(ConnectionString, opt => opt.SetTableName("OptimisticSagas").SetOptimisticConcurrency());
        }

        public IMessageDataRepository CreateMessageDataRepository(TimeProvider timeProvider)
        {
            throw new NotSupportedException("MessageData only supports pessimistic concurrency");
        }

        public Task<List<TSaga>> GetSagas<TSaga>()
            where TSaga : class, ISaga
        {
            return base.GetSagas<TSaga>("OptimisticSagas");
        }
    }


    public class PessimisticPostgresConnector : PostgresConnector,
        TestConnector
    {
        public void Connect<TSaga>(ICustomRepositoryConfigurator<TSaga> conf)
            where TSaga : class, ISaga
        {
            conf.UsingPostgres(ConnectionString, opt => opt.SetTableName("PessimisticSagas").SetPessimisticConcurrency());
        }

        public IMessageDataRepository CreateMessageDataRepository(TimeProvider timeProvider)
        {
            return new PostgresMessageDataRepository(ConnectionString, "MessageData", "Id", IsolationLevel.RepeatableRead, timeProvider.GetUtcNow);
        }

        public Task<List<TSaga>> GetSagas<TSaga>()
            where TSaga : class, ISaga
        {
            return base.GetSagas<TSaga>("PessimisticSagas");
        }
    }


    public abstract class PostgresConnector : BehaviorSaga
    {
        protected readonly string ConnectionString;

        public PostgresConnector()
        {
            ConnectionString = "Host=localhost; Username=sa; Password=Password12!; Database=masstransit";
        }

        public async Task Setup()
        {
            await RunSql(Sql.Postgres_DropJobTables);
            await RunSql(Sql.Postgres_DropSagaTables);
            await RunSql(Sql.Postgres_DropMessageDataTables);

            await RunSql(Sql.Postgres_CreateJobTables);
            await RunSql(Sql.Postgres_CreateSagaTables);
            await RunSql(Sql.Postgres_CreateMessageDataTables);
        }

        public async Task Teardown()
        {
            await RunSql(Sql.Postgres_DropJobTables);
            await RunSql(Sql.Postgres_DropSagaTables);
            await RunSql(Sql.Postgres_DropMessageDataTables);
        }

        public void Connect(ICustomJobSagaRepositoryConfigurator conf)
        {
            conf.UsingPostgres(ConnectionString);
        }

        protected async Task<List<TSaga>> GetSagas<TSaga>(string tableName)
            where TSaga : class
        {
            Func<IDataReader, TSaga> adapter = ReflectionsAdapter.CreateFor<TSaga>();

            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = $"SELECT * FROM {tableName};";
            await using var reader = await command.ExecuteReaderAsync();

            var result = new List<TSaga>();
            while (await reader.ReadAsync())
                result.Add(adapter(reader));

            return result;
        }

        async Task RunSql(string sql)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            await command.ExecuteNonQueryAsync();
        }
    }
}

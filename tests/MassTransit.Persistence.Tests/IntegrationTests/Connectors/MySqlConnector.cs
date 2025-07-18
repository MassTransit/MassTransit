namespace MassTransit.Persistence.Tests.IntegrationTests.Connectors
{
    using System.Data;
    using Configuration;
    using global::MySqlConnector;
    using Integration.SqlBuilders;
    using MessageData;
    using MySql.Configuration;
    using StateMachineSagas;
    
    public class OptimisticMySqlConnector : MySqlConnector,
        TestConnector
    {
        public DateTime RowVersion { get; set; }

        public void Connect<TSaga>(ICustomRepositoryConfigurator<TSaga> conf)
            where TSaga : class, ISaga
        {
            conf.UsingMySql(ConnectionString, opt => opt.SetTableName("OptimisticSagas").SetOptimisticConcurrency());
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


    public class PessimisticMySqlConnector : MySqlConnector,
        TestConnector
    {
        public void Connect<TSaga>(ICustomRepositoryConfigurator<TSaga> conf)
            where TSaga : class, ISaga
        {
            conf.UsingMySql(ConnectionString, opt => opt.SetTableName("PessimisticSagas").SetPessimisticConcurrency());
        }

        public IMessageDataRepository CreateMessageDataRepository(TimeProvider timeProvider)
        {
            return new MySqlMessageDataRepository(ConnectionString, "MessageData", "Id", IsolationLevel.RepeatableRead, timeProvider.GetUtcNow);
        }

        public Task<List<TSaga>> GetSagas<TSaga>()
            where TSaga : class, ISaga
        {
            return base.GetSagas<TSaga>("PessimisticSagas");
        }
    }


    public abstract class MySqlConnector : BehaviorSaga
    {
        protected readonly string ConnectionString;

        public MySqlConnector()
        {
            ConnectionString = "Server=localhost; Database=masstransit; Uid=sa; Pwd=Password12!";
        }

        public async Task Setup()
        {
            await RunSql(Sql.MySql_DropJobTables);
            await RunSql(Sql.MySql_DropSagaTables);
            await RunSql(Sql.MySql_DropMessageDataTables);

            await RunSql(Sql.MySql_CreateJobTables);
            await RunSql(Sql.MySql_CreateSagaTables);
            await RunSql(Sql.MySql_CreateMessageDataTables);
        }

        public async Task Teardown()
        {
            await RunSql(Sql.MySql_DropJobTables);
            await RunSql(Sql.MySql_DropSagaTables);
            await RunSql(Sql.MySql_DropMessageDataTables);
        }

        public void Connect(ICustomJobSagaRepositoryConfigurator conf)
        {
            conf.UsingMySql(ConnectionString);
        }

        protected async Task<List<TSaga>> GetSagas<TSaga>(string tableName)
            where TSaga : class
        {
            Func<IDataReader, TSaga> adapter = ReflectionsAdapter.CreateFor<TSaga>();

            await using var connection = new MySqlConnection(ConnectionString);
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
            await using var connection = new MySqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            await command.ExecuteNonQueryAsync();
        }
    }
}

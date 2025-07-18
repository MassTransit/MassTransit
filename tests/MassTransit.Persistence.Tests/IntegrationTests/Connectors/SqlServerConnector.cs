namespace MassTransit.Persistence.Tests.IntegrationTests.Connectors
{
    using System.Data;
    using Configuration;
    using Integration.SqlBuilders;
    using MassTransit.Tests;
    using MessageData;
    using Microsoft.Data.SqlClient;
    using SqlServer.Configuration;
    using StateMachineSagas;


    public class OptimisticSqlServerConnector : SqlServerConnector,
        TestConnector
    {
        public byte[] RowVersion { get; set; }

        void TestConnector.Connect<TSaga>(ICustomRepositoryConfigurator<TSaga> conf)
        {
            conf.UsingSqlServer(ConnectionString, opt => opt.SetTableName("OptimisticSagas").SetOptimisticConcurrency());
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


    public class PessimisticSqlServerConnector : SqlServerConnector,
        TestConnector
    {
        void TestConnector.Connect<TSaga>(ICustomRepositoryConfigurator<TSaga> conf)
        {
            conf.UsingSqlServer(ConnectionString, opt => opt.SetTableName("PessimisticSagas").SetPessimisticConcurrency());
        }

        public IMessageDataRepository CreateMessageDataRepository(TimeProvider timeProvider)
        {
            return new SqlServerMessageDataRepository(ConnectionString, "MessageData", "Id", IsolationLevel.RepeatableRead, timeProvider.GetUtcNow);
        }

        public Task<List<TSaga>> GetSagas<TSaga>()
            where TSaga : class, ISaga
        {
            return base.GetSagas<TSaga>("PessimisticSagas");
        }
    }


    public abstract class SqlServerConnector : BehaviorSaga
    {
        protected readonly string ConnectionString;

        public SqlServerConnector()
        {
            ConnectionString = LocalDbConnectionStringProvider.GetLocalDbConnectionString();
        }

        public async Task Setup()
        {
            await RunSql(Sql.SqlServer_DropJobTables);
            await RunSql(Sql.SqlServer_DropSagaTables);
            await RunSql(Sql.SqlServer_DropMessageDataTables);

            await RunSql(Sql.SqlServer_CreateJobTables);
            await RunSql(Sql.SqlServer_CreateSagaTables);
            await RunSql(Sql.SqlServer_CreateMessageDataTables);
        }

        public async Task Teardown()
        {
            await RunSql(Sql.SqlServer_DropJobTables);
            await RunSql(Sql.SqlServer_DropSagaTables);
            await RunSql(Sql.SqlServer_DropMessageDataTables);
        }

        public void Connect(ICustomJobSagaRepositoryConfigurator conf)
        {
            conf.UsingSqlServer(ConnectionString);
        }

        protected async Task<List<TSaga>> GetSagas<TSaga>(string tableName)
            where TSaga : class
        {
            Func<IDataReader, TSaga> adapter = ReflectionsAdapter.CreateFor<TSaga>();

            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();

            command.CommandText = $"SELECT * FROM {tableName};";
            await using var reader = await command.ExecuteReaderAsync();

            var result = new List<TSaga>();
            while (await reader.ReadAsync())
                result.Add(adapter(reader));

            return result;
        }

        protected async Task RunSql(string sql)
        {
            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = sql;

            await command.ExecuteNonQueryAsync();
        }
    }
}

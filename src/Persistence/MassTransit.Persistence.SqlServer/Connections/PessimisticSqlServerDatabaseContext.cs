namespace MassTransit.Persistence.SqlServer.Connections
{
    using System.Data;
    using System.Linq.Expressions;
    using Integration.Saga;
    using Integration.SqlBuilders;
    using Microsoft.Data.SqlClient;


    public class PessimisticSqlServerDatabaseContext<TSaga> : SqlServerDatabaseContext<TSaga>,
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly IsolationLevel _isolationLevel;

        public PessimisticSqlServerDatabaseContext(string connectionString, string tableName, string idColumnName, IsolationLevel isolationLevel)
            : base(connectionString, tableName, idColumnName)
        {
            _isolationLevel = isolationLevel;
        }

        protected override string BuildLoadSql()
        {
            return $"SELECT TOP 1 * FROM {TableName} WITH (UPDLOCK, ROWLOCK) WHERE [{IdColumnName}] = @correlationid";
        }

        protected override string BuildQuerySql(Expression<Func<TSaga, bool>> filterExpression, Action<string, object?> parameterCallback)
        {
            var sqlRoot = $"SELECT * FROM {TableName} WITH (UPDLOCK, ROWLOCK)";

            List<SqlPredicate> predicates = SqlExpressionVisitor.CreateFromExpression(filterExpression, Mappings);

            if (predicates.Count == 0) // good luck...
                return sqlRoot;

            var queryPredicate = BuildQueryPredicate(predicates, parameterCallback);
            return string.Concat(sqlRoot, " WHERE ", queryPredicate);
        }

        protected override string BuildInsertSql()
        {
            var properties = PersistenceHelper.BuildProperties(ModelType, Mappings);

            var columns = string.Join(", ", properties.Select(p => $"[{p.ColumnName}]"));
            var values = string.Join(", ", properties.Select(p => $"@{p.PropertyName}"));

            var sql = $"INSERT INTO {TableName} ({columns}) VALUES ({values})";

            return sql;
        }

        protected override string BuildUpdateSql()
        {
            var properties = PersistenceHelper.BuildProperties(ModelType, Mappings);

            properties.Remove(nameof(ISaga.CorrelationId));

            var updateExpression = string.Join(", ", properties.Select(p => $"[{p.ColumnName}] = @{p.PropertyName}"));

            var sql = $"UPDATE {TableName} SET {updateExpression} WHERE [{IdColumnName}] = @correlationid";

            return sql;
        }

        protected override string BuildDeleteSql()
        {
            var sql = $"DELETE FROM {TableName} WHERE [{IdColumnName}] = @correlationid";

            return sql;
        }

        protected override async ValueTask OnConnectionOpened(SqlConnection connection, CancellationToken cancellationToken)
        {
            Transaction = (SqlTransaction)await connection.BeginTransactionAsync(_isolationLevel, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}

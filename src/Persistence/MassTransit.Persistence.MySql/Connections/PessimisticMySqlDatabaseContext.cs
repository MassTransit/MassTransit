namespace MassTransit.Persistence.MySql.Connections
{
    using System.Data;
    using System.Linq.Expressions;
    using Integration.Saga;
    using Integration.SqlBuilders;
    using MySqlConnector;


    public class PessimisticMySqlDatabaseContext<TSaga> : MySqlDatabaseContext<TSaga>,
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly IsolationLevel _isolationLevel;

        public PessimisticMySqlDatabaseContext(string connectionString, string tableName, string idColumnName, IsolationLevel isolationLevel)
            : base(connectionString, tableName, idColumnName)
        {
            _isolationLevel = isolationLevel;
        }

        protected override string BuildLoadSql()
        {
            return $"SELECT * FROM {TableName} WHERE {IdColumnName} = @correlationid LIMIT 1 FOR UPDATE";
        }

        protected override string BuildQuerySql(Expression<Func<TSaga, bool>> filterExpression, Action<string, object?> parameterCallback)
        {
            var sqlRoot = $"SELECT * FROM {TableName}";
            var sqlLock = " FOR UPDATE";

            var predicates = SqlExpressionVisitor.CreateFromExpression(filterExpression, Mappings);

            if (predicates.Count == 0) // good luck...
                return string.Concat(sqlRoot, sqlLock);

            var queryPredicate = BuildQueryPredicate(predicates, parameterCallback);
            return string.Concat(sqlRoot, " WHERE ", queryPredicate, sqlLock);
        }

        protected override string BuildInsertSql()
        {
            var properties = PersistenceHelper.BuildProperties(ModelType, Mappings);

            var columns = string.Join(", ", properties.Select(p => $"{p.ColumnName}"));
            var values = string.Join(", ", properties.Select(p => $"@{p.PropertyName}"));

            var sql = $"INSERT INTO {TableName} ({columns}) VALUES ({values})";

            return sql;
        }

        protected override string BuildUpdateSql()
        {
            var properties = PersistenceHelper.BuildProperties(ModelType, Mappings);

            properties.Remove(nameof(ISaga.CorrelationId));

            var updateExpression = string.Join(", ", properties.Select(p => $"{p.ColumnName} = @{p.PropertyName}"));

            var sql = $"UPDATE {TableName} SET {updateExpression} WHERE {IdColumnName} = @correlationid";

            return sql;
        }

        protected override string BuildDeleteSql()
        {
            var sql = $"DELETE FROM {TableName} WHERE {IdColumnName} = @correlationid";

            return sql;
        }

        protected override async ValueTask OnConnectionOpened(MySqlConnection connection, CancellationToken cancellationToken)
        {
            Transaction = await connection.BeginTransactionAsync(_isolationLevel, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}

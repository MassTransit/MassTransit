namespace MassTransit.Persistence.PostgreSql.Connections
{
    using System.Linq.Expressions;
    using System.Reflection;
    using Integration.Saga;
    using Integration.SqlBuilders;
    using Npgsql;
    using NpgsqlTypes;


    public class OptimisticPostgresDatabaseContext<TSaga> : PostgresDatabaseContext<TSaga>,
        DatabaseContext<TSaga>
        where TSaga : class, ISaga
    {
        readonly string _versionColumnName;
        readonly string _versionPropertyName;
        readonly PropertyInfo _versionProperty;

        public OptimisticPostgresDatabaseContext(string connectionString, string tableName, string idColumnName, string versionPropertyName)
            : base(connectionString, tableName, idColumnName)
        {
            _versionColumnName = @"xmin";
            _versionProperty = ModelType.GetProperty(versionPropertyName)
                ?? throw new InvalidOperationException($"Cannot access version property {versionPropertyName} on {ModelType.Name}");

            _versionPropertyName = _versionProperty.Name.ToLowerInvariant();
        }

        protected override string BuildLoadSql()
        {
            return $"SELECT *, xmin AS {_versionProperty.Name} FROM {TableName} WHERE {IdColumnName} = @correlationid LIMIT 1";
        }

        protected override string BuildQuerySql(Expression<Func<TSaga, bool>> filterExpression, Action<string, object?> parameterCallback)
        {
            var sqlRoot = $"SELECT *, xmin AS {_versionProperty.Name} FROM {TableName}";

            var predicates = SqlExpressionVisitor.CreateFromExpression(filterExpression, Mappings);

            if (predicates.Count == 0) // good luck...
                return sqlRoot;

            var queryPredicate = BuildQueryPredicate(predicates, parameterCallback);
            return string.Concat(sqlRoot, " WHERE ", queryPredicate);
        }

        protected override string BuildInsertSql()
        {
            var properties = PersistenceHelper.BuildProperties(ModelType, Mappings);

            properties.Remove(_versionProperty.Name);

            var columns = string.Join(", ", properties.Select(p => $"{p.ColumnName}"));
            var values = string.Join(", ", properties.Select(p => $"@{p.PropertyName}"));

            var sql = $"INSERT INTO {TableName} ({columns}) VALUES ({values})";

            return sql;
        }

        protected override string BuildUpdateSql()
        {
            var properties = PersistenceHelper.BuildProperties(ModelType, Mappings);

            properties.Remove(nameof(ISaga.CorrelationId));
            properties.Remove(_versionProperty.Name);

            var updateExpression = string.Join(", ", properties.Select(p => $"{p.ColumnName} = @{p.PropertyName}"));

            var sql = $"UPDATE {TableName} SET {updateExpression} WHERE {IdColumnName} = @correlationid AND {_versionColumnName} = @{_versionPropertyName}";

            return sql;
        }

        protected override string BuildDeleteSql()
        {
            var sql = $"DELETE FROM {TableName} WHERE {IdColumnName} = @correlationid AND {_versionColumnName} = @{_versionPropertyName}";

            return sql;
        }

        protected override ValueTask OnParametersWritten(NpgsqlCommand command, CancellationToken cancellationToken)
        {
            var param = command.Parameters.FirstOrDefault(p => string.Equals(
                p.ParameterName,
                _versionPropertyName,
                StringComparison.OrdinalIgnoreCase
            ));

            if (param is not null)
                param.NpgsqlDbType = NpgsqlDbType.Xid;

            return base.OnParametersWritten(command, cancellationToken);
        }
    }
}

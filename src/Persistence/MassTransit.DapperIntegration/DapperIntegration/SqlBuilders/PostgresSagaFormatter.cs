namespace MassTransit.DapperIntegration.SqlBuilders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Saga;


    public class PostgresSagaFormatter<TModel> : SagaFormatterBase, ISagaSqlFormatter<TModel>
        where TModel : class
    {
        readonly string _tableName;
        readonly string _idColumnName;
        readonly string _versionColumnName;

        public PostgresSagaFormatter(string tableName = default, string idColumnName = default)
        {
            _tableName = tableName ?? GetTableName(typeof(TModel));
            _idColumnName = idColumnName ?? GetIdColumnName(typeof(TModel));
            _versionColumnName = GetColumnName(typeof(TModel), nameof(ISagaVersion.Version));
        }

        public string BuildLoadSql()
        {
            return $"SELECT * FROM {_tableName} WHERE {_idColumnName} = $1 FOR UPDATE";
        }

        public string BuildQuerySql(Expression<Func<TModel, bool>> filterExpression, Action<string, object> parameterCallback)
        {
            var sqlRoot = $"SELECT * FROM {_tableName}";
            var sqlLock = " FOR UPDATE";

            var predicates = SqlExpressionVisitor.CreateFromExpression(filterExpression, Mappings);

            if (predicates.Count == 0) // good luck...
                return string.Concat(sqlRoot, sqlLock);

            var queryPredicate = BuildQueryPredicate(predicates, parameterCallback);
            return string.Concat(sqlRoot, " WHERE ", queryPredicate, sqlLock);
        }

        public static string BuildQueryPredicate(List<SqlPredicate> predicates, Action<string, object> parameterCallback)
        {
            var queryPredicates = new List<string>();

            foreach (var p in predicates)
            {
                var paramName = $"${queryPredicates.Count + 1}";
                queryPredicates.Add($"{p.Name} {p.Operator} {paramName}");
                parameterCallback?.Invoke(paramName, p.Value);
            }

            return string.Join(" AND ", queryPredicates);
        }

        public string BuildInsertSql()
        {
            var sagaType = typeof(TModel);

            var forbidden = new HashSet<string> { _idColumnName, _versionColumnName };
            var properties = BuildProperties(sagaType, forbidden).ToList();
            properties.Insert(0, (col: GetIdColumnName(sagaType), prop: "$1"));

            if (_versionColumnName != null)
                properties.Insert(1, (col: _versionColumnName, prop: "$2"));

            var columns = string.Join(", ", properties.Select(p => $"{p.col}"));
            var values = string.Join(", ", properties.Select((p, i) => $"${i + 1}"));

            var sql = $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";

            return sql;
        }

        public string BuildUpdateSql()
        {
            var sagaType = typeof(TModel);

            var forbidden = new HashSet<string> { _idColumnName, _versionColumnName };
            var properties = BuildProperties(sagaType, forbidden).ToList();

            if (_versionColumnName != null)
                properties.Insert(0, (col: _versionColumnName, prop: string.Empty));

            // "i + 2" is to leave room for correlationId + version
            var updateExpression = string.Join(", ", properties.Select((p, i) => $"{p.col} = ${i + 2}"));

            var sql = $"UPDATE {_tableName} SET {updateExpression} WHERE {_idColumnName} = $1";

            if (_versionColumnName != null)
                sql += $" AND {_versionColumnName} < $2";

            return sql;
        }

        public string BuildDeleteSql()
        {
            var sql = $"DELETE FROM {_tableName} WHERE {_idColumnName} = $1";

            if (_versionColumnName != null)
                sql += $" AND {_versionColumnName} < $2";

            return sql;
        }

        public void MapPrefix<TProperty>(Expression<Func<TModel, TProperty>> mappingExpression, string prefixName = null)
            => MapCore(mappingExpression, prefixName, false);

        public void MapProperty<TProperty>(Expression<Func<TModel, TProperty>> mappingExpression, string targetName)
            => MapCore(mappingExpression, targetName, true);
    }
}

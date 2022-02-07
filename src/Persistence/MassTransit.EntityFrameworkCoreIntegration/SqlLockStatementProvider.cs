namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Extensions;
    using MassTransit.Saga;
    using Metadata;
    using Microsoft.EntityFrameworkCore;


    public class SqlLockStatementProvider :
        ILockStatementProvider
    {
        protected static readonly ConcurrentDictionary<Type, SchemaTableColumnTrio> TableNames = new ConcurrentDictionary<Type, SchemaTableColumnTrio>();
        readonly bool _enableSchemaCaching;

        public SqlLockStatementProvider(string defaultSchema, string rowLockStatement, bool enableSchemaCaching = true)
        {
            _enableSchemaCaching = enableSchemaCaching;
            DefaultSchema = defaultSchema ?? throw new ArgumentNullException(nameof(defaultSchema));
            RowLockStatement = rowLockStatement ?? throw new ArgumentNullException(nameof(rowLockStatement));
        }

        string DefaultSchema { get; }
        string RowLockStatement { get; }

        public virtual string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga
        {
            return FormatLockStatement<TSaga>(context);
        }

        string FormatLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga
        {
            var schemaTableTrio = GetSchemaAndTableNameAndColumnName(context, typeof(TSaga));

            return string.Format(RowLockStatement, schemaTableTrio.Schema, schemaTableTrio.Table, schemaTableTrio.ColumnName);
        }

        SchemaTableColumnTrio GetSchemaAndTableNameAndColumnName(DbContext context, Type type)
        {
            if (TableNames.TryGetValue(type, out var result) && _enableSchemaCaching)
                return result;

            var entityType = context.Model.SafeFindEntityType(type);

            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var property = entityType.GetProperties().Single(x => x.Name.Equals(nameof(ISaga.CorrelationId), StringComparison.OrdinalIgnoreCase));
            var columnName = property.GetColumnName();

            if (string.IsNullOrWhiteSpace(tableName))
                throw new MassTransitException($"Unable to determine saga table name: {TypeMetadataCache.GetShortName(type)} (using model metadata).");

            result = new SchemaTableColumnTrio(schema ?? DefaultSchema, tableName, columnName);

            if (_enableSchemaCaching)
                TableNames.TryAdd(type, result);

            return result;
        }


        protected readonly struct SchemaTableColumnTrio
        {
            public SchemaTableColumnTrio(string schema, string table, string columnName)
            {
                Schema = schema;
                Table = table;
                ColumnName = columnName;
            }

            public readonly string Schema;
            public readonly string Table;
            public readonly string ColumnName;
        }
    }
}

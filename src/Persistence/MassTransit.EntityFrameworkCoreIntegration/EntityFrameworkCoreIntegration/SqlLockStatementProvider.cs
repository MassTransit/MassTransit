namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;


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

        public virtual string GetRowLockStatement<T>(DbContext context)
            where T : class
        {
            return FormatLockStatement<T>(context);
        }

        string FormatLockStatement<T>(DbContext context)
            where T : class
        {
            var schemaTableTrio = GetSchemaAndTableNameAndColumnName(context, typeof(T));

            return string.Format(RowLockStatement, schemaTableTrio.Schema, schemaTableTrio.Table, schemaTableTrio.ColumnName);
        }

        SchemaTableColumnTrio GetSchemaAndTableNameAndColumnName(DbContext context, Type type)
        {
            if (TableNames.TryGetValue(type, out var result) && _enableSchemaCaching)
                return result;

            var entityType = context.Model.FindEntityType(type)
                ?? throw new InvalidOperationException($"Entity type not found: {TypeCache.GetShortName(type)}");

            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var property = entityType.GetProperties().Single(x => x.Name.Equals(nameof(ISaga.CorrelationId), StringComparison.OrdinalIgnoreCase));

        #if NETSTANDARD2_0
            var columnName = property.GetColumnName();
        #else
            var storeObjectIdentifier = StoreObjectIdentifier.Table(tableName, schema);
            var columnName = property.GetColumnName(storeObjectIdentifier);
        #endif

            if (string.IsNullOrWhiteSpace(tableName))
                throw new MassTransitException($"Unable to determine saga table name: {TypeCache.GetShortName(type)} (using model metadata).");

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

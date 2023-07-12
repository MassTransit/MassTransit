namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;


    public class SqlLockStatementProvider :
        ILockStatementProvider
    {
        protected static readonly ConcurrentDictionary<Type, SchemaTableColumnTrio> TableNames = new ConcurrentDictionary<Type, SchemaTableColumnTrio>();
        readonly bool _enableSchemaCaching;
        readonly ILockStatementFormatter _formatter;

        public SqlLockStatementProvider(string defaultSchema, ILockStatementFormatter formatter, bool enableSchemaCaching = true)
        {
            DefaultSchema = defaultSchema;

            _formatter = formatter;
            _enableSchemaCaching = enableSchemaCaching;
        }

        public SqlLockStatementProvider(ILockStatementFormatter formatter, bool enableSchemaCaching = true)
        {
            _formatter = formatter;
            _enableSchemaCaching = enableSchemaCaching;
        }

        string DefaultSchema { get; }

        public virtual string GetRowLockStatement<T>(DbContext context)
            where T : class
        {
            return FormatLockStatement<T>(context, nameof(ISaga.CorrelationId));
        }

        public virtual string GetRowLockStatement<T>(DbContext context, params string[] propertyNames)
            where T : class
        {
            return FormatLockStatement<T>(context, propertyNames);
        }

        public virtual string GetOutboxStatement(DbContext context)
        {
            var schemaTableTrio = GetSchemaAndTableNameAndColumnName(context, typeof(OutboxState), nameof(OutboxState.Created));

            var sb = new StringBuilder(128);
            _formatter.CreateOutboxStatement(sb, schemaTableTrio.Schema, schemaTableTrio.Table, schemaTableTrio.ColumnNames[0]);

            return sb.ToString();
        }

        string FormatLockStatement<T>(DbContext context, params string[] propertyNames)
            where T : class
        {
            var schemaTableTrio = GetSchemaAndTableNameAndColumnName(context, typeof(T), propertyNames);

            var sb = new StringBuilder(128);
            _formatter.Create(sb, schemaTableTrio.Schema, schemaTableTrio.Table);

            for (var i = 0; i < propertyNames.Length; i++)
                _formatter.AppendColumn(sb, i, schemaTableTrio.ColumnNames[i]);

            _formatter.Complete(sb);

            return sb.ToString();
        }

        SchemaTableColumnTrio GetSchemaAndTableNameAndColumnName(DbContext context, Type type, params string[] propertyNames)
        {
            if (TableNames.TryGetValue(type, out var result) && _enableSchemaCaching)
                return result;

            var entityType = context.Model.FindEntityType(type)
                ?? throw new InvalidOperationException($"Entity type not found: {TypeCache.GetShortName(type)}");

            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            var columnNames = new List<string>();

            for (var i = 0; i < propertyNames.Length; i++)
            {
                var property = entityType.GetProperties().Single(x => x.Name.Equals(propertyNames[i], StringComparison.OrdinalIgnoreCase));

            #if NETSTANDARD2_0
                var columnName = property.GetColumnName();
            #else
                var storeObjectIdentifier = StoreObjectIdentifier.Table(tableName, schema);
                var columnName = property.GetColumnName(storeObjectIdentifier);
            #endif

                columnNames.Add(columnName);
            }

            if (string.IsNullOrWhiteSpace(tableName))
                throw new MassTransitException($"Unable to determine saga table name: {TypeCache.GetShortName(type)} (using model metadata).");

            result = new SchemaTableColumnTrio(schema ?? DefaultSchema, tableName, columnNames.ToArray());

            if (_enableSchemaCaching)
                TableNames.TryAdd(type, result);

            return result;
        }


        protected readonly struct SchemaTableColumnTrio
        {
            public SchemaTableColumnTrio(string schema, string table, string[] columnNames)
            {
                Schema = schema;
                Table = table;
                ColumnNames = columnNames;
            }

            public readonly string Schema;
            public readonly string Table;
            public readonly string[] ColumnNames;
        }
    }
}

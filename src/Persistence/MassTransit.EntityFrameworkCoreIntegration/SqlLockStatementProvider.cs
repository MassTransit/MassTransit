namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;
    using MassTransit.Saga;
    using Metadata;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;


    public class SqlLockStatementProvider :
        ILockStatementProvider
    {
        readonly bool _enableSchemaCaching;
        protected static readonly ConcurrentDictionary<Type, SchemaTablePair> TableNames = new ConcurrentDictionary<Type, SchemaTablePair>();

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

        string FormatLockStatement<TSaga>(IDbContextDependencies context)
            where TSaga : class, ISaga
        {
            var schemaTablePair = GetSchemaAndTableName(context, typeof(TSaga));

            return string.Format(RowLockStatement, schemaTablePair.Schema, schemaTablePair.Table);
        }

        SchemaTablePair GetSchemaAndTableName(IDbContextDependencies dependencies, Type type)
        {
            if (TableNames.TryGetValue(type, out var result) && _enableSchemaCaching)
                return result;

            var entityType = dependencies.Model.FindEntityType(type);

            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();

            if (string.IsNullOrWhiteSpace(tableName))
                throw new MassTransitException($"Unable to determine saga table name: {TypeMetadataCache.GetShortName(type)} (using model metadata).");

            result = new SchemaTablePair(schema ?? DefaultSchema, tableName);

            if (_enableSchemaCaching)
                TableNames.TryAdd(type, result);

            return result;
        }


        protected readonly struct SchemaTablePair
        {
            public SchemaTablePair(string schema, string table)
            {
                Schema = schema;
                Table = table;
            }

            public readonly string Schema;
            public readonly string Table;
        }
    }
}

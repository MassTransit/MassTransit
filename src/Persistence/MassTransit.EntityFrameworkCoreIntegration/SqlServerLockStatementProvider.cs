namespace MassTransit.EntityFrameworkCoreIntegration
{
    using System;
    using System.Collections.Concurrent;
    using MassTransit.Saga;
    using Metadata;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.EntityFrameworkCore.Metadata;


    public class SqlServerLockStatementProvider :
        ILockStatementProvider
    {
        const string DefaultRowLockStatement = "select * from {0}.{1} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @p0";
        const string DefaultSchemaName = "dbo";

        protected static readonly ConcurrentDictionary<Type, SchemaTablePair> TableNames = new ConcurrentDictionary<Type, SchemaTablePair>();
        protected readonly ConcurrentDictionary<Type, string> Statements = new ConcurrentDictionary<Type, string>();

        public SqlServerLockStatementProvider(string defaultSchema = DefaultSchemaName, string rowLockStatement = DefaultRowLockStatement,
            Func<IEntityType, IRelationalEntityTypeAnnotations> relationalEntityTypeAnnotations = null)
        {
            DefaultSchema = defaultSchema ?? throw new ArgumentNullException(nameof(defaultSchema));
            RowLockStatement = rowLockStatement ?? throw new ArgumentNullException(nameof(rowLockStatement));

            RelationalEntityTypeAnnotations = relationalEntityTypeAnnotations ?? GetRelationalEntityTypeAnnotations;
        }

        protected string DefaultSchema { get; }
        protected string RowLockStatement { get; }
        protected Func<IEntityType, IRelationalEntityTypeAnnotations> RelationalEntityTypeAnnotations { get; }

        public virtual string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga
        {
            return Statements.GetOrAdd(typeof(TSaga), type =>
            {
                var schemaTablePair = GetSchemaAndTableName(context, typeof(TSaga));

                return string.Format(RowLockStatement, schemaTablePair.Schema, schemaTablePair.Table);
            });
        }

        SchemaTablePair GetSchemaAndTableName(IDbContextDependencies dependencies, Type type)
        {
            if (TableNames.TryGetValue(type, out var result))
                return result;

            var annotations = RelationalEntityTypeAnnotations(dependencies.Model.FindEntityType(type));

            if (string.IsNullOrWhiteSpace(result.Table))
                throw new MassTransitException($"Unable to determine saga table name: {TypeMetadataCache.GetShortName(type)} (using model metadata).");

            result = new SchemaTablePair(annotations.Schema ?? DefaultSchema, annotations.TableName);

            TableNames.TryAdd(type, result);
            return result;
        }

        protected virtual IRelationalEntityTypeAnnotations GetRelationalEntityTypeAnnotations(IEntityType entityType)
        {
            return entityType.SqlServer();
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

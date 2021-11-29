namespace MassTransit.EntityFrameworkIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Infrastructure;


    public class SqlLockStatementProvider :
        ILockStatementProvider
    {
        protected static readonly ConcurrentDictionary<Type, SchemaTablePair> TableNames = new ConcurrentDictionary<Type, SchemaTablePair>();
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
            var schemaTablePair = GetSchemaAndTableName(context, typeof(TSaga));

            return string.Format(RowLockStatement, schemaTablePair.Schema, schemaTablePair.Table);
        }

        SchemaTablePair GetSchemaAndTableName(DbContext context, Type type)
        {
            if (TableNames.TryGetValue(type, out var result) && _enableSchemaCaching)
                return result;

            var entityName = type.Name;

            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            ReadOnlyCollection<EntityContainerMapping> storageMetadata = objectContext.MetadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace);

            foreach (var mapping in storageMetadata)
            {
                if (mapping.StoreEntityContainer.TryGetEntitySetByName(entityName, true, out var entitySet))
                {
                    result = new SchemaTablePair(entitySet.Schema ?? DefaultSchema, entitySet.Table);
                    break;
                }
            }

            if (result != null && _enableSchemaCaching)
                TableNames.TryAdd(type, result);

            return result;
        }


        protected class SchemaTablePair
        {
            public readonly string Schema;
            public readonly string Table;

            public SchemaTablePair(string schema, string table)
            {
                Schema = schema;
                Table = table;
            }
        }
    }
}

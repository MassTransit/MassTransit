namespace MassTransit.EntityFrameworkIntegration
{
    using MassTransit.Saga;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    public class MsSqlLockStatements : IRawSqlLockStatements
    {
        const string DefaultRowLockStatement = "select 1 from {0}.{1} WITH (UPDLOCK, ROWLOCK) WHERE CorrelationId = @p0";

        protected static readonly ConcurrentDictionary<Type, SchemaTablePair> TableNames = new ConcurrentDictionary<Type, SchemaTablePair>();

        public MsSqlLockStatements(
            string defaultSchema = "dbo",
            string rowLockStatement = DefaultRowLockStatement
            )
        {
            DefaultSchema = defaultSchema;
            RowLockStatement = rowLockStatement ?? throw new ArgumentNullException(nameof(rowLockStatement));
        }

        protected string DefaultSchema { get; }
        protected string RowLockStatement { get; }

        public virtual string GetRowLockStatement<TSaga>(DbContext context)
            where TSaga : class, ISaga
        {
            var schemaTablePair = GetSchemaAndTableName<TSaga>(context);

            return string.Format(RowLockStatement, schemaTablePair.Schema, schemaTablePair.Table);
        }

        private SchemaTablePair GetSchemaAndTableName<T>(DbContext context)
            where T : class
        {
            var t = typeof(T);

            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;

            if (!TableNames.TryGetValue(t, out var result))
            {

                string entityName = t.Name;

                ReadOnlyCollection<EntityContainerMapping> storageMetadata = objectContext.MetadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace);

                foreach (EntityContainerMapping ecm in storageMetadata)
                {
                    if (ecm.StoreEntityContainer.TryGetEntitySetByName(entityName, true, out var entitySet))
                    {
                        result = new SchemaTablePair
                        {
                            Schema = entitySet.Schema ?? DefaultSchema,
                            Table = entitySet.Table
                        };
                        break;
                    }
                }

                if(result != null)
                {
                    TableNames.TryAdd(t, result);
                }
                else
                {
                    throw new MassTransitException("Couldn't determine table and schema name (using metadata for EF Code First).");
                }
            }

            return result;
        }

        public class SchemaTablePair
        {
            public string Schema { get; set; }
            public string Table { get; set; }
        }
    }
}

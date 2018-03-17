namespace MassTransit.EntityFrameworkCoreIntegration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using System;
    using System.Collections.Concurrent;

    public class EntityFrameworkMetadataHelper : IRelationalEntityMetadataHelper
    {
        protected static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();

        protected string SchemaTableFormat { get; }
        protected string DefaultSchema { get; }
        protected Func<IEntityType, IRelationalEntityTypeAnnotations> RelationalEntityTypeAnnotations { get; }

        public EntityFrameworkMetadataHelper(string schemaTableFormat = "{0}.{1}", string defaultSchema = "dbo", Func<IEntityType, IRelationalEntityTypeAnnotations> relationalEntityTypeAnnotations = null)
        {
            SchemaTableFormat = schemaTableFormat;
            DefaultSchema = defaultSchema;
            RelationalEntityTypeAnnotations = relationalEntityTypeAnnotations ?? GetRelationalEntityTypeAnnotations;
        }

        public virtual string GetTableName<T>(DbContext context) where T : class
        {
            var t = typeof(T);

            string result;

            if (!TableNames.TryGetValue(t, out result))
            {
                var sql = GetRelationalEntityTypeAnnotations(context.Model.FindEntityType(t));
                var sqlSchema = sql.Schema;
                if (string.IsNullOrEmpty(sqlSchema))
                {
                    sqlSchema = DefaultSchema;
                }

                result = string.Format(SchemaTableFormat, sqlSchema, sql.TableName);

                if (!string.IsNullOrWhiteSpace(result))
                {
                    TableNames.TryAdd(t, result);
                }
                else
                {
                    throw new MassTransitException("Couldn't determine table and schema name (using model metadata).");
                }
            }

            return result;
        }

        protected virtual IRelationalEntityTypeAnnotations GetRelationalEntityTypeAnnotations(IEntityType entityType)
        {
            return entityType.SqlServer();
        }
    }
}

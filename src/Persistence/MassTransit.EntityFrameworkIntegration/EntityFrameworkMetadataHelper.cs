namespace MassTransit.EntityFrameworkIntegration
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Core.Mapping;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Infrastructure;

    public class EntityFrameworkMetadataHelper : IRelationalEntityMetadataHelper
    {
        protected static readonly ConcurrentDictionary<Type, string> TableNames = new ConcurrentDictionary<Type, string>();

        protected string SchemaTableFormat { get; } = "{0}.{1}";

        public EntityFrameworkMetadataHelper()
        {
        }

        public EntityFrameworkMetadataHelper(string schemaTableFormat)
        {
            SchemaTableFormat = schemaTableFormat;
        }

        public string GetTableName<T>(DbContext context) where T : class
        {
            ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
            return GetTableName(objectContext, typeof(T));
        }

        public virtual string GetTableName(ObjectContext context, Type t)
        {
            string result;

            if (!TableNames.TryGetValue(t, out result))
            {

                string entityName = t.Name;

                ReadOnlyCollection<EntityContainerMapping> storageMetadata = context.MetadataWorkspace.GetItems<EntityContainerMapping>(DataSpace.CSSpace);

                foreach (EntityContainerMapping ecm in storageMetadata)
                {
                    EntitySet entitySet;
                    if (ecm.StoreEntityContainer.TryGetEntitySetByName(entityName, true, out entitySet))
                    {
                        result = string.Format(SchemaTableFormat, entitySet.Schema, entitySet.Table);
                        break;
                    }
                }

                if(!string.IsNullOrWhiteSpace(result))
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
    }
}

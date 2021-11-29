namespace MassTransit.AzureTable.Saga
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Azure.Cosmos.Table;


    public class EntityConverter<T> :
        IEntityConverter<T>
        where T : class
    {
        readonly IList<IEntityPropertyConverter<T>> _converters;

        public EntityConverter(IList<IEntityPropertyConverter<T>> converters)
        {
            _converters = converters;
        }

        public IDictionary<string, EntityProperty> GetDictionary(T entity)
        {
            var entityProperties = new Dictionary<string, EntityProperty>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < _converters.Count; i++)
            {
                _converters[i].FromEntity(entity, entityProperties);
            }

            return entityProperties;
        }

        public T GetObject(IDictionary<string, EntityProperty> entityProperties)
        {
            var entity = (T)Activator.CreateInstance(typeof(T));

            for (int i = 0; i < _converters.Count; i++)
            {
                _converters[i].ToEntity(entity, entityProperties);
            }

            return entity;
        }
    }
}

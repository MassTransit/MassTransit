namespace MassTransit.AzureTable.Saga
{
    using System;
    using System.Collections.Generic;


    public class EntityConverter<T> :
        IEntityConverter<T>
        where T : class
    {
        readonly IList<IEntityPropertyConverter<T>> _converters;

        public EntityConverter(IList<IEntityPropertyConverter<T>> converters)
        {
            _converters = converters;
        }

        public IDictionary<string, object> GetDictionary(T entity)
        {
            var entityProperties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < _converters.Count; i++)
                _converters[i].FromEntity(entity, entityProperties);

            return entityProperties;
        }

        public T GetObject(IDictionary<string, object> entityProperties)
        {
            var entity = (T)Activator.CreateInstance(typeof(T));

            for (var i = 0; i < _converters.Count; i++)
                _converters[i].ToEntity(entity, entityProperties);

            return entity;
        }
    }
}

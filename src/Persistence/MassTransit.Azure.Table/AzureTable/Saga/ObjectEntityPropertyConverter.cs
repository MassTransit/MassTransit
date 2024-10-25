namespace MassTransit.AzureTable.Saga
{
    using System.Collections.Generic;
    using Internals;
    using Azure.Data.Tables;
    using Serialization;


    public class ObjectEntityPropertyConverter<TEntity, TProperty> :
        IEntityPropertyConverter<TEntity>
        where TEntity : class
        where TProperty : class
    {
        readonly string _name;
        readonly IReadProperty<TEntity, TProperty> _read;
        readonly IWriteProperty<TEntity, TProperty> _write;

        public ObjectEntityPropertyConverter(string name)
        {
            _name = name;
            _read = ReadPropertyCache<TEntity>.GetProperty<TProperty>(name);
            _write = WritePropertyCache<TEntity>.GetProperty<TProperty>(name);
        }

        public void ToEntity(TEntity entity, TableEntity tableEntity)
        {
            if (tableEntity.TryGetValue(_name, out var entityProperty))
            {
                var propertyValue = ObjectDeserializer.Deserialize<TProperty>(entityProperty);

                _write.Set(entity, propertyValue);
            }
        }

        public void FromEntity(TEntity entity, IDictionary<string, object> entityProperties)
        {
            var propertyValue = _read.Get(entity);

            var text = ObjectDeserializer.Serialize(propertyValue);
            if (!string.IsNullOrWhiteSpace(text))
                entityProperties.Add(_name, text);
        }
    }
}

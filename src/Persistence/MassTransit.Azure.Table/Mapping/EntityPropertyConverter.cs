namespace MassTransit.Azure.Table.Mapping
{
    using System.Collections.Generic;
    using Initializers;
    using Internals.Reflection;
    using Microsoft.Azure.Cosmos.Table;
    using Newtonsoft.Json;


    public class EntityPropertyConverter<TEntity, TProperty> :
        IEntityPropertyConverter<TEntity>
        where TEntity : class
    {
        readonly ITypeConverter<EntityProperty, TProperty> _fromEntity;
        readonly string _name;
        readonly IReadProperty<TEntity, TProperty> _read;
        readonly ITypeConverter<TProperty, EntityProperty> _toEntity;
        readonly IWriteProperty<TEntity, TProperty> _write;

        public EntityPropertyConverter(string name)
        {
            _name = name;
            _read = ReadPropertyCache<TEntity>.GetProperty<TProperty>(name);
            _write = WritePropertyCache<TEntity>.GetProperty<TProperty>(name);

            _toEntity = EntityPropertyTypeConverter.Instance as ITypeConverter<TProperty, EntityProperty>;
            _fromEntity = EntityPropertyTypeConverter.Instance as ITypeConverter<EntityProperty, TProperty>;
        }

        public void ToEntity(TEntity entity, IDictionary<string, EntityProperty> entityProperties)
        {
            if (entityProperties.TryGetValue(_name, out var entityProperty))
            {
                if (_toEntity != null)
                {
                    if (_toEntity.TryConvert(entityProperty, out var propertyValue))
                        _write.Set(entity, propertyValue);
                }
                else
                {
                    var propertyValue = JsonConvert.DeserializeObject<TProperty>(entityProperty.StringValue);

                    _write.Set(entity, propertyValue);
                }
            }
        }

        public void FromEntity(TEntity entity, IDictionary<string, EntityProperty> entityProperties)
        {
            var propertyValue = _read.Get(entity);

            if (_fromEntity != null)
            {
                if (_fromEntity.TryConvert(propertyValue, out var entityProperty))
                    entityProperties.Add(_name, entityProperty);
            }
            else
            {
                var text = JsonConvert.SerializeObject(propertyValue, Formatting.None);
                entityProperties.Add(_name, new EntityProperty(text));
            }
        }
    }
}

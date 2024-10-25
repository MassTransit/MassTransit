namespace MassTransit.AzureTable.Saga
{
    using System;
    using System.Collections.Generic;
    using Initializers;
    using Internals;
    using Azure.Data.Tables;


    public class EntityPropertyConverter<TEntity, TProperty> :
        IEntityPropertyConverter<TEntity>
        where TEntity : class
    {
        readonly ITypeConverter<object, TProperty> _fromEntity;
        readonly string _name;
        readonly IReadProperty<TEntity, TProperty> _read;
        readonly ITypeConverter<TProperty, object> _toEntity;
        readonly IWriteProperty<TEntity, TProperty> _write;

        public EntityPropertyConverter(string name)
        {
            _name = name;
            _read = ReadPropertyCache<TEntity>.GetProperty<TProperty>(name);
            _write = WritePropertyCache<TEntity>.GetProperty<TProperty>(name);

            _toEntity = objectTypeConverter.Instance as ITypeConverter<TProperty, object>
                ?? throw new ArgumentException("Invalid property type");

            _fromEntity = objectTypeConverter.Instance as ITypeConverter<object, TProperty>
                ?? throw new ArgumentException("Invalid property type");
        }

        public void ToEntity(TEntity entity, TableEntity tableEntity)
        {
            if (tableEntity.TryGetValue(_name, out var entityProperty))
            {
                if (_toEntity.TryConvert(entityProperty, out var propertyValue))
                    _write.Set(entity, propertyValue);
            }
        }

        public void FromEntity(TEntity entity, IDictionary<string, object> entityProperties)
        {
            var propertyValue = _read.Get(entity);

            if (_fromEntity.TryConvert(propertyValue, out var entityProperty))
                entityProperties.Add(_name, entityProperty);
        }
    }
}

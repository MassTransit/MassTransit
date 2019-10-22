namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Metadata;


    public class WritePropertyCache<T> :
        IWritePropertyCache<T>
    {
        readonly Type _implementationType;
        readonly IDictionary<string, IWriteProperty<T>> _properties;

        WritePropertyCache()
        {
            _implementationType = TypeMetadataCache<T>.IsValidMessageType && typeof(T).GetTypeInfo().IsInterface
                ? TypeMetadataCache<T>.ImplementationType
                : typeof(T);

            _properties = new Dictionary<string, IWriteProperty<T>>(StringComparer.OrdinalIgnoreCase);
        }

        IWriteProperty<T, TProperty> IWritePropertyCache<T>.GetProperty<TProperty>(string name)
        {
            lock (_properties)
            {
                if (_properties.TryGetValue(name, out var property))
                    return property as IWriteProperty<T, TProperty>;

                var writeProperty = new WriteProperty<T, TProperty>(_implementationType, name);

                _properties[name] = writeProperty;

                return writeProperty;
            }
        }

        IWriteProperty<T, TProperty> IWritePropertyCache<T>.GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            lock (_properties)
            {
                var name = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

                if (_properties.TryGetValue(name, out var property))
                    return property as IWriteProperty<T, TProperty>;

                var writeProperty = new WriteProperty<T, TProperty>(_implementationType, propertyInfo);

                _properties[name] = writeProperty;

                return writeProperty;
            }
        }

        public static IWriteProperty<T, TProperty> GetProperty<TProperty>(string name)
        {
            return Cached.PropertyCache.GetProperty<TProperty>(name);
        }

        public static IWriteProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            return Cached.PropertyCache.GetProperty<TProperty>(propertyInfo);
        }


        static class Cached
        {
            internal static readonly IWritePropertyCache<T> PropertyCache = new WritePropertyCache<T>();
        }
    }
}

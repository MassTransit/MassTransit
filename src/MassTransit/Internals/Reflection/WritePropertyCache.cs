namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Generic;
    using Util;


    public class WritePropertyCache<T> :
        IWritePropertyCache<T>
    {
        readonly Type _implementationType;
        readonly IDictionary<string, IWriteProperty<T>> _properties;

        WritePropertyCache()
        {
            _implementationType = TypeMetadataCache<T>.ImplementationType;

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

        public static IWriteProperty<T, TProperty> GetProperty<TProperty>(string name)
        {
            return Cached.PropertyCache.GetProperty<TProperty>(name);
        }


        static class Cached
        {
            internal static readonly IWritePropertyCache<T> PropertyCache = new WritePropertyCache<T>();
        }
    }
}
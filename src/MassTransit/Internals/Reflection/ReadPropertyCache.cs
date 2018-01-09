namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Generic;
    using Util;


    public class ReadPropertyCache<T> :
        IReadPropertyCache<T>
    {
        readonly Type _implementationType;
        readonly IDictionary<string, IReadProperty<T>> _properties;

        ReadPropertyCache()
        {
            _implementationType = typeof(T).IsInterface ? TypeMetadataCache<T>.ImplementationType : typeof(T);

            _properties = new Dictionary<string, IReadProperty<T>>(StringComparer.OrdinalIgnoreCase);
        }

        IReadProperty<T, TProperty> IReadPropertyCache<T>.GetProperty<TProperty>(string name)
        {
            lock (_properties)
            {
                if (_properties.TryGetValue(name, out var property))
                    return property as IReadProperty<T, TProperty>;

                var writeProperty = new ReadProperty<T, TProperty>(_implementationType, name);

                _properties[name] = writeProperty;

                return writeProperty;
            }
        }

        public static IReadProperty<T, TProperty> GetProperty<TProperty>(string name)
        {
            return Cached.PropertyCache.GetProperty<TProperty>(name);
        }


        static class Cached
        {
            internal static readonly IReadPropertyCache<T> PropertyCache = new ReadPropertyCache<T>();
        }
    }
}
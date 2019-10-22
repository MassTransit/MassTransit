namespace MassTransit.Internals.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Metadata;


    public class ReadPropertyCache<T> :
        IReadPropertyCache<T>
    {
        readonly IDictionary<string, IReadProperty<T>> _properties;

        ReadPropertyCache()
        {
            _properties = new Dictionary<string, IReadProperty<T>>(StringComparer.OrdinalIgnoreCase);
        }

        IReadProperty<T, TProperty> IReadPropertyCache<T>.GetProperty<TProperty>(string name)
        {
            lock (_properties)
            {
                if (_properties.TryGetValue(name, out var property))
                    return property as IReadProperty<T, TProperty>;

                var readProperty = new ReadProperty<T, TProperty>(name);

                _properties[name] = readProperty;

                return readProperty;
            }
        }

        IReadProperty<T, TProperty> IReadPropertyCache<T>.GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            lock (_properties)
            {
                var name = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

                if (_properties.TryGetValue(name, out var property))
                    return property as IReadProperty<T, TProperty>;

                if (propertyInfo.PropertyType != typeof(TProperty))
                    throw new ArgumentException(
                        $"Property type mismatch, {TypeMetadataCache<TProperty>.ShortName} != {TypeMetadataCache.GetShortName(propertyInfo.PropertyType)}",
                        nameof(propertyInfo));

                var readProperty = new ReadProperty<T, TProperty>(propertyInfo);

                _properties[name] = readProperty;

                return readProperty;
            }
        }

        public static IReadProperty<T, TProperty> GetProperty<TProperty>(string name)
        {
            return Cached.PropertyCache.GetProperty<TProperty>(name);
        }

        public static IReadProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            return Cached.PropertyCache.GetProperty<TProperty>(propertyInfo);
        }


        static class Cached
        {
            internal static readonly IReadPropertyCache<T> PropertyCache = new ReadPropertyCache<T>();
        }
    }
}

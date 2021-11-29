namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;


    public class ReadPropertyCache<T> :
        IReadPropertyCache<T>
        where T : class
    {
        readonly IDictionary<string, IReadProperty<T>> _properties;
        readonly IDictionary<string, PropertyInfo> _propertyIndex;

        ReadPropertyCache()
        {
            _properties = new Dictionary<string, IReadProperty<T>>(StringComparer.OrdinalIgnoreCase);
            _propertyIndex = MessageTypeCache<T>.Properties
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        }

        IReadProperty<T, TProperty> IReadPropertyCache<T>.GetProperty<TProperty>(string name)
        {
            return GetReadProperty<TProperty>(name ?? throw new ArgumentNullException(nameof(name)));
        }

        IReadProperty<T, TProperty> IReadPropertyCache<T>.GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            var name = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            return GetReadProperty<TProperty>(name);
        }

        IReadProperty<T, TProperty> GetReadProperty<TProperty>(string name)
        {
            lock (_properties)
            {
                if (_properties.TryGetValue(name, out IReadProperty<T> property))
                    return property as IReadProperty<T, TProperty>;

                if (_propertyIndex.TryGetValue(name, out var propertyInfo))
                {
                    if (propertyInfo.PropertyType != typeof(TProperty))
                    {
                        throw new ArgumentException(
                            $"Property type mismatch, {TypeCache<TProperty>.ShortName} != {TypeCache.GetShortName(propertyInfo.PropertyType)}");
                    }

                    var readProperty = new ReadProperty<T, TProperty>(propertyInfo);

                    _properties[name] = readProperty;

                    return readProperty;
                }
            }

            throw new ArgumentException($"{TypeCache<T>.ShortName} does not contain the property: {name}", nameof(name));
        }

        public static IReadProperty<T, TProperty> GetProperty<TProperty>(string name)
        {
            return Cached.PropertyCache.Value.GetProperty<TProperty>(name);
        }

        public static IReadProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            return Cached.PropertyCache.Value.GetProperty<TProperty>(propertyInfo);
        }


        static class Cached
        {
            internal static readonly Lazy<IReadPropertyCache<T>> PropertyCache = new Lazy<IReadPropertyCache<T>>(() => new ReadPropertyCache<T>());
        }
    }
}

namespace MassTransit.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Metadata;


    public class WritePropertyCache<T> :
        IWritePropertyCache<T>
        where T : class
    {
        readonly Type _implementationType;
        readonly IDictionary<string, IWriteProperty<T>> _properties;
        readonly IDictionary<string, PropertyInfo> _propertyIndex;

        WritePropertyCache()
        {
            if (MessageTypeCache<T>.IsValidMessageType && typeof(T).GetTypeInfo().IsInterface)
            {
                _implementationType = TypeMetadataCache<T>.ImplementationType;
                _propertyIndex = _implementationType.GetAllProperties()
                    .GroupBy(x => x.Name)
                    .Select(x => x.Last())
                    .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                _implementationType = typeof(T);
                _propertyIndex = MessageTypeCache<T>.Properties
                    .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            }

            _properties = new Dictionary<string, IWriteProperty<T>>(StringComparer.OrdinalIgnoreCase);
        }

        bool IWritePropertyCache<T>.CanWrite(string name)
        {
            if (_propertyIndex.TryGetValue(name, out var propertyInfo))
                return propertyInfo.CanWrite;

            throw new ArgumentException($"{TypeCache<T>.ShortName} does not contain the property: {name}", nameof(name));
        }

        IWriteProperty<T, TProperty> IWritePropertyCache<T>.GetProperty<TProperty>(string name)
        {
            return GetWriteProperty<TProperty>(name);
        }

        IWriteProperty<T, TProperty> IWritePropertyCache<T>.GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            var name = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            return GetWriteProperty<TProperty>(name);
        }

        IWriteProperty<T, TProperty> GetWriteProperty<TProperty>(string name)
        {
            lock (_properties)
            {
                if (_properties.TryGetValue(name, out IWriteProperty<T> property))
                    return property as IWriteProperty<T, TProperty>;

                if (_propertyIndex.TryGetValue(name, out var propertyInfo))
                {
                    if (propertyInfo.PropertyType != typeof(TProperty))
                    {
                        throw new ArgumentException(
                            $"Property type mismatch, {TypeCache<TProperty>.ShortName} != {TypeCache.GetShortName(propertyInfo.PropertyType)}");
                    }

                    var writeProperty = new WriteProperty<T, TProperty>(_implementationType, propertyInfo);

                    _properties[name] = writeProperty;

                    return writeProperty;
                }
            }

            throw new ArgumentException($"{TypeCache<T>.ShortName} does not contain the property: {name}", nameof(name));
        }

        public static IWriteProperty<T, TProperty> GetProperty<TProperty>(string name)
        {
            return Cached.PropertyCache.Value.GetProperty<TProperty>(name);
        }

        public static IWriteProperty<T, TProperty> GetProperty<TProperty>(PropertyInfo propertyInfo)
        {
            return Cached.PropertyCache.Value.GetProperty<TProperty>(propertyInfo);
        }

        public static bool CanWrite(string name)
        {
            return Cached.PropertyCache.Value.CanWrite(name);
        }


        static class Cached
        {
            internal static readonly Lazy<IWritePropertyCache<T>> PropertyCache = new Lazy<IWritePropertyCache<T>>(() => new WritePropertyCache<T>());
        }
    }
}

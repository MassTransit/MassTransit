namespace MassTransit.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Contracts;
    using Internals.Extensions;


    public class PropertyInfoCache<T> :
        IPropertyInfoCache
    {
        readonly Contracts.PropertyInfo[] _properties;

        PropertyInfoCache()
        {
            _properties = TypeMetadataCache<T>.Properties.Select(GetPropertyInfo).ToArray();
        }

        Contracts.PropertyInfo[] IPropertyInfoCache.Properties => _properties;

        public static Contracts.PropertyInfo[] Properties => Cached.Instance.Value.Properties;

        Contracts.PropertyInfo GetPropertyInfo(System.Reflection.PropertyInfo propertyInfo)
        {
            return GetPropertyInfo(propertyInfo, propertyInfo.PropertyType.GetTypeInfo());
        }

        Contracts.PropertyInfo GetPropertyInfo(System.Reflection.PropertyInfo propertyInfo, TypeInfo type)
        {
            if (type == typeof(object))
                return new CachedPropertyInfo(propertyInfo.Name, PropertyKind.Object, TypeMetadataCache<object>.ShortName);

            if (type.IsTask(out var taskType))
                return GetPropertyInfo(propertyInfo, taskType.GetTypeInfo());

            if (type.IsNullable(out var underlyingType))
                return GetValuePropertyInfo(propertyInfo, underlyingType.GetTypeInfo(), PropertyKind.Nullable);

            if (type.IsValueTypeOrObject())
                return GetValuePropertyInfo(propertyInfo, type);

            if (type.ClosesType(typeof(IDictionary<,>), out Type[] types) || type.ClosesType(typeof(IReadOnlyDictionary<,>), out types))
                return GetDictionaryPropertyInfo(propertyInfo, types[0].GetTypeInfo(), types[1].GetTypeInfo());

            if (type.IsArray)
                return GetArrayPropertyInfo(propertyInfo, type.GetElementType().GetTypeInfo());

            if (type.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes))
            {
                if (enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out types))
                    return GetDictionaryPropertyInfo(propertyInfo, types[0].GetTypeInfo(), types[1].GetTypeInfo());

                return GetArrayPropertyInfo(propertyInfo, enumerableTypes[0].GetTypeInfo());
            }

            return GetObjectPropertyInfo(propertyInfo, type);
        }

        Contracts.PropertyInfo GetValuePropertyInfo(System.Reflection.PropertyInfo propertyInfo, TypeInfo propertyType, PropertyKind kindFlags = default)
        {
            return new CachedPropertyInfo(propertyInfo.Name, PropertyKind.Value | kindFlags, TypeMetadataCache.GetShortName(propertyType));
        }

        Contracts.PropertyInfo GetDictionaryPropertyInfo(System.Reflection.PropertyInfo propertyInfo, TypeInfo keyType, TypeInfo valueType,
            PropertyKind kindFlags = default)
        {
            var keyPropertyInfo = GetPropertyInfo(propertyInfo, keyType);
            var valuePropertyInfo = GetPropertyInfo(propertyInfo, valueType);

            return new CachedPropertyInfo(propertyInfo.Name, (valuePropertyInfo.Kind & PropertyKind.Object) | PropertyKind.Dictionary | kindFlags,
                valuePropertyInfo.PropertyType, keyPropertyInfo.PropertyType);
        }

        Contracts.PropertyInfo GetArrayPropertyInfo(System.Reflection.PropertyInfo propertyInfo, TypeInfo valueType, PropertyKind kindFlags = default)
        {
            var elementPropertyInfo = GetPropertyInfo(propertyInfo, valueType);

            return new CachedPropertyInfo(propertyInfo.Name, (elementPropertyInfo.Kind & PropertyKind.Object) | PropertyKind.Array | kindFlags,
                elementPropertyInfo.PropertyType);
        }

        Contracts.PropertyInfo GetObjectPropertyInfo(System.Reflection.PropertyInfo propertyInfo, TypeInfo valueType, PropertyKind kindFlags = default)
        {
            var objectInfo = ObjectInfoCache.GetObjectInfo(valueType);

            return new CachedPropertyInfo(propertyInfo.Name, PropertyKind.Object | kindFlags, objectInfo.ObjectType);
        }


        static class Cached
        {
            internal static readonly Lazy<IPropertyInfoCache> Instance = new Lazy<IPropertyInfoCache>(() => new PropertyInfoCache<T>());
        }


        class CachedPropertyInfo :
            Contracts.PropertyInfo
        {
            public CachedPropertyInfo(string name, PropertyKind kind, string propertyType, string keyType = default)
            {
                Name = name;
                Kind = kind;
                PropertyType = propertyType;
                KeyType = keyType;
            }

            public string Name { get; }

            public PropertyKind Kind { get; }

            public string PropertyType { get; }

            public string KeyType { get; }
        }
    }
}

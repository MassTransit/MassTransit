namespace MassTransit.Internals.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Reflection;


    public class ObjectDictionaryConverter<T> :
        IDictionaryConverter
        where T : class
    {
        readonly DictionaryConverterCache _cache;
        readonly IDictionaryMapper<T>[] _mappers;
        readonly ReadOnlyPropertyCache<T> _propertyCache;

        public ObjectDictionaryConverter(DictionaryConverterCache cache)
        {
            _cache = cache;
            _propertyCache = new ReadOnlyPropertyCache<T>();
            _mappers = _propertyCache
                .Select(property => GetDictionaryMapper(property, property.Property.PropertyType))
                .ToArray();
        }

        public IDictionary<string, object> GetDictionary(object obj)
        {
            var dictionary = new Dictionary<string, object>();
            if (obj == null)
                return dictionary;

            var value = (T)obj;
            for (int i = 0; i < _mappers.Length; i++)
                _mappers[i].WritePropertyToDictionary(dictionary, value);

            return dictionary;
        }

        IDictionaryMapper<T> GetDictionaryMapper(ReadOnlyProperty<T> property, Type valueType)
        {
            Type underlyingType;
            if (valueType.IsNullable(out underlyingType))
            {
                Type converterType = typeof(NullableValueDictionaryMapper<,>).MakeGenericType(typeof(T),
                    underlyingType);

                return (IDictionaryMapper<T>)Activator.CreateInstance(converterType, property);
            }

            if (valueType.IsEnum)
                return new EnumDictionaryMapper<T>(property);

            if (valueType.IsArray)
            {
                Type elementType = valueType.GetElementType();
                if (elementType.IsValueType || elementType == typeof(string))
                    return new ValueDictionaryMapper<T>(property);

                IDictionaryConverter elementConverter = _cache.GetConverter(elementType);

                Type converterType = typeof(ObjectArrayDictionaryMapper<,>).MakeGenericType(typeof(T), elementType);
                return (IDictionaryMapper<T>)Activator.CreateInstance(converterType, property, elementConverter);
            }

            if (valueType.IsValueType || valueType == typeof(string))
                return new ValueDictionaryMapper<T>(property);

            if (valueType.HasInterface(typeof(IEnumerable<>)))
            {
                Type elementType = valueType.GetClosingArguments(typeof(IEnumerable<>)).Single();

                if (valueType.ClosesType(typeof(IDictionary<,>)))
                {
                    Type[] arguments = valueType.GetClosingArguments(typeof(IDictionary<,>)).ToArray();
                    Type keyType = arguments[0];
                    if (keyType.IsValueType || keyType == typeof(string))
                    {
                        elementType = arguments[1];
                        if (elementType.IsValueType || elementType == typeof(string))
                        {
                            Type converterType =
                                typeof(ValueValueDictionaryDictionaryMapper<,,>).MakeGenericType(typeof(T),
                                    keyType, elementType);
                            return (IDictionaryMapper<T>)Activator.CreateInstance(converterType, property);
                        }
                        else
                        {
                            Type converterType =
                                typeof(ValueObjectDictionaryDictionaryMapper<,,>).MakeGenericType(typeof(T),
                                    keyType, elementType);
                            IDictionaryConverter elementConverter = _cache.GetConverter(elementType);
                            return
                                (IDictionaryMapper<T>)Activator.CreateInstance(converterType, property, elementConverter);
                        }
                    }

                    throw new InvalidOperationException("Unable to map a reference type key dictionary");
                }

                if (valueType.ClosesType(typeof(IList<>)) || valueType.ClosesType(typeof(IEnumerable<>)))
                {
                    Type converterType = typeof(ObjectListDictionaryMapper<,>).MakeGenericType(typeof(T), elementType);
                    IDictionaryConverter elementConverter = _cache.GetConverter(elementType);

                    return (IDictionaryMapper<T>)Activator.CreateInstance(converterType, property, elementConverter);
                }
            }
            return new ObjectDictionaryMapper<T>(property, _cache.GetConverter(valueType));
        }
    }
}
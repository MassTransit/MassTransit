// Copyright 2007-2015 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.Internals.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Extensions;
    using Reflection;
    using Util;


    public class DynamicObjectConverter<T, TImplementation> :
        IObjectConverter
        where TImplementation : T, new()
        where T : class
    {
        readonly IObjectConverterCache _cache;
        readonly IObjectMapper<TImplementation>[] _converters;

        public DynamicObjectConverter(IObjectConverterCache cache)
        {
            _cache = cache;
            _converters = TypeMetadataCache<TImplementation>.ReadWritePropertyCache
                .Select(property => GetDictionaryToObjectConverter(property, property.Property.PropertyType))
                .ToArray();
        }

        public object GetObject(IObjectValueProvider valueProvider)
        {
            var implementation = new TImplementation();

            for (int i = 0; i < _converters.Length; i++)
                _converters[i].ApplyTo(implementation, valueProvider);

            return implementation;
        }

        IObjectMapper<TImplementation> GetDictionaryToObjectConverter(ReadWriteProperty<TImplementation> property, Type valueType)
        {
            Type underlyingType = Nullable.GetUnderlyingType(valueType);
            if (underlyingType != null)
            {
                Type converterType =
                    typeof(NullableValueObjectMapper<,>).MakeGenericType(typeof(TImplementation),
                        underlyingType);

                return (IObjectMapper<TImplementation>)Activator.CreateInstance(converterType, property);
            }

            if (valueType.IsEnum)
                return new EnumObjectMapper<TImplementation>(property);

            if (valueType.IsArray)
            {
                Type elementType = valueType.GetElementType();
                if (elementType.IsValueType || elementType == typeof(string))
                {
                    Type valueConverterType = typeof(ValueArrayObjectMapper<,>).MakeGenericType(
                        typeof(TImplementation), elementType);
                    return (IObjectMapper<TImplementation>)Activator.CreateInstance(valueConverterType, property);
                }

                IObjectConverter elementConverter = _cache.GetConverter(elementType);

                Type converterType = typeof(ObjectArrayObjectMapper<,>).MakeGenericType(typeof(TImplementation),
                    elementType);
                return
                    (IObjectMapper<TImplementation>)Activator.CreateInstance(converterType, property, elementConverter);
            }

            if (valueType.IsValueType || valueType == typeof(string))
                return new ValueObjectMapper<TImplementation>(property);

            if (valueType.ClosesType(typeof(IEnumerable<>)))
            {
                if (valueType.ClosesType(typeof(IDictionary<,>)))
                {
                    Type[] genericArguments = valueType.GetClosingArguments(typeof(IDictionary<,>)).ToArray();

                    Type keyType = genericArguments[0];
                    Type elementType = genericArguments[1];


                    if (keyType.IsValueType || keyType == typeof(string))
                    {
                        if (elementType.IsValueType || elementType == typeof(string))
                        {
                            Type valueConverterType =
                                typeof(ValueValueDictionaryObjectMapper<,,>).MakeGenericType(typeof(TImplementation),
                                    keyType, elementType);
                            return (IObjectMapper<TImplementation>)Activator.CreateInstance(valueConverterType, property);
                        }
                        else
                        {
                            IObjectConverter elementConverter = _cache.GetConverter(elementType);
                            Type valueConverterType =
                                typeof(ValueObjectDictionaryObjectMapper<,,>).MakeGenericType(typeof(TImplementation),
                                    keyType, elementType);
                            return
                                (IObjectMapper<TImplementation>)
                                    Activator.CreateInstance(valueConverterType, property, elementConverter);
                        }
                    }

                    throw new InvalidOperationException("A dictionary with a reference type key is not supported: "
                        + property.Property.Name);
                }


                if (valueType.ClosesType(typeof(IList<>)) || valueType.ClosesType(typeof(IEnumerable<>)))
                {
                    Type[] genericArguments = valueType.GetClosingArguments(typeof(IEnumerable<>)).ToArray();
                    Type elementType = genericArguments[0];

                    if (elementType.IsValueType || elementType == typeof(string))
                    {
                        Type valueConverterType =
                            typeof(ValueListObjectMapper<,>).MakeGenericType(typeof(TImplementation),
                                elementType);

                        return (IObjectMapper<TImplementation>)Activator.CreateInstance(valueConverterType, property);
                    }

                    IObjectConverter elementConverter = _cache.GetConverter(elementType);

                    Type converterType = typeof(ObjectListObjectMapper<,>).MakeGenericType(typeof(TImplementation),
                        elementType);

                    return (IObjectMapper<TImplementation>)Activator.CreateInstance(converterType, property,
                        elementConverter);
                }
            }

            return new ObjectObjectMapper<TImplementation>(property, _cache.GetConverter(valueType));
        }
    }
}
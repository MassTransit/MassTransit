namespace MassTransit.Initializers.PropertyInitializers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Internals.Extensions;
    using PropertyConverters;
    using TypeConverters;
    using Variables;


    public class PropertyInitializerCache
    {
        static class Cached
        {
            internal static readonly ConcurrentDictionary<Type, SourceCandidate> SourceTypes = new ConcurrentDictionary<Type, SourceCandidate>();
            internal static readonly ConcurrentDictionary<Type, NullableCandidate> NullableTypes = new ConcurrentDictionary<Type, NullableCandidate>();
            internal static readonly ConcurrentDictionary<Type, EnumerableCandidate> EnumerableTypes = new ConcurrentDictionary<Type, EnumerableCandidate>();
            internal static readonly ConcurrentDictionary<Type, DictionaryCandidate> DictionaryTypes = new ConcurrentDictionary<Type, DictionaryCandidate>();
        }


        /// <summary>
        /// Returns the factory to convert the <paramref name="sourceType"/> to the <typeparamref name="T"/>.
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        public static bool TryGetFactory<T>(Type sourceType, out IPropertyInitializerFactory<T> converter)
        {
            return Cached.SourceTypes.GetOrAdd(sourceType, CreateSourceType).TryGetFactory(out converter);
        }

        static SourceCandidate CreateSourceType(Type type)
        {
            if (type.IsTask(out var taskType))
                return (SourceCandidate)Activator.CreateInstance(typeof(TaskSourceCandidate<>).MakeGenericType(taskType));

            if (type.IsNullable(out var underlyingType))
                return (SourceCandidate)Activator.CreateInstance(typeof(NullableSourceCandidate<>).MakeGenericType(underlyingType));

            if (type.IsValueType)
                return (SourceCandidate)Activator.CreateInstance(typeof(ValueSourceCandidate<>).MakeGenericType(type));

            if (type.ClosesType(typeof(IInitializerVariable<>), out Type[] variableTypes))
                return (SourceCandidate)Activator.CreateInstance(typeof(VariableSourceCandidate<,>).MakeGenericType(type, variableTypes[0]));

            if (type.IsArray)
                return (SourceCandidate)Activator.CreateInstance(typeof(EnumerableSourceCandidate<>).MakeGenericType(type.GetElementType()));

            if (type == typeof(string))
                return (SourceCandidate)Activator.CreateInstance(typeof(GeneralSourceCandidate<>).MakeGenericType(type));

            if (type.ClosesType(typeof(IDictionary<,>), out Type[] dictionaryTypes))
                return (SourceCandidate)Activator.CreateInstance(typeof(DictionarySourceCandidate<,>).MakeGenericType(dictionaryTypes));

            if (type.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes))
            {
                if (enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out Type[] keyValueTypes))
                    return (SourceCandidate)Activator.CreateInstance(typeof(DictionarySourceCandidate<,>).MakeGenericType(keyValueTypes));

                return (SourceCandidate)Activator.CreateInstance(typeof(EnumerableSourceCandidate<>).MakeGenericType(enumerableTypes[0]));
            }

            return (SourceCandidate)Activator.CreateInstance(typeof(GeneralSourceCandidate<>).MakeGenericType(type));
        }

        static bool TryGetNullableFactory<T, TInput>(Type sourceType, out IPropertyInitializerFactory<T> converter)
        {
            return Cached.NullableTypes.GetOrAdd(sourceType, CreateNullableType).TryGetNullableFactory<T, TInput>(out converter);
        }

        static NullableCandidate CreateNullableType(Type type)
        {
            return (NullableCandidate)Activator.CreateInstance(typeof(NullableCandidate<>).MakeGenericType(type));
        }

        static bool TryGetEnumerableFactory<T, TInputElement>(Type sourceType, out IPropertyInitializerFactory<T> converter)
        {
            return Cached.EnumerableTypes.GetOrAdd(sourceType, CreateEnumerableType).TryGetEnumerableFactory<T, TInputElement>(out converter);
        }

        static bool TryGetDictionaryFactory<T, TInputKey, TInputValue>(Type sourceType, out IPropertyInitializerFactory<T> converter)
        {
            return Cached.DictionaryTypes.GetOrAdd(sourceType, CreateDictionaryType).TryGetDictionaryFactory<T, TInputKey, TInputValue>(out converter);
        }

        static EnumerableCandidate CreateEnumerableType(Type type)
        {
            return (EnumerableCandidate)Activator.CreateInstance(typeof(EnumerableCandidate<>).MakeGenericType(type));
        }

        static DictionaryCandidate CreateDictionaryType(Type type)
        {
            if (type.ClosesType(typeof(IDictionary<,>), out Type[] arguments))
                return (DictionaryCandidate)Activator.CreateInstance(typeof(DictionaryCandidate<,>).MakeGenericType(arguments));

            if (type.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes)
                && enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out Type[] keyValueTypes))
                return (DictionaryCandidate)Activator.CreateInstance(typeof(DictionaryCandidate<,>).MakeGenericType(keyValueTypes));

            if (type.IsInterface && !type.IsValueTypeOrObject())
                return (DictionaryCandidate)Activator.CreateInstance(typeof(DictionaryObjectSourceCandidate<>).MakeGenericType(type));

            return new UnsupportedDictionaryCandidate();
        }


        interface SourceCandidate
        {
            bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter);
        }


        interface NullableCandidate
        {
            bool TryGetNullableFactory<T, TInput>(out IPropertyInitializerFactory<T> converter);
        }


        interface EnumerableCandidate
        {
            bool TryGetEnumerableFactory<T, TInputElement>(out IPropertyInitializerFactory<T> converter);
        }


        interface DictionaryCandidate
        {
            bool TryGetDictionaryFactory<T, TInputKey, TInputValue>(out IPropertyInitializerFactory<T> converter);
        }


        class NullableCandidate<TValue> :
            NullableCandidate
            where TValue : struct
        {
            public bool TryGetNullableFactory<T, TInput>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(TValue) != typeof(T))
                {
                    if (TypeConverterCache.TryGetTypeConverter<TValue, TInput>(out var simpleTypeConverter))
                    {
                        var propertyConverter = (ITypeConverter<T, TInput>)new ToNullableTypeConverter<TValue, TInput>(simpleTypeConverter);

                        converter = new TypeConverterPropertyInitializerFactory<T, TInput>(propertyConverter);
                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }


        class EnumerableCandidate<TElement> :
            EnumerableCandidate
        {
            public bool TryGetEnumerableFactory<T, TInputElement>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(TElement) == typeof(TInputElement))
                {
                    var convert = typeof(T).IsArray
                        ? (object)new ArrayPropertyConverter<TInputElement>()
                        : new ListPropertyConverter<TInputElement>();

                    var propertyConverter = (IPropertyConverter<T, IEnumerable<TInputElement>>)convert;

                    converter = new PropertyInitializerFactory<T, IEnumerable<TInputElement>>(propertyConverter);
                    return true;
                }

                if (TryGetFactory(typeof(TInputElement), out IPropertyInitializerFactory<TElement> elementConverter))
                {
                    if (elementConverter.IsMessagePropertyConverter(out IPropertyConverter<TElement, TInputElement> messagePropertyConverter))
                    {
                        var convert = typeof(T).IsArray
                            ? (object)new ArrayPropertyConverter<TElement, TInputElement>(messagePropertyConverter)
                            : new ListPropertyConverter<TElement, TInputElement>(messagePropertyConverter);

                        var propertyConverter = (IPropertyConverter<T, IEnumerable<TInputElement>>)convert;

                        converter = new PropertyInitializerFactory<T, IEnumerable<TInputElement>>(propertyConverter);
                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }


        class UnsupportedDictionaryCandidate :
            DictionaryCandidate
        {
            public bool TryGetDictionaryFactory<T, TInputKey, TInputValue>(out IPropertyInitializerFactory<T> converter)
            {
                converter = default;
                return false;
            }
        }


        class DictionaryCandidate<TKey, TValue> :
            DictionaryCandidate
        {
            public bool TryGetDictionaryFactory<T, TInputKey, TInputValue>(out IPropertyInitializerFactory<T> converter)
            {
                object convert = null;

                if (typeof(TValue) == typeof(TInputValue))
                {
                    if (typeof(TInputKey) == typeof(TKey))
                    {
                        convert = new DictionaryPropertyConverter<TKey, TValue>();
                    }
                    else if (TryGetFactory(typeof(TInputKey), out IPropertyInitializerFactory<TKey> keyConverter))
                    {
                        if (keyConverter.IsMessagePropertyConverter(out IPropertyConverter<TKey, TInputKey> keyPropertyConverter))
                        {
                            convert = new DictionaryKeyPropertyConverter<TKey, TInputKey, TValue>(keyPropertyConverter);
                        }
                    }
                }
                else if (TryGetFactory(typeof(TInputValue), out IPropertyInitializerFactory<TValue> valueConverter))
                {
                    if (valueConverter.IsMessagePropertyConverter(out IPropertyConverter<TValue, TInputValue> valuePropertyConverter))
                    {
                        if (typeof(TInputKey) == typeof(TKey))
                        {
                            convert = new DictionaryPropertyConverter<TKey, TValue, TInputValue>(valuePropertyConverter);
                        }
                        else if (TryGetFactory(typeof(TInputKey), out IPropertyInitializerFactory<TKey> keyConverter))
                        {
                            if (keyConverter.IsMessagePropertyConverter(out IPropertyConverter<TKey, TInputKey> keyPropertyConverter))
                            {
                                convert = new DictionaryPropertyConverter<TKey, TValue, TInputKey, TInputValue>(keyPropertyConverter,
                                    valuePropertyConverter);
                            }
                        }
                    }
                }

                if (convert != null)
                {
                    var propertyConverter = (IPropertyConverter<T, IEnumerable<KeyValuePair<TInputKey, TInputValue>>>)convert;

                    converter = new PropertyInitializerFactory<T, IEnumerable<KeyValuePair<TInputKey, TInputValue>>>(propertyConverter);
                    return true;
                }

                converter = default;
                return false;
            }
        }


        class GeneralSourceCandidate<TSource> :
            SourceCandidate
            where TSource : class
        {
            public bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(T) != typeof(TSource))
                {
                    if (TypeConverterCache.TryGetTypeConverter<T, TSource>(out var simpleTypeConverter))
                    {
                        converter = new TypeConverterPropertyInitializerFactory<T, TSource>(simpleTypeConverter);
                        return true;
                    }

                    if (typeof(T).IsInterface && !typeof(TSource).IsValueTypeOrObject())
                    {
                        var converterType = typeof(InitializePropertyConverter<,>).MakeGenericType(typeof(T), typeof(TSource));

                        var propertyConverter = (IPropertyConverter<T, TSource>)Activator.CreateInstance(converterType);

                        converter = new PropertyInitializerFactory<T, TSource>(propertyConverter);

                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }


        class DictionaryObjectSourceCandidate<TSource> :
            DictionaryCandidate
            where TSource : class
        {
            public bool TryGetDictionaryFactory<T, TInputKey, TInputValue>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(T) == typeof(TSource) && typeof(T).IsInterface)
                {
                    var converterType = typeof(InitializePropertyConverter<,>).MakeGenericType(typeof(T), typeof(IDictionary<TInputKey, TInputValue>));

                    var propertyConverter = (IPropertyConverter<T, IDictionary<TInputKey, TInputValue>>)Activator.CreateInstance(converterType);

                    converter = new PropertyInitializerFactory<T, IDictionary<TInputKey, TInputValue>>(propertyConverter);

                    return true;
                }

                converter = default;
                return false;
            }
        }


        class VariableSourceCandidate<TVariable, TValue> :
            SourceCandidate
            where TVariable : class, IInitializerVariable<TValue>
        {
            public bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(T) == typeof(TValue))
                {
                    var propertyConverter = (IPropertyConverter<T, TVariable>)new ConvertVariablePropertyConverter<TValue, TVariable>();
                    converter = new PropertyInitializerFactory<T, TVariable>(propertyConverter);
                    return true;
                }

                if (PropertyInitializerCache.TryGetFactory(typeof(TValue), out IPropertyInitializerFactory<T> inputConverter))
                {
                    if (inputConverter.IsPropertyTypeConverter(out ITypeConverter<T, TValue> propertyTypeConverter))
                    {
                        var propertyConverter = new ConvertTypeVariablePropertyConverter<T, TVariable, TValue>(propertyTypeConverter);

                        converter = new PropertyInitializerFactory<T, TVariable>(propertyConverter);
                        return true;
                    }

                    if (inputConverter.IsMessagePropertyConverter(out IPropertyConverter<T, TValue> messagePropertyConverter))
                    {
                        var propertyConverter = new ConvertVariablePropertyConverter<T, TVariable, TValue>(messagePropertyConverter);

                        converter = new PropertyInitializerFactory<T, TVariable>(propertyConverter);
                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }


        class EnumerableSourceCandidate<TElement> :
            SourceCandidate
        {
            public bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(T).IsArray)
                {
                    var elementType = typeof(T).GetElementType();

                    if (TryGetEnumerableFactory<T, TElement>(elementType, out converter))
                    {
                        return true;
                    }
                }

                if (typeof(T).ClosesType(typeof(IEnumerable<>), out Type[] types))
                {
                    if (TryGetEnumerableFactory<T, TElement>(types[0], out converter))
                    {
                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }


        class DictionarySourceCandidate<TKey, TValue> :
            SourceCandidate
        {
            public bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter)
            {
                if (TryGetDictionaryFactory<T, TKey, TValue>(typeof(T), out converter))
                {
                    return true;
                }

                converter = default;
                return false;
            }
        }


        class ValueSourceCandidate<TValue> :
            SourceCandidate
            where TValue : struct
        {
            public bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(T) != typeof(TValue))
                {
                    if (TypeConverterCache.TryGetTypeConverter<T, TValue>(out var simpleTypeConverter))
                    {
                        converter = new TypeConverterPropertyInitializerFactory<T, TValue>(simpleTypeConverter);
                        return true;
                    }

                    if (typeof(T).IsNullable(out var underlyingType))
                    {
                        if (underlyingType == typeof(TValue))
                        {
                            var propertyConverter = (ITypeConverter<T, TValue>)new ToNullableTypeConverter<TValue>();
                            converter = new TypeConverterPropertyInitializerFactory<T, TValue>(propertyConverter);
                            return true;
                        }

                        if (TryGetNullableFactory<T, TValue>(underlyingType, out converter))
                        {
                            return true;
                        }
                    }
                }

                converter = default;
                return false;
            }
        }


        class NullableSourceCandidate<TValue> :
            SourceCandidate
            where TValue : struct
        {
            public bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(T) == typeof(TValue))
                {
                    var propertyConverter = (ITypeConverter<T, TValue?>)new FromNullableTypeConverter<TValue>();
                    converter = new TypeConverterPropertyInitializerFactory<T, TValue?>(propertyConverter);
                    return true;
                }

                if (PropertyInitializerCache.TryGetFactory(typeof(TValue), out IPropertyInitializerFactory<T> inputConverter))
                {
                    if (inputConverter.IsPropertyTypeConverter(out ITypeConverter<T, TValue> propertyTypeConverter))
                    {
                        converter = new TypeConverterPropertyInitializerFactory<T, TValue?>(new FromNullableTypeConverter<T, TValue>(propertyTypeConverter));
                        return true;
                    }

                    if (inputConverter.IsMessagePropertyConverter(out IPropertyConverter<T, TValue> messagePropertyConverter))
                    {
                        converter = new PropertyInitializerFactory<T, TValue?>(new FromNullablePropertyConverter<T, TValue>(messagePropertyConverter));
                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }


        class TaskSourceCandidate<TTask> :
            SourceCandidate
        {
            public bool TryGetFactory<T>(out IPropertyInitializerFactory<T> converter)
            {
                if (typeof(T) == typeof(TTask))
                {
                    converter = new PropertyInitializerFactory<T, Task<T>>(new TaskPropertyConverter<T>());
                    return true;
                }

                if (PropertyInitializerCache.TryGetFactory(typeof(TTask), out IPropertyInitializerFactory<T> inputConverter))
                {
                    if (inputConverter.IsPropertyTypeConverter<TTask>(out var propertyTypeConverter))
                    {
                        converter = new PropertyInitializerFactory<T, Task<TTask>>(new AsyncConvertPropertyConverter<T, TTask>(propertyTypeConverter));
                        return true;
                    }

                    if (inputConverter.IsMessagePropertyConverter<TTask>(out var messagePropertyConverter))
                    {
                        converter = new PropertyInitializerFactory<T, Task<TTask>>(new AsyncPropertyConverter<T, TTask>(messagePropertyConverter));
                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }
    }
}

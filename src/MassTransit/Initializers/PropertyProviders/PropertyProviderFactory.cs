namespace MassTransit.Initializers.PropertyProviders
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Internals;
    using Metadata;
    using PropertyConverters;
    using TypeConverters;


    /// <summary>
    /// For an input type, builds the property providers for the requested result types
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public class PropertyProviderFactory<TInput> :
        IPropertyProviderFactory<TInput>
        where TInput : class
    {
        /// <summary>
        /// Return the factory to create a property provider for the specified type <typeparamref name="TResult" /> using the
        /// <paramref name="propertyInfo" /> as the source.
        /// </summary>
        /// <param name="propertyInfo">The input property</param>
        /// <param name="provider"></param>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public bool TryGetPropertyProvider<TResult>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, TResult> provider)
        {
            return CreateProviderFactory<TResult>(propertyInfo.PropertyType).TryGetProvider(propertyInfo, out provider);
        }

        public bool TryGetPropertyConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
        {
            return CreateProviderFactory<T>(typeof(TProperty)).TryGetConverter(out converter);
        }

        IProviderFactory CreateProviderFactory<T>(Type propertyType)
        {
            var type = typeof(T);

            if (type == propertyType)
                return new Matching<T>();

            if (type.IsTask(out var taskType))
                return (IProviderFactory)Activator.CreateInstance(typeof(TaskResult<>).MakeGenericType(typeof(TInput), taskType), this);

            if (propertyType.IsTask(out taskType))
                return (IProviderFactory)Activator.CreateInstance(typeof(TaskProperty<>).MakeGenericType(typeof(TInput), taskType), this);

            if (type.IsNullable(out var underlyingType))
                return (IProviderFactory)Activator.CreateInstance(typeof(NullableResult<>).MakeGenericType(typeof(TInput), underlyingType), this);

            if (type.ClosesType(typeof(MessageData<>), out Type[] types))
                return (IProviderFactory)Activator.CreateInstance(typeof(MessageDataResult<,>).MakeGenericType(typeof(TInput), propertyType, types[0]), this);

            if (propertyType.IsNullable(out underlyingType))
                return (IProviderFactory)Activator.CreateInstance(typeof(NullableProperty<>).MakeGenericType(typeof(TInput), underlyingType), this);

            if (propertyType.ClosesType(typeof(IInitializerVariable<>), out types))
                return (IProviderFactory)Activator.CreateInstance(typeof(VariableProperty<,>).MakeGenericType(typeof(TInput), propertyType, types[0]), this);

            if (propertyType.ClosesType(typeof(State<>), out types))
                return (IProviderFactory)Activator.CreateInstance(typeof(StateProperty<>).MakeGenericType(typeof(TInput), types[0]), this);

            if (propertyType.IsValueTypeOrObject())
                return (IProviderFactory)Activator.CreateInstance(typeof(Convert<,>).MakeGenericType(typeof(TInput), type, propertyType), this);

            if (propertyType.ClosesType(typeof(IDictionary<,>), out types) || propertyType.ClosesType(typeof(IReadOnlyDictionary<,>), out types))
            {
                return (IProviderFactory)Activator.CreateInstance(typeof(DictionaryProperty<,,>).MakeGenericType(typeof(TInput), propertyType, types[0],
                    types[1]), this);
            }

            if (propertyType.IsArray)
            {
                return (IProviderFactory)Activator.CreateInstance(typeof(ArrayProperty<,>).MakeGenericType(typeof(TInput), propertyType,
                    propertyType.GetElementType()), this);
            }

            if (propertyType.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes))
            {
                if (enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out types))
                {
                    return (IProviderFactory)Activator.CreateInstance(typeof(DictionaryProperty<,,>).MakeGenericType(typeof(TInput), propertyType, types[0],
                        types[1]), this);
                }

                return (IProviderFactory)Activator.CreateInstance(typeof(ArrayProperty<,>).MakeGenericType(typeof(TInput), propertyType, enumerableTypes[0]),
                    this);
            }

            return (IProviderFactory)Activator.CreateInstance(typeof(Convert<,>).MakeGenericType(typeof(TInput), type, propertyType), this);
        }


        interface IProviderFactory
        {
            bool TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider);

            bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter);
        }


        class Matching<TResult> :
            IProviderFactory
        {
            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (typeof(T) == typeof(TResult))
                {
                    provider = new InputPropertyProvider<TInput, T>(propertyInfo);
                    return true;
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                converter = default;
                return false;
            }
        }


        class Convert<TResult, TInputProperty> :
            IProviderFactory
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public Convert(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (TryGetConverter(out IPropertyConverter<T, TInputProperty> propertyConverter)
                    && _factory.TryGetPropertyProvider<TInputProperty>(propertyInfo, out IPropertyProvider<TInput, TInputProperty> inputFactory))
                {
                    provider = new PropertyConverterPropertyProvider<TInput, T, TInputProperty>(propertyConverter, inputFactory);
                    return true;
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(T) == typeof(TResult))
                {
                    if (typeof(TResult) == typeof(object))
                    {
                        converter = new ToObjectPropertyConverter<TProperty>() as IPropertyConverter<T, TProperty>;
                        return converter != default;
                    }

                    if (TypeConverterCache.TryGetTypeConverter<T, TProperty>(out ITypeConverter<T, TProperty> typeConverter))
                    {
                        converter = new TypePropertyConverter<T, TProperty>(typeConverter);
                        return true;
                    }

                    if (typeof(T).IsInterfaceOrConcreteClass() && MessageTypeCache<T>.IsValidMessageType && !typeof(TProperty).IsValueTypeOrObject())
                    {
                        var converterType = typeof(TProperty) == typeof(object)
                            ? typeof(InitializePropertyConverter<>).MakeGenericType(typeof(T))
                            : typeof(InitializePropertyConverter<,>).MakeGenericType(typeof(T), typeof(TProperty));

                        converter = (IPropertyConverter<T, TProperty>)Activator.CreateInstance(converterType);
                        return true;
                    }
                }

                converter = default;
                return false;
            }
        }


        /// <summary>
        /// The property on the input is a Task
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        class TaskProperty<TTask> :
            IProviderFactory
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public TaskProperty(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (typeof(T) == typeof(TTask))
                {
                    provider = new AsyncPropertyProvider<TInput, T>(new InputPropertyProvider<TInput, Task<T>>(propertyInfo));
                    return true;
                }

                if (_factory.TryGetPropertyConverter(out IPropertyConverter<T, TTask> converter))
                {
                    var inputValuePropertyProvider = new InputPropertyProvider<TInput, Task<TTask>>(propertyInfo);

                    provider = new AsyncPropertyProvider<TInput, T, TTask>(inputValuePropertyProvider, converter);
                    return true;
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(T) == typeof(TTask))
                {
                    converter = new TaskPropertyConverter<T>() as IPropertyConverter<T, TProperty>;
                    return converter != default;
                }

                if (_factory.TryGetPropertyConverter<T, TTask>(out IPropertyConverter<T, TTask> taskConverter))
                {
                    converter = new TaskPropertyConverter<T, TTask>(taskConverter) as IPropertyConverter<T, TProperty>;
                    return converter != default;
                }

                converter = default;
                return false;
            }
        }


        class TaskResult<TTask> :
            IProviderFactory
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public TaskResult(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (typeof(T).IsTask(out var taskType) && taskType == typeof(TTask))
                {
                    if (_factory.TryGetPropertyProvider<TTask>(propertyInfo, out IPropertyProvider<TInput, TTask> providerFactory))
                    {
                        provider = new TaskPropertyProvider<TInput, TTask>(providerFactory) as IPropertyProvider<TInput, T>;
                        return provider != null;
                    }
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(T) == typeof(TTask))
                {
                    converter = new TaskPropertyConverter<T>() as IPropertyConverter<T, TProperty>;
                    return converter != default;
                }

                if (_factory.TryGetPropertyConverter<T, TTask>(out IPropertyConverter<T, TTask> taskConverter))
                {
                    converter = new TaskPropertyConverter<T, TTask>(taskConverter) as IPropertyConverter<T, TProperty>;
                    return converter != default;
                }

                converter = default;
                return false;
            }
        }


        class NullableResult<TValue> :
            IProviderFactory
            where TValue : struct
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public NullableResult(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (typeof(T).IsNullable(out var underlyingType) && underlyingType == typeof(TValue))
                {
                    if (_factory.TryGetPropertyProvider<TValue>(propertyInfo, out IPropertyProvider<TInput, TValue> providerFactory))
                    {
                        provider = new ToNullablePropertyProvider<TInput, TValue>(providerFactory) as IPropertyProvider<TInput, T>;
                        return provider != null;
                    }
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(T).IsNullable(out var underlyingType) && underlyingType == typeof(TValue))
                {
                    if (typeof(TProperty) == typeof(TValue))
                    {
                        converter = new ToNullablePropertyConverter<TValue>() as IPropertyConverter<T, TProperty>;
                        return converter != default;
                    }

                    if (TypeConverterCache.TryGetTypeConverter<T, TProperty>(out ITypeConverter<T, TProperty> typeConverter))
                    {
                        converter = new TypePropertyConverter<T, TProperty>(typeConverter);
                        return true;
                    }

                    if (_factory.TryGetPropertyConverter(out IPropertyConverter<TValue, TProperty> propertyConverter))
                    {
                        converter = new ToNullablePropertyConverter<TValue, TProperty>(propertyConverter) as IPropertyConverter<T, TProperty>;
                        return converter != default;
                    }
                }

                converter = default;
                return false;
            }
        }


        class MessageDataResult<TSource, TValue> :
            IProviderFactory
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public MessageDataResult(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (TryGetConverter(out IPropertyConverter<T, TSource> propertyConverter))
                {
                    var inputValuePropertyProvider = new InputPropertyProvider<TInput, TSource>(propertyInfo);

                    provider = new PropertyConverterPropertyProvider<TInput, T, TSource>(propertyConverter, inputValuePropertyProvider);
                    return true;
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(T).ClosesType(typeof(MessageData<>), out Type[] types))
                {
                    converter = MessageDataPropertyConverter.Instance as IPropertyConverter<T, TProperty>;
                    if (converter != null)
                        return true;

                    if (TypeMetadataCache.IsValidMessageDataType(types[0]))
                    {
                        if (typeof(TProperty) == types[0] || typeof(TProperty) == typeof(MessageData<>).MakeGenericType(types[0]))
                        {
                            var converterType = typeof(MessageDataPropertyConverter<>).MakeGenericType(types[0]);

                            converter = Activator.CreateInstance(converterType) as IPropertyConverter<T, TProperty>;
                            return converter != null;
                        }
                    }
                }

                converter = default;
                return false;
            }
        }


        class NullableProperty<TValue> :
            IProviderFactory
            where TValue : struct
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public NullableProperty(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (typeof(T) == typeof(TValue))
                {
                    var propertyProvider = new InputPropertyProvider<TInput, TValue?>(propertyInfo);

                    provider = new FromNullablePropertyProvider<TInput, TValue>(propertyProvider) as IPropertyProvider<TInput, T>;
                    return provider != null;
                }

                if (TryGetConverter(out IPropertyConverter<T, TValue?> converter))
                {
                    var inputProvider = new InputPropertyProvider<TInput, TValue?>(propertyInfo);

                    provider = new PropertyConverterPropertyProvider<TInput, T, TValue?>(converter, inputProvider);
                    return true;
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(TProperty).IsNullable(out var underlyingType) && underlyingType == typeof(TValue))
                {
                    if (typeof(T) == typeof(TValue))
                    {
                        converter = new FromNullablePropertyConverter<TValue>() as IPropertyConverter<T, TProperty>;
                        return converter != default;
                    }

                    if (TypeConverterCache.TryGetTypeConverter<T, TValue>(out ITypeConverter<T, TValue> typeConverter))
                    {
                        var typePropertyConverter = new TypePropertyConverter<T, TValue>(typeConverter);
                        converter = new FromNullablePropertyConverter<T, TValue>(typePropertyConverter) as IPropertyConverter<T, TProperty>;
                        return true;
                    }

                    if (_factory.TryGetPropertyConverter(out IPropertyConverter<TValue, TProperty> propertyConverter))
                    {
                        converter = new ToNullablePropertyConverter<TValue, TProperty>(propertyConverter) as IPropertyConverter<T, TProperty>;
                        return converter != default;
                    }
                }


                converter = default;
                return false;
            }
        }


        class VariableProperty<TVariable, TValue> :
            IProviderFactory
            where TVariable : class, IInitializerVariable<TValue>
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public VariableProperty(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (TryGetConverter(out IPropertyConverter<T, TVariable> propertyConverter))
                {
                    var inputValuePropertyProvider = new InputPropertyProvider<TInput, TVariable>(propertyInfo);

                    provider = new PropertyConverterPropertyProvider<TInput, T, TVariable>(propertyConverter, inputValuePropertyProvider);
                    return true;
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(T) == typeof(TValue))
                {
                    converter = new VariablePropertyConverter<TValue, TVariable>() as IPropertyConverter<T, TProperty>;
                    return converter != null;
                }

                if (_factory.TryGetPropertyConverter<T, TValue>(out IPropertyConverter<T, TValue> elementConverter))
                {
                    converter = new VariablePropertyConverter<T, TVariable, TValue>(elementConverter) as IPropertyConverter<T, TProperty>;
                    return converter != null;
                }

                converter = default;
                return false;
            }
        }


        class StateProperty<TInstance> :
            IProviderFactory
            where TInstance : class, SagaStateMachineInstance
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public StateProperty(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            bool IProviderFactory.TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (TryGetConverter(out IPropertyConverter<T, TInstance> propertyConverter))
                {
                    var inputValuePropertyProvider = new InputPropertyProvider<TInput, TInstance>(propertyInfo);

                    provider = new PropertyConverterPropertyProvider<TInput, T, TInstance>(propertyConverter, inputValuePropertyProvider);
                    return true;
                }

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (typeof(T) == typeof(string))
                {
                    converter = new StatePropertyConverter<TInstance>() as IPropertyConverter<T, TProperty>;
                    return converter != null;
                }

                if (_factory.TryGetPropertyConverter(out IPropertyConverter<T, string> elementConverter))
                {
                    converter = new StatePropertyConverter<T, TInstance>(elementConverter) as IPropertyConverter<T, TProperty>;
                    return converter != null;
                }

                converter = default;
                return false;
            }
        }


        class ArrayProperty<TInputProperty, TInputElement> :
            IProviderFactory
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public ArrayProperty(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            public bool TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (IsSupportedType(typeof(T), out var providerFactory))
                    return providerFactory.TryGetProvider(propertyInfo, out provider);

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (IsSupportedType(typeof(T), out var providerFactory))
                    return providerFactory.TryGetConverter(out converter);

                converter = default;
                return false;
            }

            bool IsSupportedType(Type type, out IProviderFactory providerFactory)
            {
                if (type.IsArray)
                {
                    var factoryType = typeof(ArrayResult<>).MakeGenericType(typeof(TInput), typeof(TInputProperty), typeof(TInputElement),
                        type.GetElementType());

                    providerFactory = (IProviderFactory)Activator.CreateInstance(factoryType, _factory);
                    return true;
                }

                if (type.ClosesType(typeof(IEnumerable<>), out Type[] types))
                {
                    var factoryType = typeof(ListResult<>).MakeGenericType(typeof(TInput), typeof(TInputProperty), typeof(TInputElement), types[0]);

                    providerFactory = (IProviderFactory)Activator.CreateInstance(factoryType, _factory);
                    return true;
                }

                providerFactory = default;
                return false;
            }


            class ArrayResult<TElement> :
                IProviderFactory
            {
                readonly IPropertyProviderFactory<TInput> _factory;

                public ArrayResult(IPropertyProviderFactory<TInput> factory)
                {
                    _factory = factory;
                }

                public bool TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
                {
                    if (TryGetConverter(out IPropertyConverter<T, TInputProperty> propertyConverter))
                    {
                        var inputProvider = new InputPropertyProvider<TInput, TInputProperty>(propertyInfo);

                        provider = new PropertyConverterPropertyProvider<TInput, T, TInputProperty>(propertyConverter, inputProvider);
                        return provider != null;
                    }

                    provider = default;
                    return false;
                }

                public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
                {
                    if (typeof(TInputElement) == typeof(TElement))
                    {
                        converter = new ArrayPropertyConverter<TInputElement>() as IPropertyConverter<T, TProperty>;
                        return converter != null;
                    }

                    if (_factory.TryGetPropertyConverter<TElement, TInputElement>(out IPropertyConverter<TElement, TInputElement> elementConverter))
                    {
                        converter = new ArrayPropertyConverter<TElement, TInputElement>(elementConverter) as IPropertyConverter<T, TProperty>;
                        return converter != null;
                    }

                    converter = default;
                    return false;
                }
            }


            class ListResult<TElement> :
                IProviderFactory
            {
                readonly IPropertyProviderFactory<TInput> _factory;

                public ListResult(IPropertyProviderFactory<TInput> factory)
                {
                    _factory = factory;
                }

                public bool TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
                {
                    if (TryGetConverter(out IPropertyConverter<T, TInputProperty> propertyConverter))
                    {
                        var inputProvider = new InputPropertyProvider<TInput, TInputProperty>(propertyInfo);

                        provider = new PropertyConverterPropertyProvider<TInput, T, TInputProperty>(propertyConverter, inputProvider);
                        return provider != null;
                    }

                    provider = default;
                    return false;
                }

                public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
                {
                    if (typeof(TInputElement) == typeof(TElement))
                    {
                        converter = new ListPropertyConverter<TInputElement>() as IPropertyConverter<T, TProperty>;
                        return converter != null;
                    }

                    if (_factory.TryGetPropertyConverter<TElement, TInputElement>(out IPropertyConverter<TElement, TInputElement> elementConverter))
                    {
                        converter = new ListPropertyConverter<TElement, TInputElement>(elementConverter) as IPropertyConverter<T, TProperty>;
                        return converter != null;
                    }

                    converter = default;
                    return false;
                }
            }
        }


        class DictionaryProperty<TInputProperty, TInputKey, TInputValue> :
            IProviderFactory
        {
            readonly IPropertyProviderFactory<TInput> _factory;

            public DictionaryProperty(IPropertyProviderFactory<TInput> factory)
            {
                _factory = factory;
            }

            public bool TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
            {
                if (IsSupportedType(typeof(T), out var providerFactory))
                    return providerFactory.TryGetProvider(propertyInfo, out provider);

                provider = default;
                return false;
            }

            public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
            {
                if (IsSupportedType(typeof(T), out var providerFactory))
                    return providerFactory.TryGetConverter(out converter);

                converter = default;
                return false;
            }

            bool IsSupportedType(Type type, out IProviderFactory providerFactory)
            {
                if (type.ClosesType(typeof(IDictionary<,>), out Type[] types)
                    || type.ClosesType(typeof(IReadOnlyDictionary<,>), out types)
                    || type.ClosesType(typeof(IEnumerable<>), out Type[] enumerableTypes) && enumerableTypes[0].ClosesType(typeof(KeyValuePair<,>), out types))
                {
                    var factoryType = typeof(DictionaryResult<,>).MakeGenericType(typeof(TInput), typeof(TInputProperty), typeof(TInputKey),
                        typeof(TInputValue), types[0], types[1]);

                    providerFactory = (IProviderFactory)Activator.CreateInstance(factoryType, _factory);
                    return true;
                }

                if (type.IsInterfaceOrConcreteClass() && MessageTypeCache.IsValidMessageType(type) && !type.IsValueTypeOrObject())
                {
                    var factoryType = typeof(InitializerResult<>).MakeGenericType(typeof(TInput), typeof(TInputProperty), typeof(TInputKey),
                        typeof(TInputValue), type);

                    providerFactory = (IProviderFactory)Activator.CreateInstance(factoryType);
                    return true;
                }

                providerFactory = default;
                return false;
            }


            class DictionaryResult<TKey, TValue> :
                IProviderFactory
            {
                readonly IPropertyProviderFactory<TInput> _factory;

                public DictionaryResult(IPropertyProviderFactory<TInput> factory)
                {
                    _factory = factory;
                }

                public bool TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
                {
                    if (TryGetConverter(out IPropertyConverter<T, TInputProperty> propertyConverter))
                    {
                        var inputProvider = new InputPropertyProvider<TInput, TInputProperty>(propertyInfo);

                        provider = new PropertyConverterPropertyProvider<TInput, T, TInputProperty>(propertyConverter, inputProvider);
                        return provider != null;
                    }

                    provider = default;
                    return false;
                }

                public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
                {
                    if (typeof(TKey) == typeof(TInputKey))
                    {
                        if (typeof(TValue) == typeof(TInputValue))
                        {
                            converter = new DictionaryPropertyConverter<TKey, TValue>() as IPropertyConverter<T, TProperty>;
                            return converter != default;
                        }

                        if (_factory.TryGetPropertyConverter(out IPropertyConverter<TValue, TInputValue> propertyConverter))
                        {
                            converter = new DictionaryPropertyConverter<TKey, TValue, TInputValue>(propertyConverter) as IPropertyConverter<T, TProperty>;
                            return converter != default;
                        }
                    }
                    else if (_factory.TryGetPropertyConverter(out IPropertyConverter<TKey, TInputKey> keyConverter))
                    {
                        if (typeof(TValue) == typeof(TInputValue))
                        {
                            converter = new DictionaryKeyPropertyConverter<TKey, TInputKey, TValue>(keyConverter) as IPropertyConverter<T, TProperty>;
                            return converter != default;
                        }

                        if (_factory.TryGetPropertyConverter(out IPropertyConverter<TValue, TInputValue> propertyConverter))
                        {
                            converter = new DictionaryPropertyConverter<TKey, TValue, TInputKey, TInputValue>(keyConverter, propertyConverter)
                                as IPropertyConverter<T, TProperty>;

                            return converter != default;
                        }
                    }

                    converter = default;
                    return false;
                }
            }


            class InitializerResult<TObject> :
                IProviderFactory
            {
                public bool TryGetProvider<T>(PropertyInfo propertyInfo, out IPropertyProvider<TInput, T> provider)
                {
                    if (TryGetConverter(out IPropertyConverter<T, TInputProperty> propertyConverter))
                    {
                        var inputProvider = new InputPropertyProvider<TInput, TInputProperty>(propertyInfo);

                        provider = new PropertyConverterPropertyProvider<TInput, T, TInputProperty>(propertyConverter, inputProvider);
                        return provider != null;
                    }

                    provider = default;
                    return false;
                }

                public bool TryGetConverter<T, TProperty>(out IPropertyConverter<T, TProperty> converter)
                {
                    if (typeof(T) == typeof(TObject) && typeof(T).IsInterface && MessageTypeCache<T>.IsValidMessageType)
                    {
                        var converterType = typeof(InitializePropertyConverter<,>).MakeGenericType(typeof(T), typeof(TProperty));

                        converter = (IPropertyConverter<T, TProperty>)Activator.CreateInstance(converterType);
                        return true;
                    }

                    converter = default;
                    return false;
                }
            }
        }
    }
}

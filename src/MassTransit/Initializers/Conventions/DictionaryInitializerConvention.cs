namespace MassTransit.Initializers.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using HeaderInitializers;
    using Internals;
    using PropertyInitializers;
    using PropertyProviders;


    public class DictionaryInitializerConvention<TMessage, TInput, TValue> :
        IInitializerConvention<TMessage, TInput>
        where TMessage : class
        where TInput : class, IDictionary<string, TValue>
    {
        readonly IPropertyProviderFactory<TInput> _providerFactory;

        public DictionaryInitializerConvention()
        {
            _providerFactory = new PropertyProviderFactory<TInput>();
        }

        public bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
        {
            var key = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            if (typeof(TValue) == typeof(TProperty))
            {
                initializer = new DictionaryCopyPropertyInitializer<TMessage, TInput, TValue>(propertyInfo, key);
                return true;
            }

            if (_providerFactory.TryGetPropertyConverter(out IPropertyConverter<TProperty, TValue> converter))
            {
                var providerType = typeof(InputDictionaryPropertyProvider<,>).MakeGenericType(typeof(TInput), typeof(TValue));

                var provider = (IPropertyProvider<TInput, TValue>)Activator.CreateInstance(providerType, key);

                var convertProvider = new PropertyConverterPropertyProvider<TInput, TProperty, TValue>(converter, provider);

                initializer = new ProviderPropertyInitializer<TMessage, TInput, TProperty>(convertProvider, propertyInfo);
                return true;
            }

            if (typeof(TValue) == typeof(object))
            {
                var inputProviderType = typeof(InputDictionaryPropertyProvider<,>).MakeGenericType(typeof(TInput), typeof(TValue));

                var valueProvider = (IPropertyProvider<TInput, TValue>)Activator.CreateInstance(inputProviderType, key);

                var providerType = typeof(ObjectPropertyProvider<,>).MakeGenericType(typeof(TInput), typeof(TProperty));

                var provider = (IPropertyProvider<TInput, TProperty>)Activator.CreateInstance(providerType, _providerFactory, valueProvider);

                initializer = new ProviderPropertyInitializer<TMessage, TInput, TProperty>(provider, propertyInfo);
                return true;
            }

            initializer = null;
            return false;
        }

        public bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
        {
            var propertyName = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            // headers use a double underscore prefix
            var key = new StringBuilder(propertyName.Length + 2).Append("__").Append(propertyName).ToString();

            if (typeof(TValue) == typeof(TProperty))
            {
                initializer = new DictionaryCopyHeaderInitializer<TMessage, TInput, TValue>(propertyInfo, key);
                return true;
            }

            if (_providerFactory.TryGetPropertyConverter(out IPropertyConverter<TProperty, TValue> converter))
            {
                var providerType = typeof(InputDictionaryPropertyProvider<,>).MakeGenericType(typeof(TInput), typeof(TValue));

                var provider = (IPropertyProvider<TInput, TValue>)Activator.CreateInstance(providerType, propertyName);

                var convertProvider = new PropertyConverterPropertyProvider<TInput, TProperty, TValue>(converter, provider);

                initializer = new ProviderHeaderInitializer<TMessage, TInput, TProperty>(convertProvider, propertyInfo);
                return true;
            }

            initializer = default;
            return false;
        }

        public bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
        {
            initializer = default;
            return false;
        }
    }


    public class DictionaryInitializerConvention<TMessage> :
        InitializerConvention<TMessage>
        where TMessage : class
    {
        public DictionaryInitializerConvention(IInitializerConvention convention)
            : base(new CacheFactory(), convention)
        {
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageInputInitializerConvention<TMessage>>
        {
            IMessageInputInitializerConvention<TMessage> IConventionTypeCacheFactory<IMessageInputInitializerConvention<TMessage>>.Create<T>(
                IInitializerConvention convention)
            {
                if (typeof(T).ClosesType(typeof(IDictionary<,>), out Type[] argumentTypes) && argumentTypes[0] == typeof(string))
                {
                    var conventionType = typeof(DictionaryInitializerConvention<,,>).MakeGenericType(typeof(TMessage), typeof(T), argumentTypes[1]);

                    return (IMessageInputInitializerConvention<TMessage>)Activator.CreateInstance(conventionType);
                }

                return new Unsupported<T>();
            }
        }
    }


    public class DictionaryInitializerConvention :
        InitializerConvention
    {
        public DictionaryInitializerConvention()
            : base(new CacheFactory())
        {
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageInitializerConvention>
        {
            IMessageInitializerConvention IConventionTypeCacheFactory<IMessageInitializerConvention>.Create<T>(IInitializerConvention convention)
            {
                return new DictionaryInitializerConvention<T>(convention);
            }
        }
    }
}

namespace MassTransit.Initializers.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;
    using HeaderInitializers;
    using Internals.Extensions;
    using PropertyInitializers;


    public class DictionaryInitializerConvention<TMessage, TInput, TValue> :
        IInitializerConvention<TMessage, TInput>
        where TMessage : class
        where TInput : class, IDictionary<string, TValue>
    {
        public bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
        {
            var propertyName = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            if (typeof(TValue) == typeof(TProperty))
            {
                initializer = new DictionaryCopyPropertyInitializer<TMessage, TInput, TValue>(propertyInfo, propertyName);
                return true;
            }

            if (PropertyInitializerCache.TryGetFactory<TProperty>(typeof(TValue), out var propertyConverter))
            {
                var providerFactory = new DictionaryInputValuePropertyProviderFactory<TInput>(propertyName);

                initializer = propertyConverter.CreatePropertyInitializer<TMessage, TInput>(propertyInfo, providerFactory);
                return true;
            }

            initializer = null;
            return false;
        }

        public bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
        {
            var propertyName = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            // headers use a double underscore prefix
            string inputPropertyName = new StringBuilder(propertyName.Length + 2).Append("__").Append(propertyName).ToString();

            if (typeof(TValue) == typeof(TProperty))
            {
                initializer = new DictionaryCopyHeaderInitializer<TMessage, TInput, TValue>(propertyInfo, inputPropertyName);
                return true;
            }

            if (PropertyInitializerCache.TryGetFactory<TProperty>(typeof(TValue), out var propertyConverter))
            {
                var providerFactory = new DictionaryInputValuePropertyProviderFactory<TInput>(inputPropertyName);

                initializer = propertyConverter.CreateHeaderInitializer<TMessage, TInput>(propertyInfo, providerFactory);
                return true;
            }

            initializer = default;
            return false;
        }

        public bool TryGetHeaderInitializer(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
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

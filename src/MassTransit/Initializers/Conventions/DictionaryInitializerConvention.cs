namespace MassTransit.Initializers.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using HeaderInitializers;
    using Internals.Extensions;
    using PropertyInitializers;


    public class DictionaryInitializerConvention<TMessage, TInput, TValue> :
        IInitializerConvention<TMessage, TInput>
        where TMessage : class
        where TInput : class, IDictionary<string, TValue>
    {
        public bool TryGetPropertyInitializer<TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
        {
            if (typeof(TValue) == typeof(TProperty))
            {
                initializer = new DictionaryCopyPropertyInitializer<TMessage, TInput, TValue>(propertyName);
                return true;
            }

            if (PropertyInitializerCache.TryGetFactory<TProperty>(typeof(TValue), out var propertyConverter))
            {
                var providerFactory = new DictionaryInputValuePropertyProviderFactory<TInput>(propertyName);

                initializer = propertyConverter.CreatePropertyInitializer<TMessage, TInput>(propertyName, providerFactory);
                return true;
            }

            initializer = null;
            return false;
        }

        public bool TryGetHeaderInitializer<TProperty>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
        {
            string inputPropertyName = new StringBuilder(propertyName.Length + 2).Append("__").Append(propertyName).ToString();

            if (typeof(TValue) == typeof(TProperty))
            {
                initializer = new DictionaryCopyHeaderInitializer<TMessage, TInput, TValue>(propertyName, inputPropertyName);
                return true;
            }

            if (PropertyInitializerCache.TryGetFactory<TProperty>(typeof(TValue), out var propertyConverter))
            {
                var providerFactory = new DictionaryInputValuePropertyProviderFactory<TInput>(inputPropertyName);

                initializer = propertyConverter.CreateHeaderInitializer<TMessage, TInput>(propertyName, providerFactory);
                return true;
            }

            initializer = default;
            return false;
        }

        public bool TryGetHeaderInitializer(string inputPropertyName, out IHeaderInitializer<TMessage, TInput> initializer)
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

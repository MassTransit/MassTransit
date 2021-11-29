namespace MassTransit.Initializers.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using HeaderInitializers;
    using PropertyInitializers;
    using PropertyProviders;


    public class DefaultInitializerConvention<TMessage, TInput> :
        IInitializerConvention<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        readonly IReadOnlyDictionary<string, PropertyInfo> _inputProperties;
        readonly IPropertyProviderFactory<TInput> _providerFactory;

        public DefaultInitializerConvention()
        {
            _inputProperties = MessageTypeCache<TInput>.Properties.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            _providerFactory = new PropertyProviderFactory<TInput>();
        }

        public bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
        {
            var propertyName = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            if (_inputProperties.TryGetValue(propertyName, out var inputPropertyInfo))
            {
                var propertyType = typeof(TProperty);
                var inputPropertyType = inputPropertyInfo.PropertyType;

                // exactly the same type, we just copy it over unmodified
                if (inputPropertyType == propertyType)
                {
                    initializer = new CopyPropertyInitializer<TMessage, TInput, TProperty>(propertyInfo, inputPropertyInfo);
                    return true;
                }

                // can only copy to object, no idea what the destination type would/could be
                if (propertyType == typeof(object))
                {
                    var type = typeof(CopyObjectPropertyInitializer<,,>).MakeGenericType(typeof(TMessage), typeof(TInput), inputPropertyType);
                    initializer = (IPropertyInitializer<TMessage, TInput>)Activator.CreateInstance(type, propertyInfo, inputPropertyInfo);
                    return true;
                }

                if (_providerFactory.TryGetPropertyProvider(inputPropertyInfo, out IPropertyProvider<TInput, TProperty> provider))
                {
                    initializer = new ProviderPropertyInitializer<TMessage, TInput, TProperty>(provider, propertyInfo);
                    return true;
                }
            }

            initializer = null;
            return false;
        }

        public bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
        {
            var propertyName = propertyInfo?.Name ?? throw new ArgumentNullException(nameof(propertyInfo));

            // headers use a double underscore prefix
            var inputPropertyName = new StringBuilder(propertyName.Length + 2).Append("__").Append(propertyName).ToString();

            if (_inputProperties.TryGetValue(inputPropertyName, out var inputPropertyInfo))
            {
                var propertyType = typeof(TProperty);
                var inputPropertyType = inputPropertyInfo.PropertyType;

                // exactly the same type, we just copy it over unmodified
                if (inputPropertyType == propertyType)
                {
                    initializer = new CopyHeaderInitializer<TMessage, TInput, TProperty>(propertyInfo, inputPropertyInfo);
                    return true;
                }

                if (_providerFactory.TryGetPropertyProvider(inputPropertyInfo, out IPropertyProvider<TInput, TProperty> provider))
                {
                    initializer = new ProviderHeaderInitializer<TMessage, TInput, TProperty>(provider, propertyInfo);
                    return true;
                }
            }

            initializer = default;
            return false;
        }

        public bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
        {
            if (propertyInfo.Name.StartsWith("__Header_") && propertyInfo.Name.Length > 9)
            {
                var headerName = propertyInfo.Name.Substring(9).Replace("__", " ").Replace("_", "-").Replace(" ", "_");

                var inputPropertyType = propertyInfo.PropertyType;

                // exactly the same type, we just copy it over unmodified
                if (inputPropertyType == typeof(string))
                {
                    initializer = new SetStringHeaderInitializer<TMessage, TInput>(headerName, propertyInfo);
                    return true;
                }

                if (_providerFactory.TryGetPropertyProvider(propertyInfo, out IPropertyProvider<TInput, TProperty> provider))
                {
                    var type = typeof(SetHeaderInitializer<,,>).MakeGenericType(typeof(TMessage), typeof(TInput), inputPropertyType);
                    initializer = (IHeaderInitializer<TMessage, TInput>)Activator.CreateInstance(type, headerName, provider);
                    return true;
                }
            }

            initializer = default;
            return false;
        }
    }


    public class DefaultInitializerConvention<TMessage> :
        InitializerConvention<TMessage>
        where TMessage : class
    {
        public DefaultInitializerConvention(IInitializerConvention convention)
            : base(new CacheFactory(), convention)
        {
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageInputInitializerConvention<TMessage>>
        {
            IMessageInputInitializerConvention<TMessage> IConventionTypeCacheFactory<IMessageInputInitializerConvention<TMessage>>.Create<T>(
                IInitializerConvention convention)
            {
                return new DefaultInitializerConvention<TMessage, T>();
            }
        }
    }


    public class DefaultInitializerConvention :
        InitializerConvention
    {
        public DefaultInitializerConvention()
            : base(new CacheFactory())
        {
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageInitializerConvention>
        {
            IMessageInitializerConvention IConventionTypeCacheFactory<IMessageInitializerConvention>.Create<T>(IInitializerConvention convention)
            {
                return new DefaultInitializerConvention<T>(convention);
            }
        }
    }
}

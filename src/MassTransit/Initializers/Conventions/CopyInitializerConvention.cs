namespace MassTransit.Initializers.Conventions
{
    using System;
    using PropertyInitializers;


    public class CopyInitializerConvention<TMessage, TInput> :
        IInitializerConvention<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        public bool TryGetMessagePropertyInitializer<TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
        {
            var inputPropertyInfo = typeof(TInput).GetProperty(propertyName);
            if (inputPropertyInfo != null)
            {
                var propertyType = typeof(TProperty);
                var inputPropertyType = inputPropertyInfo.PropertyType;

                // exactly the same type, we just copy it over unmodified
                if (inputPropertyType == propertyType)
                {
                    initializer = new CopyPropertyInitializer<TMessage, TInput, TProperty>(propertyName);
                    return true;
                }

                // can only copy to object, no idea what the destination type would/could be
                if (propertyType == typeof(object))
                {
                    var type = typeof(CopyObjectPropertyInitializer<,,>).MakeGenericType(typeof(TMessage), typeof(TInput), inputPropertyType);
                    initializer = (IPropertyInitializer<TMessage, TInput>)Activator.CreateInstance(type, propertyName, propertyName);
                    return true;
                }

                if (PropertyInitializerCache.TryGetFactory<TProperty>(inputPropertyType, out var propertyConverter))
                {
                    initializer = propertyConverter.CreatePropertyInitializer<TMessage, TInput>(propertyName);
                    return true;
                }
            }

            initializer = null;
            return false;
        }
    }


    public class CopyInitializerConvention<TMessage> :
        InitializerConvention<TMessage>
        where TMessage : class
    {
        public CopyInitializerConvention(IInitializerConvention convention)
            : base(new CacheFactory(), convention)
        {
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageTypeInitializerConvention<TMessage>>
        {
            IMessageTypeInitializerConvention<TMessage> IConventionTypeCacheFactory<IMessageTypeInitializerConvention<TMessage>>.Create<T>(
                IInitializerConvention convention)
            {
                return new CopyInitializerConvention<TMessage, T>();
            }
        }
    }


    public class CopyMessageInitializerConvention :
        InitializerConvention
    {
        public CopyMessageInitializerConvention()
            : base(new CacheFactory())
        {
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageTypeInitializerConvention>
        {
            IMessageTypeInitializerConvention IConventionTypeCacheFactory<IMessageTypeInitializerConvention>.Create<T>(IInitializerConvention convention)
            {
                return new CopyInitializerConvention<T>(convention);
            }
        }
    }
}

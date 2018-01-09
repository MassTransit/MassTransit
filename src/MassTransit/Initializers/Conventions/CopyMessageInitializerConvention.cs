namespace MassTransit.Initializers.Conventions
{
    public class CopyMessageInitializerConvention<TMessage, TInput> :
        IMessageInitializerConvention<TMessage, TInput>
        where TMessage : class
        where TInput : class
    {
        public bool TryGetMessagePropertyInitializer<TProperty>(string propertyName, out IMessagePropertyInitializer<TMessage, TInput> convention)
        {
            var propertyInfo = typeof(TInput).GetProperty(propertyName);
            if (propertyInfo != null && propertyInfo.PropertyType == typeof(TProperty))
            {
                convention = new CopyMessagePropertyInitializer<TMessage, TInput, TProperty>(propertyName);
                return true;
            }

            convention = null;
            return false;
        }
    }


    public class CopyMessageInitializerConvention<TMessage> :
        MessageInitializerConvention<TMessage>
        where TMessage : class
    {
        public CopyMessageInitializerConvention()
            : base(new CacheFactory())
        {
        }


        class CacheFactory :
            IConventionTypeCacheFactory<IMessageTypeInitializerConvention<TMessage>>
        {
            IMessageTypeInitializerConvention<TMessage> IConventionTypeCacheFactory<IMessageTypeInitializerConvention<TMessage>>.Create<T>()
            {
                return new CopyMessageInitializerConvention<TMessage, T>();
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
            IMessageTypeInitializerConvention IConventionTypeCacheFactory<IMessageTypeInitializerConvention>.Create<T>()
            {
                return new CopyMessageInitializerConvention<T>();
            }
        }
    }
}
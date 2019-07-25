namespace MassTransit.Initializers.Conventions
{
    public abstract class InitializerConvention<TMessage> :
        IInitializerConvention<TMessage>
        where TMessage : class
    {
        readonly IConventionTypeCache<IMessageInputInitializerConvention<TMessage>> _typeCache;

        protected InitializerConvention(IConventionTypeCacheFactory<IMessageInputInitializerConvention<TMessage>> cacheFactory, IInitializerConvention
            convention)
        {
            _typeCache = new ConventionTypeCache<IMessageInputInitializerConvention<TMessage>>(cacheFactory, convention);
        }

        public bool TryGetPropertyInitializer<TInput, TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            return _typeCache.GetOrAdd<TInput, IInitializerConvention<TMessage, TInput>>().TryGetPropertyInitializer<TProperty>(propertyName, out initializer);
        }

        public bool TryGetHeaderInitializer<TInput, TProperty>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            return _typeCache.GetOrAdd<TInput, IInitializerConvention<TMessage, TInput>>().TryGetHeaderInitializer<TProperty>(propertyName, out initializer);
        }

        public bool TryGetHeaderInitializer<TInput>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            return _typeCache.GetOrAdd<TInput, IInitializerConvention<TMessage, TInput>>().TryGetHeaderInitializer(propertyName, out initializer);
        }


        protected class Unsupported<TInput> :
            IInitializerConvention<TMessage, TInput>
            where TInput : class
        {
            public bool TryGetPropertyInitializer<TProperty>(string propertyName, out IPropertyInitializer<TMessage, TInput> initializer)
            {
                initializer = default;
                return false;
            }

            public bool TryGetHeaderInitializer<TProperty>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            {
                initializer = default;
                return false;
            }

            public bool TryGetHeaderInitializer(string inputPropertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            {
                initializer = default;
                return false;
            }
        }
    }


    public abstract class InitializerConvention :
        IInitializerConvention
    {
        readonly IConventionTypeCache<IMessageInitializerConvention> _typeCache;

        protected InitializerConvention(IConventionTypeCacheFactory<IMessageInitializerConvention> cacheFactory)
        {
            _typeCache = new ConventionTypeCache<IMessageInitializerConvention>(cacheFactory, this);
        }

        public bool TryGetPropertyInitializer<TMessage, TInput, TProperty>(string propertyName,
            out IPropertyInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class
        {
            return _typeCache.GetOrAdd<TMessage, IInitializerConvention<TMessage>>()
                .TryGetPropertyInitializer<TInput, TProperty>(propertyName, out initializer);
        }

        public bool TryGetHeaderInitializer<TMessage, TInput, TProperty>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class
        {
            return _typeCache.GetOrAdd<TMessage, IInitializerConvention<TMessage>>().TryGetHeaderInitializer<TInput, TProperty>(propertyName, out initializer);
        }

        public bool TryGetHeaderInitializer<TMessage, TInput>(string propertyName, out IHeaderInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class
        {
            return _typeCache.GetOrAdd<TMessage, IInitializerConvention<TMessage>>().TryGetHeaderInitializer(propertyName, out initializer);
        }
    }
}

namespace MassTransit.Initializers.Conventions
{
    using System.Reflection;


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

        public bool TryGetPropertyInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            return _typeCache.GetOrAdd<TInput, IInitializerConvention<TMessage, TInput>>().TryGetPropertyInitializer<TProperty>(propertyInfo, out initializer);
        }

        public bool TryGetHeaderInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            return _typeCache.GetOrAdd<TInput, IInitializerConvention<TMessage, TInput>>().TryGetHeaderInitializer<TProperty>(propertyInfo, out initializer);
        }

        public bool TryGetHeadersInitializer<TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            return _typeCache.GetOrAdd<TInput, IInitializerConvention<TMessage, TInput>>().TryGetHeadersInitializer<TProperty>(propertyInfo, out initializer);
        }


        protected class Unsupported<TInput> :
            IInitializerConvention<TMessage, TInput>
            where TInput : class
        {
            public bool TryGetPropertyInitializer<TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
            {
                initializer = default;
                return false;
            }

            public bool TryGetHeaderInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            {
                initializer = default;
                return false;
            }

            public bool TryGetHeadersInitializer<TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
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

        public bool TryGetPropertyInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IPropertyInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class
        {
            return _typeCache.GetOrAdd<TMessage, IInitializerConvention<TMessage>>()
                .TryGetPropertyInitializer<TInput, TProperty>(propertyInfo, out initializer);
        }

        public bool TryGetHeaderInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class
        {
            return _typeCache.GetOrAdd<TMessage, IInitializerConvention<TMessage>>().TryGetHeaderInitializer<TInput, TProperty>(propertyInfo, out initializer);
        }

        public bool TryGetHeadersInitializer<TMessage, TInput, TProperty>(PropertyInfo propertyInfo, out IHeaderInitializer<TMessage, TInput> initializer)
            where TMessage : class
            where TInput : class
        {
            return _typeCache.GetOrAdd<TMessage, IInitializerConvention<TMessage>>().TryGetHeadersInitializer<TInput, TProperty>(propertyInfo, out initializer);
        }
    }
}

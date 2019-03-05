namespace MassTransit.Initializers.Conventions
{
    public abstract class InitializerConvention<TMessage> :
        IInitializerConvention<TMessage>
        where TMessage : class
    {
        readonly IConventionTypeCache<IMessageTypeInitializerConvention<TMessage>> _typeCache;

        protected InitializerConvention(IConventionTypeCacheFactory<IMessageTypeInitializerConvention<TMessage>> cacheFactory, IInitializerConvention
            convention)
        {
            _typeCache = new ConventionTypeCache<IMessageTypeInitializerConvention<TMessage>>(cacheFactory, convention);
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
    }


    /// <summary>
    /// Looks for a property that can be used as a CorrelationId message header, and
    /// applies a filter to set it on message send if available
    /// </summary>
    public abstract class InitializerConvention :
        IInitializerConvention
    {
        readonly IConventionTypeCache<IMessageTypeInitializerConvention> _typeCache;

        protected InitializerConvention(IConventionTypeCacheFactory<IMessageTypeInitializerConvention> cacheFactory)
        {
            _typeCache = new ConventionTypeCache<IMessageTypeInitializerConvention>(cacheFactory, this);
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
    }
}

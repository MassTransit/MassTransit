namespace MassTransit.Initializers.Conventions
{
    public class MessageInitializerConvention<TMessage> :
        IMessageInitializerConvention<TMessage>
        where TMessage : class
    {
        readonly IConventionTypeCache<IMessageTypeInitializerConvention<TMessage>> _typeCache;

        public MessageInitializerConvention(IConventionTypeCacheFactory<IMessageTypeInitializerConvention<TMessage>> cacheFactory)
        {
            _typeCache = new ConventionTypeCache<IMessageTypeInitializerConvention<TMessage>>(cacheFactory);
        }

        public bool TryGetMessagePropertyInitializer<TInput, TProperty>(string propertyName, out IMessagePropertyInitializer<TMessage, TInput> initializer)
            where TInput : class
        {
            return _typeCache.GetOrAdd<TInput, IMessageInitializerConvention<TMessage, TInput>>()
                .TryGetMessagePropertyInitializer<TProperty>(propertyName, out initializer);
        }
    }
}
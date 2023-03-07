namespace MassTransit.Configuration
{
    using System;
    using DependencyInjection;
    using Internals;
    using Middleware;
    using Serialization;


    public class ScopedFilterSpecificationObserver :
        ISendPipeSpecificationObserver,
        IPublishPipeSpecificationObserver
    {
        readonly Type _filterType;
        readonly CompositeFilter<Type> _messageTypeFilter;
        readonly IServiceProvider _provider;

        public ScopedFilterSpecificationObserver(Type filterType, IServiceProvider provider, CompositeFilter<Type> messageTypeFilter)
        {
            _filterType = filterType;
            _provider = provider;
            _messageTypeFilter = messageTypeFilter;
            _messageTypeFilter.Excludes += type => type.HasInterface<Fault>();
            _messageTypeFilter.Excludes += type => type.HasInterface<ReceiveFault>();
            // do not create filters for scheduled/outbox messages
            _messageTypeFilter.Excludes += type => type == typeof(SerializedMessageBody);
        }

        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            AddScopedFilter<PublishContext<T>, T>(specification);
        }

        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class
        {
            AddScopedFilter<SendContext<T>, T>(specification);
        }

        void AddScopedFilter<TContext, T>(IPipeConfigurator<TContext> configurator)
            where TContext : class, PipeContext
            where T : class
        {
            if (!_messageTypeFilter.Matches(typeof(T)))
                return;

            var filterType = _filterType.MakeGenericType(typeof(T));

            if (!filterType.HasInterface(typeof(IFilter<TContext>)))
                throw new ConfigurationException($"The scoped filter must implement {TypeCache<IFilter<TContext>>.ShortName} ");

            var scopeProviderType = typeof(FilterScopeProvider<,>).MakeGenericType(filterType, typeof(TContext));

            var scopeProvider = (IFilterScopeProvider<TContext>)Activator.CreateInstance(scopeProviderType, _provider);

            var filter = new ScopedFilter<TContext>(scopeProvider);
            var specification = new FilterPipeSpecification<TContext>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

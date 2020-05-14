namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using GreenPipes;
    using PublishPipeSpecifications;
    using ScopeProviders;
    using Scoping.Filters;


    public class ScopedPublishPipeSpecificationObserver :
        IPublishPipeSpecificationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _serviceProvider;

        public ScopedPublishPipeSpecificationObserver(Type filterType, IServiceProvider serviceProvider)
        {
            _filterType = filterType;
            _serviceProvider = serviceProvider;
        }

        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            var scopeProviderType =
                typeof(DependencyInjectionFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(T)), typeof(PublishContext<T>));
            var scopeProvider = (IFilterContextScopeProvider<PublishContext<T>>)Activator.CreateInstance(scopeProviderType, _serviceProvider);
            var filter = new ScopedFilter<PublishContext<T>>(scopeProvider);

            specification.UseFilter(filter);
        }
    }
}

namespace MassTransit.AutofacIntegration.Filters
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
        readonly ILifetimeScopeProvider _lifetimeScopeProvider;

        public ScopedPublishPipeSpecificationObserver(Type filterType, ILifetimeScopeProvider lifetimeScopeProvider)
        {
            _filterType = filterType;
            _lifetimeScopeProvider = lifetimeScopeProvider;
        }

        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            var scopeProviderType =
                typeof(AutofacFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(T)), typeof(PublishContext<T>));
            var scopeProvider = (IFilterContextScopeProvider<PublishContext<T>>)Activator.CreateInstance(scopeProviderType, _lifetimeScopeProvider);
            var filter = new ScopedFilter<PublishContext<T>>(scopeProvider);

            specification.UseFilter(filter);
        }
    }
}

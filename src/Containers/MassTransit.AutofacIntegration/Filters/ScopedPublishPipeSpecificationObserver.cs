namespace MassTransit.AutofacIntegration.Filters
{
    using System;
    using PublishPipeSpecifications;
    using ScopeProviders;


    public class ScopedPublishPipeSpecificationObserver :
        IPublishPipeSpecificationObserver
    {
        readonly Type _filterType;
        readonly ILifetimeScopeProvider _provider;

        public ScopedPublishPipeSpecificationObserver(Type filterType, ILifetimeScopeProvider provider)
        {
            _filterType = filterType;
            _provider = provider;
        }

        public void MessageSpecificationCreated<T>(IMessagePublishPipeSpecification<T> specification)
            where T : class
        {
            specification.AddScopedFilter<PublishContext<T>, T>(_filterType, _provider);
        }
    }
}

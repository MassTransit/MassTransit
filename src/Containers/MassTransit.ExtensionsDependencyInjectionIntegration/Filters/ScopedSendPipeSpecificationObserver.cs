namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using GreenPipes;
    using ScopeProviders;
    using Scoping.Filters;
    using SendPipeSpecifications;


    public class ScopedSendPipeSpecificationObserver :
        ISendPipeSpecificationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _serviceProvider;

        public ScopedSendPipeSpecificationObserver(Type filterType, IServiceProvider serviceProvider)
        {
            _filterType = filterType;
            _serviceProvider = serviceProvider;
        }

        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class
        {
            var scopeProviderType =
                typeof(DependencyInjectionFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(T)), typeof(SendContext<T>));
            var scopeProvider = (IFilterContextScopeProvider<SendContext<T>>)Activator.CreateInstance(scopeProviderType, _serviceProvider);
            var filter = new ScopedFilter<SendContext<T>>(scopeProvider);

            specification.UseFilter(filter);
        }
    }
}

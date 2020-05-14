namespace MassTransit.AutofacIntegration.Filters
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
        readonly ILifetimeScopeProvider _lifetimeScopeProvider;

        public ScopedSendPipeSpecificationObserver(Type filterType, ILifetimeScopeProvider lifetimeScopeProvider)
        {
            _filterType = filterType;
            _lifetimeScopeProvider = lifetimeScopeProvider;
        }

        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class
        {
            var scopeProviderType =
                typeof(AutofacFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(T)), typeof(SendContext<T>));
            var scopeProvider = (IFilterContextScopeProvider<SendContext<T>>)Activator.CreateInstance(scopeProviderType, _lifetimeScopeProvider);
            var filter = new ScopedFilter<SendContext<T>>(scopeProvider);

            specification.UseFilter(filter);
        }
    }
}

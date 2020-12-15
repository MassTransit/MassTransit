namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using ScopeProviders;
    using SendPipeSpecifications;


    public class ScopedSendPipeSpecificationObserver :
        ISendPipeSpecificationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _provider;

        public ScopedSendPipeSpecificationObserver(Type filterType, IServiceProvider provider)
        {
            _filterType = filterType;
            _provider = provider;
        }

        public void MessageSpecificationCreated<T>(IMessageSendPipeSpecification<T> specification)
            where T : class
        {
            specification.AddScopedFilter<SendContext<T>, T>(_filterType, _provider);
        }
    }
}

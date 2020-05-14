namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes.Specifications;
    using PipeConfigurators;
    using ScopeProviders;
    using Scoping.Filters;


    public class ScopedConsumePipeSpecificationObserver :
        ConfigurationObserver,
        IMessageConfigurationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _serviceProvider;

        public ScopedConsumePipeSpecificationObserver(IConsumePipeConfigurator configurator, Type filterType, IServiceProvider serviceProvider)
            : base(configurator)
        {
            _filterType = filterType;
            _serviceProvider = serviceProvider;

            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var scopeProviderType =
                typeof(DependencyInjectionFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(TMessage)),
                    typeof(ConsumeContext<TMessage>));
            var scopeProvider = (IFilterContextScopeProvider<ConsumeContext<TMessage>>)Activator.CreateInstance(scopeProviderType, _serviceProvider);
            var filter = new ScopedFilter<ConsumeContext<TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

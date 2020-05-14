namespace MassTransit.AutofacIntegration.Filters
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
        readonly ILifetimeScopeProvider _lifetimeScopeProvider;

        public ScopedConsumePipeSpecificationObserver(IConsumePipeConfigurator configurator, Type filterType, ILifetimeScopeProvider lifetimeScopeProvider)
            : base(configurator)
        {
            _filterType = filterType;
            _lifetimeScopeProvider = lifetimeScopeProvider;
            Connect(this);
        }

        public void MessageConfigured<TMessage>(IConsumePipeConfigurator configurator)
            where TMessage : class
        {
            var scopeProviderType =
                typeof(AutofacFilterContextScopeProvider<,>).MakeGenericType(_filterType.MakeGenericType(typeof(TMessage)),
                    typeof(ConsumeContext<TMessage>));
            var scopeProvider = (IFilterContextScopeProvider<ConsumeContext<TMessage>>)Activator.CreateInstance(scopeProviderType, _lifetimeScopeProvider);
            var filter = new ScopedFilter<ConsumeContext<TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumeContext<TMessage>>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

namespace MassTransit.ExtensionsDependencyInjectionIntegration.Filters
{
    using System;
    using ConsumeConfigurators;
    using GreenPipes.Specifications;
    using ScopeProviders;
    using Scoping.Filters;


    public class ScopedConsumerConsumePipeSpecificationObserver :
        IConsumerConfigurationObserver
    {
        readonly Type _filterType;
        readonly IServiceProvider _serviceProvider;

        public ScopedConsumerConsumePipeSpecificationObserver(Type filterType, IServiceProvider serviceProvider)
        {
            _filterType = filterType;
            _serviceProvider = serviceProvider;
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            var scopeProviderType = typeof(DependencyInjectionConsumeFilterContextScopeProvider<,,>)
                .MakeGenericType(_filterType.MakeGenericType(typeof(TMessage)), typeof(ConsumerConsumeContext<TConsumer, TMessage>), typeof(TMessage));

            var scopeProvider = (IFilterContextScopeProvider<ConsumerConsumeContext<TConsumer, TMessage>>)Activator.CreateInstance(scopeProviderType,
                _serviceProvider);

            var filter = new ScopedFilter<ConsumerConsumeContext<TConsumer, TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

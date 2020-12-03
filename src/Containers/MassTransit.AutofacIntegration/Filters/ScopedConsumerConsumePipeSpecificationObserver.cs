namespace MassTransit.AutofacIntegration.Filters
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
        readonly ILifetimeScopeProvider _lifetimeScopeProvider;

        public ScopedConsumerConsumePipeSpecificationObserver(Type filterType, ILifetimeScopeProvider lifetimeScopeProvider)
        {
            _filterType = filterType;
            _lifetimeScopeProvider = lifetimeScopeProvider;
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            var scopeProviderType = typeof(AutofacConsumeFilterContextScopeProvider<,,>)
                .MakeGenericType(_filterType.MakeGenericType(typeof(TMessage)), typeof(ConsumerConsumeContext<TConsumer, TMessage>), typeof(TMessage));

            var scopeProvider = (IFilterContextScopeProvider<ConsumerConsumeContext<TConsumer, TMessage>>)Activator.CreateInstance(scopeProviderType,
                _lifetimeScopeProvider);

            var filter = new ScopedFilter<ConsumerConsumeContext<TConsumer, TMessage>>(scopeProvider);
            var specification = new FilterPipeSpecification<ConsumerConsumeContext<TConsumer, TMessage>>(filter);

            configurator.AddPipeSpecification(specification);
        }
    }
}

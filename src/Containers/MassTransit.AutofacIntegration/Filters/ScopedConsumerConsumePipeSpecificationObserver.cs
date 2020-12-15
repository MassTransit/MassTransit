namespace MassTransit.AutofacIntegration.Filters
{
    using System;
    using ConsumeConfigurators;
    using ScopeProviders;


    public class ScopedConsumerConsumePipeSpecificationObserver :
        IConsumerConfigurationObserver
    {
        readonly Type _filterType;
        readonly ILifetimeScopeProvider _provider;

        public ScopedConsumerConsumePipeSpecificationObserver(Type filterType, ILifetimeScopeProvider provider)
        {
            _filterType = filterType;
            _provider = provider;
        }

        public void ConsumerConfigured<TConsumer>(IConsumerConfigurator<TConsumer> configurator)
            where TConsumer : class
        {
        }

        public void ConsumerMessageConfigured<TConsumer, TMessage>(IConsumerMessageConfigurator<TConsumer, TMessage> configurator)
            where TConsumer : class
            where TMessage : class
        {
            configurator.AddScopedFilter<ConsumerConsumeContext<TConsumer, TMessage>, ConsumeContext<TMessage>, TMessage>(_filterType, _provider);
        }
    }
}

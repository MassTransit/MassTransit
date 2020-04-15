namespace MassTransit.Registration
{
    using System;
    using System.Collections.Generic;
    using ConsumeConfigurators;
    using Context;
    using Definition;
    using Metadata;
    using Scoping;


    /// <summary>
    /// A consumer registration represents a single consumer, which will be resolved from the container using the scope
    /// provider. The consumer definition, if present, is loaded from the container and used to configure the consumer
    /// within the receive endpoint.
    /// </summary>
    /// <typeparam name="TConsumer">The consumer type</typeparam>
    public class ConsumerRegistration<TConsumer> :
        IConsumerRegistration
        where TConsumer : class, IConsumer
    {
        readonly List<Action<IConsumerConfigurator<TConsumer>>> _configureActions;
        IConsumerDefinition<TConsumer> _definition;

        public ConsumerRegistration()
        {
            _configureActions = new List<Action<IConsumerConfigurator<TConsumer>>>();
        }

        void IConsumerRegistration.AddConfigureAction<T>(Action<IConsumerConfigurator<T>> configure)
        {
            if (configure is Action<IConsumerConfigurator<TConsumer>> action)
                _configureActions.Add(action);
        }

        void IConsumerRegistration.Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var scopeProvider = configurationServiceProvider.GetRequiredService<IConsumerScopeProvider>();
            var consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);
            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            LogContext.Debug?.Log("Configuring endpoint {Endpoint}, Consumer: {ConsumerType}", configurator.InputAddress.GetLastPart(),
                TypeMetadataCache<TConsumer>.ShortName);

            GetConsumerDefinition(configurationServiceProvider)
                .Configure(configurator, consumerConfigurator);

            foreach (Action<IConsumerConfigurator<TConsumer>> action in _configureActions)
                action(consumerConfigurator);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        IConsumerDefinition IConsumerRegistration.GetDefinition(IConfigurationServiceProvider provider)
        {
            return GetConsumerDefinition(provider);
        }

        IConsumerDefinition<TConsumer> GetConsumerDefinition(IConfigurationServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = provider.GetService<IConsumerDefinition<TConsumer>>() ?? new DefaultConsumerDefinition<TConsumer>();

            var endpointDefinition = provider.GetService<IEndpointDefinition<TConsumer>>();
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}

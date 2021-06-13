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
        IConsumerRegistration<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly Func<IEnumerable<IConsumerConfiguratorAction<TConsumer>>> _actionsProvider;
        readonly List<Action<IConsumerConfigurator<TConsumer>>> _configureActions;
        IConsumerDefinition<TConsumer> _definition;

        public ConsumerRegistration(Func<IEnumerable<IConsumerConfiguratorAction<TConsumer>>> actionsProvider)
        {
            _configureActions = new List<Action<IConsumerConfigurator<TConsumer>>>();
            _actionsProvider = actionsProvider;
        }

        public void AddConfigureAction(Action<IConsumerConfigurator<TConsumer>> configure)
        {
            if (configure != null)
                _configureActions.Add(configure);
        }

        public Type ConsumerType => typeof(TConsumer);

        void IConsumerRegistration.Configure(IReceiveEndpointConfigurator configurator, IConfigurationServiceProvider configurationServiceProvider)
        {
            var scopeProvider = configurationServiceProvider.GetRequiredService<IConsumerScopeProvider>();
            IConsumerFactory<TConsumer> consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);

            var decoratorRegistration = configurationServiceProvider.GetService<IConsumerFactoryDecoratorRegistration<TConsumer>>();
            if (decoratorRegistration != null)
                consumerFactory = decoratorRegistration.DecorateConsumerFactory(consumerFactory);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            GetConsumerDefinition(configurationServiceProvider)
                .Configure(configurator, consumerConfigurator);

            foreach (IConsumerConfiguratorAction<TConsumer> action in _actionsProvider())
                action.Configure(consumerConfigurator);

            foreach (Action<IConsumerConfigurator<TConsumer>> action in _configureActions)
                action(consumerConfigurator);

            var endpointName = configurator.InputAddress.GetLastPart();

            foreach (var configureReceiveEndpoint in consumerConfigurator.SelectOptions<IConfigureReceiveEndpoint>())
                configureReceiveEndpoint.Configure(endpointName, configurator);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Consumer: {ConsumerType}", endpointName, TypeMetadataCache<TConsumer>.ShortName);

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

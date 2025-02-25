namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using Internals;
    using Microsoft.Extensions.DependencyInjection;
    using Transports;


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
        readonly List<Action<IRegistrationContext, IConsumerConfigurator<TConsumer>>> _configureActions;
        readonly IContainerSelector _selector;
        IConsumerDefinition<TConsumer> _definition;

        public ConsumerRegistration(IContainerSelector selector)
        {
            _selector = selector;
            _configureActions = new List<Action<IRegistrationContext, IConsumerConfigurator<TConsumer>>>();
            IncludeInConfigureEndpoints = !Type.HasAttribute<ExcludeFromConfigureEndpointsAttribute>();
        }

        public Type Type => typeof(TConsumer);

        public bool IncludeInConfigureEndpoints { get; set; }

        void IConsumerRegistration.AddConfigureAction<T>(Action<IRegistrationContext, IConsumerConfigurator<T>> configure)
        {
            if (configure is Action<IRegistrationContext, IConsumerConfigurator<TConsumer>> action)
                _configureActions.Add(action);
        }

        void IConsumerRegistration.Configure(IReceiveEndpointConfigurator configurator, IRegistrationContext context)
        {
            IConsumeScopeProvider scopeProvider = new ConsumeScopeProvider(context);
            IConsumerFactory<TConsumer> consumerFactory = new ScopeConsumerFactory<TConsumer>(scopeProvider);

            var decoratorRegistration = context.GetService<IConsumerFactoryDecoratorRegistration<TConsumer>>();
            if (decoratorRegistration != null)
                consumerFactory = decoratorRegistration.DecorateConsumerFactory(consumerFactory);

            var consumerConfigurator = new ConsumerConfigurator<TConsumer>(consumerFactory, configurator);

            GetConsumerDefinition(context)
                .Configure(configurator, consumerConfigurator, context);

            foreach (Action<IRegistrationContext, IConsumerConfigurator<TConsumer>> action in _configureActions)
                action(context, consumerConfigurator);

            var endpointName = configurator.InputAddress.GetEndpointName();

            foreach (var configureReceiveEndpoint in consumerConfigurator.SelectOptions<IConfigureReceiveEndpoint>())
                configureReceiveEndpoint.Configure(endpointName, configurator);

            LogContext.Info?.Log("Configured endpoint {Endpoint}, Consumer: {ConsumerType}", endpointName, TypeCache<TConsumer>.ShortName);

            configurator.AddEndpointSpecification(consumerConfigurator);
        }

        IConsumerDefinition IConsumerRegistration.GetDefinition(IRegistrationContext context)
        {
            return GetConsumerDefinition(context);
        }

        public IConsumerRegistrationConfigurator GetConsumerRegistrationConfigurator(IRegistrationConfigurator registrationConfigurator)
        {
            return new ConsumerRegistrationConfigurator<TConsumer>(registrationConfigurator, this);
        }

        IConsumerDefinition<TConsumer> GetConsumerDefinition(IServiceProvider provider)
        {
            if (_definition != null)
                return _definition;

            _definition = _selector.GetDefinition<IConsumerDefinition<TConsumer>>(provider) ?? new DefaultConsumerDefinition<TConsumer>();

            IEndpointDefinition<TConsumer> endpointDefinition = _selector.GetEndpointDefinition<TConsumer>(provider);
            if (endpointDefinition != null)
                _definition.EndpointDefinition = endpointDefinition;

            return _definition;
        }
    }
}

namespace MassTransit.DependencyInjection.Registration
{
    using System;
    using Configuration;


    public class ConsumerRegistrationConfigurator<TConsumer> :
        IConsumerRegistrationConfigurator<TConsumer>
        where TConsumer : class, IConsumer
    {
        readonly IRegistrationConfigurator _configurator;
        readonly IConsumerRegistration _registration;

        public ConsumerRegistrationConfigurator(IRegistrationConfigurator configurator, IConsumerRegistration registration)
        {
            _configurator = configurator;
            _registration = registration;
        }

        public void Endpoint(Action<IEndpointRegistrationConfigurator> configure)
        {
            if (!_registration.IncludeInConfigureEndpoints)
                throw new ConfigurationException("Consumer is excluded from ConfigureEndpoints");

            var configurator = new EndpointRegistrationConfigurator<TConsumer>();

            configure?.Invoke(configurator);

            _configurator.AddEndpoint<ConsumerEndpointDefinition<TConsumer>, TConsumer>(_registration, configurator.Settings);
        }

        public void ExcludeFromConfigureEndpoints()
        {
            _registration.IncludeInConfigureEndpoints = false;
        }
    }
}
